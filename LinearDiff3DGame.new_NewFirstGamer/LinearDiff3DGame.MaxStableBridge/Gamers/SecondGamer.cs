using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.Crossing;
using LinearDiff3DGame.MaxStableBridge.SuspiciousConnections;

namespace LinearDiff3DGame.MaxStableBridge.Gamers
{
    /// <summary>
    /// ����� �������������� � ��������������� �������� ������� ������
    /// � ���������� �������� ������� ������ �� �������� ������ ������ �, �� ������� ����� ���������� ��������� ���������� ������� fi_i
    /// </summary>
    public /*internal*/ class SecondGamer
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
        /// <summary>
        /// ����������� ������ SecondGamer
        /// </summary>
        /// <param name="approxComparer">������������, ��� ������������� ��������� �������������� �����</param>
        /// <param name="matrixC">������� (�������) C ��� ������� ������� ������</param>
        /// <param name="deltaT">��� ��������� ��� t</param>
        /// <param name="mqMax">������������ �������� ����������� (� ���� �������) �� ��������� ������� ������� ������</param>
        /// <param name="mqMin">����������� �������� ����������� (� ���� �������) �� ��������� ������� ������� ������</param>
        public SecondGamer(ApproxComp approxComparer, Matrix matrixC, Double deltaT, Double mqMax, Double mqMin)
        {
            this.approxComparer = approxComparer;
            this.matrixC = matrixC;
            this.deltaT = deltaT;
            this.mqMax = mqMax;
            this.mqMin = mqMin;
        }

        /// <summary>
        /// ����� Action - ��� �������� ������� ������ ��� �������� (������) � ������ ������ �������
        /// </summary>
        /// <param name="graph">����</param>
        /// <param name="fundCauchyMatrix">��������������� ������� ���� (������ �� �����) � ������ ������ �������</param>
        /// <param name="connSet">������ ������ �</param>
        /// <param name="scalingMatrix">������� ���������������</param>
        /// <returns>���� �������, ����� �������� ������� ������</returns>
        public Polyhedron3DGraph Action(Polyhedron3DGraph graph, Matrix fundCauchyMatrix,
                                        SuspiciousConnectionSet connSet, Matrix scalingMatrix)
        {
            // ������� (�������) E ��� ������� ������� ������ � ������ ������ �������
            Matrix matrixE = CalcMatrixE(fundCauchyMatrix, scalingMatrix);

            // ��������� ������ ������� ������ ������� Qi
            List<Vector3D> pointQiSet = new List<Vector3D>(2);
            pointQiSet.Add(new Vector3D(mqMax * matrixE[1, 1], mqMax * matrixE[2, 1], mqMax * matrixE[3, 1]));
            pointQiSet.Add(new Vector3D(mqMin * matrixE[1, 1], mqMin * matrixE[2, 1], mqMin * matrixE[3, 1]));

            // ������������ ������ ������� Qi
            Vector3D directingQi = new Vector3D(matrixE[1, 1], matrixE[2, 1], matrixE[3, 1]);
            directingQi = Vector3DUtils.NormalizeVector(directingQi);

            // ������� ������� eta_i ��� ����� �����
            for(Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
            {
#warning ����� ����� !!!!!! ��������� ������������ ��������� �������� ������� eta_i
                Polyhedron3DGraphNode currentNode = graph.NodeList[nodeIndex];
                currentNode.SupportFuncValue -= deltaT * Math.Max(currentNode.NodeNormal * pointQiSet[0],
                                                                  currentNode.NodeNormal * pointQiSet[1]);
            }
            // ������� ������� eta_i ��� ����� �����

            FillSuspiciousConnectionSet(graph, directingQi, connSet);
            return graph;
        }

        private Matrix CalcMatrixE(Matrix fundCauchyMatrix, Matrix scalingMatrix)
        {
            Matrix matrixEBeforeScaling = fundCauchyMatrix * matrixC;
            return scalingMatrix * matrixEBeforeScaling;
        }

        /// <summary>
        /// ����� FillSuspiciousConnectionSet ��������� ������ ������ � �������, �� ������� ����� ���������� ��������� ���������� ������� fi_i
        /// </summary>
        /// <param name="graph">����</param>
        /// <param name="directingVectorQi">������������ ������ ������� Qi</param>
        /// <param name="connSet">������ ������ �</param>
        private void FillSuspiciousConnectionSet(Polyhedron3DGraph graph, Vector3D directingVectorQi,
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

        /// <summary>
        /// ����� AddConns2SuspiciousConnectionSet ��������� � ����� � ������ ����������� � ����������� �������� � ��� �����
        /// </summary>
        /// <param name="currentCrossingObject">������� ������ �����������</param>
        /// <param name="connSet">������ ������ �</param>
        private void AddConns2SuspiciousConnectionSet(CrossingObject currentCrossingObject, SuspiciousConnectionSet connSet)
        {
            // ���� ������� ������ � ����
            if(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
            {
                Polyhedron3DGraphNode currentNode = currentCrossingObject.PositiveNode;

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
                Polyhedron3DGraphNode positiveNode = currentCrossingObject.PositiveNode;
                Polyhedron3DGraphNode negativeNode = currentCrossingObject.NegativeNode;

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

        /// <summary>
        /// ������������, ��� ������������� ��������� �������������� �����
        /// </summary>
        private readonly ApproxComp approxComparer;

        /// <summary>
        /// ������� (�������) C ��� ������� ������� ������
        /// </summary>
        private readonly Matrix matrixC;

        /// <summary>
        /// ��� ��������� ��� t
        /// </summary>
        private readonly Double deltaT;

        /// <summary>
        /// ������������ �������� ����������� (� ���� �������) �� ��������� ������� ������� ������
        /// </summary>
        private readonly Double mqMax;

        /// <summary>
        /// ����������� �������� ����������� (� ���� �������) �� ��������� ������� ������� ������
        /// </summary>
        private readonly Double mqMin;
    }
}