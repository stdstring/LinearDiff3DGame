using System;
using System.Collections.Generic;
using System.Text;

namespace PolyhedronStructureViewer
{
    /// <summary>
    /// 
    /// </summary>
    public class AlgorithmException : ApplicationException
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public struct Point3D
    {
        /// <summary>
        /// m_XCoord - ���������� X �����
        /// </summary>
        private Double m_XCoord;
        /// <summary>
        /// m_YCoord - ���������� Y �����
        /// </summary>
        private Double m_YCoord;
        /// <summary>
        /// m_ZCoord - ���������� Z �����
        /// </summary>
        private Double m_ZCoord;

        public Point3D(Double XCoord, Double YCoord, Double ZCoord)
        {
            this.m_XCoord = XCoord;
            this.m_YCoord = YCoord;
            this.m_ZCoord = ZCoord;
        }

        /// <summary>
        /// XCoord - �������� ��� ������� � ���������� X �����
        /// </summary>
        public Double XCoord
        {
            get
            {
                return m_XCoord;
            }
            set
            {
                m_XCoord = value;
            }
        }
        /// <summary>
        /// YCoord - �������� ��� ������� � ���������� Y �����
        /// </summary>
        public Double YCoord
        {
            get
            {
                return m_YCoord;
            }
            set
            {
                m_YCoord = value;
            }
        }
        /// <summary>
        /// ZCoord - �������� ��� ������� � ���������� Z �����
        /// </summary>
        public Double ZCoord
        {
            get
            {
                return m_ZCoord;
            }
            set
            {
                m_ZCoord = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal struct Vector3D
    {
        /// <summary>
        /// m_XCoord - ���������� X �������
        /// </summary>
        private Double m_XCoord;
        /// <summary>
        /// m_YCoord - ���������� Y �������
        /// </summary>
        private Double m_YCoord;
        /// <summary>
        /// m_ZCoord - ���������� Z �������
        /// </summary>
        private Double m_ZCoord;
        /// <summary>
        /// MinAngleBetweenVectors - ����������� �������� ���� ����� ���������, ��� ������� ������� ��������� ������� 
        /// </summary>
        public const Double MinAngleBetweenVectors = 1e-9;
        /// <summary>
        /// AcosDigits - ...
        /// </summary>
        private const Int32 AcosDigits = 9;

        public Vector3D(Double XCoord, Double YCoord, Double ZCoord)
        {
            this.m_XCoord = XCoord;
            this.m_YCoord = YCoord;
            this.m_ZCoord = ZCoord;
        }

        /// <summary>
        /// ����� Normalize ��������� ������� ������
        /// </summary>
        public void Normalize()
        {
            Double VectorLength = Math.Sqrt(m_XCoord * m_XCoord + m_YCoord * m_YCoord + m_ZCoord * m_ZCoord);

            this.m_XCoord /= VectorLength;
            this.m_YCoord /= VectorLength;
            this.m_ZCoord /= VectorLength;
        }

        /// <summary>
        /// ����� ApproxEquals ���������� true, ���� ������� � OtherVector ������� �������������� �����,
        /// �.�. ���� ���� ����� ������� � OtherVector ��������� ������, ��� �������� MinAngleBetweenVectors
        /// </summary>
        /// <param name="OtherVector"></param>
        /// <returns></returns>
        public Boolean ApproxEquals(Vector3D OtherVector)
        {
            Double AcosValue = Math.Round(Vector3D.ScalarProduct(this, OtherVector) / (this.Length * OtherVector.Length), AcosDigits);
            Double AngleBetweenVectors = Math.Acos(AcosValue);

            // ���������� ��� ���� ...  ������ ����� ��� ����������
            // return (Math.Abs(AngleBetweenVectors) < MinAngleBetweenVectors);
            return (AngleBetweenVectors == 0);
        }

        /// <summary>
        /// ����� GetParallelComponent ���������� ���������� �������� �������, ������������ ������� DirectingVector
        /// </summary>
        /// <param name="DirectingVector"></param>
        /// <returns></returns>
        public Vector3D GetParallelComponent(Vector3D DirectingVector)
        {
            if (DirectingVector.Length != 1) DirectingVector.Normalize();

            Double ScalarProductValue = Vector3D.ScalarProduct(this, DirectingVector);

            Double ParallelCompX = ScalarProductValue * DirectingVector.XCoord;
            Double ParallelCompY = ScalarProductValue * DirectingVector.YCoord;
            Double ParallelCompZ = ScalarProductValue * DirectingVector.ZCoord;

            return new Vector3D(ParallelCompX, ParallelCompY, ParallelCompZ);
        }

        /// <summary>
        /// ����� GetPerpendicularComponent ���������� ���������� �������� �������, ���������������� ������� DirectingVector
        /// </summary>
        /// <param name="DirectingVector"></param>
        /// <returns></returns>
        public Vector3D GetPerpendicularComponent(Vector3D DirectingVector)
        {
            if (DirectingVector.Length != 1) DirectingVector.Normalize();

            Double ScalarProductValue = Vector3D.ScalarProduct(this, DirectingVector);

            Double PerpendicularCompX = this.XCoord - ScalarProductValue * DirectingVector.XCoord;
            Double PerpendicularCompY = this.YCoord - ScalarProductValue * DirectingVector.YCoord;
            Double PerpendicularCompZ = this.ZCoord - ScalarProductValue * DirectingVector.ZCoord;

            return new Vector3D(PerpendicularCompX, PerpendicularCompY, PerpendicularCompZ);
        }

        /// <summary>
        /// XCoord - �������� ��� ������� � ���������� X �������
        /// </summary>
        public Double XCoord
        {
            get
            {
                return m_XCoord;
            }
            set
            {
                m_XCoord = value;
            }
        }
        /// <summary>
        /// YCoord - �������� ��� ������� � ���������� Y �������
        /// </summary>
        public Double YCoord
        {
            get
            {
                return m_YCoord;
            }
            set
            {
                m_YCoord = value;
            }
        }
        /// <summary>
        /// ZCoord - �������� ��� ������� � ���������� Z �������
        /// </summary>
        public Double ZCoord
        {
            get
            {
                return m_ZCoord;
            }
            set
            {
                m_ZCoord = value;
            }
        }

        /// <summary>
        /// Length - ����� �������
        /// </summary>
        public Double Length
        {
            get
            {
                return Math.Sqrt(m_XCoord * m_XCoord + m_YCoord * m_YCoord + m_ZCoord * m_ZCoord);
            }
        }

        /// <summary>
        /// ����� ScalarProduct ���������� ��������� ���������� ������������ �������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>��������� ���������� ������������ �������� a � b</returns>
        public static Double ScalarProduct(Vector3D a, Vector3D b)
        {
            return a.XCoord * b.XCoord + a.YCoord * b.YCoord + a.ZCoord * b.ZCoord;
        }

        /// <summary>
        /// ����� VectorProduct ���������� ��������� (������) ���������� ������������ �������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>��������� (������) ���������� ������������ �������� a � b</returns>
        public static Vector3D VectorProduct(Vector3D a, Vector3D b)
        {
            Double XCoord = a.YCoord * b.ZCoord - a.ZCoord * b.YCoord;
            Double YCoord = a.ZCoord * b.XCoord - a.XCoord * b.ZCoord;
            Double ZCoord = a.XCoord * b.YCoord - a.YCoord * b.XCoord;

            return new Vector3D(XCoord, YCoord, ZCoord);
        }

        /// <summary>
        /// ����� MixedProduct ���������� ��������� ���������� ������������ �������� a, b � c
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <param name="c">������ c</param>
        /// <returns>��������� ���������� ������������ �������� a, b � c</returns>
        public static Double MixedProduct(Vector3D a, Vector3D b, Vector3D c)
        {
            return Vector3D.ScalarProduct(a, Vector3D.VectorProduct(b, c));
        }

        /// <summary>
        /// �������� ZeroVector3D - ������� ������
        /// </summary>
        public static Vector3D ZeroVector3D
        {
            get
            {
                return new Vector3D(0, 0, 0);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class VertexClass
    {
        /// <summary>
        /// m_SideList - ������ ������, ������� ����������� ������ �������
        /// </summary>
        private List<SideClass> m_SideList;

        /// <summary>
        /// m_XCoord - ���������� X �������
        /// </summary>
        private Double m_XCoord;
        /// <summary>
        /// m_YCoord - ���������� Y �������
        /// </summary>
        private Double m_YCoord;
        /// <summary>
        /// m_ZCoord - ���������� Z �������
        /// </summary>
        private Double m_ZCoord;
        /// <summary>
        /// Dimension - ����������� ������� (� ����� ������ 3)
        /// </summary>
        public readonly Int32 Dimension = 3;
        /// <summary>
        /// ID - ���������� ������������� �������
        /// </summary>
        public readonly Int32 ID;

        public VertexClass(Double XCoord, Double YCoord, Double ZCoord, Int32 VertexID)
        {
            this.ID = VertexID;

            this.m_SideList = new List<SideClass>();
            this.m_XCoord = XCoord;
            this.m_YCoord = YCoord;
            this.m_ZCoord = ZCoord;
        }

        public VertexClass(Point3D Vertex, Int32 VertexID) : this(Vertex.XCoord, Vertex.YCoord, Vertex.ZCoord, VertexID)
        {
        }

        /// <summary>
        /// ����� AddSide ��������� ����� Side � ������ ������, ������� ����������� ������ �������
        /// </summary>
        /// <param name="Side">����������� �����</param>
        public void AddSide(SideClass Side)
        {
            m_SideList.Add(Side);
        }

        /// <summary>
        /// ����� RemoveSide ������� ����� Side �� ������ ������, ������� ����������� ������ �������
        /// </summary>
        /// <param name="Side">��������� �����</param>
        public void RemoveSide(SideClass Side)
        {
            m_SideList.Remove(Side);
        }

        /// <summary>
        /// ��������-���������� ��� ������� (������) � ����� �� ������ ������, ������� ����������� ������ �������
        /// </summary>
        /// <param name="SideIndex">������ ����� � ������ ������</param>
        /// <returns></returns>
        public SideClass this[Int32 SideIndex]
        {
            get
            {
                return m_SideList[SideIndex];
            }
            /*set
            {
            }*/
        }

        /// <summary>
        /// �������� SideCount ���������� ���������� ������ � ������ ������, ������� ����������� ������ �������
        /// </summary>
        public Int32 SideCount
        {
            get
            {
                return m_SideList.Count;
            }
        }

        /// <summary>
        /// XCoord - �������� ��� ������� � ���������� X �������
        /// </summary>
        public Double XCoord
        {
            get
            {
                return m_XCoord;
            }
            set
            {
                m_XCoord = value;
            }
        }
        /// <summary>
        /// YCoord - �������� ��� ������� � ���������� Y �������
        /// </summary>
        public Double YCoord
        {
            get
            {
                return m_YCoord;
            }
            set
            {
                m_YCoord = value;
            }
        }
        /// <summary>
        /// ZCoord - �������� ��� ������� � ���������� Z �������
        /// </summary>
        public Double ZCoord
        {
            get
            {
                return m_ZCoord;
            }
            set
            {
                m_ZCoord = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class SideClass
    {
        /// <summary>
        /// m_VertexList - ������ ������, ������� ����������� ������ �����
        /// </summary>
        private List<VertexClass> m_VertexList;
        /// <summary>
        /// m_SideNormal - "�������" ������� � ����� (����������� �����)
        /// </summary>
        private Vector3D m_SideNormal;
        /// <summary>
        /// ID - ���������� ������������� �������
        /// </summary>
        public readonly Int32 ID;

        public SideClass(List<VertexClass> VertexList, Int32 SideID, Vector3D SideNormal)
        {
            this.ID = SideID;

            this.m_VertexList = new List<VertexClass>();
            foreach (VertexClass Vertex in VertexList)
            {
                AddVertex(Vertex);
            }

            this.m_SideNormal = SideNormal;
        }

        /// <summary>
        /// ����� AddVertex ��������� ������� Vertex � ������ ������, ������� ����������� ������ �����
        /// </summary>
        /// <param name="Vertex">����������� �������</param>
        public void AddVertex(VertexClass Vertex)
        {
            m_VertexList.Add(Vertex);
            Vertex.AddSide(this);
        }

        /// <summary>
        /// ����� RemoveVertex ������� ������� Vertex �� ������ ������, ������� ����������� ������ �����
        /// </summary>
        /// <param name="Vertex">��������� �������</param>
        public void RemoveVertex(VertexClass Vertex)
        {
            m_VertexList.Remove(Vertex);
            Vertex.RemoveSide(this);
        }

        /// <summary>
        /// ����� HasVertex ���������� true, ���� ������� Vertex ����������� ������ ���������
        /// </summary>
        /// <param name="Vertex"></param>
        /// <returns></returns>
        public Boolean HasVertex(VertexClass Vertex)
        {
            return (m_VertexList.IndexOf(Vertex) != -1);
        }

        /// <summary>
        /// ����� GetNeighbourSide ���������� ��� ������ ����� ������ �� �����, ������������� ��������� EdgeVertex1 � EdgeVertex2
        /// </summary>
        /// <param name="EdgeVertex1">������ ������� �����</param>
        /// <param name="EdgeVertex2">������ ������� �����</param>
        /// <returns>����� �� ����� ��� ������ �����</returns>
        public SideClass GetNeighbourSide(VertexClass EdgeVertex1, VertexClass EdgeVertex2)
        {
            SideClass NeighbourSide = null;

            // �������� �� ��, ��� EdgeVertex1 � EdgeVertex2 ����������� ������� �����
            if ((m_VertexList.IndexOf(EdgeVertex1) == -1) || (m_VertexList.IndexOf(EdgeVertex2) == -1))
            {
                throw new ArgumentException("EdgeVertex1 and EdgeVertex2 must belong this (current) side");
            }
            // ����� ��������, ��� EdgeVertex1 � EdgeVertex2 �� ����� ���� ����� !!!!!!! (�� ���)
            /*...*/

            for (Int32 SideIndex = 0; SideIndex < EdgeVertex1.SideCount; SideIndex++)
            {
                if (EdgeVertex1[SideIndex].m_VertexList.IndexOf(EdgeVertex2) == -1)
                {
                    continue;
                }
                else if (Object.ReferenceEquals(EdgeVertex1[SideIndex], this))
                {
                    continue;
                }
                else
                {
                    NeighbourSide = EdgeVertex1[SideIndex];
                    break;
                }
            }

            return NeighbourSide;
        }

        /// <summary>
        /// ��������-���������� ��� ������� (������) � ������� �� ������ ������, ������� ����������� ������ �����
        /// </summary>
        /// <param name="VertexIndex">������ ������� � ������ ������</param>
        /// <returns></returns>
        public VertexClass this[Int32 VertexIndex]
        {
            get
            {
                return m_VertexList[VertexIndex];
            }
            /*set
            {
            }*/
        }

        /// <summary>
        /// �������� VertexCount ���������� ���������� ������ � ������ ������, ������� ����������� ������ �����
        /// </summary>
        public Int32 VertexCount
        {
            get
            {
                return m_VertexList.Count;
            }
        }

        /// <summary>
        /// SideNormal - �������� ��� ������� (������) � "�������" ������� �����
        /// </summary>
        public Vector3D SideNormal
        {
            get
            {
                return m_SideNormal;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class PolyhedronGraphNode
    {
        /// <summary>
        /// m_NodeConnectionList - ������ ������ ������� ����
        /// </summary>
        private List<PolyhedronGraphNode> m_NodeConnectionList;
        /// <summary>
        /// m_NodeNormal - "�������" ������� � �����, ������� ������������� ������� ���� �����
        /// </summary>
        private Vector3D m_NodeNormal;
        /// <summary>
        /// ID - ���������� ������������� ���� (��������� � ID �����, ������� ������� ���� �������������)
        /// </summary>
        public readonly Int32 ID;

        public PolyhedronGraphNode(Int32 ID, Vector3D NodeNormal)
        {
            m_NodeConnectionList = new List<PolyhedronGraphNode>();

            this.ID = ID;
            this.m_NodeNormal = NodeNormal;
        }

        /// <summary>
        /// ����� AddNodeConnection ������� ����� (�������� ������ �� ���� OtherGraphNode � ������ ������) ����� ������ ����� � OtherGraphNode
        /// ��� ���� ������ �� ������ ���� � ������ ������ ���� OtherGraphNode �� ���������� !!!!!!!!
        /// </summary>
        /// <param name="OtherGraphNode"></param>
        public void AddNodeConnection(PolyhedronGraphNode OtherGraphNode)
        {
            m_NodeConnectionList.Add(OtherGraphNode);
        }

        /// <summary>
        /// ����� InsertNodeConnection ������� ����� (�������� ������ �� ���� OtherGraphNode � ������ ������ �� ������� InsertPosition) ����� ������ ����� � OtherGraphNode
        /// ��� ���� ������ �� ������ ���� � ������ ������ ���� OtherGraphNode �� ���������� !!!!!!!!
        /// </summary>
        /// <param name="InsertPosition"></param>
        /// <param name="OtherGraphNode"></param>
        public void InsertNodeConnection(Int32 InsertPosition, PolyhedronGraphNode OtherGraphNode)
        {
            m_NodeConnectionList.Insert(InsertPosition, OtherGraphNode);
        }

        /// <summary>
        /// ����� RemoveNodeConnection ������� ����� (������� ������ �� ���� OtherGraphNode �� ������ ������) ����� ������ ����� � OtherGraphNode
        /// ��� ���� ������ �� ������ ���� �� ������ ������ ���� OtherGraphNode �� ��������� (��� ����� � �� ������������)
        /// </summary>
        /// <param name="OtherGraphNode"></param>
        public void RemoveNodeConnection(PolyhedronGraphNode OtherGraphNode)
        {
            m_NodeConnectionList.Remove(OtherGraphNode);
        }

        /// <summary>
        /// ����� GetConnectionIndex ���������� ������ (� ������ ������ ������� ����) ����� ������� ���� � ����� ConnectionNode
        /// </summary>
        /// <param name="ConnectionNode"></param>
        /// <returns></returns>
        public Int32 GetConnectionIndex(PolyhedronGraphNode ConnectionNode)
        {
            return m_NodeConnectionList.IndexOf(ConnectionNode);
        }

        /// <summary>
        /// ����� HasConnectionWithNode ���������� true, ���� ������ ���� ����� ����� � ����� OtherGraphNode; ����� ���������� false
        /// </summary>
        /// <param name="OtherGraphNode"></param>
        /// <returns></returns>
        public Boolean HasConnectionWithNode(PolyhedronGraphNode OtherGraphNode)
        {
            return (m_NodeConnectionList.IndexOf(OtherGraphNode) != -1);
        }

        /// <summary>
        /// ��������-���������� ��� ������� (������/������) � ����, � ������� ������ ������, �� ������ ������ ������� ����
        /// </summary>
        /// <param name="NodeConnectionIndex"></param>
        /// <returns></returns>
        public PolyhedronGraphNode this[Int32 NodeConnectionIndex]
        {
            get
            {
                return m_NodeConnectionList[NodeConnectionIndex];
            }
            set
            {
                m_NodeConnectionList[NodeConnectionIndex] = value;
            }
        }

        /// <summary>
        /// �������� NodeConnectionCount ���������� ���������� ������ ������� ���� (���������� ����� � ������ ������)
        /// </summary>
        public Int32 NodeConnectionCount
        {
            get
            {
                return m_NodeConnectionList.Count;
            }
        }

        /// <summary>
        /// NodeNormal - �������� ��� ������� (������) � "�������" ������� � �����, ������� ������������� ������� ���� �����
        /// </summary>
        public Vector3D NodeNormal
        {
            get
            {
                return m_NodeNormal;
            }
        }

        /// <summary>
        /// ����� OrderConnections ������������� ����� � ������ ������ ���� ���, ����� ��� ���� ����������� ������ �.�.
        /// </summary>
        public void OrderConnections()
        {
            // ������, ��������� � ������ �����, ���������� ����� ez
            Vector3D ez = this.NodeNormal;
            // ���������. ���������� �������, ���������� � ����� ����� 0 �� ������ �����, ������������ ����������� ez ���������� ����� ex
            Vector3D ex = m_NodeConnectionList[0].NodeNormal.GetPerpendicularComponent(ez);
            ex.Normalize();
            // ey = [ez,ex]
            Vector3D ey = Vector3D.VectorProduct(ez, ex);
            //ey.Normalize();

            List<PolyhedronGraphNode> DisorderConnList = new List<PolyhedronGraphNode>(m_NodeConnectionList);
            List<PolyhedronGraphNode> OrderConnList = new List<PolyhedronGraphNode>();

            OrderConnList.Add(m_NodeConnectionList[0]);
            DisorderConnList.RemoveAt(0);

            while (DisorderConnList.Count > 0)
            {
                Double MinExAngle = 2 * Math.PI;
                Int32 MinExAngleConnIndex = -1;

                for (Int32 DisorderConnIndex = 0; DisorderConnIndex < DisorderConnList.Count; DisorderConnIndex++)
                {
                    // ��������� ������������ �������� ex � DisorderConnList[DisorderConnIndex].NodeNormal
                    Double ExScalarProductValue = Vector3D.ScalarProduct(ex, DisorderConnList[DisorderConnIndex].NodeNormal);
                    // ��������� ������������ �������� ey � DisorderConnList[DisorderConnIndex].NodeNormal
                    Double EyScalarProductValue = Vector3D.ScalarProduct(ey, DisorderConnList[DisorderConnIndex].NodeNormal);
                    // ���� ����� ��������� ex � // ��������� ������������ �������� ex � DisorderConnList[DisorderConnIndex].NodeNormal ������ �.�. (!!!)
                    Double ExAngle = (EyScalarProductValue < 0 ? 2 * Math.PI - Math.Acos(ExScalarProductValue) : Math.Acos(ExScalarProductValue));

                    if (ExAngle < MinExAngle)
                    {
                        MinExAngle = ExAngle;
                        MinExAngleConnIndex = DisorderConnIndex;
                    }
                }

                if (MinExAngleConnIndex == -1)
                {
                    throw new AlgorithmException();
                }

                OrderConnList.Add(DisorderConnList[MinExAngleConnIndex]);
                DisorderConnList.RemoveAt(MinExAngleConnIndex);
            }

            m_NodeConnectionList = OrderConnList;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class PlaneClass
    {
        /// <summary>
        /// m_AKoeff - ����������� A ��������� ���������
        /// </summary>
        private Double m_AKoeff;
        /// <summary>
        /// m_BKoeff - ����������� B ��������� ���������
        /// </summary>
        private Double m_BKoeff;
        /// <summary>
        /// m_CKoeff - ����������� C ��������� ���������
        /// </summary>
        private Double m_CKoeff;
        /// <summary>
        /// m_DKoeff - ����������� D ��������� ���������
        /// </summary>
        private Double m_DKoeff;

        public PlaneClass(Vector3D NormalVector, Double SupportFuncValue)
        {
            // NormalVector = {A;B;C}
            m_AKoeff = NormalVector.XCoord;
            m_BKoeff = NormalVector.YCoord;
            m_CKoeff = NormalVector.ZCoord;
            // SupportFuncValue + DKoeff = 0
            m_DKoeff = -SupportFuncValue;
        }

        /// <summary>
        /// AKoeff - �������� ��� ������� (������) � ������������ A ��������� ���������
        /// </summary>
        public Double AKoeff
        {
            get
            {
                return m_AKoeff;
            }
        }

        /// <summary>
        /// BKoeff - �������� ��� ������� (������) � ������������ B ��������� ���������
        /// </summary>
        public Double BKoeff
        {
            get
            {
                return m_BKoeff;
            }
        }

        /// <summary>
        /// CKoeff - �������� ��� ������� (������) � ������������ C ��������� ���������
        /// </summary>
        public Double CKoeff
        {
            get
            {
                return m_CKoeff;
            }
        }

        /// <summary>
        /// DKoeff - �������� ��� ������� (������) � ������������ D ��������� ���������
        /// </summary>
        public Double DKoeff
        {
            get
            {
                return m_DKoeff;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal enum CrossingObjectTypeEnum
    {
        GraphNode,
        GraphConnection
    }

    /// <summary>
    /// 
    /// </summary>
    internal class CrossingObjectClass
    {
        /// <summary>
        /// CrossingObjectType - ��� ������� ����������� (���� ��� �����)
        /// </summary>
        public readonly CrossingObjectTypeEnum CrossingObjectType;
        /// <summary>
        /// PGNode1 - ���� ����� 1 ������� ����������� (������������� ���� ��� �����; ��� ���� ��� ����)
        /// </summary>
        public readonly PolyhedronGraphNode PGNode1;
        /// <summary>
        /// PGNode1 - ���� ����� 2 ������� ����������� (������������� ���� ��� �����; ��� ���� �������� null)
        /// </summary>
        public readonly PolyhedronGraphNode PGNode2;

        public CrossingObjectClass(CrossingObjectTypeEnum COType, PolyhedronGraphNode PGNode1, PolyhedronGraphNode PGNode2)
        {
            this.CrossingObjectType = COType;

            this.PGNode1 = PGNode1;
            this.PGNode2 = PGNode2;
            /* ����� ������ ���� �������� �� ������������ ������ � PGNode1 � � PGNode2 � ����������� �� ���� ������� ����������� */
        }

        public Boolean Equals(CrossingObjectClass OtherCrossingObject)
        {
            if ((Object)OtherCrossingObject == null)
            {
                return false;
            }

            if (this.CrossingObjectType != OtherCrossingObject.CrossingObjectType)
            {
                return false;
            }

            /*return (Object.ReferenceEquals(this.PGNode1, OtherCrossingObject.PGNode1) && Object.ReferenceEquals(this.PGNode2, OtherCrossingObject.PGNode2)) ||
                   (Object.ReferenceEquals(this.PGNode1, OtherCrossingObject.PGNode2) && Object.ReferenceEquals(this.PGNode2, OtherCrossingObject.PGNode1));*/
            // ���� ������ ����������� ��������� (!!!!!) ���������, �� ���������� ����� ������� � ������� �� ����������� ����
            return (Object.ReferenceEquals(this.PGNode1, OtherCrossingObject.PGNode1) && Object.ReferenceEquals(this.PGNode2, OtherCrossingObject.PGNode2));
        }

        public override Boolean Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            CrossingObjectClass OtherCrossingObject = (obj as CrossingObjectClass);
            if ((Object)OtherCrossingObject == null)
            {
                return false;
            }

            return Equals(OtherCrossingObject);
        }

        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Boolean operator==(CrossingObjectClass CrossingObject1, CrossingObjectClass CrossingObject2)
        {
            return CrossingObject1.Equals(CrossingObject2);
        }

        public static Boolean operator!=(CrossingObjectClass CrossingObject1, CrossingObjectClass CrossingObject2)
        {
            return !CrossingObject1.Equals(CrossingObject2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PolyhedronStructureClass
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
        /// m_PolyhedronVertexList2 - ������ ���� ������ �������������, ���������� ��� �������������� ��������� ������������� �� �����
        /// </summary>
        private List<VertexClass> m_PolyhedronVertexList2;
        /// <summary>
        /// m_PolyhedronSideList2 - ������ ���� ������ �������������, ���������� ��� �������������� ��������� ������������� �� �����
        /// </summary>
        private List<SideClass> m_PolyhedronSideList2;
        /// <summary>
        /// m_PieceDirectingVector - ������������ ������ ������� Pi
        /// </summary>
        private Vector3D m_PiDirectingVector;
        /// <summary>
        /// Epsilon - ...
        /// </summary>
        private readonly Double Epsilon = 1e-9;
        /// <summary>
        /// PointCoordDigits - ...
        /// </summary>
        private readonly Int32 PointCoordDigits = 9;

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
            // ���� ���������� ����� ������� ... ����� �������
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

            return new Point3D(XCoord, YCoord, ZCoord);
        }

        /// <summary>
        /// ����� PolyhedronSupportFunction ���������� �������� ������� ������� ������������� ��� ������� VectorArg
        /// </summary>
        /// <param name="VectorArg"></param>
        /// <returns></returns>
        private Double PolyhedronSupportFunction(Vector3D VectorArg)
        {
            Double ScalarProduct = 0;
            Double SupportFuncValue = Double.NaN;

            foreach (VertexClass CurrentVertex in m_PolyhedronVertexList)
            {
                ScalarProduct = VectorArg.XCoord * CurrentVertex.XCoord + VectorArg.YCoord * CurrentVertex.YCoord + VectorArg.ZCoord * CurrentVertex.ZCoord;

                if ((Double.IsNaN(SupportFuncValue)) || (SupportFuncValue < ScalarProduct))
                {
                    SupportFuncValue = ScalarProduct;
                }
            }

            return SupportFuncValue;
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
                // ����� ������� ����� ������� ���� ��� ���������� � ������� ���
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
        /// ����� FindVertexOnPoint ���� ������� (�� �������� �����) � ������ ������ m_PolyhedronVertexList2
        /// </summary>
        /// <param name="Point"></param>
        /// <returns></returns>
        private VertexClass FindVertexOnPoint(Point3D SourcePoint)
        {
            VertexClass FindingVertex = null;

            foreach (VertexClass CurrentVertex in m_PolyhedronVertexList2)
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
            while(CurrentPGNode != StartPGNode)
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
                throw new AlgorithmException();
            }

            return ShortestGraphPath;
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
                            // ������������� ������ ������ �����
                            List<VertexClass> OrderedSideVertexList = OrderingSideVertexList(CheckedSideVertexList);
                            // "�������" ������� � �����
                            Vector3D SideNormalVector = GetSideExternalNormal(LeftEdgeVertex, RightEdgeVertex, CurrentVertex);
                            // ID �����
                            Int32 NewSideID = m_PolyhedronSideList[m_PolyhedronSideList.Count - 1].ID + 1;

                            if (IsSideAlreadyAdded(SideNormalVector)) continue;
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
                        throw new AlgorithmException();
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
        /// ����� RecievePolyhedronStructureFromGraph �������� (���������������) ��������� ������������� �� ��� �����
        /// </summary>
        private void RecievePolyhedronStructureFromGraph()
        {
            List<PlaneClass> PolyhedronPlaneList = new List<PlaneClass>(m_PGNodeList.Count);

            foreach (PolyhedronGraphNode PGNode in m_PGNodeList)
            {
                Vector3D PlaneNormal = PGNode.NodeNormal;
                Double SupportFuncValue = PolyhedronSupportFunction(PlaneNormal);

                PolyhedronPlaneList.Add(new PlaneClass(PlaneNormal, SupportFuncValue));
            }

            for (Int32 PGNodeIndex = 0; PGNodeIndex < m_PGNodeList.Count; PGNodeIndex++)
            {
                PolyhedronGraphNode CurrentPGNode = m_PGNodeList[PGNodeIndex];
                // CurrentPlane ������������� CurrentPGNode
                // ��������� ����� 1
                PlaneClass CurrentPlane = PolyhedronPlaneList[PGNodeIndex];
                SideClass CurrentSide = new SideClass(new List<VertexClass>(), CurrentPGNode.ID, CurrentPGNode.NodeNormal);
                m_PolyhedronSideList2.Add(CurrentSide);

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
                        Vertex4PlanesCrossingPoint = new VertexClass(PlanesCrossingPoint, m_PolyhedronVertexList2.Count);
                        m_PolyhedronVertexList2.Add(Vertex4PlanesCrossingPoint);
                    }

                    if (CurrentSide.VertexCount == 0)
                    {
                        CurrentSide.AddVertex(Vertex4PlanesCrossingPoint);
                    }
                    else if ((CurrentSide[0] != Vertex4PlanesCrossingPoint) && (CurrentSide[CurrentSide.VertexCount-1] != Vertex4PlanesCrossingPoint))
                    {
                        CurrentSide.AddVertex(Vertex4PlanesCrossingPoint);
                    }
                }
            }
        }

        /// <summary>
        /// ����� GetFirstCrossingObject ���������� ������ ������ ����������� � G(...Pi...)
        /// </summary>
        /// <param name="StartingPGNode"></param>
        /// <returns></returns>
        private CrossingObjectClass GetFirstCrossingObject(PolyhedronGraphNode StartingPGNode)
        {
            CrossingObjectClass FirstCrossingObject = null;

            // ������� ����
            PolyhedronGraphNode CurrentPGNode = StartingPGNode;
            // ��������� ��������� ������������ �������, ���������� � ������� �����, � ������������� ������� Pi
            Double CurrentScalarProductValue = Vector3D.ScalarProduct(CurrentPGNode.NodeNormal, m_PiDirectingVector);
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
                    // ������� ��������� ������������ �������, ���������� � ���������� ���� �����, � ������������� ������� Pi
                    Double CurrentConnNodeScalarProductValue = Vector3D.ScalarProduct(CurrentConnPGNode.NodeNormal, m_PiDirectingVector);

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
        /// ����� GetFirstCrossingObject ���������� ������ ������ ����������� � G(...Pi...)
        /// </summary>
        /// <returns></returns>
        private CrossingObjectClass GetFirstCrossingObject()
        {
            return GetFirstCrossingObject(m_PGNodeList[0]);
        }

        /// <summary>
        /// ����� BuildCrossingNode ���������� ���� ����������� �������� ������� � G(...Pi...)
        /// ���� ������� ������ - ����, �� �� �� � ������������
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private PolyhedronGraphNode BuildCrossingNode(CrossingObjectClass CurrentCrossingObject)
        {
            PolyhedronGraphNode CrossingNode = null;

            if (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection)
            {
                Vector3D PlusValueVector = CurrentCrossingObject.PGNode1.NodeNormal;
                Vector3D MinusValueVector = CurrentCrossingObject.PGNode2.NodeNormal;
                // ������, ���������������� � PlusValueVector � MinusValueVector
                Vector3D Npm = Vector3D.VectorProduct(PlusValueVector, MinusValueVector);
                Vector3D ZeroValueVector = Vector3D.VectorProduct(Npm, m_PiDirectingVector);
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
        /// ����� CheckMoveDirection ���������� true, ���� ����������� �������� �� G(...Pi...) ����������, ����� ������������ false
        /// ���������� ��������� ����������� �������� ������ ������� �������, ���� �������� � ����� ������������� ������� Pi
        /// </summary>
        /// <param name="CheckCrossingObject"></param>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private Boolean CheckMoveDirection(CrossingObjectClass CheckCrossingObject, CrossingObjectClass CurrentCrossingObject)
        {
            // ������������ ������ Pi ���������� ����� ��� OZ
            Vector3D OZDirectingVector = m_PiDirectingVector;

            // ������ ���� �� ����������� �������� ������� � G(...Pi...); ������, ��������� � ���� �����, ���������� ����� ��� OX
            PolyhedronGraphNode CurrentCrossingNode = BuildCrossingNode(CurrentCrossingObject);
            Vector3D OXDirectingVector = CurrentCrossingNode.NodeNormal;

            // ������ ��� ��� OY ������ �� XYZ (��� ��������� ������������ ���� ��� OZ �� ��� ��� OX)
            Vector3D OYDirectingVector = Vector3D.VectorProduct(OZDirectingVector, OXDirectingVector);

            // ������ ���� �� ����������� ������������ ������� � G(...Pi...); ��������� ��������� ������������ �������, ���������� � ���� �����, � ���� ��� OY
            PolyhedronGraphNode CheckCrossingNode = BuildCrossingNode(CheckCrossingObject);
            Vector3D CheckVector = CheckCrossingNode.NodeNormal;
            Double ScalarProductValue = Vector3D.ScalarProduct(CheckVector, OYDirectingVector);

            // ���� ScalarProductValue = 0 - ��� ������ ������ ���������
            if (Math.Abs(ScalarProductValue) < Epsilon)
            {
                throw new AlgorithmException();
            }

            // ���� ����������� ��������� ������������ > 0, �� ����������� �������� ����������, ����� ����������� �������� ������������
            return (ScalarProductValue > 0 ? true : false);
        }

        /// <summary>
        /// ����� GetNextCrossingObject4GraphNode ���������� ��������� �� ����������� �������� ������ �����������, ���� ������� - ����
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private CrossingObjectClass GetNextCrossingObject4GraphNode(CrossingObjectClass CurrentCrossingObject)
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
                Double PGNode1ScalarProductValue = Vector3D.ScalarProduct(PGNode1.NodeNormal, m_PiDirectingVector);
                // ��������� ��������� ������������ ������� 2 � ������������� ������� Pi
                Double PGNode2ScalarProductValue = Vector3D.ScalarProduct(PGNode2.NodeNormal, m_PiDirectingVector);

                // ���� ��������� ������������ ���� 1 � ������������� ������� Pi == 0
                if (Math.Abs(PGNode1ScalarProductValue) < Epsilon)
                {
                    // ���� ����������� �������� ������� ���������, �� ���� ����� 1 ���������� ��������� �� �������� ��������
                    NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, PGNode1, null);
                    if (CheckMoveDirection(NextCrossingObject, CurrentCrossingObject))
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
                    if (CheckMoveDirection(NextCrossingObject, CurrentCrossingObject))
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
                    if (CheckMoveDirection(NextCrossingObject, CurrentCrossingObject))
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
        private CrossingObjectClass GetNextCrossingObject4GraphConn(CrossingObjectClass CurrentCrossingObject, PolyhedronGraphNode CurrentCrossingNode)
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

            Double NextPGNode1ScalarProductValue = Vector3D.ScalarProduct(NextPGNode1.NodeNormal, m_PiDirectingVector);
            Double NextPGNode2ScalarProductValue = Vector3D.ScalarProduct(NextPGNode2.NodeNormal, m_PiDirectingVector);

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
        private CrossingObjectClass GetNextCrossingObject(CrossingObjectClass CurrentCrossingObject, PolyhedronGraphNode CurrentCrossingNode)
        {
            // ���� ������� ������ � ����
            if (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode)
            {
                return GetNextCrossingObject4GraphNode(CurrentCrossingObject);
            }
            // ���� ������� ������ � �����
            else
            {
                return GetNextCrossingObject4GraphConn(CurrentCrossingObject, CurrentCrossingNode);
            }
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
        /// ����� BuildGFiGrid ������ ����� G(...Fi...) (��. ��������)
        /// </summary>
        private void BuildGFiGrid()
        {
            // ������ (�����������) ������ �����������
            CrossingObjectClass FirstCrossingObject = GetFirstCrossingObject();
            // ���������� ������ �����������
            // CrossingObjectClass PreviousCrossingObject = null;
            // ������� ������ �����������
            CrossingObjectClass CurrentCrossingObject = FirstCrossingObject;
            // ������ ���� �� ����������� �������� ������� � G(...Pi...) � ���������� ���
            // ���� ���� ���� ����������� � ������ �����, �� ��������� ��� � ��������������� ������ �� ������ ����
            PolyhedronGraphNode FirstCrossingNode = BuildCrossingNode(CurrentCrossingObject);
            // ������� ���� �����������
            PolyhedronGraphNode CurrentCrossingNode = FirstCrossingNode;
            // ���������� ���� �����������
            // PolyhedronGraphNode PreviousCrossingNode = null;
            if (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection)
            {
                AddCrossingNodeBetweenConn(CurrentCrossingObject.PGNode1, CurrentCrossingObject.PGNode2, CurrentCrossingNode);
            }

            do
            {
                CrossingObjectClass PreviousCrossingObject = CurrentCrossingObject;
                PolyhedronGraphNode PreviousCrossingNode = CurrentCrossingNode;
                // �������� ��������� �� �������� ������ (�����, ���� ����) � ������ ��� �������
                CurrentCrossingObject = GetNextCrossingObject(CurrentCrossingObject, CurrentCrossingNode);
                // ������ ���� �� ����������� �������� ������� � G(...Pi...)
                // ���� ���� ���� ����������� � ������ ����� (���� ���� ����� �������������� � ������ �����, ���� ������� ������ � ����, ���� ���� ��������� �������� ���� ����� � �� � ��� ������), �� ��������� ��� � ��������������� ������ �� ������ ����
                // �������� ������������ ������ ���� �� ������ � ������ (�����������) ������ ����������� (��� �������� ���������� ���������)
                CurrentCrossingNode = (CurrentCrossingObject == FirstCrossingObject ? FirstCrossingNode : BuildCrossingNode(CurrentCrossingObject));
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
                    // ������ ����� ����� ���������� ����� � ����� ����������� �� ������� ������� (����� ����������� � ������ ���������������)
                    // ������������� ���� ������� �����
                    PolyhedronGraphNode CurrentConnMinusNode = CurrentCrossingObject.PGNode2;
                    // ������ �� ������� ���� ����������� � ������ ������ ����������� ���� ��������� ����� ������ �� ������������� ���� ������� �����
                    Int32 PrevNode2CurrentMinusNodeConnIndex = PreviousCrossingNode.GetConnectionIndex(CurrentConnMinusNode);
                    PreviousCrossingNode.InsertNodeConnection(PrevNode2CurrentMinusNodeConnIndex + 1, CurrentCrossingNode);
                    // ������ �� ���������� ���� ��������� ����� ������ �� ������������� ���� ������� ����� (�� ������� ����� 1)
                    CurrentCrossingNode.InsertNodeConnection(1, PreviousCrossingNode);
                }

                // ���� ���������� ������ �����, � ������� ����
                if ((PreviousCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection) &&
                    (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode))
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

                // ���� ���������� � ������� ������� - �����
                if ((PreviousCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection) &&
                    (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection))
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
                        throw new AlgorithmException();
                    }
                }
            }
            while (CurrentCrossingObject != FirstCrossingObject);
        }

        public PolyhedronStructureClass(Point3D[] PolyhedronVertexList)
        {
            m_PolyhedronVertexList = new List<VertexClass>();
            m_PolyhedronSideList = new List<SideClass>();

            for (Int32 VertexIndex = 0; VertexIndex < PolyhedronVertexList.Length; VertexIndex++)
            {
                m_PolyhedronVertexList.Add(new VertexClass(PolyhedronVertexList[VertexIndex], VertexIndex + 1));
            }

            RecievePolyhedronStructure();

            m_PGNodeList = new List<PolyhedronGraphNode>();
            RecievePolyhedronGraph();
            GraphTriangulation();

            m_PolyhedronVertexList2 = new List<VertexClass>();
            m_PolyhedronSideList2 = new List<SideClass>();
            RecievePolyhedronStructureFromGraph();

            m_PiDirectingVector = new Vector3D(0.1, 1, 0);
            m_PiDirectingVector.Normalize();
            //BuildGFiGrid();
        }

        /// <summary>
        /// ����� GetPolyhedronStructureDescription ���������� � ��������� ���� (� ���� ������� �����) �������� ��������� �������������
        /// </summary>
        /// <returns></returns>
        public String[] GetPolyhedronStructureDescription()
        {
            String[] PolyhedronStructureDescription = new String[m_PolyhedronSideList.Count];

            for (Int32 SideIndex = 0; SideIndex < m_PolyhedronSideList.Count; SideIndex++)
            {
                StringBuilder CurrentString = new StringBuilder();

                CurrentString.AppendFormat("Side number {0} :\r\n", m_PolyhedronSideList[SideIndex].ID);
                CurrentString.AppendFormat("Has normal with coords ({0}; {1}; {2})\r\n", m_PolyhedronSideList[SideIndex].SideNormal.XCoord, m_PolyhedronSideList[SideIndex].SideNormal.YCoord, m_PolyhedronSideList[SideIndex].SideNormal.ZCoord);

                for (Int32 VertexIndex = 0; VertexIndex < m_PolyhedronSideList[SideIndex].VertexCount; VertexIndex++)
                {
                    VertexClass CurrentVertex = m_PolyhedronSideList[SideIndex][VertexIndex];
                    CurrentString.AppendFormat("Vertex number {0} with coords ({1};{2};{3})\r\n", CurrentVertex.ID, CurrentVertex.XCoord, CurrentVertex.YCoord, CurrentVertex.ZCoord);
                }

                CurrentString.Append("\r\n");
                PolyhedronStructureDescription[SideIndex] = CurrentString.ToString();
            }

            return PolyhedronStructureDescription;
        }

        /// <summary>
        /// ����� GetPolyhedronGraphDescription ���������� � ��������� ���� (� ���� ������� �����) �������� ����� �������������
        /// </summary>
        /// <returns></returns>
        public String[] GetPolyhedronGraphDescription()
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

        /// <summary>
        /// ����� GetPolyhedronGraphDescription ���������� � ��������� ���� (� ���� ������� �����) �������� ����� ������������� � �������������� ������ �.�. �������
        /// </summary>
        /// <returns></returns>
        public String[] GetOrderedPolyhedronGraphDescription()
        {
            foreach(PolyhedronGraphNode PGNode in m_PGNodeList)
            {
                PGNode.OrderConnections();
            }

            return GetPolyhedronGraphDescription();
        }

        /// <summary>
        /// ����� GetPolyhedronStructureDescription2 ���������� � ��������� ���� (� ���� ������� �����) �������� ��������� �������������, ���������� ��� �� �������������� �� �����
        /// </summary>
        /// <returns></returns>
        public String[] GetPolyhedronStructureDescription2()
        {
            String[] PolyhedronStructureDescription = new String[m_PolyhedronSideList2.Count];

            for (Int32 SideIndex = 0; SideIndex < m_PolyhedronSideList2.Count; SideIndex++)
            {
                StringBuilder CurrentString = new StringBuilder();

                CurrentString.AppendFormat("Side number {0} :\r\n", m_PolyhedronSideList2[SideIndex].ID);
                CurrentString.AppendFormat("Has normal with coords ({0}; {1}; {2})\r\n", m_PolyhedronSideList2[SideIndex].SideNormal.XCoord, m_PolyhedronSideList2[SideIndex].SideNormal.YCoord, m_PolyhedronSideList2[SideIndex].SideNormal.ZCoord);

                for (Int32 VertexIndex = 0; VertexIndex < m_PolyhedronSideList2[SideIndex].VertexCount; VertexIndex++)
                {
                    VertexClass CurrentVertex = m_PolyhedronSideList2[SideIndex][VertexIndex];
                    CurrentString.AppendFormat("Vertex number {0} with coords ({1};{2};{3})\r\n", CurrentVertex.ID, CurrentVertex.XCoord, CurrentVertex.YCoord, CurrentVertex.ZCoord);
                }

                CurrentString.Append("\r\n");
                PolyhedronStructureDescription[SideIndex] = CurrentString.ToString();
            }

            return PolyhedronStructureDescription;
        }

        /// <summary>
        /// ����� GetPolyhedronGraphDescription ���������� � ��������� ���� (� ���� ������� �����) �������� ����� GFi
        /// </summary>
        /// <returns></returns>
        public String[] GetGFiGraphDescription()
        {
            BuildGFiGrid();
            return GetPolyhedronGraphDescription();
        }
    }
}
