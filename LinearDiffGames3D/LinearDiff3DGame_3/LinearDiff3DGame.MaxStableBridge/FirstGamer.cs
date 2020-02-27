using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.Geometry3D;
/*using LinearDiff3DGame.System;*/

namespace LinearDiff3DGame.MaxStableBridge
{
    /// <summary>
    /// ����� �������������� � ��������������� �������� ������� ������
    /// � ���������� �������� ������� ������ �� �������� ���� G(...Fi...)
    /// </summary>
    internal class FirstGamer
    {
        /// <summary>
        /// ����������� ������ FirstGamer
        /// </summary>
        /// <param name="approxComparer">������������, ��� ������������� ��������� �������������� �����</param>
        /// <param name="matrixB">������� (�������) B ��� ������� ������� ������</param>
        /// <param name="deltaT">��� ��������� ��� t</param>
        /// <param name="mpMax">������������ �������� ����������� (� ���� �������) �� ��������� ������� ������� ������</param>
        /// <param name="mpMin">����������� �������� ����������� (� ���� �������) �� ��������� ������� ������� ������</param>
        public FirstGamer(ApproxComp approxComparer, Matrix matrixB, Double deltaT, Double mpMax, Double mpMin)
        {
            m_ApproxComparer = approxComparer;
            m_MatrixB = matrixB;
            m_DeltaT = deltaT;
            m_MpMax = mpMax;
            m_MpMin = mpMin;
        }

        /// <summary>
        /// ����� Action - ��� �������� ������� ������ ��� �������� (������) � ������ ������ �������
        /// </summary>
        /// <param name="graph">����, ��������������� ������������� Wi</param>
        /// <param name="fundCauchyMatrix">��������������� ������� ���� (������ �� �����) � ������ ������ �������</param>
        /// <returns>���� �������, ����� �������� ������� ������</returns>
        public Polyhedron3DGraph Action(Polyhedron3DGraph graph, Matrix fundCauchyMatrix)
        {
            // ������� (�������) D ��� ������� ������� ������ � ������ ������ �������
            Matrix matrixD = fundCauchyMatrix * m_MatrixB;

            // ��������� ������ ������� ������ ������� Pi
            List<Vector3D> pointPiSet = new List<Vector3D>(2);
            pointPiSet.Add(new Vector3D(m_MpMax * matrixD[1, 1], m_MpMax * matrixD[2, 1], m_MpMax * matrixD[3, 1]));
            pointPiSet.Add(new Vector3D(m_MpMin * matrixD[1, 1], m_MpMin * matrixD[2, 1], m_MpMin * matrixD[3, 1]));

            // ������������ ������ ������� Pi
            Vector3D directingVectorPi = new Vector3D(matrixD[1, 1], matrixD[2, 1], matrixD[3, 1]);
            directingVectorPi.Normalize();

            // ���������� ����� � ����� G(...Wi...)
            Int32 graphGWiNodeCount = graph.NodeList.Count;
            // ������ ���� G(...Fi...); ��� ���� ���� �������� �� ������, � �� ���� ����������� ����� graph G(...Wi...)
            Polyhedron3DGraph graphGFi = BuildGFiGrid(graph, directingVectorPi);

            // ������� ������� ������� ��� ������ ����� (��� ������������� Fi)
            for (Int32 nodeIndex = 0; nodeIndex < graphGWiNodeCount; ++nodeIndex)
            {
#warning ����� ����� !!!!!! ��������� ������������ ��������� �������� ������� �������
                Polyhedron3DGraphNode currentNode = graphGFi.NodeList[nodeIndex];
                currentNode.SupportFuncValue += m_DeltaT * Math.Max(-(currentNode.NodeNormal * pointPiSet[0]),
                                                                    -(currentNode.NodeNormal * pointPiSet[1]));
            }
            // ������� ������� ������� ��� ������ ����� (��� ������������� Fi)

            return graphGFi;
        }

        /// <summary>
        /// ����� BuildGFiGrid ������ ����� G(...Fi...) (��. ��������)
        /// </summary>
        /// <param name="graph">����, ������� ������������� �� ����� G(...Fi...)</param>
        /// <param name="directingVectorPi">������������ ������ ������� Pi</param>
        /// <returns>����� G(...Fi...)</returns>
        private Polyhedron3DGraph BuildGFiGrid(Polyhedron3DGraph graph, Vector3D directingVectorPi)
        {
            // ������ ��� ������ ����������� ����� � G(...Pi...)
            CrossingObjectFinder finder = new CrossingObjectFinder(m_ApproxComparer);

            // ������ (�����������) ������ �����������
            CrossingObject firstCrossingObject = finder.GetFirstCrossingObject(graph.NodeList[0], directingVectorPi);
            // ������� ������ �����������
            CrossingObject currentCrossingObject = firstCrossingObject;
            // ������ ���� �� ����������� �������� ������� � G(...Pi...) � ���������� ���
            // ���� ���� ���� ����������� � ������ �����, �� ��������� ��� � ��������������� ������ �� ������ ����
            Polyhedron3DGraphNode firstCrossingNode = BuildCrossingNode(currentCrossingObject, graph, directingVectorPi);
            // ������� ���� �����������
            Polyhedron3DGraphNode currentCrossingNode = firstCrossingNode;

            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
            {
                graph.NodeList.Add(currentCrossingNode);
                AddCrossingNodeBetweenConn(currentCrossingObject.PositiveNode, currentCrossingObject.NegativeNode, currentCrossingNode);
            }

            // ���� (���� ������� ������ �� ������ ������ ������������)
            do
            {
                // ���������� ������ �����������
                CrossingObject previousCrossingObject = currentCrossingObject;
                // ���������� ���� �����������
                Polyhedron3DGraphNode previousCrossingNode = currentCrossingNode;
                // �������� ��������� �� �������� ������ (�����, ���� ����) � ������ ��� �������
                currentCrossingObject = finder.GetNextCrossingObject(currentCrossingObject, currentCrossingNode, directingVectorPi);
                // ������ ���� �� ����������� �������� ������� � G(...Pi...)
                // ���� ���� ���� ����������� � ������ ����� (���� ���� ����� �������������� � ������ �����, ���� ������� ������ � ����, ���� ���� ��������� �������� ���� ����� � �� � ��� ������), �� ��������� ��� � ��������������� ������ �� ������ ����
                // �������� ������������ ������ ���� �� ������ � ������ (�����������) ������ ����������� (��� �������� ���������� ���������)
                currentCrossingNode = (currentCrossingObject == firstCrossingObject ?
                                       firstCrossingNode :
                                       BuildCrossingNode(currentCrossingObject, graph, directingVectorPi));
                if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
                    currentCrossingObject != firstCrossingObject)
                {
                    graph.NodeList.Add(currentCrossingNode);
                    AddCrossingNodeBetweenConn(currentCrossingObject.PositiveNode, currentCrossingObject.NegativeNode, currentCrossingNode);
                }

                // ���� ���������� � ������� ������� � ����
                if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode &&
                    currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
                {
                    // ������� � ��������� �������� �����
                    // continue;
                }

                // ���� ���������� ������ ����, � ������� �����
                if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode &&
                    currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
                {
                    // ������ ����� ����� ���������� ����� � ����� ����������� �� ������� �������
                    AddConns4PrevNodeCurrentConnCase(previousCrossingObject, previousCrossingNode, currentCrossingObject, currentCrossingNode);
                }

                // ���� ���������� ������ �����, � ������� ����
                if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
                    currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
                {
                    // ������ ����� ����� ����� ����������� �� ���������� ������� � ������� ����
                    AddConns4PrevConnCurrentNodeCase(previousCrossingObject, previousCrossingNode, currentCrossingObject, currentCrossingNode);
                }

                // ���� ���������� � ������� ������� - �����
                if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
                    currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
                {
                    // ������ ����� ����� ����� ����������� �� ���������� ������� � ����� ����������� �� ������� �������
                    // ������ ����� ����� ����� ����������� �� ���������� ������� � ����� ������� �����, ������� �� ����������� ���������� �����
                    AddConns4PrevConnCurrentConnCase(previousCrossingObject, previousCrossingNode, currentCrossingObject, currentCrossingNode);
                }
            }
            while (currentCrossingObject != firstCrossingObject);
            // ���� (���� ������� ������ �� ������ ������ ������������)

            return graph;
        }

        /// <summary>
        /// ����� CalcCrossingNodeNormal ��������� ������� ���� �� ������� ����������� ����� � G(...Pi...)
        /// </summary>
        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Pi...)</param>
        /// <param name="directingVectorPi">������������ ������ ������� Pi</param>
        /// <returns>������� ���� �� ������� ����������� ����� � G(...Pi...)</returns>
        private Vector3D CalcCrossingNodeNormal(CrossingObject currentCrossingObject, Vector3D directingVectorPi)
        {
            Vector3D crossingNodeNormal = Vector3D.ZeroVector3D;

            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
            {
                Vector3D plusVector = currentCrossingObject.PositiveNode.NodeNormal;
                Vector3D minusVector = currentCrossingObject.NegativeNode.NodeNormal;
                // ������ ������, ���������������� ��������, ��������� ������� ������,
                // ��� ��������� ������������ �������������� ���� ����� �� �������������
                Vector3D npm = Vector3D.VectorProduct(plusVector, minusVector);
                // ��������� ��������� ������������ ������������ ������� � ������������� ������� Pi
                crossingNodeNormal = Vector3D.VectorProduct(npm, directingVectorPi);
                crossingNodeNormal.Normalize();
            }
            else
            {
                crossingNodeNormal = currentCrossingObject.PositiveNode.NodeNormal;
            }

            return crossingNodeNormal;
        }

        /// <summary>
        /// ����� BuildCrossingNode ������� � ���������� ���� �� ������� ����������� ����� � G(...Pi...)
        /// </summary>
        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Pi...)</param>
        /// <param name="graph">����</param>
        /// <param name="directingVectorPi">������������ ������ ������� Pi</param>
        /// <returns>��������� ���� �� ������� ����������� ����� � G(...Pi...)</returns>
        private Polyhedron3DGraphNode BuildCrossingNode(CrossingObject currentCrossingObject, Polyhedron3DGraph graph, Vector3D directingVectorPi)
        {
            Polyhedron3DGraphNode crossingNode = null;

            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
            {
                Vector3D plusNodeNormal = currentCrossingObject.PositiveNode.NodeNormal;
                Vector3D minusNodeNormal = currentCrossingObject.NegativeNode.NodeNormal;
                Vector3D crossingNodeNormal = CalcCrossingNodeNormal(currentCrossingObject, directingVectorPi);
                
                // ������ ����, ��������� � ���������� (����) �������� � ���������� ���
                crossingNode = new Polyhedron3DGraphNode(graph.NodeList.Count, crossingNodeNormal);
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
            }
            else
            {
                // ��������� ��������� ������������ �������, ���������� � ������� �����, � ������������� ������� Pi
                // ���� ��������� ������������ <> 0, �� ��� ������ ������ ���������
#warning Check is absent !!!

                crossingNode = currentCrossingObject.PositiveNode;
            }

            return crossingNode;
        }

        /// <summary>
        /// ����� AddCrossingNodeBetweenConn ��������� ���� crossingNode �� ����������� ����� � G(...Pi...) � ��������������� ������� ������/��������� ������
        /// </summary>
        /// <param name="connPlusNode">������������� ���� ������������ �����</param>
        /// <param name="connMinusNode">������������� ���� ������������ �����</param>
        /// <param name="crossingNode">���� �� ����������� ����� � G(...Pi...)</param>
        private void AddCrossingNodeBetweenConn(Polyhedron3DGraphNode connPlusNode, Polyhedron3DGraphNode connMinusNode, Polyhedron3DGraphNode crossingNode)
        {
            // ���������� �� ����� ����� �������
            /*// ��������� ����� ���� � ������ ����� �����
            graph.NodeList.Add(crossingNode);*/
            // ���������� �� ����� ����� �������

            // ��������� � ������ ������ ������ ���� ������ ������� �� ������������� ���� �����, ����� �� �������������
            crossingNode.ConnectionList.Add(connPlusNode);
            crossingNode.ConnectionList.Add(connMinusNode);
            // ��� �����, ���������� �����, ������ �� ������ ���� �� ����� (������� � �������� �����) �� ������ �� ����� ����
            connPlusNode.ConnectionList[connPlusNode.ConnectionList.IndexOf(connMinusNode)] = crossingNode;
            connMinusNode.ConnectionList[connMinusNode.ConnectionList.IndexOf(connPlusNode)] = crossingNode;
        }

        /// <summary>
        /// ����� AddConns4PrevNodeCurrentConnCase ��������� ����������� ����� � ������, ���� ���������� ������ ����������� - ����, � ������� - �����
        /// ����� ����������� ��� ����, ����� ���� ��������� �����������������
        /// </summary>
        /// <param name="previousCrossingObject">���������� ������ ����������� ����� � G(...Pi...)</param>
        /// <param name="previousCrossingNode">���� �� ���������� ����������� ����� � G(...Pi...)</param>
        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Pi...)</param>
        /// <param name="currentCrossingNode">���� �� ������� ����������� ����� � G(...Pi...)</param>
        private void AddConns4PrevNodeCurrentConnCase(CrossingObject previousCrossingObject, Polyhedron3DGraphNode previousCrossingNode, CrossingObject currentCrossingObject, Polyhedron3DGraphNode currentCrossingNode)
        {
            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode &&
                         currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection,
                         "previous crossing object must be node and current crossing object - connection");

            // ������ ����� ����� ���������� ����� � ����� ����������� �� ������� ������� :

            // ������������� ���� ������� �����
            Polyhedron3DGraphNode connMinusNode = currentCrossingObject.NegativeNode;
            // ������ �� ������� ���� ����������� � ������ ������ ����������� ���� ��������� ����� ������ �� ������������� ���� ������� �����
            /*Int32 PrevNode2CurrentMinusNodeConnIndex = PreviousCrossingNode.GetConnectionIndex(CurrentConnMinusNode);
            PreviousCrossingNode.InsertNodeConnection(PrevNode2CurrentMinusNodeConnIndex + 1, CurrentCrossingNode);*/
            previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(connMinusNode) + 1,
                                                       currentCrossingNode);
            // ������ �� ���������� ���� ��������� ����� ������ �� ������������� ���� ������� ����� (�� ������� ����� 1)
            currentCrossingNode.ConnectionList.Insert(1, previousCrossingNode);
        }

        /// <summary>
        /// ����� AddConns4PrevConnCurrentNodeCase ��������� ����������� ����� � ������, ���� ���������� ������ ����������� - �����, � ������� - ����
        /// ����� ����������� ��� ����, ����� ���� ��������� �����������������
        /// </summary>
        /// <param name="previousCrossingObject">���������� ������ ����������� ����� � G(...Pi...)</param>
        /// <param name="previousCrossingNode">���� �� ���������� ����������� ����� � G(...Pi...)</param>
        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Pi...)</param>
        /// <param name="currentCrossingNode">���� �� ������� ����������� ����� � G(...Pi...)</param>
        private void AddConns4PrevConnCurrentNodeCase(CrossingObject previousCrossingObject, Polyhedron3DGraphNode previousCrossingNode, CrossingObject currentCrossingObject, Polyhedron3DGraphNode currentCrossingNode)
        {
            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
                         currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode,
                         "previous crossing object must be connection and current crossing object - node");

            // ������ ����� ����� ����� ����������� �� ���������� ������� � ������� ���� :

            // ������������� ���� ���������� �����
            Polyhedron3DGraphNode connPlusNode = previousCrossingObject.PositiveNode;
            // ������ �� ���������� ���� ����������� � ������ ������ �������� ���� ��������� ����� ������ �� ������������� ���� ���������� �����
            /*Int32 CurrentNode2PrevPlusNodeConnIndex = CurrentCrossingNode.GetConnectionIndex(PreviousConnPlusNode);
            CurrentCrossingNode.InsertNodeConnection(CurrentNode2PrevPlusNodeConnIndex + 1, PreviousCrossingNode);*/
            currentCrossingNode.ConnectionList.Insert(currentCrossingNode.ConnectionList.IndexOf(connPlusNode) + 1,
                                                      previousCrossingNode);
            // ������ �� ������� ���� ��������� � ����� ������ ������ ����������� ����
            previousCrossingNode.ConnectionList.Add(currentCrossingNode);
        }

        /// <summary>
        /// ����� AddConns4PrevConnCurrentConnCase ��������� ����������� ����� � ������, ���� � ����������, � ������� ������� ����������� - �����
        /// ����� ����������� ��� ����, ����� ���� ��������� �����������������
        /// </summary>
        /// <param name="previousCrossingObject">���������� ������ ����������� ����� � G(...Pi...)</param>
        /// <param name="previousCrossingNode">���� �� ���������� ����������� ����� � G(...Pi...)</param>
        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Pi...)</param>
        /// <param name="currentCrossingNode">���� �� ������� ����������� ����� � G(...Pi...)</param>
        private void AddConns4PrevConnCurrentConnCase(CrossingObject previousCrossingObject, Polyhedron3DGraphNode previousCrossingNode, CrossingObject currentCrossingObject, Polyhedron3DGraphNode currentCrossingNode)
        {
            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
                         currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection,
                         "previous and current crossing objects must be connections");

            // ������ ����� ����� ����� ����������� �� ���������� ������� � ����� ����������� �� ������� �������
            // ������ ����� ����� ����� ����������� �� ���������� ������� � ����� ������� �����, ������� �� ����������� ���������� �����
            // � ������ ����� ������������� ���� (������ 3�)
            if (Object.ReferenceEquals(previousCrossingObject.NegativeNode, currentCrossingObject.NegativeNode))
            {
                // ������������� ���� ���������� ����� (���� ����� 1)
                Polyhedron3DGraphNode node1 = previousCrossingObject.PositiveNode;
                // ����� ������������� ���� (���� ����� 2)
                // Polyhedron3DGraphNode node2 = previousCrossingObject.NegativeNode;
                // ������������� ���� ������� ����� (���� ����� 3)
                Polyhedron3DGraphNode node3 = currentCrossingObject.PositiveNode;
                // ��� ���� ����� 3: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ���� 1
                /*Int32 CurrentPlusNode2PrevPlusNodeConnIndex = CurrentConnPlusNode.GetConnectionIndex(PreviousConnPlusNode);
                CurrentConnPlusNode.InsertNodeConnection(CurrentPlusNode2PrevPlusNodeConnIndex + 1, PreviousCrossingNode);*/
                node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(node1) + 1, previousCrossingNode);
                // ��� ����������� ���� �����������: � ����� ������ ������ ����������� ������� ������ �� ����� ���� �����������, ����� ������ �� ���� ����� 3
                /*PreviousCrossingNode.AddNodeConnection(CurrentCrossingNode);
                PreviousCrossingNode.AddNodeConnection(CurrentConnPlusNode);*/
                previousCrossingNode.ConnectionList.Add(currentCrossingNode);
                previousCrossingNode.ConnectionList.Add(node3);
                // ��� �������� ���� �����������: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ���� ����� 3 (�.�. �� ������� ����� 1)
                currentCrossingNode.ConnectionList.Insert(1, previousCrossingNode);
            }
            // � ������ ����� ������������� ���� (������ 3�)
            else if (Object.ReferenceEquals(previousCrossingObject.PositiveNode, currentCrossingObject.PositiveNode))
            {
                // ������������� ���� ���������� ����� (���� ����� 1)
                Polyhedron3DGraphNode node1 = previousCrossingObject.NegativeNode;
                // ����� ������������� ���� (���� ����� 2)
                // Polyhedron3DGraphNode node2 = previousCrossingObject.PositiveNode;
                // ������������� ���� ������� ����� (���� ����� 3)
                Polyhedron3DGraphNode node3 = currentCrossingObject.NegativeNode;
                // ��� ���� ����� 3: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ������� ���� �����������
                /*Int32 CurrentMinusNode2CurrentCrossingNodeConnIndex = CurrentConnMinusNode.GetConnectionIndex(CurrentCrossingNode);
                CurrentConnMinusNode.InsertNodeConnection(CurrentMinusNode2CurrentCrossingNodeConnIndex + 1, PreviousCrossingNode);*/
                node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(currentCrossingNode) + 1, previousCrossingNode);
                // ��� ����������� ���� �����������: � ����� ������ ������ ����������� ������� ������ �� ���� ����� 3, ����� ������ �� ����� ���� �����������
                /*PreviousCrossingNode.AddNodeConnection(CurrentConnMinusNode);
                PreviousCrossingNode.AddNodeConnection(CurrentCrossingNode);*/
                previousCrossingNode.ConnectionList.Add(node3);
                previousCrossingNode.ConnectionList.Add(currentCrossingNode);
                // ��� �������� ���� �����������: ������ �� ���������� ���� ����������� ����������� ����� ������ �� ���� ����� 1 (�.�. �� ������� ����� 1)
                currentCrossingNode.ConnectionList.Insert(1, previousCrossingNode);
            }
            // ������ ������ ���������
            else
            {
#warning ����� ����� ������������������ ����������
                throw new Exception("AddConns4PrevConnCurrentConnCase method incorrect work");
            }
        }

        /// <summary>
        /// ������������, ��� ������������� ��������� �������������� �����
        /// </summary>
        private ApproxComp m_ApproxComparer;

        /// <summary>
        /// ������� (�������) B ��� ������� ������� ������
        /// </summary>
        private Matrix m_MatrixB;

        /// <summary>
        /// ��� ��������� ��� t
        /// </summary>
        private Double m_DeltaT;
        /// <summary>
        /// ������������ �������� ����������� (� ���� �������) �� ��������� ������� ������� ������
        /// </summary>
        private Double m_MpMax;
        /// <summary>
        /// ����������� �������� ����������� (� ���� �������) �� ��������� ������� ������� ������
        /// </summary>
        private Double m_MpMin;
    }
}
