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
    /// ����� ��� ����������� ������� fi_i (� ���������� � �������� ����� ��������� W(i+1))
    /// </summary>
    internal class BridgeGraphCorrector
    {
        /// <summary>
        /// ����������� ������ BridgeGraphCorrector
        /// </summary>
        /// <param name="approxComparer">������������ ��� ������������� ��������� �������������� �����</param>
        public BridgeGraphCorrector(ApproxComp approxComparer)
        {
            m_ApproxComparer = approxComparer;
        }

        /// <summary>
        /// ����� CheckAndCorrectBridgeGraph �������� ��������� ����������� ������� fi_i
        /// </summary>
        /// <param name="connSet">������ ������ �</param>
        /// <param name="graph">���� (�������� ������� fi)</param>
        /// <returns>�������� �������� ������� fi_i</returns>
        public Polyhedron3DGraph CheckAndCorrectBridgeGraph(SuspiciousConnectionSet connSet, Polyhedron3DGraph graph)
        {
            LESKramer3Solver solver = new LESKramer3Solver();

            // ���� ���� ����� � �� ����
            while (connSet.Count > 0)
            {
                // ����� 1-2
                Polyhedron3DGraphNode[] conn12 = connSet[0];
                Polyhedron3DGraphNode node1 = conn12[0];
                Polyhedron3DGraphNode node2 = conn12[1];

                // ���� 3; ����� 1-3 ���������� �� ��������� � ����� 1-2
                Polyhedron3DGraphNode node3 = node1.ConnectionList.GetPrevItem(node2);
                // ���� 4; ����� 1-4 ��������� �� ��������� � ����� 1-2
                Polyhedron3DGraphNode node4 = node1.ConnectionList.GetNextItem(node2);

                // ������� ������� ���. ��������� (3x3), ������������ ��� �������� ����� 1-2 �� ��������� ���������� (��. ��������)
                Matrix cone123Solution = SolveCone123EquationSystem(solver, node1, node2, node3);
                // �������� ����� 1-2 �� ��������� ����������
                Double localConvexCriterion = cone123Solution[1, 1] * node4.NodeNormal.XCoord +
                                              cone123Solution[2, 1] * node4.NodeNormal.YCoord +
                                              cone123Solution[3, 1] * node4.NodeNormal.ZCoord;
                // ���� ����� �������
                if (m_ApproxComparer.LE(localConvexCriterion, node4.SupportFuncValue))
                {
                    connSet.RemoveConnection(0);
                }
                // ���� ����� �� �������
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
#warning ����� ����� ������������������ ����������
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
                        // ������� ���� 1
                        RemoveNode(connSet, node1, graph);
                    }
                    // Lambda1<=0 && Lambda2>0
                    if (m_ApproxComparer.LE(lambda1, 0) && m_ApproxComparer.GT(lambda2, 0))
                    {
                        // ������� ���� 2
                        RemoveNode(connSet, node2, graph);
                    }
                    // Lambda1<=0 && Lambda2<=0
                    if (m_ApproxComparer.LE(lambda1, 0) && m_ApproxComparer.LE(lambda2, 0))
                    {
                        // W(i+1) = 0
#warning ����� ����� ������������������ ����������
                        throw new Exception("W(i+1)=0. Solution does not exist !!!");
                    }
                }
            }
            // ���� ���� ����� � �� ����

            return graph;
        }

        /// <summary>
        /// ����� SolveCone123EquationSystem ������ ������� ��������� ls*y = ksi(ls)
        /// ��. ������ "��������� ������� ���������������� ���� ��������� �������� �������" ���� �.�., ����� �.�.
        /// </summary>
        /// <param name="solver">������ ��� ������� ���� 3x3</param>
        /// <param name="node1">���� 1</param>
        /// <param name="node2">���� 2</param>
        /// <param name="node3">���� 3</param>
        /// <returns>������� ������� ��������� ls*y = ksi(ls)</returns>
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
        /// ����� CalcLambda123 ������ ������� ��������� l4 = lambda1*l1 + lambda2*l2 + lambda3*l3
        /// ��. ������ "��������� ������� ���������������� ���� ��������� �������� �������" ���� �.�., ����� �.�.
        /// </summary>
        /// <param name="solver">������ ��� ������� ���� 3x3</param>
        /// <param name="coneVector1">������, ��������� � ����� 1</param>
        /// <param name="coneVector2">������, ��������� � ����� 2</param>
        /// <param name="coneVector3">������, ��������� � ����� 3</param>
        /// <param name="coneVector4">������, ��������� � ����� 4</param>
        /// <returns>������� ������� ��������� l4 = lambda1*l1 + lambda2*l2 + lambda3*l3</returns>
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
        /// ����� ReplaceConn12Conn34 �������� ����� 1-2 �� ����� 3-4 (��. ������ "��������� ������� ���������������� ���� ��������� �������� �������" ���� �.�., ����� �.�.)
        /// </summary>
        /// <param name="connSet">������ ������ �</param>
        /// <param name="node1">���� 1</param>
        /// <param name="node2">���� 2</param>
        /// <param name="node3">���� 3</param>
        /// <param name="node4">���� 4</param>
        private void ReplaceConn12Conn34(SuspiciousConnectionSet connSet, Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2, Polyhedron3DGraphNode node3, Polyhedron3DGraphNode node4)
        {
            // ������� ����� �� ���� 2 �� ������ ������ ���� 1 (���������� ��� ���� 2)
            node1.ConnectionList.Remove(node2);
            node2.ConnectionList.Remove(node1);

            // � ������ ������ ���� 3, ����� ������ �� ���� 2, ��������� ������ �� ���� 4
            node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(node2) + 1, node4);
            // � ������ ������ ���� 4, ����� ������ �� ���� 1, ��������� ������ �� ���� 3
            node4.ConnectionList.Insert(node4.ConnectionList.IndexOf(node1) + 1, node3);

            // �� ������ ������ � ������� ����� 1-2
            connSet.RemoveConnection(node1, node2);
            // ��������� � ����� � �����: 1-3, 1-4, 2-3, 2-4 (�� �� ���, ������� � ������ � ���)
            connSet.AddConnection(node1, node3);
            connSet.AddConnection(node1, node4);
            connSet.AddConnection(node2, node3);
            connSet.AddConnection(node2, node4);
        }

        /// <summary>
        /// ����� RemoveNode ������� ���� removedNode �� ����� graph, ������������ (�������������) ���� graph, ������������ ������ ������ �
        /// </summary>
        /// <param name="connSet">������ ������ �</param>
        /// <param name="removedNode">��������� ����</param>
        /// <param name="graph">����</param>
        private void RemoveNode(SuspiciousConnectionSet connSet, Polyhedron3DGraphNode removedNode, Polyhedron3DGraph graph)
        {
            // ������ �����, ���������� ������ ������� K*
            CyclicList<Polyhedron3DGraphNode> sectorNodeList = new CyclicList<Polyhedron3DGraphNode>();

            // ���� (�� ���� ������ ���������� ����)
            for (Int32 connIndex = 0; connIndex < removedNode.ConnectionList.Count; ++connIndex)
            {
                // ������� ����� ���������� ����, ��������� � ��������� ����� ������� ������
                Polyhedron3DGraphNode currentConn = removedNode.ConnectionList[connIndex];
                // ������� ������� ���� � ������ �����, ���������� ������ ������� K*
                sectorNodeList.Add(currentConn);
                // �� ������ ������ �������� ���� ������� ������ �� ��������� ����
                currentConn.ConnectionList.Remove(removedNode);

            }
            // ���� (�� ���� ������ ���������� ����)

            // ���� (�� ���� ����� �� ������ ����� ���������� ������ �������)
            for (Int32 nodeIndex = 0; nodeIndex < sectorNodeList.Count; ++nodeIndex)
            {
                Polyhedron3DGraphNode currentNode = sectorNodeList[nodeIndex];
                Polyhedron3DGraphNode nextNode = sectorNodeList.GetNextItem(nodeIndex);

                // �������� �� ��, ��� currentNode - nextNode �� ����� ���� �����
#warning ����� ����� ��������� �������� ???
                Debug.Assert(currentNode.ConnectionList.IndexOf(nextNode) != -1, "nextNode must be in currentNode.ConnectionList");
                Debug.Assert(nextNode.ConnectionList.IndexOf(currentNode) != -1, "currentNode must be in nodeNode.ConnectionList");

                // ����� ������� ���� � ��������� ���� ������� � ����� �
                connSet.AddConnection(currentNode, nextNode);

            }
            // ���� (�� ���� ����� �� ������ ����� ���������� ������ �������)

            // ������� ��� �����, ���������� ��������� ����, �� ������ �
            connSet.RemoveConnections(removedNode);
            // ������� ��������� ���� �� ������ ����� �����
            graph.NodeList.Remove(removedNode);

            // ������������ ����������� �������
            if (sectorNodeList.Count > 3)
            {
                SectorTriangulation(connSet, sectorNodeList);
            }
        }

        /// <summary>
        /// ����� SectorTriangulation ������������� ������, �������� ����� �������� sectorNodeList (���� � ������� ���� �������������� ������ �.�. ...)
        /// </summary>
        /// <param name="connSet">������ ������ �</param>
        /// <param name="sectorNodeList">��������������� ������</param>
        private void SectorTriangulation(SuspiciousConnectionSet connSet, ICyclicList<Polyhedron3DGraphNode> sectorNodeList)
        {
            Debug.Assert(sectorNodeList.Count > 3, "sectorNodeList.Count must be greater than 3");

            // ����, ������������ ���� �� ��������� ���� �� ���� �����
            Boolean isNewConnBuild = false;

            // ���� (�� ���� ����� �� ������ ����� ���������� ������ �������)
            for (Int32 nodeIndex = 0; nodeIndex < sectorNodeList.Count; ++nodeIndex)
            {
                // ����� O ������� ������� ���� �� ������ �����
                Polyhedron3DGraphNode nodeO = sectorNodeList[nodeIndex];
                // ����� L ������� ���������� ���� �� ������ ����� ������������ ��������
                Polyhedron3DGraphNode nodeL = sectorNodeList.GetPrevItem(nodeIndex);
                // ����� R ������� ��������� ���� �� ������ ����� ������������ ��������
                Polyhedron3DGraphNode nodeR = sectorNodeList.GetNextItem(nodeIndex);

                // � �������� ������ ������ ������� ����� ����� ���� O � ���� R
                Polyhedron3DGraphNode startNode = nodeR;
                // ���� (�� ����� ������, ������� �� ���������� ���� ������������ ���� R � ���������� ���������� ����� ������������ ���� L)
                for (Polyhedron3DGraphNode currentNode = sectorNodeList.GetNextItem(nodeR);
                    !Object.ReferenceEquals(currentNode, nodeL);
                    currentNode = sectorNodeList.GetNextItem(currentNode))
                {
                    if (IsNewConnValid(sectorNodeList, nodeO, currentNode))
                    {
                        // ����� ��������� ����� ����� => ������������� ����
                        isNewConnBuild = true;

                        // � ������ ������ ���� O, ����� ������ � ����� L ��������� ������ �� ������� ����
                        nodeO.ConnectionList.Insert(nodeO.ConnectionList.IndexOf(nodeL), currentNode);
                        // � ������ ������ �������� ����, ����� ������ � �����, �� �������� �� ������ � �������, ��������� ������ �� ���� O
                        Polyhedron3DGraphNode prevNode = sectorNodeList.GetPrevItem(currentNode);
                        currentNode.ConnectionList.Insert(currentNode.ConnectionList.IndexOf(prevNode), nodeO);

                        // ����� ���� O � ������� ���� ��������� � ����� �
                        connSet.AddConnection(nodeO, currentNode);

                        // ���� �� ������ �����, ������� � ���� O (� ������ ������) �  ���������� ������� �����, �������� ������
                        CyclicList<Polyhedron3DGraphNode> sector = new CyclicList<Polyhedron3DGraphNode>();
                        sector.Add(nodeO);
                        for (Polyhedron3DGraphNode sectorNode = startNode;
                            !Object.ReferenceEquals(sectorNode, currentNode);
                            sectorNode = sectorNodeList.GetNextItem(sectorNode))
                        {
                            sector.Add(sectorNode);
                        }
                        sector.Add(currentNode);

                        // ���� ���������� ����� � ���� ������� ������ 3, ������������� ���� ������
                        if (sector.Count > 3)
                        {
                            SectorTriangulation(connSet, sector);
                        }

                        // � �������� ������ ������ ������� ����� ����� ���� O � ������� ����
                        startNode = currentNode;
                    }
                }
                // ���� (�� ����� ������, ������� �� ���������� ���� ������������ ���� R � ���������� ���������� ����� ������������ ���� L)

                // ���� ��� ������� ���� O ���� ��������� ���� �� ���� �����
                if (isNewConnBuild)
                {
                    // ���� �� ������ �����, ������� � ���� O (� ������ ������) �  ���������� ����� L, �������� ������
                    CyclicList<Polyhedron3DGraphNode> sector = new CyclicList<Polyhedron3DGraphNode>();
                    sector.Add(nodeO);
                    for (Polyhedron3DGraphNode sectorNode = startNode;
                        !Object.ReferenceEquals(sectorNode, nodeL);
                        sectorNode = sectorNodeList.GetNextItem(sectorNode))
                    {
                        sector.Add(sectorNode);
                    }
                    sector.Add(nodeL);

                    // ���� ���������� ����� � ���� ������� ������ 3, ������������� ���� ������
                    if (sector.Count > 3)
                    {
                        SectorTriangulation(connSet, sector);
                    }

                    // ����� ������ ���������
                    return;
                }
            }
            // ���� (�� ���� ����� �� ������ ����� ���������� ������ �������)

            // ���� ��� ������� �� ����� �� ���� ��������� �� ����� �����, �� ��� ������
#warning ����� ����� ������������������ ����������
            throw new Exception("Incorrect triangulation work !!!");
        }

        /// <summary>
        /// ����� IsNewConnValid ���������� ����� �� ���� ��������� ����� ����� ������ node1 � node2 ������ ������� sectorNodeList
        /// </summary>
        /// <param name="sectorNodeList">������ ������ �������� �� ����� ��������� ����� �����</param>
        /// <param name="node1">���� 1 ����� �����</param>
        /// <param name="node2">���� 2 ����� �����</param>
        /// <returns>true, ���� ����� ����� ������ node1 � node2 ������ ������� sectorNodeList ����� ���� ���������; ����� - false</returns>
        private Boolean IsNewConnValid(ICyclicList<Polyhedron3DGraphNode> sectorNodeList, Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2)
        {
            // ������ ������� ���������, � ������� ����� ����������� ����� ��� ��������� ������������ ������� ���� ����� �� ������
            Vector3D planeNormal = Vector3D.VectorProduct(node1.NodeNormal, node2.NodeNormal);

            // ���� (�� ����� ������, ������� �� ���������� ���� ������������ ���� 1 � ���������� ���������� ����� ������������ ���� 2)
            for (Polyhedron3DGraphNode currentNode = sectorNodeList.GetNextItem(node1);
                !Object.ReferenceEquals(currentNode, node2);
                currentNode = sectorNodeList.GetNextItem(currentNode))
            {
                // ������� ��������� ������������ �������, ���������� � ������� �����, � ����������� ������� �������
                Double scalarProduct = currentNode.NodeNormal * planeNormal;

                // ���� ��������� ������������ ������ ��� ����� 0
                if (m_ApproxComparer.GE(scalarProduct, 0))
                {
                    // ������ ���������� �������, ���������� � ������� �����, ������� ����������� ��������� ����� (������� �� lp)
                    Vector3D parallelComp = currentNode.NodeNormal.GetPerpendicularComponent(planeNormal);

                    // ���� ������ lp ����� ����� ��������� l1 � l2
                    // ����� ����� �� ����� ���� ���������
                    if (IsVectorBetweenVectors12(parallelComp, node1.NodeNormal, node2.NodeNormal)) return false;
                }

            }
            // ���� (�� ����� ������, ������� �� ���������� ���� ������������ ���� 1 � ���������� ���������� ����� ������������ ���� 2)

            // ���� (�� ����� ������, ������� �� ���������� ���� ������������ ���� 2 � ���������� ���������� ����� ������������ ���� 1)
            for (Polyhedron3DGraphNode currentNode = sectorNodeList.GetNextItem(node2);
                !Object.ReferenceEquals(currentNode, node1);
                currentNode = sectorNodeList.GetNextItem(currentNode))
            {
                // ������� ��������� ������������ �������, ���������� � ������� �����, � ����������� ������� �������
                Double scalarProduct = currentNode.NodeNormal * planeNormal;

                // ���� ��������� ������������ ������ ��� ����� 0
                if (m_ApproxComparer.LE(scalarProduct, 0))
                {
                    // ������ ���������� �������, ���������� � ������� �����, ������� ����������� ��������� ����� (������� �� lp)
                    Vector3D parallelComp = currentNode.NodeNormal.GetPerpendicularComponent(planeNormal);

                    // ���� ������ lp ����� ����� ��������� l1 � l2
                    // ����� ����� �� ����� ���� ���������
                    if (IsVectorBetweenVectors12(parallelComp, node2.NodeNormal, node1.NodeNormal)) return false;
                }
            }
            // ���� (�� ����� ������, ������� �� ���������� ���� ������������ ���� 2 � ���������� ���������� ����� ������������ ���� 1)

            // ����� ����� ����� ���� ���������
            return true;
        }

        /// <summary>
        /// ����� IsVectorBetweenVectors12 ���������� ����� �� ������ vector ����� ��������� vector1 � vector2
        /// ������� vector, vector1 � vector2 ����� � ����� ��������� (��� ����� !!!!!)
        /// </summary>
        /// <param name="vector">����������� ������</param>
        /// <param name="vector1">������ 1</param>
        /// <param name="vector2">������ 2</param>
        /// <returns>true, ���� ������ vector ����� ����� ��������� vector1 � vector2; ����� - false</returns>
        private Boolean IsVectorBetweenVectors12(Vector3D vector, Vector3D vector1, Vector3D vector2)
        {
            // ������ ��������� l = alpha*l1 + beta*l2

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

            // ���� alpha, beta >= 0 ������ ������ l ����� ����� ��������� l1 � l2
            return m_ApproxComparer.GE(alpha, 0) && m_ApproxComparer.GE(beta, 0);
        }

        /// <summary>
        /// ������������ ��� ������������� ��������� �������������� �����
        /// </summary>
        private ApproxComp m_ApproxComparer;
    }*/
}
