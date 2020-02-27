using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using PopovYuri.Visualization;
using MathPostgraduateStudy.LinearDiff3DGame;
using MathPostgraduateStudy.OpenGLColorChangeDialog;

namespace MathPostgraduateStudy.BuildRobustControl
{
    public partial class RobustControlViewerForm : Form
    {
        public RobustControlViewerForm()
        {
            InitializeComponent();

            //m_OBridgeListBase = 0;
            //m_STBridgeListBase = 0;

            m_BridgeListBases = new UInt32[] { };
            m_BridgeListCount = 0;

            m_AngleOX = 0;
            m_AngleOY = 0;

            // пока задаем цвета так ... потом будем считывать их из файла
            m_OPolyhedronColor = Color.FromArgb(255, 102, 153, 153);
            m_OPolyhedronContourColor = Color.FromArgb(255, 0, 0, 0);
            m_STPolyhedronColor = Color.FromArgb(89, 153, 51, 51);
            m_STPolyhedronContourColor = Color.FromArgb(255, 255, 0, 0);
            m_InnerPosSpotColor = Color.FromArgb(255, 255, 160, 0);
            m_OuterPosSpotColor = Color.FromArgb(255, 255, 0, 0);
            // пока задаем цвета так ... потом будем считывать их из файла

            m_RobustControlBuilder = null;

            m_FirstGamerControl = null;
            m_SecondGamerControl = null;
            m_StableBridgeSystem = null;
            //m_IsPosInMainBridge = null;
            m_NearestBottomBridgeIndex = null;
            m_SystemPos = null;
            m_TimeValueList = null;
            m_NearestBridgePoint = null;

            m_ShowOuterBridgeWithIndex = true;
            m_OuterBridgeIndex = 0;
        }

        /// <summary>
        /// формируем непрерывный диапазон дисплейных списков, представляющих стабильный мост stableBridge
        /// </summary>
        /// <param name="stableBridge"></param>
        /// <returns></returns>
        private UInt32 CreateStableBridgeDisplayLists(ConvexPolyhedron3D[] stableBridge)
        {
            PaintBox.ActivateContext();

            UInt32 displayListsBase = OpenGLControl.glGenLists(stableBridge.Length);

            // цикл по всем T-сечениям моста
            for (UInt32 sectionIndex = 0; sectionIndex < stableBridge.Length; sectionIndex++)
            {
                ConvexPolyhedron3D currentTSection = stableBridge[sectionIndex];

                OpenGLControl.glNewList(displayListsBase + sectionIndex, OpenGLControl.GL_COMPILE);
                // цикл по всем граням текущего T-сечения
                for (Int32 sideIndex = 0; sideIndex < currentTSection.SideCount; sideIndex++)
                {
                    SideClass currentSide = currentTSection.GetSide(sideIndex);

                    /*OpenGLControl.glBegin(OpenGLControl.GL_POLYGON);
                    OpenGLControl.glNormal3d(currentSide.SideNormal.XCoord, currentSide.SideNormal.YCoord, currentSide.SideNormal.ZCoord);
                    // цикл по всем вершинам текущей грани
                    for (Int32 vertexIndex = 0; vertexIndex < currentSide.VertexCount; vertexIndex++)
                    {
                        OpenGLControl.glVertex3d(currentSide[vertexIndex].XCoord,
                                                 currentSide[vertexIndex].YCoord,
                                                 currentSide[vertexIndex].ZCoord);
                    }
                    // цикл по всем вершинам текущей грани
                    OpenGLControl.glEnd();*/

                    // цикл по всем вершинам текущей грани
                    for (Int32 vertexIndex = 2; vertexIndex < currentSide.VertexCount; vertexIndex++)
                    {
                        OpenGLControl.glBegin(OpenGLControl.GL_TRIANGLES);

                        OpenGLControl.glNormal3d(currentSide.SideNormal.XCoord, currentSide.SideNormal.YCoord, currentSide.SideNormal.ZCoord);
                        OpenGLControl.glEdgeFlag(vertexIndex == 2 ? (byte)OpenGLControl.GL_TRUE : (byte)OpenGLControl.GL_FALSE);
                        OpenGLControl.glVertex3d(currentSide[0].XCoord,
                                                 currentSide[0].YCoord,
                                                 currentSide[0].ZCoord);
                        OpenGLControl.glEdgeFlag((byte)OpenGLControl.GL_TRUE);
                        OpenGLControl.glVertex3d(currentSide[vertexIndex - 1].XCoord,
                                                 currentSide[vertexIndex - 1].YCoord,
                                                 currentSide[vertexIndex - 1].ZCoord);
                        OpenGLControl.glEdgeFlag(vertexIndex == currentSide.VertexCount - 1 ? (byte)OpenGLControl.GL_TRUE : (byte)OpenGLControl.GL_FALSE);
                        OpenGLControl.glVertex3d(currentSide[vertexIndex].XCoord,
                                                 currentSide[vertexIndex].YCoord,
                                                 currentSide[vertexIndex].ZCoord);

                        OpenGLControl.glEnd();
                    }
                    // цикл по всем вершинам текущей грани
                }
                // цикл по всем граням текущего T-сечения
                OpenGLControl.glEndList();
            }
            // цикл по всем T-сечениям моста

            PaintBox.DeactivateContext();

            return displayListsBase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispalsListsBase"></param>
        /// <param name="dispalsListsCount"></param>
        private void DeleteStableBridgeDisplayLists(UInt32 dispalsListsBase, Int32 dispalsListsCount)
        {
            OpenGLControl.glDeleteLists(dispalsListsBase, dispalsListsCount);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetTimeLabel()
        {
            if (m_TimeValueList != null)
            {
#warning !!!!!!!!
                Double timeValue = Math.Round(m_TimeValueList[timeBar.Value], 4);
                lblTime.Text = "t = " + timeValue.ToString();
            }
        }

        private void CalculateRobustControl()
        {
            RCInputDataReader inputDataReader = new RCInputDataReader("InputData.xml");
            DataContainer inputData = inputDataReader.InputData;

            if (m_RobustControlBuilder == null)
            {
                m_RobustControlBuilder = new RobustControlBuilder(inputData);
            }
            Point3D startPoint = (Point3D)inputData["StartPoint"];
            DataContainer outputData = m_RobustControlBuilder.BuildRobustControl(startPoint);

            m_FirstGamerControl = outputData["FirstGamerControl"] as Double[];
            m_SecondGamerControl = outputData["SecondGamerControl"] as Double[];
            m_TimeValueList = outputData["TimeValueList"] as Double[];
            m_SystemPos = outputData["SystemPos"] as Point3D[];
            m_NearestBridgePoint = outputData["NearestBridgePoint"] as Point3D[];
            //m_IsPosInMainBridge = outputData["IsPosInMainBridge"] as Boolean[];
            m_NearestBottomBridgeIndex = outputData["NearestBottomBridgeIndex"] as Int32[];
            /*List<ConvexPolyhedron3D[]>*/
            m_StableBridgeSystem = outputData["StableBridgeSystem"] as List<ConvexPolyhedron3D[]>;

            //DeleteStableBridgeDisplayLists(m_OBridgeListBase, m_BridgeListCount);
            //DeleteStableBridgeDisplayLists(m_STBridgeListBase, m_BridgeListCount);
            for (Int32 bridgeListBaseIndex = 0; bridgeListBaseIndex < m_BridgeListBases.Length; bridgeListBaseIndex++)
            {
                DeleteStableBridgeDisplayLists(m_BridgeListBases[bridgeListBaseIndex], m_BridgeListCount);
            }

            //m_OBridgeListBase = CreateStableBridgeDisplayLists(stableBridgeSystem[0]);
            //m_STBridgeListBase = CreateStableBridgeDisplayLists(stableBridgeSystem[2]);
            m_BridgeListCount = m_TimeValueList.Length;
            m_BridgeListBases = new UInt32[m_StableBridgeSystem.Count];
            for (Int32 bridgeListBaseIndex = 0; bridgeListBaseIndex < m_BridgeListBases.Length; bridgeListBaseIndex++)
            {
                m_BridgeListBases[bridgeListBaseIndex] = CreateStableBridgeDisplayLists(m_StableBridgeSystem[bridgeListBaseIndex]);
            }
        }

        private unsafe void PaintBox_Paint(object sender, PaintEventArgs e)
        {
            OpenGLControl.glClearColor(1.0f, 1.0f, 1.0f, 0.0f);
            OpenGLControl.glClear(OpenGLControl.GL_COLOR_BUFFER_BIT | OpenGLControl.GL_DEPTH_BUFFER_BIT);

            if (m_BridgeListCount == 0) return;

            OpenGLControl.glMatrixMode(OpenGLControl.GL_MODELVIEW);

            OpenGLControl.glPushMatrix();
            //OpenGLControl.gluLookAt(0.0, 0.0, 4.0, 0.0, 0.0, -10.0, 0.0, 1.0, 0.0);
            OpenGLControl.glTranslated(0.0, 0.0, -6.0);
            OpenGLControl.glRotated(m_AngleOY, 0.0, 1.0, 0.0);
            OpenGLControl.glRotated(m_AngleOX, 1.0, 0.0, 0.0);

            OpenGLControl.glEnable(OpenGLControl.GL_LIGHTING);
            OpenGLControl.glEnable(OpenGLControl.GL_LIGHT0);
            OpenGLControl.glEnable(OpenGLControl.GL_COLOR_MATERIAL);
            OpenGLControl.glLightModeli(OpenGLControl.GL_LIGHT_MODEL_TWO_SIDE, (Int32)OpenGLControl.GL_TRUE);

            // выводим внутренний (непрозрачный) многогранник
            OpenGLControl.glPolygonMode(OpenGLControl.GL_FRONT, OpenGLControl.GL_FILL);
            OpenGLControl.glEnable(OpenGLControl.GL_CULL_FACE);
            OpenGLControl.glCullFace(OpenGLControl.GL_BACK);
            OpenGLControl.glColor4d(m_OPolyhedronColor.R / 255.0, m_OPolyhedronColor.G / 255.0, m_OPolyhedronColor.B / 255.0, m_OPolyhedronColor.A / 255.0);
            //OpenGLControl.glCallList(m_OBridgeListBase + (UInt32)timeBar.Value);
            OpenGLControl.glCallList(m_BridgeListBases[0] + (UInt32)timeBar.Value);
            // выводим внутренний (непрозрачный) многогранник

            // выводим контур внутреннего (непрозрачного) многогранника
            OpenGLControl.glPolygonMode(OpenGLControl.GL_FRONT, OpenGLControl.GL_LINE);
            OpenGLControl.glEnable(OpenGLControl.GL_CULL_FACE);
            OpenGLControl.glCullFace(OpenGLControl.GL_BACK);
            OpenGLControl.glColor4d(m_OPolyhedronContourColor.R / 255.0, m_OPolyhedronContourColor.G / 255.0, m_OPolyhedronContourColor.B / 255.0, m_OPolyhedronContourColor.A / 255.0);
            OpenGLControl.glLineWidth(2.0f);
            //OpenGLControl.glCallList(m_OBridgeListBase + (UInt32)timeBar.Value);
            OpenGLControl.glCallList(m_BridgeListBases[0] + (UInt32)timeBar.Value);
            // выводим контур внутреннего (непрозрачного) многогранника

            OpenGLControl.glPushMatrix();
            Point3D currentPos = m_SystemPos[timeBar.Value];
            Boolean isPosInMainBridge = (m_NearestBottomBridgeIndex[timeBar.Value] == -1);

            if (!isPosInMainBridge && !m_ShowOuterBridgeWithIndex)
            {
                OpenGLControl.glPushMatrix();

                Point3D nearestPoint = m_NearestBridgePoint[timeBar.Value];
                OpenGLControl.glTranslated(nearestPoint.XCoord, nearestPoint.YCoord, nearestPoint.ZCoord);

                OpenGLControl.glColor4d(0.0, 0.0, 0.0, 1.0);
                OpenGLControl.GLUquadric* quadric = OpenGLControl.gluNewQuadric();
                OpenGLControl.gluQuadricOrientation(quadric, OpenGLControl.GLU_OUTSIDE);
                OpenGLControl.gluQuadricDrawStyle(quadric, OpenGLControl.GLU_FILL);
                OpenGLControl.gluSphere(quadric, 0.05, 30, 30);
                OpenGLControl.gluDeleteQuadric(quadric);

                OpenGLControl.glPopMatrix();
            }

            OpenGLControl.glTranslated(currentPos.XCoord, currentPos.YCoord, currentPos.ZCoord);

            OpenGLControl.glDisable(OpenGLControl.GL_LIGHTING);

            if (isPosInMainBridge)
            {
                OpenGLControl.glDisable(OpenGLControl.GL_DEPTH_TEST);
                OpenGLControl.glColor4d(m_InnerPosSpotColor.R / 255.0, m_InnerPosSpotColor.G / 255.0, m_InnerPosSpotColor.B / 255.0, m_InnerPosSpotColor.A / 255.0);
            }
            else
            {
                OpenGLControl.glColor4d(m_OuterPosSpotColor.R / 255.0, m_OuterPosSpotColor.G / 255.0, m_OuterPosSpotColor.B / 255.0, m_OuterPosSpotColor.A / 255.0);
            }

            OpenGLControl.GLUquadric* quadric2 = OpenGLControl.gluNewQuadric();
            OpenGLControl.gluQuadricOrientation(quadric2, OpenGLControl.GLU_OUTSIDE);
            OpenGLControl.gluQuadricDrawStyle(quadric2, OpenGLControl.GLU_FILL);
            OpenGLControl.gluSphere(quadric2, 0.05, 30, 30);
            OpenGLControl.gluDeleteQuadric(quadric2);

            OpenGLControl.glPopMatrix();

            Int32 nearestBottomBridgeIndex = m_NearestBottomBridgeIndex[timeBar.Value];
            if ((m_ShowOuterBridgeWithIndex && m_OuterBridgeIndex > 0) || (!m_ShowOuterBridgeWithIndex && nearestBottomBridgeIndex > 0))
            {
                Int32 bridgeListBaseIndex = (m_ShowOuterBridgeWithIndex ? m_OuterBridgeIndex : nearestBottomBridgeIndex);

                OpenGLControl.glEnable(OpenGLControl.GL_LIGHTING);
                OpenGLControl.glEnable(OpenGLControl.GL_DEPTH_TEST);
                OpenGLControl.glEnable(OpenGLControl.GL_BLEND);
                OpenGLControl.glBlendFunc(OpenGLControl.GL_SRC_ALPHA, OpenGLControl.GL_ONE_MINUS_SRC_ALPHA);

                // выводим внешний (полупрозрачный) многогранник
                OpenGLControl.glPolygonMode(OpenGLControl.GL_FRONT_AND_BACK, OpenGLControl.GL_FILL);
                OpenGLControl.glColor4d(m_STPolyhedronColor.R / 255.0, m_STPolyhedronColor.G / 255.0, m_STPolyhedronColor.B / 255.0, m_STPolyhedronColor.A / 255.0);
                //OpenGLControl.glCallList(m_STBridgeListBase + (UInt32)timeBar.Value);
                OpenGLControl.glCallList(m_BridgeListBases[bridgeListBaseIndex] + (UInt32)timeBar.Value);
                // выводим внешний (полупрозрачный) многогранник

                // выводим контур внешнего (полупрозрачного) многогранника
                OpenGLControl.glPolygonMode(OpenGLControl.GL_FRONT_AND_BACK, OpenGLControl.GL_LINE);
                OpenGLControl.glColor4d(m_STPolyhedronContourColor.R / 255.0, m_STPolyhedronContourColor.G / 255.0, m_STPolyhedronContourColor.B / 255.0, m_STPolyhedronContourColor.A / 255.0);
                OpenGLControl.glLineWidth(2.0f);
                //OpenGLControl.glCallList(m_STBridgeListBase + (UInt32)timeBar.Value);
                OpenGLControl.glCallList(m_BridgeListBases[bridgeListBaseIndex] + (UInt32)timeBar.Value);
                // выводим контур внешнего (полупрозрачного) многогранника

                OpenGLControl.glDisable(OpenGLControl.GL_BLEND);
            }

            OpenGLControl.glPopMatrix();
        }

        private void miCalcRobustControl_Click(object sender, EventArgs e)
        {
            PaintBox.Invalidate();
            PaintBox.UseWaitCursor = true;

            /*RCInputDataReader inputDataReader = new RCInputDataReader("InputData.xml");
            DataContainer inputData = inputDataReader.InputData;

            if (m_RobustControlBuilder == null)
            {
                m_RobustControlBuilder = new RobustControlBuilder(inputData);
            }
            Point3D startPoint = (Point3D)inputData["StartPoint"];
            DataContainer outputData = m_RobustControlBuilder.BuildRobustControl(startPoint);

            m_TimeValueList = outputData["TimeValueList"] as Double[];
            m_SystemPos = outputData["SystemPos"] as Point3D[];
            m_NearestBridgePoint = outputData["NearestBridgePoint"] as Point3D[];
            //m_IsPosInMainBridge = outputData["IsPosInMainBridge"] as Boolean[];
            m_NearestBottomBridgeIndex = outputData["NearestBottomBridgeIndex"] as Int32[];
            m_StableBridgeSystem = outputData["StableBridgeSystem"] as List<ConvexPolyhedron3D[]>;

            //DeleteStableBridgeDisplayLists(m_OBridgeListBase, m_BridgeListCount);
            //DeleteStableBridgeDisplayLists(m_STBridgeListBase, m_BridgeListCount);
            for (Int32 bridgeListBaseIndex = 0; bridgeListBaseIndex < m_BridgeListBases.Length; bridgeListBaseIndex++)
            {
                DeleteStableBridgeDisplayLists(m_BridgeListBases[bridgeListBaseIndex], m_BridgeListCount);
            }

            //m_OBridgeListBase = CreateStableBridgeDisplayLists(stableBridgeSystem[0]);
            //m_STBridgeListBase = CreateStableBridgeDisplayLists(stableBridgeSystem[2]);
            m_BridgeListCount = m_TimeValueList.Length;
            m_BridgeListBases = new UInt32[m_StableBridgeSystem.Count];
            for (Int32 bridgeListBaseIndex = 0; bridgeListBaseIndex < m_BridgeListBases.Length; bridgeListBaseIndex++)
            {
                m_BridgeListBases[bridgeListBaseIndex] = CreateStableBridgeDisplayLists(m_StableBridgeSystem[bridgeListBaseIndex]);
            }*/

            CalculateRobustControl();

            timeBar.Minimum = 0;
            timeBar.Maximum = m_TimeValueList.Length - 1;

            PaintBox.UseWaitCursor = false;

            SetTimeLabel();
        }

        private void miOpenBridgeSystemDataFile_Click(object sender, EventArgs e)
        {
            if (openDataFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(openDataFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();

                    m_RobustControlBuilder = bf.Deserialize(fs) as RobustControlBuilder;
                }

                PaintBox.Invalidate();
                PaintBox.UseWaitCursor = true;

                CalculateRobustControl();

                timeBar.Minimum = 0;
                timeBar.Maximum = m_TimeValueList.Length - 1;

                PaintBox.UseWaitCursor = false;

                SetTimeLabel();
            }
        }

        private void miSaveBridgeSystemDataFile_Click(object sender, EventArgs e)
        {
            if (saveDataFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (m_RobustControlBuilder == null)
                {
                    return;
                }

                using (FileStream fs = new FileStream(saveDataFileDialog.FileName, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bf = new BinaryFormatter();

                    bf.Serialize(fs, m_RobustControlBuilder);
                }
            }
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            //DeleteStableBridgeDisplayLists(m_OBridgeListBase, m_BridgeListCount);
            //DeleteStableBridgeDisplayLists(m_STBridgeListBase, m_BridgeListCount);
            for (Int32 bridgeListBaseIndex = 0; bridgeListBaseIndex < m_BridgeListBases.Length; bridgeListBaseIndex++)
            {
                DeleteStableBridgeDisplayLists(m_BridgeListBases[bridgeListBaseIndex], m_BridgeListCount);
            }
            this.Close();
        }

        private void miColorOfPolyhedrons_Click(object sender, EventArgs e)
        {
            OpenGLColorChangeForm colorChanger = new OpenGLColorChangeForm();
            colorChanger.OCubeColor = m_OPolyhedronColor;
            colorChanger.OCubeContourColor = m_OPolyhedronContourColor;
            colorChanger.STCubeColor = m_STPolyhedronColor;
            colorChanger.STCubeContourColor = m_STPolyhedronContourColor;

            if (colorChanger.ShowDialog() == DialogResult.OK)
            {
                m_OPolyhedronColor = colorChanger.OCubeColor;
                m_OPolyhedronContourColor = colorChanger.OCubeContourColor;
                m_STPolyhedronColor = colorChanger.STCubeColor;
                m_STPolyhedronContourColor = colorChanger.STCubeContourColor;

                PaintBox.Invalidate();
            }

            colorChanger.Dispose();
        }

        private void PaintBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_BridgeListCount == 0) return;

            if (e.KeyCode == Keys.A)
            {
                m_AngleOY -= 5;
                if (m_AngleOY < 0) m_AngleOY += 360;
                PaintBox.Invalidate();
            }
            if (e.KeyCode == Keys.D)
            {
                m_AngleOY += 5;
                if (m_AngleOY > 360) m_AngleOY -= 360;
                PaintBox.Invalidate();
            }
            if (e.KeyCode == Keys.W)
            {
                m_AngleOX -= 5;
                if (m_AngleOX < 0) m_AngleOX += 360;
                PaintBox.Invalidate();
            }
            if (e.KeyCode == Keys.S)
            {
                m_AngleOX += 5;
                if (m_AngleOX > 360) m_AngleOX -= 360;
                PaintBox.Invalidate();
            }
        }

        private void timeBar_ValueChanged(object sender, EventArgs e)
        {
            SetTimeLabel();
            PaintBox.Invalidate();
        }

        private void miVisualizationParams_Click(object sender, EventArgs e)
        {
            VisualizationParamsForm visualizationParams = new VisualizationParamsForm();
            visualizationParams.ShowOuterBridgeWithIndex = m_ShowOuterBridgeWithIndex;
            visualizationParams.OuterBridgeIndex = m_OuterBridgeIndex;

            if (visualizationParams.ShowDialog() == DialogResult.OK)
            {
                m_ShowOuterBridgeWithIndex = visualizationParams.ShowOuterBridgeWithIndex;
                m_OuterBridgeIndex = visualizationParams.OuterBridgeIndex;
                if (m_OuterBridgeIndex >= m_BridgeListBases.Length) m_OuterBridgeIndex = m_BridgeListBases.Length - 1;
                if (m_OuterBridgeIndex < 0) m_OuterBridgeIndex = 0;

                PaintBox.Invalidate();
            }

            visualizationParams.Dispose();
        }

        private void miSaveRobustControlData_Click(object sender, EventArgs e)
        {
            if (saveDataFileDialog.ShowDialog() == DialogResult.OK)
            {
                String fileName = saveDataFileDialog.FileName;

                using (StreamWriter sw = new StreamWriter(fileName, false))
                {
                    for (Int32 sectionIndex = 0; sectionIndex < m_TimeValueList.Length; sectionIndex++)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(m_TimeValueList[sectionIndex]);
                        sb.Append('\t');
                        sb.Append(m_SystemPos[sectionIndex].XCoord);
                        sb.Append('\t');
                        sb.Append(m_SystemPos[sectionIndex].YCoord);
                        sb.Append('\t');
                        sb.Append(m_SystemPos[sectionIndex].ZCoord);
                        sb.Append('\t');
                        sb.Append(m_FirstGamerControl[sectionIndex]);
                        sb.Append('\t');
                        sb.Append(m_SecondGamerControl[sectionIndex]);

                        sw.WriteLine(sb.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// список баз дисплейных списков для набора мостов
        /// </summary>
        private UInt32[] m_BridgeListBases;
        /// <summary>
        /// количество дисплейных списков для одного моста
        /// </summary>
        private Int32 m_BridgeListCount;

        /// <summary>
        /// цвет непрозрачного многогранника
        /// </summary>
        private Color m_OPolyhedronColor;
        /// <summary>
        /// цвет контура непрозрачного многогранника
        /// </summary>
        private Color m_OPolyhedronContourColor;
        /// <summary>
        /// цвет полупрозрачного многогранника
        /// </summary>
        private Color m_STPolyhedronColor;
        /// <summary>
        /// цвет контура полупрозрачного многогранника
        /// </summary>
        private Color m_STPolyhedronContourColor;
        /// <summary>
        /// цвет пятна, когда текущая позиции внутри внутреннего (непрозрачного) многогранника
        /// </summary>
        private Color m_InnerPosSpotColor;
        /// <summary>
        /// цвет пятна, когда текущая позиции снаружи внутреннего (непрозрачного) многогранника
        /// </summary>
        private Color m_OuterPosSpotColor;

        /// <summary>
        /// угол поворота относительно оси OX глобальной СК
        /// </summary>
        private Double m_AngleOX;
        /// <summary>
        /// угол поворота относительно оси OY глобальной СК
        /// </summary>
        private Double m_AngleOY;

        private Boolean m_ShowOuterBridgeWithIndex;
        private Int32 m_OuterBridgeIndex;

        /// <summary>
        /// построитель адаптивного управления
        /// </summary>
        private RobustControlBuilder m_RobustControlBuilder;

        private Double[] m_FirstGamerControl;
        private Double[] m_SecondGamerControl;
        private List<ConvexPolyhedron3D[]> m_StableBridgeSystem;
        private Double[] m_TimeValueList;
        private Point3D[] m_SystemPos;
        private Point3D[] m_NearestBridgePoint;
        //private Boolean[] m_IsPosInMainBridge;
        private Int32[] m_NearestBottomBridgeIndex;
    }
}