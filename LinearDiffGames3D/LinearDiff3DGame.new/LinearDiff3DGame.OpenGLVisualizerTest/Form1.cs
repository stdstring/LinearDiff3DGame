//#define ViewerOnly
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.OpenGLVisualizerTest.BridgeController;
using LinearDiff3DGame.OpenGLVisualizerTest.Objects3D;
using LinearDiff3DGame.OpenGLVisualizerTest.VisualisationHelpers;
using OpenGLControlTest;

namespace LinearDiff3DGame.OpenGLVisualizerTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

#if ViewerOnly
            MainMenuStrip = mainViewerMenu;
            mainMenu.Visible = false;
#else
            MainMenuStrip = mainMenu;
            mainViewerMenu.Visible = false;
#endif
            sideVisualisation = new TriangleSideVisualization();
            //sideVisualisation = new PolygonSideVisualization();
            visualisationManager = new BridgeVisualisationManager(sideVisualisation);
            viewPointManager = new ViewPointManager();
            keyActions = new Dictionary<Keys, Action>();
            InitKeyActions();
            Clear();
        }

        private void InitKeyActions()
        {
            keyActions.Add(Keys.S, () => viewPointManager.RotateOX(-1));
            keyActions.Add(Keys.W, () => viewPointManager.RotateOX(1));
            keyActions.Add(Keys.A, () => viewPointManager.RotateOY(-1));
            keyActions.Add(Keys.D, () => viewPointManager.RotateOY(1));
            keyActions.Add(Keys.C, () => viewPointManager.MoveOZ(-1));
            keyActions.Add(Keys.Z, () => viewPointManager.MoveOZ(1));
        }

        private void Clear()
        {
            bridge = emptyBridge;
            visualisationManager.ClearVisualisation();
            viewPointManager.ResetViewPoint();
            lblCurrentTime.Text = "";
            tbTimeScale.Enabled = false;
        }

        private void CalculateAndPrepareBridge(IBridgeControllerAsync controller, Int32 sectionCount)
        {
            Clear();
            using(CalculationProgressForm calculationProgressForm = new CalculationProgressForm(sectionCount))
            {
                calculationProgressForm.OnCalculationCancel += (sender, e) => controller.Cancel();
                calculationProgressForm.OnCalculationStart += (sender, e) => controller.Run();
                controller.OnBridgeCompleted += calculationProgressForm.BridgeCalculated;
                controller.OnSectionCompleted += calculationProgressForm.SectionCalculated;
                if(calculationProgressForm.ShowDialog(this) == DialogResult.OK)
                {
                    bridge = controller.Bridge;
                    visualisationManager.CreateVisualisation(bridge);
                    tbTimeScale.Enabled = true;
                    tbTimeScale.Minimum = 0;
                    tbTimeScale.Maximum = bridge.Count - 1;
                    lblCurrentTime.Text = String.Format(timeTemplate, 0);
                    canvas.Invalidate();
                }
            }
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            Color backColor = canvas.BackColor;
            OpenGLControl.glClearColor(backColor.R / 255.0f,
                                      backColor.G / 255.0f,
                                      backColor.B / 255.0f,
                                      0f);
            // ???
            OpenGLControl.glClearDepth(0.0);
            OpenGLControl.glClear(OpenGLControl.GL_COLOR_BUFFER_BIT | OpenGLControl.GL_DEPTH_BUFFER_BIT);
            if(visualisationManager.IsEmpty) return;
            OpenGLControl.glMatrixMode(OpenGLControl.GL_MODELVIEW);
            OpenGLControl.glPushMatrix();
            viewPointManager.ApplyViewPoint();
            visualisationManager.ApplyVisualisation(tbTimeScale.Value, polyhedronColor, contourColor, contourWidth);
            OpenGLControl.glPopMatrix();
        }

        private void calculateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(CalculateParamsDialog calculateParamsDialog = new CalculateParamsDialog())
            {
                calculateParamsDialog.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                if(calculateParamsDialog.ShowDialog(this) == DialogResult.OK)
                {
                    IBridgeCalcAsync bridgeCalc = new BridgeCalcAsync();
                    Int32 count = bridgeCalc.PrepareCalculation(calculateParamsDialog.InputDataFile, calculateParamsDialog.FinishTime);
                    CalculateAndPrepareBridge(bridgeCalc, count);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            visualisationManager.Dispose();
            Close();
        }

        private void tbTimeScale_Scroll(object sender, EventArgs e)
        {
            if(tbTimeScale.Value >= bridge.Count) return;
            Pair<Double, Polyhedron> currentSection = bridge[tbTimeScale.Value];
            lblCurrentTime.Text = String.Format(timeTemplate, currentSection.Item1.ToString());
            canvas.Invalidate();
        }

        // TODO : заставить работать
        //private void colorsToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    using(ColorChooseDialog colorChooseDialog = new ColorChooseDialog(polyhedronColor, contourColor, sideVisualisation))
        //    {
        //        if(colorChooseDialog.ShowDialog(this) == DialogResult.OK)
        //        {
        //            polyhedronColor = colorChooseDialog.BodyColor;
        //            contourColor = colorChooseDialog.ContourColor;}
        //    }
        //    //canvas.Invalidate();
        //}

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                if(openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    using(FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        IBridgeLoadAsync bridgeLoad = new BridgeLoadAsync();
                        Int32 count = bridgeLoad.Prepare(fs);
                        CalculateAndPrepareBridge(bridgeLoad, count);
                    }
                }
            }
        }

        private void canvas_KeyDown(object sender, KeyEventArgs e)
        {
            Action action;
            if(keyActions.TryGetValue(e.KeyData, out action))
            {
                action();
                canvas.Invalidate();
            }
        }

        private readonly IPolyhedronSideVisualisation sideVisualisation;
        private readonly Color polyhedronColor = Color.Gold;
        private readonly Color contourColor = Color.Black;
        private readonly BridgeVisualisationManager visualisationManager;
        private readonly ViewPointManager viewPointManager;
        private readonly IDictionary<Keys, Action> keyActions;
        private IList<Pair<Double, Polyhedron>> bridge;

        private readonly IList<Pair<Double, Polyhedron>> emptyBridge =
            new ReadOnlyCollection<Pair<Double, Polyhedron>>(new List<Pair<Double, Polyhedron>>());

        private const float contourWidth = 2.0f;
        private const String timeTemplate = "T = {0}";
    }
}