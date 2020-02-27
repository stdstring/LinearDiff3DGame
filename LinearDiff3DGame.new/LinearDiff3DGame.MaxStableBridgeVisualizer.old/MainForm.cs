using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.Matrix;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.MaxStableBridge;
using LinearDiff3DGame.MaxStableBridge.Input;

namespace LinearDiff3DGame.MaxStableBridgeVisualizer.old
{
    internal struct Line2D
    {
        public Int32 x1;
        public Int32 y1;
        public Int32 x2;
        public Int32 y2;

        public Line2D(Int32 x1, Int32 y1, Int32 x2, Int32 y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
    }

    public partial class MainForm : Form
    {
        private readonly BridgeBuilder msbBuilder;

        //private MaxStableBridgeBuilder2 m_MSBBuilder;

        // m_SightDirection - вектор, задающий направление взгляда (совпадает с ортом осей Y' и Y")
        private Vector3D sightDirection;

        private Double nx;
        private Double ny;
        private Double nz;

        // m_DistanceD - расстояние между точками O' и O" (см. алгоритм)
        private Double distanceD;

        // m_MaxHorizSightAngle - максимальный угол зрения по горизонтали (в плоскости X"O"Y")
        private readonly Double maxHorizSightAngle;

        // m_MaxVertSightAngle - максимальный угол зрения по вертикали (в плоскости Z"O"Y")
        private readonly Double maxVertSightAngle;

        private IPolyhedron3D currentPolyhedron;

        private readonly List<Line2D> lineList;

        private readonly Int32 XBorder = 50;

        private readonly Int32 YBorder = 50;

        private readonly Int32 X0 = 50;

        private readonly Int32 Y0 = 50;

        private readonly Int32 XMax = 350;

        private readonly Int32 YMax = 350;

        private const String inputDataFileName = "InputData.xml";

        /// <summary>
        /// метод CalcDeterminant3 вычисляет определитель матрицы MatrixA размером 3x3
        /// </summary>
        /// <param name="MatrixA"></param>
        /// <returns></returns>
        private Double CalcDeterminant3(Matrix MatrixA)
        {
            Double Result = 0;

            Result += MatrixA[1, 1]*(MatrixA[2, 2]*MatrixA[3, 3] - MatrixA[2, 3]*MatrixA[3, 2]);
            Result += MatrixA[1, 2]*(MatrixA[2, 3]*MatrixA[3, 1] - MatrixA[2, 1]*MatrixA[3, 3]);
            Result += MatrixA[1, 3]*(MatrixA[2, 1]*MatrixA[3, 2] - MatrixA[2, 2]*MatrixA[3, 1]);

            return Result;
        }

        /// <summary>
        /// метод CalcAlgebraicAddition вычисляет алгебраическое дополнение
        /// </summary>
        /// <param name="MatrixA"></param>
        /// <param name="RowIndex"></param>
        /// <param name="ColumnIndex"></param>
        /// <returns></returns>
        private Double CalcAlgebraicAddition(Matrix MatrixA, Int32 RowIndex, Int32 ColumnIndex)
        {
            if((RowIndex == 1) && (ColumnIndex == 1))
                return (MatrixA[2, 2]*MatrixA[3, 3] - MatrixA[2, 3]*MatrixA[3, 2]);
            if((RowIndex == 1) && (ColumnIndex == 2))
                return (MatrixA[2, 3]*MatrixA[3, 1] - MatrixA[2, 1]*MatrixA[3, 3]);
            if((RowIndex == 1) && (ColumnIndex == 3))
                return (MatrixA[2, 1]*MatrixA[3, 2] - MatrixA[2, 2]*MatrixA[3, 1]);
            if((RowIndex == 2) && (ColumnIndex == 1))
                return (MatrixA[1, 3]*MatrixA[3, 2] - MatrixA[1, 2]*MatrixA[3, 3]);
            if((RowIndex == 2) && (ColumnIndex == 2))
                return (MatrixA[1, 1]*MatrixA[3, 3] - MatrixA[1, 3]*MatrixA[3, 1]);
            if((RowIndex == 2) && (ColumnIndex == 3))
                return (MatrixA[1, 2]*MatrixA[3, 1] - MatrixA[1, 1]*MatrixA[3, 2]);
            if((RowIndex == 3) && (ColumnIndex == 1))
                return (MatrixA[1, 2]*MatrixA[2, 3] - MatrixA[2, 2]*MatrixA[1, 3]);
            if((RowIndex == 3) && (ColumnIndex == 2))
                return (MatrixA[1, 3]*MatrixA[2, 1] - MatrixA[1, 1]*MatrixA[2, 3]);
            if((RowIndex == 3) && (ColumnIndex == 3))
                return (MatrixA[1, 1]*MatrixA[2, 2] - MatrixA[1, 2]*MatrixA[2, 1]);

            return Double.NaN;
        }

        /// <summary>
        /// метод InverseMatrix3 возвращает обратную матрицу для матрицы MatrixA
        /// </summary>
        /// <param name="MatrixA"></param>
        /// <returns></returns>
        private Matrix InverseMatrix3(Matrix MatrixA)
        {
            Matrix InverseMatrix = new Matrix(3, 3);

            Double MatrixADeterminantValue = CalcDeterminant3(MatrixA);

            for (Int32 RowIndex = 1; RowIndex <= MatrixA.RowCount; RowIndex++)
            {
                for (Int32 ColumnIndex = 1; ColumnIndex <= MatrixA.ColumnCount; ColumnIndex++)
                {
                    InverseMatrix[ColumnIndex, RowIndex] = CalcAlgebraicAddition(MatrixA, RowIndex, ColumnIndex)/
                                                           MatrixADeterminantValue;
                }
            }

            return InverseMatrix;
        }

        /// <summary>
        /// метод GetCoordTransformMatrix возвращает матрицу перехода из СК(XYZ) в СК(X'Y'Z')
        /// </summary>
        /// <returns></returns>
        private Matrix GetCoordTransformMatrix()
        {
            Matrix CoordTransformMatrix = new Matrix(3, 3);

            Double nx = sightDirection.X;
            Double ny = sightDirection.Y;
            Double nz = sightDirection.Z;

            Double AlphaValue = Math.Sqrt(ny*ny/(nx*nx + ny*ny));
            Double BetaValue = -Math.Sqrt(nx*nx/(nx*nx + ny*ny));

            CoordTransformMatrix[1, 1] = AlphaValue;
            CoordTransformMatrix[1, 2] = BetaValue;
            CoordTransformMatrix[1, 3] = 0;
            CoordTransformMatrix[2, 1] = nx;
            CoordTransformMatrix[2, 2] = ny;
            CoordTransformMatrix[2, 3] = nz;
            CoordTransformMatrix[3, 1] = BetaValue*nz;
            CoordTransformMatrix[3, 2] = -AlphaValue*nz;
            CoordTransformMatrix[3, 3] = AlphaValue*ny - BetaValue*nx;

            return CoordTransformMatrix;
        }

        /// <summary>
        /// метод TranformPointCoords для точки с координатами в СК(XYZ) возвращает ее координаты в СК(X"Y"Z")
        /// </summary>
        /// <param name="XCoord"></param>
        /// <param name="YCoord"></param>
        /// <param name="ZCoord"></param>
        /// <param name="InverseCoordTransformMatrix"></param>
        /// <returns></returns>
        private Point3D TransformPointCoords(Double XCoord, Double YCoord, Double ZCoord,
                                             Matrix InverseCoordTransformMatrix)
        {
            Matrix OldCoordMatrix = new Matrix(1, 3);
            // координаты точки в СК(XYZ)
            OldCoordMatrix[1, 1] = XCoord;
            OldCoordMatrix[1, 2] = YCoord;
            OldCoordMatrix[1, 3] = ZCoord;

            // координаты точеи после поворота СК - в СК (X'Y'Z')
            Matrix NewCoordMatrix = OldCoordMatrix*InverseCoordTransformMatrix;
            // координаты точки после поворота и сдвига СК - в СК(X"Y"Z")
            //NewCoordMatrix[1, 1] = +m_DistanceD * m_SightDirection.X;
            //NewCoordMatrix[1, 2] = +m_DistanceD * m_SightDirection.Y;
            NewCoordMatrix[1, 2] = +distanceD;
            //NewCoordMatrix[1, 3] = +m_DistanceD * m_SightDirection.Z;

            return new Point3D(NewCoordMatrix[1, 1], NewCoordMatrix[1, 2], NewCoordMatrix[1, 3]);
        }

        /// <summary>
        /// метод IsPointOutOfSight возвращает true, если точка за пределами прямоугольника видимости на плоскости (см. алгоритм)
        /// </summary>
        /// <param name="XR"></param>
        /// <param name="ZR"></param>
        /// <returns></returns>
        private Boolean IsPointOutOfSight(Double XR, Double ZR)
        {
            return ((Math.Abs(XR) > 1) || (Math.Abs(ZR) > 1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LineList"></param>
        private void FillLineList(List<Line2D> LineList)
        {
            LineList.Clear();

            Matrix InverseCoordTransformMatrix = InverseMatrix3(GetCoordTransformMatrix());

            foreach (IPolyhedronSide3D CurrentSide in currentPolyhedron.SideList)
            {
                // если скалярное произведение вектора направления взгляда и нормали грани >=0, то это значит, что мы эту грань не видим
                if(Vector3DUtils.ScalarProduct(sightDirection, CurrentSide.SideNormal) >= 0)
                    continue;

                for (Int32 VertexIndex = 0; VertexIndex < CurrentSide.VertexList.Count; VertexIndex++)
                {
                    IPolyhedronVertex3D CurrentVertex = CurrentSide.VertexList[VertexIndex];
                    IPolyhedronVertex3D NextVertex = (VertexIndex == CurrentSide.VertexList.Count - 1
                                                          ? CurrentSide.VertexList[0]
                                                          : CurrentSide.VertexList[VertexIndex + 1]);

                    // координаты вершин после поворота и сдвига СК - в СК(X"Y"Z")
                    Point3D CurrentVertexNewCoords = TransformPointCoords(CurrentVertex.XCoord, CurrentVertex.YCoord,
                                                                          CurrentVertex.ZCoord,
                                                                          InverseCoordTransformMatrix);
                    Point3D NextVertexNewCoords = TransformPointCoords(NextVertex.XCoord, NextVertex.YCoord,
                                                                       NextVertex.ZCoord, InverseCoordTransformMatrix);

                    // относительные координаты точки на плоскости (см. алгоритм)
                    Double X1R = CurrentVertexNewCoords.X/
                                 (CurrentVertexNewCoords.Y*Math.Tan(maxHorizSightAngle));
                    Double Z1R = CurrentVertexNewCoords.Z/
                                 (CurrentVertexNewCoords.Y*Math.Tan(maxVertSightAngle));
                    Double X2R = NextVertexNewCoords.X/(NextVertexNewCoords.Y*Math.Tan(maxHorizSightAngle));
                    Double Z2R = NextVertexNewCoords.Z/(NextVertexNewCoords.Y*Math.Tan(maxVertSightAngle));

                    // обе точки за пределами прямоугольника видимости на плоскости (см. алгоритм)
                    if(IsPointOutOfSight(X1R, Z1R) && IsPointOutOfSight(X2R, Z2R))
                        continue;
                    // первая точка за пределами прямоугольника видимости на плоскости (см. алгоритм)
                    if(IsPointOutOfSight(X1R, Z1R))
                    {
                    }
                    // вторая точка за пределами прямоугольника видимости на плоскости (см. алгоритм)
                    if(IsPointOutOfSight(X2R, Z2R))
                    {
                    }

                    Line2D CurrentLine = new Line2D();
                    CurrentLine.x1 = X0 + XMax + (Int32) (X1R*XMax);
                    CurrentLine.y1 = Y0 + YMax - (Int32) (Z1R*YMax);
                    CurrentLine.x2 = X0 + XMax + (Int32) (X2R*XMax);
                    CurrentLine.y2 = Y0 + YMax - (Int32) (Z2R*YMax);

                    LineList.Add(CurrentLine);
                }
            }
        }

        /*private Dictionary<String, Object> GetInitdata()
        {
            Dictionary<String, Object> initData = new Dictionary<String, Object>();

            initData.Add("Epsilon", 1e-9);

            initData.Add("DeltaT", 0.1);

            Matrix matrixA = new Matrix(3, 3);
            Double k = 0;
            matrixA[1, 1] = 0;
            matrixA[1, 2] = 1;
            matrixA[1, 3] = 0;
            matrixA[2, 1] = k;
            matrixA[2, 2] = 0;
            matrixA[2, 3] = 0;
            matrixA[3, 1] = 0;
            matrixA[3, 2] = 0;
            matrixA[3, 3] = 0;
            initData.Add("MatrixA", matrixA);

            Matrix matrixB = new Matrix(3, 1);
            matrixB[1, 1] = 0;
            matrixB[2, 1] = 1;
            matrixB[3, 1] = 0;
            initData.Add("MatrixB", matrixB);

            Matrix matrixC = new Matrix(3, 1);
            matrixC[1, 1] = 1;
            matrixC[2, 1] = 0;
            matrixC[3, 1] = 0;
            initData.Add("MatrixC", matrixC);

            initData.Add("MpMax", 1.0);
            initData.Add("MpMin", -1.0);

            initData.Add("MqMax", 1.0);
            initData.Add("MqMin", -1.0);

            Double maxCValue = 2.5;
            Int32 vertexCount = 5;
            Point3D[] finalSet = new Point3D[vertexCount];
            finalSet[0] = new Point3D(0, 0, 0);
            finalSet[1] = new Point3D(maxCValue, maxCValue, maxCValue);
            finalSet[2] = new Point3D(-maxCValue, maxCValue, maxCValue);
            finalSet[3] = new Point3D(-maxCValue, -maxCValue, maxCValue);
            finalSet[4] = new Point3D(maxCValue, -maxCValue, maxCValue);
            initData.Add("TerminalSet", finalSet);

            return initData;
            return null;
        }*/

        public MainForm()
        {
            InitializeComponent();

            XmlSerializer serializer = new XmlSerializer(typeof (InputParams));
            InputParams inputParams;
            using (
                StreamReader sr =
                    new StreamReader(
                        "..\\..\\..\\LinearDiff3DGame.MaxStableBridge\\Input\\MaterialPointClassicInput.xml"))
                inputParams = (InputParams) serializer.Deserialize(sr);
            ApproxComp approxComp = new ApproxComp(1e-9);
            msbBuilder = new BridgeBuilder(new BridgeBuilderData(inputParams, approxComp));
            //m_MSBBuilder = new MaxStableBridgeBuilder2(/*GetInitdata()*/);

            Height = Screen.PrimaryScreen.WorkingArea.Height;
            Width = Screen.PrimaryScreen.WorkingArea.Width;

            lblInfo.Location = new Point(X0 + 2*XMax + 40, Y0);

            // начальные значения для m_SightDirection и m_DistanceD (берутся от балды)
            /*m_nx = 1;
            m_ny = 1;
            m_nz = 0;*/
            nx = 0.01;
            ny = 0.01;
            nz = 1;
            sightDirection = Vector3DUtils.NormalizeVector(new Vector3D(nx, ny, nz));
            distanceD = 8;
            // значения для m_MaxHorizSightAngle и m_MaxVertSightAngle (требуют подбора)
            maxHorizSightAngle = 45;
            maxHorizSightAngle *= Math.PI/180;
            maxVertSightAngle = 45;
            maxVertSightAngle *= Math.PI/180;

            currentPolyhedron = msbBuilder.CurrentTSection;
            lineList = new List<Line2D>();
            FillLineList(lineList);
        }

        private void nextIterationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            msbBuilder.NextIteration();
            //MessageBox.Show("SideCount = " + m_CurrentSideList.Count.ToString());

            currentPolyhedron = msbBuilder.CurrentTSection;
            FillLineList(lineList);
            Invalidate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush WhiteBrush = new SolidBrush(Color.White);
            e.Graphics.FillRectangle(WhiteBrush, X0, Y0, 2*XMax, 2*YMax);
            Pen BlackPen = new Pen(Color.Black);
            foreach (Line2D CurrentLine in lineList)
                e.Graphics.DrawLine(BlackPen, CurrentLine.x1, CurrentLine.y1, CurrentLine.x2, CurrentLine.y2);

            Pen BluePen = new Pen(Color.Blue);
            e.Graphics.DrawRectangle(BluePen, X0, Y0, 2*XMax, 2*YMax);

            lblInfo.Text = "InverseTime = " + Math.Round(msbBuilder.InverseTime, 4) + "\nSide's count = " +
                           currentPolyhedron.SideList.Count;
        }

        private void ChangeSightParamstoolStripMenuItem_Click(object sender, EventArgs e)
        {
            SightParamForm spf = new SightParamForm();
            spf.nx = nx;
            spf.ny = ny;
            spf.nz = nz;
            spf.Distance = distanceD;

            if(spf.ShowDialog() == DialogResult.OK)
            {
                nx = spf.nx;
                ny = spf.ny;
                nz = spf.nz;
                sightDirection = new Vector3D(nx, ny, nz);
                sightDirection = Vector3DUtils.NormalizeVector(sightDirection);

                distanceD = spf.Distance;

                FillLineList(lineList);
                Invalidate();
            }
        }

        private void SaveDatatoolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<SideClass> SideList = m_AC.GetSideList();

                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, false))
                {
                    foreach (SideClass CurrentSide in SideList)
                    {
                        sw.WriteLine("{0} {1} {2}", CurrentSide.SideNormal.X, CurrentSide.SideNormal.Y, CurrentSide.SideNormal.Z);

                        for (Int32 VertexIndex = 0; VertexIndex < CurrentSide.VertexCount; VertexIndex++)
                        {
                            VertexClass CurrentVertex = CurrentSide[VertexIndex];

                            sw.Write("{0} {1} {2}", CurrentVertex.X, CurrentVertex.Y, CurrentVertex.Z);
                            if (VertexIndex < (CurrentSide.VertexCount - 1))
                            {
                                sw.Write(' ');
                            }
                        }
                        sw.WriteLine();
                    }
                }
            }*/
        }

        private void SaveData2toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            /*if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<VertexClass> VertexList = m_AC.GetVertexList();
                List<SideClass> SideList = m_AC.GetSideList();

                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, false))
                {
                    // сначало записываем координаты вершин
                    foreach (VertexClass CurrentVertex in VertexList)
                    {
                        String X = CurrentVertex.X.ToString(CultureInfo.InvariantCulture);
                        String Y = CurrentVertex.Y.ToString(CultureInfo.InvariantCulture);
                        String Z = CurrentVertex.Z.ToString(CultureInfo.InvariantCulture);

                        sw.WriteLine("{0} {1} {2}", X, Y, Z);
                    }
                    sw.WriteLine();
                    // потом для каждой грани сначало координаты внешней нормали,
                    // потом список индексов вершин, составляющих грань (упорядоченных против ч.с.)
                    foreach (SideClass CurrentSide in SideList)
                    {
                        String SideNormalXCoord = CurrentSide.SideNormal.X.ToString(CultureInfo.InvariantCulture);
                        String SideNormalYCoord = CurrentSide.SideNormal.Y.ToString(CultureInfo.InvariantCulture);
                        String SideNormalZCoord = CurrentSide.SideNormal.Z.ToString(CultureInfo.InvariantCulture);

                        sw.WriteLine("{0} {1} {2}", SideNormalXCoord, SideNormalYCoord, SideNormalZCoord);

                        for (Int32 VertexIndex = 0; VertexIndex < CurrentSide.VertexCount; VertexIndex++)
                        {
                            VertexClass CurrentVertex = CurrentSide[VertexIndex];
                            Int32 CurrentVertexIndex = VertexList.IndexOf(CurrentVertex);

                            sw.Write(CurrentVertexIndex);
                            if (VertexIndex < (CurrentSide.VertexCount - 1))
                            {
                                sw.Write(' ');
                            }
                        }
                        sw.WriteLine();
                    }
                }
            }*/
        }
    }
}