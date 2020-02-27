using System;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.Geometry3D;

namespace LinearDiff3DGame.MaxStableBridge
{
    /// <summary>
    /// ����� �������������� � ��������������� �������� ������� ������
    /// � ���������� �������� ������� ������ �� �������� ������ ������ �, �� ������� ����� ���������� ��������� ���������� ������� fi_i
    /// </summary>
    internal class SecondGamer
    {
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
            m_ApproxComparer = approxComparer;
            m_MatrixC = matrixC;
            m_DeltaT = deltaT;
            m_MqMax = mqMax;
            m_MqMin = mqMin;
        }

        /// <summary>
        /// ����� Action - ��� �������� ������� ������ ��� �������� (������) � ������ ������ �������
        /// </summary>
        /// <param name="graph">����</param>
        /// <param name="fundCauchyMatrix">��������������� ������� ���� (������ �� �����) � ������ ������ �������</param>
        /// <param name="connSet">������ ������ �</param>
        /// <returns>���� �������, ����� �������� ������� ������</returns>
        public Polyhedron3DGraph Action(Polyhedron3DGraph graph, Matrix fundCauchyMatrix, SuspiciousConnectionSet connSet)
        {
            // ������� (�������) E ��� ������� ������� ������ � ������ ������ �������
            Matrix matrixE = fundCauchyMatrix * m_MatrixC;

            // ��������� ������ ������� ������ ������� Qi
            List<Vector3D> pointQiSet = new List<Vector3D>(2);
            pointQiSet.Add(new Vector3D(m_MqMax * matrixE[1, 1], m_MqMax * matrixE[2, 1], m_MqMax * matrixE[3, 1]));
            pointQiSet.Add(new Vector3D(m_MqMin * matrixE[1, 1], m_MqMin * matrixE[2, 1], m_MqMin * matrixE[3, 1]));

            // ������������ ������ ������� Qi
            Vector3D directingVectorQi = new Vector3D(matrixE[1, 1], matrixE[2, 1], matrixE[3, 1]);
            directingVectorQi.Normalize();

            // ������� ������� eta_i ��� ����� �����
            for (Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
            {
#warning ����� ����� !!!!!! ��������� ������������ ��������� �������� ������� eta_i
                Polyhedron3DGraphNode currentNode = graph.NodeList[nodeIndex];
                currentNode.SupportFuncValue -= m_DeltaT * Math.Max(currentNode.NodeNormal * pointQiSet[0],
                                                                    currentNode.NodeNormal * pointQiSet[1]);
            }
            // ������� ������� eta_i ��� ����� �����

            FillSuspiciousConnectionSet(graph, directingVectorQi, connSet);
            return graph;
        }

        /// <summary>
        /// ����� FillSuspiciousConnectionSet ��������� ������ ������ � �������, �� ������� ����� ���������� ��������� ���������� ������� fi_i
        /// </summary>
        /// <param name="graph">����</param>
        /// <param name="directingVectorQi">������������ ������ ������� Qi</param>
        /// <param name="connSet">������ ������ �</param>
        private void FillSuspiciousConnectionSet(Polyhedron3DGraph graph, Vector3D directingVectorQi, SuspiciousConnectionSet connSet)
        {
            // ������ ��� ������ ����������� ����� � Zi
            CrossingObjectFinder finder = new CrossingObjectFinder(m_ApproxComparer);

            // ������ (�����������) ������ �����������
            CrossingObject firstCrossingObject = finder.GetFirstCrossingObject(graph.NodeList[0], directingVectorQi);
            // ������� ������ �����������
            CrossingObject currentCrossingObject = firstCrossingObject;
            // ��������� � ����� � ��� ������ ����������� � ����������� �������� � ��� �����
            AddConns2SuspiciousConnectionSet(currentCrossingObject, directingVectorQi, connSet);

            // ���� (���� ������� ������ �� ������ ������ ������������)
            do
            {
                // �������� ��������� �� �������� ������ (�����, ���� ����) � ������ ��� �������
                currentCrossingObject = finder.GetNextCrossingObject(currentCrossingObject, directingVectorQi);
                // ��������� � ����� � ��� ������ ����������� � ����������� �������� � ��� �����
                AddConns2SuspiciousConnectionSet(currentCrossingObject, directingVectorQi, connSet);
            }
            while (currentCrossingObject != firstCrossingObject);
            // ���� (���� ������� ������ �� ������ ������ ������������)
        }

        /// <summary>
        /// ����� AddConns2SuspiciousConnectionSet ��������� � ����� � ������ ����������� � ����������� �������� � ��� �����
        /// </summary>
        /// <param name="currentCrossingObject">������� ������ �����������</param>
        /// <param name="directingVectorQi">������������ ������ ������� Qi</param>
        /// <param name="connSet">������ ������ �</param>
        private void AddConns2SuspiciousConnectionSet(CrossingObject currentCrossingObject, Vector3D directingVectorQi, SuspiciousConnectionSet connSet)
        {
            // ���� ������� ������ � ����
            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
            {
                Polyhedron3DGraphNode currentNode = currentCrossingObject.PositiveNode;

                // ���� �� ���� ������ �������� ���� (�������� ������� �����������)
                for (Int32 connIndex = 0; connIndex < currentNode.ConnectionList.Count; ++connIndex)
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

                // ��� �������������� ���� �����, ����� ������ �� ���������� ���� ������������ 	������ �����; ������� ���� ���� � ����� 1
                Polyhedron3DGraphNode node1 = positiveNode.ConnectionList.GetPrevItem(negativeNode);
                Double scalarProduct1 = node1.NodeNormal * directingVectorQi;
                // ��� �������������� ���� �����, ����� ������ �� ��������� ���� ������������ 	������ �����; ������� ���� ���� � ����� 2
                Polyhedron3DGraphNode node2 = positiveNode.ConnectionList.GetNextItem(negativeNode);
                Double scalarProduct2 = node2.NodeNormal * directingVectorQi;

                // ���� ���� 1 �������� ������������� �����, ��
                if (m_ApproxComparer.GT(scalarProduct1, 0))
                {
                    // ���� ����� ���� 1 � ������������� ���� ������� ����� ����������� � ������ �, �� ��� ����� ��������� � ����� �
                    connSet.AddConnection(node1, negativeNode);
                }
                // ���� ���� 1 �������� ������������� �����, ��
                if (m_ApproxComparer.LT(scalarProduct1, 0))
                {
                    // ���� ����� ���� 1 � ������������� ���� ������� ����� ����������� � ������ �, �� ��� ����� ��������� � ����� �
                    connSet.AddConnection(positiveNode, node1);
                }
                // ���� ���� 2 �������� ������������� �����, ��
                if (m_ApproxComparer.GT(scalarProduct2, 0))
                {
                    // ���� ����� ���� 2 � ������������� ���� ������� ����� ����������� � ������ �, �� ��� ����� ��������� � ����� �
                    connSet.AddConnection(node2, negativeNode);
                }
                // ���� ���� 2 �������� ������������� �����, ��
                if (m_ApproxComparer.LT(scalarProduct2, 0))
                {
                    // ���� ����� ���� 2 � ������������� ���� ������� ����� ����������� � ������ �, �� ��� ����� ��������� � ����� �
                    connSet.AddConnection(positiveNode, node2);
                }
            }
        }

        /// <summary>
        /// ������������, ��� ������������� ��������� �������������� �����
        /// </summary>
        private ApproxComp m_ApproxComparer;

        /// <summary>
        /// ������� (�������) C ��� ������� ������� ������
        /// </summary>
        private Matrix m_MatrixC;

        /// <summary>
        /// ��� ��������� ��� t
        /// </summary>
        private Double m_DeltaT;
        /// <summary>
        /// ������������ �������� ����������� (� ���� �������) �� ��������� ������� ������� ������
        /// </summary>
        private Double m_MqMax;
        /// <summary>
        /// ����������� �������� ����������� (� ���� �������) �� ��������� ������� ������� ������
        /// </summary>
        private Double m_MqMin;
    }
}
