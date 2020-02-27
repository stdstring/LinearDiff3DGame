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
    internal struct Vector3D
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
    internal class VertexClass
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

        public VertexClass(Point3D Vertex, Int32 VertexID) : this(Vertex.XCoord, Vertex.YCoord, Vertex.ZCoord, VertexID)
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
    internal class SideClass
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
    internal class PolyhedronGraphNode
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
        /// ID - уникальный идентификатор узла (совпадает с ID грани, которая данному узлу соответствует)
        /// </summary>
        public readonly Int32 ID;

        public PolyhedronGraphNode(Int32 ID, Vector3D NodeNormal)
        {
            m_NodeConnectionList = new List<PolyhedronGraphNode>();

            this.ID = ID;
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
        /// m_PolyhedronVertexList - список всех вершин многогранника
        /// </summary>
        private List<VertexClass> m_PolyhedronVertexList;
        /// <summary>
        /// m_PolyhedronSideList - список всех граней многогранника
        /// </summary>
        private List<SideClass> m_PolyhedronSideList;
        /// <summary>
        /// m_PGNodeList - список всех узлов графа многогранника (по сути дела сам граф многогранника)
        /// </summary>
        private List<PolyhedronGraphNode> m_PGNodeList;
        /// <summary>
        /// m_PolyhedronVertexList2 - список всех вершин многогранника, получаемый при восстановлении структуры многогранника из графа
        /// </summary>
        private List<VertexClass> m_PolyhedronVertexList2;
        /// <summary>
        /// m_PolyhedronSideList2 - список всех граней многогранника, получаемый при восстановлении структуры многогранника из графа
        /// </summary>
        private List<SideClass> m_PolyhedronSideList2;
        /// <summary>
        /// m_PieceDirectingVector - направляющий вектор отрезка Pi
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
        /// метод CalcDeterminant3 вычисляет определитель матрицы 3x3
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
        /// метод GetPlanesCrossingPoint возвращает точку пересечения трех плоскостей Plane1, Plane2 и Plane3
        /// </summary>
        /// <param name="Plane1"></param>
        /// <param name="Plane2"></param>
        /// <param name="Plane3"></param>
        /// <returns></returns>
        private Point3D GetPlanesCrossingPoint(PlaneClass Plane1, PlaneClass Plane2, PlaneClass Plane3)
        {
            // пока используем метод Крамера ... потом заменим
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

            // координаты точки пересечения округляем до PointCoordDigits знаков после запятой
            // это нужно чтобы исключить погрешности округления
            Double XCoord = Math.Round((DeltaX / Delta), PointCoordDigits);
            Double YCoord = Math.Round((DeltaY / Delta), PointCoordDigits);
            Double ZCoord = Math.Round((DeltaZ / Delta), PointCoordDigits);

            return new Point3D(XCoord, YCoord, ZCoord);
        }

        /// <summary>
        /// метод PolyhedronSupportFunction возвращает значение опорной функции многогранника для вектора VectorArg
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
        /// метод GetSideCount4Edge возвращает количество граней, которым принадлежит заданное (по 2 вершинам) ребро
        /// </summary>
        /// <param name="EdgeVertex1">первая вершина ребра</param>
        /// <param name="EdgeVertex2">вторая вершина ребра</param>
        /// <returns>количество граней, которым принадлежит заданное ребро</returns>
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
        /// метод GetSideExternalNormal возвращает "внешнюю" (т.е. направленную наружу многогранника) нормаль к грани
        /// грань задается тремя вершинами Vertex1, Vertex2 и Vertex3
        /// </summary>
        /// <param name="Vertex1">вершина грани Vertex1</param>
        /// <param name="Vertex2">вершина грани Vertex2</param>
        /// <param name="Vertex3">вершина грани Vertex3</param>
        /// <returns>"внешняя" нормаль к грани, заданной тремя вершинами Vertex1, Vertex2 и Vertex3</returns>
        private Vector3D GetSideExternalNormal(VertexClass Vertex1, VertexClass Vertex2, VertexClass Vertex3)
        {
            Vector3D ExternalNormal = Vector3D.ZeroVector3D;

            if ((Object.ReferenceEquals(Vertex1, Vertex2)) || (Object.ReferenceEquals(Vertex1, Vertex3)) || (Object.ReferenceEquals(Vertex2, Vertex3)))
            {
                // более логично будет создать свой тип исключения и бросать его
                throw new ArgumentException("Vertex1, Vertex2, Vertex3 must be different");
            }

            // нормаль к плоскости вычисляем через уравнение плоскости, проходящее через 3 точки
            // A = (y2-y1)*(z3-z1)-(z2-z1)*(y3-y1)
            ExternalNormal.XCoord = (Vertex2.YCoord - Vertex1.YCoord) * (Vertex3.ZCoord - Vertex1.ZCoord) -
                                    (Vertex2.ZCoord - Vertex1.ZCoord) * (Vertex3.YCoord - Vertex1.YCoord);
            // B = (z2-z1)*(x3-x1)-(x2-x1)*(z3-z1)
            ExternalNormal.YCoord = (Vertex2.ZCoord - Vertex1.ZCoord) * (Vertex3.XCoord - Vertex1.XCoord) -
                                    (Vertex2.XCoord - Vertex1.XCoord) * (Vertex3.ZCoord - Vertex1.ZCoord);
            // C = (x2-x1)*(y3-y1)-(y2-y1)*(x3-x1)
            ExternalNormal.ZCoord = (Vertex2.XCoord - Vertex1.XCoord) * (Vertex3.YCoord - Vertex1.YCoord) -
                                    (Vertex2.YCoord - Vertex1.YCoord) * (Vertex3.XCoord - Vertex1.XCoord);

            // нормируем полученную нормаль            
            ExternalNormal.Normalize();

            // построим радиус-вектор из любой точки не лежащей на плоскости в любую точку на плоскости
            // т.к. мы работаем с выпуклым многогранником, то скалярного произведение внешней нормали и построенного радиус-вектора должно быть >0 (что очевидно)
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
        /// метод DoesVertexesFormSide проверяет образуют ли грань (выпуклого многогранника) ребро и вершина (три вершины)
        /// при это точки, принадлежащие проверяемой грани заносятся в список вершин этой грани
        /// список вершин грани - SideVertexList; первые три вершины - вершины (ребро и вершина), по которым строится грань
        /// </summary>
        /// <param name="SideVertexList">список вершин грани; первые три элемента - вершины (ребро и вершина), по которым строится грань</param>
        /// <returns>true - если ребро и вершина олбразуют грань, иначе false</returns>
        private Boolean DoesVertexesFormSide(List<VertexClass> SideVertexList)
        {
            Boolean CheckResult = true;

            Vector3D SideVector1 = Vector3D.ZeroVector3D;
            Vector3D SideVector2 = Vector3D.ZeroVector3D;

            // пусть первая точка из списка будет общим началом (точкой O)
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
        /// метод OrderingSideVertexList упорядочивает вершины грани, заданные в списке неупорядоченных вершин SideVertexList
        /// вершины упорядочиваются так, чтобы они следовали против ч.с. если смотреть на грань с конца "внешней" нормали к грани
        /// при этом список неупорядоченных вершин SideVertexList, передаваемый в метод OrderingSideVertexList остается без изменений
        /// </summary>
        /// <param name="SideVertexList">список неупорядоченных вершин</param>
        /// <returns>список упорядоченных вершин</returns>
        private List<VertexClass> OrderingSideVertexList(List<VertexClass> SideVertexList)
        {
            // "внешняя" нормаль к текущей грани (вершины которой мы упорядочиваем)
            Vector3D SideNormalVector = GetSideExternalNormal(SideVertexList[0], SideVertexList[1], SideVertexList[2]);

            // происходит копирование элементов списка. Ограниченное или полное ?
            List<VertexClass> DisorderSideVertexList = new List<VertexClass>(SideVertexList);
            List<VertexClass> OrderSideVertexList = new List<VertexClass>(DisorderSideVertexList.Count);

            OrderSideVertexList.Add(DisorderSideVertexList[0]);
            DisorderSideVertexList.RemoveAt(0);

            while (DisorderSideVertexList.Count > 0)
            {
                VertexClass NextAddedVertex = null;
                Int32 NextAddedVertexIndex = -1;
                VertexClass LastAddedVertex = OrderSideVertexList[OrderSideVertexList.Count - 1];

                // образующий вектор прямой, проходящей через последнюю добавленную и следующую для добавлении вершину
                Vector3D LineFormingVector = Vector3D.ZeroVector3D;
                // "правая" нормаль (лежащая в текущей грани) к прямой, проходящей через последнюю добавленную и следующую для добавлении вершину
                Vector3D LineNormalVector = Vector3D.ZeroVector3D;

                for (Int32 DisorderedVertexIndex = 0; DisorderedVertexIndex < DisorderSideVertexList.Count; DisorderedVertexIndex++)
                {
                    VertexClass DisorderedVertex = DisorderSideVertexList[DisorderedVertexIndex];
                    // скалярное произведение "правой" нормали к прямой и радиус вектора, проведенного из последней добавленной в текущую вершину
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

                        // "правую" нормаль к прямой получаем при помощи векторного произведения образующего вектора прямой на "внешнюю" нормаль к текущей грани (???????????????)
                        LineNormalVector = Vector3D.VectorProduct(LineFormingVector, SideNormalVector);
                    }
                }

                OrderSideVertexList.Add(NextAddedVertex);
                DisorderSideVertexList.RemoveAt(NextAddedVertexIndex);
            }

            return OrderSideVertexList;
        }

        /// <summary>
        /// метод IsSideAlreadyAdded проверяет была ли добавлена проверяемая грань в список граней
        /// при проверке сравниваются нормали граней
        /// </summary>
        /// <param name="SideNormalVector">нормаль проверяемой грани</param>
        /// <returns>true, если грань находится в списке граней, иначе false</returns>
        private Boolean IsSideAlreadyAdded(Vector3D SideNormalVector)
        {
            Boolean CheckResult = false;

            // для проверки сравниваем "внешнюю" нормаль проверяемой грани с "внешними" нормалями уже добавленных в список граней
            // для быстроты, по идее надо бы использовать хэш-таблицу, но для этого необходимо окуглять последние 2-3 знака в координатах нормалей
            // из-за того, что нормали получаются по разным вершинам и из-за погрешностей вычисления нормали одной и той же плоскости могут не совпасть
            // (при поиске в хэш-таблице используется точное равенство по нормалям, а не приближенное как у нас здесь)
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
        /// метод FindVertexOnPoint ищет вершину (по заданной точке) в списке вершин m_PolyhedronVertexList2
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
        /// метод GetShortestGraphPath строит (и возвращает в виде списка узлов) наикратчайший путь обхода (см. алгоритм)
        /// </summary>
        /// <param name="StartPGNode">исходный узел</param>
        /// <param name="StartPGConnNode">связь, определяющая начало пути</param>
        /// <param name="FinishPGConnNode">связь, определяющая конец пути</param>
        /// <returns></returns>
        private List<PolyhedronGraphNode> GetShortestGraphPath(PolyhedronGraphNode StartPGNode, PolyhedronGraphNode StartPGConnNode, PolyhedronGraphNode FinishPGConnNode)
        {
            List<PolyhedronGraphNode> ShortestGraphPath = new List<PolyhedronGraphNode>();

            // рассматриваемый узел
            PolyhedronGraphNode CurrentPGNode = StartPGConnNode;
            // узел, из которого мы пришли в рассматриваемый
            PolyhedronGraphNode PrevPGNode = StartPGNode;

            // пока рассматриваемый узел не совпадет с исходным узлом
            while(CurrentPGNode != StartPGNode)
            {
                ShortestGraphPath.Add(CurrentPGNode);

                // индекс связи (узла) по которой мы пришли в рассматриваемый узел
                Int32 ConnFromIndex = CurrentPGNode.GetConnectionIndex(PrevPGNode);
                // индекс связи (узла) предыдущей для той, по которой мы пришли в рассматриваемый узел
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
        /// метод GetFirstSide находи и возвращает первую грань многоранника (см. алгоритм)
        /// </summary>
        /// <returns>первая грань многогранника</returns>
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
                // ребро: Vertex1 - Vertex2
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
                        // упорядоченный список вершин грани
                        List<VertexClass> OrderedSideVertexList = OrderingSideVertexList(CheckedSideVertexList);
                        // "внешняя" нормаль к грани
                        Vector3D FirstSideNormalVector = GetSideExternalNormal(Vertex1, Vertex2, Vertex3);
                        FirstSide = new SideClass(OrderedSideVertexList, 1, FirstSideNormalVector);

                        // не очень красивый выход из метода !!!!!!!!!
                        return FirstSide;
                    }
                }
            }

            return FirstSide;
        }

        /// <summary>
        /// метод RecievePolyhedronStructure получает (восстанавливает) структуру многогранника
        /// </summary>
        private void RecievePolyhedronStructure()
        {
            SideClass FirstSide = GetFirstSide();
            m_PolyhedronSideList.Add(FirstSide);

            List<VertexClass> CheckedSideVertexList = new List<VertexClass>();

            // цикл по всем граням из списка граней
            for (Int32 SideIndex = 0; SideIndex < m_PolyhedronSideList.Count; SideIndex++)
            {
                SideClass CurrentSide = m_PolyhedronSideList[SideIndex];

                // цикл по всем ребрам текущей грани
                for (Int32 CurrentSideVertexIndex = 0; CurrentSideVertexIndex < CurrentSide.VertexCount; CurrentSideVertexIndex++)
                {
                    VertexClass LeftEdgeVertex = CurrentSide[CurrentSideVertexIndex];
                    VertexClass RightEdgeVertex = (CurrentSideVertexIndex == (CurrentSide.VertexCount - 1) ? CurrentSide[0] : CurrentSide[CurrentSideVertexIndex + 1]);
                    
                    // CurrentEdge: LeftEdgeVertex-RightEdgeVertex
                    Int32 SideCount4CurrentEdge = GetSideCount4Edge(LeftEdgeVertex, RightEdgeVertex);
                    // if ((SideCount4CurrentEdge != 1) && (SideCount4CurrentEdge != 2)) !!!!!
                    if (SideCount4CurrentEdge == 2) continue;

                    // цикл по всем вершинам
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
                            // упорядоченный список вершин грани
                            List<VertexClass> OrderedSideVertexList = OrderingSideVertexList(CheckedSideVertexList);
                            // "внешняя" нормаль к грани
                            Vector3D SideNormalVector = GetSideExternalNormal(LeftEdgeVertex, RightEdgeVertex, CurrentVertex);
                            // ID грани
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
        /// метод RecievePolyhedronGraph создает (получает) граф многогранника
        /// </summary>
        private void RecievePolyhedronGraph()
        {
            foreach (SideClass CurrentSide in m_PolyhedronSideList)
            {
                m_PGNodeList.Add(new PolyhedronGraphNode(CurrentSide.ID, CurrentSide.SideNormal));
            }

            // цикл по всем граням из списка граней
            foreach (SideClass CurrentSide in m_PolyhedronSideList)
            {
                // цикл по всем ребрам текущей грани
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
        /// метод GraphTriangulation выполняет процедуру триангуляции графа (см. алгоритм)
        /// </summary>
        private void GraphTriangulation()
        {
            // цикл по всем узлам графа из списка узлов графа
            foreach (PolyhedronGraphNode PGNode in m_PGNodeList)
            {
                // цикл по всем связям текущего узла
                for (Int32 NodeConnIndex = 0; NodeConnIndex < PGNode.NodeConnectionCount; /*NodeConnIndex++*/)
                {
                    // текущая и следующая связи (узлы)
                    PolyhedronGraphNode CurrentConn = PGNode[NodeConnIndex];
                    PolyhedronGraphNode NextConn = (NodeConnIndex == PGNode.NodeConnectionCount - 1 ? PGNode[0] : PGNode[NodeConnIndex + 1]);

                    // строим наикратчайший путь обхода, начинающийся на текущей связи и заканчивающийся на следующей связи
                    List<PolyhedronGraphNode> ShortestGraphPath = GetShortestGraphPath(PGNode, CurrentConn, NextConn);
                    
                    // если число узлов в построенном пути < 2, то это ошибка !!!!!!
                    if (ShortestGraphPath.Count < 2)
                    {
                        throw new AlgorithmException();
                    }

                    // цикл со 2-го по N-1 узел из построенного пути обхода
                    // если число узлов в построенном пути = 2, то цикл выполнится 0 раз
                    for (Int32 GraphPathIndex = 1; GraphPathIndex < ShortestGraphPath.Count - 1; GraphPathIndex++)
                    {
                        // в список связей текущего узла добавляем связь (между текущим узлом и i-м узлом из построенного пути обхода) за последней добавленной связью или текущей, если добавлений не было
                        PGNode.InsertNodeConnection(NodeConnIndex + 1, ShortestGraphPath[GraphPathIndex]);

                        // в список связей i-го узла из построенного пути обхода добавляем (эту же) связь перед связью, по которой мы пришли в этот узел
                        Int32 CurrentPathNodeConnFromIndex = ShortestGraphPath[GraphPathIndex].GetConnectionIndex(ShortestGraphPath[GraphPathIndex - 1]);
                        ShortestGraphPath[GraphPathIndex].InsertNodeConnection(CurrentPathNodeConnFromIndex, PGNode);

                        NodeConnIndex++;
                    }
                    
                    NodeConnIndex++;
                }
            }
        }

        /// <summary>
        /// метод RecievePolyhedronStructureFromGraph получает (восстанавливает) структуру многогранника из его графа
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
                // CurrentPlane соответствует CurrentPGNode
                // плоскость номер 1
                PlaneClass CurrentPlane = PolyhedronPlaneList[PGNodeIndex];
                SideClass CurrentSide = new SideClass(new List<VertexClass>(), CurrentPGNode.ID, CurrentPGNode.NodeNormal);
                m_PolyhedronSideList2.Add(CurrentSide);

                for (Int32 NodeConnIndex = 0; NodeConnIndex < CurrentPGNode.NodeConnectionCount; NodeConnIndex++)
                {
                    // текущая и соседняя связи 
                    PolyhedronGraphNode CurrentConnPGNode = CurrentPGNode[NodeConnIndex];
                    PolyhedronGraphNode NextConnPGNode = (NodeConnIndex == (CurrentPGNode.NodeConnectionCount - 1) ? CurrentPGNode[0] : CurrentPGNode[NodeConnIndex + 1]);

                    // плоскости, соответствующие текущей и соседней связям (плоскости номер 2 и 3)
                    PlaneClass CurrentConnPlane = PolyhedronPlaneList[CurrentConnPGNode.ID - 1];
                    PlaneClass NextConnPlane = PolyhedronPlaneList[NextConnPGNode.ID - 1];

                    // точка пересечения плоскостей 1, 2 и 3
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
        /// метод GetFirstCrossingObject возвращает первый объект пересечения с G(...Pi...)
        /// </summary>
        /// <param name="StartingPGNode"></param>
        /// <returns></returns>
        private CrossingObjectClass GetFirstCrossingObject(PolyhedronGraphNode StartingPGNode)
        {
            CrossingObjectClass FirstCrossingObject = null;

            // текущий узел
            PolyhedronGraphNode CurrentPGNode = StartingPGNode;
            // вычисляем скалярное произведение вектора, связанного с текущим узлом, и направляющего вектора Pi
            Double CurrentScalarProductValue = Vector3D.ScalarProduct(CurrentPGNode.NodeNormal, m_PiDirectingVector);
            // если скалярное произведение = 0, то текущий узел становится искомым объектом
            if (Math.Abs(CurrentScalarProductValue) < Epsilon)
            {
                FirstCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, CurrentPGNode, null);
            }

            // цикл пока не найден искомый первый объект пересечения
            while ((Object)FirstCrossingObject == null)
            {
                Double BestScalarProductValue = Double.NaN;
                PolyhedronGraphNode BestPGNode = null;

                // цикл по всем связям текущего узла
                for (Int32 NodeConnIndex = 0; NodeConnIndex < CurrentPGNode.NodeConnectionCount; NodeConnIndex++)
                {
                    // текущая связь текущего узла
                    PolyhedronGraphNode CurrentConnPGNode = CurrentPGNode[NodeConnIndex];
                    // считаем скалярное произведение вектора, связанного с полученным выше узлом, и направляющего вектора Pi
                    Double CurrentConnNodeScalarProductValue = Vector3D.ScalarProduct(CurrentConnPGNode.NodeNormal, m_PiDirectingVector);

                    // если скалярное произведение = 0, то полученный узел становится искомым объектом
                    if (Math.Abs(CurrentConnNodeScalarProductValue) < Epsilon)
                    {
                        FirstCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, CurrentConnPGNode, null);
                        break;
                    }

                    // если скалярное произведение того же знака, что и для вектора, связанного с текущим узлом, то
                    // если по абсолютному значению величина скалярного произведения меньше запомненного, то запоминаем величину и полученный узел
                    if (Math.Sign(CurrentScalarProductValue) == Math.Sign(CurrentConnNodeScalarProductValue))
                    {
                        if ((Double.IsNaN(BestScalarProductValue)) ||
                            (Math.Abs(CurrentConnNodeScalarProductValue) < Math.Abs(BestScalarProductValue)))
                        {
                            BestScalarProductValue = CurrentConnNodeScalarProductValue;
                            BestPGNode = CurrentConnPGNode;
                        }
                    }
                    // если знак скалярного произведения для полученного (выше) вектора, отличается от знака скалярного произведения для вектора, связанного с текущим узлом, то
                    // связь, соединяющая текущий и полученный (выше) узлы становится искомым объектом
                    else
                    {
                        PolyhedronGraphNode PlusPGNode = (CurrentScalarProductValue > 0 ? CurrentPGNode : CurrentConnPGNode);
                        PolyhedronGraphNode MinusPGNode = (CurrentScalarProductValue < 0 ? CurrentPGNode : CurrentConnPGNode);

                        FirstCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphConnection, PlusPGNode, MinusPGNode);
                        break;
                    }
                }

                // текущим узлом становится запомненный узел
                CurrentPGNode = BestPGNode;
                CurrentScalarProductValue = BestScalarProductValue;
            }

            return FirstCrossingObject;
        }

        /// <summary>
        /// метод GetFirstCrossingObject возвращает первый объект пересечения с G(...Pi...)
        /// </summary>
        /// <returns></returns>
        private CrossingObjectClass GetFirstCrossingObject()
        {
            return GetFirstCrossingObject(m_PGNodeList[0]);
        }

        /// <summary>
        /// метод BuildCrossingNode возвращает узел пересечении текущего объекта и G(...Pi...)
        /// если текущий объект - узел, то он же и возвращается
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
                // вектор, перпендикулярный к PlusValueVector и MinusValueVector
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
        /// метод CheckMoveDirection возвращает true, если направление движения по G(...Pi...) правильное, иначе возвращается false
        /// правильным считается направление движения против часовой стрелки, если смотреть с конца направляющего вектора Pi
        /// </summary>
        /// <param name="CheckCrossingObject"></param>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private Boolean CheckMoveDirection(CrossingObjectClass CheckCrossingObject, CrossingObjectClass CurrentCrossingObject)
        {
            // направляющий вектор Pi становится ортом оси OZ
            Vector3D OZDirectingVector = m_PiDirectingVector;

            // строим узел на пересечении текущего объекта и G(...Pi...); вектор, связанный с этим узлом, становится ортом оси OX
            PolyhedronGraphNode CurrentCrossingNode = BuildCrossingNode(CurrentCrossingObject);
            Vector3D OXDirectingVector = CurrentCrossingNode.NodeNormal;

            // строим орт оси OY правой СК XYZ (как векторное произведение орта оси OZ на орт оси OX)
            Vector3D OYDirectingVector = Vector3D.VectorProduct(OZDirectingVector, OXDirectingVector);

            // строим узел на пересечении проверяемого объекта и G(...Pi...); вычисляем скалярное произведение вектора, связанного с этим узлом, и орта оси OY
            PolyhedronGraphNode CheckCrossingNode = BuildCrossingNode(CheckCrossingObject);
            Vector3D CheckVector = CheckCrossingNode.NodeNormal;
            Double ScalarProductValue = Vector3D.ScalarProduct(CheckVector, OYDirectingVector);

            // если ScalarProductValue = 0 - это ошибка работы алгоритма
            if (Math.Abs(ScalarProductValue) < Epsilon)
            {
                throw new AlgorithmException();
            }

            // если вычисленное скалярное произведение > 0, то направление движения правильное, иначе направление движения неправильное
            return (ScalarProductValue > 0 ? true : false);
        }

        /// <summary>
        /// метод GetNextCrossingObject4GraphNode возвращает следующий по направлению движения объект пересечения, если текущий - узел
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private CrossingObjectClass GetNextCrossingObject4GraphNode(CrossingObjectClass CurrentCrossingObject)
        {
            CrossingObjectClass NextCrossingObject = null;

            PolyhedronGraphNode CurrentPGNode = CurrentCrossingObject.PGNode1;
            // цикл по всем связям текущего узла
            for (Int32 NodeConnIndex = 0; NodeConnIndex < CurrentPGNode.NodeConnectionCount; NodeConnIndex++)
            {
                // получаем узел (номер 1), связанный с текущим узлом текущей связью
                PolyhedronGraphNode PGNode1 = CurrentPGNode[NodeConnIndex];
                // получаем узел (номер 2), связанный с текущим узлом предыдущей связью
                PolyhedronGraphNode PGNode2 = (NodeConnIndex == 0 ? CurrentPGNode[CurrentPGNode.NodeConnectionCount - 1] : CurrentPGNode[NodeConnIndex - 1]);

                // вычисляем скалярное произведение вектора 1 и направляющего вектора Pi
                Double PGNode1ScalarProductValue = Vector3D.ScalarProduct(PGNode1.NodeNormal, m_PiDirectingVector);
                // вычисляем скалярное произведение вектора 2 и направляющего вектора Pi
                Double PGNode2ScalarProductValue = Vector3D.ScalarProduct(PGNode2.NodeNormal, m_PiDirectingVector);

                // если скалярное произведение узла 1 и направляющего вектора Pi == 0
                if (Math.Abs(PGNode1ScalarProductValue) < Epsilon)
                {
                    // если направление движения выбрано правильно, то узел номер 1 становится следующим по движению объектом
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

                // если скалярное произведение узла 2 и направляющего вектора Pi == 0
                if (Math.Abs(PGNode2ScalarProductValue) < Epsilon)
                {
                    // если направление движения выбрано правильно, то узел номер 2 становится следующим по движению объектом
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

                // если скалярные произведения узлов 1 и 2 и направляющего вектора Pi имеют разный знак
                if (Math.Sign(PGNode1ScalarProductValue) != Math.Sign(PGNode2ScalarProductValue))
                {
                    // если направление движения выбрано правильно, то связь, соединяющая узлы 1 и 2, становится следующим по движению объектом
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
        /// метод GetNextCrossingObject4GraphConn возвращает следующий по направлению движения объект пересечения, если текущий - связь
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <param name="CurrentCrossingNode"></param>
        /// <returns></returns>
        private CrossingObjectClass GetNextCrossingObject4GraphConn(CrossingObjectClass CurrentCrossingObject, PolyhedronGraphNode CurrentCrossingNode)
        {
            CrossingObjectClass NextCrossingObject = null;

            // положительный узел текущей связи
            PolyhedronGraphNode CurrentPGNode1 = CurrentCrossingObject.PGNode1;
            // отрицательный узел текущей связи
            PolyhedronGraphNode CurrentPGNode2 = CurrentCrossingObject.PGNode2;

            /*Int32 CurrentConnIndex1 = CurrentPGNode1.GetConnectionIndex(CurrentPGNode2);
            Int32 CurrentConnIndex2 = CurrentPGNode2.GetConnectionIndex(CurrentPGNode1);*/
            Int32 CurrentConnIndex1 = CurrentPGNode1.GetConnectionIndex(CurrentCrossingNode);
            Int32 CurrentConnIndex2 = CurrentPGNode2.GetConnectionIndex(CurrentCrossingNode);

            // для положительного узла (CurrentCrossingObject.PGNode1) берем следующую связь (относительно текущей)
            PolyhedronGraphNode NextPGNode1 = (CurrentConnIndex1 == (CurrentPGNode1.NodeConnectionCount - 1) ? CurrentPGNode1[0] : CurrentPGNode1[CurrentConnIndex1 + 1]);
            // для отрицательного узла (CurrentCrossingObject.PGNode2) берем предыдущую связь (относительно текущей)
            PolyhedronGraphNode NextPGNode2 = (CurrentConnIndex2 == 0 ? CurrentPGNode2[CurrentPGNode2.NodeConnectionCount - 1] : CurrentPGNode2[CurrentConnIndex2 - 1]);

            Double NextPGNode1ScalarProductValue = Vector3D.ScalarProduct(NextPGNode1.NodeNormal, m_PiDirectingVector);
            Double NextPGNode2ScalarProductValue = Vector3D.ScalarProduct(NextPGNode2.NodeNormal, m_PiDirectingVector);

            // если полученный узел (номер 1) нулевой
            if (Math.Abs(NextPGNode1ScalarProductValue) < Epsilon)
            {
                // если полученный узел номер 2 нулевой
                if (Math.Abs(NextPGNode2ScalarProductValue) < Epsilon)
                {
                    // полученный узел становится следующим по движению объектом
                    NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, NextPGNode1, null);
                    // exit
                }
                // если полученный узел номер 2 ненулевой
                else
                {
                    // "связь" (это связь, с которой мы начинали движение; реально ее уже нет) соединяющая положительный узел и узел номер 2 становится следующим по движению объектом
                    NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphConnection, CurrentPGNode1, NextPGNode2);
                    // exit
                }
            }
            // если полученный узел (номер 1) положительный
            else if (NextPGNode1ScalarProductValue > 0)
            {
                // связь, соединяющая новый положительный узел и старый отрицательный узел, становится следующим по движению объектом
                NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphConnection, NextPGNode1, CurrentPGNode2);
                // exit
            }
            // если полученный узел (номер 1) отрицательный
            else if (NextPGNode1ScalarProductValue < 0)
            {
                // связь, соединяющая новый отрицательный узел и старый положительный узел, становится следующим по движению объектом
                NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphConnection, CurrentPGNode1, NextPGNode2);
                // exit
            }

            return NextCrossingObject;
        }

        /// <summary>
        /// метод GetNextCrossingObject возвращает следующий по направлению движения объект пересечения
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <param name="CurrentCrossingNode"></param>
        /// <returns></returns>
        private CrossingObjectClass GetNextCrossingObject(CrossingObjectClass CurrentCrossingObject, PolyhedronGraphNode CurrentCrossingNode)
        {
            // если текущий объект – узел
            if (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode)
            {
                return GetNextCrossingObject4GraphNode(CurrentCrossingObject);
            }
            // если текущий объект – связь
            else
            {
                return GetNextCrossingObject4GraphConn(CurrentCrossingObject, CurrentCrossingNode);
            }
        }

        /// <summary>
        /// метод AddCrossingNodeBetweenConn добавляет узел CrossingNode на пересечении связи и G(...Pi...) и соответствующим образом правит/добавляет ссылки
        /// </summary>
        /// <param name="ConnPlusNode"></param>
        /// <param name="ConnMinusNode"></param>
        /// <param name="CrossingNode"></param>
        private void AddCrossingNodeBetweenConn(PolyhedronGraphNode ConnPlusNode, PolyhedronGraphNode ConnMinusNode, PolyhedronGraphNode CrossingNode)
        {
            // добавляем новый узел в список узлов графа
            m_PGNodeList.Add(CrossingNode);
            // добавляем в список ссылок нового узла ссылки сначала на положительный узел связи, потом на отрицательный
            CrossingNode.AddNodeConnection(ConnPlusNode);
            CrossingNode.AddNodeConnection(ConnMinusNode);
            // для узлов, образующих связь, меняем их ссылки друг на друга (которые и образуют связь) на ссылку на новый узел
            Int32 PlusNodeCurrentConnIndex = ConnPlusNode.GetConnectionIndex(ConnMinusNode);
            Int32 MinusNodeCurrentConnIndex = ConnMinusNode.GetConnectionIndex(ConnPlusNode);
            ConnPlusNode[PlusNodeCurrentConnIndex] = CrossingNode;
            ConnMinusNode[MinusNodeCurrentConnIndex] = CrossingNode;
        }

        /// <summary>
        /// метод BuildGFiGrid строит сетку G(...Fi...) (см. алгоритм)
        /// </summary>
        private void BuildGFiGrid()
        {
            // первый (запомненный) объект пересечения
            CrossingObjectClass FirstCrossingObject = GetFirstCrossingObject();
            // предыдущий объект пересечения
            // CrossingObjectClass PreviousCrossingObject = null;
            // текущий объект пересечения
            CrossingObjectClass CurrentCrossingObject = FirstCrossingObject;
            // строим узел на пересечении текущего объекта и G(...Pi...) и запоминаем его
            // если этот узел отсутствует в списке узлов, то добавляем его и соответствующие ссылки на данный узел
            PolyhedronGraphNode FirstCrossingNode = BuildCrossingNode(CurrentCrossingObject);
            // текущий узел пересечения
            PolyhedronGraphNode CurrentCrossingNode = FirstCrossingNode;
            // предыдущий узел пересечения
            // PolyhedronGraphNode PreviousCrossingNode = null;
            if (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection)
            {
                AddCrossingNodeBetweenConn(CurrentCrossingObject.PGNode1, CurrentCrossingObject.PGNode2, CurrentCrossingNode);
            }

            do
            {
                CrossingObjectClass PreviousCrossingObject = CurrentCrossingObject;
                PolyhedronGraphNode PreviousCrossingNode = CurrentCrossingNode;
                // получаем следующий по движению объект (связь, либо узел) и делаем его текущим
                CurrentCrossingObject = GetNextCrossingObject(CurrentCrossingObject, CurrentCrossingNode);
                // строим узел на пересечении текущего объекта и G(...Pi...)
                // если этот узел отсутствует в списке узлов (этот узел будет присутствовать в списке узлов, если текущий объект – узел, либо если начальным объектом была связь и мы в нее пришли), то добавляем его и соответствующие ссылки на данный узел
                // отдельно обрабатываем случай если мы пришли в первый (запомненный) объект пересечения (для простоты реализации алгоритма)
                CurrentCrossingNode = (CurrentCrossingObject == FirstCrossingObject ? FirstCrossingNode : BuildCrossingNode(CurrentCrossingObject));
                if ((CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection) &&
                    (CurrentCrossingObject != FirstCrossingObject))
                {
                    AddCrossingNodeBetweenConn(CurrentCrossingObject.PGNode1, CurrentCrossingObject.PGNode2, CurrentCrossingNode);
                }

                // если предыдущий и текущий объекты – узлы
                if ((PreviousCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode) &&
                    (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode))
                {
                    // переход к следующей итерации цикла
                    // continue;
                }

                // если предыдущий объект узел, а текущий связь
                if ((PreviousCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode) &&
                    (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection))
                {
                    // строим связи между предыдущим узлом и узлом пересечения на текущем объекте (связи добавляются с учетом упорядоченности)
                    // отрицательный узел текущей связи
                    PolyhedronGraphNode CurrentConnMinusNode = CurrentCrossingObject.PGNode2;
                    // ссылку на текущий узел пересечения в список ссылок предыдущего узла вставляем после ссылки на отрицательный узел текущей связи
                    Int32 PrevNode2CurrentMinusNodeConnIndex = PreviousCrossingNode.GetConnectionIndex(CurrentConnMinusNode);
                    PreviousCrossingNode.InsertNodeConnection(PrevNode2CurrentMinusNodeConnIndex + 1, CurrentCrossingNode);
                    // ссылку на предыдущий узел вставляем после ссылки на положительный узел текущей связи (на позицию номер 1)
                    CurrentCrossingNode.InsertNodeConnection(1, PreviousCrossingNode);
                }

                // если предыдущий объект связь, а текущий узел
                if ((PreviousCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection) &&
                    (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode))
                {
                    // строим связи между узлом пересечения на предыдущем объекте и текущем узле (связи добавляются с учетом упорядоченности)
                    // положительный узел предыдущей связи
                    PolyhedronGraphNode PreviousConnPlusNode = PreviousCrossingObject.PGNode1;
                    // ссылку на предыдущий узел пересечения в список ссылок текущего узла вставляем после ссылки на положительный узел предыдущей связи
                    Int32 CurrentNode2PrevPlusNodeConnIndex = CurrentCrossingNode.GetConnectionIndex(PreviousConnPlusNode);
                    CurrentCrossingNode.InsertNodeConnection(CurrentNode2PrevPlusNodeConnIndex + 1, PreviousCrossingNode);
                    // ссылку на текущий узел добавляем в конец списка ссылок предыдущего узла
                    PreviousCrossingNode.AddNodeConnection(CurrentCrossingNode);
                }

                // если предыдущий и текущий объекты - связи
                if ((PreviousCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection) &&
                    (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection))
                {
                    // строим связи между узлом пересечения на предыдущем объекте и узлом пересечения на текущем объекте (связи добавляются с учетом упорядоченности)
                    // строим связь между узлом пересечения на предыдущем объекте и узлом текущей связи, который не принадлежит предыдущей связи (связи добавляются с учетом упорядоченности)
                    // у связай общий отрицательный узел (случай 3а)
                    if (PreviousCrossingObject.PGNode2 == CurrentCrossingObject.PGNode2)
                    {
                        // положительный узел предыдущей связи (узел номер 1)
                        PolyhedronGraphNode PreviousConnPlusNode = PreviousCrossingObject.PGNode1;
                        // общий отрицательный узел (узел номер 2)
                        // PolyhedronGraphNode CommonConnMinusNode = PreviousCrossingObject.PGNode2;
                        // положительный узел текущей связи (узел номер 3)
                        PolyhedronGraphNode CurrentConnPlusNode = CurrentCrossingObject.PGNode1;
                        // для узла номер 3: ссылка на предыдущей узел пересечения вставляется после ссылки на узел 1
                        Int32 CurrentPlusNode2PrevPlusNodeConnIndex = CurrentConnPlusNode.GetConnectionIndex(PreviousConnPlusNode);
                        CurrentConnPlusNode.InsertNodeConnection(CurrentPlusNode2PrevPlusNodeConnIndex + 1, PreviousCrossingNode);
                        // для предыдущего узла пересечения: в конец списка ссылок добавляется сначала ссылка на новый узел пересечения, потом ссылка на узел номер 3
                        PreviousCrossingNode.AddNodeConnection(CurrentCrossingNode);
                        PreviousCrossingNode.AddNodeConnection(CurrentConnPlusNode);
                        // для текущего узла пересечения: ссылка на предыдущий узел пересечения вставляется после ссылки на узел номер 3 (т.е. на позицию номер 1)
                        CurrentCrossingNode.InsertNodeConnection(1, PreviousCrossingNode);
                    }
                    // у связай общий положительный узел (случай 3б)
                    else if (PreviousCrossingObject.PGNode1 == CurrentCrossingObject.PGNode1)
                    {
                        // отрицательный узел предыдущей связи (узел номер 1)
                        PolyhedronGraphNode PreviousConnMinusNode = PreviousCrossingObject.PGNode2;
                        // общий положительный узел (узел номер 2)
                        // PolyhedronGraphNode CommonConnPlusNode = PreviousCrossingObject.PGNode1;
                        // отрицательный узел текущей связи (узел номер 3)
                        PolyhedronGraphNode CurrentConnMinusNode = CurrentCrossingObject.PGNode2;
                        // для узла номер 3: ссылка на предыдущей узел пересечения вставляется после ссылки на текущий узел пересечения
                        Int32 CurrentMinusNode2CurrentCrossingNodeConnIndex = CurrentConnMinusNode.GetConnectionIndex(CurrentCrossingNode);
                        CurrentConnMinusNode.InsertNodeConnection(CurrentMinusNode2CurrentCrossingNodeConnIndex + 1, PreviousCrossingNode);
                        // для предыдущего узла пересечения: в конец списка ссылок добавляется сначала ссылка на узел номер 3, потом ссылка на новый узел пересечения
                        PreviousCrossingNode.AddNodeConnection(CurrentConnMinusNode);
                        PreviousCrossingNode.AddNodeConnection(CurrentCrossingNode);
                        // для текущего узла пересечения: ссылка на предыдущий узел пересечения вставляется после ссылки на узел номер 1 (т.е. на позицию номер 1)
                        CurrentCrossingNode.InsertNodeConnection(1, PreviousCrossingNode);
                    }
                    // ошибка работы алгоритма
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
        /// метод GetPolyhedronStructureDescription возвращает в текстовом виде (в виде массива строк) описание структуры многогранника
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
        /// метод GetPolyhedronGraphDescription возвращает в текстовом виде (в виде массива строк) описание графа многогранника
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
        /// метод GetPolyhedronGraphDescription возвращает в текстовом виде (в виде массива строк) описание графа многогранника с упорядоченными против ч.с. связями
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
        /// метод GetPolyhedronStructureDescription2 возвращает в текстовом виде (в виде массива строк) описание структуры многогранника, получаемую при ее восстановлении из графа
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
        /// метод GetPolyhedronGraphDescription возвращает в текстовом виде (в виде массива строк) описание графа GFi
        /// </summary>
        /// <returns></returns>
        public String[] GetGFiGraphDescription()
        {
            BuildGFiGrid();
            return GetPolyhedronGraphDescription();
        }
    }
}
