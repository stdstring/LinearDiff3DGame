using System;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.Geometry3D;

namespace LinearDiff3DGame.MaxStableBridge
{
    /// <summary>
    /// ����� ��� ���������� ����������� ����� � G(...Xi...), ��� Xi - ����� �������, G(...Xi...) - ������� ����, ���������������� Xi � ���������� ����� ����� 0
    /// </summary>
    internal class CrossingObjectFinder
    {
        /// <summary>
        /// ����������� ������ CrossingObjectFinder
        /// </summary>
        /// <param name="approxComparer">������������, ��� ������������� ��������� �������������� �����</param>
        public CrossingObjectFinder(ApproxComp approxComparer)
        {
            m_ApproxComparer = approxComparer;
        }

        /// <summary>
        /// ����� GetFirstCrossingObject ���������� ������ ������ ����������� ����� � G(...Xi...)
        /// </summary>
        /// <param name="startNode">���� �����, � �������� ���������� �����</param>
        /// <param name="directingVectorXi">������������ ������ ������� Xi</param>
        /// <returns>������ ������ ����������� ����� � G(...Xi...)</returns>
        public CrossingObject GetFirstCrossingObject(Polyhedron3DGraphNode startNode, Vector3D directingVectorXi)
        {
            CrossingObject firstCrossingObject = null;

            // ������� ����
            Polyhedron3DGraphNode currentNode = startNode;
            // ��������� ��������� ������������ �������, ���������� � ������� �����, � ������������� ������� ������� Xi
            Double currentScalarProduct = Vector3D.ScalarProduct(currentNode.NodeNormal, directingVectorXi);
            // ���� ��������� ������������ = 0, �� ������� ���� ���������� ������� ��������
            if (m_ApproxComparer.EQ(currentScalarProduct, 0))
            {
                firstCrossingObject = new CrossingObject(CrossingObjectType.GraphNode, currentNode, currentNode);
            }

            // ���� ���� �� ������ ������� ������ ������ �����������
            while (Object.ReferenceEquals(firstCrossingObject, null))
            {
                Double bestScalarProduct = Double.NaN;
                Polyhedron3DGraphNode bestNode = null;

                // ���� �� ���� ������ �������� ����
                for (Int32 connIndex = 0; connIndex < currentNode.ConnectionList.Count; ++connIndex)
                {
                    // ������� ����� �������� ����
                    Polyhedron3DGraphNode currentConnNode = currentNode.ConnectionList[connIndex];
                    // ������� ��������� ������������ �������, ���������� � ���������� ���� �����, � ������������� ������� ������� Xi
                    Double scalarProduct = Vector3D.ScalarProduct(currentConnNode.NodeNormal, directingVectorXi);

                    // ���� ��������� ������������ = 0, �� ���������� ���� ���������� ������� ��������
                    if (m_ApproxComparer.EQ(scalarProduct, 0))
                    {
                        firstCrossingObject = new CrossingObject(CrossingObjectType.GraphNode, currentConnNode, currentConnNode);
                        break;
                    }

                    // ���� ��������� ������������ ���� �� �����, ��� � ��� �������, ���������� � ������� �����, ��
                    // ���� �� ����������� �������� �������� ���������� ������������ ������ ������������, �� ���������� �������� � ���������� ����
                    if (Math.Sign(currentScalarProduct) == Math.Sign(scalarProduct))
                    {
                        if (Double.IsNaN(bestScalarProduct) ||
                            (Math.Abs(scalarProduct) < Math.Abs(bestScalarProduct)))
                        {
                            bestScalarProduct = scalarProduct;
                            bestNode = currentConnNode;
                        }
                    }
                    // ���� ���� ���������� ������������ ��� ����������� (����) �������, ���������� �� ����� ���������� ������������ ��� �������, ���������� � ������� �����, ��
                    // �����, ����������� ������� � ���������� (����) ���� ���������� ������� ��������
                    else
                    {
                        Polyhedron3DGraphNode plusNode = (currentScalarProduct > 0 ? currentNode : currentConnNode);
                        Polyhedron3DGraphNode minusNode = (currentScalarProduct < 0 ? currentNode : currentConnNode);

                        firstCrossingObject = new CrossingObject(CrossingObjectType.GraphConnection, plusNode, minusNode);
                        break;
                    }
                }

                // ������� ����� ���������� ����������� ����
                currentNode = bestNode;
                currentScalarProduct = bestScalarProduct;
            }

            return firstCrossingObject;
        }

        /// <summary>
        /// ����� GetNextCrossingObject ���������� ��������� �� ����������� �������� ������ �����������
        /// ��� ���� ������� ������ ����������� currentCrossingObject ����� ���� "��������" ��-�� ������� 
        /// ������ ���� ���� (���� ������� ������ ����������� - �����) currentCrossingNode �� ����������� ������� � G(...Xi...)
        /// </summary>
        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Xi...)</param>
        /// <param name="currentCrossingNode">���� �� ������� ����������� ����� � G(...Xi...)</param>
        /// <param name="directingVectorXi">������������ ������ ������� Xi</param>
        /// <returns>��������� �� ����������� �������� ������ �����������</returns>
        public CrossingObject GetNextCrossingObject(CrossingObject currentCrossingObject, Polyhedron3DGraphNode currentCrossingNode, Vector3D directingVectorXi)
        {
            CrossingObject nextCrossingObject = null;

            // ���� ������� ������ � ����
            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
            {
                nextCrossingObject = GetNextCrossingObject4GraphNode(currentCrossingObject, directingVectorXi);
            }
            // ���� ������� ������ � �����
            else
            {
                nextCrossingObject = GetNextCrossingObject4GraphConn(currentCrossingObject, currentCrossingNode, directingVectorXi);
            }

            return nextCrossingObject;
        }

        /// <summary>
        /// ����� GetNextCrossingObject ���������� ��������� �� ����������� �������� ������ �����������
        /// </summary>
        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Xi...)</param>
        /// <param name="directingVectorXi">������������ ������ ������� Xi</param>
        /// <returns>��������� �� ����������� �������� ������ �����������</returns>
        public CrossingObject GetNextCrossingObject(CrossingObject currentCrossingObject, Vector3D directingVectorXi)
        {
            CrossingObject nextCrossingObject = null;

            // ���� ������� ������ � ����
            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
            {
                nextCrossingObject = GetNextCrossingObject4GraphNode(currentCrossingObject, directingVectorXi);
            }
            // ���� ������� ������ � �����
            else
            {
                nextCrossingObject = GetNextCrossingObject4GraphConn(currentCrossingObject, directingVectorXi);
            }

            return nextCrossingObject;
        }

        /// <summary>
        /// ����� GetNextCrossingObject4GraphNode ���������� ��������� �� ����������� �������� ������ �����������, ���� ������� - ����
        /// </summary>
        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Xi...)</param>
        /// <param name="directingVectorXi">������������ ������ ������� Xi</param>
        /// <returns>��������� �� ����������� �������� ������ �����������</returns>
        private CrossingObject GetNextCrossingObject4GraphNode(CrossingObject currentCrossingObject, Vector3D directingVectorXi)
        {
            CrossingObject nextCrossingObject = null;

            Polyhedron3DGraphNode currentNode = currentCrossingObject.PositiveNode;
            // ���� �� ���� ������ �������� ����
            for (Int32 connIndex = 0; connIndex < currentNode.ConnectionList.Count; ++connIndex)
            {
                // �������� ���� (����� 1), ��������� � ������� ����� ������� ������
                Polyhedron3DGraphNode node1 = currentNode.ConnectionList[connIndex];
                // �������� ���� (����� 2), ��������� � ������� ����� ���������� ������
                Polyhedron3DGraphNode node2 = currentNode.ConnectionList.GetPrevItem(connIndex);

                // ��������� ��������� ������������ ������� 1 � ������������� ������� Xi
                Double scalarProduct1 = Vector3D.ScalarProduct(node1.NodeNormal, directingVectorXi);
                // ��������� ��������� ������������ ������� 2 � ������������� ������� Xi
                Double scalarProduct2 = Vector3D.ScalarProduct(node2.NodeNormal, directingVectorXi);

                // ���� ��������� ������������ ���� 1 � ������������� ������� Xi == 0
                if (m_ApproxComparer.EQ(scalarProduct1, 0))
                {
                    // ���� ����������� �������� ������� ���������, �� ���� ����� 1 ���������� ��������� �� �������� ��������
                    nextCrossingObject = new CrossingObject(CrossingObjectType.GraphNode, node1, node1);
                    if (CheckMoveDirection(nextCrossingObject, currentCrossingObject, directingVectorXi))
                    {
                        break;
                    }
                    else
                    {
                        nextCrossingObject = null;
                    }
                }

                // ���� ��������� ������������ ���� 2 � ������������� ������� Xi == 0
                if (m_ApproxComparer.EQ(scalarProduct2, 0))
                {
                    // ���� ����������� �������� ������� ���������, �� ���� ����� 2 ���������� ��������� �� �������� ��������
                    nextCrossingObject = new CrossingObject(CrossingObjectType.GraphNode, node2, node2);
                    if (CheckMoveDirection(nextCrossingObject, currentCrossingObject, directingVectorXi))
                    {
                        break;
                    }
                    else
                    {
                        nextCrossingObject = null;
                    }
                }

                // ���� ��������� ������������ ����� 1 � 2 � ������������� ������� Xi ����� ������ ����
                if (Math.Sign(scalarProduct1) != Math.Sign(scalarProduct2))
                {
                    // ���� ����������� �������� ������� ���������, �� �����, ����������� ���� 1 � 2, ���������� ��������� �� �������� ��������
                    Polyhedron3DGraphNode plusNode = (scalarProduct1 > 0 ? node1 : node2);
                    Polyhedron3DGraphNode minusNode = (scalarProduct1 < 0 ? node1 : node2);
                    nextCrossingObject = new CrossingObject(CrossingObjectType.GraphConnection, plusNode, minusNode);
                    if (CheckMoveDirection(nextCrossingObject, currentCrossingObject, directingVectorXi))
                    {
                        break;
                    }
                    else
                    {
                        nextCrossingObject = null;
                    }
                }
            }
            // ���� �� ���� ������ �������� ����

            return nextCrossingObject;
        }

        /// <summary>
        /// ����� GetNextCrossingObject4GraphConn ���������� ��������� �� ����������� �������� ������ �����������, ���� ������� - �����
        /// ��� ���� ������� ������ ����������� currentCrossingObject ����� ���� "��������" ��-�� ������� 
        /// ������ ���� ���� currentCrossingNode �� ����������� ������� � G(...Xi...)
        /// </summary>
        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Xi...)</param>
        /// <param name="currentCrossingNode">���� �� ������� ����������� ����� � G(...Xi...)</param>
        /// <param name="directingVectorXi">������������ ������ ������� Xi</param>
        /// <returns>��������� �� ����������� �������� ������ �����������</returns>
        private CrossingObject GetNextCrossingObject4GraphConn(CrossingObject currentCrossingObject, Polyhedron3DGraphNode currentCrossingNode, Vector3D directingVectorXi)
        {
            CrossingObject nextCrossingObject = null;

            // ������������� ���� ������� �����
            Polyhedron3DGraphNode plusNode = currentCrossingObject.PositiveNode;
            // ������������� ���� ������� �����
            Polyhedron3DGraphNode minusNode = currentCrossingObject.NegativeNode;

            // ��� �������������� ���� (currentCrossingObject.PositiveNode) ����� ��������� ����� (������������ �������)
            Polyhedron3DGraphNode nextNode1 = plusNode.ConnectionList.GetNextItem(currentCrossingNode);
            // ��� �������������� ���� (currentCrossingObject.NegativeNode) ����� ���������� ����� (������������ �������)
            Polyhedron3DGraphNode nextNode2 = minusNode.ConnectionList.GetPrevItem(currentCrossingNode);

            Double scalarProduct1 = Vector3D.ScalarProduct(nextNode1.NodeNormal, directingVectorXi);
            Double scalarProduct2 = Vector3D.ScalarProduct(nextNode2.NodeNormal, directingVectorXi);

            // ���� ���������� ���� (����� 1) �������
            if (m_ApproxComparer.EQ(scalarProduct1, 0))
            {
                // ���� ���������� ���� ����� 2 �������
                if (m_ApproxComparer.EQ(scalarProduct2, 0))
                {
                    // ���������� ���� ���������� ��������� �� �������� ��������
                    nextCrossingObject = new CrossingObject(CrossingObjectType.GraphNode, nextNode1, nextNode1);
                    // exit
                }
                // ���� ���������� ���� ����� 2 ���������
                else
                {
                    // "�����" (��� �����, � ������� �� �������� ��������; ������� �� ��� ���) ����������� ������������� ���� � ���� ����� 2 ���������� ��������� �� �������� ��������
                    // ����� � ������� �� �������� �������� ��� ��-�� ����, ��� �� ������ ���� ����� ������ ����� ��� �������� ���� �� ����������� ���� ����� � G(...Pi...)
                    nextCrossingObject = new CrossingObject(CrossingObjectType.GraphConnection, plusNode, nextNode2);
                    // exit
                }
            }
            // ���� ���������� ���� (����� 1) �������������
            else if (m_ApproxComparer.GT(scalarProduct1, 0))
            {
                // �����, ����������� ����� ������������� ���� � ������ ������������� ����, ���������� ��������� �� �������� ��������
                nextCrossingObject = new CrossingObject(CrossingObjectType.GraphConnection, nextNode1, minusNode);
                // exit
            }
            // ���� ���������� ���� (����� 1) �������������
            else if (m_ApproxComparer.LT(scalarProduct1, 0))
            {
                // �����, ����������� ����� ������������� ���� � ������ ������������� ����, ���������� ��������� �� �������� ��������
                nextCrossingObject = new CrossingObject(CrossingObjectType.GraphConnection, plusNode, nextNode2);
                // exit
            }

            return nextCrossingObject;
        }

        /// <summary>
        /// ����� GetNextCrossingObject4GraphConn ���������� ��������� �� ����������� �������� ������ �����������, ���� ������� - �����
        /// </summary>
        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Xi...)</param>
        /// <param name="directingVectorXi">������������ ������ ������� Xi</param>
        /// <returns>��������� �� ����������� �������� ������ �����������</returns>
        private CrossingObject GetNextCrossingObject4GraphConn(CrossingObject currentCrossingObject, Vector3D directingVectorXi)
        {
            CrossingObject nextCrossingObject = null;

            // ������������� ���� ������� �����
            Polyhedron3DGraphNode plusNode = currentCrossingObject.PositiveNode;
            // ������������� ���� ������� �����
            Polyhedron3DGraphNode minusNode = currentCrossingObject.NegativeNode;

            // ��� �������������� ���� (CurrentCrossingObject.PositiveNode) ����� ��������� ����� (������������ �������)
            Polyhedron3DGraphNode nextNode1 = plusNode.ConnectionList.GetNextItem(minusNode);
            // ��� �������������� ���� (CurrentCrossingObject.NegativeNode) ����� ���������� ����� (������������ �������)
            Polyhedron3DGraphNode nextNode2 = minusNode.ConnectionList.GetPrevItem(plusNode);

            Double scalarProduct1 = Vector3D.ScalarProduct(nextNode1.NodeNormal, directingVectorXi);
            Double scalarProduct2 = Vector3D.ScalarProduct(nextNode2.NodeNormal, directingVectorXi);

            // ���� ���������� ���� (����� 1) �������
            if (m_ApproxComparer.EQ(scalarProduct1, 0))
            {
                // ���� ���������� ���� ����� 2 �� �������
                if (m_ApproxComparer.NE(scalarProduct2, 0))
                {
#warning ����� ����� ������������������ ����������
                    throw new Exception("GetNextCrossingObject4GraphConn2 method incorrect work");
                }

                // ���������� ���� ���������� ��������� �� �������� ��������
                nextCrossingObject = new CrossingObject(CrossingObjectType.GraphNode, nextNode1, nextNode1);
                // exit
            }
            // ���� ���������� ���� (����� 1) �������������
            else if (m_ApproxComparer.GT(scalarProduct1, 0))
            {
                // �����, ����������� ����� ������������� ���� � ������ ������������� ����, ���������� ��������� �� �������� ��������
                nextCrossingObject = new CrossingObject(CrossingObjectType.GraphConnection, nextNode1, minusNode);
                // exit
            }
            // ���� ���������� ���� (����� 1) �������������
            else if (m_ApproxComparer.LT(scalarProduct1, 0))
            {
                // �����, ����������� ����� ������������� ���� � ������ ������������� ����, ���������� ��������� �� �������� ��������
                nextCrossingObject = new CrossingObject(CrossingObjectType.GraphConnection, plusNode, nextNode2);
                // exit
            }

            return nextCrossingObject;
        }

        /// <summary>
        /// ����� CalcCrossingNodeNormal ��������� ������� ���� �� ������� ����������� ����� � G(...Xi...)
        /// Xi - ����� �������, G(...Xi...) - ������� ����, ���������������� Xi � ���������� ����� ����� 0
        /// </summary>
        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Xi...)</param>
        /// <param name="directingVectorXi">������������ ������ ������� Xi</param>
        /// <returns>������� ���� �� ������� ����������� ����� � G(...Xi...)</returns>
        private Vector3D CalcCrossingNodeNormal(CrossingObject currentCrossingObject, Vector3D directingVectorXi)
        {
            Vector3D crossingNodeNormal = Vector3D.ZeroVector3D;

            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
            {
                Vector3D plusVector = currentCrossingObject.PositiveNode.NodeNormal;
                Vector3D minusVector = currentCrossingObject.NegativeNode.NodeNormal;
                // ������ ������, ���������������� ��������, ��������� ������� ������,
                // ��� ��������� ������������ �������������� ���� ����� �� �������������
                Vector3D npm = Vector3D.VectorProduct(plusVector, minusVector);
                // ��������� ��������� ������������ ������������ ������� � ������������� ������� Xi
                crossingNodeNormal = Vector3D.VectorProduct(npm, directingVectorXi);
                crossingNodeNormal.Normalize();
            }
            else
            {
                crossingNodeNormal = currentCrossingObject.PositiveNode.NodeNormal;
            }

            return crossingNodeNormal;
        }

        /// <summary>
        /// ����� CheckMoveDirection ���������� true, ���� ����������� �������� �� G(...Xi...) ����������, ����� ������������ false
        /// ���������� ��������� ����������� �������� ������ ������� �������, ���� �������� � ����� ������������� ������� Xi
        /// </summary>
        /// <param name="checkCrossingObject">����������� ������ ����������� ����� � G(...Xi...)</param>
        /// <param name="currentCrossingObject">������� ������ ����������� ����� � G(...Xi...)</param>
        /// <param name="directingVectorXi">������������ ������ ������� Xi</param>
        /// <returns>true, ���� ��� ���������� �������� ����������� �� ��������� ����������; ����� - false</returns>
        private Boolean CheckMoveDirection(CrossingObject checkCrossingObject, CrossingObject currentCrossingObject, Vector3D directingVectorXi)
        {
            // ������������ ������ Xi ���������� ����� ��� OZ
            Vector3D directingVectorOZ = directingVectorXi;

            // ������ ����������� �������� ������� � G(...Xi...); ������, ���������� ��� ���������� �����������, ���������� ����� ��� OX
            Vector3D directingVectorOX = CalcCrossingNodeNormal(currentCrossingObject, directingVectorXi);

            // ������ ��� ��� OY ������ �� XYZ (��� ��������� ������������ ���� ��� OZ �� ��� ��� OX)
            Vector3D directingVectorOY = Vector3D.VectorProduct(directingVectorOZ, directingVectorOX);

            // ������ ����������� ������������ ������� � G(...Xi...); ��������� ��������� ������������ �������, ����������� ��� ���������� �����������, � ���� ��� OY
            Vector3D checkVector = CalcCrossingNodeNormal(checkCrossingObject, directingVectorXi);
            Double scalarProduct = Vector3D.ScalarProduct(checkVector, directingVectorOY);

            // ���� ScalarProductValue = 0 - ��� ������ ������ ���������
            if (m_ApproxComparer.EQ(scalarProduct, 0))
            {
#warning ����� ����� ������������������ ����������
                throw new Exception("CheckMoveDirection method incorrect work");
            }

            // ���� ����������� ��������� ������������ > 0, �� ����������� �������� ����������, ����� ����������� �������� ������������
            return (scalarProduct > 0 ? true : false);
        }

        /// <summary>
        /// ������������, ��� ������������� ��������� �������������� �����
        /// </summary>
        private ApproxComp m_ApproxComparer;
    }
}
