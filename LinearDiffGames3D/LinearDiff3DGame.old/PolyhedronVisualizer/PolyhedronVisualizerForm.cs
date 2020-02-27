using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MathPostgraduateStudy.LinearDiff3DGame;

namespace PolyhedronVisualizer
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

    public partial class PolyhedronVisualizerForm : Form
    {
        /// <summary>
        /// m_PolyhedronVertexList - ������ ���� ������ �������������
        /// </summary>
        private List<VertexClass> m_PolyhedronVertexList;
        /// <summary>
        /// m_PolyhedronSideList - ������ ���� ������ �������������
        /// </summary>
        private List<SideClass> m_PolyhedronSideList;
        /// <summary>
        /// Epsilon - ...
        /// </summary>
        private readonly Double Epsilon = 1e-9;

        /// <summary>
        /// m_SightDirection - ������, �������� ����������� ������� (��������� � ����� ���� Y' � Y")
        /// </summary>
        private Vector3D m_SightDirection;
        /// <summary>
        /// m_DistanceD - ���������� ����� ������� O' � O" (��. ��������)
        /// </summary>
        private Double m_DistanceD;
        /// <summary>
        /// m_MaxHorizSightAngle - ������������ ���� ������ �� ����������� (� ��������� X"O"Y")
        /// </summary>
        private Double m_MaxHorizSightAngle;
        /// <summary>
        /// m_MaxVertSightAngle - ������������ ���� ������ �� ��������� (� ��������� Z"O"Y")
        /// </summary>
        private Double m_MaxVertSightAngle;
        /// <summary>
        /// m_InverseCoordTransformMatrix - �������� ������� �������� �� ��(XYZ) � ��(X'Y'Z')
        /// </summary>
        private Matrix m_InverseCoordTransformMatrix;

        /// <summary>
        /// 
        /// </summary>
        private List<Line2D> m_LineList;

        /// <summary>
        /// 
        /// </summary>
        private readonly Int32 X0 = 100;
        /// <summary>
        /// 
        /// </summary>
        private readonly Int32 Y0 = 100;
        /// <summary>
        /// 
        /// </summary>
        private readonly Int32 XMax = 200;
        /// <summary>
        /// 
        /// </summary>
        private readonly Int32 YMax = 200;
        
        /// <summary>
        /// ����� CalcDeterminant3 ��������� ������������ ������� MatrixA �������� 3x3
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
        /// ����� CalcAlgebraicAddition ��������� �������������� ����������
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
        /// ����� InverseMatrix3 ���������� �������� ������� ��� ������� MatrixA
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
        /// ����� FillVertexList ��������� ����� ������ m_PolyhedronVertexList �� ��������� ������� ��������� ������
        /// </summary>
        /// <param name="VertexList"></param>
        private void FillVertexList(Point3D[] VertexList)
        {
            for (Int32 VertexIndex = 0; VertexIndex < VertexList.Length; VertexIndex++)
            {
                m_PolyhedronVertexList.Add(new VertexClass(VertexList[VertexIndex], VertexIndex + 1));
            }
        }

        /// <summary>
        /// ����� GetSideCount4Edge ���������� ���������� ������, ������� ����������� �������� (�� 2 ��������) �����
        /// </summary>
        /// <param name="EdgeVertex1">������ ������� �����</param>
        /// <param name="EdgeVertex2">������ ������� �����</param>
        /// <returns>���������� ������, ������� ����������� �������� �����</returns>
        private Int32 GetSideCount4Edge(VertexClass EdgeVertex1, VertexClass EdgeVertex2)
        {
            Int32 SideCount4Edge = 0;

            for (Int32 EdgeVertex1SideListIndex = 0; EdgeVertex1SideListIndex < EdgeVertex1.SideCount; EdgeVertex1SideListIndex++)
            {
                if (EdgeVertex1[EdgeVertex1SideListIndex].HasVertex(EdgeVertex2)) SideCount4Edge++;
            }

            return SideCount4Edge;
        }

        /// <summary>
        /// ����� GetSideExternalNormal ���������� "�������" (�.�. ������������ ������ �������������) ������� � �����
        /// ����� �������� ����� ��������� Vertex1, Vertex2 � Vertex3
        /// </summary>
        /// <param name="Vertex1">������� ����� Vertex1</param>
        /// <param name="Vertex2">������� ����� Vertex2</param>
        /// <param name="Vertex3">������� ����� Vertex3</param>
        /// <returns>"�������" ������� � �����, �������� ����� ��������� Vertex1, Vertex2 � Vertex3</returns>
        private Vector3D GetSideExternalNormal(VertexClass Vertex1, VertexClass Vertex2, VertexClass Vertex3)
        {
            Vector3D ExternalNormal = Vector3D.ZeroVector3D;

            if ((Object.ReferenceEquals(Vertex1, Vertex2)) || (Object.ReferenceEquals(Vertex1, Vertex3)) || (Object.ReferenceEquals(Vertex2, Vertex3)))
            {
                throw new ArgumentException("Vertex1, Vertex2, Vertex3 must be different");
            }

            // ������� � ��������� ��������� ����� ��������� ���������, ���������� ����� 3 �����
            // A = (y2-y1)*(z3-z1)-(z2-z1)*(y3-y1)
            ExternalNormal.XCoord = (Vertex2.YCoord - Vertex1.YCoord) * (Vertex3.ZCoord - Vertex1.ZCoord) -
                                    (Vertex2.ZCoord - Vertex1.ZCoord) * (Vertex3.YCoord - Vertex1.YCoord);
            // B = (z2-z1)*(x3-x1)-(x2-x1)*(z3-z1)
            ExternalNormal.YCoord = (Vertex2.ZCoord - Vertex1.ZCoord) * (Vertex3.XCoord - Vertex1.XCoord) -
                                    (Vertex2.XCoord - Vertex1.XCoord) * (Vertex3.ZCoord - Vertex1.ZCoord);
            // C = (x2-x1)*(y3-y1)-(y2-y1)*(x3-x1)
            ExternalNormal.ZCoord = (Vertex2.XCoord - Vertex1.XCoord) * (Vertex3.YCoord - Vertex1.YCoord) -
                                    (Vertex2.YCoord - Vertex1.YCoord) * (Vertex3.XCoord - Vertex1.XCoord);

            // ��������� ���������� �������            
            ExternalNormal.Normalize();

            // �������� ������-������ �� ����� ����� �� ������� �� ��������� � ����� ����� �� ���������
            // �.�. �� �������� � �������� ��������������, �� ���������� ������������ ������� ������� � ������������ ������-������� ������ ���� >0 (��� ��������)
            Double ScalarProduct = 0;
            // Math.Abs(ScalarProduct) < Epsilon ???
            //for (Int32 VertexIndex = 0; ((ScalarProduct == 0) && (VertexIndex < m_PolyhedronVertexList.Count)); VertexIndex++)
            for (Int32 VertexIndex = 0; ((Math.Abs(ScalarProduct) < Epsilon) && (VertexIndex < m_PolyhedronVertexList.Count)); VertexIndex++)
            {
                VertexClass CurrentVertex = m_PolyhedronVertexList[VertexIndex];
                ScalarProduct = ExternalNormal.XCoord * (Vertex1.XCoord - CurrentVertex.XCoord) +
                                ExternalNormal.YCoord * (Vertex1.YCoord - CurrentVertex.YCoord) +
                                ExternalNormal.ZCoord * (Vertex1.ZCoord - CurrentVertex.ZCoord);
            }
            // Math.Abs(ScalarProduct) < Epsilon ???
            //if (ScalarProduct == 0)
            if (Math.Abs(ScalarProduct) < Epsilon)
            {
                // some exception !!!
            }
            if (ScalarProduct < 0)
            {
                ExternalNormal.XCoord *= -1;
                ExternalNormal.YCoord *= -1;
                ExternalNormal.ZCoord *= -1;
            }

            return ExternalNormal;
        }

        /// <summary>
        /// ����� DoesVertexesFormSide ��������� �������� �� ����� (��������� �������������) ����� � ������� (��� �������)
        /// ��� ��� �����, ������������� ����������� ����� ��������� � ������ ������ ���� �����
        /// ������ ������ ����� - SideVertexList; ������ ��� ������� - ������� (����� � �������), �� ������� �������� �����
        /// </summary>
        /// <param name="SideVertexList">������ ������ �����; ������ ��� �������� - ������� (����� � �������), �� ������� �������� �����</param>
        /// <returns>true - ���� ����� � ������� ��������� �����, ����� false</returns>
        private Boolean DoesVertexesFormSide(List<VertexClass> SideVertexList)
        {
            Boolean CheckResult = true;

            Vector3D SideVector1 = Vector3D.ZeroVector3D;
            Vector3D SideVector2 = Vector3D.ZeroVector3D;

            // ����� ������ ����� �� ������ ����� ����� ������� (������ O)
            SideVector1.XCoord = SideVertexList[1].XCoord - SideVertexList[0].XCoord;
            SideVector1.YCoord = SideVertexList[1].YCoord - SideVertexList[0].YCoord;
            SideVector1.ZCoord = SideVertexList[1].ZCoord - SideVertexList[0].ZCoord;
            SideVector2.XCoord = SideVertexList[2].XCoord - SideVertexList[0].XCoord;
            SideVector2.YCoord = SideVertexList[2].YCoord - SideVertexList[0].YCoord;
            SideVector2.ZCoord = SideVertexList[2].ZCoord - SideVertexList[0].ZCoord;

            Int32 FirstMixedProductValueSign = 0;
            foreach (VertexClass CurrentVertex in m_PolyhedronVertexList)
            {
                if ((Object.ReferenceEquals(CurrentVertex, SideVertexList[0])) || (Object.ReferenceEquals(CurrentVertex, SideVertexList[1])) || (Object.ReferenceEquals(CurrentVertex, SideVertexList[2])))
                {
                    continue;
                }

                Vector3D CurrentVector = Vector3D.ZeroVector3D;
                CurrentVector.XCoord = CurrentVertex.XCoord - SideVertexList[0].XCoord;
                CurrentVector.YCoord = CurrentVertex.YCoord - SideVertexList[0].YCoord;
                CurrentVector.ZCoord = CurrentVertex.ZCoord - SideVertexList[0].ZCoord;

                // vector a = CurrentVector, vector b = SideVector1, vector c = SideVector2
                Double CurrentMixedProduct = Vector3D.MixedProduct(CurrentVector, SideVector1, SideVector2);

                // Math.Abs(MixedProduct) < Eps ???
                // if (CurrentMixedProduct == 0)
                if (Math.Abs(CurrentMixedProduct) < Epsilon)
                {
                    SideVertexList.Add(CurrentVertex);
                }
                else
                {
                    if (FirstMixedProductValueSign == 0)
                    {
                        FirstMixedProductValueSign = Math.Sign(CurrentMixedProduct);
                    }
                    else if (FirstMixedProductValueSign != Math.Sign(CurrentMixedProduct))
                    {
                        CheckResult = false;
                        break;
                    }
                }
            }

            return CheckResult;
        }

        /// <summary>
        /// ����� OrderingSideVertexList ������������� ������� �����, �������� � ������ ��������������� ������ SideVertexList
        /// ������� ��������������� ���, ����� ��� ��������� ������ �.�. ���� �������� �� ����� � ����� "�������" ������� � �����
        /// ��� ���� ������ ��������������� ������ SideVertexList, ������������ � ����� OrderingSideVertexList �������� ��� ���������
        /// </summary>
        /// <param name="SideVertexList">������ ��������������� ������</param>
        /// <returns>������ ������������� ������</returns>
        private List<VertexClass> OrderingSideVertexList(List<VertexClass> SideVertexList)
        {
            // "�������" ������� � ������� ����� (������� ������� �� �������������)
            Vector3D SideNormalVector = GetSideExternalNormal(SideVertexList[0], SideVertexList[1], SideVertexList[2]);

            // ���������� ����������� ��������� ������. ������������ ��� ������ ?
            List<VertexClass> DisorderSideVertexList = new List<VertexClass>(SideVertexList);
            List<VertexClass> OrderSideVertexList = new List<VertexClass>(DisorderSideVertexList.Count);

            OrderSideVertexList.Add(DisorderSideVertexList[0]);
            DisorderSideVertexList.RemoveAt(0);

            while (DisorderSideVertexList.Count > 0)
            {
                VertexClass NextAddedVertex = null;
                Int32 NextAddedVertexIndex = -1;
                VertexClass LastAddedVertex = OrderSideVertexList[OrderSideVertexList.Count - 1];

                // ���������� ������ ������, ���������� ����� ��������� ����������� � ��������� ��� ���������� �������
                Vector3D LineFormingVector = Vector3D.ZeroVector3D;
                // "������" ������� (������� � ������� �����) � ������, ���������� ����� ��������� ����������� � ��������� ��� ���������� �������
                Vector3D LineNormalVector = Vector3D.ZeroVector3D;

                for (Int32 DisorderedVertexIndex = 0; DisorderedVertexIndex < DisorderSideVertexList.Count; DisorderedVertexIndex++)
                {
                    VertexClass DisorderedVertex = DisorderSideVertexList[DisorderedVertexIndex];
                    // ��������� ������������ "������" ������� � ������ � ������ �������, ������������ �� ��������� ����������� � ������� �������
                    Double ScalarProduct = LineNormalVector.XCoord * (DisorderedVertex.XCoord - LastAddedVertex.XCoord) +
                                           LineNormalVector.YCoord * (DisorderedVertex.YCoord - LastAddedVertex.YCoord) +
                                           LineNormalVector.ZCoord * (DisorderedVertex.ZCoord - LastAddedVertex.ZCoord);

                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    if (ScalarProduct >= 0)
                    {
                        NextAddedVertex = DisorderedVertex;
                        NextAddedVertexIndex = DisorderedVertexIndex;

                        LineFormingVector.XCoord = NextAddedVertex.XCoord - LastAddedVertex.XCoord;
                        LineFormingVector.YCoord = NextAddedVertex.YCoord - LastAddedVertex.YCoord;
                        LineFormingVector.ZCoord = NextAddedVertex.ZCoord - LastAddedVertex.ZCoord;
                        LineFormingVector.Normalize();

                        // "������" ������� � ������ �������� ��� ������ ���������� ������������ ����������� ������� ������ �� "�������" ������� � ������� ����� (???????????????)
                        LineNormalVector = Vector3D.VectorProduct(LineFormingVector, SideNormalVector);
                    }
                }

                OrderSideVertexList.Add(NextAddedVertex);
                DisorderSideVertexList.RemoveAt(NextAddedVertexIndex);
            }

            return OrderSideVertexList;
        }

        /// <summary>
        /// ����� IsSideAlreadyAdded ��������� ���� �� ��������� ����������� ����� � ������ ������
        /// ��� �������� ������������ ������� ������
        /// </summary>
        /// <param name="SideNormalVector">������� ����������� �����</param>
        /// <returns>true, ���� ����� ��������� � ������ ������, ����� false</returns>
        private Boolean IsSideAlreadyAdded(Vector3D SideNormalVector)
        {
            Boolean CheckResult = false;

            // ��� �������� ���������� "�������" ������� ����������� ����� � "��������" ��������� ��� ����������� � ������ ������
            // ��� ��������, �� ���� ���� �� ������������ ���-�������, �� ��� ����� ���������� �������� ��������� 2-3 ����� � ����������� ��������
            // ��-�� ����, ��� ������� ���������� �� ������ �������� � ��-�� ������������ ���������� ������� ����� � ��� �� ��������� ����� �� ��������
            // (��� ������ � ���-������� ������������ ������ ��������� �� ��������, � �� ������������ ��� � ��� �����)
            foreach (SideClass Side in m_PolyhedronSideList)
            {
                if (Side.SideNormal.ApproxEquals(SideNormalVector))
                {
                    CheckResult = true;
                    break;
                }
            }

            return CheckResult;
        }

        /// <summary>
        /// ����� GetFirstSide ������ � ���������� ������ ����� ������������ (��. ��������)
        /// </summary>
        /// <returns>������ ����� �������������</returns>
        private SideClass GetFirstSide()
        {
            SideClass FirstSide = null;

            VertexClass Vertex1 = null;
            VertexClass Vertex2 = null;
            VertexClass Vertex3 = null;

            List<VertexClass> CheckedSideVertexList = new List<VertexClass>();

            Vertex1 = m_PolyhedronVertexList[0];
            for (Int32 Vertex2Index = 1; Vertex2Index < m_PolyhedronVertexList.Count; Vertex2Index++)
            {
                // �����: Vertex1 - Vertex2
                Vertex2 = m_PolyhedronVertexList[Vertex2Index];

                for (Int32 Vertex3Index = 1; Vertex3Index < m_PolyhedronVertexList.Count; Vertex3Index++)
                {
                    if (Vertex2Index == Vertex3Index) continue;

                    Vertex3 = m_PolyhedronVertexList[Vertex3Index];
                    CheckedSideVertexList.Clear();
                    CheckedSideVertexList.Add(Vertex1);
                    CheckedSideVertexList.Add(Vertex2);
                    CheckedSideVertexList.Add(Vertex3);

                    Boolean CheckResult = DoesVertexesFormSide(CheckedSideVertexList);
                    if (CheckResult)
                    {
                        // ������������� ������ ������ �����
                        List<VertexClass> OrderedSideVertexList = OrderingSideVertexList(CheckedSideVertexList);
                        // "�������" ������� � �����
                        Vector3D FirstSideNormalVector = GetSideExternalNormal(Vertex1, Vertex2, Vertex3);
                        FirstSide = new SideClass(OrderedSideVertexList, 1, FirstSideNormalVector);

                        // �� ����� �������� ����� �� ������ !!!!!!!!!
                        return FirstSide;
                    }
                }
            }

            return FirstSide;
        }

        /// <summary>
        /// ����� RecievePolyhedronStructure �������� (���������������) ��������� �������������
        /// </summary>
        private void RecievePolyhedronStructure()
        {
            SideClass FirstSide = GetFirstSide();
            m_PolyhedronSideList.Add(FirstSide);

            List<VertexClass> CheckedSideVertexList = new List<VertexClass>();

            // ���� �� ���� ������ �� ������ ������
            for (Int32 SideIndex = 0; SideIndex < m_PolyhedronSideList.Count; SideIndex++)
            {
                SideClass CurrentSide = m_PolyhedronSideList[SideIndex];

                // ���� �� ���� ������ ������� �����
                for (Int32 CurrentSideVertexIndex = 0; CurrentSideVertexIndex < CurrentSide.VertexCount; CurrentSideVertexIndex++)
                {
                    VertexClass LeftEdgeVertex = CurrentSide[CurrentSideVertexIndex];
                    VertexClass RightEdgeVertex = (CurrentSideVertexIndex == (CurrentSide.VertexCount - 1) ? CurrentSide[0] : CurrentSide[CurrentSideVertexIndex + 1]);

                    // CurrentEdge: LeftEdgeVertex-RightEdgeVertex
                    Int32 SideCount4CurrentEdge = GetSideCount4Edge(LeftEdgeVertex, RightEdgeVertex);
                    // if ((SideCount4CurrentEdge != 1) && (SideCount4CurrentEdge != 2)) !!!!!
                    if (SideCount4CurrentEdge == 2) continue;

                    // ���� �� ���� ��������
                    for (Int32 VertexIndex = 0; VertexIndex < m_PolyhedronVertexList.Count; VertexIndex++)
                    {
                        VertexClass CurrentVertex = m_PolyhedronVertexList[VertexIndex];

                        if ((Object.ReferenceEquals(LeftEdgeVertex, CurrentVertex)) || (Object.ReferenceEquals(RightEdgeVertex, CurrentVertex)))
                        {
                            continue;
                        }

                        CheckedSideVertexList.Clear();
                        CheckedSideVertexList.Add(LeftEdgeVertex);
                        CheckedSideVertexList.Add(RightEdgeVertex);
                        CheckedSideVertexList.Add(CurrentVertex);

                        Boolean CheckResult = DoesVertexesFormSide(CheckedSideVertexList);
                        if (CheckResult)
                        {
                            // "�������" ������� � �����
                            Vector3D SideNormalVector = GetSideExternalNormal(LeftEdgeVertex, RightEdgeVertex, CurrentVertex);
                            if (IsSideAlreadyAdded(SideNormalVector)) continue;

                            // ������������� ������ ������ �����
                            List<VertexClass> OrderedSideVertexList = OrderingSideVertexList(CheckedSideVertexList);
                            // ID �����
                            Int32 NewSideID = m_PolyhedronSideList[m_PolyhedronSideList.Count - 1].ID + 1;

                            SideClass NewSide = new SideClass(OrderedSideVertexList, NewSideID, SideNormalVector);
                            m_PolyhedronSideList.Add(NewSide);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ����� GetCoordTransformMatrix ���������� ������� �������� �� ��(XYZ) � ��(X'Y'Z')
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
        /// ����� TranformPointCoords ��� ����� � ������������ � ��(XYZ) ���������� �� ���������� � ��(X"Y"Z")
        /// </summary>
        /// <param name="XCoord"></param>
        /// <param name="YCoord"></param>
        /// <param name="ZCoord"></param>
        /// <returns></returns>
        private Point3D TransformPointCoords(Double XCoord, Double YCoord, Double ZCoord)
        {
            Matrix OldCoordMatrix = new Matrix(1, 3);
            // ���������� ����� � ��(XYZ)
            OldCoordMatrix[1, 1] = XCoord;
            OldCoordMatrix[1, 2] = YCoord;
            OldCoordMatrix[1, 3] = ZCoord;

            // ���������� ����� ����� �������� �� - � �� (X'Y'Z')
            Matrix NewCoordMatrix = OldCoordMatrix * m_InverseCoordTransformMatrix;
            // ���������� ����� ����� �������� � ������ �� - � ��(X"Y"Z")
            //NewCoordMatrix[1, 1] = +m_DistanceD * m_SightDirection.XCoord;
            //NewCoordMatrix[1, 2] = +m_DistanceD * m_SightDirection.YCoord;
            NewCoordMatrix[1, 2] = +m_DistanceD;
            //NewCoordMatrix[1, 3] = +m_DistanceD * m_SightDirection.ZCoord;

            return new Point3D(NewCoordMatrix[1, 1], NewCoordMatrix[1, 2], NewCoordMatrix[1, 3]);
        }

        /// <summary>
        /// ����� IsPointOutOfSight ���������� true, ���� ����� �� ��������� �������������� ��������� �� ��������� (��. ��������)
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

            foreach (SideClass CurrentSide in m_PolyhedronSideList)
            {
                // ���� ��������� ������������ ������� ����������� ������� � ������� ����� >=0, �� ��� ������, ��� �� ��� ����� �� �����
                if (Vector3D.ScalarProduct(m_SightDirection, CurrentSide.SideNormal) >= 0)
                {
                    continue;
                }

                for (Int32 VertexIndex = 0; VertexIndex < CurrentSide.VertexCount; VertexIndex++)
                {
                    VertexClass CurrentVertex = CurrentSide[VertexIndex];
                    VertexClass NextVertex = (VertexIndex == CurrentSide.VertexCount - 1 ? CurrentSide[0] : CurrentSide[VertexIndex + 1]);

                    // ���������� ������ ����� �������� � ������ �� - � ��(X"Y"Z")
                    Point3D CurrentVertexNewCoords = TransformPointCoords(CurrentVertex.XCoord, CurrentVertex.YCoord, CurrentVertex.ZCoord);
                    Point3D NextVertexNewCoords = TransformPointCoords(NextVertex.XCoord, NextVertex.YCoord, NextVertex.ZCoord);

                    // ������������� ���������� ����� �� ��������� (��. ��������)
                    Double X1R = CurrentVertexNewCoords.XCoord / (CurrentVertexNewCoords.YCoord * Math.Tan(m_MaxHorizSightAngle));
                    Double Z1R = CurrentVertexNewCoords.ZCoord / (CurrentVertexNewCoords.YCoord * Math.Tan(m_MaxVertSightAngle));
                    Double X2R = NextVertexNewCoords.XCoord / (NextVertexNewCoords.YCoord * Math.Tan(m_MaxHorizSightAngle));
                    Double Z2R = NextVertexNewCoords.ZCoord / (NextVertexNewCoords.YCoord * Math.Tan(m_MaxVertSightAngle));

                    // ��� ����� �� ��������� �������������� ��������� �� ��������� (��. ��������)
                    if (IsPointOutOfSight(X1R, Z1R) && IsPointOutOfSight(X2R, Z2R))
                    {
                        continue;
                    }
                    // ������ ����� �� ��������� �������������� ��������� �� ��������� (��. ��������)
                    if (IsPointOutOfSight(X1R, Z1R))
                    {
                    }
                    // ������ ����� �� ��������� �������������� ��������� �� ��������� (��. ��������)
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

        public PolyhedronVisualizerForm()
        {
            InitializeComponent();

            m_PolyhedronVertexList = new List<VertexClass>();
            m_PolyhedronSideList = new List<SideClass>();

            /*Point3D[] VertexList = new Point3D[8];
            VertexList[0] = new Point3D(1, 1, 1);
            VertexList[1] = new Point3D(-1, 1, 1);
            VertexList[2] = new Point3D(-1, -1, 1);
            VertexList[3] = new Point3D(1, -1, 1);
            VertexList[4] = new Point3D(1, 1, -1);
            VertexList[5] = new Point3D(-1, 1, -1);
            VertexList[6] = new Point3D(-1, -1, -1);
            VertexList[7] = new Point3D(1, -1, -1);*/
            /*Point3D[] VertexList = new Point3D[5];
            VertexList[0] = new Point3D(0, 0, 0);
            VertexList[1] = new Point3D(1, 1, 2);
            VertexList[2] = new Point3D(-1, 1, 2);
            VertexList[3] = new Point3D(-1, -1, 2);
            VertexList[4] = new Point3D(1, -1, 2);*/
            Point3D[] VertexList = new Point3D[8];
            VertexList[0] = new Point3D(1, 1, 1);
            VertexList[1] = new Point3D(-1, 1, 1);
            VertexList[2] = new Point3D(-1, -1, 1);
            VertexList[3] = new Point3D(1, -1, 1);
            VertexList[4] = new Point3D(2, 2, -1);
            VertexList[5] = new Point3D(-2, 2, -1);
            VertexList[6] = new Point3D(-2, -2, -1);
            VertexList[7] = new Point3D(2, -2, -1);


            FillVertexList(VertexList);
            RecievePolyhedronStructure();

            // ��������� �������� ��� m_SightDirection � m_DistanceD (������� �� �����)
            m_SightDirection = new Vector3D(0.1, 0.1, -0.5);
            m_SightDirection.Normalize();
            m_DistanceD = 5;
            // �������� ��� m_MaxHorizSightAngle � m_MaxVertSightAngle (������� �������)
            m_MaxHorizSightAngle = 45;
            m_MaxHorizSightAngle *= Math.PI / 180;
            m_MaxVertSightAngle = 45;
            m_MaxVertSightAngle *= Math.PI / 180;

            m_InverseCoordTransformMatrix = InverseMatrix3(GetCoordTransformMatrix());

            m_LineList = new List<Line2D>();
            FillLineList(m_LineList);

            this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;
        }

        private void PolyhedronVisualizerForm_Paint(object sender, PaintEventArgs e)
        {
            Pen BlackPen = new Pen(Color.Black);
            foreach (Line2D CurrentLine in m_LineList)
            {
                e.Graphics.DrawLine(BlackPen, CurrentLine.x1, CurrentLine.y1, CurrentLine.x2, CurrentLine.y2);
            }

            Pen BluePen = new Pen(Color.Blue);
            e.Graphics.DrawRectangle(BluePen, X0, Y0, X0 + 2 * XMax, Y0 + 2 * YMax);
        }
    }
}