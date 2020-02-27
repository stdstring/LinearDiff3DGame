using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.LinearEquationsSet;
using LinearDiff3DGame.AdvMath.Matrix;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.SuspiciousConnections;

namespace LinearDiff3DGame.MaxStableBridge.Corrector
{
    internal class BridgeGraphCorrector
    {
        public BridgeGraphCorrector(ApproxComp approxComparer)
        {
            this.approxComparer = approxComparer;
            lesSolver = new LESKramer3Solver();
        }

        // ����� CheckAndCorrectBridgeGraph �������� ��������� ����������� ������� fi_i
        public IPolyhedron3DGraph CheckAndCorrectBridgeGraph(SuspiciousConnectionSet connSet, IPolyhedron3DGraph graph)
        {
            // ���� ���� ����� � �� ����
            while(connSet.Count > 0)
            {
                // ����� 1-2
                IPolyhedron3DGraphNode[] conn12 = connSet[0];
                IPolyhedron3DGraphNode node1 = conn12[0];
                IPolyhedron3DGraphNode node2 = conn12[1];

                // ���� 3; ����� 1-3 ���������� �� ��������� � ����� 1-2
                IPolyhedron3DGraphNode node3 = node1.ConnectionList.GetPrevItem(node2);
                // ���� 4; ����� 1-4 ��������� �� ��������� � ����� 1-2
                IPolyhedron3DGraphNode node4 = node1.ConnectionList.GetNextItem(node2);

                /*// ������� ������� ���. ��������� (3x3), ������������ ��� �������� ����� 1-2 �� ��������� ���������� (��. ��������)
                Matrix cone123Solution = SolveCone123EquationSystem(solver, node1, node2, node3);
                // �������� ����� 1-2 �� ��������� ����������
                Double localConvexCriterion = cone123Solution[1, 1] * node4.NodeNormal.X +
                                              cone123Solution[2, 1] * node4.NodeNormal.Y +
                                              cone123Solution[3, 1] * node4.NodeNormal.Z;
                // ���� ����� �������
                if (approxComparer.LE(localConvexCriterion, node4.SupportFuncValue))
                {
                    connSet.RemoveConnection(0);
                }*/
                // ���� ����� �������
                if(CheckConnConvexity(node1, node2, node3, node4))
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
                    if(approxComparer.GE(lambda3, 0))
                    {
#warning ����� ����� ������������������ ����������
                        //throw new Exception("Lambda3 must be < 0");
                    }

                    // Lambda1>0 && Lambda2>0
                    if(approxComparer.GT(lambda1, 0) && approxComparer.GT(lambda2, 0))
                    {
                        ReplaceConn12Conn34(connSet, node1, node2, node3, node4);
                    }
                    // Lambda1>0 && Lambda2<=0
                    if(approxComparer.GT(lambda1, 0) && approxComparer.LE(lambda2, 0))
                    {
                        // ������� ���� 1
                        RemoveNode(connSet, node1, graph);
                    }
                    // Lambda1<=0 && Lambda2>0
                    if(approxComparer.LE(lambda1, 0) && approxComparer.GT(lambda2, 0))
                    {
                        // ������� ���� 2
                        RemoveNode(connSet, node2, graph);
                    }
                    // Lambda1<=0 && Lambda2<=0
                    if(approxComparer.LE(lambda1, 0) && approxComparer.LE(lambda2, 0))
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

        // �������� ����� 1-2 (� ��������� 3, 4) �� ����������
        private Boolean CheckConnConvexity(IPolyhedron3DGraphNode node1,
                                           IPolyhedron3DGraphNode node2,
                                           IPolyhedron3DGraphNode node3,
                                           IPolyhedron3DGraphNode node4)
        {
            // ������� ������� ���. ��������� (3x3), ������������ ��� �������� ����� 1-2 �� ��������� ���������� (��. ��������)
            Matrix cone123Solution = SolveCone123EquationSystem(node1, node2, node3);
            // �������� ����� 1-2 �� ��������� ����������
            Double localConvexCriterion = cone123Solution[1, 1]*node4.NodeNormal.X +
                                          cone123Solution[2, 1]*node4.NodeNormal.Y +
                                          cone123Solution[3, 1]*node4.NodeNormal.Z;

            // if (localConvexCriterion <= node4.SupportFuncValue) �� ����� �������
            return approxComparer.LE(localConvexCriterion, node4.SupportFuncValue);
        }

        // ����� SolveCone123EquationSystem ������ ������� ��������� ls*y = ksi(ls)
        // ��. ������ "��������� ������� ���������������� ���� ��������� �������� �������" ���� �.�., ����� �.�.
        private Matrix SolveCone123EquationSystem(IPolyhedron3DGraphNode node1,
                                                  IPolyhedron3DGraphNode node2,
                                                  IPolyhedron3DGraphNode node3)
        {
            Matrix matrixA = new Matrix(3, 3);
            matrixA[1, 1] = node1.NodeNormal.X;
            matrixA[1, 2] = node1.NodeNormal.Y;
            matrixA[1, 3] = node1.NodeNormal.Z;
            matrixA[2, 1] = node2.NodeNormal.X;
            matrixA[2, 2] = node2.NodeNormal.Y;
            matrixA[2, 3] = node2.NodeNormal.Z;
            matrixA[3, 1] = node3.NodeNormal.X;
            matrixA[3, 2] = node3.NodeNormal.Y;
            matrixA[3, 3] = node3.NodeNormal.Z;

            Matrix matrixB = new Matrix(3, 1);
            matrixB[1, 1] = node1.SupportFuncValue;
            matrixB[2, 1] = node2.SupportFuncValue;
            matrixB[3, 1] = node3.SupportFuncValue;

            Matrix matrixError;

            Matrix solution = lesSolver.Solve(matrixA, matrixB, out matrixError);
            return solution;
        }

        // ����� CalcLambda123 ������ ������� ��������� l4 = lambda1*l1 + lambda2*l2 + lambda3*l3
        // ��. ������ "��������� ������� ���������������� ���� ��������� �������� �������" ���� �.�., ����� �.�.
        private Matrix CalcLambda123(Vector3D coneVector1,
                                     Vector3D coneVector2,
                                     Vector3D coneVector3,
                                     Vector3D coneVector4)
        {
            Matrix matrixA = new Matrix(3, 3);
            matrixA[1, 1] = coneVector1.X;
            matrixA[1, 2] = coneVector2.X;
            matrixA[1, 3] = coneVector3.X;
            matrixA[2, 1] = coneVector1.Y;
            matrixA[2, 2] = coneVector2.Y;
            matrixA[2, 3] = coneVector3.Y;
            matrixA[3, 1] = coneVector1.Z;
            matrixA[3, 2] = coneVector2.Z;
            matrixA[3, 3] = coneVector3.Z;

            Matrix matrixB = new Matrix(3, 1);
            matrixB[1, 1] = coneVector4.X;
            matrixB[2, 1] = coneVector4.Y;
            matrixB[3, 1] = coneVector4.Z;

            Matrix matrixError;

            Matrix solution = lesSolver.Solve(matrixA, matrixB, out matrixError);
            return solution;
        }

        private Matrix CalcLambda123(Vector3D coneVector1,
                                     Vector3D coneVector2,
                                     Vector3D coneVector3,
                                     Vector3D coneVector4,
                                     out Matrix matrixError)
        {
            Matrix matrixA = new Matrix(3, 3);
            matrixA[1, 1] = coneVector1.X;
            matrixA[1, 2] = coneVector2.X;
            matrixA[1, 3] = coneVector3.X;
            matrixA[2, 1] = coneVector1.Y;
            matrixA[2, 2] = coneVector2.Y;
            matrixA[2, 3] = coneVector3.Y;
            matrixA[3, 1] = coneVector1.Z;
            matrixA[3, 2] = coneVector2.Z;
            matrixA[3, 3] = coneVector3.Z;

            Matrix matrixB = new Matrix(3, 1);
            matrixB[1, 1] = coneVector4.X;
            matrixB[2, 1] = coneVector4.Y;
            matrixB[3, 1] = coneVector4.Z;

            Matrix solution = lesSolver.Solve(matrixA, matrixB, out matrixError);
            return solution;
        }

        // ����� ReplaceConn12Conn34 �������� ����� 1-2 �� ����� 3-4 (��. ������ "��������� ������� ���������������� ���� ��������� �������� �������" ���� �.�., ����� �.�.)
        private static void ReplaceConn12Conn34(SuspiciousConnectionSet connSet,
                                                IPolyhedron3DGraphNode node1,
                                                IPolyhedron3DGraphNode node2,
                                                IPolyhedron3DGraphNode node3,
                                                IPolyhedron3DGraphNode node4)
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

        // ����� RemoveNode ������� ���� removedNode �� ����� graph, ������������ (�������������) ���� graph, ������������ ������ ������ �
        private void RemoveNode(SuspiciousConnectionSet connSet,
                                IPolyhedron3DGraphNode removedNode,
                                IPolyhedron3DGraph graph)
        {
            // ������ �����, ���������� ������ ������� K*
            List<IPolyhedron3DGraphNode> sectorNodeList = new List<IPolyhedron3DGraphNode>();

            // ���� (�� ���� ������ ���������� ����)
            for(Int32 connIndex = 0; connIndex < removedNode.ConnectionList.Count; ++connIndex)
            {
                // ������� ����� ���������� ����, ��������� � ��������� ����� ������� ������
                IPolyhedron3DGraphNode currentConn = removedNode.ConnectionList[connIndex];
                // ������� ������� ���� � ������ �����, ���������� ������ ������� K*
                sectorNodeList.Add(currentConn);
                // �� ������ ������ �������� ���� ������� ������ �� ��������� ����
                currentConn.ConnectionList.Remove(removedNode);
            }
            // ���� (�� ���� ������ ���������� ����)

            // ���� (�� ���� ����� �� ������ ����� ���������� ������ �������)
            for(Int32 nodeIndex = 0; nodeIndex < sectorNodeList.Count; ++nodeIndex)
            {
                IPolyhedron3DGraphNode currentNode = sectorNodeList[nodeIndex];
                IPolyhedron3DGraphNode nextNode = sectorNodeList.GetNextItem(nodeIndex);
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
            if(sectorNodeList.Count > 3)
            {
                SectorTriangulation(connSet, sectorNodeList);
            }
        }

        // ����� SectorTriangulation ������������� ������, �������� ����� �������� sectorNodeList (���� � ������� ���� �������������� ������ �.�. ...)
        private void SectorTriangulation(SuspiciousConnectionSet connSet,
                                         IList<IPolyhedron3DGraphNode> sectorNodeList)
        {
            // ���� ���������� ����� � ������� <= 3, �� ����� ������ ���������
            if(sectorNodeList.Count <= 3) return;

            // ���� (�� ���� ����� �� ������ ����� ���������� ������ �������)
            for(Int32 sectorNodeIndex = 0; sectorNodeIndex < sectorNodeList.Count; ++sectorNodeIndex)
            {
                // ������� ���� �������
                IPolyhedron3DGraphNode currentNode = sectorNodeList[sectorNodeIndex];
                // ���� ����� ������� ����� �������
                IPolyhedron3DGraphNode beforeCurrentNode = sectorNodeList.GetPrevItem(currentNode);
                // ��� �������� ���� ���������� ������ ������ � �������� (�� �������� ����) ������
                List<IPolyhedron3DGraphNode> dirtyVisibleNodes = GetVisibleNodes(sectorNodeList, currentNode);
                /*// ������� �� ������ ��� ����� � ��������� ������ (������� ???)
                visibleNodes.RemoveAt(visibleNodes.Count - 1);
                visibleNodes.RemoveAt(0);*/
                // ���� ������ ������ == 2, ������� � ���������� ����
                if(dirtyVisibleNodes.Count == 2) continue;
                List<IPolyhedron3DGraphNode> visibleNodes = new List<IPolyhedron3DGraphNode>();
                visibleNodes.Add(dirtyVisibleNodes[0]);
                // ���� (�� ������ ������� ������)
                for(Int32 dirtyVisibleNodeIndex = 0;
                    dirtyVisibleNodeIndex < dirtyVisibleNodes.Count;
                    ++dirtyVisibleNodeIndex)
                {
                    // ���� ����� ������������� ������� (*), �� �� ��������� � ������ �������, ����������� (*) ������
                    if(CheckConnInSector(dirtyVisibleNodes, currentNode, dirtyVisibleNodes[dirtyVisibleNodeIndex]))
                    {
                        visibleNodes.Add(dirtyVisibleNodes[dirtyVisibleNodeIndex]);
                    }
                }
                // ���� (�� ������ ������� ������)
                visibleNodes.Add(dirtyVisibleNodes[dirtyVisibleNodes.Count - 1]);
                // ���� ������ ������ == 2, ������� � ���������� ����
                if(visibleNodes.Count == 2) continue;

                // ���� (�� ������ �������, ?��������? ������)
                for(Int32 visibleNodeIndex = 0; visibleNodeIndex < visibleNodes.Count; ++visibleNodeIndex)
                {
                    // ������� ������� ���� (������� ������� �����)
                    IPolyhedron3DGraphNode currentVisibleNode = visibleNodes[visibleNodeIndex];

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
                    List<IPolyhedron3DGraphNode> subSector = new List<IPolyhedron3DGraphNode>();
                    subSector.Add(currentNode);
                    for(IPolyhedron3DGraphNode afterCurrentNode = sectorNodeList.GetNextItem(currentNode);
                        afterCurrentNode != currentVisibleNode;
                        afterCurrentNode = sectorNodeList.GetNextItem(currentNode))
                    {
                        subSector.Add(afterCurrentNode);
                        sectorNodeList.Remove(afterCurrentNode);
                    }
                    subSector.Add(currentVisibleNode);

                    // ���� ���������� ����� ����������� ������� > 3, �� ������������� ���
                    if(subSector.Count > 3)
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

        // ������ ������� ����� ������� sectorNodeList �� ���� initialNode
        private List<IPolyhedron3DGraphNode> GetVisibleNodes(IList<IPolyhedron3DGraphNode> sectorNodeList,
                                                             IPolyhedron3DGraphNode initialNode)
        {
            List<IPolyhedron3DGraphNode> visibleNodes = new List<IPolyhedron3DGraphNode>();
            // ��� ������������
            IPolyhedron3DGraphNode node1 = initialNode;
            IPolyhedron3DGraphNode node2 = sectorNodeList.GetNextItem(node1);
            IPolyhedron3DGraphNode nodem = sectorNodeList.GetPrevItem(node1);
            visibleNodes.Add(node2);
            for(IPolyhedron3DGraphNode nodek = sectorNodeList.GetNextItem(node2);
                nodek != nodem;
                nodek = sectorNodeList.GetNextItem(nodek))
            {
                if(CheckNodeVisibility(sectorNodeList, initialNode, nodek)) visibleNodes.Add(nodek);
            }
            visibleNodes.Add(nodem);
            return visibleNodes;
        }

        // �������� ����, ��� � ������� sectorNodeList ���� checkedNode ����� �� ���� initialNode
        private Boolean CheckNodeVisibility(IList<IPolyhedron3DGraphNode> sectorNodeList,
                                            IPolyhedron3DGraphNode initialNode,
                                            IPolyhedron3DGraphNode checkedNode)
        {
            // ��� ������������
            IPolyhedron3DGraphNode node1 = initialNode;
            IPolyhedron3DGraphNode nodek = checkedNode;
            // ����������� ����� 1
            IPolyhedron3DGraphNode node2 = sectorNodeList.GetNextItem(node1);
            IPolyhedron3DGraphNode nodem = sectorNodeList.GetPrevItem(node1);
            Double mixedProdact12k = Vector3DUtils.MixedProduct(nodek.NodeNormal, node1.NodeNormal, node2.NodeNormal);
            Double mixedProdact1mk = Vector3DUtils.MixedProduct(nodek.NodeNormal, node1.NodeNormal, nodem.NodeNormal);
            // must be mixedProdact12k < 0, mixedProdact1mk > 0
            if(!approxComparer.LT(mixedProdact12k, 0) || !approxComparer.GT(mixedProdact1mk, 0)) return false;
            // ����������� ����� 2
            for(IPolyhedron3DGraphNode nodei = node2; nodei != nodem; nodei = sectorNodeList.GetNextItem(nodei))
            {
                IPolyhedron3DGraphNode nodeip1 = sectorNodeList.GetNextItem(nodei);

                Vector3D crossingVector =
                    Vector3DUtils.VectorProduct(Vector3DUtils.VectorProduct(node1.NodeNormal, nodek.NodeNormal),
                                                Vector3DUtils.VectorProduct(nodei.NodeNormal, nodeip1.NodeNormal));

                Double scalarProduct1 = Vector3DUtils.ScalarProduct(node1.NodeNormal, crossingVector);
                Double scalarProductk = Vector3DUtils.ScalarProduct(nodek.NodeNormal, crossingVector);
                Double scalarProduct1k = Vector3DUtils.ScalarProduct(node1.NodeNormal, nodek.NodeNormal);

                Double scalarProducti = Vector3DUtils.ScalarProduct(nodei.NodeNormal, crossingVector);
                Double scalarProductip1 = Vector3DUtils.ScalarProduct(nodeip1.NodeNormal, crossingVector);
                Double scalarProductiip1 = Vector3DUtils.ScalarProduct(nodei.NodeNormal, nodeip1.NodeNormal);

                Double lambda1 = (scalarProduct1 - scalarProductk*scalarProduct1k)/(1 - scalarProduct1k*scalarProduct1k);
                Double lambda2 = (scalarProductk - scalarProduct1*scalarProduct1k)/(1 - scalarProduct1k*scalarProduct1k);
                Double lambda3 = (scalarProducti - scalarProductip1*scalarProductiip1)/
                                 (1 - scalarProductiip1*scalarProductiip1);
                Double lambda4 = (scalarProductip1 - scalarProducti*scalarProductiip1)/
                                 (1 - scalarProductiip1*scalarProductiip1);
                Double[] lambdas = new[] {lambda1, lambda2, lambda3, lambda4};

                // must be max(lambda1, lambda2, lambda3, lambda4) > 0
                if(!approxComparer.GT(lambdas.Max(), 0)) return false;
                // must be min(lambda1, lambda2, lambda3, lambda4) < 0
                if(!approxComparer.LT(lambdas.Min(), 0)) return false;
            }

            return true;
        }

        // �������� ����� 1k � ������� ... ������ ��. �������� ...
        private Boolean CheckConnInSector(IList<IPolyhedron3DGraphNode> visibleNodes,
                                          IPolyhedron3DGraphNode initialNode,
                                          IPolyhedron3DGraphNode checkedNode)
        {
            Int32 checkedNodeIndex = visibleNodes.IndexOf(checkedNode);
            // ��� ������������
            IPolyhedron3DGraphNode node1 = initialNode;
            IPolyhedron3DGraphNode nodek = checkedNode;
            // visibleNodes[0] == l2, visibleNodes[visibleNodes.Count-1] == lm,
            for(Int32 nodei1Index = 0; nodei1Index < checkedNodeIndex; ++nodei1Index)
            {
                IPolyhedron3DGraphNode nodei1 = visibleNodes[nodei1Index];
                for(Int32 nodei2Index = checkedNodeIndex + 1; nodei2Index < visibleNodes.Count; ++nodei2Index)
                {
                    IPolyhedron3DGraphNode nodei2 = visibleNodes[nodei2Index];
                    // ���� ����� 1k �� ������� � ��������� i1, i2, �� ����� 1k �������� �� ������
                    if(!CheckConnConvexity(node1, nodek, nodei1, nodei2)) return false;
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
                if (approxComparer.GE(scalarProduct, 0))
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
                if (approxComparer.LE(scalarProduct, 0))
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
            return approxComparer.GE(alpha, 0) && approxComparer.GE(beta, 0);
        }*/

        private readonly ApproxComp approxComparer;
        private readonly ILinearEquationsSystemSolver lesSolver;
    }
}