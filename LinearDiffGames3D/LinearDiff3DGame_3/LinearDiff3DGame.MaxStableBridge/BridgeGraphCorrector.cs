using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.Geometry3D;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.MaxStableBridge
{
    /*/// <summary>
    /// класс для овыпукления функции fi_i (и построения в конечном итоге множества W(i+1))
    /// </summary>
    internal class BridgeGraphCorrector
    {
        /// <summary>
        /// конструктор класса BridgeGraphCorrector
        /// </summary>
        /// <param name="approxComparer">сравниватель для приближенного сравнения действительных чисел</param>
        public BridgeGraphCorrector(ApproxComp approxComparer)
        {
            m_ApproxComparer = approxComparer;
        }

        /// <summary>
        /// метод CheckAndCorrectBridgeGraph проводит процедуру овыпукления функции fi_i
        /// </summary>
        /// <param name="connSet">список связей П</param>
        /// <param name="graph">граф (исходная функция fi)</param>
        /// <returns>выпуклая оболочка функции fi_i</returns>
        public Polyhedron3DGraph CheckAndCorrectBridgeGraph(SuspiciousConnectionSet connSet, Polyhedron3DGraph graph)
        {
            LESKramer3Solver solver = new LESKramer3Solver();

            // Цикл пока набор П не пуст
            while (connSet.Count > 0)
            {
                // связь 1-2
                Polyhedron3DGraphNode[] conn12 = connSet[0];
                Polyhedron3DGraphNode node1 = conn12[0];
                Polyhedron3DGraphNode node2 = conn12[1];

                // узел 3; связь 1-3 предыдущая по отношению к связи 1-2
                Polyhedron3DGraphNode node3 = node1.ConnectionList.GetPrevItem(node2);
                // узел 4; связь 1-4 следующая по отношению к связи 1-2
                Polyhedron3DGraphNode node4 = node1.ConnectionList.GetNextItem(node2);

                // решение системы лин. уравнений (3x3), используемое для проверки связи 1-2 на локальную выпуклость (см. алгоритм)
                Matrix cone123Solution = SolveCone123EquationSystem(solver, node1, node2, node3);
                // проверка связи 1-2 на локальную выпуклость
                Double localConvexCriterion = cone123Solution[1, 1] * node4.NodeNormal.XCoord +
                                              cone123Solution[2, 1] * node4.NodeNormal.YCoord +
                                              cone123Solution[3, 1] * node4.NodeNormal.ZCoord;
                // если связь выпукла
                if (m_ApproxComparer.LE(localConvexCriterion, node4.SupportFuncValue))
                {
                    connSet.RemoveConnection(0);
                }
                // если связь не выпукла
                else
                {
                    Matrix matrixError = null;
                    Matrix lambda123 = CalcLambda123(solver, node1.NodeNormal, node2.NodeNormal, node3.NodeNormal, node4.NodeNormal, out matrixError);
                    Double lambda1 = lambda123[1, 1];
                    Double lambda2 = lambda123[2, 1];
                    Double lambda3 = lambda123[3, 1];

                    // lambda3 must be < 0
                    if (m_ApproxComparer.GE(lambda3, 0))
                    {
#warning может более специализированное исключение
                        //throw new Exception("Lambda3 must be < 0");
                    }

                    // Lambda1>0 && Lambda2>0
                    if (m_ApproxComparer.GT(lambda1, 0) && m_ApproxComparer.GT(lambda2, 0))
                    {
                        ReplaceConn12Conn34(connSet, node1, node2, node3, node4);
                    }
                    // Lambda1>0 && Lambda2<=0
                    if (m_ApproxComparer.GT(lambda1, 0) && m_ApproxComparer.LE(lambda2, 0))
                    {
                        // удаляем узел 1
                        RemoveNode(connSet, node1, graph);
                    }
                    // Lambda1<=0 && Lambda2>0
                    if (m_ApproxComparer.LE(lambda1, 0) && m_ApproxComparer.GT(lambda2, 0))
                    {
                        // удаляем узел 2
                        RemoveNode(connSet, node2, graph);
                    }
                    // Lambda1<=0 && Lambda2<=0
                    if (m_ApproxComparer.LE(lambda1, 0) && m_ApproxComparer.LE(lambda2, 0))
                    {
                        // W(i+1) = 0
#warning может более специализированное исключение
                        throw new Exception("W(i+1)=0. Solution does not exist !!!");
                    }
                }
            }
            // Цикл пока набор П не пуст

            return graph;
        }

        /// <summary>
        /// метод SolveCone123EquationSystem решает систему уравнений ls*y = ksi(ls)
        /// См. статью "Численное решение дифференциальной игры наведения третьего порядка" Зарх М.А., Пацко В.С.
        /// </summary>
        /// <param name="solver">объект для решения СЛАУ 3x3</param>
        /// <param name="node1">узел 1</param>
        /// <param name="node2">узел 2</param>
        /// <param name="node3">узел 3</param>
        /// <returns>решение системы уравнений ls*y = ksi(ls)</returns>
        private Matrix SolveCone123EquationSystem(LESKramer3Solver solver, Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2, Polyhedron3DGraphNode node3)
        {
            Matrix matrixA = new Matrix(3, 3);
            matrixA[1, 1] = node1.NodeNormal.XCoord;
            matrixA[1, 2] = node1.NodeNormal.YCoord;
            matrixA[1, 3] = node1.NodeNormal.ZCoord;
            matrixA[2, 1] = node2.NodeNormal.XCoord;
            matrixA[2, 2] = node2.NodeNormal.YCoord;
            matrixA[2, 3] = node2.NodeNormal.ZCoord;
            matrixA[3, 1] = node3.NodeNormal.XCoord;
            matrixA[3, 2] = node3.NodeNormal.YCoord;
            matrixA[3, 3] = node3.NodeNormal.ZCoord;

            Matrix matrixB = new Matrix(3, 1);
            matrixB[1, 1] = node1.SupportFuncValue;
            matrixB[2, 1] = node2.SupportFuncValue;
            matrixB[3, 1] = node3.SupportFuncValue;

            Matrix matrixError = null;

            Matrix solution = solver.Solve(matrixA, matrixB, out matrixError);
            return solution;
        }

        /// <summary>
        /// метод CalcLambda123 решает систему уравнений l4 = lambda1*l1 + lambda2*l2 + lambda3*l3
        /// См. статью "Численное решение дифференциальной игры наведения третьего порядка" Зарх М.А., Пацко В.С.
        /// </summary>
        /// <param name="solver">объект для решения СЛАУ 3x3</param>
        /// <param name="coneVector1">вектор, связанный с узлом 1</param>
        /// <param name="coneVector2">вектор, связанный с узлом 2</param>
        /// <param name="coneVector3">вектор, связанный с узлом 3</param>
        /// <param name="coneVector4">вектор, связанный с узлом 4</param>
        /// <returns>решение системы уравнений l4 = lambda1*l1 + lambda2*l2 + lambda3*l3</returns>
        private Matrix CalcLambda123(LESKramer3Solver solver, Vector3D coneVector1, Vector3D coneVector2, Vector3D coneVector3, Vector3D coneVector4)
        {
            Matrix matrixA = new Matrix(3, 3);
            matrixA[1, 1] = coneVector1.XCoord;
            matrixA[1, 2] = coneVector2.XCoord;
            matrixA[1, 3] = coneVector3.XCoord;
            matrixA[2, 1] = coneVector1.YCoord;
            matrixA[2, 2] = coneVector2.YCoord;
            matrixA[2, 3] = coneVector3.YCoord;
            matrixA[3, 1] = coneVector1.ZCoord;
            matrixA[3, 2] = coneVector2.ZCoord;
            matrixA[3, 3] = coneVector3.ZCoord;

            Matrix matrixB = new Matrix(3, 1);
            matrixB[1, 1] = coneVector4.XCoord;
            matrixB[2, 1] = coneVector4.YCoord;
            matrixB[3, 1] = coneVector4.ZCoord;

            Matrix matrixError = null;

            Matrix solution = solver.Solve(matrixA, matrixB, out matrixError);
            return solution;
        }

        private Matrix CalcLambda123(LESKramer3Solver solver, Vector3D coneVector1, Vector3D coneVector2, Vector3D coneVector3, Vector3D coneVector4, out Matrix matrixError)
        {
            Matrix matrixA = new Matrix(3, 3);
            matrixA[1, 1] = coneVector1.XCoord;
            matrixA[1, 2] = coneVector2.XCoord;
            matrixA[1, 3] = coneVector3.XCoord;
            matrixA[2, 1] = coneVector1.YCoord;
            matrixA[2, 2] = coneVector2.YCoord;
            matrixA[2, 3] = coneVector3.YCoord;
            matrixA[3, 1] = coneVector1.ZCoord;
            matrixA[3, 2] = coneVector2.ZCoord;
            matrixA[3, 3] = coneVector3.ZCoord;

            Matrix matrixB = new Matrix(3, 1);
            matrixB[1, 1] = coneVector4.XCoord;
            matrixB[2, 1] = coneVector4.YCoord;
            matrixB[3, 1] = coneVector4.ZCoord;

            matrixError = null;

            Matrix solution = solver.Solve(matrixA, matrixB, out matrixError);
            return solution;
        }

        /// <summary>
        /// метод ReplaceConn12Conn34 заменяет связь 1-2 на связь 3-4 (См. статью "Численное решение дифференциальной игры наведения третьего порядка" Зарх М.А., Пацко В.С.)
        /// </summary>
        /// <param name="connSet">список связей П</param>
        /// <param name="node1">узел 1</param>
        /// <param name="node2">узел 2</param>
        /// <param name="node3">узел 3</param>
        /// <param name="node4">узел 4</param>
        private void ReplaceConn12Conn34(SuspiciousConnectionSet connSet, Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2, Polyhedron3DGraphNode node3, Polyhedron3DGraphNode node4)
        {
            // удаляем связь на узел 2 из списка связей узла 1 (аналогично для узла 2)
            node1.ConnectionList.Remove(node2);
            node2.ConnectionList.Remove(node1);

            // в список связей узла 3, после ссылки на узел 2, вставляем ссылку на узел 4
            node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(node2) + 1, node4);
            // в список связей узла 4, после ссылки на узел 1, вставляем ссылку на узел 3
            node4.ConnectionList.Insert(node4.ConnectionList.IndexOf(node1) + 1, node3);

            // из списка связей П удаляем связь 1-2
            connSet.RemoveConnection(node1, node2);
            // добавляем в набор П связи: 1-3, 1-4, 2-3, 2-4 (те из них, которых в наборе П нет)
            connSet.AddConnection(node1, node3);
            connSet.AddConnection(node1, node4);
            connSet.AddConnection(node2, node3);
            connSet.AddConnection(node2, node4);
        }

        /// <summary>
        /// метод RemoveNode удаляет узел removedNode из графа graph, модифицирует (триангулирует) граф graph, модифицирует список связей П
        /// </summary>
        /// <param name="connSet">список связей П</param>
        /// <param name="removedNode">удаляемый узел</param>
        /// <param name="graph">граф</param>
        private void RemoveNode(SuspiciousConnectionSet connSet, Polyhedron3DGraphNode removedNode, Polyhedron3DGraph graph)
        {
            // список узлов, образующих контур сектора K*
            CyclicList<Polyhedron3DGraphNode> sectorNodeList = new CyclicList<Polyhedron3DGraphNode>();

            // Цикл (по всем связям удаляемого узла)
            for (Int32 connIndex = 0; connIndex < removedNode.ConnectionList.Count; ++connIndex)
            {
                // Текущим узлом становится узел, связанный с удаляемым узлом текущей связью
                Polyhedron3DGraphNode currentConn = removedNode.ConnectionList[connIndex];
                // Заносим текущий узел в список узлов, образующих контур сектора K*
                sectorNodeList.Add(currentConn);
                // Из списка связей текущего узла удаляем ссылку на удаляемый узел
                currentConn.ConnectionList.Remove(removedNode);

            }
            // Цикл (по всем связям удаляемого узла)

            // Цикл (по всем узлам из списка узлов образующих контур сектора)
            for (Int32 nodeIndex = 0; nodeIndex < sectorNodeList.Count; ++nodeIndex)
            {
                Polyhedron3DGraphNode currentNode = sectorNodeList[nodeIndex];
                Polyhedron3DGraphNode nextNode = sectorNodeList.GetNextItem(nodeIndex);

                // проверка на то, что currentNode - nextNode на самом деле связь
#warning может нужна настоящая проверка ???
                Debug.Assert(currentNode.ConnectionList.IndexOf(nextNode) != -1, "nextNode must be in currentNode.ConnectionList");
                Debug.Assert(nextNode.ConnectionList.IndexOf(currentNode) != -1, "currentNode must be in nodeNode.ConnectionList");

                // Связь текущий узел – следующий узел заносим в набор П
                connSet.AddConnection(currentNode, nextNode);

            }
            // Цикл (по всем узлам из списка узлов образующих контур сектора)

            // Удаляем все связи, содержащих удаляемый узел, из набора П
            connSet.RemoveConnections(removedNode);
            // Удаляем удаляемый узел из списка узлов графа
            graph.NodeList.Remove(removedNode);

            // триангуляция полученного сектора
            if (sectorNodeList.Count > 3)
            {
                SectorTriangulation(connSet, sectorNodeList);
            }
        }

        /// <summary>
        /// метод SectorTriangulation триангулирует сектор, заданный своим контуром sectorNodeList (узлы в контуре идут упорядоченными против ч.с. ...)
        /// </summary>
        /// <param name="connSet">список связей П</param>
        /// <param name="sectorNodeList">триангулируемый сектор</param>
        private void SectorTriangulation(SuspiciousConnectionSet connSet, ICyclicList<Polyhedron3DGraphNode> sectorNodeList)
        {
            Debug.Assert(sectorNodeList.Count > 3, "sectorNodeList.Count must be greater than 3");

            // флаг, показывающий была ли построена хотя бы одна связь
            Boolean isNewConnBuild = false;

            // Цикл (по всем узлам из списка узлов образующих контур сектора)
            for (Int32 nodeIndex = 0; nodeIndex < sectorNodeList.Count; ++nodeIndex)
            {
                // Узлом O назовем текущий узел из списка узлов
                Polyhedron3DGraphNode nodeO = sectorNodeList[nodeIndex];
                // Узлом L назовем предыдущий узел из списка узлов относительно текущего
                Polyhedron3DGraphNode nodeL = sectorNodeList.GetPrevItem(nodeIndex);
                // Узлом R назовем следующий узел из списка узлов относительно текущего
                Polyhedron3DGraphNode nodeR = sectorNodeList.GetNextItem(nodeIndex);

                // В качестве начала обхода контура берем связь узел O – узел R
                Polyhedron3DGraphNode startNode = nodeR;
                // Цикл (по узлам списка, начиная со следующего узла относительно узла R и заканчивая предыдущим узлом относительно узла L)
                for (Polyhedron3DGraphNode currentNode = sectorNodeList.GetNextItem(nodeR);
                    !Object.ReferenceEquals(currentNode, nodeL);
                    currentNode = sectorNodeList.GetNextItem(currentNode))
                {
                    if (IsNewConnValid(sectorNodeList, nodeO, currentNode))
                    {
                        // можем построить новую связь => устанавливаем флаг
                        isNewConnBuild = true;

                        // В список связей узла O, перед связью с узлом L добавляем ссылку на текущий узел
                        nodeO.ConnectionList.Insert(nodeO.ConnectionList.IndexOf(nodeL), currentNode);
                        // В список связей текущего узла, перед связью с узлом, из которого мы пришли в текущий, добавляем ссылку на узел O
                        Polyhedron3DGraphNode prevNode = sectorNodeList.GetPrevItem(currentNode);
                        currentNode.ConnectionList.Insert(currentNode.ConnectionList.IndexOf(prevNode), nodeO);

                        // Связь узел O – текущий узел добавляем в набор П
                        connSet.AddConnection(nodeO, currentNode);

                        // Узлы из списка узлов, начиная с узла O (с начала обхода) и  заканчивая текущим узлом, образуют сектор
                        CyclicList<Polyhedron3DGraphNode> sector = new CyclicList<Polyhedron3DGraphNode>();
                        sector.Add(nodeO);
                        for (Polyhedron3DGraphNode sectorNode = startNode;
                            !Object.ReferenceEquals(sectorNode, currentNode);
                            sectorNode = sectorNodeList.GetNextItem(sectorNode))
                        {
                            sector.Add(sectorNode);
                        }
                        sector.Add(currentNode);

                        // Если количество узлов в этом секторе больше 3, триангулируем этот сектор
                        if (sector.Count > 3)
                        {
                            SectorTriangulation(connSet, sector);
                        }

                        // В качестве начала обхода контура берем связь узел O – текущий узел
                        startNode = currentNode;
                    }
                }
                // Цикл (по узлам списка, начиная со следующего узла относительно узла R и заканчивая предыдущим узлом относительно узла L)

                // Если для данного узла O была построена хотя бы одна связь
                if (isNewConnBuild)
                {
                    // Узлы из списка узлов, начиная с узла O (с начала обхода) и  заканчивая узлом L, образуют сектор
                    CyclicList<Polyhedron3DGraphNode> sector = new CyclicList<Polyhedron3DGraphNode>();
                    sector.Add(nodeO);
                    for (Polyhedron3DGraphNode sectorNode = startNode;
                        !Object.ReferenceEquals(sectorNode, nodeL);
                        sectorNode = sectorNodeList.GetNextItem(sectorNode))
                    {
                        sector.Add(sectorNode);
                    }
                    sector.Add(nodeL);

                    // Если количество узлов в этом секторе больше 3, триангулируем этот сектор
                    if (sector.Count > 3)
                    {
                        SectorTriangulation(connSet, sector);
                    }

                    // Конец работы алгоритма
                    return;
                }
            }
            // Цикл (по всем узлам из списка узлов образующих контур сектора)

            // Если при проходе по циклу не было построено ни одной связи, то это ошибка
#warning может более специализированное исключение
            throw new Exception("Incorrect triangulation work !!!");
        }

        /// <summary>
        /// метод IsNewConnValid определяет может ли быть построена связь между узлами node1 и node2 внутри сектора sectorNodeList
        /// </summary>
        /// <param name="sectorNodeList">сектор внутри которого мы хотим построить новую связь</param>
        /// <param name="node1">узел 1 новой связи</param>
        /// <param name="node2">узел 2 новой связи</param>
        /// <returns>true, если связь между узлами node1 и node2 внутри сектора sectorNodeList может быть построена; иначе - false</returns>
        private Boolean IsNewConnValid(ICyclicList<Polyhedron3DGraphNode> sectorNodeList, Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2)
        {
            // Строим нормаль плоскости, в которой лежит проверяемая связь как векторное произведение первого узла связи на второй
            Vector3D planeNormal = Vector3D.VectorProduct(node1.NodeNormal, node2.NodeNormal);

            // Цикл (по узлам списка, начиная со следующего узла относительно узла 1 и заканчивая предыдущим узлом относительно узла 2)
            for (Polyhedron3DGraphNode currentNode = sectorNodeList.GetNextItem(node1);
                !Object.ReferenceEquals(currentNode, node2);
                currentNode = sectorNodeList.GetNextItem(currentNode))
            {
                // Считаем скалярное произведение вектора, связанного с текущим узлом, и полученного вектора нормали
                Double scalarProduct = currentNode.NodeNormal * planeNormal;

                // Если скалярное произведение больше или равно 0
                if (m_ApproxComparer.GE(scalarProduct, 0))
                {
                    // Строим компоненту вектора, связанного с текущим узлом, которая параллельна плоскости связи (назовем ее lp)
                    Vector3D parallelComp = currentNode.NodeNormal.GetPerpendicularComponent(planeNormal);

                    // если вектор lp лежит между векторами l1 и l2
                    // новая связь не может быть построена
                    if (IsVectorBetweenVectors12(parallelComp, node1.NodeNormal, node2.NodeNormal)) return false;
                }

            }
            // Цикл (по узлам списка, начиная со следующего узла относительно узла 1 и заканчивая предыдущим узлом относительно узла 2)

            // Цикл (по узлам списка, начиная со следующего узла относительно узла 2 и заканчивая предыдущим узлом относительно узла 1)
            for (Polyhedron3DGraphNode currentNode = sectorNodeList.GetNextItem(node2);
                !Object.ReferenceEquals(currentNode, node1);
                currentNode = sectorNodeList.GetNextItem(currentNode))
            {
                // Считаем скалярное произведение вектора, связанного с текущим узлом, и полученного вектора нормали
                Double scalarProduct = currentNode.NodeNormal * planeNormal;

                // Если скалярное произведение меньше или равно 0
                if (m_ApproxComparer.LE(scalarProduct, 0))
                {
                    // Строим компоненту вектора, связанного с текущим узлом, которая параллельна плоскости связи (назовем ее lp)
                    Vector3D parallelComp = currentNode.NodeNormal.GetPerpendicularComponent(planeNormal);

                    // если вектор lp лежит между векторами l1 и l2
                    // новая связь не может быть построена
                    if (IsVectorBetweenVectors12(parallelComp, node2.NodeNormal, node1.NodeNormal)) return false;
                }
            }
            // Цикл (по узлам списка, начиная со следующего узла относительно узла 2 и заканчивая предыдущим узлом относительно узла 1)

            // новая связь может быть построена
            return true;
        }

        /// <summary>
        /// метод IsVectorBetweenVectors12 определяет лежит ли вектор vector между векторами vector1 и vector2
        /// вектора vector, vector1 и vector2 лежат в одной плоскости (это важно !!!!!)
        /// </summary>
        /// <param name="vector">проверяемый вектор</param>
        /// <param name="vector1">вектор 1</param>
        /// <param name="vector2">вектор 2</param>
        /// <returns>true, если вектор vector лежит между векторами vector1 и vector2; иначе - false</returns>
        private Boolean IsVectorBetweenVectors12(Vector3D vector, Vector3D vector1, Vector3D vector2)
        {
            // Решаем уравнение l = alpha*l1 + beta*l2

            // (l1, l):
            Double scalarProduct1 = vector1 * vector;
            // (l2, l):
            Double scalarProduct2 = vector2 * vector;
            // (l1, l2):
            Double scalarProduct12 = vector1 * vector2;
            // delta = 1 - (l1, l2)*(l1, l2)
            Double delta = 1 - scalarProduct12 * scalarProduct12;

            Double alpha = (scalarProduct1 - scalarProduct12 * scalarProduct2) / delta;
            Double beta = (scalarProduct2 - scalarProduct12 * scalarProduct1) / delta;

            // если alpha, beta >= 0 значит вектор l лежит между векторами l1 и l2
            return m_ApproxComparer.GE(alpha, 0) && m_ApproxComparer.GE(beta, 0);
        }

        /// <summary>
        /// сравниватель для приближенного сравнения действительных чисел
        /// </summary>
        private ApproxComp m_ApproxComparer;
    }*/
}
