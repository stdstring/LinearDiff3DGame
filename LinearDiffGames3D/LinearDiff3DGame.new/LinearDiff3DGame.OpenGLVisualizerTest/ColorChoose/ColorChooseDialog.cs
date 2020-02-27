using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.OpenGLVisualizerTest.Objects3D;
using LinearDiff3DGame.OpenGLVisualizerTest.VisualisationHelpers;
using OpenGLControlTest;

namespace LinearDiff3DGame.OpenGLVisualizerTest.ColorChoose
{
    internal partial class ColorChooseDialog : Form
    {
        public ColorChooseDialog(Color bodyColor, Color contourColor, IPolyhedronSideVisualisation sideVisualisation)
        {
            InitializeComponent();
            rgbBodyColor.Color = BodyColor = bodyColor;
            rgbContourColor.Color = ContourColor = contourColor;
            Polyhedron samplePolyhedron = new ChooseColorController().GetSamplePolyhedron();
            IList<Pair<Double, Polyhedron>> psevdoBridge = new List<Pair<Double, Polyhedron>>
                                                               {
                                                                   new Pair<Double, Polyhedron>(0, samplePolyhedron)
                                                               };
            visualisationManager = new BridgeVisualisationManager(sideVisualisation);
            visualisationManager.CreateVisualisation(psevdoBridge);
            viewPointManager = new ViewPointManager(45.0, 45.0, 6, 0, 0, 0);
        }

        public Color BodyColor { get; private set; }
        public Color ContourColor { get; private set; }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            Color backColor = Color.Black; //canvas.BackColor;
            OpenGLControl.glClearColor(backColor.R / 255.0f,
                                      backColor.G / 255.0f,
                                      backColor.B / 255.0f,
                                      backColor.A / 255.0f);
            OpenGLControl.glClearDepth(1.0);
            OpenGLControl.glClear(OpenGLControl.GL_COLOR_BUFFER_BIT | OpenGLControl.GL_DEPTH_BUFFER_BIT);
            if(visualisationManager.IsEmpty) return;
            OpenGLControl.glMatrixMode(OpenGLControl.GL_MODELVIEW);
            OpenGLControl.glPushMatrix();
            viewPointManager.ApplyViewPoint();
            visualisationManager.ApplyVisualisation(0, BodyColor, ContourColor, contourWidth);
            OpenGLControl.glPopMatrix();
        }

        private Boolean TryApplyColors()
        {
            Color bodyColor = rgbBodyColor.Color;
            Color contourColor = rgbContourColor.Color;
            if(bodyColor != Color.Empty && contourColor != Color.Empty)
            {
                BodyColor = bodyColor;
                ContourColor = contourColor;
                return true;
            }
            return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(TryApplyColors()) DialogResult = DialogResult.OK;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            //if(TryApplyColors()) canvas.Invalidate();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private readonly BridgeVisualisationManager visualisationManager;
        private readonly ViewPointManager viewPointManager;
        private const Single contourWidth = 2.0f;
    }
}