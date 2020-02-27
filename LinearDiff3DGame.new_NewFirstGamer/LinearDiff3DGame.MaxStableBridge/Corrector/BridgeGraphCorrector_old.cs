using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.LinearEquationsSet;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.SuspiciousConnections;

namespace LinearDiff3DGame.MaxStableBridge.Corrector
{
    public /*internal*/ sealed class BridgeGraphCorrector_old : IBridgeGraphCorrector
    {
        public BridgeGraphCorrector_old(ApproxComp approxComparer)
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
            while(connSet.Count > 0)
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
                if(CheckConnConvexity(node1, node2, node3, node4))
                    connSet.RemoveConnection(0);
                    // если связь не выпукла
                else
                {
                    Matrix matrixError;
                    Matrix lambda123 = CalcLambda123(node1.NodeNormal,
                                                     node2.NodeNormal,
                                                     node3.NodeNormal,
                                                     node4.NodeNormal,
                                                     out matrixError);
                    Double lambda1 = lambda123[1, 1];
                    Double lambda2 = lambda123[2, 1];
                    Double lambda3 = lambda123[3, 1];

                    // lambda3 must be < 0
                    if(m_ApproxComparer.GE(lambda3, 0))
                    {
#warning может более специализированное исключение
                        throw new Exception("Lambda3 must be < 0");
                    }

                    // Lambda1>0 && Lambda2>0
                    if(m_ApproxComparer.GT(lambda1, 0) && m_ApproxComparer.GT(lambda2, 0))
                        ReplaceConn12Conn34(connSet, node1, node2, node3, node4);
                    // Lambda1>0 && Lambda2<=0
                    if(m_ApproxComparer.GT(lambda1, 0) && m_ApproxComparer.LE(lambda2, 0))
                    {
                        // удаляем узел 1
                        RemoveNode(graph, node1, connSet);
                    }
                    // Lambda1<=0 && Lambda2>0
                    if(m_ApproxComparer.LE(lambda1, 0) && m_ApproxComparer.GT(lambda2, 0))
                    {
                        // удаляем узел 2
                        RemoveNode(graph, node2, connSet);
                    }
                    // Lambda1<=0 && Lambda2<=0
                    if(m_ApproxComparer.LE(lambda1, 0) && m_ApproxComparer.LE(lambda2, 0))
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
        public /*private*/ Boolean CheckConnConvexity(Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2,
                                                      Polyhedron3DGraphNode node3, Polyhedron3DGraphNode node4)
        {
            // решение системы лин. уравнений (3x3), используемое для проверки связи 1-2 на локальную выпуклость (см. алгоритм)
            Matrix cone123Solution = SolveCone123EquationSystem(node1, node2, node3);
            // проверка связи 1-2 на локальную выпуклость
            Double localConvexCriterion = cone123Solution[1, 1] * node4.NodeNormal.XCoord +
                                          cone123Solution[2, 1] * node4.NodeNormal.YCoord +
                                          cone123Solution[3, 1] * node4.NodeNormal.ZCoord;

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
        public /*private*/ Matrix SolveCone123EquationSystem(Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2,
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
        public /*private*/ Matrix CalcLambda123(Vector3D coneVector1, Vector3D coneVector2, Vector3D coneVector3,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coneVector1"></param>
        /// <param name="coneVector2"></param>
        /// <param name="coneVector3"></param>
        /// <param name="coneVector4"></param>
        /// <param name="matrixError"></param>
        /// <returns></returns>
        public /*private*/ Matrix CalcLambda123(Vector3D coneVector1, Vector3D coneVector2, Vector3D coneVector3,
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
        public /*private*/ void ReplaceConn12Conn34(SuspiciousConnectionSet connSet, Polyhedron3DGraphNode node1,
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

        /*/// <summary>
        /// метод RemoveNode удаляет узел removedNode из графа graph, модифицирует (триангулирует) граф graph, модифицирует список связей П
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="removedNode"></param>
        /// <param name="connSet"></param>
        public void RemoveNode(Polyhedron3DGraph graph, Polyhedron3DGraphNode removedNode, SuspiciousConnectionSet connSet)
        {
            // список узлов, образующих контур сектора K*
            ICyclicList<Polyhedron3DGraphNode> sectorNodes = new CyclicList<Polyhedron3DGraphNode>();

            // Цикл (по всем связям удаляемого узла)
            for (Int32 connIndex = 0; connIndex < removedNode.ConnectionList.Count; ++connIndex)
            {
                // Текущим узлом становится узел, связанный с удаляемым узлом текущей связью
                Polyhedron3DGraphNode currentConn = removedNode.ConnectionList[connIndex];
                // Заносим текущий узел в список узлов, образующих контур сектора K*
                sectorNodes.Add(currentConn);
                // Из списка связей текущего узла удаляем ссылку на удаляемый узел
                currentConn.ConnectionList.Remove(removedNode);
            }

            // Цикл (по всем узлам из списка узлов образующих контур сектора)
            for (Int32 nodeIndex = 0; nodeIndex < sectorNodes.Count; ++nodeIndex)
            {
                Polyhedron3DGraphNode currentNode = sectorNodes[nodeIndex];
                Polyhedron3DGraphNode nextNode = sectorNodes.GetNextItem(nodeIndex);

                // проверка на то, что currentNode - nextNode на самом деле связь
                //Debug.Assert(currentNode.ConnectionList.IndexOf(nextNode) != -1, "nextNode must be in currentNode.ConnectionList");
                //Debug.Assert(nextNode.ConnectionList.IndexOf(currentNode) != -1, "currentNode must be in nodeNode.ConnectionList");

                // Связь текущий узел – следующий узел заносим в набор П
                connSet.AddConnection(currentNode, nextNode);

            }
            // Цикл (по всем узлам из списка узлов образующих контур сектора)

            // Удаляем все связи, содержащих удаляемый узел, из набора П
            connSet.RemoveConnections(removedNode);
            // Удаляем удаляемый узел из списка узлов графа
            graph.NodeList.Remove(removedNode);

            // триангуляция полученного сектора
            if (sectorNodes.Count > 3)
            {
                SectorTriangulation(sectorNodes, connSet);
            }
        }*/

        /// <summary>
        /// метод RemoveNode удаляет узел RemovedNode и триангулирует полученный сектор
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="RemovedNode"></param>
        /// <param name="SuspiciousConnectionSet"></param>
        public /*private*/ void RemoveNode(Polyhedron3DGraph graph, Polyhedron3DGraphNode RemovedNode,
                                           SuspiciousConnectionSet SuspiciousConnectionSet)
        {
            // удаляем удаляемый узел из списка узлов графа
            graph.NodeList.Remove(RemovedNode);
            // удаляем из набора П все связи содержащие удаляемый узел
            SuspiciousConnectionSet.RemoveConnections(RemovedNode);

            List<Polyhedron3DGraphNode> SectorPGNodeList = new List<Polyhedron3DGraphNode>();
            // цикл по всем связям удаляемого узла
            for(Int32 ConnIndex = 0; ConnIndex < RemovedNode.ConnectionList.Count; ConnIndex++)
            {
                // узел, связанный с удаляемым текущей связью, становится текущим узлом
                Polyhedron3DGraphNode CurrentConnNode = RemovedNode.ConnectionList[ConnIndex];
                Polyhedron3DGraphNode NextConnNode = (ConnIndex == RemovedNode.ConnectionList.Count - 1
                                                          ? RemovedNode.ConnectionList[0]
                                                          : RemovedNode.ConnectionList[ConnIndex + 1]);
                // добавляем текущий узел в конец упорядоченного (против ч.с.) списка узлов сектора
                SectorPGNodeList.Add(CurrentConnNode);
                // удаляем ссылку на удаляемый узел из списка связей текущего узла
                CurrentConnNode.ConnectionList.Remove(RemovedNode);
                // добавляем в набор П связь, соединяющую текущий и следующий (узел, связанный с удаляемым следующей по отношению к текущей связью) узлы
                SuspiciousConnectionSet.AddConnection(CurrentConnNode, NextConnNode);
            }

            // текущим узлом становится первый узел из упорядоченного списка узлов сектора
            Polyhedron3DGraphNode CurrentPGNode = SectorPGNodeList[0];
            Int32 CurrentPGNodeIndex = 0;

            Int32 IterationNumber = 0;
            while(SectorPGNodeList.Count > 3)
            {
                // узлом номер 1 становится текущий узел
                Polyhedron3DGraphNode PGNode1 = CurrentPGNode;
                // узлом номер 2 становится следующий (относительно узла 1) по направлению движения (из упорядоченного списка узлов сектора)
                Polyhedron3DGraphNode PGNode2 = (CurrentPGNodeIndex + 1 >= SectorPGNodeList.Count
                                                     ? SectorPGNodeList[CurrentPGNodeIndex + 1 - SectorPGNodeList.Count]
                                                     : SectorPGNodeList[CurrentPGNodeIndex + 1]);
                // узлом номер 3 становится следующий (относительно узла 2) по направлению движения (из упорядоченного списка узлов сектора)
                Polyhedron3DGraphNode PGNode3 = (CurrentPGNodeIndex + 2 >= SectorPGNodeList.Count
                                                     ? SectorPGNodeList[CurrentPGNodeIndex + 2 - SectorPGNodeList.Count]
                                                     : SectorPGNodeList[CurrentPGNodeIndex + 2]);
                // узлом номер 4 становится следующий (относительно узла 3) по направлению движения (из упорядоченного списка узлов сектора)
                //Polyhedron3DGraphNode PGNode4 = (CurrentPGNodeIndex + 3 >= SectorPGNodeList.Count
                //                                     ? SectorPGNodeList[CurrentPGNodeIndex + 3 - SectorPGNodeList.Count]
                //                                     : SectorPGNodeList[CurrentPGNodeIndex + 3]);

                // вычисляем смешанное произведение векторов, связанных с узлами 2, 1 и 3
                Double MixedProduct213Value = Vector3D.MixedProduct(PGNode2.NodeNormal, PGNode1.NodeNormal,
                                                                    PGNode3.NodeNormal);
                // вычисляем смешанное произведение векторов, связанных с узлами 4, 1 и 3
                //Double MixedProduct413Value = Vector3D.MixedProduct(PGNode4.NodeNormal, PGNode1.NodeNormal,
                //                                                    PGNode3.NodeNormal);
                // вычисляем смешанное произведение векторов, связанных с удаляемым узлом и узлами 1 и 3
                Double MixedProductR13Value = Vector3D.MixedProduct(RemovedNode.NodeNormal, PGNode1.NodeNormal,
                                                                    PGNode3.NodeNormal);

                // если знаки вычисленных смешанных произведений разные, то связь 1-3 может быть построена 
                Boolean IsConn13Correct = ((MixedProduct213Value > m_ApproxComparer.Epsilon) &&
                                           (MixedProductR13Value < -m_ApproxComparer.Epsilon)) ||
                                          ((MixedProductR13Value > m_ApproxComparer.Epsilon) &&
                                           (MixedProduct213Value < -m_ApproxComparer.Epsilon)) ||
                                          ((MixedProductR13Value > -m_ApproxComparer.Epsilon) &&
                                           (MixedProductR13Value < m_ApproxComparer.Epsilon));

                // если знаки вычисленных смешанных произведений MixedProduct213Value и MixedProductR13Value разные, то связь 1-3 может быть построена
                //Boolean IsConn13Correct = ((MixedProduct213Value > Epsilon) && (MixedProductR13Value < -Epsilon)) || ((MixedProductR13Value > Epsilon) && (MixedProduct213Value < -Epsilon)) || ((MixedProductR13Value > -Epsilon) && (MixedProductR13Value < Epsilon));
                // Boolean IsConn13Correct = ((MixedProduct213Value > Epsilon) && (MixedProductR13Value < -Epsilon)) || ((MixedProductR13Value > Epsilon) && (MixedProduct213Value < -Epsilon));
                // если MixedProductR13Value = 0 и знаки вычисленных смешанных произведений MixedProduct213Value и MixedProduct413Value разные, то связь 1-3 может быть построена
                // IsConn13Correct = IsConn13Correct || ((MixedProduct213Value > Epsilon) && (MixedProduct413Value < -Epsilon)) || ((MixedProduct413Value > Epsilon) && (MixedProduct213Value < -Epsilon));
                //Boolean IsConn13Correct = (MixedProduct213Value * MixedProductR13Value < 0) || ((MixedProductR13Value == 0) && (MixedProduct213Value * MixedProduct413Value < 0));

                if(IsConn13Correct)
                {
                    Int32 Conn12Index = PGNode1.ConnectionList.IndexOf(PGNode2);
                    Int32 Conn32Index = PGNode3.ConnectionList.IndexOf(PGNode2);

                    // в список связей узла 1 после ссылки на узел 2 добавляем ссылку на узел 3
                    PGNode1.ConnectionList.Insert(Conn12Index + 1, PGNode3);
                    // в список связей узла 3 перед ссылкой на узел 2 добавляем ссылку на узел 1
                    PGNode3.ConnectionList.Insert(Conn32Index, PGNode1);
                    // удаляем узел 2 из упорядоченного списка узлов сектора
                    SectorPGNodeList.Remove(PGNode2);

                    if(CurrentPGNodeIndex == SectorPGNodeList.Count) CurrentPGNodeIndex--;

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

                if(IterationNumber == SectorPGNodeList.Count)
                    throw new Exception("!!!");
            }
        }

        /// <summary>
        /// метод SectorTriangulation триангулирует сектор, заданный своим контуром sectorNodes (узлы в контуре идут упорядоченными против ч.с. ...)
        /// </summary>
        /// <param name="sectorNodes"></param>
        /// <param name="connSet"></param>
        public /*private*/ void SectorTriangulation(IList<Polyhedron3DGraphNode> sectorNodes,
                                                    SuspiciousConnectionSet connSet)
        {
            // Если количество узлов в секторе <= 3, то конец работы алгоритма
            if(sectorNodes.Count <= 3) return;

            // в списке sectorNodes узлы распологаются против ч.с.
            // => список sectorNodes обходим в обратном порядке, чтобы получить упорядочение по ч.с.
            for(Int32 sectorNodeIndex = 0; sectorNodeIndex < sectorNodes.Count; ++sectorNodeIndex)
            {
                Polyhedron3DGraphNode initialNode = sectorNodes[sectorNodeIndex];

                // в visibleNodes минимум 2 узла : следующий и предыдущий для initialNode
                List<Polyhedron3DGraphNode> visibleNodes = GetVisibleNodes(sectorNodes, initialNode);
                if(visibleNodes.Count == 2)
                    continue;

                List<Polyhedron3DGraphNode> connList = GetConnectionList4Sector(sectorNodes, visibleNodes, initialNode);
                // в connList минимум 2 узла : следующий и предыдущий для initialNode
                if(connList.Count == 2)
                    continue;
                IList<Polyhedron3DGraphNode> subSectorNodes = new List<Polyhedron3DGraphNode>();

                // в списке connList узлы распологаются по ч.с =>
                // обходя список в обратном порядке мы получим узлы, расположенные против ч.с.
                for(Int32 connIndex = connList.Count - 1; connIndex >= 1; --connIndex)
                {
                    Polyhedron3DGraphNode startNode = connList[connIndex];
                    Polyhedron3DGraphNode finishNode = connList[connIndex - 1];

                    for(Polyhedron3DGraphNode node = startNode;
                        node != finishNode;
                        node = sectorNodes.GetNextItem(node))
                        subSectorNodes.Add(node);

                    // если connIndex == 1, то finishNode - это сосед узла initialNode
                    if(connIndex > 1)
                    {
                        initialNode.ConnectionList.Insert(initialNode.ConnectionList.GetNextItemIndex(startNode),
                                                          finishNode);
                        finishNode.ConnectionList.Insert(
                            finishNode.ConnectionList.GetPrevItemIndex(subSectorNodes[subSectorNodes.Count - 1]),
                            initialNode);

                        connSet.AddConnection(initialNode, finishNode);
                    }

                    subSectorNodes.Add(finishNode);
                    subSectorNodes.Add(initialNode);

                    if(subSectorNodes.Count > 3)
                        SectorTriangulation(subSectorNodes, connSet);
                }

                return;
            }

            // если дошли сюда, то триангуляцию сектора провести не смогли
#warning может более специализированное исключение
            throw new Exception("Sector triangulation failed !!!");
        }

        /// <summary>
        /// список видимых узлов сектора sectorNodes из узла initialNode
        /// </summary>
        /// <param name="sectorNodes"></param>
        /// <param name="initialNode"></param>
        /// <returns></returns>
        public /*private*/ List<Polyhedron3DGraphNode> GetVisibleNodes(IList<Polyhedron3DGraphNode> sectorNodes,
                                                                       Polyhedron3DGraphNode initialNode)
        {
            // список видимых из initialNode узлов сектора, упорядоченных по ч.с.
            List<Polyhedron3DGraphNode> visibleNodes = new List<Polyhedron3DGraphNode>();

            // в списке sectorNodes узлы распологаются против ч.с.
            // => список sectorNodes обходим в обратном порядке, чтобы получить упорядочение по ч.с.
            Polyhedron3DGraphNode node1 = initialNode;
            Polyhedron3DGraphNode node2 = sectorNodes.GetPrevItem(node1);
            Polyhedron3DGraphNode nodem = sectorNodes.GetNextItem(node1);

            visibleNodes.Add(node2);
            for(Polyhedron3DGraphNode nodek = sectorNodes.GetPrevItem(node2);
                nodek != nodem;
                nodek = sectorNodes.GetPrevItem(nodek))
            {
                if(CheckNodeVisibility(sectorNodes, node1, nodek))
                    visibleNodes.Add(nodek);
            }
            visibleNodes.Add(nodem);

            return visibleNodes;
        }

        /// <summary>
        /// проверка того, что в секторе sectorNodeList узел nodek виден из узла node1
        /// </summary>
        /// <param name="sectorNodes"></param>
        /// <param name="node1"></param>
        /// <param name="nodek"></param>
        /// <returns></returns>
        public /*private*/ Boolean CheckNodeVisibility(IList<Polyhedron3DGraphNode> sectorNodes,
                                                       Polyhedron3DGraphNode node1, Polyhedron3DGraphNode nodek)
        {
            // в списке sectorNodes узлы распологаются против ч.с.
            // => список sectorNodes обходим в обратном порядке, чтобы получить упорядочение по ч.с.
            Polyhedron3DGraphNode node2 = sectorNodes.GetPrevItem(node1);
            Polyhedron3DGraphNode nodem = sectorNodes.GetNextItem(node1);

            // соотношение номер 1 (!!! ВНИМАНИЕ. ОЧЕНЬ ВАЖЕН ПОРЯДОК. СМ. КНИГУ !!!)
            //Double mixedProductk12 = Vector3D.MixedProduct(nodek.NodeNormal, node1.NodeNormal, node2.NodeNormal);
            //Double mixedProductk1m = Vector3D.MixedProduct(nodek.NodeNormal, node1.NodeNormal, nodem.NodeNormal);
            Double mixedProductk12 = -Vector3D.MixedProduct(nodek.NodeNormal, node1.NodeNormal, node2.NodeNormal);
            Double mixedProductk1m = -Vector3D.MixedProduct(nodek.NodeNormal, node1.NodeNormal, nodem.NodeNormal);
            // должно быть : mixedProductk12 > 0, mixedProductk1m < 0
            if(m_ApproxComparer.LE(mixedProductk12, 0) || m_ApproxComparer.GE(mixedProductk1m, 0))
                return false;

            // соотношение номер 2
            // описание алгоритма см. в документах (или на листочке)
            for(Polyhedron3DGraphNode nodei = node2; nodei != nodem; nodei = sectorNodes.GetPrevItem(nodei))
            {
                Polyhedron3DGraphNode nodeip1 = sectorNodes.GetPrevItem(nodei);

                Vector3D v0 = Vector3D.VectorProduct(Vector3D.VectorProduct(nodeip1.NodeNormal, nodei.NodeNormal),
                                                     Vector3D.VectorProduct(node1.NodeNormal, nodek.NodeNormal));

                Double scalarProduct01 = Vector3D.ScalarProduct(v0, node1.NodeNormal);
                Double scalarProduct0k = Vector3D.ScalarProduct(v0, nodek.NodeNormal);
                Double scalarProduct1k = Vector3D.ScalarProduct(node1.NodeNormal, nodek.NodeNormal);

                Double scalarProduct0i = Vector3D.ScalarProduct(v0, nodei.NodeNormal);
                Double scalarProduct0ip1 = Vector3D.ScalarProduct(v0, nodeip1.NodeNormal);
                Double scalarProductiip1 = Vector3D.ScalarProduct(nodei.NodeNormal, nodeip1.NodeNormal);

                Double lambda1 = (scalarProduct01 - scalarProduct0k * scalarProduct1k) /
                                 (1 - scalarProduct1k * scalarProduct1k);
                Double lambda2 = (scalarProduct0k - scalarProduct01 * scalarProduct1k) /
                                 (1 - scalarProduct1k * scalarProduct1k);
                Double lambda3 = (scalarProduct0i - scalarProduct0ip1 * scalarProductiip1) /
                                 (1 - scalarProductiip1 * scalarProductiip1);
                Double lambda4 = (scalarProduct0ip1 - scalarProduct0i * scalarProductiip1) /
                                 (1 - scalarProductiip1 * scalarProductiip1);

                // must be max(lambda1, lambda2, lambda3, lambda4) > 0
                if(GetMaxValue(lambda1, lambda2, lambda3, lambda4) <= 0) return false;
                // must be min(lambda1, lambda2, lambda3, lambda4) < 0
                if(GetMinValue(lambda1, lambda2, lambda3, lambda4) >= 0) return false;
            }

            return true;
        }

        /// <summary>
        /// список всех узлов из сектора ( упорядоченных по ч.с) для которых могут быть построены связи из узла initialNode 
        /// </summary>
        /// <param name="sectorNodes"></param>
        /// <param name="visibleNodes"></param>
        /// <param name="initialNode"></param>
        /// <returns></returns>
        public /*private*/ List<Polyhedron3DGraphNode> GetConnectionList4Sector(IList<Polyhedron3DGraphNode> sectorNodes,
                                                                                List<Polyhedron3DGraphNode> visibleNodes,
                                                                                Polyhedron3DGraphNode initialNode)
        {
            List<Polyhedron3DGraphNode> connList4Sector = new List<Polyhedron3DGraphNode>();

            Polyhedron3DGraphNode node1 = initialNode;

            // visibleNodes[0] = node2, visibleNodes[Count-1] = nodem
            connList4Sector.Add(visibleNodes[0]);

            // остальные узлы, составляют связи с node1 и их нужно проверить на Z - выпуклость
            for(Int32 vnodeIndex = 1; vnodeIndex < visibleNodes.Count - 1; ++vnodeIndex)
            {
                Polyhedron3DGraphNode nodek = visibleNodes[vnodeIndex];
                if(CheckConnInSector(visibleNodes, node1, nodek))
                    connList4Sector.Add(nodek);
            }
            // остальные узлы, составляют связи с node1 и их нужно проверить на Z - выпуклость

            // visibleNodes[0] = node2, visibleNodes[Count-1] = nodem
            connList4Sector.Add(visibleNodes[visibleNodes.Count - 1]);

            // список visibleNodes упорядочен по ч.с. => и список connList4Sector упорядочен по ч.с.
            // т.к. он получается из списка visibleNodes
            return connList4Sector;
        }

        /// <summary>
        /// проверка связи 1k в секторе на Z-выпуклость на списку видимых узлов из узла 1 (см. алгоритм в книжке)
        /// </summary>
        /// <param name="visibleNodes"></param>
        /// <param name="node1"></param>
        /// <param name="nodek"></param>
        /// <returns></returns>
        public /*private*/ Boolean CheckConnInSector(List<Polyhedron3DGraphNode> visibleNodes,
                                                     Polyhedron3DGraphNode node1, Polyhedron3DGraphNode nodek)
        {
            Int32 nodekIndex = visibleNodes.IndexOf(nodek);

            // всех i1, i2 : 0 <= i1 < nodekIndex, nodekIndex < i2 < visibleNodes.Count
            for(Int32 beforeIndex = 0; beforeIndex < nodekIndex; ++beforeIndex)
            {
                Polyhedron3DGraphNode nodei1 = visibleNodes[beforeIndex];
                for(Int32 afterIndex = nodekIndex + 1; afterIndex < visibleNodes.Count; ++afterIndex)
                {
                    Polyhedron3DGraphNode nodei2 = visibleNodes[afterIndex];

                    // если связь 1k в окружении i1, i2 не выпукла, то она проверку не проходит
                    if(!CheckConnConvexity(node1, nodek, nodei1, nodei2)) return false;
                }
            }
            // всех i1, i2 : 0 <= i1 < nodekIndex, nodekIndex < i2 < visibleNodes.Count

            return true;
        }

        /// <summary>
        /// max значение из списка values
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static Double GetMaxValue(params Double[] values)
        {
            if(values.Length == 0) return Double.NaN;

            Double maxValue = values[0];

            for(Int32 index = 1; index < values.Length; ++index)
                if(values[index] > maxValue) maxValue = values[index];

            return maxValue;
        }

        /// <summary>
        /// min значение из списка values
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static Double GetMinValue(params Double[] values)
        {
            if(values.Length == 0) return Double.NaN;

            Double minValue = values[0];

            for(Int32 index = 1; index < values.Length; ++index)
                if(values[index] < minValue) minValue = values[index];

            return minValue;
        }

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