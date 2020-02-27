using System;
using System.Collections.Generic;
using System.Diagnostics;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.Matrix;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.Crossing;

namespace LinearDiff3DGame.MaxStableBridge.Gamers
{
    // ������ �����
    // � ���������� �������� ������� ������ �� �������� ���� G(...Fi...)
    internal class FirstGamer
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
            angleNearnessComparer = new ApproxComp(epsilon*epsilon);
            this.matrixB = matrixB;
            this.deltaT = deltaT;
            this.mpMax = mpMax;
            this.mpMin = mpMin;
        }

        public IPolyhedron3DGraph Action(IPolyhedron3DGraph graph,
                                         Matrix fundCauchyMatrix,
                                         Int32 generationID,
                                         Matrix scalingMatrix)
        {
            // ������� (�������) D ��� ������� ������� ������ � ������ ������ �������
            Matrix matrixD = CalcMatrixD(fundCauchyMatrix, scalingMatrix);
            // ��������� ������ ������� ������ ������� Pi
            List<Vector3D> pointPiSet = new List<Vector3D>(2);
            pointPiSet.Add(new Vector3D(mpMax*matrixD[1, 1], mpMax*matrixD[2, 1], mpMax*matrixD[3, 1]));
            pointPiSet.Add(new Vector3D(mpMin*matrixD[1, 1], mpMin*matrixD[2, 1], mpMin*matrixD[3, 1]));
            // ������������ ������ ������� Pi
            Vector3D directingPi = new Vector3D(matrixD[1, 1], matrixD[2, 1], matrixD[3, 1]);
            directingPi = Vector3DUtils.NormalizeVector(directingPi);
            // ���������� ����� � ����� G(...Wi...)
            Int32 graphGWiNodeCount = graph.NodeList.Count;
            // ������ ���� G(...Fi...); ��� ���� ���� �������� �� ������, � �� ���� ����������� ����� graph G(...Wi...)
            IPolyhedron3DGraph graphGFi = BuildGFiGrid(graph, directingPi, generationID);
            // ������� ������� ������� ��� ������ ����� (��� ������������� Fi)
            for(Int32 nodeIndex = 0; nodeIndex < graphGWiNodeCount; ++nodeIndex)
            {
#warning ����� ����� !!!!!! ��������� ������������ ��������� �������� ������� �������
                IPolyhedron3DGraphNode currentNode = graphGFi.NodeList[nodeIndex];
                currentNode.SupportFuncValue += deltaT*Math.Max(-(currentNode.NodeNormal*pointPiSet[0]),
                                                                -(currentNode.NodeNormal*pointPiSet[1]));
            }
            return graphGFi;
        }

        private Matrix CalcMatrixD(Matrix fundCauchyMatrix, Matrix scalingMatrix)
        {
            Matrix matrixDBeforeScaling = fundCauchyMatrix*matrixB;
            return scalingMatrix*matrixDBeforeScaling;
        }

        // TODO : ��FUCK������
        private IPolyhedron3DGraph BuildGFiGrid(IPolyhedron3DGraph graph, Vector3D directionPi, Int32 generationID)
        {
            CrossingObjectsSearch search = new CrossingObjectsSearch(approxComparer);
            IList<CrossingObject> crossingObjects = search.GetCrossingObjects(graph, directionPi);
            CrossingObject previousCrossing = CheckCrossingNearnessAndCorrect(crossingObjects[0], directionPi);
            IPolyhedron3DGraphNode previousNode;
            if(previousCrossing.CrossingObjectType == CrossingObjectType.GraphConnection)
            {
                previousNode = BuildCrossingNode(previousCrossing, directionPi, graph.NodeList.Count, generationID);
                graph.NodeList.Add(previousNode);
                AddCrossingNodeBetweenConn(previousCrossing, previousNode);
            }
            else
                previousNode = previousCrossing.PositiveNode;
            CrossingObject firstCrossing = previousCrossing;
            IPolyhedron3DGraphNode firstNode = previousNode;
            for(Int32 crossingObjectIndex = 1; crossingObjectIndex < crossingObjects.Count; ++crossingObjectIndex)
            {
                CrossingObject currentCrossing = CheckCrossingNearnessAndCorrect(crossingObjects[crossingObjectIndex],
                                                                                 directionPi);
                IPolyhedron3DGraphNode currentNode;
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
            Double positiveNodeCosAngle = Vector3DUtils.CosAngleBetweenVectors(crossingNormal,
                                                                               crossingObject.PositiveNode.NodeNormal);
            Double negativeNodeCosAngle = Vector3DUtils.CosAngleBetweenVectors(crossingNormal,
                                                                               crossingObject.NegativeNode.NodeNormal);
            if(positiveNodeCosAngle >= negativeNodeCosAngle && angleNearnessComparer.EQ(positiveNodeCosAngle, 1))
            {
                IPolyhedron3DGraphNode crossingNode = crossingObject.PositiveNode;
                return new CrossingObject(CrossingObjectType.GraphNode, crossingNode, crossingNode);
            }
            if(negativeNodeCosAngle >= positiveNodeCosAngle && angleNearnessComparer.EQ(negativeNodeCosAngle, 1))
            {
                IPolyhedron3DGraphNode crossingNode = crossingObject.NegativeNode;
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
                Vector3D npm = Vector3DUtils.VectorProduct(plusVector, minusVector);
                // ��������� ��������� ������������ ������������ ������� � ������������� ������� Pi
                Vector3D crossingNodeNormal = Vector3DUtils.VectorProduct(npm, directionPi);
                crossingNodeNormal = Vector3DUtils.NormalizeVector(crossingNodeNormal);
                return crossingNodeNormal;
            }
            return currentCrossingObject.PositiveNode.NodeNormal;
        }

        private IPolyhedron3DGraphNode BuildCrossingNode(CrossingObject currentCrossingObject,
                                                         Vector3D directionPi,
                                                         Int32 nodeID,
                                                         Int32 generationID)
        {
            if(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
            {
                Vector3D plusNodeNormal = currentCrossingObject.PositiveNode.NodeNormal;
                Vector3D minusNodeNormal = currentCrossingObject.NegativeNode.NodeNormal;
                Vector3D crossingNodeNormal = CalcCrossingNormal(currentCrossingObject, directionPi);
                IPolyhedron3DGraphNode crossingNode = new Polyhedron3DGraphNode(nodeID, generationID, crossingNodeNormal);
                // TODO : ��������, ��� ����� ��� delta ������� � 0 � ��� ����� ��������
                // TODO : �������� ����������� �����, ��� � ����� ����� - �������
                // ������� �������� ������� ������� ��� ������������ ����
                // (l1, l):
                Double scalarProduct1 = plusNodeNormal*crossingNodeNormal;
                // (l2, l):
                Double scalarProduct2 = minusNodeNormal*crossingNodeNormal;
                // (l1, l2):
                Double scalarProduct12 = plusNodeNormal*minusNodeNormal;
                // delta = 1 - (l1, l2)*(l1, l2)
                Double delta = 1 - scalarProduct12*scalarProduct12;
                Double alpha = (scalarProduct1 - scalarProduct12*scalarProduct2)/delta;
                Double beta = (scalarProduct2 - scalarProduct12*scalarProduct1)/delta;
#warning ����� ����� !!!!!! ��������� ������������ ��������� �������� ������� �������
#warning ����� ����� !!!!!! �������, ��� ������� Pi �������� ����� ����� 0 !!!!!!!!!!
                crossingNode.SupportFuncValue = alpha*currentCrossingObject.PositiveNode.SupportFuncValue +
                                                beta*currentCrossingObject.NegativeNode.SupportFuncValue;
                return crossingNode;
            }
            Debug.Assert(
                approxComparer.EQ(
                    Vector3DUtils.ScalarProduct(currentCrossingObject.PositiveNode.NodeNormal, directionPi), 0));
            return currentCrossingObject.PositiveNode;
        }

        private static void AddCrossingNodeBetweenConn(CrossingObject crossingObject,
                                                       IPolyhedron3DGraphNode crossingNode)
        {
            Debug.Assert(crossingObject.CrossingObjectType == CrossingObjectType.GraphConnection);
            // ��������� � ������ ������ ������ ���� ������ ������� �� ������������� ���� �����, ����� �� �������������
            IPolyhedron3DGraphNode plusNode = crossingObject.PositiveNode;
            IPolyhedron3DGraphNode minusNode = crossingObject.NegativeNode;
            crossingNode.ConnectionList.Add(plusNode);
            crossingNode.ConnectionList.Add(minusNode);
            // ��� �����, ���������� �����, ������ �� ������ ���� �� ����� (������� � �������� �����) �� ������ �� ����� ����
            plusNode.ConnectionList[plusNode.ConnectionList.IndexOf(minusNode)] = crossingNode;
            minusNode.ConnectionList[minusNode.ConnectionList.IndexOf(plusNode)] = crossingNode;
        }

        private static void AddConnectionsIfNeed(CrossingObject previousCrossingObject,
                                                 IPolyhedron3DGraphNode previousCrossingNode,
                                                 CrossingObject currentCrossingObject,
                                                 IPolyhedron3DGraphNode currentCrossingNode)
        {
            if(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode &&
               currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
                AddConnections4PrevNodeCurrentConn(previousCrossingObject, currentCrossingObject, currentCrossingNode);
            if(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
               currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
                AddConnections4PrevConnCurrentNode(previousCrossingObject, previousCrossingNode, currentCrossingObject);
            if(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
               currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
                AddConnections4PrevConnCurrentConn(previousCrossingObject, previousCrossingNode, currentCrossingObject,
                                                   currentCrossingNode);
        }

        private static void AddConnections4PrevNodeCurrentConn(CrossingObject previousCrossingObject,
                                                               CrossingObject currentCrossingObject,
                                                               IPolyhedron3DGraphNode currentCrossingNode)
        {
            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode);
            Debug.Assert(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection);

            IPolyhedron3DGraphNode positiveNode = currentCrossingObject.PositiveNode;
            IPolyhedron3DGraphNode negativeNode = currentCrossingObject.NegativeNode;
            IPolyhedron3DGraphNode thirdTrNode = positiveNode.ConnectionList.GetPrevItem(currentCrossingNode);
            // ��������� ����������� �����
            Debug.Assert(thirdTrNode == negativeNode.ConnectionList.GetNextItem(currentCrossingNode));
            thirdTrNode.ConnectionList.Insert(thirdTrNode.ConnectionList.IndexOf(positiveNode), currentCrossingNode);
            currentCrossingNode.ConnectionList.Insert(currentCrossingNode.ConnectionList.IndexOf(negativeNode),
                                                      thirdTrNode);
        }

        private static void AddConnections4PrevConnCurrentNode(CrossingObject previousCrossingObject,
                                                               IPolyhedron3DGraphNode previousCrossingNode,
                                                               CrossingObject currentCrossingObject)
        {
            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection);
            Debug.Assert(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode);

            IPolyhedron3DGraphNode positiveNode = previousCrossingObject.PositiveNode;
            IPolyhedron3DGraphNode negativeNode = previousCrossingObject.NegativeNode;
            IPolyhedron3DGraphNode thirdTrNode = positiveNode.ConnectionList.GetNextItem(previousCrossingNode);
            // ��������� ����������� �����
            Debug.Assert(thirdTrNode == negativeNode.ConnectionList.GetPrevItem(previousCrossingNode));
            thirdTrNode.ConnectionList.Insert(thirdTrNode.ConnectionList.IndexOf(negativeNode), previousCrossingNode);
            previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(positiveNode),
                                                       thirdTrNode);
        }

        // TODO : ��FUCK������
        private static void AddConnections4PrevConnCurrentConn(CrossingObject previousCrossingObject,
                                                               IPolyhedron3DGraphNode previousCrossingNode,
                                                               CrossingObject currentCrossingObject,
                                                               IPolyhedron3DGraphNode currentCrossingNode)
        {
            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection);
            Debug.Assert(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection);

            // � ������ ����� ������������� ���� (������ 3�)
            if(ReferenceEquals(previousCrossingObject.NegativeNode, currentCrossingObject.NegativeNode))
            {
                // ������������� ���� ���������� ����� (���� ����� 1)
                IPolyhedron3DGraphNode node1 = previousCrossingObject.PositiveNode;
                // ����� ������������� ���� (���� ����� 2)
                IPolyhedron3DGraphNode node2 = previousCrossingObject.NegativeNode;
                // ������������� ���� ������� ����� (���� ����� 3)
                IPolyhedron3DGraphNode node3 = currentCrossingObject.PositiveNode;
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
                IPolyhedron3DGraphNode node1 = previousCrossingObject.NegativeNode;
                // ����� ������������� ���� (���� ����� 2)
                IPolyhedron3DGraphNode node2 = previousCrossingObject.PositiveNode;
                // ������������� ���� ������� ����� (���� ����� 3)
                IPolyhedron3DGraphNode node3 = currentCrossingObject.NegativeNode;
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
    }
}