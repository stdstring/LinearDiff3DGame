using System;
using System.Collections.Generic;
using System.Diagnostics;
using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.Crossing;

namespace LinearDiff3DGame.MaxStableBridge.Gamers
{
    /// <summary>
    /// класс представляющий и инкапсулирующий действия первого игрока
    /// в результате действий первого игрока мы получаем граф G(...Fi...)
    /// </summary>
    public /*internal*/ class FirstGamer
    {
        public FirstGamer(FirstGamerInitData initData)
        {
            approxComparer = initData.ApproxComp;
            matrixB = initData.Matrix;
            mpMax = initData.MaxSection;
            mpMin = initData.MinSection;
            deltaT = initData.DeltaT;
            angleNearnessComparer = new ApproxComp(initData.SeparateNodeValue);
        }

        [Obsolete]
        public FirstGamer(ApproxComp approxComparer, Matrix matrixB, Double deltaT, Double mpMax, Double mpMin)
        {
            this.approxComparer = approxComparer;
            const Double epsilon = 0.02;
            angleNearnessComparer = new ApproxComp(epsilon * epsilon);
            this.matrixB = matrixB;
            this.deltaT = deltaT;
            this.mpMax = mpMax;
            this.mpMin = mpMin;
        }

        public Polyhedron3DGraph Action(Polyhedron3DGraph graph,
                                        Matrix fundCauchyMatrix,
                                        Int32 generationID,
                                        Matrix scalingMatrix)
        {
            // столбец (матрица) D для данного первого игрока в данный момент времени
            Matrix matrixD = CalcMatrixD(fundCauchyMatrix, scalingMatrix);
            // вычисляем радиус векторы вершин отрезка Pi
            List<Vector3D> pointPiSet = new List<Vector3D>(2);
            pointPiSet.Add(new Vector3D(mpMax * matrixD[1, 1], mpMax * matrixD[2, 1], mpMax * matrixD[3, 1]));
            pointPiSet.Add(new Vector3D(mpMin * matrixD[1, 1], mpMin * matrixD[2, 1], mpMin * matrixD[3, 1]));
            // направляющий вектор отрезка Pi
            Vector3D directingPi = new Vector3D(matrixD[1, 1], matrixD[2, 1], matrixD[3, 1]);
            directingPi = Vector3DUtils.NormalizeVector(directingPi);
            // количество узлов в графе G(...Wi...)
            Int32 graphGWiNodeCount = graph.NodeList.Count;
            // строим граф G(...Fi...); при этом граф строится не заново, а за счет модификации графа graph G(...Wi...)
            Polyhedron3DGraph graphGFi = BuildGFiGrid(graph, directingPi, generationID);
            // подсчет опорной функции для старых узлов (для многогранника Fi)
            for(Int32 nodeIndex = 0; nodeIndex < graphGWiNodeCount; ++nodeIndex)
            {
#warning ОЧЕНЬ ВАЖНО !!!!!! ПРОВЕРИТЬ ПРАВИЛЬНОСТЬ ПОЛУЧЕНИЯ ЗНАЧЕНИЯ ОПОРНОЙ ФУНКЦИИ
                Polyhedron3DGraphNode currentNode = graphGFi.NodeList[nodeIndex];
                currentNode.SupportFuncValue += deltaT * Math.Max(-(currentNode.NodeNormal * pointPiSet[0]),
                                                                  -(currentNode.NodeNormal * pointPiSet[1]));
            }
            return graphGFi;
        }

        private Matrix CalcMatrixD(Matrix fundCauchyMatrix, Matrix scalingMatrix)
        {
            Matrix matrixDBeforeScaling = fundCauchyMatrix * matrixB;
            return scalingMatrix * matrixDBeforeScaling;
        }

        // TODO : реFUCKторинг
        private Polyhedron3DGraph BuildGFiGrid(Polyhedron3DGraph graph, Vector3D directionPi, Int32 generationID)
        {
            CrossingObjectsSearch search = new CrossingObjectsSearch(approxComparer);
            IList<CrossingObject> crossingObjects = search.GetCrossingObjects(graph, directionPi);
            CrossingObject previousCrossing = CheckCrossingNearnessAndCorrect(crossingObjects[0], directionPi);
            Polyhedron3DGraphNode previousNode;
            if(previousCrossing.CrossingObjectType == CrossingObjectType.GraphConnection)
            {
                previousNode = BuildCrossingNode(previousCrossing, directionPi, graph.NodeList.Count, generationID);
                graph.NodeList.Add(previousNode);
                AddCrossingNodeBetweenConn(previousCrossing, previousNode);
            }
            else
                previousNode = previousCrossing.PositiveNode;
            CrossingObject firstCrossing = previousCrossing;
            Polyhedron3DGraphNode firstNode = previousNode;
            for(Int32 crossingObjectIndex = 1; crossingObjectIndex < crossingObjects.Count; ++crossingObjectIndex)
            {
                CrossingObject currentCrossing = CheckCrossingNearnessAndCorrect(crossingObjects[crossingObjectIndex],
                                                                                 directionPi);
                Polyhedron3DGraphNode currentNode;
                if(currentCrossing.CrossingObjectType == CrossingObjectType.GraphConnection)
                {
                    currentNode = BuildCrossingNode(currentCrossing, directionPi, graph.NodeList.Count, generationID);
                    graph.NodeList.Add(currentNode);
                    AddCrossingNodeBetweenConn(currentCrossing, currentNode);
                }
                else
                    currentNode = currentCrossing.PositiveNode;
                AddConnectionsIfNeed(previousCrossing, previousNode, currentCrossing, currentNode);
                previousCrossing = currentCrossing;
                previousNode = currentNode;
            }
            AddConnectionsIfNeed(previousCrossing, previousNode, firstCrossing, firstNode);
            return graph;
        }

        private CrossingObject CheckCrossingNearnessAndCorrect(CrossingObject crossingObject, Vector3D directionPi)
        {
            if(crossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
                return crossingObject;

            Vector3D crossingNormal = CalcCrossingNormal(crossingObject, directionPi);
            Double positiveNodeCosAngle = Vector3DUtils.CosAngleBetweenVectors(crossingNormal, crossingObject.PositiveNode.NodeNormal);
            Double negativeNodeCosAngle = Vector3DUtils.CosAngleBetweenVectors(crossingNormal, crossingObject.NegativeNode.NodeNormal);
            if(positiveNodeCosAngle >= negativeNodeCosAngle && angleNearnessComparer.EQ(positiveNodeCosAngle, 1))
            {
                Polyhedron3DGraphNode crossingNode = crossingObject.PositiveNode;
                return new CrossingObject(CrossingObjectType.GraphNode, crossingNode, crossingNode);
            }
            if(negativeNodeCosAngle >= positiveNodeCosAngle && angleNearnessComparer.EQ(negativeNodeCosAngle, 1))
            {
                Polyhedron3DGraphNode crossingNode = crossingObject.NegativeNode;
                return new CrossingObject(CrossingObjectType.GraphNode, crossingNode, crossingNode);
            }
            return crossingObject;
        }

        private static Vector3D CalcCrossingNormal(CrossingObject currentCrossingObject, Vector3D directionPi)
        {
            if(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
            {
                Vector3D plusVector = currentCrossingObject.PositiveNode.NodeNormal;
                Vector3D minusVector = currentCrossingObject.NegativeNode.NodeNormal;
                // Строим вектор, перпендикулярный векторам, связанным текущей связью,
                // как векторное произведение положительного узла связи на отрицательный
                Vector3D npm = Vector3D.VectorProduct(plusVector, minusVector);
                // Вычисляем векторное произведение построенного вектора и направляющего вектора Pi
                Vector3D crossingNodeNormal = Vector3D.VectorProduct(npm, directionPi);
                crossingNodeNormal = Vector3DUtils.NormalizeVector(crossingNodeNormal);
                return crossingNodeNormal;
            }
            return currentCrossingObject.PositiveNode.NodeNormal;
        }

        private Polyhedron3DGraphNode BuildCrossingNode(CrossingObject currentCrossingObject,
                                                        Vector3D directionPi,
                                                        Int32 nodeID,
                                                        Int32 generationID)
        {
            if(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
            {
                Vector3D plusNodeNormal = currentCrossingObject.PositiveNode.NodeNormal;
                Vector3D minusNodeNormal = currentCrossingObject.NegativeNode.NodeNormal;
                Vector3D crossingNodeNormal = CalcCrossingNormal(currentCrossingObject, directionPi);
                Polyhedron3DGraphNode crossingNode = new Polyhedron3DGraphNode(nodeID, generationID, crossingNodeNormal);
                // TODO : подумать, что будет при delta близкой к 0 и как этого избежать
                // подсчет значения опорной функции для построенного узла
                // (l1, l):
                Double scalarProduct1 = plusNodeNormal * crossingNodeNormal;
                // (l2, l):
                Double scalarProduct2 = minusNodeNormal * crossingNodeNormal;
                // (l1, l2):
                Double scalarProduct12 = plusNodeNormal * minusNodeNormal;
                // delta = 1 - (l1, l2)*(l1, l2)
                Double delta = 1 - scalarProduct12 * scalarProduct12;
                Double alpha = (scalarProduct1 - scalarProduct12 * scalarProduct2) / delta;
                Double beta = (scalarProduct2 - scalarProduct12 * scalarProduct1) / delta;
#warning ОЧЕНЬ ВАЖНО !!!!!! ПРОВЕРИТЬ ПРАВИЛЬНОСТЬ ПОЛУЧЕНИЯ ЗНАЧЕНИЯ ОПОРНОЙ ФУНКЦИИ
#warning ОЧЕНЬ ВАЖНО !!!!!! считаем, что отрезок Pi проходит через точку 0 !!!!!!!!!!
                crossingNode.SupportFuncValue = alpha * currentCrossingObject.PositiveNode.SupportFuncValue +
                                                beta * currentCrossingObject.NegativeNode.SupportFuncValue;
                return crossingNode;
            }
            Debug.Assert(approxComparer.EQ(Vector3D.ScalarProduct(currentCrossingObject.PositiveNode.NodeNormal, directionPi), 0));
            return currentCrossingObject.PositiveNode;
        }

        private static void AddCrossingNodeBetweenConn(CrossingObject crossingObject, Polyhedron3DGraphNode crossingNode)
        {
            Debug.Assert(crossingObject.CrossingObjectType == CrossingObjectType.GraphConnection);
            // добавляем в список ссылок нового узла ссылки сначала на положительный узел связи, потом на отрицательный
            Polyhedron3DGraphNode plusNode = crossingObject.PositiveNode;
            Polyhedron3DGraphNode minusNode = crossingObject.NegativeNode;
            crossingNode.ConnectionList.Add(plusNode);
            crossingNode.ConnectionList.Add(minusNode);
            // для узлов, образующих связь, меняем их ссылки друг на друга (которые и образуют связь) на ссылку на новый узел
            plusNode.ConnectionList[plusNode.ConnectionList.IndexOf(minusNode)] = crossingNode;
            minusNode.ConnectionList[minusNode.ConnectionList.IndexOf(plusNode)] = crossingNode;
        }

        private static void AddConnectionsIfNeed(CrossingObject previousCrossingObject,
                                                 Polyhedron3DGraphNode previousCrossingNode,
                                                 CrossingObject currentCrossingObject,
                                                 Polyhedron3DGraphNode currentCrossingNode)
        {
            if(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode &&
               currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
                AddConnections4PrevNodeCurrentConn(previousCrossingObject, currentCrossingObject, currentCrossingNode);
            if(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
               currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
                AddConnections4PrevConnCurrentNode(previousCrossingObject, previousCrossingNode, currentCrossingObject);
            if(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
               currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
                AddConnections4PrevConnCurrentConn(previousCrossingObject, previousCrossingNode, currentCrossingObject, currentCrossingNode);
        }

        private static void AddConnections4PrevNodeCurrentConn(CrossingObject previousCrossingObject,
                                                               CrossingObject currentCrossingObject,
                                                               Polyhedron3DGraphNode currentCrossingNode)
        {
            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode);
            Debug.Assert(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection);

            Polyhedron3DGraphNode positiveNode = currentCrossingObject.PositiveNode;
            Polyhedron3DGraphNode negativeNode = currentCrossingObject.NegativeNode;
            Polyhedron3DGraphNode thirdTrNode = positiveNode.ConnectionList.GetPrevItem(currentCrossingNode);
            // инвариант целостности графа
            Debug.Assert(thirdTrNode == negativeNode.ConnectionList.GetNextItem(currentCrossingNode));
            thirdTrNode.ConnectionList.Insert(thirdTrNode.ConnectionList.IndexOf(positiveNode), currentCrossingNode);
            currentCrossingNode.ConnectionList.Insert(currentCrossingNode.ConnectionList.IndexOf(negativeNode),
                                                      thirdTrNode);
        }

        private static void AddConnections4PrevConnCurrentNode(CrossingObject previousCrossingObject,
                                                               Polyhedron3DGraphNode previousCrossingNode,
                                                               CrossingObject currentCrossingObject)
        {
            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection);
            Debug.Assert(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode);

            Polyhedron3DGraphNode positiveNode = previousCrossingObject.PositiveNode;
            Polyhedron3DGraphNode negativeNode = previousCrossingObject.NegativeNode;
            Polyhedron3DGraphNode thirdTrNode = positiveNode.ConnectionList.GetNextItem(previousCrossingNode);
            // инвариант целостности графа
            Debug.Assert(thirdTrNode == negativeNode.ConnectionList.GetPrevItem(previousCrossingNode));
            thirdTrNode.ConnectionList.Insert(thirdTrNode.ConnectionList.IndexOf(negativeNode), previousCrossingNode);
            previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(positiveNode),
                                                       thirdTrNode);
        }

        // TODO : реFUCKторинг
        private static void AddConnections4PrevConnCurrentConn(CrossingObject previousCrossingObject,
                                                               Polyhedron3DGraphNode previousCrossingNode,
                                                               CrossingObject currentCrossingObject,
                                                               Polyhedron3DGraphNode currentCrossingNode)
        {
            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection);
            Debug.Assert(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection);

            // у связей общий отрицательный узел (случай 3а)
            if(ReferenceEquals(previousCrossingObject.NegativeNode, currentCrossingObject.NegativeNode))
            {
                // положительный узел предыдущей связи (узел номер 1)
                Polyhedron3DGraphNode node1 = previousCrossingObject.PositiveNode;
                // общий отрицательный узел (узел номер 2)
                Polyhedron3DGraphNode node2 = previousCrossingObject.NegativeNode;
                // положительный узел текущей связи (узел номер 3)
                Polyhedron3DGraphNode node3 = currentCrossingObject.PositiveNode;
                // для узла номер 3: ссылка на предыдущей узел пересечения вставляется после ссылки на узел 1
                node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(node1) + 1,
                                            previousCrossingNode);
                // для предыдущего узла пересечения: после узла 2 добавляется сначала ссылка на новый узел пересечения, потом ссылка на узел номер 3
                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node2) + 1,
                                                           currentCrossingNode);
                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node2) + 2,
                                                           node3);
                // для текущего узла пересечения: ссылка на предыдущий узел пересечения вставляется после ссылки на узел номер 3
                currentCrossingNode.ConnectionList.Insert(currentCrossingNode.ConnectionList.IndexOf(node3) + 1,
                                                          previousCrossingNode);
            }
                // у связай общий положительный узел (случай 3б)
            else if(ReferenceEquals(previousCrossingObject.PositiveNode, currentCrossingObject.PositiveNode))
            {
                // отрицательный узел предыдущей связи (узел номер 1)
                Polyhedron3DGraphNode node1 = previousCrossingObject.NegativeNode;
                // общий положительный узел (узел номер 2)
                Polyhedron3DGraphNode node2 = previousCrossingObject.PositiveNode;
                // отрицательный узел текущей связи (узел номер 3)
                Polyhedron3DGraphNode node3 = currentCrossingObject.NegativeNode;
                // для узла номер 3: ссылка на предыдущей узел пересечения вставляется после ссылки на текущий узел пересечения
                node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(currentCrossingNode) + 1,
                                            previousCrossingNode);
                // для предыдущего узла пересечения: посде узла 1 добавляется сначала ссылка на узел номер 3, потом ссылка на новый узел пересечения
                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node1) + 1,
                                                           node3);
                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node1) + 2,
                                                           currentCrossingNode);
                // для текущего узла пересечения: ссылка на предыдущий узел пересечения вставляется после ссылки на узел номер 2
                currentCrossingNode.ConnectionList.Insert(currentCrossingNode.ConnectionList.IndexOf(node2) + 1,
                                                          previousCrossingNode);
            }
            else
                throw new Exception("Abnormal algorithm result");
        }

        private readonly ApproxComp approxComparer;
        private readonly ApproxComp angleNearnessComparer;
        private readonly Double deltaT;
        private readonly Matrix matrixB;
        private readonly Double mpMax;
        private readonly Double mpMin;

        ///// <summary>
        ///// метод BuildGFiGrid строит сетку G(...Fi...) (см. алгоритм)
        ///// </summary>
        ///// <param name="graph">граф, который достраивается до сетки G(...Fi...)</param>
        ///// <param name="directingVectorPi">направляющий вектор отрезка Pi</param>
        ///// <param name="generationID">ID поколения</param>
        ///// <returns>сетка G(...Fi...)</returns>
        //private Polyhedron3DGraph BuildGFiGrid(Polyhedron3DGraph graph, Vector3D directingVectorPi, Int32 generationID)
        //{
        //    // объект для поиска пересечений графа с G(...Pi...)
        //    CrossingObjectFinder finder = new CrossingObjectFinder(m_ApproxComparer);

        //    // первый (запомненный) объект пересечения
        //    CrossingObject firstCrossingObject = finder.GetFirstCrossingObject(graph.NodeList[0], directingVectorPi);
        //    // текущий объект пересечения
        //    CrossingObject currentCrossingObject = firstCrossingObject;
        //    // строим узел на пересечении текущего объекта и G(...Pi...) и запоминаем его
        //    // если этот узел отсутствует в списке узлов, то добавляем его и соответствующие ссылки на данный узел
        //    Polyhedron3DGraphNode firstCrossingNode = BuildCrossingNode(currentCrossingObject, graph, directingVectorPi, generationID);
        //    // текущий узел пересечения
        //    Polyhedron3DGraphNode currentCrossingNode = firstCrossingNode;

        //    if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
        //    {
        //        graph.NodeList.Add(currentCrossingNode);
        //        AddCrossingNodeBetweenConn(currentCrossingObject.PositiveNode, currentCrossingObject.NegativeNode, currentCrossingNode);
        //    }

        //    // Цикл (пока текущий объект не станет равным запомненному)
        //    do
        //    {
        //        // предыдущий объект пересечения
        //        CrossingObject previousCrossingObject = currentCrossingObject;
        //        // предыдущий узел пересечения
        //        Polyhedron3DGraphNode previousCrossingNode = currentCrossingNode;
        //        // получаем следующий по движению объект (связь, либо узел) и делаем его текущим
        //        currentCrossingObject = finder.GetNextCrossingObject(currentCrossingObject, currentCrossingNode, directingVectorPi);
        //        // строим узел на пересечении текущего объекта и G(...Pi...)
        //        // если этот узел отсутствует в списке узлов (этот узел будет присутствовать в списке узлов, если текущий объект – узел, либо если начальным объектом была связь и мы в нее пришли), то добавляем его и соответствующие ссылки на данный узел
        //        // отдельно обрабатываем случай если мы пришли в первый (запомненный) объект пересечения (для простоты реализации алгоритма)
        //        currentCrossingNode = (currentCrossingObject == firstCrossingObject ?
        //                               firstCrossingNode :
        //                               BuildCrossingNode(currentCrossingObject, graph, directingVectorPi, generationID));
        //        if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
        //            currentCrossingObject != firstCrossingObject)
        //        {
        //            graph.NodeList.Add(currentCrossingNode);
        //            AddCrossingNodeBetweenConn(currentCrossingObject.PositiveNode, currentCrossingObject.NegativeNode, currentCrossingNode);
        //        }

        //        // если предыдущий и текущий объекты – узлы
        //        if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode &&
        //            currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
        //        {
        //            // переход к следующей итерации цикла
        //            // continue;
        //        }

        //        // если предыдущий объект узел, а текущий связь
        //        if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode &&
        //            currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
        //        {
        //            // Строим связи между предыдущим узлом и узлом пересечения на текущем объекте
        //            AddConns4PrevNodeCurrentConnCase(previousCrossingObject, previousCrossingNode, currentCrossingObject, currentCrossingNode);
        //        }

        //        // если предыдущий объект связь, а текущий узел
        //        if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
        //            currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
        //        {
        //            // Строим связи между узлом пересечения на предыдущем объекте и текущем узле
        //            AddConns4PrevConnCurrentNodeCase(previousCrossingObject, previousCrossingNode, currentCrossingObject, currentCrossingNode);
        //        }

        //        // если предыдущий и текущий объекты - связи
        //        if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
        //            currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
        //        {
        //            // Строим связи между узлом пересечения на предыдущем объекте и узлом пересечения на текущем объекте
        //            // Строим связь между узлом пересечения на предыдущем объекте и узлом текущей связи, который не принадлежит предыдущей связи
        //            AddConns4PrevConnCurrentConnCase(previousCrossingObject, previousCrossingNode, currentCrossingObject, currentCrossingNode);
        //        }
        //    }
        //    while (currentCrossingObject != firstCrossingObject);
        //    // Цикл (пока текущий объект не станет равным запомненному)

        //    return graph;
        //}

//        /// <summary>
//        /// метод BuildGFiGrid строит сетку G(...Fi...) (см. алгоритм)
//        /// </summary>
//        /// <param name="graph">граф, который достраивается до сетки G(...Fi...)</param>
//        /// <param name="directingVectorPi">направляющий вектор отрезка Pi</param>
//        /// <param name="generationID">ID поколения</param>
//        /// <returns>сетка G(...Fi...)</returns>
//        private Polyhedron3DGraph BuildGFiGrid(Polyhedron3DGraph graph, Vector3D directingVectorPi, Int32 generationID)
//        {
//            const Double epsilon = 0.02;
//            ApproxComp cosAngleComparer = new ApproxComp(epsilon*epsilon);

//            // объект для поиска пересечений графа с G(...Pi...)
//            CrossingObjectsSearch finder = new CrossingObjectsSearch(m_ApproxComparer);

//            // первый (запомненный) объект пересечения
//            CrossingObject firstCrObject = finder.GetFirstCrossingObject(graph.NodeList[0], directingVectorPi);
//            // строим узел на пересечении текущего объекта и G(...Pi...) и запоминаем его
//            // если этот узел отсутствует в списке узлов, то добавляем его и соответствующие ссылки на данный узел
//            Polyhedron3DGraphNode firstCrNode = BuildCrossingNode(firstCrObject, graph, directingVectorPi, generationID);

//            // текущий объект пересечения
//            CrossingObject currentCrObject = firstCrObject;
//            // текущий узел пересечения
//            Polyhedron3DGraphNode currentCrNode = firstCrNode;

//            // узел пересечения, который будет построен
//            Polyhedron3DGraphNode actualCrNode = firstCrNode;
//            // объект пересечения, который будет построен
//            CrossingObject actualCrObject = CheckNodesNearnessAndCorrect(firstCrObject, ref actualCrNode, cosAngleComparer);

//            //if (currentCrObject.CrossingObjectType == CrossingObjectType.GraphConnection)
//            //{
//            //    graph.NodeList.Add(currentCrNode);
//            //    AddCrossingNodeBetweenConn(currentCrObject.PositiveNode, currentCrObject.NegativeNode, currentCrNode);
//            //}
//            if (actualCrObject.CrossingObjectType == CrossingObjectType.GraphConnection)
//            {
//                graph.NodeList.Add(actualCrNode);
//                AddCrossingNodeBetweenConn(actualCrObject.PositiveNode, actualCrObject.NegativeNode, actualCrNode);
//            }

//            //
//            Boolean reachFirstCrObject;
//            // Цикл (пока текущий объект не станет равным запомненному)
//            do
//            {
//                // получаем следующий по движению объект (связь, либо узел) и делаем его текущим
//                if (graph.NodeList[graph.NodeList.Count - 1] == currentCrNode)
//                {
//                    currentCrObject = finder.GetNextCrossingObject(currentCrObject, currentCrNode, directingVectorPi);
//                }
//                else
//                {
//                    currentCrObject = finder.GetNextCrossingObject(currentCrObject, directingVectorPi);
//                }
//                // строим узел на пересечении текущего объекта и G(...Pi...)
//                // если этот узел отсутствует в списке узлов (этот узел будет присутствовать в списке узлов, если текущий объект – узел, либо если начальным объектом была связь и мы в нее пришли),
//                // то добавляем его и соответствующие ссылки на данный узел
//                // отдельно обрабатываем случай если мы пришли в первый (запомненный) объект пересечения (для простоты реализации алгоритма)
//                currentCrNode = (currentCrObject == firstCrObject
//                                     ? firstCrNode
//                                     : BuildCrossingNode(currentCrObject, graph, directingVectorPi, generationID));
//                //
//                reachFirstCrObject = (currentCrObject == firstCrObject);
//                // предыдущий объект пересечения
//                CrossingObject prevActualCrObject = actualCrObject;
//                // предыдущий узел пересечения
//                Polyhedron3DGraphNode prevActualCrNode = actualCrNode;
//                // узел пересечения, который будет построен
//                actualCrNode = currentCrNode;
//                // объект пересечения, который будет построен
//                actualCrObject = CheckNodesNearnessAndCorrect(currentCrObject, ref actualCrNode, cosAngleComparer);
//                //
//                if (actualCrObject.CrossingObjectType == CrossingObjectType.GraphConnection && !reachFirstCrObject)
//                {
//                    graph.NodeList.Add(actualCrNode);
//                    AddCrossingNodeBetweenConn(actualCrObject.PositiveNode, actualCrObject.NegativeNode, actualCrNode);
//                }

//                // если предыдущий и текущий объекты – узлы
//                if (prevActualCrObject.CrossingObjectType == CrossingObjectType.GraphNode &&
//                    actualCrObject.CrossingObjectType == CrossingObjectType.GraphNode)
//                {
//                    // переход к следующей итерации цикла
//                    // continue;
//                }

//                // если предыдущий объект узел, а текущий связь
//                if (prevActualCrObject.CrossingObjectType == CrossingObjectType.GraphNode &&
//                    actualCrObject.CrossingObjectType == CrossingObjectType.GraphConnection)
//                {
//                    // Строим связи между предыдущим узлом и узлом пересечения на текущем объекте
//                    AddConns4PrevNodeCurrentConnCase(prevActualCrObject, prevActualCrNode, actualCrObject, actualCrNode);
//                }

//                // если предыдущий объект связь, а текущий узел
//                if (prevActualCrObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
//                    actualCrObject.CrossingObjectType == CrossingObjectType.GraphNode)
//                {
//                    // Строим связи между узлом пересечения на предыдущем объекте и текущем узле
//                    AddConns4PrevConnCurrentNodeCase(prevActualCrObject, prevActualCrNode, actualCrObject, actualCrNode);
//                }

//                // если предыдущий и текущий объекты - связи
//                if (prevActualCrObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
//                    actualCrObject.CrossingObjectType == CrossingObjectType.GraphConnection)
//                {
//                    // Строим связи между узлом пересечения на предыдущем объекте и узлом пересечения на текущем объекте
//                    // Строим связь между узлом пересечения на предыдущем объекте и узлом текущей связи, который не принадлежит предыдущей связи
//                    AddConns4PrevConnCurrentConnCase(prevActualCrObject, prevActualCrNode, actualCrObject, actualCrNode);
//                }
//            } while (!reachFirstCrObject);
//            // Цикл (пока текущий объект не станет равным запомненному)

//            return graph;
//        }

//        private CrossingObject CheckNodesNearnessAndCorrect(CrossingObject crossingObject,
//                                                            ref Polyhedron3DGraphNode crossingNode,
//                                                            ApproxComp cosAngleComparer)
//        {
//            if (crossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
//            {
//                return crossingObject;
//            }

//            Double positiveNodeCosAngle = Vector3DUtils.CosAngleBetweenVectors(crossingNode.NodeNormal,
//                                                                               crossingObject.PositiveNode.NodeNormal);
//            Double negativeNodeCosAngle = Vector3DUtils.CosAngleBetweenVectors(crossingNode.NodeNormal,
//                                                                               crossingObject.NegativeNode.NodeNormal);

//            if (positiveNodeCosAngle >= negativeNodeCosAngle && cosAngleComparer.EQ(positiveNodeCosAngle, 1))
//            {
//                crossingNode = crossingObject.PositiveNode;
//                return new CrossingObject(CrossingObjectType.GraphNode, crossingNode, crossingNode);
//            }

//            if (negativeNodeCosAngle >= positiveNodeCosAngle && cosAngleComparer.EQ(negativeNodeCosAngle, 1))
//            {
//                crossingNode = crossingObject.NegativeNode;
//                return new CrossingObject(CrossingObjectType.GraphNode, crossingNode, crossingNode);
//            }

//            return crossingObject;
//        }

//        /// <summary>
//        /// метод CalcCrossingNodeNormal вычисляет нормаль узла на текущем пересечении графа с G(...Pi...)
//        /// </summary>
//        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Pi...)</param>
//        /// <param name="directingVectorPi">направляющий вектор отрезка Pi</param>
//        /// <returns>нормаль узла на текущем пересечении графа с G(...Pi...)</returns>
//        private Vector3D CalcCrossingNodeNormal(CrossingObject currentCrossingObject, Vector3D directingVectorPi)
//        {
//            Vector3D crossingNodeNormal;

//            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
//            {
//                Vector3D plusVector = currentCrossingObject.PositiveNode.NodeNormal;
//                Vector3D minusVector = currentCrossingObject.NegativeNode.NodeNormal;
//                // Строим вектор, перпендикулярный векторам, связанным текущей связью,
//                // как векторное произведение положительного узла связи на отрицательный
//                Vector3D npm = Vector3D.VectorProduct(plusVector, minusVector);
//                // Вычисляем векторное произведение построенного вектора и направляющего вектора Pi
//                crossingNodeNormal = Vector3D.VectorProduct(npm, directingVectorPi);
//                crossingNodeNormal.Normalize();
//            }
//            else
//            {
//                crossingNodeNormal = currentCrossingObject.PositiveNode.NodeNormal;
//            }

//            return crossingNodeNormal;
//        }

//        /// <summary>
//        /// метод BuildCrossingNode создает и возвращает узел на текущем пересечении графа с G(...Pi...)
//        /// </summary>
//        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Pi...)</param>
//        /// <param name="graph">граф</param>
//        /// <param name="directingVectorPi">направляющий вектор отрезка Pi</param>
//        /// <param name="generationID">ID поколения</param>
//        /// <returns>созданный узел на текущем пересечении графа с G(...Pi...)</returns>
//        private Polyhedron3DGraphNode BuildCrossingNode(CrossingObject currentCrossingObject, Polyhedron3DGraph graph,
//                                                        Vector3D directingVectorPi, Int32 generationID)
//        {
//            Polyhedron3DGraphNode crossingNode;

//            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
//            {
//                Vector3D plusNodeNormal = currentCrossingObject.PositiveNode.NodeNormal;
//                Vector3D minusNodeNormal = currentCrossingObject.NegativeNode.NodeNormal;
//                Vector3D crossingNodeNormal = CalcCrossingNodeNormal(currentCrossingObject, directingVectorPi);

//                // Строим узел, связанный с полученным (выше) вектором и возвращаем его
//                crossingNode = new Polyhedron3DGraphNode(graph.NodeList.Count, generationID, crossingNodeNormal);
//                // подсчет значения опорной функции для построенного узла
//                // (l1, l):
//                Double scalarProduct1 = plusNodeNormal*crossingNodeNormal;
//                // (l2, l):
//                Double scalarProduct2 = minusNodeNormal*crossingNodeNormal;
//                // (l1, l2):
//                Double scalarProduct12 = plusNodeNormal*minusNodeNormal;
//                // delta = 1 - (l1, l2)*(l1, l2)
//                Double delta = 1 - scalarProduct12*scalarProduct12;

//                Double alpha = (scalarProduct1 - scalarProduct12*scalarProduct2)/delta;
//                Double beta = (scalarProduct2 - scalarProduct12*scalarProduct1)/delta;

//#warning ОЧЕНЬ ВАЖНО !!!!!! ПРОВЕРИТЬ ПРАВИЛЬНОСТЬ ПОЛУЧЕНИЯ ЗНАЧЕНИЯ ОПОРНОЙ ФУНКЦИИ
//#warning ОЧЕНЬ ВАЖНО !!!!!! считаем, что отрезок Pi проходит через точку 0 !!!!!!!!!!
//                crossingNode.SupportFuncValue = alpha*currentCrossingObject.PositiveNode.SupportFuncValue +
//                                                beta*currentCrossingObject.NegativeNode.SupportFuncValue;
//            }
//            else
//            {
//                // Вычисляем скалярное произведение вектора, связанного с текущим узлом, и направляющего вектора Pi
//                // Если скалярное произведение <> 0, то это ошибка работы алгоритма
//#warning Check is absent !!!

//                crossingNode = currentCrossingObject.PositiveNode;
//            }

//            return crossingNode;
//        }

//        /// <summary>
//        /// метод AddCrossingNodeBetweenConn добавляет узел crossingNode на пересечении связи и G(...Pi...) и соответствующим образом правит/добавляет ссылки
//        /// </summary>
//        /// <param name="connPlusNode">положительный узел пересекаемой связи</param>
//        /// <param name="connMinusNode">отрицательный узел пересекаемой связи</param>
//        /// <param name="crossingNode">узел на пересечении связи и G(...Pi...)</param>
//        private void AddCrossingNodeBetweenConn(Polyhedron3DGraphNode connPlusNode, Polyhedron3DGraphNode connMinusNode,
//                                                Polyhedron3DGraphNode crossingNode)
//        {
//            // возложение на метод левых функций
//            /*// добавляем новый узел в список узлов графа
//            graph.NodeList.Add(crossingNode);*/
//            // возложение на метод левых функций

//            // добавляем в список ссылок нового узла ссылки сначала на положительный узел связи, потом на отрицательный
//            crossingNode.ConnectionList.Add(connPlusNode);
//            crossingNode.ConnectionList.Add(connMinusNode);
//            // для узлов, образующих связь, меняем их ссылки друг на друга (которые и образуют связь) на ссылку на новый узел
//            connPlusNode.ConnectionList[connPlusNode.ConnectionList.IndexOf(connMinusNode)] = crossingNode;
//            connMinusNode.ConnectionList[connMinusNode.ConnectionList.IndexOf(connPlusNode)] = crossingNode;
//        }

//        /// <summary>
//        /// метод AddConns4PrevNodeCurrentConnCase добавляет необходимые связи в случае, если предыдущий объект пересечения - узел, а текущий - связь
//        /// связи добавляются для того, чтобы граф оставался триангулированным
//        /// </summary>
//        /// <param name="previousCrossingObject">предыдущий объект пересечения графа с G(...Pi...)</param>
//        /// <param name="previousCrossingNode">узел на предыдущем пересечении графа с G(...Pi...)</param>
//        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Pi...)</param>
//        /// <param name="currentCrossingNode">узел на текущем пересечении графа с G(...Pi...)</param>
//        private void AddConns4PrevNodeCurrentConnCase(CrossingObject previousCrossingObject,
//                                                      Polyhedron3DGraphNode previousCrossingNode,
//                                                      CrossingObject currentCrossingObject,
//                                                      Polyhedron3DGraphNode currentCrossingNode)
//        {
//            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode &&
//                         currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection,
//                         "previous crossing object must be node and current crossing object - connection");

//            //// строим связи между предыдущим узлом и узлом пересечения на текущем объекте :

//            //// отрицательный узел текущей связи
//            //Polyhedron3DGraphNode connMinusNode = currentCrossingObject.NegativeNode;
//            //// ссылку на текущий узел пересечения в список ссылок предыдущего узла вставляем после ссылки на отрицательный узел текущей связи
//            ///*Int32 PrevNode2CurrentMinusNodeConnIndex = PreviousCrossingNode.GetConnectionIndex(CurrentConnMinusNode);
//            //PreviousCrossingNode.InsertNodeConnection(PrevNode2CurrentMinusNodeConnIndex + 1, CurrentCrossingNode);*/
//            //previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(connMinusNode) + 1,
//            //                                           currentCrossingNode);
//            //// ссылку на предыдущий узел вставляем после ссылки на положительный узел текущей связи (на позицию номер 1)
//            //currentCrossingNode.ConnectionList.Insert(1, previousCrossingNode);

//            Polyhedron3DGraphNode positiveNode = currentCrossingObject.PositiveNode;
//            Polyhedron3DGraphNode negativeNode = currentCrossingObject.NegativeNode;
//            Polyhedron3DGraphNode thirdTrNode = positiveNode.ConnectionList.GetPrevItem(currentCrossingNode);
//            thirdTrNode.ConnectionList.Insert(thirdTrNode.ConnectionList.IndexOf(positiveNode), currentCrossingNode);
//            currentCrossingNode.ConnectionList.Insert(currentCrossingNode.ConnectionList.IndexOf(negativeNode),
//                                                      thirdTrNode);
//        }

//        /// <summary>
//        /// метод AddConns4PrevConnCurrentNodeCase добавляет необходимые связи в случае, если предыдущий объект пересечения - связь, а текущий - узел
//        /// связи добавляются для того, чтобы граф оставался триангулированным
//        /// </summary>
//        /// <param name="previousCrossingObject">предыдущий объект пересечения графа с G(...Pi...)</param>
//        /// <param name="previousCrossingNode">узел на предыдущем пересечении графа с G(...Pi...)</param>
//        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Pi...)</param>
//        /// <param name="currentCrossingNode">узел на текущем пересечении графа с G(...Pi...)</param>
//        private void AddConns4PrevConnCurrentNodeCase(CrossingObject previousCrossingObject,
//                                                      Polyhedron3DGraphNode previousCrossingNode,
//                                                      CrossingObject currentCrossingObject,
//                                                      Polyhedron3DGraphNode currentCrossingNode)
//        {
//            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
//                         currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode,
//                         "previous crossing object must be connection and current crossing object - node");

//            //// строим связи между узлом пересечения на предыдущем объекте и текущем узле :

//            //// положительный узел предыдущей связи
//            //Polyhedron3DGraphNode connPlusNode = previousCrossingObject.PositiveNode;
//            //// ссылку на предыдущий узел пересечения в список ссылок текущего узла вставляем после ссылки на положительный узел предыдущей связи
//            ///*Int32 CurrentNode2PrevPlusNodeConnIndex = CurrentCrossingNode.GetConnectionIndex(PreviousConnPlusNode);
//            //CurrentCrossingNode.InsertNodeConnection(CurrentNode2PrevPlusNodeConnIndex + 1, PreviousCrossingNode);*/
//            //currentCrossingNode.ConnectionList.Insert(currentCrossingNode.ConnectionList.IndexOf(connPlusNode) + 1,
//            //                                          previousCrossingNode);
//            //// ссылку на текущий узел добавляем в конец списка ссылок предыдущего узла
//            //previousCrossingNode.ConnectionList.Add(currentCrossingNode);

//            Polyhedron3DGraphNode positiveNode = previousCrossingObject.PositiveNode;
//            Polyhedron3DGraphNode negativeNode = previousCrossingObject.NegativeNode;
//            Polyhedron3DGraphNode thirdTrNode = positiveNode.ConnectionList.GetNextItem(previousCrossingNode);
//            thirdTrNode.ConnectionList.Insert(thirdTrNode.ConnectionList.IndexOf(negativeNode), previousCrossingNode);
//            previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(positiveNode),
//                                                       thirdTrNode);
//        }

//        /// <summary>
//        /// метод AddConns4PrevConnCurrentConnCase добавляет необходимые связи в случае, если и предыдущий, и текущий объекты пересечения - связи
//        /// связи добавляются для того, чтобы граф оставался триангулированным
//        /// </summary>
//        /// <param name="previousCrossingObject">предыдущий объект пересечения графа с G(...Pi...)</param>
//        /// <param name="previousCrossingNode">узел на предыдущем пересечении графа с G(...Pi...)</param>
//        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Pi...)</param>
//        /// <param name="currentCrossingNode">узел на текущем пересечении графа с G(...Pi...)</param>
//        private void AddConns4PrevConnCurrentConnCase(CrossingObject previousCrossingObject,
//                                                      Polyhedron3DGraphNode previousCrossingNode,
//                                                      CrossingObject currentCrossingObject,
//                                                      Polyhedron3DGraphNode currentCrossingNode)
//        {
//            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
//                         currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection,
//                         "previous and current crossing objects must be connections");

//            // строим связи между узлом пересечения на предыдущем объекте и узлом пересечения на текущем объекте
//            // строим связь между узлом пересечения на предыдущем объекте и узлом текущей связи, который не принадлежит предыдущей связи
//            // у связай общий отрицательный узел (случай 3а)
//            if (ReferenceEquals(previousCrossingObject.NegativeNode, currentCrossingObject.NegativeNode))
//            {
//                //// положительный узел предыдущей связи (узел номер 1)
//                //Polyhedron3DGraphNode node1 = previousCrossingObject.PositiveNode;
//                //// общий отрицательный узел (узел номер 2)
//                //// Polyhedron3DGraphNode node2 = previousCrossingObject.NegativeNode;
//                //// положительный узел текущей связи (узел номер 3)
//                //Polyhedron3DGraphNode node3 = currentCrossingObject.PositiveNode;
//                //// для узла номер 3: ссылка на предыдущей узел пересечения вставляется после ссылки на узел 1                
//                //node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(node1) + 1, previousCrossingNode);
//                //// для предыдущего узла пересечения: в конец списка ссылок добавляется сначала ссылка на новый узел пересечения, потом ссылка на узел номер 3
//                //previousCrossingNode.ConnectionList.Add(currentCrossingNode);
//                //previousCrossingNode.ConnectionList.Add(node3);
//                //// для текущего узла пересечения: ссылка на предыдущий узел пересечения вставляется после ссылки на узел номер 3 (т.е. на позицию номер 1)
//                //currentCrossingNode.ConnectionList.Insert(1, previousCrossingNode);
//                // положительный узел предыдущей связи (узел номер 1)
//                Polyhedron3DGraphNode node1 = previousCrossingObject.PositiveNode;
//                // общий отрицательный узел (узел номер 2)
//                Polyhedron3DGraphNode node2 = previousCrossingObject.NegativeNode;
//                // положительный узел текущей связи (узел номер 3)
//                Polyhedron3DGraphNode node3 = currentCrossingObject.PositiveNode;
//                // для узла номер 3: ссылка на предыдущей узел пересечения вставляется после ссылки на узел 1                
//                node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(node1) + 1, previousCrossingNode);
//                // для предыдущего узла пересечения: после узла 2 добавляется сначала ссылка на новый узел пересечения, потом ссылка на узел номер 3
//                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node2) + 1,
//                                                           currentCrossingNode);
//                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node2) + 2, node3);
//                // для текущего узла пересечения: ссылка на предыдущий узел пересечения вставляется после ссылки на узел номер 3
//                currentCrossingNode.ConnectionList.Insert(currentCrossingNode.ConnectionList.IndexOf(node3) + 1,
//                                                          previousCrossingNode);
//            }
//                // у связай общий положительный узел (случай 3б)
//            else if (ReferenceEquals(previousCrossingObject.PositiveNode, currentCrossingObject.PositiveNode))
//            {
//                //// отрицательный узел предыдущей связи (узел номер 1)
//                ////Polyhedron3DGraphNode node1 = previousCrossingObject.NegativeNode;
//                //// общий положительный узел (узел номер 2)
//                //// Polyhedron3DGraphNode node2 = previousCrossingObject.PositiveNode;
//                //// отрицательный узел текущей связи (узел номер 3)
//                //Polyhedron3DGraphNode node3 = currentCrossingObject.NegativeNode;
//                //// для узла номер 3: ссылка на предыдущей узел пересечения вставляется после ссылки на текущий узел пересечения
//                //node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(currentCrossingNode) + 1, previousCrossingNode);
//                //// для предыдущего узла пересечения: в конец списка ссылок добавляется сначала ссылка на узел номер 3, потом ссылка на новый узел пересечения
//                //previousCrossingNode.ConnectionList.Add(node3);
//                //previousCrossingNode.ConnectionList.Add(currentCrossingNode);
//                //// для текущего узла пересечения: ссылка на предыдущий узел пересечения вставляется после ссылки на узел номер 1 (т.е. на позицию номер 1)
//                //currentCrossingNode.ConnectionList.Insert(1, previousCrossingNode);
//                // отрицательный узел предыдущей связи (узел номер 1)
//                Polyhedron3DGraphNode node1 = previousCrossingObject.NegativeNode;
//                // общий положительный узел (узел номер 2)
//                Polyhedron3DGraphNode node2 = previousCrossingObject.PositiveNode;
//                // отрицательный узел текущей связи (узел номер 3)
//                Polyhedron3DGraphNode node3 = currentCrossingObject.NegativeNode;
//                // для узла номер 3: ссылка на предыдущей узел пересечения вставляется после ссылки на текущий узел пересечения
//                node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(currentCrossingNode) + 1, previousCrossingNode);
//                // для предыдущего узла пересечения: посде узла 1 добавляется сначала ссылка на узел номер 3, потом ссылка на новый узел пересечения
//                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node1) + 1, node3);
//                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node1) + 2,
//                                                           currentCrossingNode);
//                // для текущего узла пересечения: ссылка на предыдущий узел пересечения вставляется после ссылки на узел номер 2
//                currentCrossingNode.ConnectionList.Insert(currentCrossingNode.ConnectionList.IndexOf(node2) + 1,
//                                                          previousCrossingNode);
//            }
//                // ошибка работы алгоритма
//            else
//            {
//#warning может более специализированное исключение
//                throw new Exception("AddConns4PrevConnCurrentConnCase method incorrect work");
//            }
//        }
    }
}