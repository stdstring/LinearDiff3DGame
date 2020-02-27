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
        /// m_Pi1DirectingVector - направляющий вектор отрезка Pi1
        /// </summary>
        private Vector3D m_Pi1DirectingVector;
        /// <summary>
        /// m_Qi1DirectingVector - направляющий вектор отрезка Qi1
        /// </summary>
        private Vector3D m_Qi1DirectingVector;
        /// <summary>
        /// m_Pi1Set - множество (отрезок) Pi1
        /// </summary>
        private List<Point3D> m_Pi1Set;
        /// <summary>
        /// m_Qi1Set - множество (отрезок) Qi1
        /// </summary>
        private List<Point3D> m_Qi1Set;
        /// <summary>
        /// m_Pi2DirectingVector - направляющий вектор отрезка Pi2
        /// </summary>
        private Vector3D m_Pi2DirectingVector;
        /// <summary>
        /// m_Qi2DirectingVector - направляющий вектор отрезка Qi2
        /// </summary>
        private Vector3D m_Qi2DirectingVector;
        /// <summary>
        /// m_Pi2Set - множество (отрезок) Pi2
        /// </summary>
        private List<Point3D> m_Pi2Set;
        /// <summary>
        /// m_Qi2Set - множество (отрезок) Qi2
        /// </summary>
        private List<Point3D> m_Qi2Set;
        /// <summary>
        /// m_CurrentInverseTime - текущее обратное время
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
        /// MinVectorDistinguishAngle - минимальное значение угла между векторами, при котором мы считаем два вектора различными (не близкими)
        /// </summary>
        private readonly Double MinVectorDistinguishAngle = 0.01;
        /// <summary>
        /// m_FundKoshiMatrix - значение фундаментальной матрицы Коши в момент обратного времени m_CurrentInverseTime;
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
                            // "внешняя" нормаль к грани
                            Vector3D SideNormalVector = GetSideExternalNormal(LeftEdgeVertex, RightEdgeVertex, CurrentVertex);
                            if (IsSideAlreadyAdded(SideNormalVector)) continue;

                            // упорядоченный список вершин грани
                            List<VertexClass> OrderedSideVertexList = OrderingSideVertexList(CheckedSideVertexList);
                            // ID грани
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
            while (CurrentPGNode != StartPGNode)
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
                throw new AlgorithmException("Error at construction of the graph's path !!!");
            }

            return ShortestGraphPath;
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
                        throw new AlgorithmException("Error at construction of the graph's path !!!");
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
        /// метод GetFirstCrossingObject возвращает первый объект пересечения с G(...Pi...)
        /// </summary>
        /// <param name="StartingPGNode"></param>
        /// <returns></returns>
        private CrossingObjectClass GetFirstCrossingObject(PolyhedronGraphNode StartingPGNode, Vector3D PiDirectingVector)
        {
            CrossingObjectClass FirstCrossingObject = null;

            // текущий узел
            PolyhedronGraphNode CurrentPGNode = StartingPGNode;
            // вычисляем скалярное произведение вектора, связанного с текущим узлом, и направляющего вектора отрезка Pi
            Double CurrentScalarProductValue = Vector3D.ScalarProduct(CurrentPGNode.NodeNormal, PiDirectingVector);
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
                    // считаем скалярное произведение вектора, связанного с полученным выше узлом, и направляющего вектора отрезка Pi
                    Double CurrentConnNodeScalarProductValue = Vector3D.ScalarProduct(CurrentConnPGNode.NodeNormal, PiDirectingVector);

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
        /// метод GetFirstCrossingObject возвращает первый объект пересечения с G(...Pi...)
        /// </summary>
        /// <returns></returns>
        private CrossingObjectClass GetFirstCrossingObject(Vector3D PiDirectingVector)
        {
            return GetFirstCrossingObject(m_PGNodeList[0], PiDirectingVector);
        }
        
        /// <summary>
        /// метод BuildCrossingNode возвращает узел пересечении текущего объекта и G(...Pi...)
        /// если текущий объект - узел, то он же и возвращается
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
                // вектор, перпендикулярный к PlusValueVector и MinusValueVector
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
        /// метод CheckMoveDirection возвращает true, если направление движения по G(...Pi...) правильное, иначе возвращается false
        /// правильным считается направление движения против часовой стрелки, если смотреть с конца направляющего вектора Pi
        /// </summary>
        /// <param name="CheckCrossingObject"></param>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private Boolean CheckMoveDirection(CrossingObjectClass CheckCrossingObject, CrossingObjectClass CurrentCrossingObject, Vector3D PiDirectingVector)
        {
            // направляющий вектор Pi становится ортом оси OZ
            Vector3D OZDirectingVector = PiDirectingVector;

            // строим узел на пересечении текущего объекта и G(...Pi...); вектор, связанный с этим узлом, становится ортом оси OX
            PolyhedronGraphNode CurrentCrossingNode = BuildCrossingNode(CurrentCrossingObject, PiDirectingVector);
            Vector3D OXDirectingVector = CurrentCrossingNode.NodeNormal;

            // строим орт оси OY правой СК XYZ (как векторное произведение орта оси OZ на орт оси OX)
            Vector3D OYDirectingVector = Vector3D.VectorProduct(OZDirectingVector, OXDirectingVector);

            // строим узел на пересечении проверяемого объекта и G(...Pi...); вычисляем скалярное произведение вектора, связанного с этим узлом, и орта оси OY
            PolyhedronGraphNode CheckCrossingNode = BuildCrossingNode(CheckCrossingObject, PiDirectingVector);
            Vector3D CheckVector = CheckCrossingNode.NodeNormal;
            Double ScalarProductValue = Vector3D.ScalarProduct(CheckVector, OYDirectingVector);

            // если ScalarProductValue = 0 - это ошибка работы алгоритма
            if (Math.Abs(ScalarProductValue) < Epsilon)
            {
                throw new AlgorithmException("CheckMoveDirection method incorrect work");
            }

            // если вычисленное скалярное произведение > 0, то направление движения правильное, иначе направление движения неправильное
            return (ScalarProductValue > 0 ? true : false);
        }

        /// <summary>
        /// метод GetNextCrossingObject4GraphNode возвращает следующий по направлению движения объект пересечения, если текущий - узел
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private CrossingObjectClass GetNextCrossingObject4GraphNode(CrossingObjectClass CurrentCrossingObject, Vector3D PiDirectingVector)
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
                Double PGNode1ScalarProductValue = Vector3D.ScalarProduct(PGNode1.NodeNormal, PiDirectingVector);
                // вычисляем скалярное произведение вектора 2 и направляющего вектора Pi
                Double PGNode2ScalarProductValue = Vector3D.ScalarProduct(PGNode2.NodeNormal, PiDirectingVector);

                // если скалярное произведение узла 1 и направляющего вектора Pi == 0
                if (Math.Abs(PGNode1ScalarProductValue) < Epsilon)
                {
                    // если направление движения выбрано правильно, то узел номер 1 становится следующим по движению объектом
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

                // если скалярное произведение узла 2 и направляющего вектора Pi == 0
                if (Math.Abs(PGNode2ScalarProductValue) < Epsilon)
                {
                    // если направление движения выбрано правильно, то узел номер 2 становится следующим по движению объектом
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

                // если скалярные произведения узлов 1 и 2 и направляющего вектора Pi имеют разный знак
                if (Math.Sign(PGNode1ScalarProductValue) != Math.Sign(PGNode2ScalarProductValue))
                {
                    // если направление движения выбрано правильно, то связь, соединяющая узлы 1 и 2, становится следующим по движению объектом
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
        /// метод GetNextCrossingObject4GraphConn возвращает следующий по направлению движения объект пересечения, если текущий - связь
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <param name="CurrentCrossingNode"></param>
        /// <returns></returns>
        private CrossingObjectClass GetNextCrossingObject4GraphConn(CrossingObjectClass CurrentCrossingObject, PolyhedronGraphNode CurrentCrossingNode, Vector3D PiDirectingVector)
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

            Double NextPGNode1ScalarProductValue = Vector3D.ScalarProduct(NextPGNode1.NodeNormal, PiDirectingVector);
            Double NextPGNode2ScalarProductValue = Vector3D.ScalarProduct(NextPGNode2.NodeNormal, PiDirectingVector);

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
        private CrossingObjectClass GetNextCrossingObject(CrossingObjectClass CurrentCrossingObject, PolyhedronGraphNode CurrentCrossingNode, Vector3D PiDirectingVector)
        {
            CrossingObjectClass NextCrossingObject = null;

            // если текущий объект – узел
            if (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode)
            {
                NextCrossingObject = GetNextCrossingObject4GraphNode(CurrentCrossingObject, PiDirectingVector);
            }
            // если текущий объект – связь
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
        /// метод AddConns4PrevNodeCurrentConnCase добавляет необходимые связи в случае, если предыдущий объект пересечения - узел, а текущий - связь
        /// связи добавляются для того, чтобы граф оставался триангулированным
        /// </summary>
        /// <param name="PreviousCrossingObject"></param>
        /// <param name="PreviousCrossingNode"></param>
        /// <param name="CurrentCrossingObject"></param>
        /// <param name="CurrentCrossingNode"></param>
        private void AddConns4PrevNodeCurrentConnCase(CrossingObjectClass PreviousCrossingObject, PolyhedronGraphNode PreviousCrossingNode, CrossingObjectClass CurrentCrossingObject, PolyhedronGraphNode CurrentCrossingNode)
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

        /// <summary>
        /// метод AddConns4PrevConnCurrentNodeCase добавляет необходимые связи в случае, если предыдущий объект пересечения - связь, а текущий - узел
        /// связи добавляются для того, чтобы граф оставался триангулированным
        /// </summary>
        /// <param name="PreviousCrossingObject"></param>
        /// <param name="PreviousCrossingNode"></param>
        /// <param name="CurrentCrossingObject"></param>
        /// <param name="CurrentCrossingNode"></param>
        private void AddConns4PrevConnCurrentNodeCase(CrossingObjectClass PreviousCrossingObject, PolyhedronGraphNode PreviousCrossingNode, CrossingObjectClass CurrentCrossingObject, PolyhedronGraphNode CurrentCrossingNode)
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

        /// <summary>
        /// метод AddConns4PrevConnCurrentConnCase добавляет необходимые связи в случае, если и предыдущий, и текущий объекты пересечения - связи
        /// связи добавляются для того, чтобы граф оставался триангулированным
        /// </summary>
        /// <param name="PreviousCrossingObject"></param>
        /// <param name="PreviousCrossingNode"></param>
        /// <param name="CurrentCrossingObject"></param>
        /// <param name="CurrentCrossingNode"></param>
        private void AddConns4PrevConnCurrentConnCase(CrossingObjectClass PreviousCrossingObject, PolyhedronGraphNode PreviousCrossingNode, CrossingObjectClass CurrentCrossingObject, PolyhedronGraphNode CurrentCrossingNode)
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
                throw new AlgorithmException("AddConns4PrevConnCurrentConnCase method incorrect work");
            }
        }

        /// <summary>
        /// метод BuildGFiGrid строит сетку G(...Fi...) (см. алгоритм)
        /// </summary>
        private void BuildGFiGrid(Vector3D PiDirectingVector)
        {
            // первый (запомненный) объект пересечения
            CrossingObjectClass FirstCrossingObject = GetFirstCrossingObject(PiDirectingVector);
            // текущий объект пересечения
            CrossingObjectClass CurrentCrossingObject = FirstCrossingObject;
            // строим узел на пересечении текущего объекта и G(...Pi...) и запоминаем его
            // если этот узел отсутствует в списке узлов, то добавляем его и соответствующие ссылки на данный узел
            PolyhedronGraphNode FirstCrossingNode = BuildCrossingNode(CurrentCrossingObject, PiDirectingVector);
            // текущий узел пересечения
            PolyhedronGraphNode CurrentCrossingNode = FirstCrossingNode;

            if (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection)
            {
                AddCrossingNodeBetweenConn(CurrentCrossingObject.PGNode1, CurrentCrossingObject.PGNode2, CurrentCrossingNode);
            }

            do
            {
                // предыдущий объект пересечения
                CrossingObjectClass PreviousCrossingObject = CurrentCrossingObject;
                // предыдущий узел пересечения
                PolyhedronGraphNode PreviousCrossingNode = CurrentCrossingNode;
                // получаем следующий по движению объект (связь, либо узел) и делаем его текущим
                CurrentCrossingObject = GetNextCrossingObject(CurrentCrossingObject, CurrentCrossingNode, PiDirectingVector);
                // строим узел на пересечении текущего объекта и G(...Pi...)
                // если этот узел отсутствует в списке узлов (этот узел будет присутствовать в списке узлов, если текущий объект – узел, либо если начальным объектом была связь и мы в нее пришли), то добавляем его и соответствующие ссылки на данный узел
                // отдельно обрабатываем случай если мы пришли в первый (запомненный) объект пересечения (для простоты реализации алгоритма)
                CurrentCrossingNode = (CurrentCrossingObject == FirstCrossingObject ? FirstCrossingNode : BuildCrossingNode(CurrentCrossingObject, PiDirectingVector));
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
                    AddConns4PrevNodeCurrentConnCase(PreviousCrossingObject, PreviousCrossingNode, CurrentCrossingObject, CurrentCrossingNode);
                }

                // если предыдущий объект связь, а текущий узел
                if ((PreviousCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection) &&
                    (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode))
                {
                    AddConns4PrevConnCurrentNodeCase(PreviousCrossingObject, PreviousCrossingNode, CurrentCrossingObject, CurrentCrossingNode);
                }

                // если предыдущий и текущий объекты - связи
                if ((PreviousCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection) &&
                    (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphConnection))
                {
                    AddConns4PrevConnCurrentConnCase(PreviousCrossingObject, PreviousCrossingNode, CurrentCrossingObject, CurrentCrossingNode);
                }
            }
            while (CurrentCrossingObject != FirstCrossingObject);
        }

        /// <summary>
        /// метод GetFirstCrossingObject2 возвращает первый объект пересечения Zi с G(...Fi...)
        /// </summary>
        /// <param name="StartingPGNode"></param>
        /// <returns></returns>
        private CrossingObjectClass GetFirstCrossingObject2(PolyhedronGraphNode StartingPGNode, Vector3D QiDirectingVector)
        {
            CrossingObjectClass FirstCrossingObject = null;

            // текущий узел
            PolyhedronGraphNode CurrentPGNode = StartingPGNode;
            // вычисляем скалярное произведение вектора, связанного с текущим узлом, и направляющего вектора отрезка Qi
            Double CurrentScalarProductValue = Vector3D.ScalarProduct(CurrentPGNode.NodeNormal, QiDirectingVector);
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
                    // считаем скалярное произведение вектора, связанного с полученным выше узлом, и направляющего вектора отрезка Qi
                    Double CurrentConnNodeScalarProductValue = Vector3D.ScalarProduct(CurrentConnPGNode.NodeNormal, QiDirectingVector);

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
        /// метод GetFirstCrossingObject2 возвращает первый объект пересечения Zi с G(...Fi...)
        /// </summary>
        /// <returns></returns>
        private CrossingObjectClass GetFirstCrossingObject2(Vector3D QiDirectingVector)
        {
            return GetFirstCrossingObject2(m_PGNodeList[0], QiDirectingVector);
        }

        /// <summary>
        /// метод BuildCrossingNode2 возвращает узел пересечении текущего объекта с Zi
        /// если текущий объект - узел, то он же и возвращается
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
                // вектор, перпендикулярный к PlusValueVector и MinusValueVector
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
        /// метод CheckMoveDirection2 возвращает true, если направление движения по Zi правильное, иначе возвращается false
        /// правильным считается направление движения против часовой стрелки, если смотреть с конца направляющего вектора Qi
        /// </summary>
        /// <param name="CheckCrossingObject"></param>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private Boolean CheckMoveDirection2(CrossingObjectClass CheckCrossingObject, CrossingObjectClass CurrentCrossingObject, Vector3D QiDirectingVector)
        {
            // направляющий вектор Qi становится ортом оси OZ
            Vector3D OZDirectingVector = QiDirectingVector;

            // строим узел на пересечении текущего объекта и Zi; вектор, связанный с этим узлом, становится ортом оси OX
            PolyhedronGraphNode CurrentCrossingNode = BuildCrossingNode2(CurrentCrossingObject, QiDirectingVector);
            Vector3D OXDirectingVector = CurrentCrossingNode.NodeNormal;

            // строим орт оси OY правой СК XYZ (как векторное произведение орта оси OZ на орт оси OX)
            Vector3D OYDirectingVector = Vector3D.VectorProduct(OZDirectingVector, OXDirectingVector);

            // строим узел на пересечении проверяемого объекта и Zi; вычисляем скалярное произведение вектора, связанного с этим узлом, и орта оси OY
            PolyhedronGraphNode CheckCrossingNode = BuildCrossingNode2(CheckCrossingObject, QiDirectingVector);
            Vector3D CheckVector = CheckCrossingNode.NodeNormal;
            Double ScalarProductValue = Vector3D.ScalarProduct(CheckVector, OYDirectingVector);

            // если ScalarProductValue = 0 - это ошибка работы алгоритма
            if (Math.Abs(ScalarProductValue) < Epsilon)
            {
                throw new AlgorithmException("CheckMoveDirection method incorrect work");
            }

            // если вычисленное скалярное произведение > 0, то направление движения правильное, иначе направление движения неправильное
            return (ScalarProductValue > 0 ? true : false);
        }

        /// <summary>
        /// метод AddConns2SuspiciousConnectionsList добавляет в список "подозрительных" связей сам объект пересечения (если это связь) и все соседние связи
        /// если объект пересечения - узел, то возможно надо добавлять не только связи, которые содержат этот узел, но и все соседние связи (пока так и делается) ???!!!
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <param name="SuspiciousConnectionSet"></param>
        private void AddConns2SuspiciousConnectionsList(CrossingObjectClass CurrentCrossingObject, SuspiciousConnectionSetClass SuspiciousConnectionSet)
        {
            // если текущий объект – узел
            if (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode)
            {
                // текущий узел
                PolyhedronGraphNode CurrentPGNode = CurrentCrossingObject.PGNode1;

                // цикл по всем связям текущего узла (текущего объекта пересечения)
                for (Int32 ConnIndex = 0; ConnIndex < CurrentPGNode.NodeConnectionCount; ConnIndex++)
                {
                    // текущая связь
                    PolyhedronGraphNode CurrentPGConn = CurrentPGNode[ConnIndex];
                    // если текущая связь отсутствует в наборе П, то добавляем ее в этот набор
                    SuspiciousConnectionSet.AddConnection(CurrentPGNode, CurrentPGConn);
                }
            }
            // если текущий объект – связь
            else
            {
                PolyhedronGraphNode PlusPGNode = CurrentCrossingObject.PGNode1;
                PolyhedronGraphNode MinusPGNode = CurrentCrossingObject.PGNode2;

                // если текущая связь (текущий объект пересечения) отсутствует в наборе П, то добавляем ее в этот набор
                SuspiciousConnectionSet.AddConnection(PlusPGNode, MinusPGNode);
                // цикл по всем связям положительного узла текущей связи
                for (Int32 ConnIndex = 0; ConnIndex < PlusPGNode.NodeConnectionCount; ConnIndex++)
                {
                    // если рассматриваемая связь положительного узла текущей связи отсутствует в наборе П, то добавляем ее в этот набор
                    SuspiciousConnectionSet.AddConnection(PlusPGNode, PlusPGNode[ConnIndex]);
                }
                // цикл по всем связям отрицательного узла текущей связи
                for (Int32 ConnIndex = 0; ConnIndex < MinusPGNode.NodeConnectionCount; ConnIndex++)
                {
                    // если рассматриваемая связь отрицательного узла текущей связи отсутствует в наборе П, то добавляем ее в этот набор
                    SuspiciousConnectionSet.AddConnection(MinusPGNode, MinusPGNode[ConnIndex]);
                }
            }
        }

        /// <summary>
        /// метод GetNextCrossingObject4GraphNode2 возвращает следующий по направлению движения объект пересечения (Zi с G(...Fi...)), если текущий - узел
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private CrossingObjectClass GetNextCrossingObject4GraphNode2(CrossingObjectClass CurrentCrossingObject, Vector3D QiDirectingVector)
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

                // вычисляем скалярное произведение вектора 1 и направляющего вектора Qi
                Double PGNode1ScalarProductValue = Vector3D.ScalarProduct(PGNode1.NodeNormal, QiDirectingVector);
                // вычисляем скалярное произведение вектора 2 и направляющего вектора Qi
                Double PGNode2ScalarProductValue = Vector3D.ScalarProduct(PGNode2.NodeNormal, QiDirectingVector);

                // если скалярное произведение узла 1 и направляющего вектора Qi == 0
                if (Math.Abs(PGNode1ScalarProductValue) < Epsilon)
                {
                    // если направление движения выбрано правильно, то узел номер 1 становится следующим по движению объектом
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

                // если скалярное произведение узла 2 и направляющего вектора Qi == 0
                if (Math.Abs(PGNode2ScalarProductValue) < Epsilon)
                {
                    // если направление движения выбрано правильно, то узел номер 2 становится следующим по движению объектом
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

                // если скалярные произведения узлов 1 и 2 и направляющего вектора Qi имеют разный знак
                if (Math.Sign(PGNode1ScalarProductValue) != Math.Sign(PGNode2ScalarProductValue))
                {
                    // если направление движения выбрано правильно, то связь, соединяющая узлы 1 и 2, становится следующим по движению объектом
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
        /// метод GetNextCrossingObject4GraphConn2 возвращает следующий по направлению движения объект пересечения (Zi с G(...Fi...)), если текущий - связь
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private CrossingObjectClass GetNextCrossingObject4GraphConn2(CrossingObjectClass CurrentCrossingObject, Vector3D QiDirectingVector)
        {
            CrossingObjectClass NextCrossingObject = null;

            // положительный узел текущей связи
            PolyhedronGraphNode PlusPGNode = CurrentCrossingObject.PGNode1;
            // отрицательный узел текущей связи
            PolyhedronGraphNode MinusPGNode = CurrentCrossingObject.PGNode2;

            Int32 CurrentConnPlusIndex = PlusPGNode.GetConnectionIndex(MinusPGNode);
            Int32 CurrentConnMinusIndex = MinusPGNode.GetConnectionIndex(PlusPGNode);
            /*Int32 CurrentConnIndex1 = CurrentPGNode1.GetConnectionIndex(CurrentCrossingNode);
            Int32 CurrentConnIndex2 = CurrentPGNode2.GetConnectionIndex(CurrentCrossingNode);*/

            // для положительного узла (CurrentCrossingObject.PGNode1==PlusPGNode) берем следующую связь (относительно текущей)
            PolyhedronGraphNode NextPGNode1 = (CurrentConnPlusIndex == (PlusPGNode.NodeConnectionCount - 1) ? PlusPGNode[0] : PlusPGNode[CurrentConnPlusIndex + 1]);
            // для отрицательного узла (CurrentCrossingObject.PGNode2==MinusPGNode) берем предыдущую связь (относительно текущей)
            PolyhedronGraphNode NextPGNode2 = (CurrentConnMinusIndex == 0 ? MinusPGNode[MinusPGNode.NodeConnectionCount - 1] : MinusPGNode[CurrentConnMinusIndex - 1]);

            Double NextPGNode1ScalarProductValue = Vector3D.ScalarProduct(NextPGNode1.NodeNormal, QiDirectingVector);
            Double NextPGNode2ScalarProductValue = Vector3D.ScalarProduct(NextPGNode2.NodeNormal, QiDirectingVector);

            // если полученный узел (номер 1) нулевой
            if (Math.Abs(NextPGNode1ScalarProductValue) < Epsilon)
            {
                // если полученный узел номер 2 не нулевой
                if (Math.Abs(NextPGNode2ScalarProductValue) >= Epsilon)
                {
                    throw new AlgorithmException("GetNextCrossingObject4GraphConn2 method incorrect work");
                }

                // полученный узел становится следующим по движению объектом
                NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphNode, NextPGNode1, null);
                // exit
            }
            // если полученный узел (номер 1) положительный
            else if (NextPGNode1ScalarProductValue > 0)
            {
                // связь, соединяющая новый положительный узел и старый отрицательный узел, становится следующим по движению объектом
                NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphConnection, NextPGNode1, MinusPGNode);
                // exit
            }
            // если полученный узел (номер 1) отрицательный
            else if (NextPGNode1ScalarProductValue < 0)
            {
                // связь, соединяющая новый отрицательный узел и старый положительный узел, становится следующим по движению объектом
                NextCrossingObject = new CrossingObjectClass(CrossingObjectTypeEnum.GraphConnection, PlusPGNode, NextPGNode2);
                // exit
            }

            return NextCrossingObject;
        }

        /// <summary>
        /// метод GetNextCrossingObject2 возвращает следующий по направлению движения объект пересечения (Zi с G(...Fi...))
        /// </summary>
        /// <param name="CurrentCrossingObject"></param>
        /// <returns></returns>
        private CrossingObjectClass GetNextCrossingObject2(CrossingObjectClass CurrentCrossingObject, Vector3D QiDirectingVector)
        {
            // если текущий объект – узел
            if (CurrentCrossingObject.CrossingObjectType == CrossingObjectTypeEnum.GraphNode)
            {
                return GetNextCrossingObject4GraphNode2(CurrentCrossingObject, QiDirectingVector);
            }
            // если текущий объект – связь
            else
            {
                return GetNextCrossingObject4GraphConn2(CurrentCrossingObject, QiDirectingVector);
            }
        }

        /// <summary>
        /// метод GetSuspiciousConnectionsList возвращает набор всех "подозрительных" связей (набор П)
        /// </summary>
        /// <returns></returns>
        private SuspiciousConnectionSetClass GetSuspiciousConnectionsList(Vector3D QiDirectingVector)
        {
            // создаем набор П
            SuspiciousConnectionSetClass SuspiciousConnectionSet = new SuspiciousConnectionSetClass();

            // первый (запомненный) объект пересечения
            CrossingObjectClass FirstCrossingObject = GetFirstCrossingObject2(QiDirectingVector);
            // текущий объект пересечения
            CrossingObjectClass CurrentCrossingObject = FirstCrossingObject;
            // добавляем в набор П сам объект пересечения и все соседние с ним связи
            AddConns2SuspiciousConnectionsList(CurrentCrossingObject, SuspiciousConnectionSet);

            do
            {
                // получаем следующий по движению объект (связь, либо узел) и делаем его текущим
                CurrentCrossingObject = GetNextCrossingObject2(CurrentCrossingObject, QiDirectingVector);
                // добавляем в набор П сам объект пересечения и все соседние с ним связи
                AddConns2SuspiciousConnectionsList(CurrentCrossingObject, SuspiciousConnectionSet);
            }
            while (CurrentCrossingObject != FirstCrossingObject);

            return SuspiciousConnectionSet;
        }

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
        /// метод SolveEquationSystem3Kramer решает систему лин. уравнений (3x3) методом Крамера
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
        /// метод SolveEquationSystem3 решает систему лин. уравнений (3x3) одним из методов
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
        /// метод FuncFi возвращает значение функции Fi (см. алгоритм)
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
        /// метод SolveCone123EquationSystem решает систему лин. уравнений (3x3); решение используется для проверки локальной выпуклости связи 1-2 (см. алгоритм)
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
        /// метод CalcLambda123Koeff вычисляет коеффициенты Lambda1, ..., Lambda3 и возвращает их в виде матрицы-столбца
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
        /// метод ReplaceConn12WithConn34 заменяет невыпуклую связь 1-2 на связь 3-4 (см. алгоритм)
        /// </summary>
        /// <param name="PGNode1"></param>
        /// <param name="PGNode2"></param>
        /// <param name="PGNode3"></param>
        /// <param name="PGNode4"></param>
        /// <param name="SuspiciousConnectionSet"></param>
        private void ReplaceConn12WithConn34(PolyhedronGraphNode PGNode1, PolyhedronGraphNode PGNode2, PolyhedronGraphNode PGNode3, PolyhedronGraphNode PGNode4, SuspiciousConnectionSetClass SuspiciousConnectionSet)
        {
            // удаляем связь на узел 2 из списка связей узла 1 (аналогично для узла 2)
            PGNode1.RemoveNodeConnection(PGNode2);
            PGNode2.RemoveNodeConnection(PGNode1);

            // индекс связи на узел 3 в списке связей узла 1
            Int32 Conn31Index = PGNode3.GetConnectionIndex(PGNode1);
            // связь 3-4 вставляется перед связью 3-1 (т.е. на ее место)
            PGNode3.InsertNodeConnection(Conn31Index, PGNode4);

            // индекс связи на узел 1 в списке связей узла 4
            Int32 Conn41Index = PGNode4.GetConnectionIndex(PGNode1);
            // связь 4-3 вставляется после связи 4-1 (т.е. на следующее место)
            PGNode4.InsertNodeConnection(Conn41Index + 1, PGNode3);

            // удаляем из множества П связь 1-2 (ее нет)
            SuspiciousConnectionSet.RemoveConnection(PGNode1, PGNode2);
            // добавляем в набор П связи: 1-3, 1-4, 2-3, 2-4 (те из них, которых в наборе П нет)
            SuspiciousConnectionSet.AddConnection(PGNode1, PGNode3);
            SuspiciousConnectionSet.AddConnection(PGNode1, PGNode4);
            SuspiciousConnectionSet.AddConnection(PGNode2, PGNode3);
            SuspiciousConnectionSet.AddConnection(PGNode2, PGNode4);
        }

        /// <summary>
        /// метод RemoveNode2 удаляет узел графа номер 1 (см. алгоритм)
        /// </summary>
        /// <param name="PGNode1"></param>
        /// <param name="PGNode2"></param>
        /// <param name="PGNode3"></param>
        /// <param name="PGNode4"></param>
        /// <param name="SuspiciousConnectionSet"></param>
        private void RemoveNode1(PolyhedronGraphNode PGNode1, PolyhedronGraphNode PGNode2, PolyhedronGraphNode PGNode3, PolyhedronGraphNode PGNode4, SuspiciousConnectionSetClass SuspiciousConnectionSet)
        {
            /*// случае удаления узла 1 аналогичен случаю удаления узла 2 если поменять местами узлы 1 и 2, а также узлы 3 и 4 (см. алгоритм)
            RemoveNode2(PGNode2, PGNode1, PGNode4, PGNode3, SuspiciousConnectionSet);*/

            /*// удаляем из набора П связи, содержащие узел 1
            SuspiciousConnectionSet.RemoveConnections(PGNode1);
            // удаляем ссылку на узел 1 из списка связей узлов 2,3,4
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
            // добавляем в набор П связи 2-3 и 2-4
            SuspiciousConnectionSet.AddConnection(PGNode2, PGNode3);
            SuspiciousConnectionSet.AddConnection(PGNode2, PGNode4);

            //
            m_PGNodeList.Remove(PGNode1);*/
        }

        /// <summary>
        /// метод RemoveNode2 удаляет узел графа номер 2 (см. алгоритм)
        /// </summary>
        /// <param name="PGNode1"></param>
        /// <param name="PGNode2"></param>
        /// <param name="PGNode3"></param>
        /// <param name="PGNode4"></param>
        /// <param name="SuspiciousConnectionSet"></param>
        private void RemoveNode2(PolyhedronGraphNode PGNode1, PolyhedronGraphNode PGNode2, PolyhedronGraphNode PGNode3, PolyhedronGraphNode PGNode4, SuspiciousConnectionSetClass SuspiciousConnectionSet)
        {
            /*// удаляем ссылки на узел 2 из списка связей тех узлов, с которыми связан узел 2
            for (Int32 PGConnIndex = 0; PGConnIndex < PGNode2.NodeConnectionCount; PGConnIndex++)
            {
                PGNode2[PGConnIndex].RemoveNodeConnection(PGNode2);
            }
            // удаляем узел 2
            m_PGNodeList.Remove(PGNode2);
            SuspiciousConnectionSet.RemoveConnections(PGNode2);

            // индекс связи на узел 3 в списке связей узла 1
            Int32 Conn13Index = PGNode1.GetConnectionIndex(PGNode3);
            // строим наикратчайший путь обхода, начинающийся на связи 1-3 и заканчивающийся на связи 4-1
            List<PolyhedronGraphNode> ShortestGraphPath = GetShortestGraphPath(PGNode1, PGNode3, PGNode4);
            // цикл со 2-го по N-1 узел из построенного пути обхода
            for (Int32 GraphPathIndex = 1; GraphPathIndex < ShortestGraphPath.Count - 1; GraphPathIndex++)
            {
                // в список связей узла 1 добавляем ссылку на текущий узел (со 2-го по Count-1) из пути обхода
                // ссылка добавляется после последней добавленной связи или связи 1-3, если последней добавленной связи нет
                PGNode1.InsertNodeConnection(Conn13Index + GraphPathIndex, ShortestGraphPath[GraphPathIndex]);

                // в список связей i-го узла из построенного пути обхода добавляем (эту же) связь перед связью, по которой мы пришли в этот узел
                Int32 CurrentPathNodeConnFromIndex = ShortestGraphPath[GraphPathIndex].GetConnectionIndex(ShortestGraphPath[GraphPathIndex - 1]);
                ShortestGraphPath[GraphPathIndex].InsertNodeConnection(CurrentPathNodeConnFromIndex, PGNode1);

                // добавляем в набор П только что созданную связь
                SuspiciousConnectionSet.AddConnection(PGNode1, ShortestGraphPath[GraphPathIndex]);
                // добавляем в набор П связь между (i-1)-м и i-м узлами из найденного пути (элементы границы сектора К*)
                SuspiciousConnectionSet.AddConnection(ShortestGraphPath[GraphPathIndex - 1], ShortestGraphPath[GraphPathIndex]);
            }
            SuspiciousConnectionSet.AddConnection(ShortestGraphPath[ShortestGraphPath.Count - 2], ShortestGraphPath[ShortestGraphPath.Count - 1]);

            // добавляем в набор П связи 1-3 и 1-4
            SuspiciousConnectionSet.AddConnection(PGNode1, PGNode3);
            SuspiciousConnectionSet.AddConnection(PGNode1, PGNode4);*/

            /*// удаляем из набора П связи, содержащие узел 2
            SuspiciousConnectionSet.RemoveConnections(PGNode2);
            // удаляем ссылку на узел 2 из списка связей узлов 1,3,4
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
            // добавляем в набор П связи 1-3 и 1-4
            SuspiciousConnectionSet.AddConnection(PGNode1, PGNode3);
            SuspiciousConnectionSet.AddConnection(PGNode1, PGNode4);

            //
            m_PGNodeList.Remove(PGNode2);*/
        }

        /*/// <summary>
        /// метод RemoveNode удаляет узел RemovedNode и триангулирует полученный сектор
        /// </summary>
        /// <param name="RemovedNode"></param>
        private void RemoveNode(PolyhedronGraphNode RemovedNode)
        {
            // удаляем удаляемый узел из списка узлов графа
            m_PGNodeList.Remove(RemovedNode);

            List<PolyhedronGraphNode> SectorPGNodeList = new List<PolyhedronGraphNode>();
            // цикл по всем связям удаляемого узла
            for (Int32 ConnIndex = 0; ConnIndex < RemovedNode.NodeConnectionCount; ConnIndex++)
            {
                // узел, связанный с удаляемым текущей связью, становится текущим узлом
                PolyhedronGraphNode CurrentConnNode = RemovedNode[ConnIndex];
                PolyhedronGraphNode NextConnNode = (ConnIndex == RemovedNode.NodeConnectionCount - 1 ? RemovedNode[0] : RemovedNode[ConnIndex + 1]);
                // добавляем текущий узел в конец упорядоченного (против ч.с.) списка узлов сектора
                SectorPGNodeList.Add(CurrentConnNode);
                // удаляем ссылку на удаляемый узел из списка связей текущего узла
                CurrentConnNode.RemoveNodeConnection(RemovedNode);
            }

            // текущим узлом становится первый узел из упорядоченного списка узлов сектора
            PolyhedronGraphNode CurrentPGNode = SectorPGNodeList[0];
            Int32 CurrentPGNodeIndex = 0;

            while (SectorPGNodeList.Count > 3)
            {
                // узлом номер 1 становится текущий узел
                PolyhedronGraphNode PGNode1 = CurrentPGNode;
                // узлом номер 2 становится следующий (относительно узла 1) по направлению движения (из упорядоченного списка узлов сектора)
                PolyhedronGraphNode PGNode2 = (CurrentPGNodeIndex + 1 >= SectorPGNodeList.Count ? SectorPGNodeList[CurrentPGNodeIndex + 1 - SectorPGNodeList.Count] : SectorPGNodeList[CurrentPGNodeIndex + 1]);
                // узлом номер 3 становится следующий (относительно узла 2) по направлению движения (из упорядоченного списка узлов сектора)
                PolyhedronGraphNode PGNode3 = (CurrentPGNodeIndex + 2 >= SectorPGNodeList.Count ? SectorPGNodeList[CurrentPGNodeIndex + 2 - SectorPGNodeList.Count] : SectorPGNodeList[CurrentPGNodeIndex + 2]);

                // вычисляем смешанное произведение векторов, связанных с узлами 2, 1 и 3
                Double MixedProduct213Value = Vector3D.MixedProduct(PGNode2.NodeNormal, PGNode1.NodeNormal, PGNode3.NodeNormal);
                // вычисляем смешанное произведение векторов, связанных с удаляемым узлом и узлами 1 и 3
                Double MixedProductR13Value = Vector3D.MixedProduct(RemovedNode.NodeNormal, PGNode1.NodeNormal, PGNode3.NodeNormal);

                // если знаки вычисленных смешанных произведений разные, то связь 1-3 может быть построена 
                Boolean IsConn13Correct = ((MixedProduct213Value > Epsilon) && (MixedProductR13Value < -Epsilon)) || ((MixedProductR13Value > Epsilon) && (MixedProduct213Value < -Epsilon)) || ((MixedProductR13Value > -Epsilon) && (MixedProductR13Value < Epsilon));

                if (IsConn13Correct)
                {
                    Int32 Conn12Index = PGNode1.GetConnectionIndex(PGNode2);
                    Int32 Conn32Index = PGNode3.GetConnectionIndex(PGNode2);

                    // в список связей узла 1 после ссылки на узел 2 добавляем ссылку на узел 3
                    PGNode1.InsertNodeConnection(Conn12Index + 1, PGNode3);
                    // в список связей узла 3 перед ссылкой на узел 2 добавляем ссылку на узел 1
                    PGNode3.InsertNodeConnection(Conn32Index, PGNode1);
                    // удаляем узел 2 из упорядоченного списка узлов сектора
                    SectorPGNodeList.Remove(PGNode2);

                    if (CurrentPGNodeIndex == SectorPGNodeList.Count) CurrentPGNodeIndex--;
                }
                else
                {
                    // текущим становится узел номер 2
                    CurrentPGNode = PGNode2;
                    CurrentPGNodeIndex = (CurrentPGNodeIndex == SectorPGNodeList.Count - 1 ? 0 : CurrentPGNodeIndex + 1);
                }
            }
        }*/

        /// <summary>
        /// метод RemoveNode удаляет узел RemovedNode и триангулирует полученный сектор
        /// </summary>
        /// <param name="RemovedNode"></param>
        /// <param name="SuspiciousConnectionSet"></param>
        private void RemoveNode(PolyhedronGraphNode RemovedNode, SuspiciousConnectionSetClass SuspiciousConnectionSet)
        {
            // удаляем удаляемый узел из списка узлов графа
            m_PGNodeList.Remove(RemovedNode);
            // удаляем из набора П все связи содержащие удаляемый узел
            SuspiciousConnectionSet.RemoveConnections(RemovedNode);

            List<PolyhedronGraphNode> SectorPGNodeList = new List<PolyhedronGraphNode>();
            // цикл по всем связям удаляемого узла
            for (Int32 ConnIndex = 0; ConnIndex < RemovedNode.NodeConnectionCount; ConnIndex++)
            {
                // узел, связанный с удаляемым текущей связью, становится текущим узлом
                PolyhedronGraphNode CurrentConnNode = RemovedNode[ConnIndex];
                PolyhedronGraphNode NextConnNode = (ConnIndex == RemovedNode.NodeConnectionCount - 1 ? RemovedNode[0] : RemovedNode[ConnIndex + 1]);
                // добавляем текущий узел в конец упорядоченного (против ч.с.) списка узлов сектора
                SectorPGNodeList.Add(CurrentConnNode);
                // удаляем ссылку на удаляемый узел из списка связей текущего узла
                CurrentConnNode.RemoveNodeConnection(RemovedNode);
                // добавляем в набор П связь, соединяющую текущий и следующий (узел, связанный с удаляемым следующей по отношению к текущей связью) узлы
                SuspiciousConnectionSet.AddConnection(CurrentConnNode, NextConnNode);
            }

            // текущим узлом становится первый узел из упорядоченного списка узлов сектора
            PolyhedronGraphNode CurrentPGNode = SectorPGNodeList[0];
            Int32 CurrentPGNodeIndex = 0;

            Int32 IterationNumber = 0;
            while (SectorPGNodeList.Count > 3)
            {
                // узлом номер 1 становится текущий узел
                PolyhedronGraphNode PGNode1 = CurrentPGNode;
                // узлом номер 2 становится следующий (относительно узла 1) по направлению движения (из упорядоченного списка узлов сектора)
                PolyhedronGraphNode PGNode2 = (CurrentPGNodeIndex + 1 >= SectorPGNodeList.Count ? SectorPGNodeList[CurrentPGNodeIndex + 1 - SectorPGNodeList.Count] : SectorPGNodeList[CurrentPGNodeIndex + 1]);
                // узлом номер 3 становится следующий (относительно узла 2) по направлению движения (из упорядоченного списка узлов сектора)
                PolyhedronGraphNode PGNode3 = (CurrentPGNodeIndex + 2 >= SectorPGNodeList.Count ? SectorPGNodeList[CurrentPGNodeIndex + 2 - SectorPGNodeList.Count] : SectorPGNodeList[CurrentPGNodeIndex + 2]);
                // узлом номер 4 становится следующий (относительно узла 3) по направлению движения (из упорядоченного списка узлов сектора)
                PolyhedronGraphNode PGNode4 = (CurrentPGNodeIndex + 3 >= SectorPGNodeList.Count ? SectorPGNodeList[CurrentPGNodeIndex + 3 - SectorPGNodeList.Count] : SectorPGNodeList[CurrentPGNodeIndex + 3]);

                // вычисляем смешанное произведение векторов, связанных с узлами 2, 1 и 3
                Double MixedProduct213Value = Vector3D.MixedProduct(PGNode2.NodeNormal, PGNode1.NodeNormal, PGNode3.NodeNormal);
                // вычисляем смешанное произведение векторов, связанных с узлами 4, 1 и 3
                Double MixedProduct413Value = Vector3D.MixedProduct(PGNode4.NodeNormal, PGNode1.NodeNormal, PGNode3.NodeNormal);
                // вычисляем смешанное произведение векторов, связанных с удаляемым узлом и узлами 1 и 3
                Double MixedProductR13Value = Vector3D.MixedProduct(RemovedNode.NodeNormal, PGNode1.NodeNormal, PGNode3.NodeNormal);

                // если знаки вычисленных смешанных произведений разные, то связь 1-3 может быть построена 
                Boolean IsConn13Correct = ((MixedProduct213Value > Epsilon) && (MixedProductR13Value < -Epsilon)) || ((MixedProductR13Value > Epsilon) && (MixedProduct213Value < -Epsilon)) || ((MixedProductR13Value > -Epsilon) && (MixedProductR13Value < Epsilon));

                // если знаки вычисленных смешанных произведений MixedProduct213Value и MixedProductR13Value разные, то связь 1-3 может быть построена
                //Boolean IsConn13Correct = ((MixedProduct213Value > Epsilon) && (MixedProductR13Value < -Epsilon)) || ((MixedProductR13Value > Epsilon) && (MixedProduct213Value < -Epsilon)) || ((MixedProductR13Value > -Epsilon) && (MixedProductR13Value < Epsilon));
                // Boolean IsConn13Correct = ((MixedProduct213Value > Epsilon) && (MixedProductR13Value < -Epsilon)) || ((MixedProductR13Value > Epsilon) && (MixedProduct213Value < -Epsilon));
                // если MixedProductR13Value = 0 и знаки вычисленных смешанных произведений MixedProduct213Value и MixedProduct413Value разные, то связь 1-3 может быть построена
                // IsConn13Correct = IsConn13Correct || ((MixedProduct213Value > Epsilon) && (MixedProduct413Value < -Epsilon)) || ((MixedProduct413Value > Epsilon) && (MixedProduct213Value < -Epsilon));
                //Boolean IsConn13Correct = (MixedProduct213Value * MixedProductR13Value < 0) || ((MixedProductR13Value == 0) && (MixedProduct213Value * MixedProduct413Value < 0));

                if (IsConn13Correct)
                {
                    Int32 Conn12Index = PGNode1.GetConnectionIndex(PGNode2);
                    Int32 Conn32Index = PGNode3.GetConnectionIndex(PGNode2);

                    // в список связей узла 1 после ссылки на узел 2 добавляем ссылку на узел 3
                    PGNode1.InsertNodeConnection(Conn12Index + 1, PGNode3);
                    // в список связей узла 3 перед ссылкой на узел 2 добавляем ссылку на узел 1
                    PGNode3.InsertNodeConnection(Conn32Index, PGNode1);
                    // удаляем узел 2 из упорядоченного списка узлов сектора
                    SectorPGNodeList.Remove(PGNode2);

                    if (CurrentPGNodeIndex == SectorPGNodeList.Count) CurrentPGNodeIndex--;
                    
                    // добавляем в набор П связь 1-3
                    SuspiciousConnectionSet.AddConnection(PGNode1, PGNode3);

                    IterationNumber = 0;
                }
                else
                {
                    // текущим становится узел номер 2
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
        /// метод RemoveNode удаляет узел RemovedNode и триангулирует полученный сектор
        /// </summary>
        /// <param name="RemovedNode"></param>
        /// <param name="SuspiciousConnectionSet"></param>
        private void RemoveNode(PolyhedronGraphNode RemovedNode, SuspiciousConnectionSetClass SuspiciousConnectionSet)
        {
            // удаляем удаляемый узел из списка узлов графа
            m_PGNodeList.Remove(RemovedNode);
            // удаляем из набора П все связи содержащие удаляемый узел
            SuspiciousConnectionSet.RemoveConnections(RemovedNode);

            List<PolyhedronGraphNode> SectorPGNodeList = new List<PolyhedronGraphNode>();
            // цикл по всем связям удаляемого узла
            for (Int32 ConnIndex = 0; ConnIndex < RemovedNode.NodeConnectionCount; ConnIndex++)
            {
                // узел, связанный с удаляемым текущей связью, становится текущим узлом
                PolyhedronGraphNode CurrentConnNode = RemovedNode[ConnIndex];
                PolyhedronGraphNode NextConnNode = (ConnIndex == RemovedNode.NodeConnectionCount - 1 ? RemovedNode[0] : RemovedNode[ConnIndex + 1]);
                // добавляем текущий узел в конец упорядоченного (против ч.с.) списка узлов сектора
                SectorPGNodeList.Add(CurrentConnNode);
                // удаляем ссылку на удаляемый узел из списка связей текущего узла
                CurrentConnNode.RemoveNodeConnection(RemovedNode);
                // добавляем в набор П связь, соединяющую текущий и следующий (узел, связанный с удаляемым следующей по отношению к текущей связью) узлы
                SuspiciousConnectionSet.AddConnection(CurrentConnNode, NextConnNode);
            }

            // триангуляция полученного сектора
            TriangulateGraphSector(SectorPGNodeList, SuspiciousConnectionSet);
        }*/


        /// <summary>
        /// триангуляция сектора graphSector графа
        /// </summary>
        /// <param name="graphSector"></param>
        /// <param name="SuspiciousConnectionSet"></param>
        // алгоритм триангуляции сектора на псевдоязыке:
        //
        // если в переданном для триангуляции секторе 3 узла, то сектор триангулировать не нужно - выход из алгоритма
        // 
        // цикл (по всем узлам триангулируемого сектора)
        // {
        //      берем текущий узел; называем его начальным узлом
        //      цикл по всем узлам из сектора начиная с начального + 2 и заканчивая начальным - 2 (список узлов сектора - циклический)
        //      {
        //          берем текущий узел (из внутреннего цикла); называем его конечным узлом
        //          
        //          если все узлы из сектора между начальным и конечным лежат с одной стороны
        //             а все узлы между конечным и начальным с другой, то
        //          {
        //              строим связь между начальным и конечным узлами
        //              добавляем эту связь в список подозрительных связей
        //
        //              триангулируем сектор, составленный из узлов между начальным и конечным узлами (вход в алгоритм триангуляции для нового сектора)
        //              триангулируем сектор, составленный из узлов между конечным и начальным узлами (вход в алгоритм триангуляции для нового сектора)
        //
        //              выход из алгоритма
        //          }
        //      }
        // }
        // 
        // если оказались в этой точке алгоритма, это означает что триангуляция сектора невозможна ... т.е. ошибка
        private void TriangulateGraphSector(List<PolyhedronGraphNode> graphSector, SuspiciousConnectionSetClass suspiciousConnectionSet)
        {
            // если в секторе 3 узла, то его триангулировать не нужно
            if (graphSector.Count == 3) return;

            // цикл (по всем узлам триангулируемого сектора)
            for (Int32 nodeSectorIndex = 0; nodeSectorIndex < graphSector.Count; nodeSectorIndex++)
            {
                //  берем текущий узел; называем его начальным узлом
                PolyhedronGraphNode startNode = graphSector[nodeSectorIndex];

                // цикл по всем узлам из сектора начиная с начального + 2 и заканчивая начальным - 2 список узлов сектора - циклический)
                // т.е. начиная с начального + 2, пока != начальный - 1 (чтобы начальный - 2 не выпал из рассмотрения)
                Int32 startSearchIndex = (nodeSectorIndex <= graphSector.Count - 3 ? nodeSectorIndex + 2 : nodeSectorIndex + 2 - graphSector.Count);
                Int32 finishSearchIndex = (nodeSectorIndex == 0 ? graphSector.Count - 1 : nodeSectorIndex - 1);
                for (Int32 currentSearchIndex = startSearchIndex; currentSearchIndex != finishSearchIndex; )
                {
                    // берем текущий узел (из внутреннего цикла); называем его конечным узлом
                    PolyhedronGraphNode finishNode = graphSector[currentSearchIndex];

                    Int32 testStartIndex;
                    Int32 testFinishIndex;

                    // проверяем все ли узлы из сектора между начальным и конечным лежат с одной стороны
                    Boolean IsStartFinishSectorCorrect = true;
                    Double mixedProductValue1 = Double.NaN;
                    testStartIndex = (nodeSectorIndex == graphSector.Count - 1 ? 0 : nodeSectorIndex + 1);
                    testFinishIndex = currentSearchIndex;
                    for (Int32 textIndex = testStartIndex; textIndex != testFinishIndex; )
                    {
                        // числовой критерий того, что все рассматриваемые узлы лежат с одной стороны
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

                    // проверяем все ли узлы из сектора между конечным и начальным лежат с другой стороны
                    Boolean IsFinishStartSectorCorrect = true;
                    Double mixedProductValue2 = Double.NaN;
                    testStartIndex = (currentSearchIndex == graphSector.Count - 1 ? 0 : currentSearchIndex + 1);
                    testFinishIndex = nodeSectorIndex;
                    for (Int32 textIndex = testStartIndex; textIndex != testFinishIndex; )
                    {
                        // числовой критерий того, что все рассматриваемые узлы лежат с другой стороны
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

                    // строим связь между начальным и конечным узлами
                    // ссылку на finishNode вставляем после ссылки на следующий в секторе узел
                    PolyhedronGraphNode startNextNode = (nodeSectorIndex == graphSector.Count - 1 ? graphSector[0] : graphSector[nodeSectorIndex + 1]);
                    startNode.InsertNodeConnection(startNode.GetConnectionIndex(startNextNode) + 1, finishNode);
                    // ссылку на startNode вставляем после ссылки на следующий в секторе узел
                    PolyhedronGraphNode finishNextNode = (currentSearchIndex == graphSector.Count - 1 ? graphSector[0] : graphSector[currentSearchIndex + 1]);
                    finishNode.InsertNodeConnection(finishNode.GetConnectionIndex(finishNextNode) + 1, startNode);

                    //  добавляем эту связь в список подозрительных связей
                    suspiciousConnectionSet.AddConnection(startNode, finishNode);

                    // триангулируем сектор, составленный из узлов между начальным и конечным узлами (вход в алгоритм триангуляции для нового сектора)
                    List<PolyhedronGraphNode> startFinishSector = new List<PolyhedronGraphNode>();
                    for (Int32 copyNodeIndex = nodeSectorIndex; copyNodeIndex != currentSearchIndex; )
                    {
                        startFinishSector.Add(graphSector[copyNodeIndex]);
                        copyNodeIndex = (copyNodeIndex == graphSector.Count - 1 ? 0 : copyNodeIndex + 1);
                    }
                    startFinishSector.Add(graphSector[currentSearchIndex]);
                    TriangulateGraphSector(startFinishSector, suspiciousConnectionSet);

                    // триангулируем сектор, составленный из узлов между конечным и начальным узлами (вход в алгоритм триангуляции для нового сектора)
                    List<PolyhedronGraphNode> finishStartSector = new List<PolyhedronGraphNode>();
                    for (Int32 copyNodeIndex = currentSearchIndex; copyNodeIndex != nodeSectorIndex; )
                    {
                        finishStartSector.Add(graphSector[copyNodeIndex]);
                        copyNodeIndex = (copyNodeIndex == graphSector.Count - 1 ? 0 : copyNodeIndex + 1);
                    }
                    finishStartSector.Add(graphSector[nodeSectorIndex]);
                    TriangulateGraphSector(finishStartSector, suspiciousConnectionSet);

                    // выход из алгоритма
                    return;

                    currentSearchIndex = (currentSearchIndex == graphSector.Count - 1 ? 0 : currentSearchIndex + 1);
                }
            }

            // если оказались в этой точке алгоритма, это означает 
            // что триангуляция сектора невозможна ... т.е. ошибка
            throw new Exception("!!!!!!!");
        }

        /// <summary>
        /// метод SolutionIteration являеься одной итерацией решения линейной дифференциальной игры
        /// </summary>
        private void SolutionIteration(Vector3D QiDirectingVector)
        {
            SuspiciousConnectionSetClass SuspiciousConnectionSet = GetSuspiciousConnectionsList(QiDirectingVector);

            while (SuspiciousConnectionSet.ConnectionCount > 0)
            {
                // получаем связь 1-2
                PolyhedronGraphNode[] CurrentConnPGNodes = SuspiciousConnectionSet.GetConnection(0);
                PolyhedronGraphNode PGNode1 = CurrentConnPGNodes[0];
                PolyhedronGraphNode PGNode2 = CurrentConnPGNodes[1];

                // номер связи 1-2 в списке связей 1-го узла
                Int32 Conn12Index = PGNode1.GetConnectionIndex(PGNode2);

                // узел 3; связь 1-3 предыдущая по отношению к связи 1-2
                PolyhedronGraphNode PGNode3 = (Conn12Index == 0 ? PGNode1[PGNode1.NodeConnectionCount - 1] : PGNode1[Conn12Index - 1]);
                // узел 4; связь 1-4 следующая по отношению к связи 1-2
                PolyhedronGraphNode PGNode4 = (Conn12Index == PGNode1.NodeConnectionCount - 1 ? PGNode1[0] : PGNode1[Conn12Index + 1]);

                // решение системы лин. уравнений (3x3), используемое для проверки связи 1-2 на локальную выпуклость (см. алгоритм)
                Matrix Cone123SystemSolution = SolveCone123EquationSystem(PGNode1.NodeNormal, PGNode2.NodeNormal, PGNode3.NodeNormal);
                // проверка связи 1-2 на локальную выпуклость
                Double ConeVector4FuncFiValue = FuncFi(PGNode4.NodeNormal, m_PolyhedronVertexList, m_Pi1Set, m_Pi2Set, m_Qi1Set, m_Qi2Set);
                Double LocalConvexCriterionValue = Cone123SystemSolution[1, 1] * PGNode4.NodeNormal.XCoord +
                                                   Cone123SystemSolution[2, 1] * PGNode4.NodeNormal.YCoord +
                                                   Cone123SystemSolution[3, 1] * PGNode4.NodeNormal.ZCoord;
                // если связь выпукла
                // LocalConvexCriterionValue<=ConeVector4FuncFiValue, или что то же самое ConeVector4FuncFiValue-LocalConvexCriterionValue>=0
                if (ConeVector4FuncFiValue-LocalConvexCriterionValue >= -Epsilon)
                {
                    SuspiciousConnectionSet.RemoveConnection(0);
                }
                // если связь не выпукла
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
                        // связь 1-2 заменяем на связь 3-4 ...
                        ReplaceConn12WithConn34(PGNode1, PGNode2, PGNode3, PGNode4, SuspiciousConnectionSet);
                    }
                    // Lambda1>0 && Lambda2<=0
                    if ((Lambda1 > Epsilon) && (Lambda2 <= Epsilon))
                    {
                        // удаляем узел 1 ...
                        //RemoveNode1(PGNode1, PGNode2, PGNode3, PGNode4, SuspiciousConnectionSet);
                        RemoveNode(PGNode1, SuspiciousConnectionSet);
                    }
                    // Lambda1<=0 && Lambda2>0
                    if ((Lambda1 <= Epsilon) && (Lambda2 > Epsilon))
                    {
                        // удаляем узел 2 ...
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
        /// метод GetPlanesCrossingPoint возвращает точку пересечения трех плоскостей Plane1, Plane2 и Plane3
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

            // координаты точки пересечения округляем до PointCoordDigits знаков после запятой
            // это нужно чтобы исключить погрешности округления
            Double XCoord = Math.Round(SolutionMatrix[1, 1], PointCoordDigits);
            Double YCoord = Math.Round(SolutionMatrix[2, 1], PointCoordDigits);
            Double ZCoord = Math.Round(SolutionMatrix[3, 1], PointCoordDigits);

            return new Point3D(XCoord, YCoord, ZCoord);
        }*/

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

            // координаты точки пересечения округляем до PointCoordDigits знаков после запятой
            // это нужно чтобы исключить погрешности округления
            Double XCoord = Math.Round(Solution[1, 1], PointCoordDigits);
            Double YCoord = Math.Round(Solution[2, 1], PointCoordDigits);
            Double ZCoord = Math.Round(Solution[3, 1], PointCoordDigits);

            return new Point3D(XCoord, YCoord, ZCoord);

            /*// пока используем метод Крамера ... потом заменим
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

            return new Point3D(XCoord, YCoord, ZCoord);*/
        }

        /// <summary>
        /// прореживатель получаемого графа
        /// </summary>
        private void GraphChopper()
        {
        }

        /// <summary>
        /// метод FindVertexOnPoint ищет вершину (по заданной точке) в списке вершин m_PolyhedronVertexList
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
        /// метод RecievePolyhedronStructureFromGraph получает (восстанавливает) структуру многогранника из его графа
        /// </summary>
        private void RecievePolyhedronStructureFromGraph()
        {
            List<PlaneClass> PolyhedronPlaneList = new List<PlaneClass>(m_PGNodeList.Count);

            // каждому узлу графа соответствует грань
            // для "восстановления" этой грани каждому узлу графу сопостовляем плоскость, в которой лежит искомая грань
            for (Int32 PGNodeIndex = 0; PGNodeIndex < m_PGNodeList.Count; PGNodeIndex++)
            {
                PolyhedronGraphNode PGNode = m_PGNodeList[PGNodeIndex];
                // после удаления части узлов ID узла может <> Index узла + 1
                PGNode.ID = PGNodeIndex + 1;

                Vector3D PlaneNormal = PGNode.NodeNormal;
                Double SupportFuncValue = FuncFi(PlaneNormal, m_PolyhedronVertexList, m_Pi1Set, m_Pi2Set, m_Qi1Set, m_Qi2Set);

                PolyhedronPlaneList.Add(new PlaneClass(PlaneNormal, SupportFuncValue));
            }

            // формируем список вершин и граней многогранника заново
            m_PolyhedronSideList.Clear();
            m_PolyhedronVertexList.Clear();

            for (Int32 PGNodeIndex = 0; PGNodeIndex < m_PGNodeList.Count; PGNodeIndex++)
            {
                PolyhedronGraphNode CurrentPGNode = m_PGNodeList[PGNodeIndex];
                // CurrentPlane соответствует CurrentPGNode
                // плоскость номер 1
                PlaneClass CurrentPlane = PolyhedronPlaneList[PGNodeIndex];
                SideClass CurrentSide = new SideClass(new List<VertexClass>(), CurrentPGNode.ID, CurrentPGNode.NodeNormal);
                m_PolyhedronSideList.Add(CurrentSide);

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
        /// метод InitAlgorithmWork инициализирует работу алгоритма
        /// </summary>
        private void InitAlgorithmWork()
        {
            RecievePolyhedronStructure();
            RecievePolyhedronGraph();
            GraphTriangulation();
        }

        /*/// <summary>
        /// метод GetAMatrix возвращает матрицу A (см. алгоритм)
        /// </summary>
        /// <returns></returns>
        private Matrix GetAMatrix()
        {
            // по идее матрицу A надо получать из файла
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
        /// метод GetB1Matrix возвращает матрицу (столбец) B1 (см. алгоритм)
        /// </summary>
        /// <returns></returns>
        private Matrix GetB1Matrix()
        {
            // по идее матрицу B1 надо получать из файла
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
        /// метод GetB2Matrix возвращает матрицу (столбец) B2 (см. алгоритм)
        /// </summary>
        /// <returns></returns>
        private Matrix GetB2Matrix()
        {
            // по идее матрицу B2 надо получать из файла
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
        /// метод GetC1Matrix возвращает матрицу (столбец) C1 (см. алгоритм)
        /// </summary>
        /// <returns></returns>
        private Matrix GetC1Matrix()
        {
            // по идее матрицу C1 надо получать из файла
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
        /// метод GetC2Matrix возвращает матрицу (столбец) C2 (см. алгоритм)
        /// </summary>
        /// <returns></returns>
        private Matrix GetC2Matrix()
        {
            // по идее матрицу C2 надо получать из файла
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
        /// метод GetD1Matrix возвращает матрицу (столбец) D1 (см. алгоритм)
        /// </summary>
        /// <param name="InverseTime">обратное время = время окончания - текущее время</param>
        /// <returns></returns>
        private Matrix GetD1Matrix(Double InverseTime)
        {
            return m_FundKoshiMatrix.FundKoshiMatrixCalc(InverseTime) * GetB1Matrix();
        }

        /// <summary>
        /// метод GetD2Matrix возвращает матрицу (столбец) D2 (см. алгоритм)
        /// </summary>
        /// <param name="InverseTime">обратное время = время окончания - текущее время</param>
        /// <returns></returns>
        private Matrix GetD2Matrix(Double InverseTime)
        {
            return m_FundKoshiMatrix.FundKoshiMatrixCalc(InverseTime) * GetB2Matrix();
        }

        /// <summary>
        /// метод GetE1Matrix возвращает матрицу (столбец) E1 (см. алгоритм)
        /// </summary>
        /// <param name="InverseTime">обратное время = время окончания - текущее время</param>
        /// <returns></returns>
        private Matrix GetE1Matrix(Double InverseTime)
        {
            return m_FundKoshiMatrix.FundKoshiMatrixCalc(InverseTime) * GetC1Matrix();
        }

        /// <summary>
        /// метод GetE2Matrix возвращает матрицу (столбец) E2 (см. алгоритм)
        /// </summary>
        /// <param name="InverseTime">обратное время = время окончания - текущее время</param>
        /// <returns></returns>
        private Matrix GetE2Matrix(Double InverseTime)
        {
            return m_FundKoshiMatrix.FundKoshiMatrixCalc(InverseTime) * GetC2Matrix();
        }*/

        /// <summary>
        /// метод GetDMatrix возвращает матрицу (столбец) D (см. алгоритм)
        /// </summary>
        /// <param name="InverseTime">обратное время = время окончания - текущее время</param>
        /// <returns></returns>
        private Matrix GetDMatrix(Double InverseTime)
        {
            return m_FundKoshiMatrix.FundKoshiMatrixCalc(InverseTime) * GetBMatrix();
        }

        /// <summary>
        /// метод GetEMatrix возвращает матрицу (столбец) E (см. алгоритм)
        /// </summary>
        /// <param name="InverseTime">обратное время = время окончания - текущее время</param>
        /// <returns></returns>
        private Matrix GetEMatrix(Double InverseTime)
        {
            return m_FundKoshiMatrix.FundKoshiMatrixCalc(InverseTime) * GetCMatrix();
        }

        /// <summary>
        /// метод GetPolyhedronGraphDescription возвращает в текстовом виде (в виде массива строк) описание графа многогранника
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
                    // полярный угол !!! 
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

            // m_PGNodeList.Count - кол-во узлов => m_PGNodeList.Count-1 - индекс последнего узла
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

                    // удаляем узел 2, если он связан с узлом 1 и угол между ними < MinVectorDistinguishAngle
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
        /// метод GetAMatrix возвращает матрицу A (см. алгоритм)
        /// </summary>
        /// <returns></returns>
        public Matrix GetAMatrix()
        {
            // по идее матрицу A надо получать из файла
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
        /// метод GetBMatrix возвращает матрицу (столбец) B (см. алгоритм)
        /// </summary>
        /// <returns></returns>
        public Matrix GetBMatrix()
        {
            // по идее матрицу B надо получать из файла
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
        /// метод GetCMatrix возвращает матрицу (столбец) C (см. алгоритм)
        /// </summary>
        /// <returns></returns>
        public Matrix GetCMatrix()
        {
            // по идее матрицу C надо получать из файла
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
