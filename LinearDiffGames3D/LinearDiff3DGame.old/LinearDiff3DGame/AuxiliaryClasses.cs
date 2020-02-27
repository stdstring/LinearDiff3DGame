using System;
using System.Collections.Generic;
using System.Text;

namespace MathPostgraduateStudy.LinearDiff3DGame
{
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
    public struct Vector3D
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
        /// ����� AngleBetweenVectors ���������� �������� ���� (� ��������) ����� ��������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>���� (� ��������) ����� ��������� a � b</returns>
        public static Double AngleBetweenVectors(Vector3D a, Vector3D b)
        {
            Double CosValue = Vector3D.ScalarProduct(a, b) / (a.Length * b.Length);
            // ���������� ����� ������, ��� ��-�� ������ ���������� �������� �������� ���� ����� ����� > 1
            return Math.Acos(Math.Round(CosValue, AcosDigits));
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
    public class VertexClass
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

        public VertexClass(Point3D Vertex, Int32 VertexID)
            : this(Vertex.XCoord, Vertex.YCoord, Vertex.ZCoord, VertexID)
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
    public class SideClass
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
    public class PolyhedronGraphNode
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
        /// m_ID - ���������� ������������� ���� (��������� � ID �����, ������� ������� ���� �������������)
        /// </summary>
        private Int32 m_ID;

        public PolyhedronGraphNode(Int32 ID, Vector3D NodeNormal)
        {
            m_NodeConnectionList = new List<PolyhedronGraphNode>();

            this.m_ID = ID;
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
        /// ID - �������� ��� ������� (������/������) � ����������� �������������� ����
        /// </summary>
        public Int32 ID
        {
            get
            {
                return m_ID;
            }
            set
            {
                m_ID = value;
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
                    throw new AlgorithmException("Error when ordering connections");
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
    public class PlaneClass
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
    public enum CrossingObjectTypeEnum
    {
        GraphNode,
        GraphConnection
    }

    /// <summary>
    /// 
    /// </summary>
    public class CrossingObjectClass
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

        public static Boolean operator ==(CrossingObjectClass CrossingObject1, CrossingObjectClass CrossingObject2)
        {
            return CrossingObject1.Equals(CrossingObject2);
        }

        public static Boolean operator !=(CrossingObjectClass CrossingObject1, CrossingObjectClass CrossingObject2)
        {
            return !CrossingObject1.Equals(CrossingObject2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal struct PolyhedronGraphConnection
    {
        /// <summary>
        /// 
        /// </summary>
        private PolyhedronGraphNode m_PGNode1;
        /// <summary>
        /// 
        /// </summary>
        private PolyhedronGraphNode m_PGNode2;

        public PolyhedronGraphConnection(PolyhedronGraphNode PGNode1, PolyhedronGraphNode PGNode2)
        {
            m_PGNode1 = (PGNode1.ID < PGNode2.ID ? PGNode1 : PGNode2);
            m_PGNode2 = (PGNode2.ID < PGNode1.ID ? PGNode1 : PGNode2);
        }

        /// <summary>
        /// 
        /// </summary>
        public PolyhedronGraphNode PGNode1
        {
            get
            {
                return m_PGNode1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PolyhedronGraphNode PGNode2
        {
            get
            {
                return m_PGNode2;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SuspiciousConnectionSetClass
    {
        /// <summary>
        /// 
        /// </summary>
        private List<PolyhedronGraphConnection> m_SuspiciousConnectionSet;

        public SuspiciousConnectionSetClass()
        {
            m_SuspiciousConnectionSet = new List<PolyhedronGraphConnection>();
        }

        /// <summary>
        /// ����� AddConnection ��������� ����� � ������ "��������������" ������ �
        /// </summary>
        /// <param name="PGNode1"></param>
        /// <param name="PGNode2"></param>
        public void AddConnection(PolyhedronGraphNode PGNode1, PolyhedronGraphNode PGNode2)
        {
            PolyhedronGraphConnection PGConn = new PolyhedronGraphConnection(PGNode1, PGNode2);

            if (m_SuspiciousConnectionSet.IndexOf(PGConn) == -1)
            {
                m_SuspiciousConnectionSet.Add(PGConn);
            }
        }

        /// <summary>
        /// ����� GetConnection ���������� ����, ���������� "��������������" ����� � ������� (��������) ConnectionIndex (� ������ �)
        /// </summary>
        /// <param name="ConnectionIndex"></param>
        /// <returns></returns>
        public PolyhedronGraphNode[] GetConnection(Int32 ConnectionIndex)
        {
            PolyhedronGraphConnection CurrentPGConn = m_SuspiciousConnectionSet[ConnectionIndex];

            PolyhedronGraphNode[] ConnPGNodes = new PolyhedronGraphNode[2];
            ConnPGNodes[0] = CurrentPGConn.PGNode1;
            ConnPGNodes[1] = CurrentPGConn.PGNode2;

            return ConnPGNodes;
        }

        /// <summary>
        /// ����� RemoveConnection ������� ����� � ������� (��������) ConnectionIndex �� ������ "��������������" ������ (�� ������ �)
        /// </summary>
        /// <param name="ConnectionIndex"></param>
        public void RemoveConnection(Int32 ConnectionIndex)
        {
            m_SuspiciousConnectionSet.RemoveAt(ConnectionIndex);
        }

        /// <summary>
        /// ����� RemoveConnection ������� ����� ����� ������ PGNode1 � PGNode2 �� ������ "��������������" ������ (�� ������ �)
        /// </summary>
        /// <param name="PGNode1"></param>
        /// <param name="PGNode2"></param>
        public void RemoveConnection(PolyhedronGraphNode PGNode1, PolyhedronGraphNode PGNode2)
        {
            PolyhedronGraphConnection PGConn = new PolyhedronGraphConnection(PGNode1, PGNode2);
            
            m_SuspiciousConnectionSet.Remove(PGConn);
        }

        /// <summary>
        /// ����� RemoveConnections ������� ��� ����� ���� PGNode �� ������ "��������������" ������ (�� ������ �)
        /// </summary>
        /// <param name="PGNode"></param>
        public void RemoveConnections(PolyhedronGraphNode PGNode)
        {
            for (Int32 PGConnIndex = 0; PGConnIndex < m_SuspiciousConnectionSet.Count; )
            {
                PolyhedronGraphConnection CurrentPGConn = m_SuspiciousConnectionSet[PGConnIndex];
                if ((CurrentPGConn.PGNode1 == PGNode) || (CurrentPGConn.PGNode2 == PGNode))
                {
                    m_SuspiciousConnectionSet.RemoveAt(PGConnIndex);
                }
                else
                {
                    PGConnIndex++;
                }
            }
        }

        /// <summary>
        /// ConnectionCount - ���������� "��������������" ������ (� ������ �)
        /// </summary>
        public Int32 ConnectionCount
        {
            get
            {
                return m_SuspiciousConnectionSet.Count;
            }
        }
    }
}
