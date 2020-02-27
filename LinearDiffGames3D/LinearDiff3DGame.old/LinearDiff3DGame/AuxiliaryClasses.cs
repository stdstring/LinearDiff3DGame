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
        /// m_XCoord - координата X точки
        /// </summary>
        private Double m_XCoord;
        /// <summary>
        /// m_YCoord - координата Y точки
        /// </summary>
        private Double m_YCoord;
        /// <summary>
        /// m_ZCoord - координата Z точки
        /// </summary>
        private Double m_ZCoord;

        public Point3D(Double XCoord, Double YCoord, Double ZCoord)
        {
            this.m_XCoord = XCoord;
            this.m_YCoord = YCoord;
            this.m_ZCoord = ZCoord;
        }

        /// <summary>
        /// XCoord - свойство для доступа к координате X точки
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
        /// YCoord - свойство для доступа к координате Y точки
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
        /// ZCoord - свойство для доступа к координате Z точки
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
        /// m_XCoord - координата X вектора
        /// </summary>
        private Double m_XCoord;
        /// <summary>
        /// m_YCoord - координата Y вектора
        /// </summary>
        private Double m_YCoord;
        /// <summary>
        /// m_ZCoord - координата Z вектора
        /// </summary>
        private Double m_ZCoord;
        /// <summary>
        /// MinAngleBetweenVectors - минимальное значение угла между векторами, при котором вектора считаются разными 
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
        /// метод Normalize нормирует текущий вектор
        /// </summary>
        public void Normalize()
        {
            Double VectorLength = Math.Sqrt(m_XCoord * m_XCoord + m_YCoord * m_YCoord + m_ZCoord * m_ZCoord);

            this.m_XCoord /= VectorLength;
            this.m_YCoord /= VectorLength;
            this.m_ZCoord /= VectorLength;
        }

        /// <summary>
        /// метод ApproxEquals возвращает true, если текущий и OtherVector вектора приблизительно равны,
        /// т.е. если угол между текущим и OtherVector векторами меньше, чем величина MinAngleBetweenVectors
        /// </summary>
        /// <param name="OtherVector"></param>
        /// <returns></returns>
        public Boolean ApproxEquals(Vector3D OtherVector)
        {
            Double AcosValue = Math.Round(Vector3D.ScalarProduct(this, OtherVector) / (this.Length * OtherVector.Length), AcosDigits);
            Double AngleBetweenVectors = Math.Acos(AcosValue);

            // округление уже было ...  скорее всего его достаточно
            // return (Math.Abs(AngleBetweenVectors) < MinAngleBetweenVectors);
            return (AngleBetweenVectors == 0);
        }

        /// <summary>
        /// метод GetParallelComponent возвращает компоненту текущего вектора, параллельную вектору DirectingVector
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
        /// метод GetPerpendicularComponent возвращает компоненту текущего вектора, перпендикулярную вектору DirectingVector
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
        /// XCoord - свойство для доступа к координате X вектора
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
        /// YCoord - свойство для доступа к координате Y вектора
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
        /// ZCoord - свойство для доступа к координате Z вектора
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
        /// Length - длина вектора
        /// </summary>
        public Double Length
        {
            get
            {
                return Math.Sqrt(m_XCoord * m_XCoord + m_YCoord * m_YCoord + m_ZCoord * m_ZCoord);
            }
        }

        /// <summary>
        /// метод ScalarProduct возвращает результат скалярного произведения векторов a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>результат скалярного произведения векторов a и b</returns>
        public static Double ScalarProduct(Vector3D a, Vector3D b)
        {
            return a.XCoord * b.XCoord + a.YCoord * b.YCoord + a.ZCoord * b.ZCoord;
        }

        /// <summary>
        /// метод VectorProduct возвращает результат (вектор) векторного произведения векторов a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>результат (вектор) векторного произведения векторов a и b</returns>
        public static Vector3D VectorProduct(Vector3D a, Vector3D b)
        {
            Double XCoord = a.YCoord * b.ZCoord - a.ZCoord * b.YCoord;
            Double YCoord = a.ZCoord * b.XCoord - a.XCoord * b.ZCoord;
            Double ZCoord = a.XCoord * b.YCoord - a.YCoord * b.XCoord;

            return new Vector3D(XCoord, YCoord, ZCoord);
        }

        /// <summary>
        /// метод MixedProduct возвращает результат смешанного произведения векторов a, b и c
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <param name="c">вектор c</param>
        /// <returns>результат смешанного произведения векторов a, b и c</returns>
        public static Double MixedProduct(Vector3D a, Vector3D b, Vector3D c)
        {
            return Vector3D.ScalarProduct(a, Vector3D.VectorProduct(b, c));
        }

        /// <summary>
        /// метод AngleBetweenVectors возвращает величину угла (в радианах) между векторами a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>угол (в радианах) между векторами a и b</returns>
        public static Double AngleBetweenVectors(Vector3D a, Vector3D b)
        {
            Double CosValue = Vector3D.ScalarProduct(a, b) / (a.Length * b.Length);
            // округление нужно потому, что из-за ошибок округления значение косинуса угла может стать > 1
            return Math.Acos(Math.Round(CosValue, AcosDigits));
        }

        /// <summary>
        /// свойство ZeroVector3D - нулевой вектор
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
        /// m_SideList - список граней, которым принадлежит данная вершина
        /// </summary>
        private List<SideClass> m_SideList;

        /// <summary>
        /// m_XCoord - координата X вершины
        /// </summary>
        private Double m_XCoord;
        /// <summary>
        /// m_YCoord - координата Y вершины
        /// </summary>
        private Double m_YCoord;
        /// <summary>
        /// m_ZCoord - координата Z вершины
        /// </summary>
        private Double m_ZCoord;
        /// <summary>
        /// Dimension - размерность вершины (в нашем случае 3)
        /// </summary>
        public readonly Int32 Dimension = 3;
        /// <summary>
        /// ID - уникальный идентификатор вершины
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
        /// метод AddSide добавляет грань Side в список граней, которым принадлежит данная вершина
        /// </summary>
        /// <param name="Side">добавляемая грань</param>
        public void AddSide(SideClass Side)
        {
            m_SideList.Add(Side);
        }

        /// <summary>
        /// метод RemoveSide удаляет грань Side из списка граней, которым принадлежит данная вершина
        /// </summary>
        /// <param name="Side">удаляемая грань</param>
        public void RemoveSide(SideClass Side)
        {
            m_SideList.Remove(Side);
        }

        /// <summary>
        /// свойство-индексатор для доступа (чтение) к грани из списка граней, которым принадлежит данная вершина
        /// </summary>
        /// <param name="SideIndex">индекс грани в списке граней</param>
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
        /// свойство SideCount возвращает количество граней в списке граней, которым принадлежит данная вершина
        /// </summary>
        public Int32 SideCount
        {
            get
            {
                return m_SideList.Count;
            }
        }

        /// <summary>
        /// XCoord - свойство для доступа к координате X вершины
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
        /// YCoord - свойство для доступа к координате Y вершины
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
        /// ZCoord - свойство для доступа к координате Z вершины
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
        /// m_VertexList - список вершин, которые принадлежат данной грани
        /// </summary>
        private List<VertexClass> m_VertexList;
        /// <summary>
        /// m_SideNormal - "внешняя" нормаль к грани (вычисляется извне)
        /// </summary>
        private Vector3D m_SideNormal;
        /// <summary>
        /// ID - уникальный идентификатор вершины
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
        /// метод AddVertex добавляет вершину Vertex в список вершин, которые принадлежат данной грани
        /// </summary>
        /// <param name="Vertex">добавляемая вершина</param>
        public void AddVertex(VertexClass Vertex)
        {
            m_VertexList.Add(Vertex);
            Vertex.AddSide(this);
        }

        /// <summary>
        /// метод RemoveVertex удаляет вершину Vertex из списка вершин, которые принадлежат данной грани
        /// </summary>
        /// <param name="Vertex">удаляемая вершина</param>
        public void RemoveVertex(VertexClass Vertex)
        {
            m_VertexList.Remove(Vertex);
            Vertex.RemoveSide(this);
        }

        /// <summary>
        /// метод HasVertex возвращает true, если вершина Vertex принадлежит данной плоскости
        /// </summary>
        /// <param name="Vertex"></param>
        /// <returns></returns>
        public Boolean HasVertex(VertexClass Vertex)
        {
            return (m_VertexList.IndexOf(Vertex) != -1);
        }

        /// <summary>
        /// метод GetNeighbourSide возвращает для данной грани соседа по ребру, образованному вершинами EdgeVertex1 и EdgeVertex2
        /// </summary>
        /// <param name="EdgeVertex1">первая вершина ребра</param>
        /// <param name="EdgeVertex2">вторая вершина ребра</param>
        /// <returns>сосед по ребру для данной грани</returns>
        public SideClass GetNeighbourSide(VertexClass EdgeVertex1, VertexClass EdgeVertex2)
        {
            SideClass NeighbourSide = null;

            // проверка на то, что EdgeVertex1 и EdgeVertex2 принадлежат текущей грани
            if ((m_VertexList.IndexOf(EdgeVertex1) == -1) || (m_VertexList.IndexOf(EdgeVertex2) == -1))
            {
                throw new ArgumentException("EdgeVertex1 and EdgeVertex2 must belong this (current) side");
            }
            // нужна проверка, что EdgeVertex1 и EdgeVertex2 на самом деле ребро !!!!!!! (ее нет)
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
        /// свойство-индексатор для доступа (чтение) к вершине из списка вершин, которые принадлежат данной грани
        /// </summary>
        /// <param name="VertexIndex">индекс вершины в списке вершин</param>
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
        /// свойство VertexCount возвращает количество вершин в списке вершин, которые принадлежат данной грани
        /// </summary>
        public Int32 VertexCount
        {
            get
            {
                return m_VertexList.Count;
            }
        }

        /// <summary>
        /// SideNormal - свойство для доступа (чтение) к "внешней" нормали грани
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
        /// m_NodeConnectionList - список связей данного узла
        /// </summary>
        private List<PolyhedronGraphNode> m_NodeConnectionList;
        /// <summary>
        /// m_NodeNormal - "внешняя" нормаль к грани, которая соответствует данному узлу графа
        /// </summary>
        private Vector3D m_NodeNormal;
        /// <summary>
        /// m_ID - уникальный идентификатор узла (совпадает с ID грани, которая данному узлу соответствует)
        /// </summary>
        private Int32 m_ID;

        public PolyhedronGraphNode(Int32 ID, Vector3D NodeNormal)
        {
            m_NodeConnectionList = new List<PolyhedronGraphNode>();

            this.m_ID = ID;
            this.m_NodeNormal = NodeNormal;
        }

        /// <summary>
        /// метод AddNodeConnection создает связь (помещает ссылку на узел OtherGraphNode в список связей) между данным узлом и OtherGraphNode
        /// при этом ссылка на данный узел с список связей узла OtherGraphNode не помещается !!!!!!!!
        /// </summary>
        /// <param name="OtherGraphNode"></param>
        public void AddNodeConnection(PolyhedronGraphNode OtherGraphNode)
        {
            m_NodeConnectionList.Add(OtherGraphNode);
        }

        /// <summary>
        /// метод InsertNodeConnection создает связь (помещает ссылку на узел OtherGraphNode в список связей по индексу InsertPosition) между данным узлом и OtherGraphNode
        /// при этом ссылка на данный узел с список связей узла OtherGraphNode не помещается !!!!!!!!
        /// </summary>
        /// <param name="InsertPosition"></param>
        /// <param name="OtherGraphNode"></param>
        public void InsertNodeConnection(Int32 InsertPosition, PolyhedronGraphNode OtherGraphNode)
        {
            m_NodeConnectionList.Insert(InsertPosition, OtherGraphNode);
        }

        /// <summary>
        /// метод RemoveNodeConnection удаляет связь (удаляет ссылку на узел OtherGraphNode из списка связей) между данным узлом и OtherGraphNode
        /// при этом ссылка на данный узел из списка связей узла OtherGraphNode не удаляется (она может и не существовать)
        /// </summary>
        /// <param name="OtherGraphNode"></param>
        public void RemoveNodeConnection(PolyhedronGraphNode OtherGraphNode)
        {
            m_NodeConnectionList.Remove(OtherGraphNode);
        }

        /// <summary>
        /// метод GetConnectionIndex возвращает индекс (в списке связей данного узла) связи данного узла с узлом ConnectionNode
        /// </summary>
        /// <param name="ConnectionNode"></param>
        /// <returns></returns>
        public Int32 GetConnectionIndex(PolyhedronGraphNode ConnectionNode)
        {
            return m_NodeConnectionList.IndexOf(ConnectionNode);
        }

        /// <summary>
        /// метод HasConnectionWithNode возвращает true, если данный узел имеет связь с узлом OtherGraphNode; иначе возвращает false
        /// </summary>
        /// <param name="OtherGraphNode"></param>
        /// <returns></returns>
        public Boolean HasConnectionWithNode(PolyhedronGraphNode OtherGraphNode)
        {
            return (m_NodeConnectionList.IndexOf(OtherGraphNode) != -1);
        }

        /// <summary>
        /// свойство-индексатор для доступа (чтение/запись) к узлу, с которым связан данный, из списка связей данного узла
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
        /// свойство NodeConnectionCount возвращает количество связей данного узла (количество узлов в списке связей)
        /// </summary>
        public Int32 NodeConnectionCount
        {
            get
            {
                return m_NodeConnectionList.Count;
            }
        }

        /// <summary>
        /// NodeNormal - свойство для доступа (чтение) к "внешней" нормали к грани, которая соответствует данному узлу графа
        /// </summary>
        public Vector3D NodeNormal
        {
            get
            {
                return m_NodeNormal;
            }
        }

        /// <summary>
        /// ID - свойство для доступа (чтение/запись) к уникальному идентификатору узла
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
        /// метод OrderConnections упорядочивает связи в списке связей узла так, чтобы они были расположены против ч.с.
        /// </summary>
        public void OrderConnections()
        {
            // вектор, связанный с данным узлом, становится ортом ez
            Vector3D ez = this.NodeNormal;
            // перпендик. компонента вектора, связанного с узлом номер 0 из списка узлов, относительно направления ez становится ортом ex
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
                    // скалярное произведение векторов ex и DisorderConnList[DisorderConnIndex].NodeNormal
                    Double ExScalarProductValue = Vector3D.ScalarProduct(ex, DisorderConnList[DisorderConnIndex].NodeNormal);
                    // скалярное произведение векторов ey и DisorderConnList[DisorderConnIndex].NodeNormal
                    Double EyScalarProductValue = Vector3D.ScalarProduct(ey, DisorderConnList[DisorderConnIndex].NodeNormal);
                    // угол между векторами ex и // скалярное произведение векторов ex и DisorderConnList[DisorderConnIndex].NodeNormal против ч.с. (!!!)
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
        /// m_AKoeff - коэффициент A уравнения плоскости
        /// </summary>
        private Double m_AKoeff;
        /// <summary>
        /// m_BKoeff - коэффициент B уравнения плоскости
        /// </summary>
        private Double m_BKoeff;
        /// <summary>
        /// m_CKoeff - коэффициент C уравнения плоскости
        /// </summary>
        private Double m_CKoeff;
        /// <summary>
        /// m_DKoeff - коэффициент D уравнения плоскости
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
        /// AKoeff - свойство для доступа (чтение) к коэффициенту A уравнения плоскости
        /// </summary>
        public Double AKoeff
        {
            get
            {
                return m_AKoeff;
            }
        }

        /// <summary>
        /// BKoeff - свойство для доступа (чтение) к коэффициенту B уравнения плоскости
        /// </summary>
        public Double BKoeff
        {
            get
            {
                return m_BKoeff;
            }
        }

        /// <summary>
        /// CKoeff - свойство для доступа (чтение) к коэффициенту C уравнения плоскости
        /// </summary>
        public Double CKoeff
        {
            get
            {
                return m_CKoeff;
            }
        }

        /// <summary>
        /// DKoeff - свойство для доступа (чтение) к коэффициенту D уравнения плоскости
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
        /// CrossingObjectType - тип объекта пересечения (узел или связь)
        /// </summary>
        public readonly CrossingObjectTypeEnum CrossingObjectType;
        /// <summary>
        /// PGNode1 - узел номер 1 объекта пересечения (положительный узел для связи; для узла сам узел)
        /// </summary>
        public readonly PolyhedronGraphNode PGNode1;
        /// <summary>
        /// PGNode1 - узел номер 2 объекта пересечения (отрицательный узел для связи; для узла значение null)
        /// </summary>
        public readonly PolyhedronGraphNode PGNode2;

        public CrossingObjectClass(CrossingObjectTypeEnum COType, PolyhedronGraphNode PGNode1, PolyhedronGraphNode PGNode2)
        {
            this.CrossingObjectType = COType;

            this.PGNode1 = PGNode1;
            this.PGNode2 = PGNode2;
            /* здесь должны быть проверки на корректность данных в PGNode1 и в PGNode2 в зависимости от типа объекта пересечения */
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
            // если объект пересечения правильно (!!!!!) составлен, то достаточно одной строчки в отличии от написанного выше
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
        /// метод AddConnection добавляет связь к набору "подозрительных" связей П
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
        /// метод GetConnection возвращает узлы, образующие "подозрительную" связь с номером (индексом) ConnectionIndex (в наборе П)
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
        /// метод RemoveConnection удаляет связь с номером (индексом) ConnectionIndex из списка "подозрительных" связей (из набора П)
        /// </summary>
        /// <param name="ConnectionIndex"></param>
        public void RemoveConnection(Int32 ConnectionIndex)
        {
            m_SuspiciousConnectionSet.RemoveAt(ConnectionIndex);
        }

        /// <summary>
        /// метод RemoveConnection удаляет связь между узлами PGNode1 и PGNode2 из списка "подозрительных" связей (из набора П)
        /// </summary>
        /// <param name="PGNode1"></param>
        /// <param name="PGNode2"></param>
        public void RemoveConnection(PolyhedronGraphNode PGNode1, PolyhedronGraphNode PGNode2)
        {
            PolyhedronGraphConnection PGConn = new PolyhedronGraphConnection(PGNode1, PGNode2);
            
            m_SuspiciousConnectionSet.Remove(PGConn);
        }

        /// <summary>
        /// метод RemoveConnections удаляет все связи узла PGNode из списка "подозрительных" связей (из набора П)
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
        /// ConnectionCount - количество "подозрительных" связей (в наборе П)
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
