using System;
using System.Collections.Generic;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    /// <summary>
    /// ����� ��� ������������ ����� ��������� 3-������� �������������
    /// </summary>
    public class Polyhedron3DGraphSimpleTriangulator
    {
        /// <summary>
        /// �������� ������������ ����� graph, ������������ �� ��������� ��������� 3-� ������� �������������
        /// ��� ���� �������������� (� ����� �� ����������� !!!), ��� ��� �������� � ����� - ��������
        /// </summary>
        /// <param name="graph">�������� ����</param>
        /// <returns>���� ����� �������� ������������</returns>
        public Polyhedron3DGraph Triangulate(Polyhedron3DGraph graph)
        {
            IList<Polyhedron3DGraphNode> nodeList = graph.NodeList;

            // ���� �� ���� ����� ����� �� ������ ����� �����
            for (Int32 nodeIndex = 0; nodeIndex < nodeList.Count; ++nodeIndex)
            {
                Polyhedron3DGraphNode currentNode = nodeList[nodeIndex];

                // ���� �� ���� ������ �������� ����
                for (Int32 connIndex = 0; connIndex < currentNode.ConnectionList.Count;)
                {
                    // ������� � ��������� ����� (����)
                    Polyhedron3DGraphNode currentConn = currentNode.ConnectionList[connIndex];
                    Polyhedron3DGraphNode nextConn = currentNode.ConnectionList.GetNextItem(connIndex);

                    // ������ ������������� ���� ������, ������������ �� ������� ����� � ��������������� �� ��������� �����
                    List<Polyhedron3DGraphNode> shortestGraphPath = GetShortestGraphPath(currentNode, currentConn,
                                                                                         nextConn);

                    // ���� ����� ����� � ����������� ���� < 2, �� ��� ������ !!!!!!
                    if (shortestGraphPath.Count < 2)
                    {
#warning ����� ����� ������������������ ����������
                        throw new Exception("Error at construction of the graph's path !!!");
                    }

                    // ���� �� 2-�� �� N-1 ���� �� ������������ ���� ������
                    // ���� ����� ����� � ����������� ���� = 2, �� ���� ���������� 0 ���
                    for (Int32 graphPathIndex = 1; graphPathIndex < shortestGraphPath.Count - 1; graphPathIndex++)
                    {
                        // � ������ ������ �������� ���� ��������� ����� (����� ������� ����� � i-� ����� �� ������������ ���� ������) �� ��������� ����������� ������ ��� �������, ���� ���������� �� ����
                        currentNode.ConnectionList.Insert(connIndex + 1, shortestGraphPath[graphPathIndex]);

                        // � ������ ������ i-�� ���� �� ������������ ���� ������ ��������� (��� ��) ����� ����� ������, �� ������� �� ������ � ���� ����
                        Int32 index =
                            shortestGraphPath[graphPathIndex].ConnectionList.IndexOf(
                                shortestGraphPath[graphPathIndex - 1]);
                        shortestGraphPath[graphPathIndex].ConnectionList.Insert(index, currentNode);

                        connIndex++;
                    }
                    // ���� �� 2-�� �� N-1 ���� �� ������������ ���� ������

                    connIndex++;
                }
                // ���� �� ���� ������ �������� ����
            }
            // ���� �� ���� ����� ����� �� ������ ����� �����

            // ���������� ���� ����� ������������
            return graph;
        }

        /// <summary>
        /// ���������� �������������� ���� ������ �� ����� �� ��������� ������ � ����� ����
        /// </summary>
        /// <param name="startNode">�������� ����</param>
        /// <param name="startConn">����� - ������ ����</param>
        /// <param name="finishConn">����� - ����� ����</param>
        /// <returns>������������� ���� ������ �� ����� �� ��������� ������ � ����� ����</returns>
        private List<Polyhedron3DGraphNode> GetShortestGraphPath(Polyhedron3DGraphNode startNode,
                                                                 Polyhedron3DGraphNode startConn,
                                                                 Polyhedron3DGraphNode finishConn)
        {
            List<Polyhedron3DGraphNode> shortestGraphPath = new List<Polyhedron3DGraphNode>();

            // ��������������� ����
            Polyhedron3DGraphNode currentNode = startConn;
            // ����, �� �������� �� ������ � ���������������
            Polyhedron3DGraphNode prevNode = startNode;

            // ���� ��������������� ���� �� �������� � �������� �����
            while (currentNode != startNode)
            {
                // ��������� ��������������� ���� � ����� ������ �����, ������������ ���� ������
                shortestGraphPath.Add(currentNode);

                // ������ ����� (����) �� ������� �� ������ � ��������������� ����
                Int32 connFromIndex = currentNode.ConnectionList.IndexOf(prevNode);
                // ������ ����� (����) ���������� ��� ���, �� ������� �� ������ � ��������������� ����
                Int32 connToIndex = currentNode.ConnectionList.GetPrevItemIndex(connFromIndex);
                prevNode = currentNode;
                currentNode = currentNode.ConnectionList[connToIndex];

                /*// � �������� �����, ��� ���������� ������ ���������������� ����, ����� �����,
                // ������� ����� ���������� (� ������ ������ �������� ���������������� ����) ��� �����,
                // �� ������� �� ������ � ������� ��������������� ����
                Polyhedron3DGraphNode temp = currentNode.ConnectionList.GetPrevItem(prevNode);
                // ����, � ������� ������ ������� ��������������� ���� ���������� (��. ����) ������,
                // ���������� ����� ��������������� �����
                prevNode = currentNode;
                currentNode = temp;*/
            }
            // ���� ��������������� ���� �� �������� � �������� �����

            if (prevNode != finishConn)
            {
#warning ����� ����� ������������������ ����������
                throw new Exception("Error at construction of the graph's path !!!");
            }

            return shortestGraphPath;
        }
    }
}