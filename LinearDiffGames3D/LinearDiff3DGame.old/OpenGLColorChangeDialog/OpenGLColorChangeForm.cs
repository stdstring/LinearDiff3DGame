using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using PopovYuri.Visualization;

using MathPostgraduateStudy.LinearDiff3DGame;

namespace MathPostgraduateStudy.OpenGLColorChangeDialog
{
    public partial class OpenGLColorChangeForm : Form
    {
        public OpenGLColorChangeForm()
        {
            InitializeComponent();

            m_AngleOX = 45.0;
            m_AngleOY = 45.0;

            m_OpaqueListBase = CreateCubeDisplayList(1.0);
            m_SemiTransparentListBase = CreateCubeDisplayList(2.0);

            // цвета по умолчанию
            m_OCubeColor = Color.FromArgb(255, 102, 153, 153);
            m_OCubeContourColor = Color.FromArgb(255, 0, 0, 0);
            m_STCubeColor = Color.FromArgb(89, 153, 51, 51);
            m_STCubeContourColor = Color.FromArgb(255, 255, 0, 0);
            // цвета по умолчанию

            OCubeColorPB.ColorProperty = m_OCubeColor;
            OCubeContourColorPB.ColorProperty = m_OCubeContourColor;
            STCubeColorPB.ColorProperty = m_STCubeColor;
            STCubeContourColorPB.ColorProperty = m_STCubeContourColor;
        }

        /// <summary>
        /// цвет непрозрачного куба
        /// </summary>
        public Color OCubeColor
        {
            get
            {
                return m_OCubeColor;
            }
            set
            {
                m_OCubeColor = value;
                OCubeColorPB.ColorProperty = m_OCubeColor;
            }
        }

        /// <summary>
        /// цвет контура непрозрачного куба
        /// </summary>
        public Color OCubeContourColor
        {
            get
            {
                return m_OCubeContourColor;
            }
            set
            {
                m_OCubeContourColor = value;
                OCubeContourColorPB.ColorProperty = m_OCubeContourColor;
            }
        }

        /// <summary>
        /// цвет полупрозрачного куба
        /// </summary>
        public Color STCubeColor
        {
            get
            {
                return m_STCubeColor;
            }
            set
            {
                m_STCubeColor = value;
                STCubeColorPB.ColorProperty = m_STCubeColor;
            }
        }

        /// <summary>
        /// цвет контура полупрозрачного куба
        /// </summary>
        public Color STCubeContourColor
        {
            get
            {
                return m_STCubeContourColor;
            }
            set
            {
                m_STCubeContourColor = value;
                STCubeContourColorPB.ColorProperty = m_STCubeContourColor;
            }
        }

        private UInt32 CreateCubeDisplayList(Double scaleKoeff)
        {
            PaintBox.ActivateContext();

            UInt32 displayListsBase = OpenGLControl.glGenLists(1);

            OpenGLControl.glNewList(displayListsBase, OpenGLControl.GL_COMPILE);
            // цикл по всем граням куба
            for (Int32 cubeSideIndex = 0; cubeSideIndex < m_CubeSides.GetLength(0); cubeSideIndex++)
            {
                Vector3D sideNormal = m_CubeSideNormals[cubeSideIndex];

                // грань куба разбиваем на 2 треугольника
                OpenGLControl.glBegin(OpenGLControl.GL_TRIANGLES);
                OpenGLControl.glNormal3d(sideNormal.XCoord, sideNormal.YCoord, sideNormal.ZCoord);
                OpenGLControl.glEdgeFlag((byte)OpenGLControl.GL_TRUE);
                OpenGLControl.glVertex3d(m_CubeSides[cubeSideIndex, 0].XCoord * scaleKoeff,
                                         m_CubeSides[cubeSideIndex, 0].YCoord * scaleKoeff,
                                         m_CubeSides[cubeSideIndex, 0].ZCoord * scaleKoeff);
                OpenGLControl.glEdgeFlag((byte)OpenGLControl.GL_TRUE);
                OpenGLControl.glVertex3d(m_CubeSides[cubeSideIndex, 1].XCoord * scaleKoeff,
                                         m_CubeSides[cubeSideIndex, 1].YCoord * scaleKoeff,
                                         m_CubeSides[cubeSideIndex, 1].ZCoord * scaleKoeff);
                OpenGLControl.glEdgeFlag((byte)OpenGLControl.GL_FALSE);
                OpenGLControl.glVertex3d(m_CubeSides[cubeSideIndex, 2].XCoord * scaleKoeff,
                                         m_CubeSides[cubeSideIndex, 2].YCoord * scaleKoeff,
                                         m_CubeSides[cubeSideIndex, 2].ZCoord * scaleKoeff);
                OpenGLControl.glEnd();

                OpenGLControl.glBegin(OpenGLControl.GL_TRIANGLES);
                OpenGLControl.glNormal3d(sideNormal.XCoord, sideNormal.YCoord, sideNormal.ZCoord);
                OpenGLControl.glEdgeFlag((byte)OpenGLControl.GL_FALSE);
                OpenGLControl.glVertex3d(m_CubeSides[cubeSideIndex, 0].XCoord * scaleKoeff,
                                         m_CubeSides[cubeSideIndex, 0].YCoord * scaleKoeff,
                                         m_CubeSides[cubeSideIndex, 0].ZCoord * scaleKoeff);
                OpenGLControl.glEdgeFlag((byte)OpenGLControl.GL_TRUE);
                OpenGLControl.glVertex3d(m_CubeSides[cubeSideIndex, 2].XCoord * scaleKoeff,
                                         m_CubeSides[cubeSideIndex, 2].YCoord * scaleKoeff,
                                         m_CubeSides[cubeSideIndex, 2].ZCoord * scaleKoeff);
                OpenGLControl.glEdgeFlag((byte)OpenGLControl.GL_TRUE);
                OpenGLControl.glVertex3d(m_CubeSides[cubeSideIndex, 3].XCoord * scaleKoeff,
                                         m_CubeSides[cubeSideIndex, 3].YCoord * scaleKoeff,
                                         m_CubeSides[cubeSideIndex, 3].ZCoord * scaleKoeff);
                OpenGLControl.glEnd();
                // грань куба разбиваем на 2 треугольника
            }
            // цикл по всем граням куба
            OpenGLControl.glEndList();

            PaintBox.DeactivateContext();

            return displayListsBase;
        }

        private void PaintBox_Paint(object sender, PaintEventArgs e)
        {
            OpenGLControl.glClearColor(1.0f, 1.0f, 1.0f, 0.0f);
            OpenGLControl.glClear(OpenGLControl.GL_COLOR_BUFFER_BIT | OpenGLControl.GL_DEPTH_BUFFER_BIT);

            OpenGLControl.glMatrixMode(OpenGLControl.GL_MODELVIEW);

            OpenGLControl.glPushMatrix();

            OpenGLControl.glTranslated(0.0, 0.0, -5.0);

            OpenGLControl.glRotated(m_AngleOY, 0.0, 1.0, 0.0);
            OpenGLControl.glRotated(m_AngleOX, 1.0, 0.0, 0.0);

            OpenGLControl.glEnable(OpenGLControl.GL_LIGHTING);
            OpenGLControl.glEnable(OpenGLControl.GL_LIGHT0);
            OpenGLControl.glEnable(OpenGLControl.GL_COLOR_MATERIAL);
            OpenGLControl.glLightModeli(OpenGLControl.GL_LIGHT_MODEL_TWO_SIDE, (Int32)OpenGLControl.GL_TRUE);

            // выводим непрозрачный куб
            OpenGLControl.glPolygonMode(OpenGLControl.GL_FRONT, OpenGLControl.GL_FILL);
            OpenGLControl.glEnable(OpenGLControl.GL_CULL_FACE);
            OpenGLControl.glCullFace(OpenGLControl.GL_BACK);
            OpenGLControl.glColor4d(m_OCubeColor.R / 255.0, m_OCubeColor.G / 255.0, m_OCubeColor.B / 255.0, m_OCubeColor.A / 255.0);
            OpenGLControl.glCallList(m_OpaqueListBase);
            // выводим непрозрачный куб

            // выводим контур непрозрачного куба
            OpenGLControl.glPolygonMode(OpenGLControl.GL_FRONT_AND_BACK, OpenGLControl.GL_LINE);
            OpenGLControl.glEnable(OpenGLControl.GL_CULL_FACE);
            OpenGLControl.glCullFace(OpenGLControl.GL_BACK);
            OpenGLControl.glColor4d(m_OCubeContourColor.R / 255.0, m_OCubeContourColor.G / 255.0, m_OCubeContourColor.B / 255.0, m_OCubeContourColor.A / 255.0);
            OpenGLControl.glLineWidth(2.0f);
            OpenGLControl.glCallList(m_OpaqueListBase);
            // выводим контур непрозрачного куба

            //OpenGLControl.glEnable(OpenGLControl.GL_LIGHTING);
            //OpenGLControl.glDisable(OpenGLControl.GL_CULL_FACE);
            OpenGLControl.glEnable(OpenGLControl.GL_DEPTH_TEST);
            OpenGLControl.glEnable(OpenGLControl.GL_BLEND);
            OpenGLControl.glBlendFunc(OpenGLControl.GL_SRC_ALPHA, OpenGLControl.GL_ONE_MINUS_SRC_ALPHA);

            // выводим полупрозрачный куб
            OpenGLControl.glPolygonMode(OpenGLControl.GL_FRONT_AND_BACK, OpenGLControl.GL_FILL);
            OpenGLControl.glColor4d(m_STCubeColor.R / 255.0, m_STCubeColor.G / 255.0, m_STCubeColor.B / 255.0, m_STCubeColor.A / 255.0);
            OpenGLControl.glCallList(m_SemiTransparentListBase);
            // выводим полупрозрачный куб

            // выводим контур полупрозрачного куба
            OpenGLControl.glPolygonMode(OpenGLControl.GL_FRONT_AND_BACK, OpenGLControl.GL_LINE);
            OpenGLControl.glColor4d(m_STCubeContourColor.R / 255.0, m_STCubeContourColor.G / 255.0, m_STCubeContourColor.B / 255.0, m_STCubeContourColor.A / 255.0);
            OpenGLControl.glLineWidth(2.0f);
            OpenGLControl.glCallList(m_SemiTransparentListBase);
            // выводим контур полупрозрачного куба

            OpenGLControl.glDisable(OpenGLControl.GL_BLEND);

            OpenGLControl.glPopMatrix();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            m_OCubeColor = OCubeColorPB.ColorProperty;
            m_OCubeContourColor = OCubeContourColorPB.ColorProperty;
            m_STCubeColor = STCubeColorPB.ColorProperty;
            m_STCubeContourColor = STCubeContourColorPB.ColorProperty;

            PaintBox.Invalidate();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// 
        /// </summary>
        private Point3D[,] m_CubeSides = new Point3D[,] { { new Point3D(1.0, 1.0, 1.0), new Point3D(-1.0, 1.0, 1.0), new Point3D(-1.0, -1.0, 1.0), new Point3D(1.0, -1.0, 1.0) },
                                                          { new Point3D(1.0, 1.0, 1.0), new Point3D(1.0, -1.0, 1.0), new Point3D(1.0, -1.0, -1.0), new Point3D(1.0, 1.0, -1.0) },
                                                          { new Point3D(1.0, 1.0, 1.0), new Point3D(1.0, 1.0, -1.0), new Point3D(-1.0, 1.0, -1.0), new Point3D(-1.0, 1.0, 1.0) },
                                                          { new Point3D(-1.0, -1.0, -1.0), new Point3D(-1.0, -1.0 ,1.0), new Point3D(-1.0, 1.0, 1.0), new Point3D(-1.0, 1.0, -1.0) },
                                                          { new Point3D(-1.0, -1.0, -1.0), new Point3D(1.0, -1.0, -1.0), new Point3D(1.0, -1.0, 1.0), new Point3D(-1.0, -1.0, 1.0) },
                                                          { new Point3D(-1.0, -1.0, -1.0), new Point3D(-1.0, 1.0, -1.0), new Point3D(1.0, 1.0, -1.0), new Point3D(1.0, -1.0, -1.0) } };
        /// <summary>
        /// 
        /// </summary>
        private Vector3D[] m_CubeSideNormals = new Vector3D[] { new Vector3D(0.0, 0.0, 1.0),
                                                                new Vector3D(1.0, 0.0, 0.0),
                                                                new Vector3D(0.0, 1.0, 0.0),
                                                                new Vector3D(-1.0, 0.0, 0.0),
                                                                new Vector3D(0.0, -1.0, 0.0),
                                                                new Vector3D(0.0, 0.0, -1.0) };

        /// <summary>
        /// база дисплейного списка, содержащего непрозрачный куб
        /// </summary>
        private UInt32 m_OpaqueListBase;
        /// <summary>
        /// база дисплейного списка, содержащего полупрозрачный куб
        /// </summary>
        private UInt32 m_SemiTransparentListBase;

        /// <summary>
        /// цвет непрозрачного куба
        /// </summary>
        private Color m_OCubeColor;
        /// <summary>
        /// цвет контура непрозрачного куба
        /// </summary>
        private Color m_OCubeContourColor;
        /// <summary>
        /// цвет полупрозрачного куба
        /// </summary>
        private Color m_STCubeColor;
        /// <summary>
        /// цвет контура полупрозрачного куба
        /// </summary>
        private Color m_STCubeContourColor;

        /// <summary>
        /// угол поворота относительно оси OX
        /// </summary>
        private Double m_AngleOX;
        /// <summary>
        /// угол поворота относительно оси OY
        /// </summary>
        private Double m_AngleOY;
    }
}