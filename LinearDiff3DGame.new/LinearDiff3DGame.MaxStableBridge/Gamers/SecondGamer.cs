using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.Matrix;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.Crossing;
using LinearDiff3DGame.MaxStableBridge.SuspiciousConnections;

namespace LinearDiff3DGame.MaxStableBridge.Gamers
{
    // ������ �����
    // � ���������� �������� ������� ������ �� �������� ������ ������ �, �� ������� ����� ���������� ��������� ���������� ������� fi_i
    internal class SecondGamer
    {
        public SecondGamer(GamerInitData initData)
        {
            approxComparer = initData.ApproxComp;
            matrixC = initData.Matrix;
            mqMax = initData.MaxSection;
            mqMin = initData.MinSection;
            deltaT = initData.DeltaT;
        }

        [Obsolete]
        public SecondGamer(ApproxComp approxComparer, Matrix matrixC, Double deltaT, Double mqMax, Double mqMin)
        {
            this.approxComparer = approxComparer;
            this.matrixC = matrixC;
            this.deltaT = deltaT;
            this.mqMax = mqMax;
            this.mqMin = mqMin;
        }

        // �������� ������� ������ ��� �������� (������) �� ������ ����
        public IPolyhedron3DGraph Action(IPolyhedron3DGraph graph,
                                         Matrix fundCauchyMatrix,
                                         SuspiciousConnectionSet connSet,
                                         Matrix scalingMatrix)
        {
            // ������� (�������) E ��� ������� ������� ������ � ������ ������ �������
            Matrix matrixE = CalcMatrixE(fundCauchyMatrix, scalingMatrix);

            // ��������� ������ ������� ������ ������� Qi
            List<Vector3D> pointQiSet = new List<Vector3D>(2);
            pointQiSet.Add(new Vector3D(mqMax*matrixE[1, 1], mqMax*matrixE[2, 1], mqMax*matrixE[3, 1]));
            pointQiSet.Add(new Vector3D(mqMin*matrixE[1, 1], mqMin*matrixE[2, 1], mqMin*matrixE[3, 1]));

            // ������������ ������ ������� Qi
            Vector3D directingQi = new Vector3D(matrixE[1, 1], matrixE[2, 1], matrixE[3, 1]);
            directingQi = Vector3DUtils.NormalizeVector(directingQi);

            // ������� ������� eta_i ��� ����� �����
            for(Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
            {
#warning ����� ����� !!!!!! ��������� ������������ ��������� �������� ������� eta_i
                IPolyhedron3DGraphNode currentNode = graph.NodeList[nodeIndex];
                currentNode.SupportFuncValue -= deltaT*Math.Max(currentNode.NodeNormal*pointQiSet[0],
                                                                currentNode.NodeNormal*pointQiSet[1]);
            }
            // ������� ������� eta_i ��� ����� �����

            FillSuspiciousConnectionSet(graph, directingQi, connSet);
            return graph;
        }

        private Matrix CalcMatrixE(Matrix fundCauchyMatrix, Matrix scalingMatrix)
        {
            Matrix matrixEBeforeScaling = fundCauchyMatrix*matrixC;
            return scalingMatrix*matrixEBeforeScaling;
        }

        // ���������� ������ ������ � �������, �� ������� ����� ���������� ��������� ���������� ������� fi_i
        private void FillSuspiciousConnectionSet(IPolyhedron3DGraph graph,
                                                 Vector3D directingVectorQi,
                                                 SuspiciousConnectionSet connSet)
        {
            // ������ ��� ������ ����������� ����� � Zi
            CrossingObjectsSearch finder = new CrossingObjectsSearch(approxComparer);

            // ������ (�����������) ������ �����������
            CrossingObject firstCrossingObject = finder.GetFirstCrossingObject(graph.NodeList[0], directingVectorQi);
            // ������� ������ �����������
            CrossingObject currentCrossingObject = firstCrossingObject;
            // ��������� � ����� � ��� ������ ����������� � ����������� �������� � ��� �����
            AddConns2SuspiciousConnectionSet(currentCrossingObject, connSet);

            // ���� (���� ������� ������ �� ������ ������ ������������)
            do
            {
                // �������� ��������� �� �������� ������ (�����, ���� ����) � ������ ��� �������
                currentCrossingObject = finder.GetNextCrossingObject(currentCrossingObject, directingVectorQi);
                // ��������� � ����� � ��� ������ ����������� � ����������� �������� � ��� �����
                AddConns2SuspiciousConnectionSet(currentCrossingObject, connSet);
            } while(currentCrossingObject != firstCrossingObject);
            // ���� (���� ������� ������ �� ������ ������ ������������)
        }

        // ���������� � ����� � ������ ����������� � ����������� �������� � ��� �����
        private static void AddConns2SuspiciousConnectionSet(CrossingObject currentCrossingObject,
                                                             SuspiciousConnectionSet connSet)
        {
            // ���� ������� ������ � ����
            if(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
            {
                IPolyhedron3DGraphNode currentNode = currentCrossingObject.PositiveNode;
                // ���� �� ���� ������ �������� ���� (�������� ������� �����������)
                for(Int32 connIndex = 0; connIndex < currentNode.ConnectionList.Count; ++connIndex)
                {
                    // ���� ������� ����� ����������� � ������ �, �� ��������� �� � ���� �����
                    connSet.AddConnection(currentNode, currentNode.ConnectionList[connIndex]);
                }
                // ���� �� ���� ������ �������� ���� (�������� ������� �����������)
            }
                // ���� ������� ������ � �����
            else
            {
                IPolyhedron3DGraphNode positiveNode = currentCrossingObject.PositiveNode;
                IPolyhedron3DGraphNode negativeNode = currentCrossingObject.NegativeNode;
                // ���� ������� ����� � ������ � �����������, �� ��� ����� ��������� � ����� �
                connSet.AddConnection(positiveNode, negativeNode);
                // ���� �� ���� ������ �������������� ���� ������� �����
                for(Int32 connIndex = 0; connIndex < positiveNode.ConnectionList.Count; ++connIndex)
                {
                    // ���� ��������������� ����� �������������� ���� ������� ����� ����������� � ������ �, �� ��������� �� � ���� �����
                    connSet.AddConnection(positiveNode, positiveNode.ConnectionList[connIndex]);
                }
                // ���� �� ���� ������ �������������� ���� ������� �����
                for(Int32 connIndex = 0; connIndex < negativeNode.ConnectionList.Count; ++connIndex)
                {
                    // ���� ��������������� ����� �������������� ���� ������� ����� ����������� � ������ �, �� ��������� �� � ���� �����
                    connSet.AddConnection(negativeNode, negativeNode.ConnectionList[connIndex]);
                }
                /*// ��� �������������� ���� �����, ����� ������ �� ���������� ���� ������������ 	������ �����; ������� ���� ���� � ����� 1
                Polyhedron3DGraphNode node1 = positiveNode.ConnectionList.GetPrevItem(negativeNode);
                Double scalarProduct1 = node1.NodeNormal * directingVectorQi;
                // ��� �������������� ���� �����, ����� ������ �� ��������� ���� ������������ 	������ �����; ������� ���� ���� � ����� 2
                Polyhedron3DGraphNode node2 = positiveNode.ConnectionList.GetNextItem(negativeNode);
                Double scalarProduct2 = node2.NodeNormal * directingVectorQi;

                // ���� ���� 1 �������� ������������� �����, ��
                if (approxComparer.GT(scalarProduct1, 0))
                {
                    // ���� ����� ���� 1 � ������������� ���� ������� ����� ����������� � ������ �, �� ��� ����� ��������� � ����� �
                    connSet.AddConnection(node1, negativeNode);
                }
                // ���� ���� 1 �������� ������������� �����, ��
                if (approxComparer.LT(scalarProduct1, 0))
                {
                    // ���� ����� ���� 1 � ������������� ���� ������� ����� ����������� � ������ �, �� ��� ����� ��������� � ����� �
                    connSet.AddConnection(positiveNode, node1);
                }
                // ���� ���� 2 �������� ������������� �����, ��
                if (approxComparer.GT(scalarProduct2, 0))
                {
                    // ���� ����� ���� 2 � ������������� ���� ������� ����� ����������� � ������ �, �� ��� ����� ��������� � ����� �
                    connSet.AddConnection(node2, negativeNode);
                }
                // ���� ���� 2 �������� ������������� �����, ��
                if (approxComparer.LT(scalarProduct2, 0))
                {
                    // ���� ����� ���� 2 � ������������� ���� ������� ����� ����������� � ������ �, �� ��� ����� ��������� � ����� �
                    connSet.AddConnection(positiveNode, node2);
                }*/
            }
        }

        private readonly ApproxComp approxComparer;

        // ������� (�������) C ��� ������� ������� ������
        private readonly Matrix matrixC;

        // ��� ��������� ��� t
        private readonly Double deltaT;

        // ������������ �������� ����������� (� ���� �������) �� ��������� ������� ������� ������
        private readonly Double mqMax;

        // ����������� �������� ����������� (� ���� �������) �� ��������� ������� ������� ������
        private readonly Double mqMin;
    }
}