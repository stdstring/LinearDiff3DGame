using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace MathPostgraduateStudy.LinearDiff3DGame
{
    /// <summary>
    /// 
    /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        private AlgorithmClass m_AC;

        /// <summary>
        /// m_SightDirection - вектор, задающий направление взгляда (совпадает с ортом осей Y' и Y")
        /// </summary>
        private Vector3D m_SightDirection;
        private Double m_nx;
        private Double m_ny;
        private Double m_nz;
        /// <summary>
        /// m_DistanceD - расстояние между точками O' и O" (см. алгоритм)
        /// </summary>
        private Double m_DistanceD;
        /// <summary>
        /// m_MaxHorizSightAngle - максимальный угол зрения по горизонтали (в плоскости X"O"Y")
        /// </summary>
        private Double m_MaxHorizSightAngle;
        /// <summary>
        /// m_MaxVertSightAngle - максимальный угол зрения по вертикали (в плоскости Z"O"Y")
        /// </summary>
        private Double m_MaxVertSightAngle;

        /// <summary>
        /// 
        /// </summary>
        private List<SideClass> m_CurrentSideList;
        /// <summary>
        /// 
        /// </summary>
        private List<Line2D> m_LineList;

        /// <summary>
        /// 
        /// </summary>
        private readonly Int32 XBorder = 50;
        /// <summary>
        /// 
        /// </summary>
        private readonly Int32 YBorder = 50;
        /// <summary>
        /// 
        /// </summary>
        private readonly Int32 X0 = 50;
        /// <summary>
        /// 
        /// </summary>
        private readonly Int32 Y0 = 50;
        /// <summary>
        /// 
        /// </summary>
        private readonly Int32 XMax = 350;
        /// <summary>
        /// 
        /// </summary>
        private readonly Int32 YMax = 350;

        /// <summary>
        /// 
        /// </summary>
        private const String m_InputDataFileName = "InputData.xml";

        /// <summary>
        /// метод CalcDeterminant3 вычисляет определитель матрицы MatrixA размером 3x3
        /// </summary>
        /// <param name="MatrixA"></param>
        /// <returns></returns>
        private Double CalcDeterminant3(Matrix MatrixA)
        {
            Double Result = 0;

            Result += MatrixA[1, 1] * (MatrixA[2, 2] * MatrixA[3, 3] - MatrixA[2, 3] * MatrixA[3, 2]);
            Result += MatrixA[1, 2] * (MatrixA[2, 3] * MatrixA[3, 1] - MatrixA[2, 1] * MatrixA[3, 3]);
            Result += MatrixA[1, 3] * (MatrixA[2, 1] * MatrixA[3, 2] - MatrixA[2, 2] * MatrixA[3, 1]);

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
            if ((RowIndex == 1) && (ColumnIndex == 1)) return (MatrixA[2, 2] * MatrixA[3, 3] - MatrixA[2, 3] * MatrixA[3, 2]);
            if ((RowIndex == 1) && (ColumnIndex == 2)) return (MatrixA[2, 3] * MatrixA[3, 1] - MatrixA[2, 1] * MatrixA[3, 3]);
            if ((RowIndex == 1) && (ColumnIndex == 3)) return (MatrixA[2, 1] * MatrixA[3, 2] - MatrixA[2, 2] * MatrixA[3, 1]);
            if ((RowIndex == 2) && (ColumnIndex == 1)) return (MatrixA[1, 3] * MatrixA[3, 2] - MatrixA[1, 2] * MatrixA[3, 3]);
            if ((RowIndex == 2) && (ColumnIndex == 2)) return (MatrixA[1, 1] * MatrixA[3, 3] - MatrixA[1, 3] * MatrixA[3, 1]);
            if ((RowIndex == 2) && (ColumnIndex == 3)) return (MatrixA[1, 2] * MatrixA[3, 1] - MatrixA[1, 1] * MatrixA[3, 2]);
            if ((RowIndex == 3) && (ColumnIndex == 1)) return (MatrixA[1, 2] * MatrixA[2, 3] - MatrixA[2, 2] * MatrixA[1, 3]);
            if ((RowIndex == 3) && (ColumnIndex == 2)) return (MatrixA[1, 3] * MatrixA[2, 1] - MatrixA[1, 1] * MatrixA[2, 3]);
            if ((RowIndex == 3) && (ColumnIndex == 3)) return (MatrixA[1, 1] * MatrixA[2, 2] - MatrixA[1, 2] * MatrixA[2, 1]);

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
                    InverseMatrix[ColumnIndex, RowIndex] = CalcAlgebraicAddition(MatrixA, RowIndex, ColumnIndex) / MatrixADeterminantValue;
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

            Double nx = m_SightDirection.XCoord;
            Double ny = m_SightDirection.YCoord;
            Double nz = m_SightDirection.ZCoord;

            Double AlphaValue = Math.Sqrt(ny * ny / (nx * nx + ny * ny));
            Double BetaValue = -Math.Sqrt(nx * nx / (nx * nx + ny * ny));

            CoordTransformMatrix[1, 1] = AlphaValue;
            CoordTransformMatrix[1, 2] = BetaValue;
            CoordTransformMatrix[1, 3] = 0;
            CoordTransformMatrix[2, 1] = nx;
            CoordTransformMatrix[2, 2] = ny;
            CoordTransformMatrix[2, 3] = nz;
            CoordTransformMatrix[3, 1] = BetaValue * nz;
            CoordTransformMatrix[3, 2] = -AlphaValue * nz;
            CoordTransformMatrix[3, 3] = AlphaValue * ny - BetaValue * nx;

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
        private Point3D TransformPointCoords(Double XCoord, Double YCoord, Double ZCoord, Matrix InverseCoordTransformMatrix)
        {
            Matrix OldCoordMatrix = new Matrix(1, 3);
            // координаты точки в СК(XYZ)
            OldCoordMatrix[1, 1] = XCoord;
            OldCoordMatrix[1, 2] = YCoord;
            OldCoordMatrix[1, 3] = ZCoord;

            // координаты точеи после поворота СК - в СК (X'Y'Z')
            Matrix NewCoordMatrix = OldCoordMatrix * InverseCoordTransformMatrix;
            // координаты точки после поворота и сдвига СК - в СК(X"Y"Z")
            //NewCoordMatrix[1, 1] = +m_DistanceD * m_SightDirection.XCoord;
            //NewCoordMatrix[1, 2] = +m_DistanceD * m_SightDirection.YCoord;
            NewCoordMatrix[1, 2] = +m_DistanceD;
            //NewCoordMatrix[1, 3] = +m_DistanceD * m_SightDirection.ZCoord;

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

            foreach (SideClass CurrentSide in m_CurrentSideList)
            {
                // если скалярное произведение вектора направления взгляда и нормали грани >=0, то это значит, что мы эту грань не видим
                if (Vector3D.ScalarProduct(m_SightDirection, CurrentSide.SideNormal) >= 0)
                {
                    continue;
                }

                for (Int32 VertexIndex = 0; VertexIndex < CurrentSide.VertexCount; VertexIndex++)
                {
                    VertexClass CurrentVertex = CurrentSide[VertexIndex];
                    VertexClass NextVertex = (VertexIndex == CurrentSide.VertexCount - 1 ? CurrentSide[0] : CurrentSide[VertexIndex + 1]);

                    // координаты вершин после поворота и сдвига СК - в СК(X"Y"Z")
                    Point3D CurrentVertexNewCoords = TransformPointCoords(CurrentVertex.XCoord, CurrentVertex.YCoord, CurrentVertex.ZCoord, InverseCoordTransformMatrix);
                    Point3D NextVertexNewCoords = TransformPointCoords(NextVertex.XCoord, NextVertex.YCoord, NextVertex.ZCoord, InverseCoordTransformMatrix);

                    // относительные координаты точки на плоскости (см. алгоритм)
                    Double X1R = CurrentVertexNewCoords.XCoord / (CurrentVertexNewCoords.YCoord * Math.Tan(m_MaxHorizSightAngle));
                    Double Z1R = CurrentVertexNewCoords.ZCoord / (CurrentVertexNewCoords.YCoord * Math.Tan(m_MaxVertSightAngle));
                    Double X2R = NextVertexNewCoords.XCoord / (NextVertexNewCoords.YCoord * Math.Tan(m_MaxHorizSightAngle));
                    Double Z2R = NextVertexNewCoords.ZCoord / (NextVertexNewCoords.YCoord * Math.Tan(m_MaxVertSightAngle));

                    // обе точки за пределами прямоугольника видимости на плоскости (см. алгоритм)
                    if (IsPointOutOfSight(X1R, Z1R) && IsPointOutOfSight(X2R, Z2R))
                    {
                        continue;
                    }
                    // первая точка за пределами прямоугольника видимости на плоскости (см. алгоритм)
                    if (IsPointOutOfSight(X1R, Z1R))
                    {
                    }
                    // вторая точка за пределами прямоугольника видимости на плоскости (см. алгоритм)
                    if (IsPointOutOfSight(X2R, Z2R))
                    {
                    }

                    Line2D CurrentLine = new Line2D();
                    CurrentLine.x1 = X0 + XMax + (Int32)(X1R * XMax);
                    CurrentLine.y1 = Y0 + YMax - (Int32)(Z1R * YMax);
                    CurrentLine.x2 = X0 + XMax + (Int32)(X2R * XMax);
                    CurrentLine.y2 = Y0 + YMax - (Int32)(Z2R * YMax);

                    LineList.Add(CurrentLine);
                }
            }
        }

        public MainForm()
        {
            InitializeComponent();

            m_AC = new AlgorithmClass(new InputDataReader(m_InputDataFileName).InputData);

            this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            this.Width = Screen.PrimaryScreen.WorkingArea.Width;

            lblInfo.Location = new Point(X0 + 2 * XMax + 40, Y0);

            // начальные значения для m_SightDirection и m_DistanceD (берутся от балды)
            /*m_nx = 1;
            m_ny = 1;
            m_nz = 0;*/
            m_nx = 0.1;
            m_ny = 0.1;
            m_nz = 1;
            m_SightDirection = new Vector3D(m_nx, m_ny, m_nz);
            m_SightDirection.Normalize();
            m_DistanceD = 8;
            // значения для m_MaxHorizSightAngle и m_MaxVertSightAngle (требуют подбора)
            m_MaxHorizSightAngle = 45;
            m_MaxHorizSightAngle *= Math.PI / 180;
            m_MaxVertSightAngle = 45;
            m_MaxVertSightAngle *= Math.PI / 180;

            m_CurrentSideList = m_AC.GetSideList();
            m_LineList = new List<Line2D>();
            FillLineList(m_LineList);
        }

        private void nextIterationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_AC.NextSolutionIteration();
            //MessageBox.Show("SideCount = " + m_CurrentSideList.Count.ToString());

            m_CurrentSideList = m_AC.GetSideList();
            FillLineList(m_LineList);
            this.Invalidate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush WhiteBrush = new SolidBrush(Color.White);
            e.Graphics.FillRectangle(WhiteBrush, X0, Y0, 2 * XMax, 2 * YMax);
            Pen BlackPen = new Pen(Color.Black);
            foreach (Line2D CurrentLine in m_LineList)
            {
                e.Graphics.DrawLine(BlackPen, CurrentLine.x1, CurrentLine.y1, CurrentLine.x2, CurrentLine.y2);
            }

            Pen BluePen = new Pen(Color.Blue);
            e.Graphics.DrawRectangle(BluePen, X0, Y0, 2 * XMax, 2 * YMax);

            lblInfo.Text = "InverseTime = " + Math.Round(m_AC.InverseTime, 4).ToString() + "\nSide's count = " + m_CurrentSideList.Count.ToString();
        }

        private void ChangeSightParamstoolStripMenuItem_Click(object sender, EventArgs e)
        {
            SightParamForm spf = new SightParamForm();
            spf.nx = m_nx;
            spf.ny = m_ny;
            spf.nz = m_nz;
            spf.Distance = m_DistanceD;

            if (spf.ShowDialog() == DialogResult.OK)
            {
                m_nx = m_SightDirection.XCoord = spf.nx;
                m_ny = m_SightDirection.YCoord = spf.ny;
                m_nz = m_SightDirection.ZCoord = spf.nz;
                m_SightDirection.Normalize();

                m_DistanceD = spf.Distance;

                FillLineList(m_LineList);
                this.Invalidate();
            }
        }

        private void SaveDatatoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<SideClass> SideList = m_AC.GetSideList();

                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, false))
                {
                    foreach (SideClass CurrentSide in SideList)
                    {
                        sw.WriteLine("{0} {1} {2}", CurrentSide.SideNormal.XCoord, CurrentSide.SideNormal.YCoord, CurrentSide.SideNormal.ZCoord);

                        /*for (Int32 VertexIndex = CurrentSide.VertexCount - 1; VertexIndex >= 0; VertexIndex--)
                        {
                            VertexClass CurrentVertex = CurrentSide[VertexIndex];
                            
                            sw.Write("{0} {1} {2}", CurrentVertex.XCoord, CurrentVertex.YCoord, CurrentVertex.ZCoord);
                            if (VertexIndex > 0)
                            {
                                sw.Write(' ');
                            }
                        }*/
                        for (Int32 VertexIndex = 0; VertexIndex < CurrentSide.VertexCount; VertexIndex++)
                        {
                            VertexClass CurrentVertex = CurrentSide[VertexIndex];

                            sw.Write("{0} {1} {2}", CurrentVertex.XCoord, CurrentVertex.YCoord, CurrentVertex.ZCoord);
                            if (VertexIndex < (CurrentSide.VertexCount - 1))
                            {
                                sw.Write(' ');
                            }
                        }
                        sw.WriteLine();
                    }
                }
            }
        }

        private void SaveData2toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<VertexClass> VertexList = m_AC.GetVertexList();
                List<SideClass> SideList = m_AC.GetSideList();

                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, false))
                {
                    // сначало записываем координаты вершин
                    foreach (VertexClass CurrentVertex in VertexList)
                    {
                        String XCoord = CurrentVertex.XCoord.ToString(CultureInfo.InvariantCulture);
                        String YCoord = CurrentVertex.YCoord.ToString(CultureInfo.InvariantCulture);
                        String ZCoord = CurrentVertex.ZCoord.ToString(CultureInfo.InvariantCulture);

                        sw.WriteLine("{0} {1} {2}", XCoord, YCoord, ZCoord);
                    }
                    sw.WriteLine();
                    // потом для каждой грани сначало координаты внешней нормали,
                    // потом список индексов вершин, составляющих грань (упорядоченных против ч.с.)
                    foreach (SideClass CurrentSide in SideList)
                    {
                        String SideNormalXCoord = CurrentSide.SideNormal.XCoord.ToString(CultureInfo.InvariantCulture);
                        String SideNormalYCoord = CurrentSide.SideNormal.YCoord.ToString(CultureInfo.InvariantCulture);
                        String SideNormalZCoord = CurrentSide.SideNormal.ZCoord.ToString(CultureInfo.InvariantCulture);

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
            }
        }
    }
}