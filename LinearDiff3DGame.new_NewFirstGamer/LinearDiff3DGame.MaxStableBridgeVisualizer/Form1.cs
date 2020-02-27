using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LinearDiff3DGame.MaxStableBridgeVisualizer.Space3D;
using OpenGLPaintControl;

namespace LinearDiff3DGame.MaxStableBridgeVisualizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            m_Controller = new Controller();
            m_VisualizationData = new OpenGLVisualizationData();
            m_TimeMarkers = new List<Double>();
            m_CurrentTimeMarker = -1;
            // may be from config
            // colors
            m_VisualizationData.BackColor = Color.FromArgb(255, 255, 255, 255);
            m_VisualizationData.ObjectColor = Color.FromArgb(255, 102, 153, 153);
            m_VisualizationData.ContourColor = Color.FromArgb(255, 0, 0, 0);
            // visualization parameters
            m_VisualizationData.ContourLineWidth = 2.0f;
            // viewer
            m_VisualizationData.AngleOX = 0;
            m_VisualizationData.AngleOY = 0;
            m_VisualizationData.DistanseOZ = 20;
            m_VisualizationData.DeltaAngleOX = 5;
            m_VisualizationData.DeltaAngleOY = 5;
            m_VisualizationData.DeltaDistanseOZ = 1;
        }

        private void Prepare4Visualisation(IList<MaxStableBridgeSection> sectionList)
        {
            if (m_VisualizationData.ListBase != 0)
                OpenGLWrapper.glDeleteLists(m_VisualizationData.ListBase, m_VisualizationData.ListCount);

            Canvas.ActivateContext();
            try
            {
                m_TimeMarkers.Clear();

                m_VisualizationData.ListBase = OpenGLWrapper.glGenLists(sectionList.Count);
                m_VisualizationData.ListCount = sectionList.Count;

                for (UInt32 sectionIndex = 0; sectionIndex < sectionList.Count; ++sectionIndex)
                {
                    Double t = sectionList[(Int32)sectionIndex].T;
                    Polyhedron polyhedron = sectionList[(Int32)sectionIndex].Polyhedron;

                    m_TimeMarkers.Add(t);
                    OpenGLWrapper.glNewList(m_VisualizationData.ListBase + sectionIndex, OpenGLWrapper.GL_COMPILE);
                    CreateVisualization4Polyhedron(polyhedron);
                    OpenGLWrapper.glEndList();
                }
            }
            finally
            {
                Canvas.DeactivateContext();
            }

            m_CurrentTimeMarker = 0;
        }

        // ReSharper disable MemberCanBeMadeStatic
        private void CreateVisualization4Polyhedron(Polyhedron polyhedron)
        // ReSharper restore MemberCanBeMadeStatic
        {
            for (Int32 sideIndex = 0; sideIndex < polyhedron.SideList.Count; ++sideIndex)
            {
                PolyhedronSide side = polyhedron.SideList[sideIndex];
                for (Int32 vertexIndex = 2; vertexIndex < side.VertexList.Count; ++vertexIndex)
                {
                    OpenGLWrapper.glBegin(OpenGLWrapper.GL_TRIANGLES);
                    OpenGLWrapper.glNormal3d(side.Normal.X, side.Normal.Y, side.Normal.Z);
                    OpenGLWrapper.glEdgeFlag(vertexIndex == 2 ? (Byte)OpenGLWrapper.GL_TRUE : (Byte)OpenGLWrapper.GL_FALSE);
                    OpenGLWrapper.glVertex3d(side.VertexList[0].X,
                                             side.VertexList[0].Y,
                                             side.VertexList[0].Z);
                    OpenGLWrapper.glEdgeFlag((Byte)OpenGLWrapper.GL_TRUE);
                    OpenGLWrapper.glVertex3d(side.VertexList[vertexIndex - 1].X,
                                             side.VertexList[vertexIndex - 1].Y,
                                             side.VertexList[vertexIndex - 1].Z);
                    OpenGLWrapper.glEdgeFlag(vertexIndex == side.VertexList.Count - 1 ? (Byte)OpenGLWrapper.GL_TRUE : (Byte)OpenGLWrapper.GL_FALSE);
                    OpenGLWrapper.glVertex3d(side.VertexList[vertexIndex].X,
                                             side.VertexList[vertexIndex].Y,
                                             side.VertexList[vertexIndex].Z);
                    OpenGLWrapper.glEnd();
                }
            }
        }

        private void RefreshTimeMarker()
        {
            m_CurrentTimeMarker = tbTimeLine.Value;
            Double timeValue = Math.Round(m_TimeMarkers[m_CurrentTimeMarker], 4);
            lblCurrentTime.Text = String.Format("t = {0}", timeValue);
        }

        private readonly Controller m_Controller;
        private readonly OpenGLVisualizationData m_VisualizationData;
        private readonly IList<Double> m_TimeMarkers;
        private Int32 m_CurrentTimeMarker;

        private class OpenGLVisualizationData
        {
            public UInt32 ListBase
            {
                get;
                set;
            }

            public Int32 ListCount
            {
                get;
                set;
            }

            public Color BackColor
            {
                get;
                set;
            }

            public Color ObjectColor
            {
                get;
                set;
            }

            public Color ContourColor
            {
                get;
                set;
            }

            public Single ContourLineWidth
            {
                get;
                set;
            }

            public Double AngleOX
            {
                get;
                set;
            }

            public Double AngleOY
            {
                get;
                set;
            }

            public Double DistanseOZ
            {
                get;
                set;
            }

            public Double DeltaAngleOX
            {
                get;
                set;
            }

            public Double DeltaAngleOY
            {
                get;
                set;
            }

            public Double DeltaDistanseOZ
            {
                get;
                set;
            }
        }

        private void miCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                IList<MaxStableBridgeSection> sectionList = m_Controller.CalculateSectionList(1.0);
                Prepare4Visualisation(sectionList);
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            Canvas.Invalidate();
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Color backColor = m_VisualizationData.BackColor;
            OpenGLWrapper.glClearColor(backColor.R / 255.0f,
                                       backColor.G / 255.0f,
                                       backColor.B / 255.0f,
                                       backColor.A / 255.0f);
            OpenGLWrapper.glClear(OpenGLWrapper.GL_COLOR_BUFFER_BIT | OpenGLWrapper.GL_DEPTH_BUFFER_BIT);

            if (m_VisualizationData.ListCount == 0) return;

            OpenGLWrapper.glMatrixMode(OpenGLWrapper.GL_MODELVIEW);

            OpenGLWrapper.glPushMatrix();

            OpenGLWrapper.glTranslated(0.0, 0.0, -m_VisualizationData.DistanseOZ);
            OpenGLWrapper.glRotated(m_VisualizationData.AngleOY, 0.0, 1.0, 0.0);
            OpenGLWrapper.glRotated(m_VisualizationData.AngleOX, 1.0, 0.0, 0.0);

            OpenGLWrapper.glEnable(OpenGLWrapper.GL_LIGHTING);
            OpenGLWrapper.glEnable(OpenGLWrapper.GL_LIGHT0);
            OpenGLWrapper.glEnable(OpenGLWrapper.GL_COLOR_MATERIAL);
            OpenGLWrapper.glLightModeli(OpenGLWrapper.GL_LIGHT_MODEL_TWO_SIDE, (Int32)OpenGLWrapper.GL_TRUE);

            // многогранник
            OpenGLWrapper.glPolygonMode(OpenGLWrapper.GL_FRONT, OpenGLWrapper.GL_FILL);
            OpenGLWrapper.glEnable(OpenGLWrapper.GL_CULL_FACE);
            OpenGLWrapper.glCullFace(OpenGLWrapper.GL_BACK);
            Color objectColor = m_VisualizationData.ObjectColor;
            OpenGLWrapper.glColor4d(objectColor.R / 255.0,
                                    objectColor.G / 255.0,
                                    objectColor.B / 255.0,
                                    objectColor.A / 255.0);
            OpenGLWrapper.glCallList(m_VisualizationData.ListBase + (UInt32)m_CurrentTimeMarker);
            // многогранник

            // контур многогранника
            OpenGLWrapper.glPolygonMode(OpenGLWrapper.GL_FRONT, OpenGLWrapper.GL_LINE);
            OpenGLWrapper.glEnable(OpenGLWrapper.GL_CULL_FACE);
            OpenGLWrapper.glCullFace(OpenGLWrapper.GL_BACK);
            Color contourColor = m_VisualizationData.ContourColor;
            OpenGLWrapper.glColor4d(contourColor.R / 255.0,
                                    contourColor.G / 255.0,
                                    contourColor.B / 255.0,
                                    contourColor.A / 255.0);
            OpenGLWrapper.glLineWidth(m_VisualizationData.ContourLineWidth);
            OpenGLWrapper.glCallList(m_VisualizationData.ListBase + (UInt32)m_CurrentTimeMarker);
            // контур многогранника

            OpenGLWrapper.glPopMatrix();
        }

        private void tbTimeLine_ValueChanged(object sender, EventArgs e)
        {
            RefreshTimeMarker();
            Canvas.Invalidate();
        }

        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                m_VisualizationData.AngleOY -= m_VisualizationData.DeltaAngleOY;
                if (m_VisualizationData.AngleOY < 0)
                    m_VisualizationData.AngleOY += 360;
                Canvas.Invalidate();
            }
            if (e.KeyCode == Keys.D)
            {
                m_VisualizationData.AngleOY += m_VisualizationData.DeltaAngleOY;
                if (m_VisualizationData.AngleOY > 360)
                    m_VisualizationData.AngleOY -= 360;
                Canvas.Invalidate();
            }
            if (e.KeyCode == Keys.W)
            {
                m_VisualizationData.AngleOX -= m_VisualizationData.DeltaAngleOX;
                if (m_VisualizationData.AngleOX < 0)
                    m_VisualizationData.AngleOX += 360;
                Canvas.Invalidate();
            }
            if (e.KeyCode == Keys.S)
            {
                m_VisualizationData.AngleOX += m_VisualizationData.DeltaAngleOX;
                if (m_VisualizationData.AngleOX > 360)
                    m_VisualizationData.AngleOX -= 360;
                Canvas.Invalidate();
            }
            if (e.KeyCode == Keys.Insert)
            {
                //m_VisualizationData.DistanseOZ -= m_VisualizationData.DeltaDistanseOZ
            }
            if (e.KeyCode == Keys.Delete)
            {

            }
        }
    }
}
