using System;
using System.Collections.Generic;
using System.Diagnostics;
using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.LinearEquationsSet;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.SuspiciousConnections;

namespace LinearDiff3DGame.MaxStableBridge.Corrector
{
    internal class BridgeGraphCorrector
    {
        /// <summary>
        /// конструктор класса BridgeGraphCorrector
        /// </summary>
        /// <param name="approxComparer">сравниватель для приближенного сравнения действительных чисел</param>
        public BridgeGraphCorrector(ApproxComp approxComparer)
        {
            m_ApproxComparer = approxComparer;
            m_Solver = new LESKramer3Solver();
        }

        /// <summary>
        /// метод CheckAndCorrectBridgeGraph проводит процедуру овыпукления функции fi_i
        /// </summary>
        /// <param name="connSet">список связей П</param>
        /// <param name="graph">граф (исходная функция fi)</param>
        /// <returns>выпуклая оболочка функции fi_i</returns>
        public Polyhedron3DGraph CheckAndCorrectBridgeGraph(SuspiciousConnectionSet connSet, Polyhedron3DGraph graph)
        {
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

                /*// решение системы лин. уравнений (3x3), используемое для проверки связи 1-2 на локальную выпуклость (см. алгоритм)
                Matrix cone123Solution = SolveCone123EquationSystem(solver, node1, node2, node3);
                // проверка связи 1-2 на локальную выпуклость
                Double localConvexCriterion = cone123Solution[1, 1] * node4.NodeNormal.XCoord +
                                              cone123Solution[2, 1] * node4.NodeNormal.YCoord +
                                              cone123Solution[3, 1] * node4.NodeNormal.ZCoord;
                // если связь выпукла
                if (m_ApproxComparer.LE(localConvexCriterion, node4.SupportFuncValue))
                {
                    connSet.RemoveConnection(0);
                }*/
                // если связь выпукла
                if (CheckConnConvexity(node1, node2, node3, node4))
                {
                    connSet.RemoveConnection(0);
                }
                    // если связь не выпукла
                else
                {
                    Matrix matrixError;
                    Matrix lambda123 = CalcLambda123(node1.NodeNormal, node2.NodeNormal, node3.NodeNormal,
                                                     node4.NodeNormal, out matrixError);
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
        /// проверка связи 1-2 (в окружении 3, 4) на выпуклость
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <param name="node3"></param>
        /// <param name="node4"></param>
        /// <returns></returns>
        private Boolean CheckConnConvexity(Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2,
                                           Polyhedron3DGraphNode node3, Polyhedron3DGraphNode node4)
        {
            // решение системы лин. уравнений (3x3), используемое для проверки связи 1-2 на локальную выпуклость (см. алгоритм)
            Matrix cone123Solution = SolveCone123EquationSystem(node1, node2, node3);
            // проверка связи 1-2 на локальную выпуклость
            Double localConvexCriterion = cone123Solution[1, 1]*node4.NodeNormal.XCoord +
                                          cone123Solution[2, 1]*node4.NodeNormal.YCoord +
                                          cone123Solution[3, 1]*node4.NodeNormal.ZCoord;

            // if (localConvexCriterion <= node4.SupportFuncValue) то связь выпукла
            return m_ApproxComparer.LE(localConvexCriterion, node4.SupportFuncValue);
        }

        /// <summary>
        /// метод SolveCone123EquationSystem решает систему уравнений ls*y = ksi(ls)
        /// См. статью "Численное решение дифференциальной игры наведения третьего порядка" Зарх М.А., Пацко В.С.
        /// </summary>
        /// <param name="node1">узел 1</param>
        /// <param name="node2">узел 2</param>
        /// <param name="node3">узел 3</param>
        /// <returns>решение системы уравнений ls*y = ksi(ls)</returns>
        private Matrix SolveCone123EquationSystem(Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2,
                                                  Polyhedron3DGraphNode node3)
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

            Matrix matrixError;

            Matrix solution = m_Solver.Solve(matrixA, matrixB, out matrixError);
            return solution;
        }

        /// <summary>
        /// метод CalcLambda123 решает систему уравнений l4 = lambda1*l1 + lambda2*l2 + lambda3*l3
        /// См. статью "Численное решение дифференциальной игры наведения третьего порядка" Зарх М.А., Пацко В.С.
        /// </summary>
        /// <param name="coneVector1">вектор, связанный с узлом 1</param>
        /// <param name="coneVector2">вектор, связанный с узлом 2</param>
        /// <param name="coneVector3">вектор, связанный с узлом 3</param>
        /// <param name="coneVector4">вектор, связанный с узлом 4</param>
        /// <returns>решение системы уравнений l4 = lambda1*l1 + lambda2*l2 + lambda3*l3</returns>
        private Matrix CalcLambda123(Vector3D coneVector1, Vector3D coneVector2, Vector3D coneVector3,
                                     Vector3D coneVector4)
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

            Matrix matrixError;

            Matrix solution = m_Solver.Solve(matrixA, matrixB, out matrixError);
            return solution;
        }

        private Matrix CalcLambda123(Vector3D coneVector1, Vector3D coneVector2, Vector3D coneVector3,
                                     Vector3D coneVector4, out Matrix matrixError)
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

            Matrix solution = m_Solver.Solve(matrixA, matrixB, out matrixError);
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
        private void ReplaceConn12Conn34(SuspiciousConnectionSet connSet, Polyhedron3DGraphNode node1,
                                         Polyhedron3DGraphNode node2, Polyhedron3DGraphNode node3,
                                         Polyhedron3DGraphNode node4)
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
        private void RemoveNode(SuspiciousConnectionSet connSet, Polyhedron3DGraphNode removedNode,
                                Polyhedron3DGraph graph)
        {
            // список узлов, образующих контур сектора K*
            List<Polyhedron3DGraphNode> sectorNodeList = new List<Polyhedron3DGraphNode>();

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
                Debug.Assert(currentNode.ConnectionList.IndexOf(nextNode) != -1,
                             "nextNode must be in currentNode.ConnectionList");
                Debug.Assert(nextNode.ConnectionList.IndexOf(currentNode) != -1,
                             "currentNode must be in nodeNode.ConnectionList");

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
        private void SectorTriangulation(SuspiciousConnectionSet connSet,
                                         IList<Polyhedron3DGraphNode> sectorNodeList)
        {
            // Если количество узлов в секторе <= 3, то конец работы алгоритма
            if (sectorNodeList.Count <= 3) return;

            // Цикл (по всем узлам из списка узлов образующих контур сектора)
            for (Int32 sectorNodeIndex = 0; sectorNodeIndex < sectorNodeList.Count; ++sectorNodeIndex)
            {
                // текущий узел сектора
                Polyhedron3DGraphNode currentNode = sectorNodeList[sectorNodeIndex];
                // узел перед текущим узлом сектора
                Polyhedron3DGraphNode beforeCurrentNode = sectorNodeList.GetPrevItem(currentNode);

                // Для текущего узла составляем список связей с видимыми (из текущего узла) узлами
                List<Polyhedron3DGraphNode> dirtyVisibleNodes = GetVisibleNodes(sectorNodeList, currentNode);
                /*// Удаляем из списка две связи с соседними узлами (удобнее ???)
                visibleNodes.RemoveAt(visibleNodes.Count - 1);
                visibleNodes.RemoveAt(0);*/
                // Если список связей == 2, переход к следующему узлу
                if (dirtyVisibleNodes.Count == 2) continue;

                List<Polyhedron3DGraphNode> visibleNodes = new List<Polyhedron3DGraphNode>();
                visibleNodes.Add(dirtyVisibleNodes[0]);
                // Цикл (по списку видимых связей)
                for (Int32 dirtyVisibleNodeIndex = 0;
                     dirtyVisibleNodeIndex < dirtyVisibleNodes.Count;
                     ++dirtyVisibleNodeIndex)
                {
                    // Если связь удовлетворяет условию (*), то ее добавляем в список видимых, проверенных (*) связей
                    if (CheckConnInSector(dirtyVisibleNodes, currentNode, dirtyVisibleNodes[dirtyVisibleNodeIndex]))
                    {
                        visibleNodes.Add(dirtyVisibleNodes[dirtyVisibleNodeIndex]);
                    }
                }
                // Цикл (по списку видимых связей)
                visibleNodes.Add(dirtyVisibleNodes[dirtyVisibleNodes.Count - 1]);
                // Если список связей == 2, переход к следующему узлу
                if (visibleNodes.Count == 2) continue;

                // Цикл (по списку видимых, ?выпуклых? связей)
                for (Int32 visibleNodeIndex = 0; visibleNodeIndex < visibleNodes.Count; ++visibleNodeIndex)
                {
                    // текущий видимый узел (текущая видимая связь)
                    Polyhedron3DGraphNode currentVisibleNode = visibleNodes[visibleNodeIndex];

                    // Строим (реально) текущую связь из списка видимых связей
                    // проверить правильно ли ???
                    currentNode.ConnectionList.Insert(currentNode.ConnectionList.IndexOf(beforeCurrentNode),
                                                      currentVisibleNode);
                    currentVisibleNode.ConnectionList.Insert(
                        currentVisibleNode.ConnectionList.IndexOf(sectorNodeList.GetPrevItem(currentVisibleNode)),
                        currentNode);
                    // проверить правильно ли ???

                    // Добавляем построенную связь в набор П (нужно ли ???)                    
                    connSet.AddConnection(currentNode, currentVisibleNode);

                    // Отсекаем от триангулируемого сектора при помощи построенной связи сектор
                    List<Polyhedron3DGraphNode> subSector = new List<Polyhedron3DGraphNode>();
                    subSector.Add(currentNode);
                    for (Polyhedron3DGraphNode afterCurrentNode = sectorNodeList.GetNextItem(currentNode);
                         afterCurrentNode != currentVisibleNode;
                         afterCurrentNode = sectorNodeList.GetNextItem(currentNode))
                    {
                        subSector.Add(afterCurrentNode);
                        sectorNodeList.Remove(afterCurrentNode);
                    }
                    subSector.Add(currentVisibleNode);

                    // Если количество узлов отсеченного сектора > 3, то триангулируем его
                    if (subSector.Count > 3)
                    {
                        SectorTriangulation(connSet, subSector);
                    }
                }
                // Цикл (по списку видимых, ?выпуклых? связей)

                // если дошли сюда, то конец работы алгоритма
                return;
            }
            // Цикл (по всем узлам из списка узлов образующих контур сектора)

            // если дошли сюда, то триангуляцию сектора провести не смогли
#warning может более специализированное исключение
            throw new Exception("Sector triangulation failed !!!");
        }

        /// <summary>
        /// список видимых узлов сектора sectorNodeList из узла initialNode
        /// </summary>
        /// <param name="sectorNodeList"></param>
        /// <param name="initialNode"></param>
        /// <returns></returns>
        private List<Polyhedron3DGraphNode> GetVisibleNodes(IList<Polyhedron3DGraphNode> sectorNodeList,
                                                            Polyhedron3DGraphNode initialNode)
        {
            List<Polyhedron3DGraphNode> visibleNodes = new List<Polyhedron3DGraphNode>();

            // для единообразия
            Polyhedron3DGraphNode node1 = initialNode;
            Polyhedron3DGraphNode node2 = sectorNodeList.GetNextItem(node1);
            Polyhedron3DGraphNode nodem = sectorNodeList.GetPrevItem(node1);

            visibleNodes.Add(node2);
            for (Polyhedron3DGraphNode nodek = sectorNodeList.GetNextItem(node2);
                 nodek != nodem;
                 nodek = sectorNodeList.GetNextItem(nodek))
            {
                if (CheckNodeVisibility(sectorNodeList, initialNode, nodek)) visibleNodes.Add(nodek);
            }
            visibleNodes.Add(nodem);

            return visibleNodes;
        }

        /// <summary>
        /// проверка того, что в секторе sectorNodeList узел checkedNode виден из узла initialNode
        /// </summary>
        /// <param name="sectorNodeList"></param>
        /// <param name="initialNode"></param>
        /// <param name="checkedNode"></param>
        /// <returns></returns>
        private Boolean CheckNodeVisibility(IList<Polyhedron3DGraphNode> sectorNodeList,
                                            Polyhedron3DGraphNode initialNode, Polyhedron3DGraphNode checkedNode)
        {
            // для единообразия
            Polyhedron3DGraphNode node1 = initialNode;
            Polyhedron3DGraphNode nodek = checkedNode;

            // соотношение номер 1
            Polyhedron3DGraphNode node2 = sectorNodeList.GetNextItem(node1);
            Polyhedron3DGraphNode nodem = sectorNodeList.GetPrevItem(node1);

            Double mixedProdact12k = Vector3D.MixedProduct(nodek.NodeNormal, node1.NodeNormal, node2.NodeNormal);
            Double mixedProdact1mk = Vector3D.MixedProduct(nodek.NodeNormal, node1.NodeNormal, nodem.NodeNormal);

            // must be mixedProdact12k < 0, mixedProdact1mk > 0
            if (!m_ApproxComparer.LT(mixedProdact12k, 0) || !m_ApproxComparer.GT(mixedProdact1mk, 0)) return false;

            // соотношение номер 2
            for (Polyhedron3DGraphNode nodei = node2; nodei != nodem; nodei = sectorNodeList.GetNextItem(nodei))
            {
                Polyhedron3DGraphNode nodeip1 = sectorNodeList.GetNextItem(nodei);

                Vector3D crossingVector =
                    Vector3D.VectorProduct(Vector3D.VectorProduct(node1.NodeNormal, nodek.NodeNormal),
                                           Vector3D.VectorProduct(nodei.NodeNormal, nodeip1.NodeNormal));

                Double scalarProduct1 = Vector3D.ScalarProduct(node1.NodeNormal, crossingVector);
                Double scalarProductk = Vector3D.ScalarProduct(nodek.NodeNormal, crossingVector);
                Double scalarProduct1k = Vector3D.ScalarProduct(node1.NodeNormal, nodek.NodeNormal);

                Double scalarProducti = Vector3D.ScalarProduct(nodei.NodeNormal, crossingVector);
                Double scalarProductip1 = Vector3D.ScalarProduct(nodeip1.NodeNormal, crossingVector);
                Double scalarProductiip1 = Vector3D.ScalarProduct(nodei.NodeNormal, nodeip1.NodeNormal);

                Double lambda1 = (scalarProduct1 - scalarProductk*scalarProduct1k)/(1 - scalarProduct1k*scalarProduct1k);
                Double lambda2 = (scalarProductk - scalarProduct1*scalarProduct1k)/(1 - scalarProduct1k*scalarProduct1k);
                Double lambda3 = (scalarProducti - scalarProductip1*scalarProductiip1)/
                                 (1 - scalarProductiip1*scalarProductiip1);
                Double lambda4 = (scalarProductip1 - scalarProducti*scalarProductiip1)/
                                 (1 - scalarProductiip1*scalarProductiip1);

                // must be max(lambda1, lambda2, lambda3, lambda4) > 0
                if (!m_ApproxComparer.GT(GetMaxValue(lambda1, lambda2, lambda3, lambda4), 0)) return false;
                // must be min(lambda1, lambda2, lambda3, lambda4) < 0
                if (!m_ApproxComparer.LT(GetMinValue(lambda1, lambda2, lambda3, lambda4), 0)) return false;
            }

            return true;
        }

        [Obsolete("фигово выглядит и не тут должен быть имхо")]
        private Double GetMaxValue(Double value1, Double value2, Double value3, Double value4)
        {
            Double maxValue = value1;
            if (maxValue < value2) maxValue = value2;
            if (maxValue < value3) maxValue = value3;
            if (maxValue < value4) maxValue = value4;
            return maxValue;
        }

        [Obsolete("фигово выглядит и не тут должен быть имхо")]
        private Double GetMinValue(Double value1, Double value2, Double value3, Double value4)
        {
            Double minValue = value1;
            if (minValue > value2) minValue = value2;
            if (minValue > value3) minValue = value3;
            if (minValue > value4) minValue = value4;
            return minValue;
        }

        /// <summary>
        /// проверка связи 1k в секторе ... вобщем см. алгоритм ...
        /// </summary>
        /// <param name="visibleNodes"></param>
        /// <param name="initialNode"></param>
        /// <param name="checkedNode"></param>
        /// <returns></returns>
        private Boolean CheckConnInSector(List<Polyhedron3DGraphNode> visibleNodes, Polyhedron3DGraphNode initialNode,
                                          Polyhedron3DGraphNode checkedNode)
        {
            Int32 checkedNodeIndex = visibleNodes.IndexOf(checkedNode);

            // для единообразия
            Polyhedron3DGraphNode node1 = initialNode;
            Polyhedron3DGraphNode nodek = checkedNode;

            // visibleNodes[0] == l2, visibleNodes[visibleNodes.Count-1] == lm,
            for (Int32 nodei1Index = 0; nodei1Index < checkedNodeIndex; ++nodei1Index)
            {
                Polyhedron3DGraphNode nodei1 = visibleNodes[nodei1Index];

                for (Int32 nodei2Index = checkedNodeIndex + 1; nodei2Index < visibleNodes.Count; ++nodei2Index)
                {
                    Polyhedron3DGraphNode nodei2 = visibleNodes[nodei2Index];

                    // если связь 1k не выпукла в окружении i1, i2, то связь 1k проверку не прошла
                    if (!CheckConnConvexity(node1, nodek, nodei1, nodei2)) return false;
                }
            }

            return true;
        }

        /*/// <summary>
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
        }*/

        /*/// <summary>
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
        }*/

        /// <summary>
        /// сравниватель для приближенного сравнения действительных чисел
        /// </summary>
        private readonly ApproxComp m_ApproxComparer;

        /// <summary>
        /// объект для решения СЛАУ 3x3
        /// </summary>
        private readonly ILinearEquationsSystemSolver m_Solver;
    }
}