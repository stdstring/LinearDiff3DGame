//#define TASK1
//#define TASK2

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MathPostgraduateStudy.LinearDiff3DGame
{
    /// <summary>
    /// 
    /// </summary>
    public class AlgorithmException : ApplicationException
    {
        /*public AlgorithmException() : this("Exception raises in algorithm", null)
        {
        }*/

        public AlgorithmException(String Message) : this(Message, null)
        {
        }

        public AlgorithmException(String Message, Exception innerException) : base(Message, innerException)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SolutionNotExistException : ApplicationException
    {
        public SolutionNotExistException() : this("Solution does not exist", null)
        {
        }

        public SolutionNotExistException(String Message) : this(Message, null)
        {
        }

        public SolutionNotExistException(String Message, Exception innerException) : base(Message, innerException)
        {
        }
    }

    public class AlgorithmClass
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
        /// m_PGNodeList - ������ ���� ����� ����� ������������� (�� ���� ���� ��� ���� �������������)
        /// </summary>
        private List<PolyhedronGraphNode> m_PGNodeList;
        /// <summary>
        /// m_Pi1DirectingVector - ������������ ������ ������� Pi1
        /// </summary>
        private Vector3D m_Pi1DirectingVector;
        /// <summary>
        /// m_Qi1DirectingVector - ������������ ������ ������� Qi1
        /// </summary>
        private Vector3D m_Qi1DirectingVector;
        /// <summary>
        /// m_Pi1Set - ��������� (�������) Pi1
        /// </summary>
        private List<Point3D> m_Pi1Set;
        /// <summary>
        /// m_Qi1Set - ��������� (�������) Qi1
        /// </summary>
        private List<Point3D> m_Qi1Set;
        /// <summary>
        /// m_Pi2DirectingVector - ������������ ������ ������� Pi2
        /// </summary>
        private Vector3D m_Pi2DirectingVector;
        /// <summary>
        /// m_Qi2DirectingVector - ������������ ������ ������� Qi2
        /// </summary>
        private Vector3D m_Qi2DirectingVector;
        /// <summary>
        /// m_Pi2Set - ��������� (�������) Pi2
        /// </summary>
        private List<Point3D> m_Pi2Set;
        /// <summary>
        /// m_Qi2Set - ��������� (�������) Qi2
        /// </summary>
        private List<Point3D> m_Qi2Set;
        /// <summary>
        /// m_CurrentInverseTime - ������� �������� �����
        /// </summary>
        private Double m_CurrentInverseTime;
        /// <summary>
        /// m_DeltaT - ...
        /// </summary>
        private readonly Double m_DeltaT = 0.1;
        #if TASK1        
        /// <summary>
        /// m_Mp1 - ...
        /// </summary>
        private readonly Double m_Mp1 = 2;
        /// <summary>
        /// m_Mp2 - ...
        /// </summary>
        private readonly Double m_Mp2 = 1;
        /// <summary>
        /// m_Mq1 - ...
        /// </summary>
        private readonly Double m_Mq1 = 2;
        /// <summary>
        /// m_Mq2 - ...
        /// </summary>
        private readonly Double m_Mq2 = 1;
        #elif TASK2
        /// <summary>
        /// m_Mp1 - ...
        /// </summary>
        private readonly Double m_Mp1 = 0.7;
        /// <summary>
        /// m_Mp2 - ...
        /// </summary>
        private readonly Double m_Mp2 = 0.17;
        /// <summary>
        /// m_Mq1 - ...
        /// </summary>
        private readonly Double m_Mq1 = 22;
        /// <summary>
        /// m_Mq2 - ...
        /// </summary>
        private readonly Double m_Mq2 = 18;
        #endif
        /// <summary>
        /// m_Mp - ...
        /// </summary>
        private readonly Double m_Mp1 = 1;
        /// <summary>
        /// m_Mq - ...
        /// </summary>
        private readonly Double m_Mq1 = 1;
        /// <summary>
        /// Epsilon - ...
        /// </summary>
        private readonly Double Epsilon = 1e-9;
        /// <summary>
        /// PointCoordDigits - ...
        /// </summary>
        private readonly Int32 PointCoordDigits = 9;
        /// <summary>
        /// MinVectorDistinguishAngle - ����������� �������� ���� ����� ���������, ��� ������� �� ������� ��� ������� ���������� (�� ��������)
        /// </summary>
        private readonly Double MinVectorDistinguishAngle = 0.01;
        /// <summary>
        /// m_FundKoshiMatrix - �������� ��������������� ������� ���� � ������ ��������� ������� m_CurrentInverseTime;
        /// </summary>
        private FundKoshiMatrix m_FundKoshiMatrix;

        /// <summary>
        /// 
        /// </summary>
        private Matrix m_MatrixA;
        /// <summary>
        /// 
        /// </summary>
        private Matrix m_MatrixB;
        /// <summary>
        /// 
        /// </summary>
        private Matrix m_MatrixC;

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
        /// ����� RecievePolyhedronGraph ������� (��������) ���� �������������
        /// </summary>
        private void RecievePolyhedronGraph()
        {
            foreach (SideClass CurrentSide in m_PolyhedronSideList)
            {
                m_PGNodeList.Add(new PolyhedronGraphNode(CurrentSide.ID, CurrentSide.SideNormal));
            }

            // ���� �� ���� ������ �� ������ ������
            foreach (SideClass CurrentSide in m_PolyhedronSideList)
            {
                // ���� �� ���� ������ ������� �����
                for (Int32 VertexIndex = 0; VertexIndex < CurrentSide.VertexCount; VertexIndex++)
                {
                    VertexClass LeftEdgeVertex = CurrentSide[VertexIndex];
                    VertexClass RightEdgeVertex = (VertexIndex == (CurrentSide.VertexCount - 1) ? CurrentSide[0] : CurrentSide[VertexIndex + 1]);

                    SideClass NeighbourSide = CurrentSide.GetNeighbourSide(LeftEdgeVertex, RightEdgeVertex);

                    Int32 CurrentPGNodeIndex = CurrentSide.ID - 1;
                    Int32 NeighbourPGNodeIndex = NeighbourSide.ID - 1;
                    PolyhedronGraphNode CurrentPGNode = m_PGNodeList[CurrentPGNodeIndex];
                    PolyhedronGraphNode NeighbourPGNode = m_PGNodeList[NeighbourPGNodeIndex];

                    CurrentPGNode.AddNodeConnection(NeighbourPGNode);
                }
            }
        }

        /// <summary>
        /// ����� GetShortestGraphPath ������ (� ���������� � ���� ������ �����) ������������� ���� ������ (��. ��������)
        /// </summary>
        /// <param name="StartPGNode">�������� ����</param>
        /// <param name="StartPGConnNode">�����, ������������ ������ ����</param>
        /// <param name="FinishPGConnNode">�����, ������������ ����� ����</param>
        /// <returns></returns>
        private List<PolyhedronGraphNode> GetShortestGraphPath(PolyhedronGraphNode StartPGNode, PolyhedronGraphNode StartPGConnNode, PolyhedronGraphNode FinishPGConnNode)
        {
            List<PolyhedronGraphNode> ShortestGraphPath = new List<PolyhedronGraphNode>();

            // ��������������� ����
            PolyhedronGraphNode CurrentPGNode = StartPGConnNode;
            // ����, �� �������� �� ������ � ���������������
            PolyhedronGraphNode PrevPGNode = StartPGNode;

            // ���� ��������������� ���� �� �������� � �������� �����
            while (CurrentPGNode != StartPGNode)
            {
                ShortestGraphPath.Add(CurrentPGNode);

                // ������ ����� (����) �� ������� �� ������ � ��������������� ����
                Int32 ConnFromIndex = CurrentPGNode.GetConnectionIndex(PrevPGNode);
                // ������ ����� (����) ���������� ��� ���, �� ������� �� ������ � ��������������� ����
                Int32 ConnToIndex = (ConnFromIndex == 0 ? CurrentPGNode.NodeConnectionCount - 1 : ConnFromIndex - 1);
                PrevPGNode = CurrentPGNode;
                CurrentPGNode = CurrentPGNode[ConnToIndex];
            }

            if (PrevPGNode != FinishPGConnNode)
            {
                throw new AlgorithmException("Error at construction of the graph's path !!!");
            }

            return ShortestGraphPath;
        }

        /// <summary>
        /// ����� GraphTriangulation ��������� ��������� ������������ ����� (��. ��������)
        /// </summary>
        private void GraphTriangulation()
        {
            // ���� �� ���� ����� ����� �� ������ ����� �����
            foreach (PolyhedronGraphNode PGNode in m_PGNodeList)
            {
                // ���� �� ���� ������ �������� ����
                for (Int32 NodeConnIndex = 0; NodeConnIndex < PGNode.NodeConnectionCount; /*NodeConnIndex++*/)
                {
                    // ������� � ��������� ����� (����)
                    PolyhedronGraphNode CurrentConn = PGNode[NodeConnIndex];
                    PolyhedronGraphNode NextConn = (NodeConnIndex == PGNode.NodeConnectionCount - 1 ? PGNode[0] : PGNode[NodeConnIndex + 1]);

                    // ������ ������������� ���� ������, ������������ �� ������� ����� � ��������������� �� ��������� �����
                    List<PolyhedronGraphNode> ShortestGraphPath = GetShortestGraphPath(PGNode, CurrentConn, NextConn);

                    // ���� ����� ����� � ����������� ���� < 2, �� ��� ������ !!!!!!
                    if (ShortestGraphPath.Count < 2)
                    {
                        throw new AlgorithmException("Error at construction of the graph's path !!!");
                    }

                    // ���� �� 2-�� �� N-1 ���� �� ������������ ���� ������
                    // ���� ����� ����� � ����������� ���� = 2, �� ���� ���������� 0 ���
                    for (Int32 GraphPathIndex = 1; GraphPathIndex < ShortestGraphPath.Count - 1; GraphPathIndex++)
                    {
                        // � ������ ������ �������� ���� ��������� ����� (����� ������� ����� � i-� ����� �� ������������ ���� ������) �� ��������� ����������� ������ ��� �������, ���� ���������� �� ����
                        PGNode.InsertNodeConnection(NodeConnIndex + 1, ShortestGraphPath[GraphPathIndex]);

                        // � ������ ������ i-�� ���� �� ������������ ���� ������ ��������� (��� ��) ����� ����� ������, �� ������� �� ������ � ���� ����
                        Int32 CurrentPathNodeConnFromIndex = ShortestGraphPath[GraphPathIndex].GetConnectionIndex(ShortestGraphPath[GraphPathIndex - 1]);
                        ShortestGraphPath[GraphPathIndex].InsertNodeConnection(CurrentPathNodeConnFromIndex, PGNode);

                        NodeConnIndex++;
                    }

                    NodeConnIndex++;
                }
            }
        }

        /// <summary>
        /// ����� GetFirstCrossingObject ���������� ������ ������ ����������� � G(...Pi...)
        /// </summary>
        /// <param name="StartingPGNode"></param>
        /// <returns></returns>
        private CrossingObjectClass GetFirstCrossingObject(PolyhedronGraphNode StartingPGNode, Vector3D PiDirectingVector)
        {
            CrossingObjectClass FirstCrossingObject = null;

            // ������� ����
            PolyhedronGraphNode CurrentPGNode = StartingPGNode;
            // ��������� ��������� ������������ �������, ���������� � ������� �����, � ������������� ������� ������� Pi
            Double CurrentScalarProductValue = Vector3D.ScalarProduct(CurrentPGNode.NodeNormal, PiDirectingVector);
            // ���� ��������� ������������ = 0, �� ������� ���� ���������� ������� ��������
            if (Math.Abs(CurrentScalarProductValue) < Epsilon)
            {
                FirstCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, CurrentPGNode, null);
            }

            // ���� ���� �� ������ ������� ������ ������ �����������
            while ((Object)FirstCrossingObject == null)
            {
                Double BestScalarProductValue = Double.NaN;
                PolyhedronGraphNode BestPGNode = null;

                // ���� �� ���� ������ �������� ����
                for (Int32 NodeConnIndex = 0; NodeConnIndex < CurrentPGNode.NodeConnectionCount; NodeConnIndex++)
                {
                    // ������� ����� �������� ����
                    PolyhedronGraphNode CurrentConnPGNode = CurrentPGNode[NodeConnIndex];
                    // ������� ��������� ������������ �������, ���������� � ���������� ���� �����, � ������������� ������� ������� Pi
                    Double CurrentConnNodeScalarProductValue = Vector3D.ScalarProduct(CurrentConnPGNode.NodeNormal, PiDirectingVector);

                    // ���� ��������� ������������ = 0, �� ���������� ���� ���������� ������� ��������
                    if (Math.Abs(CurrentConnNodeScalarProductValue) < Epsilon)
                    {
                        FirstCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, CurrentConnPGNode, null);
                        break;
                    }

                    // ���� ��������� ������������ ���� �� �����, ��� � ��� �������, ���������� � ������� �����, ��
                    // ���� �� ����������� �������� �������� ���������� ������������ ������ ������������, �� ���������� �������� � ���������� ����
                    if (Math.Sign(CurrentScalarProductValue) == Math.Sign(CurrentConnNodeScalarProductValue))
                    {
                        if ((Double.IsNaN(BestScalarProductValue)) ||
                            (Math.Abs(CurrentConnNodeScalarProductValue) < Math.Abs(BestScalarProductValue)))
                        {
                            BestScalarProductValue = CurrentConnNodeScalarProductValue;
                            BestPGNode = CurrentConnPGNode;
                        }
                    }
                    // ���� ���� ���������� ������������ ��� ����������� (����) �������, ���������� �� ����� ���������� ������������ ��� �������, ���������� � ������� �����, ��
                    // �����, ����������� ������� � ���������� (����) ���� ���������� ������� ��������
                    else
                    {
                        PolyhedronGraphNode PlusPGNode = (CurrentScalarProductValue > 0 ? CurrentPGNode : CurrentConnPGNode);
                        PolyhedronGraphNode MinusPGNode = (CurrentScalarProductValue < 0 ? CurrentPGNode : CurrentConnPGNode);

                        FirstCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphConnection, PlusPGNode, MinusPGNode);
                        break;
                    }
                }

                // ������� ����� ���������� ����������� ����
                CurrentPGNode = BestPGNode;
                CurrentScalarProductValue = BestScalarProductValue;
            }

            /*// 
            if (FirstCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection)
            {
                //
                PolyhedronGraphNode PlusNode = FirstCrossingObject.PGNode1;
                PolyhedronGraphNode MinusNode = FirstCrossingObject.PGNode2;
                PolyhedronGraphNode ZeroNode = BuildCrossingNode(FirstCrossingObject);

                //
                Double ZeroPlusVectorsAngle = Math.Acos(Vector3D.ScalarProduct(ZeroNode.NodeNormal, PlusNode.NodeNormal));
                //
                Double ZeroMinusVectorsAngle = Math.Acos(Vector3D.ScalarProduct(ZeroNode.NodeNormal, MinusNode.NodeNormal));

                //
                if (ZeroPlusVectorsAngle < MinVectorDistinguishAngle)
                {
                    FirstCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, PlusNode, null);
                }
                //
                else if (ZeroMinusVectorsAngle < MinVectorDistinguishAngle)
                {
                    FirstCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, MinusNode, null);
                }
            }*/

            return FirstCrossingObject;
        }

        /// <summary>
        /// ����� GetFirstCrossingObject ���������� ������ ������ ����������� � G(...Pi...)
        /// </summary>
        /// <returns></returns>
        private CrossingObjectClass GetFirstCrossingObject(Vector3D PiDirectingVector)
        {
            return GetFirstCrossingObject(m_PGNodeList[0], PiDirectingVector);
        }
        
        /// <summary>
        /// ����� BuildCrossingNode ���������� ���� ����������� �������� ������� � G(...Pi...)
        /// ���� ������� ������ - ����, �� �� �� � ������������
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private PolyhedronGraphNode BuildCrossingNode(CrossingObjectClass CurrentCrossingObject, Vector3D PiDirectingVector)
        {
            PolyhedronGraphNode CrossingNode = null;

            if (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection)
            {
                Vector3D PlusValueVector = CurrentCrossingObject.PGNode1.NodeNormal;
                Vector3D MinusValueVector = CurrentCrossingObject.PGNode2.NodeNormal;
                // ������, ���������������� � PlusValueVector � MinusValueVector
                Vector3D Npm = Vector3D.VectorProduct(PlusValueVector, MinusValueVector);
                Vector3D ZeroValueVector = Vector3D.VectorProduct(Npm, PiDirectingVector);
                ZeroValueVector.Normalize();

                /*//
                Double ZeroPlusVectorsAngle = Math.Acos(Vector3D.ScalarProduct(ZeroValueVector, PlusValueVector));
                //
                Double ZeroMinusVectorsAngle = Math.Acos(Vector3D.ScalarProduct(ZeroValueVector, MinusValueVector));

                //
                if (ZeroPlusVectorsAngle < MinVectorDistinguishAngle)
                {
                    CrossingNode = CurrentCrossingObject.PGNode1;
                }
                //
                else if (ZeroMinusVectorsAngle < MinVectorDistinguishAngle)
                {
                    CrossingNode = CurrentCrossingObject.PGNode2;
                }
                //
                else
                {
                    CrossingNode = new PolyhedronGraphNode(m_PGNodeList.Count + 1, ZeroValueVector);
                }*/
                CrossingNode = new PolyhedronGraphNode(m_PGNodeList.Count + 1, ZeroValueVector);
            }
            else
            {
                CrossingNode = CurrentCrossingObject.PGNode1;
            }

            return CrossingNode;
        }

        /// <summary>
        /// ����� CheckMoveDirection ���������� true, ���� ����������� �������� �� G(...Pi...) ����������, ����� ������������ false
        /// ���������� ��������� ����������� �������� ������ ������� �������, ���� �������� � ����� ������������� ������� Pi
        /// </summary>
        /// <param name="CheckCrossingObject"></param>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private Boolean CheckMoveDirection(CrossingObjectClass CheckCrossingObject, CrossingObjectClass CurrentCrossingObject, Vector3D PiDirectingVector)
        {
            // ������������ ������ Pi ���������� ����� ��� OZ
            Vector3D OZDirectingVector = PiDirectingVector;

            // ������ ���� �� ����������� �������� ������� � G(...Pi...); ������, ��������� � ���� �����, ���������� ����� ��� OX
            PolyhedronGraphNode CurrentCrossingNode = BuildCrossingNode(CurrentCrossingObject, PiDirectingVector);
            Vector3D OXDirectingVector = CurrentCrossingNode.NodeNormal;

            // ������ ��� ��� OY ������ �� XYZ (��� ��������� ������������ ���� ��� OZ �� ��� ��� OX)
            Vector3D OYDirectingVector = Vector3D.VectorProduct(OZDirectingVector, OXDirectingVector);

            // ������ ���� �� ����������� ������������ ������� � G(...Pi...); ��������� ��������� ������������ �������, ���������� � ���� �����, � ���� ��� OY
            PolyhedronGraphNode CheckCrossingNode = BuildCrossingNode(CheckCrossingObject, PiDirectingVector);
            Vector3D CheckVector = CheckCrossingNode.NodeNormal;
            Double ScalarProductValue = Vector3D.ScalarProduct(CheckVector, OYDirectingVector);

            // ���� ScalarProductValue = 0 - ��� ������ ������ ���������
            if (Math.Abs(ScalarProductValue) < Epsilon)
            {
                throw new AlgorithmException("CheckMoveDirection method incorrect work");
            }

            // ���� ����������� ��������� ������������ > 0, �� ����������� �������� ����������, ����� ����������� �������� ������������
            return (ScalarProductValue > 0 ? true : false);
        }

        /// <summary>
        /// ����� GetNextCrossingObject4GraphNode ���������� ��������� �� ����������� �������� ������ �����������, ���� ������� - ����
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private CrossingObjectClass GetNextCrossingObject4GraphNode(CrossingObjectClass CurrentCrossingObject, Vector3D PiDirectingVector)
        {
            CrossingObjectClass NextCrossingObject = null;

            PolyhedronGraphNode CurrentPGNode = CurrentCrossingObject.PGNode1;
            // ���� �� ���� ������ �������� ����
            for (Int32 NodeConnIndex = 0; NodeConnIndex < CurrentPGNode.NodeConnectionCount; NodeConnIndex++)
            {
                // �������� ���� (����� 1), ��������� � ������� ����� ������� ������
                PolyhedronGraphNode PGNode1 = CurrentPGNode[NodeConnIndex];
                // �������� ���� (����� 2), ��������� � ������� ����� ���������� ������
                PolyhedronGraphNode PGNode2 = (NodeConnIndex == 0 ? CurrentPGNode[CurrentPGNode.NodeConnectionCount - 1] : CurrentPGNode[NodeConnIndex - 1]);

                // ��������� ��������� ������������ ������� 1 � ������������� ������� Pi
                Double PGNode1ScalarProductValue = Vector3D.ScalarProduct(PGNode1.NodeNormal, PiDirectingVector);
                // ��������� ��������� ������������ ������� 2 � ������������� ������� Pi
                Double PGNode2ScalarProductValue = Vector3D.ScalarProduct(PGNode2.NodeNormal, PiDirectingVector);

                // ���� ��������� ������������ ���� 1 � ������������� ������� Pi == 0
                if (Math.Abs(PGNode1ScalarProductValue) < Epsilon)
                {
                    // ���� ����������� �������� ������� ���������, �� ���� ����� 1 ���������� ��������� �� �������� ��������
                    NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, PGNode1, null);
                    if (CheckMoveDirection(NextCrossingObject, CurrentCrossingObject, PiDirectingVector))
                    {
                        break;
                    }
                    else
                    {
                        NextCrossingObject = null;
                    }
                }

                // ���� ��������� ������������ ���� 2 � ������������� ������� Pi == 0
                if (Math.Abs(PGNode2ScalarProductValue) < Epsilon)
                {
                    // ���� ����������� �������� ������� ���������, �� ���� ����� 2 ���������� ��������� �� �������� ��������
                    NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, PGNode2, null);
                    if (CheckMoveDirection(NextCrossingObject, CurrentCrossingObject, PiDirectingVector))
                    {
                        break;
                    }
                    else
                    {
                        NextCrossingObject = null;
                    }
                }

                // ���� ��������� ������������ ����� 1 � 2 � ������������� ������� Pi ����� ������ ����
                if (Math.Sign(PGNode1ScalarProductValue) != Math.Sign(PGNode2ScalarProductValue))
                {
                    // ���� ����������� �������� ������� ���������, �� �����, ����������� ���� 1 � 2, ���������� ��������� �� �������� ��������
                    PolyhedronGraphNode PlusPGNode = (PGNode1ScalarProductValue > 0 ? PGNode1 : PGNode2);
                    PolyhedronGraphNode MinusPGNode = (PGNode1ScalarProductValue < 0 ? PGNode1 : PGNode2);
                    NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphConnection, PlusPGNode, MinusPGNode);
                    if (CheckMoveDirection(NextCrossingObject, CurrentCrossingObject, PiDirectingVector))
                    {
                        break;
                    }
                    else
                    {
                        NextCrossingObject = null;
                    }
                }
            }

            return NextCrossingObject;
        }

        /// <summary>
        /// ����� GetNextCrossingObject4GraphConn ���������� ��������� �� ����������� �������� ������ �����������, ���� ������� - �����
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <param name="CurrentCrossingNode"></param>
        /// <returns></returns>
        private CrossingObjectClass GetNextCrossingObject4GraphConn(CrossingObjectClass CurrentCrossingObject, PolyhedronGraphNode CurrentCrossingNode, Vector3D PiDirectingVector)
        {
            CrossingObjectClass NextCrossingObject = null;

            // ������������� ���� ������� �����
            PolyhedronGraphNode CurrentPGNode1 = CurrentCrossingObject.PGNode1;
            // ������������� ���� ������� �����
            PolyhedronGraphNode CurrentPGNode2 = CurrentCrossingObject.PGNode2;

            /*Int32 CurrentConnIndex1 = CurrentPGNode1.GetConnectionIndex(CurrentPGNode2);
            Int32 CurrentConnIndex2 = CurrentPGNode2.GetConnectionIndex(CurrentPGNode1);*/
            Int32 CurrentConnIndex1 = CurrentPGNode1.GetConnectionIndex(CurrentCrossingNode);
            Int32 CurrentConnIndex2 = CurrentPGNode2.GetConnectionIndex(CurrentCrossingNode);

            // ��� �������������� ���� (CurrentCrossingObject.PGNode1) ����� ��������� ����� (������������ �������)
            PolyhedronGraphNode NextPGNode1 = (CurrentConnIndex1 == (CurrentPGNode1.NodeConnectionCount - 1) ? CurrentPGNode1[0] : CurrentPGNode1[CurrentConnIndex1 + 1]);
            // ��� �������������� ���� (CurrentCrossingObject.PGNode2) ����� ���������� ����� (������������ �������)
            PolyhedronGraphNode NextPGNode2 = (CurrentConnIndex2 == 0 ? CurrentPGNode2[CurrentPGNode2.NodeConnectionCount - 1] : CurrentPGNode2[CurrentConnIndex2 - 1]);

            Double NextPGNode1ScalarProductValue = Vector3D.ScalarProduct(NextPGNode1.NodeNormal, PiDirectingVector);
            Double NextPGNode2ScalarProductValue = Vector3D.ScalarProduct(NextPGNode2.NodeNormal, PiDirectingVector);

            // ���� ���������� ���� (����� 1) �������
            if (Math.Abs(NextPGNode1ScalarProductValue) < Epsilon)
            {
                // ���� ���������� ���� ����� 2 �������
                if (Math.Abs(NextPGNode2ScalarProductValue) < Epsilon)
                {
                    // ���������� ���� ���������� ��������� �� �������� ��������
                    NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, NextPGNode1, null);
                    // exit
                }
                // ���� ���������� ���� ����� 2 ���������
                else
                {
                    // "�����" (��� �����, � ������� �� �������� ��������; ������� �� ��� ���) ����������� ������������� ���� � ���� ����� 2 ���������� ��������� �� �������� ��������
                    NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphConnection, CurrentPGNode1, NextPGNode2);
                    // exit
                }
            }
            // ���� ���������� ���� (����� 1) �������������
            else if (NextPGNode1ScalarProductValue > 0)
            {
                // �����, ����������� ����� ������������� ���� � ������ ������������� ����, ���������� ��������� �� �������� ��������
                NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphConnection, NextPGNode1, CurrentPGNode2);
                // exit
            }
            // ���� ���������� ���� (����� 1) �������������
            else if (NextPGNode1ScalarProductValue < 0)
            {
                // �����, ����������� ����� ������������� ���� � ������ ������������� ����, ���������� ��������� �� �������� ��������
                NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphConnection, CurrentPGNode1, NextPGNode2);
                // exit
            }

            return NextCrossingObject;
        }

        /// <summary>
        /// ����� GetNextCrossingObject ���������� ��������� �� ����������� �������� ������ �����������
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <param name="CurrentCrossingNode"></param>
        /// <returns></returns>
        private CrossingObjectClass GetNextCrossingObject(CrossingObjectClass CurrentCrossingObject, PolyhedronGraphNode CurrentCrossingNode, Vector3D PiDirectingVector)
        {
            CrossingObjectClass NextCrossingObject = null;

            // ���� ������� ������ � ����
            if (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode)
            {
                NextCrossingObject = GetNextCrossingObject4GraphNode(CurrentCrossingObject, PiDirectingVector);
            }
            // ���� ������� ������ � �����
            else
            {
                NextCrossingObject = GetNextCrossingObject4GraphConn(CurrentCrossingObject, CurrentCrossingNode, PiDirectingVector);
            }

            /*// 
            if (NextCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection)
            {
                //
                PolyhedronGraphNode PlusNode = NextCrossingObject.PGNode1;
                PolyhedronGraphNode MinusNode = NextCrossingObject.PGNode2;
                PolyhedronGraphNode ZeroNode = BuildCrossingNode(NextCrossingObject);

                //
                Double ZeroPlusVectorsAngle = Math.Acos(Vector3D.ScalarProduct(ZeroNode.NodeNormal, PlusNode.NodeNormal));
                //
                Double ZeroMinusVectorsAngle = Math.Acos(Vector3D.ScalarProduct(ZeroNode.NodeNormal, MinusNode.NodeNormal));

                //
                if (ZeroPlusVectorsAngle < MinVectorDistinguishAngle)
                {
                    NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, PlusNode, null);
                }
                //
                else if (ZeroMinusVectorsAngle < MinVectorDistinguishAngle)
                {
                    NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, MinusNode, null);
                }
            }*/

            return NextCrossingObject;
        }

        /// <summary>
        /// ����� AddCrossingNodeBetweenConn ��������� ���� CrossingNode �� ����������� ����� � G(...Pi...) � ��������������� ������� ������/��������� ������
        /// </summary>
        /// <param name="ConnPlusNode"></param>
        /// <param name="ConnMinusNode"></param>
        /// <param name="CrossingNode"></param>
        private void AddCrossingNodeBetweenConn(PolyhedronGraphNode ConnPlusNode, PolyhedronGraphNode ConnMinusNode, PolyhedronGraphNode CrossingNode)
        {
            // ��������� ����� ���� � ������ ����� �����
            m_PGNodeList.Add(CrossingNode);
            // ��������� � ������ ������ ������ ���� ������ ������� �� ������������� ���� �����, ����� �� �������������
            CrossingNode.AddNodeConnection(ConnPlusNode);
            CrossingNode.AddNodeConnection(ConnMinusNode);
            // ��� �����, ���������� �����, ������ �� ������ ���� �� ����� (������� � �������� �����) �� ������ �� ����� ����
            Int32 PlusNodeCurrentConnIndex = ConnPlusNode.GetConnectionIndex(ConnMinusNode);
            Int32 MinusNodeCurrentConnIndex = ConnMinusNode.GetConnectionIndex(ConnPlusNode);
            ConnPlusNode[PlusNodeCurrentConnIndex] = CrossingNode;
            ConnMinusNode[MinusNodeCurrentConnIndex] = CrossingNode;
        }

        /// <summary>
        /// ����� AddConns4PrevNodeCurrentConnCase ��������� ����������� ����� � ������, ���� ���������� ������ ����������� - ����, � ������� - �����
        /// ����� ����������� ��� ����, ����� ���� ��������� �����������������
        /// </summary>
        /// <param name="PreviousCrossingObject"></param>
        /// <param name="PreviousCrossingNode"></param>
        /// <param name="CurrentCrossingObject"></param>
        /// <param name="CurrentCrossingNode"></param>
        private void AddConns4PrevNodeCurrentConnCase(CrossingObjectClass PreviousCrossingObject, PolyhedronGraphNode PreviousCrossingNode, CrossingObjectClass CurrentCrossingObject, PolyhedronGraphNode CurrentCrossingNode)
        {
            // ������ ����� ����� ���������� ����� � ����� ����������� �� ������� ������� (����� ����������� � ������ ���������������)
            // ������������� ���� ������� �����
            PolyhedronGraphNode CurrentConnMinusNode = CurrentCrossingObject.PGNode2;
            // ������ �� ������� ���� ����������� � ������ ������ ����������� ���� ��������� ����� ������ �� ������������� ���� ������� �����
            Int32 PrevNode2CurrentMinusNodeConnIndex = PreviousCrossingNode.GetConnectionIndex(CurrentConnMinusNode);
            PreviousCrossingNode.InsertNodeConnection(PrevNode2CurrentMinusNodeConnIndex + 1, CurrentCrossingNode);
            // ������ �� ���������� ���� ��������� ����� ������ �� ������������� ���� ������� ����� (�� ������� ����� 1)
            CurrentCrossingNode.InsertNodeConnection(1, PreviousCrossingNode);
        }

        /// <summary>
        /// ����� AddConns4PrevConnCurrentNodeCase ��������� ����������� ����� � ������, ���� ���������� ������ ����������� - �����, � ������� - ����
        /// ����� ����������� ��� ����, ����� ���� ��������� �����������������
        /// </summary>
        /// <param name="PreviousCrossingObject"></param>
        /// <param name="PreviousCrossingNode"></param>
        /// <param name="CurrentCrossingObject"></param>
        /// <param name="CurrentCrossingNode"></param>
        private void AddConns4PrevConnCurrentNodeCase(CrossingObjectClass PreviousCrossingObject, PolyhedronGraphNode PreviousCrossingNode, CrossingObjectClass CurrentCrossingObject, PolyhedronGraphNode CurrentCrossingNode)
        {
            // ������ ����� ����� ����� ����������� �� ���������� ������� � ������� ���� (����� ����������� � ������ ���������������)
            // ������������� ���� ���������� �����
            PolyhedronGraphNode PreviousConnPlusNode = PreviousCrossingObject.PGNode1;
            // ������ �� ���������� ���� ����������� � ������ ������ �������� ���� ��������� ����� ������ �� ������������� ���� ���������� �����
            Int32 CurrentNode2PrevPlusNodeConnIndex = CurrentCrossingNode.GetConnectionIndex(PreviousConnPlusNode);
            CurrentCrossingNode.InsertNodeConnection(CurrentNode2PrevPlusNodeConnIndex + 1, PreviousCrossingNode);
            // ������ �� ������� ���� ��������� � ����� ������ ������ ����������� ����
            PreviousCrossingNode.AddNodeConnection(CurrentCrossingNode);
        }

        /// <summary>
        /// ����� AddConns4PrevConnCurrentConnCase ��������� ����������� ����� � ������, ���� � ����������, � ������� ������� ����������� - �����
        /// ����� ����������� ��� ����, ����� ���� ��������� �����������������
        /// </summary>
        /// <param name="PreviousCrossingObject"></param>
        /// <param name="PreviousCrossingNode"></param>
        /// <param name="CurrentCrossingObject"></param>
        /// <param name="CurrentCrossingNode"></param>
        private void AddConns4PrevConnCurrentConnCase(CrossingObjectClass PreviousCrossingObject, PolyhedronGraphNode PreviousCrossingNode, CrossingObjectClass CurrentCrossingObject, PolyhedronGraphNode CurrentCrossingNode)
        {
            // ������ ����� ����� ����� ����������� �� ���������� ������� � ����� ����������� �� ������� ������� (����� ����������� � ������ ���������������)
            // ������ ����� ����� ����� ����������� �� ���������� ������� � ����� ������� �����, ������� �� ����������� ���������� ����� (����� ����������� � ������ ���������������)
            // � ������ ����� ������������� ���� (������ 3�)
            if (PreviousCrossingObject.PGNode2 == CurrentCrossingObject.PGNode2)
            {
                // ������������� ���� ���������� ����� (���� ����� 1)
                PolyhedronGraphNode PreviousConnPlusNode = PreviousCrossingObject.PGNode1;
                // ����� ������������� ���� (���� ����� 2)
                // PolyhedronGraphNode CommonConnMinusNode = PreviousCrossingObject.PGNode2;
                // ������������� ���� ������� ����� (���� ����� 3)
                PolyhedronGraphNode CurrentConnPlusNode = CurrentCrossingObject.PGNode1;
                // ��� ���� ����� 3: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ���� 1
                Int32 CurrentPlusNode2PrevPlusNodeConnIndex = CurrentConnPlusNode.GetConnectionIndex(PreviousConnPlusNode);
                CurrentConnPlusNode.InsertNodeConnection(CurrentPlusNode2PrevPlusNodeConnIndex + 1, PreviousCrossingNode);
                // ��� ����������� ���� �����������: � ����� ������ ������ ����������� ������� ������ �� ����� ���� �����������, ����� ������ �� ���� ����� 3
                PreviousCrossingNode.AddNodeConnection(CurrentCrossingNode);
                PreviousCrossingNode.AddNodeConnection(CurrentConnPlusNode);
                // ��� �������� ���� �����������: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ���� ����� 3 (�.�. �� ������� ����� 1)
                CurrentCrossingNode.InsertNodeConnection(1, PreviousCrossingNode);
            }
            // � ������ ����� ������������� ���� (������ 3�)
            else if (PreviousCrossingObject.PGNode1 == CurrentCrossingObject.PGNode1)
            {
                // ������������� ���� ���������� ����� (���� ����� 1)
                PolyhedronGraphNode PreviousConnMinusNode = PreviousCrossingObject.PGNode2;
                // ����� ������������� ���� (���� ����� 2)
                // PolyhedronGraphNode CommonConnPlusNode = PreviousCrossingObject.PGNode1;
                // ������������� ���� ������� ����� (���� ����� 3)
                PolyhedronGraphNode CurrentConnMinusNode = CurrentCrossingObject.PGNode2;
                // ��� ���� ����� 3: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ������� ���� �����������
                Int32 CurrentMinusNode2CurrentCrossingNodeConnIndex = CurrentConnMinusNode.GetConnectionIndex(CurrentCrossingNode);
                CurrentConnMinusNode.InsertNodeConnection(CurrentMinusNode2CurrentCrossingNodeConnIndex + 1, PreviousCrossingNode);
                // ��� ����������� ���� �����������: � ����� ������ ������ ����������� ������� ������ �� ���� ����� 3, ����� ������ �� ����� ���� �����������
                PreviousCrossingNode.AddNodeConnection(CurrentConnMinusNode);
                PreviousCrossingNode.AddNodeConnection(CurrentCrossingNode);
                // ��� �������� ���� �����������: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ���� ����� 1 (�.�. �� ������� ����� 1)
                CurrentCrossingNode.InsertNodeConnection(1, PreviousCrossingNode);
            }
            // ������ ������ ���������
            else
            {
                throw new AlgorithmException("AddConns4PrevConnCurrentConnCase method incorrect work");
            }
        }

        /// <summary>
        /// ����� BuildGFiGrid ������ ����� G(...Fi...) (��. ��������)
        /// </summary>
        private void BuildGFiGrid(Vector3D PiDirectingVector)
        {
            // ������ (�����������) ������ �����������
            CrossingObjectClass FirstCrossingObject = GetFirstCrossingObject(PiDirectingVector);
            // ������� ������ �����������
            CrossingObjectClass CurrentCrossingObject = FirstCrossingObject;
            // ������ ���� �� ����������� �������� ������� � G(...Pi...) � ���������� ���
            // ���� ���� ���� ����������� � ������ �����, �� ��������� ��� � ��������������� ������ �� ������ ����
            PolyhedronGraphNode FirstCrossingNode = BuildCrossingNode(CurrentCrossingObject, PiDirectingVector);
            // ������� ���� �����������
            PolyhedronGraphNode CurrentCrossingNode = FirstCrossingNode;

            if (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection)
            {
                AddCrossingNodeBetweenConn(CurrentCrossingObject.PGNode1, CurrentCrossingObject.PGNode2, CurrentCrossingNode);
            }

            do
            {
                // ���������� ������ �����������
                CrossingObjectClass PreviousCrossingObject = CurrentCrossingObject;
                // ���������� ���� �����������
                PolyhedronGraphNode PreviousCrossingNode = CurrentCrossingNode;
                // �������� ��������� �� �������� ������ (�����, ���� ����) � ������ ��� �������
                CurrentCrossingObject = GetNextCrossingObject(CurrentCrossingObject, CurrentCrossingNode, PiDirectingVector);
                // ������ ���� �� ����������� �������� ������� � G(...Pi...)
                // ���� ���� ���� ����������� � ������ ����� (���� ���� ����� �������������� � ������ �����, ���� ������� ������ � ����, ���� ���� ��������� �������� ���� ����� � �� � ��� ������), �� ��������� ��� � ��������������� ������ �� ������ ����
                // �������� ������������ ������ ���� �� ������ � ������ (�����������) ������ ����������� (��� �������� ���������� ���������)
                CurrentCrossingNode = (CurrentCrossingObject == FirstCrossingObject ? FirstCrossingNode : BuildCrossingNode(CurrentCrossingObject, PiDirectingVector));
                if ((CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection) &&
                    (CurrentCrossingObject != FirstCrossingObject))
                {
                    AddCrossingNodeBetweenConn(CurrentCrossingObject.PGNode1, CurrentCrossingObject.PGNode2, CurrentCrossingNode);
                }

                // ���� ���������� � ������� ������� � ����
                if ((PreviousCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode) &&
                    (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode))
                {
                    // ������� � ��������� �������� �����
                    // continue;
                }

                // ���� ���������� ������ ����, � ������� �����
                if ((PreviousCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode) &&
                    (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection))
                {
                    AddConns4PrevNodeCurrentConnCase(PreviousCrossingObject, PreviousCrossingNode, CurrentCrossingObject, CurrentCrossingNode);
                }

                // ���� ���������� ������ �����, � ������� ����
                if ((PreviousCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection) &&
                    (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode))
                {
                    AddConns4PrevConnCurrentNodeCase(PreviousCrossingObject, PreviousCrossingNode, CurrentCrossingObject, CurrentCrossingNode);
                }

                // ���� ���������� � ������� ������� - �����
                if ((PreviousCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection) &&
                    (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection))
                {
                    AddConns4PrevConnCurrentConnCase(PreviousCrossingObject, PreviousCrossingNode, CurrentCrossingObject, CurrentCrossingNode);
                }
            }
            while (CurrentCrossingObject != FirstCrossingObject);
        }

        /// <summary>
        /// ����� GetFirstCrossingObject2 ���������� ������ ������ ����������� Zi � G(...Fi...)
        /// </summary>
        /// <param name="StartingPGNode"></param>
        /// <returns></returns>
        private CrossingObjectClass GetFirstCrossingObject2(PolyhedronGraphNode StartingPGNode, Vector3D QiDirectingVector)
        {
            CrossingObjectClass FirstCrossingObject = null;

            // ������� ����
            PolyhedronGraphNode CurrentPGNode = StartingPGNode;
            // ��������� ��������� ������������ �������, ���������� � ������� �����, � ������������� ������� ������� Qi
            Double CurrentScalarProductValue = Vector3D.ScalarProduct(CurrentPGNode.NodeNormal, QiDirectingVector);
            // ���� ��������� ������������ = 0, �� ������� ���� ���������� ������� ��������
            if (Math.Abs(CurrentScalarProductValue) < Epsilon)
            {
                FirstCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, CurrentPGNode, null);
            }

            // ���� ���� �� ������ ������� ������ ������ �����������
            while ((Object)FirstCrossingObject == null)
            {
                Double BestScalarProductValue = Double.NaN;
                PolyhedronGraphNode BestPGNode = null;

                // ���� �� ���� ������ �������� ����
                for (Int32 NodeConnIndex = 0; NodeConnIndex < CurrentPGNode.NodeConnectionCount; NodeConnIndex++)
                {
                    // ������� ����� �������� ����
                    PolyhedronGraphNode CurrentConnPGNode = CurrentPGNode[NodeConnIndex];
                    // ������� ��������� ������������ �������, ���������� � ���������� ���� �����, � ������������� ������� ������� Qi
                    Double CurrentConnNodeScalarProductValue = Vector3D.ScalarProduct(CurrentConnPGNode.NodeNormal, QiDirectingVector);

                    // ���� ��������� ������������ = 0, �� ���������� ���� ���������� ������� ��������
                    if (Math.Abs(CurrentConnNodeScalarProductValue) < Epsilon)
                    {
                        FirstCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, CurrentConnPGNode, null);
                        break;
                    }

                    // ���� ��������� ������������ ���� �� �����, ��� � ��� �������, ���������� � ������� �����, ��
                    // ���� �� ����������� �������� �������� ���������� ������������ ������ ������������, �� ���������� �������� � ���������� ����
                    if (Math.Sign(CurrentScalarProductValue) == Math.Sign(CurrentConnNodeScalarProductValue))
                    {
                        if ((Double.IsNaN(BestScalarProductValue)) ||
                            (Math.Abs(CurrentConnNodeScalarProductValue) < Math.Abs(BestScalarProductValue)))
                        {
                            BestScalarProductValue = CurrentConnNodeScalarProductValue;
                            BestPGNode = CurrentConnPGNode;
                        }
                    }
                    // ���� ���� ���������� ������������ ��� ����������� (����) �������, ���������� �� ����� ���������� ������������ ��� �������, ���������� � ������� �����, ��
                    // �����, ����������� ������� � ���������� (����) ���� ���������� ������� ��������
                    else
                    {
                        PolyhedronGraphNode PlusPGNode = (CurrentScalarProductValue > 0 ? CurrentPGNode : CurrentConnPGNode);
                        PolyhedronGraphNode MinusPGNode = (CurrentScalarProductValue < 0 ? CurrentPGNode : CurrentConnPGNode);

                        FirstCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphConnection, PlusPGNode, MinusPGNode);
                        break;
                    }
                }

                // ������� ����� ���������� ����������� ����
                CurrentPGNode = BestPGNode;
                CurrentScalarProductValue = BestScalarProductValue;
            }

            return FirstCrossingObject;
        }

        /// <summary>
        /// ����� GetFirstCrossingObject2 ���������� ������ ������ ����������� Zi � G(...Fi...)
        /// </summary>
        /// <returns></returns>
        private CrossingObjectClass GetFirstCrossingObject2(Vector3D QiDirectingVector)
        {
            return GetFirstCrossingObject2(m_PGNodeList[0], QiDirectingVector);
        }

        /// <summary>
        /// ����� BuildCrossingNode2 ���������� ���� ����������� �������� ������� � Zi
        /// ���� ������� ������ - ����, �� �� �� � ������������
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private PolyhedronGraphNode BuildCrossingNode2(CrossingObjectClass CurrentCrossingObject, Vector3D QiDirectingVector)
        {
            PolyhedronGraphNode CrossingNode = null;

            if (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection)
            {
                Vector3D PlusValueVector = CurrentCrossingObject.PGNode1.NodeNormal;
                Vector3D MinusValueVector = CurrentCrossingObject.PGNode2.NodeNormal;
                // ������, ���������������� � PlusValueVector � MinusValueVector
                Vector3D Npm = Vector3D.VectorProduct(PlusValueVector, MinusValueVector);
                Vector3D ZeroValueVector = Vector3D.VectorProduct(Npm, QiDirectingVector);
                ZeroValueVector.Normalize();
                CrossingNode = new PolyhedronGraphNode(m_PGNodeList.Count + 1, ZeroValueVector);
            }
            else
            {
                CrossingNode = CurrentCrossingObject.PGNode1;
            }

            return CrossingNode;
        }

        /// <summary>
        /// ����� CheckMoveDirection2 ���������� true, ���� ����������� �������� �� Zi ����������, ����� ������������ false
        /// ���������� ��������� ����������� �������� ������ ������� �������, ���� �������� � ����� ������������� ������� Qi
        /// </summary>
        /// <param name="CheckCrossingObject"></param>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private Boolean CheckMoveDirection2(CrossingObjectClass CheckCrossingObject, CrossingObjectClass CurrentCrossingObject, Vector3D QiDirectingVector)
        {
            // ������������ ������ Qi ���������� ����� ��� OZ
            Vector3D OZDirectingVector = QiDirectingVector;

            // ������ ���� �� ����������� �������� ������� � Zi; ������, ��������� � ���� �����, ���������� ����� ��� OX
            PolyhedronGraphNode CurrentCrossingNode = BuildCrossingNode2(CurrentCrossingObject, QiDirectingVector);
            Vector3D OXDirectingVector = CurrentCrossingNode.NodeNormal;

            // ������ ��� ��� OY ������ �� XYZ (��� ��������� ������������ ���� ��� OZ �� ��� ��� OX)
            Vector3D OYDirectingVector = Vector3D.VectorProduct(OZDirectingVector, OXDirectingVector);

            // ������ ���� �� ����������� ������������ ������� � Zi; ��������� ��������� ������������ �������, ���������� � ���� �����, � ���� ��� OY
            PolyhedronGraphNode CheckCrossingNode = BuildCrossingNode2(CheckCrossingObject, QiDirectingVector);
            Vector3D CheckVector = CheckCrossingNode.NodeNormal;
            Double ScalarProductValue = Vector3D.ScalarProduct(CheckVector, OYDirectingVector);

            // ���� ScalarProductValue = 0 - ��� ������ ������ ���������
            if (Math.Abs(ScalarProductValue) < Epsilon)
            {
                throw new AlgorithmException("CheckMoveDirection method incorrect work");
            }

            // ���� ����������� ��������� ������������ > 0, �� ����������� �������� ����������, ����� ����������� �������� ������������
            return (ScalarProductValue > 0 ? true : false);
        }

        /// <summary>
        /// ����� AddConns2SuspiciousConnectionsList ��������� � ������ "��������������" ������ ��� ������ ����������� (���� ��� �����) � ��� �������� �����
        /// ���� ������ ����������� - ����, �� �������� ���� ��������� �� ������ �����, ������� �������� ���� ����, �� � ��� �������� ����� (���� ��� � ��������) ???!!!
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <param name="SuspiciousConnectionSet"></param>
        private void AddConns2SuspiciousConnectionsList(CrossingObjectClass CurrentCrossingObject, SuspiciousConnectionSetClass SuspiciousConnectionSet)
        {
            // ���� ������� ������ � ����
            if (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode)
            {
                // ������� ����
                PolyhedronGraphNode CurrentPGNode = CurrentCrossingObject.PGNode1;

                // ���� �� ���� ������ �������� ���� (�������� ������� �����������)
                for (Int32 ConnIndex = 0; ConnIndex < CurrentPGNode.NodeConnectionCount; ConnIndex++)
                {
                    // ������� �����
                    PolyhedronGraphNode CurrentPGConn = CurrentPGNode[ConnIndex];
                    // ���� ������� ����� ����������� � ������ �, �� ��������� �� � ���� �����
                    SuspiciousConnectionSet.AddConnection(CurrentPGNode, CurrentPGConn);
                }
            }
            // ���� ������� ������ � �����
            else
            {
                PolyhedronGraphNode PlusPGNode = CurrentCrossingObject.PGNode1;
                PolyhedronGraphNode MinusPGNode = CurrentCrossingObject.PGNode2;

                // ���� ������� ����� (������� ������ �����������) ����������� � ������ �, �� ��������� �� � ���� �����
                SuspiciousConnectionSet.AddConnection(PlusPGNode, MinusPGNode);
                // ���� �� ���� ������ �������������� ���� ������� �����
                for (Int32 ConnIndex = 0; ConnIndex < PlusPGNode.NodeConnectionCount; ConnIndex++)
                {
                    // ���� ��������������� ����� �������������� ���� ������� ����� ����������� � ������ �, �� ��������� �� � ���� �����
                    SuspiciousConnectionSet.AddConnection(PlusPGNode, PlusPGNode[ConnIndex]);
                }
                // ���� �� ���� ������ �������������� ���� ������� �����
                for (Int32 ConnIndex = 0; ConnIndex < MinusPGNode.NodeConnectionCount; ConnIndex++)
                {
                    // ���� ��������������� ����� �������������� ���� ������� ����� ����������� � ������ �, �� ��������� �� � ���� �����
                    SuspiciousConnectionSet.AddConnection(MinusPGNode, MinusPGNode[ConnIndex]);
                }
            }
        }

        /// <summary>
        /// ����� GetNextCrossingObject4GraphNode2 ���������� ��������� �� ����������� �������� ������ ����������� (Zi � G(...Fi...)), ���� ������� - ����
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private CrossingObjectClass GetNextCrossingObject4GraphNode2(CrossingObjectClass CurrentCrossingObject, Vector3D QiDirectingVector)
        {
            CrossingObjectClass NextCrossingObject = null;

            PolyhedronGraphNode CurrentPGNode = CurrentCrossingObject.PGNode1;
            // ���� �� ���� ������ �������� ����
            for (Int32 NodeConnIndex = 0; NodeConnIndex < CurrentPGNode.NodeConnectionCount; NodeConnIndex++)
            {
                // �������� ���� (����� 1), ��������� � ������� ����� ������� ������
                PolyhedronGraphNode PGNode1 = CurrentPGNode[NodeConnIndex];
                // �������� ���� (����� 2), ��������� � ������� ����� ���������� ������
                PolyhedronGraphNode PGNode2 = (NodeConnIndex == 0 ? CurrentPGNode[CurrentPGNode.NodeConnectionCount - 1] : CurrentPGNode[NodeConnIndex - 1]);

                // ��������� ��������� ������������ ������� 1 � ������������� ������� Qi
                Double PGNode1ScalarProductValue = Vector3D.ScalarProduct(PGNode1.NodeNormal, QiDirectingVector);
                // ��������� ��������� ������������ ������� 2 � ������������� ������� Qi
                Double PGNode2ScalarProductValue = Vector3D.ScalarProduct(PGNode2.NodeNormal, QiDirectingVector);

                // ���� ��������� ������������ ���� 1 � ������������� ������� Qi == 0
                if (Math.Abs(PGNode1ScalarProductValue) < Epsilon)
                {
                    // ���� ����������� �������� ������� ���������, �� ���� ����� 1 ���������� ��������� �� �������� ��������
                    NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, PGNode1, null);
                    if (CheckMoveDirection2(NextCrossingObject, CurrentCrossingObject, QiDirectingVector))
                    {
                        break;
                    }
                    else
                    {
                        NextCrossingObject = null;
                    }
                }

                // ���� ��������� ������������ ���� 2 � ������������� ������� Qi == 0
                if (Math.Abs(PGNode2ScalarProductValue) < Epsilon)
                {
                    // ���� ����������� �������� ������� ���������, �� ���� ����� 2 ���������� ��������� �� �������� ��������
                    NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, PGNode2, null);
                    if (CheckMoveDirection2(NextCrossingObject, CurrentCrossingObject, QiDirectingVector))
                    {
                        break;
                    }
                    else
                    {
                        NextCrossingObject = null;
                    }
                }

                // ���� ��������� ������������ ����� 1 � 2 � ������������� ������� Qi ����� ������ ����
                if (Math.Sign(PGNode1ScalarProductValue) != Math.Sign(PGNode2ScalarProductValue))
                {
                    // ���� ����������� �������� ������� ���������, �� �����, ����������� ���� 1 � 2, ���������� ��������� �� �������� ��������
                    PolyhedronGraphNode PlusPGNode = (PGNode1ScalarProductValue > 0 ? PGNode1 : PGNode2);
                    PolyhedronGraphNode MinusPGNode = (PGNode1ScalarProductValue < 0 ? PGNode1 : PGNode2);
                    NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphConnection, PlusPGNode, MinusPGNode);
                    if (CheckMoveDirection2(NextCrossingObject, CurrentCrossingObject, QiDirectingVector))
                    {
                        break;
                    }
                    else
                    {
                        NextCrossingObject = null;
                    }
                }
            }

            return NextCrossingObject;
        }

        /// <summary>
        /// ����� GetNextCrossingObject4GraphConn2 ���������� ��������� �� ����������� �������� ������ ����������� (Zi � G(...Fi...)), ���� ������� - �����
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private CrossingObjectClass GetNextCrossingObject4GraphConn2(CrossingObjectClass CurrentCrossingObject, Vector3D QiDirectingVector)
        {
            CrossingObjectClass NextCrossingObject = null;

            // ������������� ���� ������� �����
            PolyhedronGraphNode PlusPGNode = CurrentCrossingObject.PGNode1;
            // ������������� ���� ������� �����
            PolyhedronGraphNode MinusPGNode = CurrentCrossingObject.PGNode2;

            Int32 CurrentConnPlusIndex = PlusPGNode.GetConnectionIndex(MinusPGNode);
            Int32 CurrentConnMinusIndex = MinusPGNode.GetConnectionIndex(PlusPGNode);
            /*Int32 CurrentConnIndex1 = CurrentPGNode1.GetConnectionIndex(CurrentCrossingNode);
            Int32 CurrentConnIndex2 = CurrentPGNode2.GetConnectionIndex(CurrentCrossingNode);*/

            // ��� �������������� ���� (CurrentCrossingObject.PGNode1==PlusPGNode) ����� ��������� ����� (������������ �������)
            PolyhedronGraphNode NextPGNode1 = (CurrentConnPlusIndex == (PlusPGNode.NodeConnectionCount - 1) ? PlusPGNode[0] : PlusPGNode[CurrentConnPlusIndex + 1]);
            // ��� �������������� ���� (CurrentCrossingObject.PGNode2==MinusPGNode) ����� ���������� ����� (������������ �������)
            PolyhedronGraphNode NextPGNode2 = (CurrentConnMinusIndex == 0 ? MinusPGNode[MinusPGNode.NodeConnectionCount - 1] : MinusPGNode[CurrentConnMinusIndex - 1]);

            Double NextPGNode1ScalarProductValue = Vector3D.ScalarProduct(NextPGNode1.NodeNormal, QiDirectingVector);
            Double NextPGNode2ScalarProductValue = Vector3D.ScalarProduct(NextPGNode2.NodeNormal, QiDirectingVector);

            // ���� ���������� ���� (����� 1) �������
            if (Math.Abs(NextPGNode1ScalarProductValue) < Epsilon)
            {
                // ���� ���������� ���� ����� 2 �� �������
                if (Math.Abs(NextPGNode2ScalarProductValue) >= Epsilon)
                {
                    throw new AlgorithmException("GetNextCrossingObject4GraphConn2 method incorrect work");
                }

                // ���������� ���� ���������� ��������� �� �������� ��������
                NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, NextPGNode1, null);
                // exit
            }
            // ���� ���������� ���� (����� 1) �������������
            else if (NextPGNode1ScalarProductValue > 0)
            {
                // �����, ����������� ����� ������������� ���� � ������ ������������� ����, ���������� ��������� �� �������� ��������
                NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphConnection, NextPGNode1, MinusPGNode);
                // exit
            }
            // ���� ���������� ���� (����� 1) �������������
            else if (NextPGNode1ScalarProductValue < 0)
            {
                // �����, ����������� ����� ������������� ���� � ������ ������������� ����, ���������� ��������� �� �������� ��������
                NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphConnection, PlusPGNode, NextPGNode2);
                // exit
            }

            return NextCrossingObject;
        }

        /// <summary>
        /// ����� GetNextCrossingObject2 ���������� ��������� �� ����������� �������� ������ ����������� (Zi � G(...Fi...))
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private CrossingObjectClass GetNextCrossingObject2(CrossingObjectClass CurrentCrossingObject, Vector3D QiDirectingVector)
        {
            // ���� ������� ������ � ����
            if (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode)
            {
                return GetNextCrossingObject4GraphNode2(CurrentCrossingObject, QiDirectingVector);
            }
            // ���� ������� ������ � �����
            else
            {
                return GetNextCrossingObject4GraphConn2(CurrentCrossingObject, QiDirectingVector);
            }
        }

        /// <summary>
        /// ����� GetSuspiciousConnectionsList ���������� ����� ���� "��������������" ������ (����� �)
        /// </summary>
        /// <returns></returns>
        private SuspiciousConnectionSetClass GetSuspiciousConnectionsList(Vector3D QiDirectingVector)
        {
            // ������� ����� �
            SuspiciousConnectionSetClass SuspiciousConnectionSet = new SuspiciousConnectionSetClass();

            // ������ (�����������) ������ �����������
            CrossingObjectClass FirstCrossingObject = GetFirstCrossingObject2(QiDirectingVector);
            // ������� ������ �����������
            CrossingObjectClass CurrentCrossingObject = FirstCrossingObject;
            // ��������� � ����� � ��� ������ ����������� � ��� �������� � ��� �����
            AddConns2SuspiciousConnectionsList(CurrentCrossingObject, SuspiciousConnectionSet);

            do
            {
                // �������� ��������� �� �������� ������ (�����, ���� ����) � ������ ��� �������
                CurrentCrossingObject = GetNextCrossingObject2(CurrentCrossingObject, QiDirectingVector);
                // ��������� � ����� � ��� ������ ����������� � ��� �������� � ��� �����
                AddConns2SuspiciousConnectionsList(CurrentCrossingObject, SuspiciousConnectionSet);
            }
            while (CurrentCrossingObject != FirstCrossingObject);

            return SuspiciousConnectionSet;
        }

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
        /// ����� SolveEquationSystem3Kramer ������ ������� ���. ��������� (3x3) ������� �������
        /// </summary>
        /// <param name="MatrixA"></param>
        /// <param name="MatrixB"></param>
        /// <returns></returns>
        private Matrix SolveEquationSystem3Kramer(Matrix MatrixA, Matrix MatrixB)
        {
            if ((MatrixA.ColumnCount != 3) && (MatrixA.RowCount != 3))
            {
                throw new ArgumentException("MatrixA must be 3x3");
            }
            if ((MatrixB.ColumnCount != 1) && (MatrixB.RowCount != 3))
            {
                throw new ArgumentException("MatrixB must be 3x1");
            }

            Double Delta = CalcDeterminant3(MatrixA);
            
            Matrix MatrixAX = MatrixA.Clone();
            MatrixAX[1, 1] = MatrixB[1, 1];
            MatrixAX[2, 1] = MatrixB[2, 1];
            MatrixAX[3, 1] = MatrixB[3, 1];
            Double DeltaX = CalcDeterminant3(MatrixAX);

            Matrix MatrixAY = MatrixA.Clone();
            MatrixAY[1, 2] = MatrixB[1, 1];
            MatrixAY[2, 2] = MatrixB[2, 1];
            MatrixAY[3, 2] = MatrixB[3, 1];
            Double DeltaY = CalcDeterminant3(MatrixAY);

            Matrix MatrixAZ = MatrixA.Clone();
            MatrixAZ[1, 3] = MatrixB[1, 1];
            MatrixAZ[2, 3] = MatrixB[2, 1];
            MatrixAZ[3, 3] = MatrixB[3, 1];
            Double DeltaZ = CalcDeterminant3(MatrixAZ);

            Matrix SolutionMatrix = new Matrix(3, 1);
            SolutionMatrix[1, 1] = DeltaX / Delta;
            SolutionMatrix[2, 1] = DeltaY / Delta;
            SolutionMatrix[3, 1] = DeltaZ / Delta;

            return SolutionMatrix;
        }

        private Matrix SolveEquationSystem3Gauss(Matrix MA, Matrix MB, out Matrix MError)
        {
            Matrix MatrixA = MA.Clone();
            Matrix MatrixB = MB.Clone();

            Int32 RowCount = MatrixA.RowCount;

            for (Int32 RowIndex1 = 1; RowIndex1 <= RowCount; RowIndex1++)
            {
                //
                Double MaxElementAbsValue = Math.Abs(MatrixA[RowIndex1, RowIndex1]);
                Int32 MaxElementRowIndex = RowIndex1;
                //
                for (Int32 RowIndex2 = RowIndex1 + 1; RowIndex2 <= RowCount; RowIndex2++)
                {
                    Double CurrentElementAbsValue = Math.Abs(MatrixA[RowIndex2, RowIndex1]);
                    if (MaxElementAbsValue < CurrentElementAbsValue)
                    {
                        MaxElementAbsValue = CurrentElementAbsValue;
                        MaxElementRowIndex = RowIndex2;
                    }
                }
                //
                if (MaxElementRowIndex != RowIndex1)
                {
                    Matrix Row1 = MatrixA.GetMatrixRow(RowIndex1);
                    Matrix MaxElementRow = MatrixA.GetMatrixRow(MaxElementRowIndex);
                    MatrixA.SetMatrixRow(RowIndex1, MaxElementRow);
                    MatrixA.SetMatrixRow(MaxElementRowIndex, Row1);

                    Double TempValue = MatrixB[RowIndex1, 1];
                    MatrixB[RowIndex1, 1] = MatrixB[MaxElementRowIndex, 1];
                    MatrixB[MaxElementRowIndex, 1] = TempValue;
                }

                //
                Matrix MatrixARow1 = MatrixA.GetMatrixRow(RowIndex1);
                Double MatrixBRow1 = MatrixB[RowIndex1, 1];
                //
                for (Int32 RowIndex2 = RowIndex1 + 1; RowIndex2 <= RowCount; RowIndex2++)
                {
                    Double Koeff = MatrixA[RowIndex2, RowIndex1] / MatrixA[RowIndex1, RowIndex1];

                    Matrix MatrixACurrentRow = MatrixA.GetMatrixRow(RowIndex2);
                    MatrixA.SetMatrixRow(RowIndex2, MatrixACurrentRow - Koeff * MatrixARow1);

                    MatrixB[RowIndex2, 1] -= Koeff * MatrixBRow1;
                }
            }

            Matrix SolutionMatrix = new Matrix(3, 1);

            //
            SolutionMatrix[3, 1] = MatrixB[3, 1] / MatrixA[3, 3];
            SolutionMatrix[2, 1] = (MatrixB[2, 1] - MatrixA[2, 3] * SolutionMatrix[3, 1]) / MatrixA[2, 2];
            SolutionMatrix[1, 1] = (MatrixB[1, 1] - MatrixA[1, 3] * SolutionMatrix[3, 1] - MatrixA[1, 2] * SolutionMatrix[2, 1]) / MatrixA[1, 1];

            //
            MError = MatrixA * SolutionMatrix - MatrixB;

            return SolutionMatrix;
        }

        /// <summary>
        /// ����� SolveEquationSystem3 ������ ������� ���. ��������� (3x3) ����� �� �������
        /// </summary>
        /// <param name="MatrixA"></param>
        /// <param name="MatrixB"></param>
        /// <returns></returns>
        private Matrix SolveEquationSystem3(Matrix MatrixA, Matrix MatrixB)
        {
            //return SolveEquationSystem3Kramer(MatrixA, MatrixB);
            Matrix MError = null;
            Matrix Solution = SolveEquationSystem3Gauss(MatrixA, MatrixB, out MError);

            Double Delta1 = Math.Abs(MError[1, 1]) + Math.Abs(MError[2, 1]) + Math.Abs(MError[3, 1]);
            Delta1 /= (Math.Abs(Solution[1, 1]) + Math.Abs(Solution[2, 1]) + Math.Abs(Solution[3, 1]));
            Double Delta2 = Math.Sqrt(MError[1, 1] * MError[1, 1] + MError[2, 1] * MError[2, 1] + MError[3, 1] * MError[3, 1]);
            Delta2 /= Math.Sqrt(Solution[1, 1] * Solution[1, 1] + Solution[2, 1] * Solution[2, 1] + Solution[3, 1] * Solution[3, 1]);

            if (Delta1 > 0.01)
            {
                throw new AlgorithmException("Delta1 = " + Delta1.ToString());
            }
            if (Delta2 > 0.01)
            {
                throw new AlgorithmException("Delta2 = " + Delta2.ToString());
            }

            return Solution;
        }

        /// <summary>
        /// ����� FuncFi ���������� �������� ������� Fi (��. ��������)
        /// </summary>
        /// <param name="VectorArg"></param>
        /// <param name="PolyhedronSet"></param>
        /// <param name="PiSet"></param>
        /// <param name="QiSet"></param>
        /// <returns></returns>
        private Double FuncFi(Vector3D VectorArg, List<VertexClass> PolyhedronSet, List<Point3D> Pi1Set, List<Point3D> Pi2Set, List<Point3D> Qi1Set, List<Point3D> Qi2Set)
        {
            Double PolyhedronSupportFuncValue = Double.NaN;
            foreach (VertexClass CurrentVertex in PolyhedronSet)
            {
                Double ScalarProductValue = VectorArg.XCoord * CurrentVertex.XCoord + VectorArg.YCoord * CurrentVertex.YCoord + VectorArg.ZCoord * CurrentVertex.ZCoord;
                if ((Double.IsNaN(PolyhedronSupportFuncValue)) || (PolyhedronSupportFuncValue < ScalarProductValue))
                {
                    PolyhedronSupportFuncValue = ScalarProductValue;
                }
            }
            if (Double.IsNaN(PolyhedronSupportFuncValue)) PolyhedronSupportFuncValue = 0;

            Double Pi1SetSupportFuncValue = Double.NaN;
            foreach (Point3D CurrentPoint in Pi1Set)
            {
                Double ScalarProductValue = VectorArg.XCoord * CurrentPoint.XCoord + VectorArg.YCoord * CurrentPoint.YCoord + VectorArg.ZCoord * CurrentPoint.ZCoord;
                if ((Double.IsNaN(Pi1SetSupportFuncValue)) || (Pi1SetSupportFuncValue < ScalarProductValue))
                {
                    Pi1SetSupportFuncValue = ScalarProductValue;
                }
            }
            if (Double.IsNaN(Pi1SetSupportFuncValue)) Pi1SetSupportFuncValue = 0;

            Double Pi2SetSupportFuncValue = Double.NaN;
            foreach (Point3D CurrentPoint in Pi2Set)
            {
                Double ScalarProductValue = VectorArg.XCoord * CurrentPoint.XCoord + VectorArg.YCoord * CurrentPoint.YCoord + VectorArg.ZCoord * CurrentPoint.ZCoord;
                if ((Double.IsNaN(Pi2SetSupportFuncValue)) || (Pi2SetSupportFuncValue < ScalarProductValue))
                {
                    Pi2SetSupportFuncValue = ScalarProductValue;
                }
            }
            if (Double.IsNaN(Pi2SetSupportFuncValue)) Pi2SetSupportFuncValue = 0;

            Double Qi1SetSupportFuncValue = Double.NaN;
            foreach (Point3D CurrentPoint in Qi1Set)
            {
                Double ScalarProductValue = VectorArg.XCoord * CurrentPoint.XCoord + VectorArg.YCoord * CurrentPoint.YCoord + VectorArg.ZCoord * CurrentPoint.ZCoord;
                if ((Double.IsNaN(Qi1SetSupportFuncValue)) || (Qi1SetSupportFuncValue < ScalarProductValue))
                {
                    Qi1SetSupportFuncValue = ScalarProductValue;
                }
            }
            if (Double.IsNaN(Qi1SetSupportFuncValue)) Qi1SetSupportFuncValue = 0;

            Double Qi2SetSupportFuncValue = Double.NaN;
            foreach (Point3D CurrentPoint in Qi2Set)
            {
                Double ScalarProductValue = VectorArg.XCoord * CurrentPoint.XCoord + VectorArg.YCoord * CurrentPoint.YCoord + VectorArg.ZCoord * CurrentPoint.ZCoord;
                if ((Double.IsNaN(Qi2SetSupportFuncValue)) || (Qi2SetSupportFuncValue < ScalarProductValue))
                {
                    Qi2SetSupportFuncValue = ScalarProductValue;
                }
            }
            if (Double.IsNaN(Qi2SetSupportFuncValue)) Qi2SetSupportFuncValue = 0;

            return PolyhedronSupportFuncValue + Pi1SetSupportFuncValue + Pi2SetSupportFuncValue - Qi1SetSupportFuncValue - Qi2SetSupportFuncValue;
        }

        /// <summary>
        /// ����� SolveCone123EquationSystem ������ ������� ���. ��������� (3x3); ������� ������������ ��� �������� ��������� ���������� ����� 1-2 (��. ��������)
        /// </summary>
        /// <param name="ConeVector1"></param>
        /// <param name="ConeVector2"></param>
        /// <param name="ConeVector3"></param>
        /// <returns></returns>
        private Matrix SolveCone123EquationSystem(Vector3D ConeVector1, Vector3D ConeVector2, Vector3D ConeVector3)
        {
            Double ConeVector1FuncFiValue = FuncFi(ConeVector1, m_PolyhedronVertexList, m_Pi1Set, m_Pi2Set, m_Qi1Set, m_Qi2Set);
            Double ConeVector2FuncFiValue = FuncFi(ConeVector2, m_PolyhedronVertexList, m_Pi1Set, m_Pi2Set, m_Qi1Set, m_Qi2Set);
            Double ConeVector3FuncFiValue = FuncFi(ConeVector3, m_PolyhedronVertexList, m_Pi1Set, m_Pi2Set, m_Qi1Set, m_Qi2Set);

            Matrix MatrixA = new Matrix(3, 3);
            MatrixA[1, 1] = ConeVector1.XCoord;
            MatrixA[1, 2] = ConeVector1.YCoord;
            MatrixA[1, 3] = ConeVector1.ZCoord;
            MatrixA[2, 1] = ConeVector2.XCoord;
            MatrixA[2, 2] = ConeVector2.YCoord;
            MatrixA[2, 3] = ConeVector2.ZCoord;
            MatrixA[3, 1] = ConeVector3.XCoord;
            MatrixA[3, 2] = ConeVector3.YCoord;
            MatrixA[3, 3] = ConeVector3.ZCoord;

            Matrix MatrixB = new Matrix(3, 1);
            MatrixB[1, 1] = ConeVector1FuncFiValue;
            MatrixB[2, 1] = ConeVector2FuncFiValue;
            MatrixB[3, 1] = ConeVector3FuncFiValue;

            return SolveEquationSystem3(MatrixA, MatrixB);

            /*Matrix MatrixA = new Matrix(3, 3);
            MatrixA[1, 1] = ConeVector1.XCoord;
            MatrixA[1, 2] = ConeVector1.YCoord;
            MatrixA[1, 3] = ConeVector1.ZCoord;
            MatrixA[2, 1] = ConeVector2.XCoord;
            MatrixA[2, 2] = ConeVector2.YCoord;
            MatrixA[2, 3] = ConeVector2.ZCoord;
            MatrixA[3, 1] = ConeVector3.XCoord;
            MatrixA[3, 2] = ConeVector3.YCoord;
            MatrixA[3, 3] = ConeVector3.ZCoord;

            Matrix MatrixB = new Matrix(3, 1);
            MatrixB[1, 1] = ConeVector1FuncFiValue;
            MatrixB[2, 1] = ConeVector2FuncFiValue;
            MatrixB[3, 1] = ConeVector3FuncFiValue;

            return SolveEquationSystem3(MatrixA, MatrixB);*/
            /*Double Delta = CalcDeterminant3(ConeVector1.XCoord, ConeVector1.YCoord, ConeVector1.ZCoord, ConeVector2.XCoord, ConeVector2.YCoord, ConeVector2.ZCoord, ConeVector3.XCoord, ConeVector3.YCoord, ConeVector3.ZCoord);
            Double DeltaX = CalcDeterminant3(ConeVector1FuncFiValue, ConeVector1.YCoord, ConeVector1.ZCoord, ConeVector2FuncFiValue, ConeVector2.YCoord, ConeVector2.ZCoord, ConeVector3FuncFiValue, ConeVector3.YCoord, ConeVector3.ZCoord);
            Double DeltaY = CalcDeterminant3(ConeVector1.XCoord, ConeVector1FuncFiValue, ConeVector1.ZCoord, ConeVector2.XCoord, ConeVector2FuncFiValue, ConeVector2.ZCoord, ConeVector3.XCoord, ConeVector3FuncFiValue, ConeVector3.ZCoord);
            Double DeltaZ = CalcDeterminant3(ConeVector1.XCoord, ConeVector1.YCoord, ConeVector1FuncFiValue, ConeVector2.XCoord, ConeVector2.YCoord, ConeVector2FuncFiValue, ConeVector3.XCoord, ConeVector3.YCoord, ConeVector3FuncFiValue);

            Matrix Solution = new Matrix(3, 1);
            Solution[1, 1] = DeltaX / Delta;
            Solution[2, 1] = DeltaY / Delta;
            Solution[3, 1] = DeltaZ / Delta;
            return Solution;*/
        }

        /// <summary>
        /// ����� CalcLambda123Koeff ��������� ������������ Lambda1, ..., Lambda3 � ���������� �� � ���� �������-�������
        /// </summary>
        /// <param name="ConeVector1"></param>
        /// <param name="ConeVector2"></param>
        /// <param name="ConeVector3"></param>
        /// <param name="ConeVector4"></param>
        /// <returns></returns>
        private Matrix CalcLambda123Koeff(Vector3D ConeVector1, Vector3D ConeVector2, Vector3D ConeVector3, Vector3D ConeVector4)
        {
            /*Matrix MatrixA = new Matrix(3, 3);
            MatrixA[1, 1] = ConeVector1.XCoord;
            MatrixA[1, 2] = ConeVector2.XCoord;
            MatrixA[1, 3] = ConeVector3.XCoord;
            MatrixA[2, 1] = ConeVector1.YCoord;
            MatrixA[2, 2] = ConeVector2.YCoord;
            MatrixA[2, 3] = ConeVector3.YCoord;
            MatrixA[3, 1] = ConeVector1.ZCoord;
            MatrixA[3, 2] = ConeVector2.ZCoord;
            MatrixA[3, 3] = ConeVector3.ZCoord;

            Matrix MatrixB = new Matrix(3, 1);
            MatrixB[1, 1] = ConeVector4.XCoord;
            MatrixB[2, 1] = ConeVector4.YCoord;
            MatrixB[3, 1] = ConeVector4.ZCoord;

            return SolveEquationSystem3(MatrixA, MatrixB);*/
            Double Delta = CalcDeterminant3(ConeVector1.XCoord, ConeVector2.XCoord, ConeVector3.XCoord, ConeVector1.YCoord, ConeVector2.YCoord, ConeVector3.YCoord, ConeVector1.ZCoord, ConeVector2.ZCoord, ConeVector3.ZCoord);
            Double DeltaX = CalcDeterminant3(ConeVector4.XCoord, ConeVector2.XCoord, ConeVector3.XCoord, ConeVector4.YCoord, ConeVector2.YCoord, ConeVector3.YCoord, ConeVector4.ZCoord, ConeVector2.ZCoord, ConeVector3.ZCoord);
            Double DeltaY = CalcDeterminant3(ConeVector1.XCoord, ConeVector4.XCoord, ConeVector3.XCoord, ConeVector1.YCoord, ConeVector4.YCoord, ConeVector3.YCoord, ConeVector1.ZCoord, ConeVector4.ZCoord, ConeVector3.ZCoord);
            Double DeltaZ = CalcDeterminant3(ConeVector1.XCoord, ConeVector2.XCoord, ConeVector4.XCoord, ConeVector1.YCoord, ConeVector2.YCoord, ConeVector4.YCoord, ConeVector1.ZCoord, ConeVector2.ZCoord, ConeVector4.ZCoord);

            Matrix Solution = new Matrix(3, 1);
            Solution[1, 1] = DeltaX / Delta;
            Solution[2, 1] = DeltaY / Delta;
            Solution[3, 1] = DeltaZ / Delta;
            return Solution;
        }

        /// <summary>
        /// ����� ReplaceConn12WithConn34 �������� ���������� ����� 1-2 �� ����� 3-4 (��. ��������)
        /// </summary>
        /// <param name="PGNode1"></param>
        /// <param name="PGNode2"></param>
        /// <param name="PGNode3"></param>
        /// <param name="PGNode4"></param>
        /// <param name="SuspiciousConnectionSet"></param>
        private void ReplaceConn12WithConn34(PolyhedronGraphNode PGNode1, PolyhedronGraphNode PGNode2, PolyhedronGraphNode PGNode3, PolyhedronGraphNode PGNode4, SuspiciousConnectionSetClass SuspiciousConnectionSet)
        {
            // ������� ����� �� ���� 2 �� ������ ������ ���� 1 (���������� ��� ���� 2)
            PGNode1.RemoveNodeConnection(PGNode2);
            PGNode2.RemoveNodeConnection(PGNode1);

            // ������ ����� �� ���� 3 � ������ ������ ���� 1
            Int32 Conn31Index = PGNode3.GetConnectionIndex(PGNode1);
            // ����� 3-4 ����������� ����� ������ 3-1 (�.�. �� �� �����)
            PGNode3.InsertNodeConnection(Conn31Index, PGNode4);

            // ������ ����� �� ���� 1 � ������ ������ ���� 4
            Int32 Conn41Index = PGNode4.GetConnectionIndex(PGNode1);
            // ����� 4-3 ����������� ����� ����� 4-1 (�.�. �� ��������� �����)
            PGNode4.InsertNodeConnection(Conn41Index + 1, PGNode3);

            // ������� �� ��������� � ����� 1-2 (�� ���)
            SuspiciousConnectionSet.RemoveConnection(PGNode1, PGNode2);
            // ��������� � ����� � �����: 1-3, 1-4, 2-3, 2-4 (�� �� ���, ������� � ������ � ���)
            SuspiciousConnectionSet.AddConnection(PGNode1, PGNode3);
            SuspiciousConnectionSet.AddConnection(PGNode1, PGNode4);
            SuspiciousConnectionSet.AddConnection(PGNode2, PGNode3);
            SuspiciousConnectionSet.AddConnection(PGNode2, PGNode4);
        }

        /// <summary>
        /// ����� RemoveNode2 ������� ���� ����� ����� 1 (��. ��������)
        /// </summary>
        /// <param name="PGNode1"></param>
        /// <param name="PGNode2"></param>
        /// <param name="PGNode3"></param>
        /// <param name="PGNode4"></param>
        /// <param name="SuspiciousConnectionSet"></param>
        private void RemoveNode1(PolyhedronGraphNode PGNode1, PolyhedronGraphNode PGNode2, PolyhedronGraphNode PGNode3, PolyhedronGraphNode PGNode4, SuspiciousConnectionSetClass SuspiciousConnectionSet)
        {
            /*// ������ �������� ���� 1 ���������� ������ �������� ���� 2 ���� �������� ������� ���� 1 � 2, � ����� ���� 3 � 4 (��. ��������)
            RemoveNode2(PGNode2, PGNode1, PGNode4, PGNode3, SuspiciousConnectionSet);*/

            /*// ������� �� ������ � �����, ���������� ���� 1
            SuspiciousConnectionSet.RemoveConnections(PGNode1);
            // ������� ������ �� ���� 1 �� ������ ������ ����� 2,3,4
            PGNode2.RemoveNodeConnection(PGNode1);
            PGNode3.RemoveNodeConnection(PGNode1);
            PGNode4.RemoveNodeConnection(PGNode1);

            //
            Int32 LastAddedConnIndex = PGNode2.GetConnectionIndex(PGNode4);
            //
            Int32 PrevConnIndex = PGNode1.GetConnectionIndex(PGNode4);
            //
            Int32 CurrentConnIndex = (PrevConnIndex == PGNode1.NodeConnectionCount - 1 ? 0 : PrevConnIndex + 1);
            //
            Int32 FinishConnIndex = PGNode1.GetConnectionIndex(PGNode3);
            //
            PolyhedronGraphNode PrevPGNode = PGNode4;
            //
            PolyhedronGraphNode CurrentPGNode = PGNode1[CurrentConnIndex];

            while (CurrentConnIndex != FinishConnIndex)
            {
                //
                Int32 ConnCurrentNode1Index = CurrentPGNode.GetConnectionIndex(PGNode1);
                //
                CurrentPGNode.InsertNodeConnection(ConnCurrentNode1Index, PGNode2);
                CurrentPGNode.RemoveNodeConnection(PGNode1);
                //
                LastAddedConnIndex++;
                PGNode2.InsertNodeConnection(LastAddedConnIndex, CurrentPGNode);

                //
                SuspiciousConnectionSet.AddConnection(PGNode2, CurrentPGNode);
                SuspiciousConnectionSet.AddConnection(PrevPGNode, CurrentPGNode);

                //
                if ((PrevPGNode.GetConnectionIndex(CurrentPGNode) == -1) || (CurrentPGNode.GetConnectionIndex(PrevPGNode) == -1))
                {
                    throw new AlgorithmException("Incorrect graph !!!");
                }

                //
                PrevConnIndex = CurrentConnIndex;
                CurrentConnIndex = (PrevConnIndex == PGNode1.NodeConnectionCount - 1 ? 0 : PrevConnIndex + 1);
                PrevPGNode = CurrentPGNode;
                CurrentPGNode = PGNode1[CurrentConnIndex];
            }

            //
            SuspiciousConnectionSet.AddConnection(PrevPGNode, CurrentPGNode);
            // ��������� � ����� � ����� 2-3 � 2-4
            SuspiciousConnectionSet.AddConnection(PGNode2, PGNode3);
            SuspiciousConnectionSet.AddConnection(PGNode2, PGNode4);

            //
            m_PGNodeList.Remove(PGNode1);*/
        }

        /// <summary>
        /// ����� RemoveNode2 ������� ���� ����� ����� 2 (��. ��������)
        /// </summary>
        /// <param name="PGNode1"></param>
        /// <param name="PGNode2"></param>
        /// <param name="PGNode3"></param>
        /// <param name="PGNode4"></param>
        /// <param name="SuspiciousConnectionSet"></param>
        private void RemoveNode2(PolyhedronGraphNode PGNode1, PolyhedronGraphNode PGNode2, PolyhedronGraphNode PGNode3, PolyhedronGraphNode PGNode4, SuspiciousConnectionSetClass SuspiciousConnectionSet)
        {
            /*// ������� ������ �� ���� 2 �� ������ ������ ��� �����, � �������� ������ ���� 2
            for (Int32 PGConnIndex = 0; PGConnIndex < PGNode2.NodeConnectionCount; PGConnIndex++)
            {
                PGNode2[PGConnIndex].RemoveNodeConnection(PGNode2);
            }
            // ������� ���� 2
            m_PGNodeList.Remove(PGNode2);
            SuspiciousConnectionSet.RemoveConnections(PGNode2);

            // ������ ����� �� ���� 3 � ������ ������ ���� 1
            Int32 Conn13Index = PGNode1.GetConnectionIndex(PGNode3);
            // ������ ������������� ���� ������, ������������ �� ����� 1-3 � ��������������� �� ����� 4-1
            List<PolyhedronGraphNode> ShortestGraphPath = GetShortestGraphPath(PGNode1, PGNode3, PGNode4);
            // ���� �� 2-�� �� N-1 ���� �� ������������ ���� ������
            for (Int32 GraphPathIndex = 1; GraphPathIndex < ShortestGraphPath.Count - 1; GraphPathIndex++)
            {
                // � ������ ������ ���� 1 ��������� ������ �� ������� ���� (�� 2-�� �� Count-1) �� ���� ������
                // ������ ����������� ����� ��������� ����������� ����� ��� ����� 1-3, ���� ��������� ����������� ����� ���
                PGNode1.InsertNodeConnection(Conn13Index + GraphPathIndex, ShortestGraphPath[GraphPathIndex]);

                // � ������ ������ i-�� ���� �� ������������ ���� ������ ��������� (��� ��) ����� ����� ������, �� ������� �� ������ � ���� ����
                Int32 CurrentPathNodeConnFromIndex = ShortestGraphPath[GraphPathIndex].GetConnectionIndex(ShortestGraphPath[GraphPathIndex - 1]);
                ShortestGraphPath[GraphPathIndex].InsertNodeConnection(CurrentPathNodeConnFromIndex, PGNode1);

                // ��������� � ����� � ������ ��� ��������� �����
                SuspiciousConnectionSet.AddConnection(PGNode1, ShortestGraphPath[GraphPathIndex]);
                // ��������� � ����� � ����� ����� (i-1)-� � i-� ������ �� ���������� ���� (�������� ������� ������� �*)
                SuspiciousConnectionSet.AddConnection(ShortestGraphPath[GraphPathIndex - 1], ShortestGraphPath[GraphPathIndex]);
            }
            SuspiciousConnectionSet.AddConnection(ShortestGraphPath[ShortestGraphPath.Count - 2], ShortestGraphPath[ShortestGraphPath.Count - 1]);

            // ��������� � ����� � ����� 1-3 � 1-4
            SuspiciousConnectionSet.AddConnection(PGNode1, PGNode3);
            SuspiciousConnectionSet.AddConnection(PGNode1, PGNode4);*/

            /*// ������� �� ������ � �����, ���������� ���� 2
            SuspiciousConnectionSet.RemoveConnections(PGNode2);
            // ������� ������ �� ���� 2 �� ������ ������ ����� 1,3,4
            PGNode1.RemoveNodeConnection(PGNode2);
            PGNode3.RemoveNodeConnection(PGNode2);
            PGNode4.RemoveNodeConnection(PGNode2);

            //
            Int32 LastAddedConnIndex = PGNode1.GetConnectionIndex(PGNode3);
            //
            Int32 PrevConnIndex = PGNode2.GetConnectionIndex(PGNode3);
            //
            Int32 CurrentConnIndex = (PrevConnIndex == PGNode2.NodeConnectionCount - 1 ? 0 : PrevConnIndex + 1);
            //
            Int32 FinishConnIndex = PGNode2.GetConnectionIndex(PGNode4);
            //
            PolyhedronGraphNode PrevPGNode = PGNode3;
            //
            PolyhedronGraphNode CurrentPGNode = PGNode2[CurrentConnIndex];

            while (CurrentConnIndex != FinishConnIndex)
            {
                //
                Int32 ConnCurrentNode2Index = CurrentPGNode.GetConnectionIndex(PGNode2);
                //
                CurrentPGNode.InsertNodeConnection(ConnCurrentNode2Index, PGNode1);
                CurrentPGNode.RemoveNodeConnection(PGNode2);
                //
                LastAddedConnIndex++;
                PGNode1.InsertNodeConnection(LastAddedConnIndex, CurrentPGNode);

                //
                SuspiciousConnectionSet.AddConnection(PGNode1, CurrentPGNode);
                SuspiciousConnectionSet.AddConnection(PrevPGNode, CurrentPGNode);

                //
                if ((PrevPGNode.GetConnectionIndex(CurrentPGNode) == -1) || (CurrentPGNode.GetConnectionIndex(PrevPGNode) == -1))
                {
                    throw new AlgorithmException("Incorrect graph !!!");
                }

                //
                PrevConnIndex = CurrentConnIndex;
                CurrentConnIndex = (PrevConnIndex == PGNode2.NodeConnectionCount - 1 ? 0 : PrevConnIndex + 1);
                PrevPGNode = CurrentPGNode;
                CurrentPGNode = PGNode2[CurrentConnIndex];
            }

            //
            SuspiciousConnectionSet.AddConnection(PrevPGNode, CurrentPGNode);
            // ��������� � ����� � ����� 1-3 � 1-4
            SuspiciousConnectionSet.AddConnection(PGNode1, PGNode3);
            SuspiciousConnectionSet.AddConnection(PGNode1, PGNode4);

            //
            m_PGNodeList.Remove(PGNode2);*/
        }

        /*/// <summary>
        /// ����� RemoveNode ������� ���� RemovedNode � ������������� ���������� ������
        /// </summary>
        /// <param name="RemovedNode"></param>
        private void RemoveNode(PolyhedronGraphNode RemovedNode)
        {
            // ������� ��������� ���� �� ������ ����� �����
            m_PGNodeList.Remove(RemovedNode);

            List<PolyhedronGraphNode> SectorPGNodeList = new List<PolyhedronGraphNode>();
            // ���� �� ���� ������ ���������� ����
            for (Int32 ConnIndex = 0; ConnIndex < RemovedNode.NodeConnectionCount; ConnIndex++)
            {
                // ����, ��������� � ��������� ������� ������, ���������� ������� �����
                PolyhedronGraphNode CurrentConnNode = RemovedNode[ConnIndex];
                PolyhedronGraphNode NextConnNode = (ConnIndex == RemovedNode.NodeConnectionCount - 1 ? RemovedNode[0] : RemovedNode[ConnIndex + 1]);
                // ��������� ������� ���� � ����� �������������� (������ �.�.) ������ ����� �������
                SectorPGNodeList.Add(CurrentConnNode);
                // ������� ������ �� ��������� ���� �� ������ ������ �������� ����
                CurrentConnNode.RemoveNodeConnection(RemovedNode);
            }

            // ������� ����� ���������� ������ ���� �� �������������� ������ ����� �������
            PolyhedronGraphNode CurrentPGNode = SectorPGNodeList[0];
            Int32 CurrentPGNodeIndex = 0;

            while (SectorPGNodeList.Count > 3)
            {
                // ����� ����� 1 ���������� ������� ����
                PolyhedronGraphNode PGNode1 = CurrentPGNode;
                // ����� ����� 2 ���������� ��������� (������������ ���� 1) �� ����������� �������� (�� �������������� ������ ����� �������)
                PolyhedronGraphNode PGNode2 = (CurrentPGNodeIndex + 1 >= SectorPGNodeList.Count ? SectorPGNodeList[CurrentPGNodeIndex + 1 - SectorPGNodeList.Count] : SectorPGNodeList[CurrentPGNodeIndex + 1]);
                // ����� ����� 3 ���������� ��������� (������������ ���� 2) �� ����������� �������� (�� �������������� ������ ����� �������)
                PolyhedronGraphNode PGNode3 = (CurrentPGNodeIndex + 2 >= SectorPGNodeList.Count ? SectorPGNodeList[CurrentPGNodeIndex + 2 - SectorPGNodeList.Count] : SectorPGNodeList[CurrentPGNodeIndex + 2]);

                // ��������� ��������� ������������ ��������, ��������� � ������ 2, 1 � 3
                Double MixedProduct213Value = Vector3D.MixedProduct(PGNode2.NodeNormal, PGNode1.NodeNormal, PGNode3.NodeNormal);
                // ��������� ��������� ������������ ��������, ��������� � ��������� ����� � ������ 1 � 3
                Double MixedProductR13Value = Vector3D.MixedProduct(RemovedNode.NodeNormal, PGNode1.NodeNormal, PGNode3.NodeNormal);

                // ���� ����� ����������� ��������� ������������ ������, �� ����� 1-3 ����� ���� ��������� 
                Boolean IsConn13Correct = ((MixedProduct213Value > Epsilon) && (MixedProductR13Value < -Epsilon)) || ((MixedProductR13Value > Epsilon) && (MixedProduct213Value < -Epsilon)) || ((MixedProductR13Value > -Epsilon) && (MixedProductR13Value < Epsilon));

                if (IsConn13Correct)
                {
                    Int32 Conn12Index = PGNode1.GetConnectionIndex(PGNode2);
                    Int32 Conn32Index = PGNode3.GetConnectionIndex(PGNode2);

                    // � ������ ������ ���� 1 ����� ������ �� ���� 2 ��������� ������ �� ���� 3
                    PGNode1.InsertNodeConnection(Conn12Index + 1, PGNode3);
                    // � ������ ������ ���� 3 ����� ������� �� ���� 2 ��������� ������ �� ���� 1
                    PGNode3.InsertNodeConnection(Conn32Index, PGNode1);
                    // ������� ���� 2 �� �������������� ������ ����� �������
                    SectorPGNodeList.Remove(PGNode2);

                    if (CurrentPGNodeIndex == SectorPGNodeList.Count) CurrentPGNodeIndex--;
                }
                else
                {
                    // ������� ���������� ���� ����� 2
                    CurrentPGNode = PGNode2;
                    CurrentPGNodeIndex = (CurrentPGNodeIndex == SectorPGNodeList.Count - 1 ? 0 : CurrentPGNodeIndex + 1);
                }
            }
        }*/

        /// <summary>
        /// ����� RemoveNode ������� ���� RemovedNode � ������������� ���������� ������
        /// </summary>
        /// <param name="RemovedNode"></param>
        /// <param name="SuspiciousConnectionSet"></param>
        private void RemoveNode(PolyhedronGraphNode RemovedNode, SuspiciousConnectionSetClass SuspiciousConnectionSet)
        {
            // ������� ��������� ���� �� ������ ����� �����
            m_PGNodeList.Remove(RemovedNode);
            // ������� �� ������ � ��� ����� ���������� ��������� ����
            SuspiciousConnectionSet.RemoveConnections(RemovedNode);

            List<PolyhedronGraphNode> SectorPGNodeList = new List<PolyhedronGraphNode>();
            // ���� �� ���� ������ ���������� ����
            for (Int32 ConnIndex = 0; ConnIndex < RemovedNode.NodeConnectionCount; ConnIndex++)
            {
                // ����, ��������� � ��������� ������� ������, ���������� ������� �����
                PolyhedronGraphNode CurrentConnNode = RemovedNode[ConnIndex];
                PolyhedronGraphNode NextConnNode = (ConnIndex == RemovedNode.NodeConnectionCount - 1 ? RemovedNode[0] : RemovedNode[ConnIndex + 1]);
                // ��������� ������� ���� � ����� �������������� (������ �.�.) ������ ����� �������
                SectorPGNodeList.Add(CurrentConnNode);
                // ������� ������ �� ��������� ���� �� ������ ������ �������� ����
                CurrentConnNode.RemoveNodeConnection(RemovedNode);
                // ��������� � ����� � �����, ����������� ������� � ��������� (����, ��������� � ��������� ��������� �� ��������� � ������� ������) ����
                SuspiciousConnectionSet.AddConnection(CurrentConnNode, NextConnNode);
            }

            // ������� ����� ���������� ������ ���� �� �������������� ������ ����� �������
            PolyhedronGraphNode CurrentPGNode = SectorPGNodeList[0];
            Int32 CurrentPGNodeIndex = 0;

            Int32 IterationNumber = 0;
            while (SectorPGNodeList.Count > 3)
            {
                // ����� ����� 1 ���������� ������� ����
                PolyhedronGraphNode PGNode1 = CurrentPGNode;
                // ����� ����� 2 ���������� ��������� (������������ ���� 1) �� ����������� �������� (�� �������������� ������ ����� �������)
                PolyhedronGraphNode PGNode2 = (CurrentPGNodeIndex + 1 >= SectorPGNodeList.Count ? SectorPGNodeList[CurrentPGNodeIndex + 1 - SectorPGNodeList.Count] : SectorPGNodeList[CurrentPGNodeIndex + 1]);
                // ����� ����� 3 ���������� ��������� (������������ ���� 2) �� ����������� �������� (�� �������������� ������ ����� �������)
                PolyhedronGraphNode PGNode3 = (CurrentPGNodeIndex + 2 >= SectorPGNodeList.Count ? SectorPGNodeList[CurrentPGNodeIndex + 2 - SectorPGNodeList.Count] : SectorPGNodeList[CurrentPGNodeIndex + 2]);
                // ����� ����� 4 ���������� ��������� (������������ ���� 3) �� ����������� �������� (�� �������������� ������ ����� �������)
                PolyhedronGraphNode PGNode4 = (CurrentPGNodeIndex + 3 >= SectorPGNodeList.Count ? SectorPGNodeList[CurrentPGNodeIndex + 3 - SectorPGNodeList.Count] : SectorPGNodeList[CurrentPGNodeIndex + 3]);

                // ��������� ��������� ������������ ��������, ��������� � ������ 2, 1 � 3
                Double MixedProduct213Value = Vector3D.MixedProduct(PGNode2.NodeNormal, PGNode1.NodeNormal, PGNode3.NodeNormal);
                // ��������� ��������� ������������ ��������, ��������� � ������ 4, 1 � 3
                Double MixedProduct413Value = Vector3D.MixedProduct(PGNode4.NodeNormal, PGNode1.NodeNormal, PGNode3.NodeNormal);
                // ��������� ��������� ������������ ��������, ��������� � ��������� ����� � ������ 1 � 3
                Double MixedProductR13Value = Vector3D.MixedProduct(RemovedNode.NodeNormal, PGNode1.NodeNormal, PGNode3.NodeNormal);

                // ���� ����� ����������� ��������� ������������ ������, �� ����� 1-3 ����� ���� ��������� 
                Boolean IsConn13Correct = ((MixedProduct213Value > Epsilon) && (MixedProductR13Value < -Epsilon)) || ((MixedProductR13Value > Epsilon) && (MixedProduct213Value < -Epsilon)) || ((MixedProductR13Value > -Epsilon) && (MixedProductR13Value < Epsilon));

                // ���� ����� ����������� ��������� ������������ MixedProduct213Value � MixedProductR13Value ������, �� ����� 1-3 ����� ���� ���������
                //Boolean IsConn13Correct = ((MixedProduct213Value > Epsilon) && (MixedProductR13Value < -Epsilon)) || ((MixedProductR13Value > Epsilon) && (MixedProduct213Value < -Epsilon)) || ((MixedProductR13Value > -Epsilon) && (MixedProductR13Value < Epsilon));
                // Boolean IsConn13Correct = ((MixedProduct213Value > Epsilon) && (MixedProductR13Value < -Epsilon)) || ((MixedProductR13Value > Epsilon) && (MixedProduct213Value < -Epsilon));
                // ���� MixedProductR13Value = 0 � ����� ����������� ��������� ������������ MixedProduct213Value � MixedProduct413Value ������, �� ����� 1-3 ����� ���� ���������
                // IsConn13Correct = IsConn13Correct || ((MixedProduct213Value > Epsilon) && (MixedProduct413Value < -Epsilon)) || ((MixedProduct413Value > Epsilon) && (MixedProduct213Value < -Epsilon));
                //Boolean IsConn13Correct = (MixedProduct213Value * MixedProductR13Value < 0) || ((MixedProductR13Value == 0) && (MixedProduct213Value * MixedProduct413Value < 0));

                if (IsConn13Correct)
                {
                    Int32 Conn12Index = PGNode1.GetConnectionIndex(PGNode2);
                    Int32 Conn32Index = PGNode3.GetConnectionIndex(PGNode2);

                    // � ������ ������ ���� 1 ����� ������ �� ���� 2 ��������� ������ �� ���� 3
                    PGNode1.InsertNodeConnection(Conn12Index + 1, PGNode3);
                    // � ������ ������ ���� 3 ����� ������� �� ���� 2 ��������� ������ �� ���� 1
                    PGNode3.InsertNodeConnection(Conn32Index, PGNode1);
                    // ������� ���� 2 �� �������������� ������ ����� �������
                    SectorPGNodeList.Remove(PGNode2);

                    if (CurrentPGNodeIndex == SectorPGNodeList.Count) CurrentPGNodeIndex--;
                    
                    // ��������� � ����� � ����� 1-3
                    SuspiciousConnectionSet.AddConnection(PGNode1, PGNode3);

                    IterationNumber = 0;
                }
                else
                {
                    // ������� ���������� ���� ����� 2
                    CurrentPGNode = PGNode2;
                    CurrentPGNodeIndex = (CurrentPGNodeIndex == SectorPGNodeList.Count - 1 ? 0 : CurrentPGNodeIndex + 1);

                    IterationNumber++;
                }

                if (IterationNumber == SectorPGNodeList.Count)
                {
                    throw new AlgorithmException("!!!");
                }
            }
        }

        /*/// <summary>
        /// ����� RemoveNode ������� ���� RemovedNode � ������������� ���������� ������
        /// </summary>
        /// <param name="RemovedNode"></param>
        /// <param name="SuspiciousConnectionSet"></param>
        private void RemoveNode(PolyhedronGraphNode RemovedNode, SuspiciousConnectionSetClass SuspiciousConnectionSet)
        {
            // ������� ��������� ���� �� ������ ����� �����
            m_PGNodeList.Remove(RemovedNode);
            // ������� �� ������ � ��� ����� ���������� ��������� ����
            SuspiciousConnectionSet.RemoveConnections(RemovedNode);

            List<PolyhedronGraphNode> SectorPGNodeList = new List<PolyhedronGraphNode>();
            // ���� �� ���� ������ ���������� ����
            for (Int32 ConnIndex = 0; ConnIndex < RemovedNode.NodeConnectionCount; ConnIndex++)
            {
                // ����, ��������� � ��������� ������� ������, ���������� ������� �����
                PolyhedronGraphNode CurrentConnNode = RemovedNode[ConnIndex];
                PolyhedronGraphNode NextConnNode = (ConnIndex == RemovedNode.NodeConnectionCount - 1 ? RemovedNode[0] : RemovedNode[ConnIndex + 1]);
                // ��������� ������� ���� � ����� �������������� (������ �.�.) ������ ����� �������
                SectorPGNodeList.Add(CurrentConnNode);
                // ������� ������ �� ��������� ���� �� ������ ������ �������� ����
                CurrentConnNode.RemoveNodeConnection(RemovedNode);
                // ��������� � ����� � �����, ����������� ������� � ��������� (����, ��������� � ��������� ��������� �� ��������� � ������� ������) ����
                SuspiciousConnectionSet.AddConnection(CurrentConnNode, NextConnNode);
            }

            // ������������ ����������� �������
            TriangulateGraphSector(SectorPGNodeList, SuspiciousConnectionSet);
        }*/


        /// <summary>
        /// ������������ ������� graphSector �����
        /// </summary>
        /// <param name="graphSector"></param>
        /// <param name="SuspiciousConnectionSet"></param>
        // �������� ������������ ������� �� �����������:
        //
        // ���� � ���������� ��� ������������ ������� 3 ����, �� ������ ��������������� �� ����� - ����� �� ���������
        // 
        // ���� (�� ���� ����� ���������������� �������)
        // {
        //      ����� ������� ����; �������� ��� ��������� �����
        //      ���� �� ���� ����� �� ������� ������� � ���������� + 2 � ���������� ��������� - 2 (������ ����� ������� - �����������)
        //      {
        //          ����� ������� ���� (�� ����������� �����); �������� ��� �������� �����
        //          
        //          ���� ��� ���� �� ������� ����� ��������� � �������� ����� � ����� �������
        //             � ��� ���� ����� �������� � ��������� � ������, ��
        //          {
        //              ������ ����� ����� ��������� � �������� ������
        //              ��������� ��� ����� � ������ �������������� ������
        //
        //              ������������� ������, ������������ �� ����� ����� ��������� � �������� ������ (���� � �������� ������������ ��� ������ �������)
        //              ������������� ������, ������������ �� ����� ����� �������� � ��������� ������ (���� � �������� ������������ ��� ������ �������)
        //
        //              ����� �� ���������
        //          }
        //      }
        // }
        // 
        // ���� ��������� � ���� ����� ���������, ��� �������� ��� ������������ ������� ���������� ... �.�. ������
        private void TriangulateGraphSector(List<PolyhedronGraphNode> graphSector, SuspiciousConnectionSetClass suspiciousConnectionSet)
        {
            // ���� � ������� 3 ����, �� ��� ��������������� �� �����
            if (graphSector.Count == 3) return;

            // ���� (�� ���� ����� ���������������� �������)
            for (Int32 nodeSectorIndex = 0; nodeSectorIndex < graphSector.Count; nodeSectorIndex++)
            {
                //  ����� ������� ����; �������� ��� ��������� �����
                PolyhedronGraphNode startNode = graphSector[nodeSectorIndex];

                // ���� �� ���� ����� �� ������� ������� � ���������� + 2 � ���������� ��������� - 2 ������ ����� ������� - �����������)
                // �.�. ������� � ���������� + 2, ���� != ��������� - 1 (����� ��������� - 2 �� ����� �� ������������)
                Int32 startSearchIndex = (nodeSectorIndex <= graphSector.Count - 3 ? nodeSectorIndex + 2 : nodeSectorIndex + 2 - graphSector.Count);
                Int32 finishSearchIndex = (nodeSectorIndex == 0 ? graphSector.Count - 1 : nodeSectorIndex - 1);
                for (Int32 currentSearchIndex = startSearchIndex; currentSearchIndex != finishSearchIndex; )
                {
                    // ����� ������� ���� (�� ����������� �����); �������� ��� �������� �����
                    PolyhedronGraphNode finishNode = graphSector[currentSearchIndex];

                    Int32 testStartIndex;
                    Int32 testFinishIndex;

                    // ��������� ��� �� ���� �� ������� ����� ��������� � �������� ����� � ����� �������
                    Boolean IsStartFinishSectorCorrect = true;
                    Double mixedProductValue1 = Double.NaN;
                    testStartIndex = (nodeSectorIndex == graphSector.Count - 1 ? 0 : nodeSectorIndex + 1);
                    testFinishIndex = currentSearchIndex;
                    for (Int32 textIndex = testStartIndex; textIndex != testFinishIndex; )
                    {
                        // �������� �������� ����, ��� ��� ��������������� ���� ����� � ����� �������
                        Double mixedProductValue = Vector3D.MixedProduct(startNode.NodeNormal,
                                                                         finishNode.NodeNormal,
                                                                         graphSector[textIndex].NodeNormal);
                        
                        if (Double.IsNaN(mixedProductValue1)) mixedProductValue1 = mixedProductValue;
                        // epsilon ???
                        if (mixedProductValue * mixedProductValue1 <= 0)
                        {
                            IsStartFinishSectorCorrect = false;
                            break;
                        }

                        textIndex = (textIndex == graphSector.Count - 1 ? 0 : textIndex + 1);
                    }

                    // ��������� ��� �� ���� �� ������� ����� �������� � ��������� ����� � ������ �������
                    Boolean IsFinishStartSectorCorrect = true;
                    Double mixedProductValue2 = Double.NaN;
                    testStartIndex = (currentSearchIndex == graphSector.Count - 1 ? 0 : currentSearchIndex + 1);
                    testFinishIndex = nodeSectorIndex;
                    for (Int32 textIndex = testStartIndex; textIndex != testFinishIndex; )
                    {
                        // �������� �������� ����, ��� ��� ��������������� ���� ����� � ������ �������
                        Double mixedProductValue = Vector3D.MixedProduct(startNode.NodeNormal,
                                                                         finishNode.NodeNormal,
                                                                         graphSector[textIndex].NodeNormal);

                        if (Double.IsNaN(mixedProductValue2)) mixedProductValue2 = mixedProductValue;
                        // epsilon ???
                        if (mixedProductValue * mixedProductValue2 <= 0)
                        {
                            IsFinishStartSectorCorrect = false;
                            break;
                        }
                        if (mixedProductValue1*mixedProductValue2 > 0)
                        {
                            IsFinishStartSectorCorrect = false;
                            break;
                        }

                        textIndex = (textIndex == graphSector.Count - 1 ? 0 : textIndex + 1);
                    }

                    if (!IsStartFinishSectorCorrect || !IsFinishStartSectorCorrect) continue;

                    // ������ ����� ����� ��������� � �������� ������
                    // ������ �� finishNode ��������� ����� ������ �� ��������� � ������� ����
                    PolyhedronGraphNode startNextNode = (nodeSectorIndex == graphSector.Count - 1 ? graphSector[0] : graphSector[nodeSectorIndex + 1]);
                    startNode.InsertNodeConnection(startNode.GetConnectionIndex(startNextNode) + 1, finishNode);
                    // ������ �� startNode ��������� ����� ������ �� ��������� � ������� ����
                    PolyhedronGraphNode finishNextNode = (currentSearchIndex == graphSector.Count - 1 ? graphSector[0] : graphSector[currentSearchIndex + 1]);
                    finishNode.InsertNodeConnection(finishNode.GetConnectionIndex(finishNextNode) + 1, startNode);

                    //  ��������� ��� ����� � ������ �������������� ������
                    suspiciousConnectionSet.AddConnection(startNode, finishNode);

                    // ������������� ������, ������������ �� ����� ����� ��������� � �������� ������ (���� � �������� ������������ ��� ������ �������)
                    List<PolyhedronGraphNode> startFinishSector = new List<PolyhedronGraphNode>();
                    for (Int32 copyNodeIndex = nodeSectorIndex; copyNodeIndex != currentSearchIndex; )
                    {
                        startFinishSector.Add(graphSector[copyNodeIndex]);
                        copyNodeIndex = (copyNodeIndex == graphSector.Count - 1 ? 0 : copyNodeIndex + 1);
                    }
                    startFinishSector.Add(graphSector[currentSearchIndex]);
                    TriangulateGraphSector(startFinishSector, suspiciousConnectionSet);

                    // ������������� ������, ������������ �� ����� ����� �������� � ��������� ������ (���� � �������� ������������ ��� ������ �������)
                    List<PolyhedronGraphNode> finishStartSector = new List<PolyhedronGraphNode>();
                    for (Int32 copyNodeIndex = currentSearchIndex; copyNodeIndex != nodeSectorIndex; )
                    {
                        finishStartSector.Add(graphSector[copyNodeIndex]);
                        copyNodeIndex = (copyNodeIndex == graphSector.Count - 1 ? 0 : copyNodeIndex + 1);
                    }
                    finishStartSector.Add(graphSector[nodeSectorIndex]);
                    TriangulateGraphSector(finishStartSector, suspiciousConnectionSet);

                    // ����� �� ���������
                    return;

                    currentSearchIndex = (currentSearchIndex == graphSector.Count - 1 ? 0 : currentSearchIndex + 1);
                }
            }

            // ���� ��������� � ���� ����� ���������, ��� �������� 
            // ��� ������������ ������� ���������� ... �.�. ������
            throw new Exception("!!!!!!!");
        }

        /// <summary>
        /// ����� SolutionIteration �������� ����� ��������� ������� �������� ���������������� ����
        /// </summary>
        private void SolutionIteration(Vector3D QiDirectingVector)
        {
            SuspiciousConnectionSetClass SuspiciousConnectionSet = GetSuspiciousConnectionsList(QiDirectingVector);

            while (SuspiciousConnectionSet.ConnectionCount > 0)
            {
                // �������� ����� 1-2
                PolyhedronGraphNode[] CurrentConnPGNodes = SuspiciousConnectionSet.GetConnection(0);
                PolyhedronGraphNode PGNode1 = CurrentConnPGNodes[0];
                PolyhedronGraphNode PGNode2 = CurrentConnPGNodes[1];

                // ����� ����� 1-2 � ������ ������ 1-�� ����
                Int32 Conn12Index = PGNode1.GetConnectionIndex(PGNode2);

                // ���� 3; ����� 1-3 ���������� �� ��������� � ����� 1-2
                PolyhedronGraphNode PGNode3 = (Conn12Index == 0 ? PGNode1[PGNode1.NodeConnectionCount - 1] : PGNode1[Conn12Index - 1]);
                // ���� 4; ����� 1-4 ��������� �� ��������� � ����� 1-2
                PolyhedronGraphNode PGNode4 = (Conn12Index == PGNode1.NodeConnectionCount - 1 ? PGNode1[0] : PGNode1[Conn12Index + 1]);

                // ������� ������� ���. ��������� (3x3), ������������ ��� �������� ����� 1-2 �� ��������� ���������� (��. ��������)
                Matrix Cone123SystemSolution = SolveCone123EquationSystem(PGNode1.NodeNormal, PGNode2.NodeNormal, PGNode3.NodeNormal);
                // �������� ����� 1-2 �� ��������� ����������
                Double ConeVector4FuncFiValue = FuncFi(PGNode4.NodeNormal, m_PolyhedronVertexList, m_Pi1Set, m_Pi2Set, m_Qi1Set, m_Qi2Set);
                Double LocalConvexCriterionValue = Cone123SystemSolution[1, 1] * PGNode4.NodeNormal.XCoord +
                                                   Cone123SystemSolution[2, 1] * PGNode4.NodeNormal.YCoord +
                                                   Cone123SystemSolution[3, 1] * PGNode4.NodeNormal.ZCoord;
                // ���� ����� �������
                // LocalConvexCriterionValue<=ConeVector4FuncFiValue, ��� ��� �� �� ����� ConeVector4FuncFiValue-LocalConvexCriterionValue>=0
                if (ConeVector4FuncFiValue-LocalConvexCriterionValue >= -Epsilon)
                {
                    SuspiciousConnectionSet.RemoveConnection(0);
                }
                // ���� ����� �� �������
                else
                {
                    Matrix LambdaColumn = CalcLambda123Koeff(PGNode1.NodeNormal, PGNode2.NodeNormal, PGNode3.NodeNormal, PGNode4.NodeNormal);
                    Double Lambda1 = LambdaColumn[1, 1];
                    Double Lambda2 = LambdaColumn[2, 1];
                    Double Lambda3 = LambdaColumn[3, 1];
                    
                    if (Lambda3 >= 0)
                    {
                        // Lambda3 must be < 0 ... may be exception
                        throw new Exception("Lambda3 >= 0");
                    }
                    // Lambda1>0 && Lambda2>0
                    if ((Lambda1 > Epsilon) && (Lambda2 > Epsilon))
                    {
                        // ����� 1-2 �������� �� ����� 3-4 ...
                        ReplaceConn12WithConn34(PGNode1, PGNode2, PGNode3, PGNode4, SuspiciousConnectionSet);
                    }
                    // Lambda1>0 && Lambda2<=0
                    if ((Lambda1 > Epsilon) && (Lambda2 <= Epsilon))
                    {
                        // ������� ���� 1 ...
                        //RemoveNode1(PGNode1, PGNode2, PGNode3, PGNode4, SuspiciousConnectionSet);
                        RemoveNode(PGNode1, SuspiciousConnectionSet);
                    }
                    // Lambda1<=0 && Lambda2>0
                    if ((Lambda1 <= Epsilon) && (Lambda2 > Epsilon))
                    {
                        // ������� ���� 2 ...
                        //RemoveNode2(PGNode1, PGNode2, PGNode3, PGNode4, SuspiciousConnectionSet);
                        RemoveNode(PGNode2, SuspiciousConnectionSet);
                    }
                    // Lambda1<=0 && Lambda2<=0
                    if ((Lambda1 <= Epsilon) && (Lambda2 <= Epsilon))
                    {
                        // W(i+1) = 0
                        throw new SolutionNotExistException("W(i+1)=0. Solution does not exist !!!");
                    }
                }
            }
        }

        /*/// <summary>
        /// ����� GetPlanesCrossingPoint ���������� ����� ����������� ���� ���������� Plane1, Plane2 � Plane3
        /// </summary>
        /// <param name="Plane1"></param>
        /// <param name="Plane2"></param>
        /// <param name="Plane3"></param>
        /// <returns></returns>
        private Point3D GetPlanesCrossingPoint(PlaneClass Plane1, PlaneClass Plane2, PlaneClass Plane3)
        {
            Matrix AMatrix = new Matrix(3, 3);
            AMatrix[1, 1] = Plane1.AKoeff;
            AMatrix[1, 2] = Plane1.BKoeff;
            AMatrix[1, 3] = Plane1.CKoeff;
            AMatrix[2, 1] = Plane2.AKoeff;
            AMatrix[2, 2] = Plane2.BKoeff;
            AMatrix[2, 3] = Plane2.CKoeff;
            AMatrix[3, 1] = Plane3.AKoeff;
            AMatrix[3, 2] = Plane3.BKoeff;
            AMatrix[3, 3] = Plane3.CKoeff;

            Matrix BMatrix = new Matrix(3, 1);
            BMatrix[1, 1] = -Plane1.DKoeff;
            BMatrix[2, 1] = -Plane2.DKoeff;
            BMatrix[3, 1] = -Plane3.DKoeff;

            Matrix SolutionMatrix = SolveEquationSystem3(AMatrix, BMatrix);

            // ���������� ����� ����������� ��������� �� PointCoordDigits ������ ����� �������
            // ��� ����� ����� ��������� ����������� ����������
            Double XCoord = Math.Round(SolutionMatrix[1, 1], PointCoordDigits);
            Double YCoord = Math.Round(SolutionMatrix[2, 1], PointCoordDigits);
            Double ZCoord = Math.Round(SolutionMatrix[3, 1], PointCoordDigits);

            return new Point3D(XCoord, YCoord, ZCoord);
        }*/

        /// <summary>
        /// ����� CalcDeterminant3 ��������� ������������ ������� 3x3
        /// </summary>
        /// <param name="a11"></param>
        /// <param name="a12"></param>
        /// <param name="a13"></param>
        /// <param name="a21"></param>
        /// <param name="a22"></param>
        /// <param name="a23"></param>
        /// <param name="a31"></param>
        /// <param name="a32"></param>
        /// <param name="a33"></param>
        /// <returns></returns>
        private Double CalcDeterminant3(Double a11, Double a12, Double a13, Double a21, Double a22, Double a23, Double a31, Double a32, Double a33)
        {
            Double Result = 0;

            Result += a11 * (a22 * a33 - a23 * a32);
            Result += a12 * (a23 * a31 - a21 * a33);
            Result += a13 * (a21 * a32 - a22 * a31);

            return Result;
        }
        
        /// <summary>
        /// ����� GetPlanesCrossingPoint ���������� ����� ����������� ���� ���������� Plane1, Plane2 � Plane3
        /// </summary>
        /// <param name="Plane1"></param>
        /// <param name="Plane2"></param>
        /// <param name="Plane3"></param>
        /// <returns></returns>
        private Point3D GetPlanesCrossingPoint(PlaneClass Plane1, PlaneClass Plane2, PlaneClass Plane3)
        {
            Double A1 = Plane1.AKoeff;
            Double B1 = Plane1.BKoeff;
            Double C1 = Plane1.CKoeff;
            Double D1 = Plane1.DKoeff;
            Double A2 = Plane2.AKoeff;
            Double B2 = Plane2.BKoeff;
            Double C2 = Plane2.CKoeff;
            Double D2 = Plane2.DKoeff;
            Double A3 = Plane3.AKoeff;
            Double B3 = Plane3.BKoeff;
            Double C3 = Plane3.CKoeff;
            Double D3 = Plane3.DKoeff;

            Matrix MatrixA = new Matrix(3, 3);
            MatrixA[1, 1] = A1;
            MatrixA[1, 2] = B1;
            MatrixA[1, 3] = C1;
            MatrixA[2, 1] = A2;
            MatrixA[2, 2] = B2;
            MatrixA[2, 3] = C2;
            MatrixA[3, 1] = A3;
            MatrixA[3, 2] = B3;
            MatrixA[3, 3] = C3;

            Matrix MatrixB = new Matrix(3, 1);
            MatrixB[1, 1] = -D1;
            MatrixB[2, 1] = -D2;
            MatrixB[3, 1] = -D3;

            Matrix Solution = SolveEquationSystem3(MatrixA, MatrixB);

            // ���������� ����� ����������� ��������� �� PointCoordDigits ������ ����� �������
            // ��� ����� ����� ��������� ����������� ����������
            Double XCoord = Math.Round(Solution[1, 1], PointCoordDigits);
            Double YCoord = Math.Round(Solution[2, 1], PointCoordDigits);
            Double ZCoord = Math.Round(Solution[3, 1], PointCoordDigits);

            return new Point3D(XCoord, YCoord, ZCoord);

            /*// ���� ���������� ����� ������� ... ����� �������
            Double A1 = Plane1.AKoeff;
            Double B1 = Plane1.BKoeff;
            Double C1 = Plane1.CKoeff;
            Double D1 = Plane1.DKoeff;
            Double A2 = Plane2.AKoeff;
            Double B2 = Plane2.BKoeff;
            Double C2 = Plane2.CKoeff;
            Double D2 = Plane2.DKoeff;
            Double A3 = Plane3.AKoeff;
            Double B3 = Plane3.BKoeff;
            Double C3 = Plane3.CKoeff;
            Double D3 = Plane3.DKoeff;

            Double Delta = CalcDeterminant3(A1, B1, C1, A2, B2, C2, A3, B3, C3);
            Double DeltaX = CalcDeterminant3(-D1, B1, C1, -D2, B2, C2, -D3, B3, C3);
            Double DeltaY = CalcDeterminant3(A1, -D1, C1, A2, -D2, C2, A3, -D3, C3);
            Double DeltaZ = CalcDeterminant3(A1, B1, -D1, A2, B2, -D2, A3, B3, -D3);

            // ���������� ����� ����������� ��������� �� PointCoordDigits ������ ����� �������
            // ��� ����� ����� ��������� ����������� ����������
            Double XCoord = Math.Round((DeltaX / Delta), PointCoordDigits);
            Double YCoord = Math.Round((DeltaY / Delta), PointCoordDigits);
            Double ZCoord = Math.Round((DeltaZ / Delta), PointCoordDigits);

            return new Point3D(XCoord, YCoord, ZCoord);*/
        }

        /// <summary>
        /// ������������� ����������� �����
        /// </summary>
        private void GraphChopper()
        {
        }

        /// <summary>
        /// ����� FindVertexOnPoint ���� ������� (�� �������� �����) � ������ ������ m_PolyhedronVertexList
        /// </summary>
        /// <param name="Point"></param>
        /// <returns></returns>
        private VertexClass FindVertexOnPoint(Point3D SourcePoint)
        {
            VertexClass FindingVertex = null;

            foreach (VertexClass CurrentVertex in m_PolyhedronVertexList)
            {
                if ((CurrentVertex.XCoord == SourcePoint.XCoord) && (CurrentVertex.YCoord == SourcePoint.YCoord) && (CurrentVertex.ZCoord == SourcePoint.ZCoord))
                {
                    FindingVertex = CurrentVertex;
                    break;
                }
            }

            return FindingVertex;
        }

        /// <summary>
        /// ����� RecievePolyhedronStructureFromGraph �������� (���������������) ��������� ������������� �� ��� �����
        /// </summary>
        private void RecievePolyhedronStructureFromGraph()
        {
            List<PlaneClass> PolyhedronPlaneList = new List<PlaneClass>(m_PGNodeList.Count);

            // ������� ���� ����� ������������� �����
            // ��� "��������������" ���� ����� ������� ���� ����� ������������ ���������, � ������� ����� ������� �����
            for (Int32 PGNodeIndex = 0; PGNodeIndex < m_PGNodeList.Count; PGNodeIndex++)
            {
                PolyhedronGraphNode PGNode = m_PGNodeList[PGNodeIndex];
                // ����� �������� ����� ����� ID ���� ����� <> Index ���� + 1
                PGNode.ID = PGNodeIndex + 1;

                Vector3D PlaneNormal = PGNode.NodeNormal;
                Double SupportFuncValue = FuncFi(PlaneNormal, m_PolyhedronVertexList, m_Pi1Set, m_Pi2Set, m_Qi1Set, m_Qi2Set);

                PolyhedronPlaneList.Add(new PlaneClass(PlaneNormal, SupportFuncValue));
            }

            // ��������� ������ ������ � ������ ������������� ������
            m_PolyhedronSideList.Clear();
            m_PolyhedronVertexList.Clear();

            for (Int32 PGNodeIndex = 0; PGNodeIndex < m_PGNodeList.Count; PGNodeIndex++)
            {
                PolyhedronGraphNode CurrentPGNode = m_PGNodeList[PGNodeIndex];
                // CurrentPlane ������������� CurrentPGNode
                // ��������� ����� 1
                PlaneClass CurrentPlane = PolyhedronPlaneList[PGNodeIndex];
                SideClass CurrentSide = new SideClass(new List<VertexClass>(), CurrentPGNode.ID, CurrentPGNode.NodeNormal);
                m_PolyhedronSideList.Add(CurrentSide);

                for (Int32 NodeConnIndex = 0; NodeConnIndex < CurrentPGNode.NodeConnectionCount; NodeConnIndex++)
                {
                    // ������� � �������� ����� 
                    PolyhedronGraphNode CurrentConnPGNode = CurrentPGNode[NodeConnIndex];
                    PolyhedronGraphNode NextConnPGNode = (NodeConnIndex == (CurrentPGNode.NodeConnectionCount - 1) ? CurrentPGNode[0] : CurrentPGNode[NodeConnIndex + 1]);

                    // ���������, ��������������� ������� � �������� ������ (��������� ����� 2 � 3)
                    PlaneClass CurrentConnPlane = PolyhedronPlaneList[CurrentConnPGNode.ID - 1];
                    PlaneClass NextConnPlane = PolyhedronPlaneList[NextConnPGNode.ID - 1];

                    // ����� ����������� ���������� 1, 2 � 3
                    Point3D PlanesCrossingPoint = GetPlanesCrossingPoint(CurrentPlane, CurrentConnPlane, NextConnPlane);

                    VertexClass Vertex4PlanesCrossingPoint = FindVertexOnPoint(PlanesCrossingPoint);
                    if (Vertex4PlanesCrossingPoint == null)
                    {
                        // ???????????????????????????????????????
                        Vertex4PlanesCrossingPoint = new VertexClass(PlanesCrossingPoint, m_PolyhedronVertexList.Count + 1);
                        m_PolyhedronVertexList.Add(Vertex4PlanesCrossingPoint);
                    }

                    if (CurrentSide.VertexCount == 0)
                    {
                        CurrentSide.AddVertex(Vertex4PlanesCrossingPoint);
                    }
                    else if ((CurrentSide[0] != Vertex4PlanesCrossingPoint) && (CurrentSide[CurrentSide.VertexCount - 1] != Vertex4PlanesCrossingPoint))
                    {
                        CurrentSide.AddVertex(Vertex4PlanesCrossingPoint);
                    }
                }
            }
        }

        /// <summary>
        /// ����� InitAlgorithmWork �������������� ������ ���������
        /// </summary>
        private void InitAlgorithmWork()
        {
            RecievePolyhedronStructure();
            RecievePolyhedronGraph();
            GraphTriangulation();
        }

        /*/// <summary>
        /// ����� GetAMatrix ���������� ������� A (��. ��������)
        /// </summary>
        /// <returns></returns>
        private Matrix GetAMatrix()
        {
            // �� ���� ������� A ���� �������� �� �����
            #warning Matrix A must be from input datafile

            #if TASK1
            Double k = -1;
            Matrix AMatrix = new Matrix(3, 3);
            AMatrix[1, 1] = 0;
            AMatrix[1, 2] = 1;
            AMatrix[1, 3] = 0;
            AMatrix[2, 1] = k;
            AMatrix[2, 2] = 0;
            AMatrix[2, 3] = 0;
            AMatrix[3, 1] = 0;
            AMatrix[3, 2] = 0;
            AMatrix[3, 3] = 0;

            #elif TASK2
            Matrix AMatrix = new Matrix(3, 3);
            AMatrix[1, 1] = -0.032;
            AMatrix[1, 2] = 0;
            AMatrix[1, 3] = -0.135;
            AMatrix[2, 1] = 0;
            AMatrix[2, 2] = 0;
            AMatrix[2, 3] = 1;
            AMatrix[3, 1] = 0.27;
            AMatrix[3, 2] = 0;
            AMatrix[3, 3] = -0.014;
            #endif

            return AMatrix;
        }

        /// <summary>
        /// ����� GetB1Matrix ���������� ������� (�������) B1 (��. ��������)
        /// </summary>
        /// <returns></returns>
        private Matrix GetB1Matrix()
        {
            // �� ���� ������� B1 ���� �������� �� �����
            #warning Matrix B1 must be from input datafile

            #if TASK1
            Matrix B1Matrix = new Matrix(3, 1);
            B1Matrix[1, 1] = 0;
            B1Matrix[2, 1] = 1;
            B1Matrix[3, 1] = 0;
            #elif TASK2
            Matrix B1Matrix = new Matrix(3, 1);
            B1Matrix[1, 1] = 2.577;
            B1Matrix[2, 1] = 0;
            B1Matrix[3, 1] = 0.288;
            #endif

            return B1Matrix;
        }

        /// <summary>
        /// ����� GetB2Matrix ���������� ������� (�������) B2 (��. ��������)
        /// </summary>
        /// <returns></returns>
        private Matrix GetB2Matrix()
        {
            // �� ���� ������� B2 ���� �������� �� �����
            #warning Matrix B2 must be from input datafile

            #if TASK1
            Matrix B2Matrix = new Matrix(3, 1);
            B2Matrix[1, 1] = 1;
            B2Matrix[2, 1] = 0;
            B2Matrix[3, 1] = 0;
            #elif TASK2
            Matrix B2Matrix = new Matrix(3, 1);
            B2Matrix[1, 1] = -2.886;
            B2Matrix[2, 1] = 0;
            B2Matrix[3, 1] = 40.234;
            #endif

            return B2Matrix;
        }

        /// <summary>
        /// ����� GetC1Matrix ���������� ������� (�������) C1 (��. ��������)
        /// </summary>
        /// <returns></returns>
        private Matrix GetC1Matrix()
        {
            // �� ���� ������� C1 ���� �������� �� �����
            #warning Matrix C1 must be from input datafile

            #if TASK1
            Matrix C1Matrix = new Matrix(3, 1);
            C1Matrix[1, 1] = 1;
            C1Matrix[2, 1] = 0;
            C1Matrix[3, 1] = 0;
            #elif TASK2
            Matrix C1Matrix = new Matrix(3, 1);
            C1Matrix[1, 1] = 0.032;
            C1Matrix[2, 1] = 0;
            C1Matrix[3, 1] = -0.27;
            #endif

            return C1Matrix;
        }

        /// <summary>
        /// ����� GetC2Matrix ���������� ������� (�������) C2 (��. ��������)
        /// </summary>
        /// <returns></returns>
        private Matrix GetC2Matrix()
        {
            // �� ���� ������� C2 ���� �������� �� �����
            #warning Matrix C2 must be from input datafile

            #if TASK1
            Matrix C2Matrix = new Matrix(3, 1);
            C2Matrix[1, 1] = 0;
            C2Matrix[2, 1] = 1;
            C2Matrix[3, 1] = 0;
            #elif TASK2
            Matrix C2Matrix = new Matrix(3, 1);
            C2Matrix[1, 1] = 0.135;
            C2Matrix[2, 1] = 0;
            C2Matrix[3, 1] = 0.014;
            #endif

            return C2Matrix;
        }

        /// <summary>
        /// ����� GetD1Matrix ���������� ������� (�������) D1 (��. ��������)
        /// </summary>
        /// <param name="InverseTime">�������� ����� = ����� ��������� - ������� �����</param>
        /// <returns></returns>
        private Matrix GetD1Matrix(Double InverseTime)
        {
            return m_FundKoshiMatrix.FundKoshiMatrixCalc(InverseTime) * GetB1Matrix();
        }

        /// <summary>
        /// ����� GetD2Matrix ���������� ������� (�������) D2 (��. ��������)
        /// </summary>
        /// <param name="InverseTime">�������� ����� = ����� ��������� - ������� �����</param>
        /// <returns></returns>
        private Matrix GetD2Matrix(Double InverseTime)
        {
            return m_FundKoshiMatrix.FundKoshiMatrixCalc(InverseTime) * GetB2Matrix();
        }

        /// <summary>
        /// ����� GetE1Matrix ���������� ������� (�������) E1 (��. ��������)
        /// </summary>
        /// <param name="InverseTime">�������� ����� = ����� ��������� - ������� �����</param>
        /// <returns></returns>
        private Matrix GetE1Matrix(Double InverseTime)
        {
            return m_FundKoshiMatrix.FundKoshiMatrixCalc(InverseTime) * GetC1Matrix();
        }

        /// <summary>
        /// ����� GetE2Matrix ���������� ������� (�������) E2 (��. ��������)
        /// </summary>
        /// <param name="InverseTime">�������� ����� = ����� ��������� - ������� �����</param>
        /// <returns></returns>
        private Matrix GetE2Matrix(Double InverseTime)
        {
            return m_FundKoshiMatrix.FundKoshiMatrixCalc(InverseTime) * GetC2Matrix();
        }*/

        /// <summary>
        /// ����� GetDMatrix ���������� ������� (�������) D (��. ��������)
        /// </summary>
        /// <param name="InverseTime">�������� ����� = ����� ��������� - ������� �����</param>
        /// <returns></returns>
        private Matrix GetDMatrix(Double InverseTime)
        {
            return m_FundKoshiMatrix.FundKoshiMatrixCalc(InverseTime) * GetBMatrix();
        }

        /// <summary>
        /// ����� GetEMatrix ���������� ������� (�������) E (��. ��������)
        /// </summary>
        /// <param name="InverseTime">�������� ����� = ����� ��������� - ������� �����</param>
        /// <returns></returns>
        private Matrix GetEMatrix(Double InverseTime)
        {
            return m_FundKoshiMatrix.FundKoshiMatrixCalc(InverseTime) * GetCMatrix();
        }

        /// <summary>
        /// ����� GetPolyhedronGraphDescription ���������� � ��������� ���� (� ���� ������� �����) �������� ����� �������������
        /// </summary>
        /// <returns></returns>
        private String[] GetPolyhedronGraphDescription()
        {
            String[] PolyhedronGraphDescription = new String[m_PGNodeList.Count];

            for (Int32 PGNodeIndex = 0; PGNodeIndex < m_PGNodeList.Count; PGNodeIndex++)
            {
                StringBuilder CurrentString = new StringBuilder();

                CurrentString.AppendFormat("PGNode number {0} has normal with coords ({1}; {2}; {3})\r\n", m_PGNodeList[PGNodeIndex].ID, m_PGNodeList[PGNodeIndex].NodeNormal.XCoord, m_PGNodeList[PGNodeIndex].NodeNormal.YCoord, m_PGNodeList[PGNodeIndex].NodeNormal.ZCoord);
                CurrentString.AppendFormat("Connected with folowing PGNodes : \r\n");

                for (Int32 NodeConnIndex = 0; NodeConnIndex < m_PGNodeList[PGNodeIndex].NodeConnectionCount; NodeConnIndex++)
                {
                    PolyhedronGraphNode NeighbourPGNode = m_PGNodeList[PGNodeIndex][NodeConnIndex];

                    CurrentString.AppendFormat("\tPGNode number {0}\r\n", NeighbourPGNode.ID);
                }

                CurrentString.Append("\r\n");
                PolyhedronGraphDescription[PGNodeIndex] = CurrentString.ToString();
            }

            return PolyhedronGraphDescription;
        }

        private void SavePGGraphInfo2File(String PGGraphInfo)
        {
            /*String PGraphFileName = "Graph.descr";
            using (StreamWriter sw = new StreamWriter(PGraphFileName, true))
            {
                sw.WriteLine(PGGraphInfo);
            }*/
        }

        private void SavePGraph2File()
        {
            /*String PGraphFileName = "Graph.descr";
            using (StreamWriter sw = new StreamWriter(PGraphFileName, true))
            {
                String[] PGDescr = GetPolyhedronGraphDescription();
                foreach (String PGDescrString in PGDescr)
                {
                    sw.WriteLine(PGDescrString);
                }
            }*/
        }

        /*private Boolean CheckGraphStructure()
        {
            foreach (PolyhedronGraphNode PGNode in m_PGNodeList)
            {
                for (Int32 PGConnIndex = 0; PGConnIndex < PGNode.NodeConnectionCount; PGConnIndex++)
                {
                    PolyhedronGraphNode CurrentPGConn = PGNode[PGConnIndex];
                    PolyhedronGraphNode PrevPGConn = (PGConnIndex == 0 ? PGNode[PGNode.NodeConnectionCount - 1] : PGNode[PGConnIndex - 1]);

                    if ((CurrentPGConn.GetConnectionIndex(PrevPGConn) == -1) || (PrevPGConn.GetConnectionIndex(CurrentPGConn) == -1))
                    {
                        return false;
                    }
                }
            }

            return true;
        }*/

        /*private Boolean CheckGraphStructure2()
        {
            SortedList<Double, PolyhedronGraphNode> OXYPlanePGNodes = new SortedList<double, PolyhedronGraphNode>();

            Vector3D iVector = new Vector3D(1,0,0);

            foreach (PolyhedronGraphNode PGNode in m_PGNodeList)
            {
                Double ZCoord = PGNode.NodeNormal.ZCoord;
                if ((ZCoord >= -Epsilon) && (ZCoord <= Epsilon))
                {
                    // �������� ���� !!! 
                    Double PolarAngle = (PGNode.NodeNormal.YCoord > 0 ? Vector3D.AngleBetweenVectors(PGNode.NodeNormal, iVector) : 2 * Math.PI - Vector3D.AngleBetweenVectors(PGNode.NodeNormal, iVector));
                    OXYPlanePGNodes.Add(PolarAngle, PGNode);
                }
            }
        }*/

        /*public AlgorithmClass()
        {
            Point3D[] FinalSetVertexList = GetFinalSet();

            m_PolyhedronVertexList = new List<VertexClass>();
            m_PolyhedronSideList = new List<SideClass>();

            for (Int32 VertexIndex = 0; VertexIndex < FinalSetVertexList.Length; VertexIndex++)
            {
                m_PolyhedronVertexList.Add(new VertexClass(FinalSetVertexList[VertexIndex], VertexIndex + 1));
            }

            m_PGNodeList = new List<PolyhedronGraphNode>();

            InitAlgorithmWork();

            m_CurrentInverseTime = 0;

            m_Pi1Set = new List<Point3D>();
            m_Pi2Set = new List<Point3D>();
            m_Qi1Set = new List<Point3D>();
            m_Qi2Set = new List<Point3D>();

            m_FundKoshiMatrix = new FundKoshiMatrix(GetAMatrix(), new Int32[] { 1, 2, 3 });
            m_FundKoshiMatrix.FundKoshiMatrixCalc(0);
        }*/

        /*public AlgorithmClass(InputDataReader reader)
        {
            m_MatrixA = reader.MatrixA;
            m_MatrixB = reader.MatrixB;
            m_MatrixC = reader.MatrixC;

            m_Mp1 = reader.Mp;
            m_Mq1 = reader.Mq;

            m_DeltaT = reader.DeltaT;

            Epsilon = reader.Epsilon;
            MinVectorDistinguishAngle = reader.MinVectorDistinguishAngle;

            Point3D[] FinalSetVertexList = reader.FinalSet;

            m_PolyhedronVertexList = new List<VertexClass>();
            m_PolyhedronSideList = new List<SideClass>();

            for (Int32 VertexIndex = 0; VertexIndex < FinalSetVertexList.Length; VertexIndex++)
            {
                m_PolyhedronVertexList.Add(new VertexClass(FinalSetVertexList[VertexIndex], VertexIndex + 1));
            }

            m_PGNodeList = new List<PolyhedronGraphNode>();

            InitAlgorithmWork();

            m_CurrentInverseTime = 0;

            m_Pi1Set = new List<Point3D>();
            m_Pi2Set = new List<Point3D>();
            m_Qi1Set = new List<Point3D>();
            m_Qi2Set = new List<Point3D>();

            m_FundKoshiMatrix = new FundKoshiMatrix(GetAMatrix(), new Int32[] { 1, 2, 3 });
            m_FundKoshiMatrix.FundKoshiMatrixCalc(0);
        }*/

        public AlgorithmClass(Dictionary<String, Object> inputData)
        {
            m_MatrixA = inputData["MatrixA"] as Matrix;
            m_MatrixB = inputData["MatrixB"] as Matrix;
            m_MatrixC = inputData["MatrixC"] as Matrix;

            m_Mp1 = (Double)inputData["Mp"];
            m_Mq1 = (Double)inputData["Mq"];

            m_DeltaT = (Double)inputData["DeltaT"];

            Epsilon = (Double)inputData["Epsilon"];
            MinVectorDistinguishAngle = (Double)inputData["MinVectorDistinguishAngle"];

            Point3D[] FinalSetVertexList = inputData["FinalSet"] as Point3D[];

            m_PolyhedronVertexList = new List<VertexClass>();
            m_PolyhedronSideList = new List<SideClass>();

            for (Int32 VertexIndex = 0; VertexIndex < FinalSetVertexList.Length; VertexIndex++)
            {
                m_PolyhedronVertexList.Add(new VertexClass(FinalSetVertexList[VertexIndex], VertexIndex + 1));
            }

            m_PGNodeList = new List<PolyhedronGraphNode>();

            InitAlgorithmWork();

            m_CurrentInverseTime = 0;

            m_Pi1Set = new List<Point3D>();
            m_Pi2Set = new List<Point3D>();
            m_Qi1Set = new List<Point3D>();
            m_Qi2Set = new List<Point3D>();

            m_FundKoshiMatrix = new FundKoshiMatrix(GetAMatrix(), new Int32[] { 1, 2, 3 });
            m_FundKoshiMatrix.FundKoshiMatrixCalc(0);
        }

        /*public AlgorithmClass(Matrix matrixA, Matrix matrixB, Matrix matrixC, Double mp, Double mq, Double deltaT, Double epsilon, Double minVectorDistinguishAngle, Point3D[] finalSet)
        {
            m_MatrixA = matrixA;
            m_MatrixB = matrixB;
            m_MatrixC = matrixC;

            m_Mp1 = mp;
            m_Mq1 = mq;

            m_DeltaT = deltaT;

            Epsilon = epsilon;
            MinVectorDistinguishAngle = minVectorDistinguishAngle;

            Point3D[] FinalSetVertexList = finalSet;

            m_PolyhedronVertexList = new List<VertexClass>();
            m_PolyhedronSideList = new List<SideClass>();

            for (Int32 VertexIndex = 0; VertexIndex < FinalSetVertexList.Length; VertexIndex++)
            {
                m_PolyhedronVertexList.Add(new VertexClass(FinalSetVertexList[VertexIndex], VertexIndex + 1));
            }

            m_PGNodeList = new List<PolyhedronGraphNode>();

            InitAlgorithmWork();

            m_CurrentInverseTime = 0;

            m_Pi1Set = new List<Point3D>();
            m_Pi2Set = new List<Point3D>();
            m_Qi1Set = new List<Point3D>();
            m_Qi2Set = new List<Point3D>();

            m_FundKoshiMatrix = new FundKoshiMatrix(GetAMatrix(), new Int32[] { 1, 2, 3 });
            m_FundKoshiMatrix.FundKoshiMatrixCalc(0);
        }*/

        /// <summary>
        /// 
        /// </summary>
        public void NextSolutionIteration()
        {
            m_CurrentInverseTime += m_DeltaT;

            /*Matrix D1Matrix = GetD1Matrix(m_CurrentInverseTime);
            Matrix D2Matrix = GetD2Matrix(m_CurrentInverseTime);
            Matrix E1Matrix = GetE1Matrix(m_CurrentInverseTime);
            Matrix E2Matrix = GetE2Matrix(m_CurrentInverseTime);*/
            Matrix matrixD = GetDMatrix(m_CurrentInverseTime);
            Matrix matrixE = GetEMatrix(m_CurrentInverseTime);

            m_Pi1Set.Clear();
            m_Pi1Set.Add(new Point3D(-m_Mp1 * m_DeltaT * matrixD[1, 1], -m_Mp1 * m_DeltaT * matrixD[2, 1], -m_Mp1 * m_DeltaT * matrixD[3, 1]));
            m_Pi1Set.Add(new Point3D(m_Mp1 * m_DeltaT * matrixD[1, 1], m_Mp1 * m_DeltaT * matrixD[2, 1], m_Mp1 * m_DeltaT * matrixD[3, 1]));
            /*m_Pi2Set.Clear();
            m_Pi2Set.Add(new Point3D(-m_Mp2 * m_DeltaT * D2Matrix[1, 1], -m_Mp2 * m_DeltaT * D2Matrix[2, 1], -m_Mp2 * m_DeltaT * D2Matrix[3, 1]));
            m_Pi2Set.Add(new Point3D(m_Mp2 * m_DeltaT * D2Matrix[1, 1], m_Mp2 * m_DeltaT * D2Matrix[2, 1], m_Mp2 * m_DeltaT * D2Matrix[3, 1]));
            */
            m_Qi1Set.Clear();
            m_Qi1Set.Add(new Point3D(-m_Mq1 * m_DeltaT * matrixE[1, 1], -m_Mq1 * m_DeltaT * matrixE[2, 1], -m_Mq1 * m_DeltaT * matrixE[3, 1]));
            m_Qi1Set.Add(new Point3D(m_Mq1 * m_DeltaT * matrixE[1, 1], m_Mq1 * m_DeltaT * matrixE[2, 1], m_Mq1 * m_DeltaT * matrixE[3, 1]));
            /*m_Qi2Set.Clear();
            m_Qi2Set.Add(new Point3D(-m_Mq2 * m_DeltaT * E2Matrix[1, 1], -m_Mq2 * m_DeltaT * E2Matrix[2, 1], -m_Mq2 * m_DeltaT * E2Matrix[3, 1]));
            m_Qi2Set.Add(new Point3D(m_Mq2 * m_DeltaT * E2Matrix[1, 1], m_Mq2 * m_DeltaT * E2Matrix[2, 1], m_Mq2 * m_DeltaT * E2Matrix[3, 1]));
            */

            m_Pi1DirectingVector = new Vector3D(m_Pi1Set[1].XCoord - m_Pi1Set[0].XCoord,
                                                m_Pi1Set[1].YCoord - m_Pi1Set[0].YCoord,
                                                m_Pi1Set[1].ZCoord - m_Pi1Set[0].ZCoord);
            m_Pi1DirectingVector.Normalize();
            /*m_Pi2DirectingVector = new Vector3D(m_Pi2Set[1].XCoord - m_Pi2Set[0].XCoord,
                                                m_Pi2Set[1].YCoord - m_Pi2Set[0].YCoord,
                                                m_Pi2Set[1].ZCoord - m_Pi2Set[0].ZCoord);
            m_Pi2DirectingVector.Normalize();
            */
            m_Qi1DirectingVector = new Vector3D(m_Qi1Set[1].XCoord - m_Qi1Set[0].XCoord,
                                                m_Qi1Set[1].YCoord - m_Qi1Set[0].YCoord,
                                                m_Qi1Set[1].ZCoord - m_Qi1Set[0].ZCoord);
            m_Qi1DirectingVector.Normalize();
            /*m_Qi2DirectingVector = new Vector3D(m_Qi2Set[1].XCoord - m_Qi2Set[0].XCoord,
                                                m_Qi2Set[1].YCoord - m_Qi2Set[0].YCoord,
                                                m_Qi2Set[1].ZCoord - m_Qi2Set[0].ZCoord);
            m_Qi2DirectingVector.Normalize();
            */

            //SavePGGraphInfo2File("CurrentInverseTime = "+m_CurrentInverseTime.ToString());

            // m_PGNodeList.Count - ���-�� ����� => m_PGNodeList.Count-1 - ������ ���������� ����
            Int32 OldNodesLastIndex = m_PGNodeList.Count - 1;

            BuildGFiGrid(m_Pi1DirectingVector);
            //SavePGGraphInfo2File("after first gamer part 1");
            //SavePGraph2File();
            //BuildGFiGrid(m_Pi2DirectingVector);
            //SavePGGraphInfo2File("after first gamer part 2");
            //SavePGraph2File();

            /*List<PolyhedronGraphNode> Nodes4Removing = new List<PolyhedronGraphNode>();
            for (Int32 NewNodeIndex = OldNodesLastIndex + 1; NewNodeIndex < m_PGNodeList.Count; NewNodeIndex++)
            {
                PolyhedronGraphNode CurrentNewPGNode = m_PGNodeList[NewNodeIndex];

                for (Int32 PGConnIndex = 0; PGConnIndex < CurrentNewPGNode.NodeConnectionCount; PGConnIndex++)
                {
                    PolyhedronGraphNode CurrentPGConnNode = CurrentNewPGNode[PGConnIndex];

                    Double AngleBetweenVectors = Vector3D.AngleBetweenVectors(CurrentNewPGNode.NodeNormal, CurrentPGConnNode.NodeNormal);
                    // ???
                    //if ((CurrentPGConnNode.ID <= OldNodesLastIndex) && (AngleBetweenVectors < MinVectorDistinguishAngle))
                    // ???
                    if (AngleBetweenVectors < MinVectorDistinguishAngle)
                    {
                        Nodes4Removing.Add(CurrentNewPGNode);
                        break;
                    }
                }
            }

            foreach (PolyhedronGraphNode PGNode in Nodes4Removing)
            {
                RemoveNode(PGNode);
            }*/

            /*Int32 NewNodeIndex = OldNodesLastIndex + 1;
            while (NewNodeIndex < m_PGNodeList.Count)
            {
                PolyhedronGraphNode CurrentNewPGNode = m_PGNodeList[NewNodeIndex];

                for (Int32 PGConnIndex = 0; PGConnIndex < CurrentNewPGNode.NodeConnectionCount; PGConnIndex++)
                {
                    PolyhedronGraphNode CurrentPGConnNode = CurrentNewPGNode[PGConnIndex];

                    Double AngleBetweenVectors = Vector3D.AngleBetweenVectors(CurrentNewPGNode.NodeNormal, CurrentPGConnNode.NodeNormal);
                    if ((CurrentPGConnNode.ID <= OldNodesLastIndex) && (AngleBetweenVectors < MinVectorDistinguishAngle))
                    {
                        NewNodeIndex--;
                        RemoveNode(CurrentNewPGNode);
                        break;
                    }
                }

                NewNodeIndex++;
            }*/

            /*for (Int32 PGNodeIndex1 = 0; PGNodeIndex1 < m_PGNodeList.Count; PGNodeIndex1++)
            {
                Int32 PGNodeIndex2 = PGNodeIndex1 + 1;
                while (PGNodeIndex2 < m_PGNodeList.Count)
                {
                    Vector3D Vector1 = m_PGNodeList[PGNodeIndex1].NodeNormal;
                    Vector3D Vector2 = m_PGNodeList[PGNodeIndex2].NodeNormal;

                    if (Vector3D.AngleBetweenVectors(Vector1, Vector2) < MinVectorDistinguishAngle)
                    {
                        RemoveNode(m_PGNodeList[PGNodeIndex2]);
                    }
                    else
                    {
                        PGNodeIndex2++;
                    }
                }
            }*/

            /*for (Int32 PGNodeIndex1 = OldNodesLastIndex + 1; PGNodeIndex1 < m_PGNodeList.Count; PGNodeIndex1++)
            {
                Int32 PGNodeIndex2 = PGNodeIndex1 + 1;
                while (PGNodeIndex2 < m_PGNodeList.Count)
                {

                    Vector3D Vector1 = m_PGNodeList[PGNodeIndex1].NodeNormal;
                    Vector3D Vector2 = m_PGNodeList[PGNodeIndex2].NodeNormal;

                    // ������� ���� 2, ���� �� ������ � ����� 1 � ���� ����� ���� < MinVectorDistinguishAngle
                    if ((m_PGNodeList[PGNodeIndex1].HasConnectionWithNode(m_PGNodeList[PGNodeIndex2])) &&
                        (Vector3D.AngleBetweenVectors(Vector1, Vector2) < MinVectorDistinguishAngle))
                    {
                        RemoveNode(m_PGNodeList[PGNodeIndex2]);
                    }
                    else
                    {
                        PGNodeIndex2++;
                    }
                }
            }*/

            //SavePGGraphInfo2File("after removing near nodes");
            //SavePGraph2File();

            /*SavePGGraphInfo2File("after first gamer");
            SavePGraph2File();*/
            SolutionIteration(m_Qi1DirectingVector);
            //SolutionIteration(m_Qi2DirectingVector);
            /*if (!CheckGraphStructure())
            {
                throw new AlgorithmException("Wrong structure of polyhedron graph");
            }*/

            RecievePolyhedronStructureFromGraph();
            //SavePGGraphInfo2File("after second gamer");
            //SavePGraph2File();
        }

        /*public void SolveDiff2DGame(Double InverseSolveTime)
        {
            while ()
            {
            }
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Point3D[] GetFinalSet()
        {
            #if TASK1
            Double MaxCValue = 2.5;
            Int32 VertexCount = 5;

            Point3D[] FinalSet = new Point3D[VertexCount];
            FinalSet[0] = new Point3D(0, 0, 0);
            FinalSet[1] = new Point3D(MaxCValue, MaxCValue, MaxCValue);
            FinalSet[2] = new Point3D(-MaxCValue, MaxCValue, MaxCValue);
            FinalSet[3] = new Point3D(-MaxCValue, -MaxCValue, MaxCValue);
            FinalSet[4] = new Point3D(MaxCValue, -MaxCValue, MaxCValue);

            #elif TASK2
            Double XMax = 10;
            Double YMax = 2;
            Double ZMax = 0.5;
            /*Double XMax = 2;
            Double YMax = 1;
            Double ZMax = 1;*/

            Point3D[] FinalSet = new Point3D[8];
            FinalSet[0] = new Point3D(XMax, YMax, ZMax);
            FinalSet[1] = new Point3D(-XMax, YMax, ZMax);
            FinalSet[2] = new Point3D(-XMax, -YMax, ZMax);
            FinalSet[3] = new Point3D(XMax, -YMax, ZMax);
            FinalSet[4] = new Point3D(XMax, YMax, -ZMax);
            FinalSet[5] = new Point3D(-XMax, YMax, -ZMax);
            FinalSet[6] = new Point3D(-XMax, -YMax, -ZMax);
            FinalSet[7] = new Point3D(XMax, -YMax, -ZMax);
            #endif

            Double alpha = 1;

            Point3D[] finalSet = new Point3D[8];
            finalSet[0] = new Point3D(alpha, alpha, alpha);
            finalSet[1] = new Point3D(-alpha, alpha, alpha);
            finalSet[2] = new Point3D(alpha, -alpha, alpha);
            finalSet[3] = new Point3D(-alpha, -alpha, alpha);
            finalSet[4] = new Point3D(alpha, alpha, -alpha);
            finalSet[5] = new Point3D(-alpha, alpha, -alpha);
            finalSet[6] = new Point3D(alpha, -alpha, -alpha);
            finalSet[7] = new Point3D(-alpha, -alpha, -alpha);
            /*Double maxCValue = 2.5;
            Int32 vertexCount = 5;

            Point3D[] finalSet = new Point3D[vertexCount];
            finalSet[0] = new Point3D(0, 0, 0);
            finalSet[1] = new Point3D(maxCValue, maxCValue, maxCValue);
            finalSet[2] = new Point3D(-maxCValue, maxCValue, maxCValue);
            finalSet[3] = new Point3D(-maxCValue, -maxCValue, maxCValue);
            finalSet[4] = new Point3D(maxCValue, -maxCValue, maxCValue);*/

            return finalSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<SideClass> GetSideList()
        {
            return m_PolyhedronSideList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<VertexClass> GetVertexList()
        {
            return m_PolyhedronVertexList;
        }

        /// <summary>
        /// 
        /// </summary>
        public Double InverseTime
        {
            get
            {
                return m_CurrentInverseTime;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FundKoshiMatrix GetFundKoshiMatrix()
        {
            return m_FundKoshiMatrix;
        }

        /// <summary>
        /// ����� GetAMatrix ���������� ������� A (��. ��������)
        /// </summary>
        /// <returns></returns>
        public Matrix GetAMatrix()
        {
            // �� ���� ������� A ���� �������� �� �����
            /*Matrix matrixA = new Matrix(3, 3);

            matrixA[1, 1] = 0;
            matrixA[1, 2] = 1;
            matrixA[1, 3] = 0;
            matrixA[2, 1] = 0;
            matrixA[2, 2] = 0;
            matrixA[2, 3] = 1;
            matrixA[3, 1] = 0;
            matrixA[3, 2] = 0;
            matrixA[3, 3] = 0;*/
            /*Double k = 0;
            matrixA[1, 1] = 0;
            matrixA[1, 2] = 1;
            matrixA[1, 3] = 0;
            matrixA[2, 1] = k;
            matrixA[2, 2] = 0;
            matrixA[2, 3] = 0;
            matrixA[3, 1] = 0;
            matrixA[3, 2] = 0;
            matrixA[3, 3] = 0;*/

            return m_MatrixA;
        }

        /// <summary>
        /// ����� GetBMatrix ���������� ������� (�������) B (��. ��������)
        /// </summary>
        /// <returns></returns>
        public Matrix GetBMatrix()
        {
            // �� ���� ������� B ���� �������� �� �����
            /*Matrix matrixB = new Matrix(3, 1);

            matrixB[1, 1] = 0;
            matrixB[2, 1] = 0;
            matrixB[3, 1] = 1;*/
            /*matrixB[1, 1] = 0;
            matrixB[2, 1] = 1;
            matrixB[3, 1] = 0;*/

            return m_MatrixB;
        }

        /// <summary>
        /// ����� GetCMatrix ���������� ������� (�������) C (��. ��������)
        /// </summary>
        /// <returns></returns>
        public Matrix GetCMatrix()
        {
            // �� ���� ������� C ���� �������� �� �����
            /*Matrix matrixC = new Matrix(3, 1);

            matrixC[1, 1] = 1;
            matrixC[2, 1] = 0;
            matrixC[3, 1] = 0;*/
            /*matrixC[1, 1] = 1;
            matrixC[2, 1] = 0;
            matrixC[3, 1] = 0;*/

            return m_MatrixC;
        }

        public Double Mp
        {
            get
            {
                return m_Mp1;
            }
        }

        public Double Mq
        {
            get
            {
                return m_Mq1;
            }
        }

        public Double DeltaT
        {
            get
            {
                return m_DeltaT;
            }
        }
    }
}
