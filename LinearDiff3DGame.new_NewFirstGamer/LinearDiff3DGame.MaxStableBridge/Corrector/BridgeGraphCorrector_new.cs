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
        /// ����������� ������ BridgeGraphCorrector
        /// </summary>
        /// <param name="approxComparer">������������ ��� ������������� ��������� �������������� �����</param>
        public BridgeGraphCorrector(ApproxComp approxComparer)
        {
            m_ApproxComparer = approxComparer;
            m_Solver = new LESKramer3Solver();
        }

        /// <summary>
        /// ����� CheckAndCorrectBridgeGraph �������� ��������� ����������� ������� fi_i
        /// </summary>
        /// <param name="connSet">������ ������ �</param>
        /// <param name="graph">���� (�������� ������� fi)</param>
        /// <returns>�������� �������� ������� fi_i</returns>
        public Polyhedron3DGraph CheckAndCorrectBridgeGraph(SuspiciousConnectionSet connSet, Polyhedron3DGraph graph)
        {
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

                /*// ������� ������� ���. ��������� (3x3), ������������ ��� �������� ����� 1-2 �� ��������� ���������� (��. ��������)
                Matrix cone123Solution = SolveCone123EquationSystem(solver, node1, node2, node3);
                // �������� ����� 1-2 �� ��������� ����������
                Double localConvexCriterion = cone123Solution[1, 1] * node4.NodeNormal.XCoord +
                                              cone123Solution[2, 1] * node4.NodeNormal.YCoord +
                                              cone123Solution[3, 1] * node4.NodeNormal.ZCoord;
                // ���� ����� �������
                if (m_ApproxComparer.LE(localConvexCriterion, node4.SupportFuncValue))
                {
                    connSet.RemoveConnection(0);
                }*/
                // ���� ����� �������
                if (CheckConnConvexity(node1, node2, node3, node4))
                {
                    connSet.RemoveConnection(0);
                }
                    // ���� ����� �� �������
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
        /// �������� ����� 1-2 (� ��������� 3, 4) �� ����������
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <param name="node3"></param>
        /// <param name="node4"></param>
        /// <returns></returns>
        private Boolean CheckConnConvexity(Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2,
                                           Polyhedron3DGraphNode node3, Polyhedron3DGraphNode node4)
        {
            // ������� ������� ���. ��������� (3x3), ������������ ��� �������� ����� 1-2 �� ��������� ���������� (��. ��������)
            Matrix cone123Solution = SolveCone123EquationSystem(node1, node2, node3);
            // �������� ����� 1-2 �� ��������� ����������
            Double localConvexCriterion = cone123Solution[1, 1]*node4.NodeNormal.XCoord +
                                          cone123Solution[2, 1]*node4.NodeNormal.YCoord +
                                          cone123Solution[3, 1]*node4.NodeNormal.ZCoord;

            // if (localConvexCriterion <= node4.SupportFuncValue) �� ����� �������
            return m_ApproxComparer.LE(localConvexCriterion, node4.SupportFuncValue);
        }

        /// <summary>
        /// ����� SolveCone123EquationSystem ������ ������� ��������� ls*y = ksi(ls)
        /// ��. ������ "��������� ������� ���������������� ���� ��������� �������� �������" ���� �.�., ����� �.�.
        /// </summary>
        /// <param name="node1">���� 1</param>
        /// <param name="node2">���� 2</param>
        /// <param name="node3">���� 3</param>
        /// <returns>������� ������� ��������� ls*y = ksi(ls)</returns>
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
        /// ����� CalcLambda123 ������ ������� ��������� l4 = lambda1*l1 + lambda2*l2 + lambda3*l3
        /// ��. ������ "��������� ������� ���������������� ���� ��������� �������� �������" ���� �.�., ����� �.�.
        /// </summary>
        /// <param name="coneVector1">������, ��������� � ����� 1</param>
        /// <param name="coneVector2">������, ��������� � ����� 2</param>
        /// <param name="coneVector3">������, ��������� � ����� 3</param>
        /// <param name="coneVector4">������, ��������� � ����� 4</param>
        /// <returns>������� ������� ��������� l4 = lambda1*l1 + lambda2*l2 + lambda3*l3</returns>
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
        /// ����� ReplaceConn12Conn34 �������� ����� 1-2 �� ����� 3-4 (��. ������ "��������� ������� ���������������� ���� ��������� �������� �������" ���� �.�., ����� �.�.)
        /// </summary>
        /// <param name="connSet">������ ������ �</param>
        /// <param name="node1">���� 1</param>
        /// <param name="node2">���� 2</param>
        /// <param name="node3">���� 3</param>
        /// <param name="node4">���� 4</param>
        private void ReplaceConn12Conn34(SuspiciousConnectionSet connSet, Polyhedron3DGraphNode node1,
                                         Polyhedron3DGraphNode node2, Polyhedron3DGraphNode node3,
                                         Polyhedron3DGraphNode node4)
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
        private void RemoveNode(SuspiciousConnectionSet connSet, Polyhedron3DGraphNode removedNode,
                                Polyhedron3DGraph graph)
        {
            // ������ �����, ���������� ������ ������� K*
            List<Polyhedron3DGraphNode> sectorNodeList = new List<Polyhedron3DGraphNode>();

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
                Debug.Assert(currentNode.ConnectionList.IndexOf(nextNode) != -1,
                             "nextNode must be in currentNode.ConnectionList");
                Debug.Assert(nextNode.ConnectionList.IndexOf(currentNode) != -1,
                             "currentNode must be in nodeNode.ConnectionList");

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
        private void SectorTriangulation(SuspiciousConnectionSet connSet,
                                         IList<Polyhedron3DGraphNode> sectorNodeList)
        {
            // ���� ���������� ����� � ������� <= 3, �� ����� ������ ���������
            if (sectorNodeList.Count <= 3) return;

            // ���� (�� ���� ����� �� ������ ����� ���������� ������ �������)
            for (Int32 sectorNodeIndex = 0; sectorNodeIndex < sectorNodeList.Count; ++sectorNodeIndex)
            {
                // ������� ���� �������
                Polyhedron3DGraphNode currentNode = sectorNodeList[sectorNodeIndex];
                // ���� ����� ������� ����� �������
                Polyhedron3DGraphNode beforeCurrentNode = sectorNodeList.GetPrevItem(currentNode);

                // ��� �������� ���� ���������� ������ ������ � �������� (�� �������� ����) ������
                List<Polyhedron3DGraphNode> dirtyVisibleNodes = GetVisibleNodes(sectorNodeList, currentNode);
                /*// ������� �� ������ ��� ����� � ��������� ������ (������� ???)
                visibleNodes.RemoveAt(visibleNodes.Count - 1);
                visibleNodes.RemoveAt(0);*/
                // ���� ������ ������ == 2, ������� � ���������� ����
                if (dirtyVisibleNodes.Count == 2) continue;

                List<Polyhedron3DGraphNode> visibleNodes = new List<Polyhedron3DGraphNode>();
                visibleNodes.Add(dirtyVisibleNodes[0]);
                // ���� (�� ������ ������� ������)
                for (Int32 dirtyVisibleNodeIndex = 0;
                     dirtyVisibleNodeIndex < dirtyVisibleNodes.Count;
                     ++dirtyVisibleNodeIndex)
                {
                    // ���� ����� ������������� ������� (*), �� �� ��������� � ������ �������, ����������� (*) ������
                    if (CheckConnInSector(dirtyVisibleNodes, currentNode, dirtyVisibleNodes[dirtyVisibleNodeIndex]))
                    {
                        visibleNodes.Add(dirtyVisibleNodes[dirtyVisibleNodeIndex]);
                    }
                }
                // ���� (�� ������ ������� ������)
                visibleNodes.Add(dirtyVisibleNodes[dirtyVisibleNodes.Count - 1]);
                // ���� ������ ������ == 2, ������� � ���������� ����
                if (visibleNodes.Count == 2) continue;

                // ���� (�� ������ �������, ?��������? ������)
                for (Int32 visibleNodeIndex = 0; visibleNodeIndex < visibleNodes.Count; ++visibleNodeIndex)
                {
                    // ������� ������� ���� (������� ������� �����)
                    Polyhedron3DGraphNode currentVisibleNode = visibleNodes[visibleNodeIndex];

                    // ������ (�������) ������� ����� �� ������ ������� ������
                    // ��������� ��������� �� ???
                    currentNode.ConnectionList.Insert(currentNode.ConnectionList.IndexOf(beforeCurrentNode),
                                                      currentVisibleNode);
                    currentVisibleNode.ConnectionList.Insert(
                        currentVisibleNode.ConnectionList.IndexOf(sectorNodeList.GetPrevItem(currentVisibleNode)),
                        currentNode);
                    // ��������� ��������� �� ???

                    // ��������� ����������� ����� � ����� � (����� �� ???)                    
                    connSet.AddConnection(currentNode, currentVisibleNode);

                    // �������� �� ���������������� ������� ��� ������ ����������� ����� ������
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

                    // ���� ���������� ����� ����������� ������� > 3, �� ������������� ���
                    if (subSector.Count > 3)
                    {
                        SectorTriangulation(connSet, subSector);
                    }
                }
                // ���� (�� ������ �������, ?��������? ������)

                // ���� ����� ����, �� ����� ������ ���������
                return;
            }
            // ���� (�� ���� ����� �� ������ ����� ���������� ������ �������)

            // ���� ����� ����, �� ������������ ������� �������� �� ������
#warning ����� ����� ������������������ ����������
            throw new Exception("Sector triangulation failed !!!");
        }

        /// <summary>
        /// ������ ������� ����� ������� sectorNodeList �� ���� initialNode
        /// </summary>
        /// <param name="sectorNodeList"></param>
        /// <param name="initialNode"></param>
        /// <returns></returns>
        private List<Polyhedron3DGraphNode> GetVisibleNodes(IList<Polyhedron3DGraphNode> sectorNodeList,
                                                            Polyhedron3DGraphNode initialNode)
        {
            List<Polyhedron3DGraphNode> visibleNodes = new List<Polyhedron3DGraphNode>();

            // ��� ������������
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
        /// �������� ����, ��� � ������� sectorNodeList ���� checkedNode ����� �� ���� initialNode
        /// </summary>
        /// <param name="sectorNodeList"></param>
        /// <param name="initialNode"></param>
        /// <param name="checkedNode"></param>
        /// <returns></returns>
        private Boolean CheckNodeVisibility(IList<Polyhedron3DGraphNode> sectorNodeList,
                                            Polyhedron3DGraphNode initialNode, Polyhedron3DGraphNode checkedNode)
        {
            // ��� ������������
            Polyhedron3DGraphNode node1 = initialNode;
            Polyhedron3DGraphNode nodek = checkedNode;

            // ����������� ����� 1
            Polyhedron3DGraphNode node2 = sectorNodeList.GetNextItem(node1);
            Polyhedron3DGraphNode nodem = sectorNodeList.GetPrevItem(node1);

            Double mixedProdact12k = Vector3D.MixedProduct(nodek.NodeNormal, node1.NodeNormal, node2.NodeNormal);
            Double mixedProdact1mk = Vector3D.MixedProduct(nodek.NodeNormal, node1.NodeNormal, nodem.NodeNormal);

            // must be mixedProdact12k < 0, mixedProdact1mk > 0
            if (!m_ApproxComparer.LT(mixedProdact12k, 0) || !m_ApproxComparer.GT(mixedProdact1mk, 0)) return false;

            // ����������� ����� 2
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

        [Obsolete("������ �������� � �� ��� ������ ���� ����")]
        private Double GetMaxValue(Double value1, Double value2, Double value3, Double value4)
        {
            Double maxValue = value1;
            if (maxValue < value2) maxValue = value2;
            if (maxValue < value3) maxValue = value3;
            if (maxValue < value4) maxValue = value4;
            return maxValue;
        }

        [Obsolete("������ �������� � �� ��� ������ ���� ����")]
        private Double GetMinValue(Double value1, Double value2, Double value3, Double value4)
        {
            Double minValue = value1;
            if (minValue > value2) minValue = value2;
            if (minValue > value3) minValue = value3;
            if (minValue > value4) minValue = value4;
            return minValue;
        }

        /// <summary>
        /// �������� ����� 1k � ������� ... ������ ��. �������� ...
        /// </summary>
        /// <param name="visibleNodes"></param>
        /// <param name="initialNode"></param>
        /// <param name="checkedNode"></param>
        /// <returns></returns>
        private Boolean CheckConnInSector(List<Polyhedron3DGraphNode> visibleNodes, Polyhedron3DGraphNode initialNode,
                                          Polyhedron3DGraphNode checkedNode)
        {
            Int32 checkedNodeIndex = visibleNodes.IndexOf(checkedNode);

            // ��� ������������
            Polyhedron3DGraphNode node1 = initialNode;
            Polyhedron3DGraphNode nodek = checkedNode;

            // visibleNodes[0] == l2, visibleNodes[visibleNodes.Count-1] == lm,
            for (Int32 nodei1Index = 0; nodei1Index < checkedNodeIndex; ++nodei1Index)
            {
                Polyhedron3DGraphNode nodei1 = visibleNodes[nodei1Index];

                for (Int32 nodei2Index = checkedNodeIndex + 1; nodei2Index < visibleNodes.Count; ++nodei2Index)
                {
                    Polyhedron3DGraphNode nodei2 = visibleNodes[nodei2Index];

                    // ���� ����� 1k �� ������� � ��������� i1, i2, �� ����� 1k �������� �� ������
                    if (!CheckConnConvexity(node1, nodek, nodei1, nodei2)) return false;
                }
            }

            return true;
        }

        /*/// <summary>
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
        }*/

        /*/// <summary>
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
        }*/

        /// <summary>
        /// ������������ ��� ������������� ��������� �������������� �����
        /// </summary>
        private readonly ApproxComp m_ApproxComparer;

        /// <summary>
        /// ������ ��� ������� ���� 3x3
        /// </summary>
        private readonly ILinearEquationsSystemSolver m_Solver;
    }
}