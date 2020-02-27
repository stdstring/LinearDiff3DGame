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
    /// ����� �������������� � ��������������� �������� ������� ������
    /// � ���������� �������� ������� ������ �� �������� ���� G(...Fi...)
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
            // ������� (�������) D ��� ������� ������� ������ � ������ ������ �������
            Matrix matrixD = CalcMatrixD(fundCauchyMatrix, scalingMatrix);
            // ��������� ������ ������� ������ ������� Pi
            List<Vector3D> pointPiSet = new List<Vector3D>(2);
            pointPiSet.Add(new Vector3D(mpMax * matrixD[1, 1], mpMax * matrixD[2, 1], mpMax * matrixD[3, 1]));
            pointPiSet.Add(new Vector3D(mpMin * matrixD[1, 1], mpMin * matrixD[2, 1], mpMin * matrixD[3, 1]));
            // ������������ ������ ������� Pi
            Vector3D directingPi = new Vector3D(matrixD[1, 1], matrixD[2, 1], matrixD[3, 1]);
            directingPi = Vector3DUtils.NormalizeVector(directingPi);
            // ���������� ����� � ����� G(...Wi...)
            Int32 graphGWiNodeCount = graph.NodeList.Count;
            // ������ ���� G(...Fi...); ��� ���� ���� �������� �� ������, � �� ���� ����������� ����� graph G(...Wi...)
            Polyhedron3DGraph graphGFi = BuildGFiGrid(graph, directingPi, generationID);
            // ������� ������� ������� ��� ������ ����� (��� ������������� Fi)
            for(Int32 nodeIndex = 0; nodeIndex < graphGWiNodeCount; ++nodeIndex)
            {
#warning ����� ����� !!!!!! ��������� ������������ ��������� �������� ������� �������
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

        // TODO : ��FUCK������
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
                // ������ ������, ���������������� ��������, ��������� ������� ������,
                // ��� ��������� ������������ �������������� ���� ����� �� �������������
                Vector3D npm = Vector3D.VectorProduct(plusVector, minusVector);
                // ��������� ��������� ������������ ������������ ������� � ������������� ������� Pi
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
                // TODO : ��������, ��� ����� ��� delta ������� � 0 � ��� ����� ��������
                // ������� �������� ������� ������� ��� ������������ ����
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
#warning ����� ����� !!!!!! ��������� ������������ ��������� �������� ������� �������
#warning ����� ����� !!!!!! �������, ��� ������� Pi �������� ����� ����� 0 !!!!!!!!!!
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
            // ��������� � ������ ������ ������ ���� ������ ������� �� ������������� ���� �����, ����� �� �������������
            Polyhedron3DGraphNode plusNode = crossingObject.PositiveNode;
            Polyhedron3DGraphNode minusNode = crossingObject.NegativeNode;
            crossingNode.ConnectionList.Add(plusNode);
            crossingNode.ConnectionList.Add(minusNode);
            // ��� �����, ���������� �����, ������ �� ������ ���� �� ����� (������� � �������� �����) �� ������ �� ����� ����
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
            // ��������� ����������� �����
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
            // ��������� ����������� �����
            Debug.Assert(thirdTrNode == negativeNode.ConnectionList.GetPrevItem(previousCrossingNode));
            thirdTrNode.ConnectionList.Insert(thirdTrNode.ConnectionList.IndexOf(negativeNode), previousCrossingNode);
            previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(positiveNode),
                                                       thirdTrNode);
        }

        // TODO : ��FUCK������
        private static void AddConnections4PrevConnCurrentConn(CrossingObject previousCrossingObject,
                                                               Polyhedron3DGraphNode previousCrossingNode,
                                                               CrossingObject currentCrossingObject,
                                                               Polyhedron3DGraphNode currentCrossingNode)
        {
            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection);
            Debug.Assert(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection);

            // � ������ ����� ������������� ���� (������ 3�)
            if(ReferenceEquals(previousCrossingObject.NegativeNode, currentCrossingObject.NegativeNode))
            {
                // ������������� ���� ���������� ����� (���� ����� 1)
                Polyhedron3DGraphNode node1 = previousCrossingObject.PositiveNode;
                // ����� ������������� ���� (���� ����� 2)
                Polyhedron3DGraphNode node2 = previousCrossingObject.NegativeNode;
                // ������������� ���� ������� ����� (���� ����� 3)
                Polyhedron3DGraphNode node3 = currentCrossingObject.PositiveNode;
                // ��� ���� ����� 3: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ���� 1
                node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(node1) + 1,
                                            previousCrossingNode);
                // ��� ����������� ���� �����������: ����� ���� 2 ����������� ������� ������ �� ����� ���� �����������, ����� ������ �� ���� ����� 3
                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node2) + 1,
                                                           currentCrossingNode);
                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node2) + 2,
                                                           node3);
                // ��� �������� ���� �����������: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ���� ����� 3
                currentCrossingNode.ConnectionList.Insert(currentCrossingNode.ConnectionList.IndexOf(node3) + 1,
                                                          previousCrossingNode);
            }
                // � ������ ����� ������������� ���� (������ 3�)
            else if(ReferenceEquals(previousCrossingObject.PositiveNode, currentCrossingObject.PositiveNode))
            {
                // ������������� ���� ���������� ����� (���� ����� 1)
                Polyhedron3DGraphNode node1 = previousCrossingObject.NegativeNode;
                // ����� ������������� ���� (���� ����� 2)
                Polyhedron3DGraphNode node2 = previousCrossingObject.PositiveNode;
                // ������������� ���� ������� ����� (���� ����� 3)
                Polyhedron3DGraphNode node3 = currentCrossingObject.NegativeNode;
                // ��� ���� ����� 3: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ������� ���� �����������
                node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(currentCrossingNode) + 1,
                                            previousCrossingNode);
                // ��� ����������� ���� �����������: ����� ���� 1 ����������� ������� ������ �� ���� ����� 3, ����� ������ �� ����� ���� �����������
                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node1) + 1,
                                                           node3);
                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node1) + 2,
                                                           currentCrossingNode);
                // ��� �������� ���� �����������: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ���� ����� 2
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
        ///// ����� BuildGFiGrid ������ ����� G(...Fi...) (��. ��������)
        ///// </summary>
        ///// <param name="graph">����, ������� ������������� �� ����� G(...Fi...)</param>
        ///// <param name="directingVectorPi">������������ ������ ������� Pi</param>
        ///// <param name="generationID">ID ���������</param>
        ///// <returns>����� G(...Fi...)</returns>
        //private Polyhedron3DGraph BuildGFiGrid(Polyhedron3DGraph graph, Vector3D directingVectorPi, Int32 generationID)
        //{
        //    // ������ ��� ������ ����������� ����� � G(...Pi...)
        //    CrossingObjectFinder finder = new CrossingObjectFinder(m_ApproxComparer);

        //    // ������ (�����������) ������ �����������
        //    CrossingObject firstCrossingObject = finder.GetFirstCrossingObject(graph.NodeList[0], directingVectorPi);
        //    // ������� ������ �����������
        //    CrossingObject currentCrossingObject = firstCrossingObject;
        //    // ������ ���� �� ����������� �������� ������� � G(...Pi...) � ���������� ���
        //    // ���� ���� ���� ����������� � ������ �����, �� ��������� ��� � ��������������� ������ �� ������ ����
        //    Polyhedron3DGraphNode firstCrossingNode = BuildCrossingNode(currentCrossingObject, graph, directingVectorPi, generationID);
        //    // ������� ���� �����������
        //    Polyhedron3DGraphNode currentCrossingNode = firstCrossingNode;

        //    if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
        //    {
        //        graph.NodeList.Add(currentCrossingNode);
        //        AddCrossingNodeBetweenConn(currentCrossingObject.PositiveNode, currentCrossingObject.NegativeNode, currentCrossingNode);
        //    }

        //    // ���� (���� ������� ������ �� ������ ������ ������������)
        //    do
        //    {
        //        // ���������� ������ �����������
        //        CrossingObject previousCrossingObject = currentCrossingObject;
        //        // ���������� ���� �����������
        //        Polyhedron3DGraphNode previousCrossingNode = currentCrossingNode;
        //        // �������� ��������� �� �������� ������ (�����, ���� ����) � ������ ��� �������
        //        currentCrossingObject = finder.GetNextCrossingObject(currentCrossingObject, currentCrossingNode, directingVectorPi);
        //        // ������ ���� �� ����������� �������� ������� � G(...Pi...)
        //        // ���� ���� ���� ����������� � ������ ����� (���� ���� ����� �������������� � ������ �����, ���� ������� ������ � ����, ���� ���� ��������� �������� ���� ����� � �� � ��� ������), �� ��������� ��� � ��������������� ������ �� ������ ����
        //        // �������� ������������ ������ ���� �� ������ � ������ (�����������) ������ ����������� (��� �������� ���������� ���������)
        //        currentCrossingNode = (currentCrossingObject == firstCrossingObject ?
        //                               firstCrossingNode :
        //                               BuildCrossingNode(currentCrossingObject, graph, directingVectorPi, generationID));
        //        if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
        //            currentCrossingObject != firstCrossingObject)
        //        {
        //            graph.NodeList.Add(currentCrossingNode);
        //            AddCrossingNodeBetweenConn(currentCrossingObject.PositiveNode, currentCrossingObject.NegativeNode, currentCrossingNode);
        //        }

        //        // ���� ���������� � ������� ������� � ����
        //        if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode &&
        //            currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
        //        {
        //            // ������� � ��������� �������� �����
        //            // continue;
        //        }

        //        // ���� ���������� ������ ����, � ������� �����
        //        if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode &&
        //            currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
        //        {
        //            // ������ ����� ����� ���������� ����� � ����� ����������� �� ������� �������
        //            AddConns4PrevNodeCurrentConnCase(previousCrossingObject, previousCrossingNode, currentCrossingObject, currentCrossingNode);
        //        }

        //        // ���� ���������� ������ �����, � ������� ����
        //        if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
        //            currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
        //        {
        //            // ������ ����� ����� ����� ����������� �� ���������� ������� � ������� ����
        //            AddConns4PrevConnCurrentNodeCase(previousCrossingObject, previousCrossingNode, currentCrossingObject, currentCrossingNode);
        //        }

        //        // ���� ���������� � ������� ������� - �����
        //        if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
        //            currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
        //        {
        //            // ������ ����� ����� ����� ����������� �� ���������� ������� � ����� ����������� �� ������� �������
        //            // ������ ����� ����� ����� ����������� �� ���������� ������� � ����� ������� �����, ������� �� ����������� ���������� �����
        //            AddConns4PrevConnCurrentConnCase(previousCrossingObject, previousCrossingNode, currentCrossingObject, currentCrossingNode);
        //        }
        //    }
        //    while (currentCrossingObject != firstCrossingObject);
        //    // ���� (���� ������� ������ �� ������ ������ ������������)

        //    return graph;
        //}

//        /// <summary>
//        /// ����� BuildGFiGrid ������ ����� G(...Fi...) (��. ��������)
//        /// </summary>
//        /// <param name="graph">����, ������� ������������� �� ����� G(...Fi...)</param>
//        /// <param name="directingVectorPi">������������ ������ ������� Pi</param>
//        /// <param name="generationID">ID ���������</param>
//        /// <returns>����� G(...Fi...)</returns>
//        private Polyhedron3DGraph BuildGFiGrid(Polyhedron3DGraph graph, Vector3D directingVectorPi, Int32 generationID)
//        {
//            const Double epsilon = 0.02;
//            ApproxComp cosAngleComparer = new ApproxComp(epsilon*epsilon);

//            // ������ ��� ������ ����������� ����� � G(...Pi...)
//            CrossingObjectsSearch finder = new CrossingObjectsSearch(m_ApproxComparer);

//            // ������ (�����������) ������ �����������
//            CrossingObject firstCrObject = finder.GetFirstCrossingObject(graph.NodeList[0], directingVectorPi);
//            // ������ ���� �� ����������� �������� ������� � G(...Pi...) � ���������� ���
//            // ���� ���� ���� ����������� � ������ �����, �� ��������� ��� � ��������������� ������ �� ������ ����
//            Polyhedron3DGraphNode firstCrNode = BuildCrossingNode(firstCrObject, graph, directingVectorPi, generationID);

//            // ������� ������ �����������
//            CrossingObject currentCrObject = firstCrObject;
//            // ������� ���� �����������
//            Polyhedron3DGraphNode currentCrNode = firstCrNode;

//            // ���� �����������, ������� ����� ��������
//            Polyhedron3DGraphNode actualCrNode = firstCrNode;
//            // ������ �����������, ������� ����� ��������
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
//            // ���� (���� ������� ������ �� ������ ������ ������������)
//            do
//            {
//                // �������� ��������� �� �������� ������ (�����, ���� ����) � ������ ��� �������
//                if (graph.NodeList[graph.NodeList.Count - 1] == currentCrNode)
//                {
//                    currentCrObject = finder.GetNextCrossingObject(currentCrObject, currentCrNode, directingVectorPi);
//                }
//                else
//                {
//                    currentCrObject = finder.GetNextCrossingObject(currentCrObject, directingVectorPi);
//                }
//                // ������ ���� �� ����������� �������� ������� � G(...Pi...)
//                // ���� ���� ���� ����������� � ������ ����� (���� ���� ����� �������������� � ������ �����, ���� ������� ������ � ����, ���� ���� ��������� �������� ���� ����� � �� � ��� ������),
//                // �� ��������� ��� � ��������������� ������ �� ������ ����
//                // �������� ������������ ������ ���� �� ������ � ������ (�����������) ������ ����������� (��� �������� ���������� ���������)
//                currentCrNode = (currentCrObject == firstCrObject
//                                     ? firstCrNode
//                                     : BuildCrossingNode(currentCrObject, graph, directingVectorPi, generationID));
//                //
//                reachFirstCrObject = (currentCrObject == firstCrObject);
//                // ���������� ������ �����������
//                CrossingObject prevActualCrObject = actualCrObject;
//                // ���������� ���� �����������
//                Polyhedron3DGraphNode prevActualCrNode = actualCrNode;
//                // ���� �����������, ������� ����� ��������
//                actualCrNode = currentCrNode;
//                // ������ �����������, ������� ����� ��������
//                actualCrObject = CheckNodesNearnessAndCorrect(currentCrObject, ref actualCrNode, cosAngleComparer);
//                //
//                if (actualCrObject.CrossingObjectType == CrossingObjectType.GraphConnection && !reachFirstCrObject)
//                {
//                    graph.NodeList.Add(actualCrNode);
//                    AddCrossingNodeBetweenConn(actualCrObject.PositiveNode, actualCrObject.NegativeNode, actualCrNode);
//                }

//                // ���� ���������� � ������� ������� � ����
//                if (prevActualCrObject.CrossingObjectType == CrossingObjectType.GraphNode &&
//                    actualCrObject.CrossingObjectType == CrossingObjectType.GraphNode)
//                {
//                    // ������� � ��������� �������� �����
//                    // continue;
//                }

//                // ���� ���������� ������ ����, � ������� �����
//                if (prevActualCrObject.CrossingObjectType == CrossingObjectType.GraphNode &&
//                    actualCrObject.CrossingObjectType == CrossingObjectType.GraphConnection)
//                {
//                    // ������ ����� ����� ���������� ����� � ����� ����������� �� ������� �������
//                    AddConns4PrevNodeCurrentConnCase(prevActualCrObject, prevActualCrNode, actualCrObject, actualCrNode);
//                }

//                // ���� ���������� ������ �����, � ������� ����
//                if (prevActualCrObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
//                    actualCrObject.CrossingObjectType == CrossingObjectType.GraphNode)
//                {
//                    // ������ ����� ����� ����� ����������� �� ���������� ������� � ������� ����
//                    AddConns4PrevConnCurrentNodeCase(prevActualCrObject, prevActualCrNode, actualCrObject, actualCrNode);
//                }

//                // ���� ���������� � ������� ������� - �����
//                if (prevActualCrObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
//                    actualCrObject.CrossingObjectType == CrossingObjectType.GraphConnection)
//                {
//                    // ������ ����� ����� ����� ����������� �� ���������� ������� � ����� ����������� �� ������� �������
//                    // ������ ����� ����� ����� ����������� �� ���������� ������� � ����� ������� �����, ������� �� ����������� ���������� �����
//                    AddConns4PrevConnCurrentConnCase(prevActualCrObject, prevActualCrNode, actualCrObject, actualCrNode);
//                }
//            } while (!reachFirstCrObject);
//            // ���� (���� ������� ������ �� ������ ������ ������������)

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
//        /// ����� CalcCrossingNodeNormal ��������� ������� ���� �� ������� ����������� ����� � G(...Pi...)
//        /// </summary>
//        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Pi...)</param>
//        /// <param name="directingVectorPi">������������ ������ ������� Pi</param>
//        /// <returns>������� ���� �� ������� ����������� ����� � G(...Pi...)</returns>
//        private Vector3D CalcCrossingNodeNormal(CrossingObject currentCrossingObject, Vector3D directingVectorPi)
//        {
//            Vector3D crossingNodeNormal;

//            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
//            {
//                Vector3D plusVector = currentCrossingObject.PositiveNode.NodeNormal;
//                Vector3D minusVector = currentCrossingObject.NegativeNode.NodeNormal;
//                // ������ ������, ���������������� ��������, ��������� ������� ������,
//                // ��� ��������� ������������ �������������� ���� ����� �� �������������
//                Vector3D npm = Vector3D.VectorProduct(plusVector, minusVector);
//                // ��������� ��������� ������������ ������������ ������� � ������������� ������� Pi
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
//        /// ����� BuildCrossingNode ������� � ���������� ���� �� ������� ����������� ����� � G(...Pi...)
//        /// </summary>
//        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Pi...)</param>
//        /// <param name="graph">����</param>
//        /// <param name="directingVectorPi">������������ ������ ������� Pi</param>
//        /// <param name="generationID">ID ���������</param>
//        /// <returns>��������� ���� �� ������� ����������� ����� � G(...Pi...)</returns>
//        private Polyhedron3DGraphNode BuildCrossingNode(CrossingObject currentCrossingObject, Polyhedron3DGraph graph,
//                                                        Vector3D directingVectorPi, Int32 generationID)
//        {
//            Polyhedron3DGraphNode crossingNode;

//            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
//            {
//                Vector3D plusNodeNormal = currentCrossingObject.PositiveNode.NodeNormal;
//                Vector3D minusNodeNormal = currentCrossingObject.NegativeNode.NodeNormal;
//                Vector3D crossingNodeNormal = CalcCrossingNodeNormal(currentCrossingObject, directingVectorPi);

//                // ������ ����, ��������� � ���������� (����) �������� � ���������� ���
//                crossingNode = new Polyhedron3DGraphNode(graph.NodeList.Count, generationID, crossingNodeNormal);
//                // ������� �������� ������� ������� ��� ������������ ����
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

//#warning ����� ����� !!!!!! ��������� ������������ ��������� �������� ������� �������
//#warning ����� ����� !!!!!! �������, ��� ������� Pi �������� ����� ����� 0 !!!!!!!!!!
//                crossingNode.SupportFuncValue = alpha*currentCrossingObject.PositiveNode.SupportFuncValue +
//                                                beta*currentCrossingObject.NegativeNode.SupportFuncValue;
//            }
//            else
//            {
//                // ��������� ��������� ������������ �������, ���������� � ������� �����, � ������������� ������� Pi
//                // ���� ��������� ������������ <> 0, �� ��� ������ ������ ���������
//#warning Check is absent !!!

//                crossingNode = currentCrossingObject.PositiveNode;
//            }

//            return crossingNode;
//        }

//        /// <summary>
//        /// ����� AddCrossingNodeBetweenConn ��������� ���� crossingNode �� ����������� ����� � G(...Pi...) � ��������������� ������� ������/��������� ������
//        /// </summary>
//        /// <param name="connPlusNode">������������� ���� ������������ �����</param>
//        /// <param name="connMinusNode">������������� ���� ������������ �����</param>
//        /// <param name="crossingNode">���� �� ����������� ����� � G(...Pi...)</param>
//        private void AddCrossingNodeBetweenConn(Polyhedron3DGraphNode connPlusNode, Polyhedron3DGraphNode connMinusNode,
//                                                Polyhedron3DGraphNode crossingNode)
//        {
//            // ���������� �� ����� ����� �������
//            /*// ��������� ����� ���� � ������ ����� �����
//            graph.NodeList.Add(crossingNode);*/
//            // ���������� �� ����� ����� �������

//            // ��������� � ������ ������ ������ ���� ������ ������� �� ������������� ���� �����, ����� �� �������������
//            crossingNode.ConnectionList.Add(connPlusNode);
//            crossingNode.ConnectionList.Add(connMinusNode);
//            // ��� �����, ���������� �����, ������ �� ������ ���� �� ����� (������� � �������� �����) �� ������ �� ����� ����
//            connPlusNode.ConnectionList[connPlusNode.ConnectionList.IndexOf(connMinusNode)] = crossingNode;
//            connMinusNode.ConnectionList[connMinusNode.ConnectionList.IndexOf(connPlusNode)] = crossingNode;
//        }

//        /// <summary>
//        /// ����� AddConns4PrevNodeCurrentConnCase ��������� ����������� ����� � ������, ���� ���������� ������ ����������� - ����, � ������� - �����
//        /// ����� ����������� ��� ����, ����� ���� ��������� �����������������
//        /// </summary>
//        /// <param name="previousCrossingObject">���������� ������ ����������� ����� � G(...Pi...)</param>
//        /// <param name="previousCrossingNode">���� �� ���������� ����������� ����� � G(...Pi...)</param>
//        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Pi...)</param>
//        /// <param name="currentCrossingNode">���� �� ������� ����������� ����� � G(...Pi...)</param>
//        private void AddConns4PrevNodeCurrentConnCase(CrossingObject previousCrossingObject,
//                                                      Polyhedron3DGraphNode previousCrossingNode,
//                                                      CrossingObject currentCrossingObject,
//                                                      Polyhedron3DGraphNode currentCrossingNode)
//        {
//            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode &&
//                         currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection,
//                         "previous crossing object must be node and current crossing object - connection");

//            //// ������ ����� ����� ���������� ����� � ����� ����������� �� ������� ������� :

//            //// ������������� ���� ������� �����
//            //Polyhedron3DGraphNode connMinusNode = currentCrossingObject.NegativeNode;
//            //// ������ �� ������� ���� ����������� � ������ ������ ����������� ���� ��������� ����� ������ �� ������������� ���� ������� �����
//            ///*Int32 PrevNode2CurrentMinusNodeConnIndex = PreviousCrossingNode.GetConnectionIndex(CurrentConnMinusNode);
//            //PreviousCrossingNode.InsertNodeConnection(PrevNode2CurrentMinusNodeConnIndex + 1, CurrentCrossingNode);*/
//            //previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(connMinusNode) + 1,
//            //                                           currentCrossingNode);
//            //// ������ �� ���������� ���� ��������� ����� ������ �� ������������� ���� ������� ����� (�� ������� ����� 1)
//            //currentCrossingNode.ConnectionList.Insert(1, previousCrossingNode);

//            Polyhedron3DGraphNode positiveNode = currentCrossingObject.PositiveNode;
//            Polyhedron3DGraphNode negativeNode = currentCrossingObject.NegativeNode;
//            Polyhedron3DGraphNode thirdTrNode = positiveNode.ConnectionList.GetPrevItem(currentCrossingNode);
//            thirdTrNode.ConnectionList.Insert(thirdTrNode.ConnectionList.IndexOf(positiveNode), currentCrossingNode);
//            currentCrossingNode.ConnectionList.Insert(currentCrossingNode.ConnectionList.IndexOf(negativeNode),
//                                                      thirdTrNode);
//        }

//        /// <summary>
//        /// ����� AddConns4PrevConnCurrentNodeCase ��������� ����������� ����� � ������, ���� ���������� ������ ����������� - �����, � ������� - ����
//        /// ����� ����������� ��� ����, ����� ���� ��������� �����������������
//        /// </summary>
//        /// <param name="previousCrossingObject">���������� ������ ����������� ����� � G(...Pi...)</param>
//        /// <param name="previousCrossingNode">���� �� ���������� ����������� ����� � G(...Pi...)</param>
//        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Pi...)</param>
//        /// <param name="currentCrossingNode">���� �� ������� ����������� ����� � G(...Pi...)</param>
//        private void AddConns4PrevConnCurrentNodeCase(CrossingObject previousCrossingObject,
//                                                      Polyhedron3DGraphNode previousCrossingNode,
//                                                      CrossingObject currentCrossingObject,
//                                                      Polyhedron3DGraphNode currentCrossingNode)
//        {
//            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
//                         currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode,
//                         "previous crossing object must be connection and current crossing object - node");

//            //// ������ ����� ����� ����� ����������� �� ���������� ������� � ������� ���� :

//            //// ������������� ���� ���������� �����
//            //Polyhedron3DGraphNode connPlusNode = previousCrossingObject.PositiveNode;
//            //// ������ �� ���������� ���� ����������� � ������ ������ �������� ���� ��������� ����� ������ �� ������������� ���� ���������� �����
//            ///*Int32 CurrentNode2PrevPlusNodeConnIndex = CurrentCrossingNode.GetConnectionIndex(PreviousConnPlusNode);
//            //CurrentCrossingNode.InsertNodeConnection(CurrentNode2PrevPlusNodeConnIndex + 1, PreviousCrossingNode);*/
//            //currentCrossingNode.ConnectionList.Insert(currentCrossingNode.ConnectionList.IndexOf(connPlusNode) + 1,
//            //                                          previousCrossingNode);
//            //// ������ �� ������� ���� ��������� � ����� ������ ������ ����������� ����
//            //previousCrossingNode.ConnectionList.Add(currentCrossingNode);

//            Polyhedron3DGraphNode positiveNode = previousCrossingObject.PositiveNode;
//            Polyhedron3DGraphNode negativeNode = previousCrossingObject.NegativeNode;
//            Polyhedron3DGraphNode thirdTrNode = positiveNode.ConnectionList.GetNextItem(previousCrossingNode);
//            thirdTrNode.ConnectionList.Insert(thirdTrNode.ConnectionList.IndexOf(negativeNode), previousCrossingNode);
//            previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(positiveNode),
//                                                       thirdTrNode);
//        }

//        /// <summary>
//        /// ����� AddConns4PrevConnCurrentConnCase ��������� ����������� ����� � ������, ���� � ����������, � ������� ������� ����������� - �����
//        /// ����� ����������� ��� ����, ����� ���� ��������� �����������������
//        /// </summary>
//        /// <param name="previousCrossingObject">���������� ������ ����������� ����� � G(...Pi...)</param>
//        /// <param name="previousCrossingNode">���� �� ���������� ����������� ����� � G(...Pi...)</param>
//        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Pi...)</param>
//        /// <param name="currentCrossingNode">���� �� ������� ����������� ����� � G(...Pi...)</param>
//        private void AddConns4PrevConnCurrentConnCase(CrossingObject previousCrossingObject,
//                                                      Polyhedron3DGraphNode previousCrossingNode,
//                                                      CrossingObject currentCrossingObject,
//                                                      Polyhedron3DGraphNode currentCrossingNode)
//        {
//            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
//                         currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection,
//                         "previous and current crossing objects must be connections");

//            // ������ ����� ����� ����� ����������� �� ���������� ������� � ����� ����������� �� ������� �������
//            // ������ ����� ����� ����� ����������� �� ���������� ������� � ����� ������� �����, ������� �� ����������� ���������� �����
//            // � ������ ����� ������������� ���� (������ 3�)
//            if (ReferenceEquals(previousCrossingObject.NegativeNode, currentCrossingObject.NegativeNode))
//            {
//                //// ������������� ���� ���������� ����� (���� ����� 1)
//                //Polyhedron3DGraphNode node1 = previousCrossingObject.PositiveNode;
//                //// ����� ������������� ���� (���� ����� 2)
//                //// Polyhedron3DGraphNode node2 = previousCrossingObject.NegativeNode;
//                //// ������������� ���� ������� ����� (���� ����� 3)
//                //Polyhedron3DGraphNode node3 = currentCrossingObject.PositiveNode;
//                //// ��� ���� ����� 3: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ���� 1                
//                //node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(node1) + 1, previousCrossingNode);
//                //// ��� ����������� ���� �����������: � ����� ������ ������ ����������� ������� ������ �� ����� ���� �����������, ����� ������ �� ���� ����� 3
//                //previousCrossingNode.ConnectionList.Add(currentCrossingNode);
//                //previousCrossingNode.ConnectionList.Add(node3);
//                //// ��� �������� ���� �����������: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ���� ����� 3 (�.�. �� ������� ����� 1)
//                //currentCrossingNode.ConnectionList.Insert(1, previousCrossingNode);
//                // ������������� ���� ���������� ����� (���� ����� 1)
//                Polyhedron3DGraphNode node1 = previousCrossingObject.PositiveNode;
//                // ����� ������������� ���� (���� ����� 2)
//                Polyhedron3DGraphNode node2 = previousCrossingObject.NegativeNode;
//                // ������������� ���� ������� ����� (���� ����� 3)
//                Polyhedron3DGraphNode node3 = currentCrossingObject.PositiveNode;
//                // ��� ���� ����� 3: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ���� 1                
//                node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(node1) + 1, previousCrossingNode);
//                // ��� ����������� ���� �����������: ����� ���� 2 ����������� ������� ������ �� ����� ���� �����������, ����� ������ �� ���� ����� 3
//                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node2) + 1,
//                                                           currentCrossingNode);
//                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node2) + 2, node3);
//                // ��� �������� ���� �����������: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ���� ����� 3
//                currentCrossingNode.ConnectionList.Insert(currentCrossingNode.ConnectionList.IndexOf(node3) + 1,
//                                                          previousCrossingNode);
//            }
//                // � ������ ����� ������������� ���� (������ 3�)
//            else if (ReferenceEquals(previousCrossingObject.PositiveNode, currentCrossingObject.PositiveNode))
//            {
//                //// ������������� ���� ���������� ����� (���� ����� 1)
//                ////Polyhedron3DGraphNode node1 = previousCrossingObject.NegativeNode;
//                //// ����� ������������� ���� (���� ����� 2)
//                //// Polyhedron3DGraphNode node2 = previousCrossingObject.PositiveNode;
//                //// ������������� ���� ������� ����� (���� ����� 3)
//                //Polyhedron3DGraphNode node3 = currentCrossingObject.NegativeNode;
//                //// ��� ���� ����� 3: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ������� ���� �����������
//                //node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(currentCrossingNode) + 1, previousCrossingNode);
//                //// ��� ����������� ���� �����������: � ����� ������ ������ ����������� ������� ������ �� ���� ����� 3, ����� ������ �� ����� ���� �����������
//                //previousCrossingNode.ConnectionList.Add(node3);
//                //previousCrossingNode.ConnectionList.Add(currentCrossingNode);
//                //// ��� �������� ���� �����������: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ���� ����� 1 (�.�. �� ������� ����� 1)
//                //currentCrossingNode.ConnectionList.Insert(1, previousCrossingNode);
//                // ������������� ���� ���������� ����� (���� ����� 1)
//                Polyhedron3DGraphNode node1 = previousCrossingObject.NegativeNode;
//                // ����� ������������� ���� (���� ����� 2)
//                Polyhedron3DGraphNode node2 = previousCrossingObject.PositiveNode;
//                // ������������� ���� ������� ����� (���� ����� 3)
//                Polyhedron3DGraphNode node3 = currentCrossingObject.NegativeNode;
//                // ��� ���� ����� 3: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ������� ���� �����������
//                node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(currentCrossingNode) + 1, previousCrossingNode);
//                // ��� ����������� ���� �����������: ����� ���� 1 ����������� ������� ������ �� ���� ����� 3, ����� ������ �� ����� ���� �����������
//                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node1) + 1, node3);
//                previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(node1) + 2,
//                                                           currentCrossingNode);
//                // ��� �������� ���� �����������: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ���� ����� 2
//                currentCrossingNode.ConnectionList.Insert(currentCrossingNode.ConnectionList.IndexOf(node2) + 1,
//                                                          previousCrossingNode);
//            }
//                // ������ ������ ���������
//            else
//            {
//#warning ����� ����� ������������������ ����������
//                throw new Exception("AddConns4PrevConnCurrentConnCase method incorrect work");
//            }
//        }
    }
}