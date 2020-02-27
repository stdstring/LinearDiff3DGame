using System;
using System.Collections.Generic;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    // ������� ������������ ����� ��������� 3-������� �������������
    public class Polyhedron3DGraphSimpleTriangulator
    {
        // �������� ������������ ����� graph, ������������ �� ��������� ��������� 3-� ������� �������������
        // ��� ���� �������������� (� ����� �� ����������� !!!), ��� ��� �������� � ����� - ��������
        public IPolyhedron3DGraph Triangulate(IPolyhedron3DGraph graph)
        {
            IList<IPolyhedron3DGraphNode> nodeList = graph.NodeList;

            // ���� �� ���� ����� ����� �� ������ ����� �����
            for(Int32 nodeIndex = 0; nodeIndex < nodeList.Count; ++nodeIndex)
            {
                IPolyhedron3DGraphNode currentNode = nodeList[nodeIndex];

                // ���� �� ���� ������ �������� ����
                for(Int32 connIndex = 0; connIndex < currentNode.ConnectionList.Count;)
                {
                    // ������� � ��������� ����� (����)
                    IPolyhedron3DGraphNode currentConn = currentNode.ConnectionList[connIndex];
                    IPolyhedron3DGraphNode nextConn = currentNode.ConnectionList.GetNextItem(connIndex);

                    // ������ ������������� ���� ������, ������������ �� ������� ����� � ��������������� �� ��������� �����
                    List<IPolyhedron3DGraphNode> shortestGraphPath = GetShortestGraphPath(currentNode,
                                                                                          currentConn,
                                                                                          nextConn);

                    // ���� ����� ����� � ����������� ���� < 2, �� ��� ������ !!!!!!
                    if(shortestGraphPath.Count < 2)
                    {
#warning ����� ����� ������������������ ����������
                        throw new Exception("Error at construction of the graph's path !!!");
                    }

                    // ���� �� 2-�� �� N-1 ���� �� ������������ ���� ������
                    // ���� ����� ����� � ����������� ���� = 2, �� ���� ���������� 0 ���
                    for(Int32 graphPathIndex = 1; graphPathIndex < shortestGraphPath.Count - 1; graphPathIndex++)
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

        // ���������� �������������� ���� ������ �� ����� �� ��������� ������ � ����� ����
        private static List<IPolyhedron3DGraphNode> GetShortestGraphPath(IPolyhedron3DGraphNode startNode,
                                                                         IPolyhedron3DGraphNode startConn,
                                                                         IPolyhedron3DGraphNode finishConn)
        {
            List<IPolyhedron3DGraphNode> shortestGraphPath = new List<IPolyhedron3DGraphNode>();

            // ��������������� ����
            IPolyhedron3DGraphNode currentNode = startConn;
            // ����, �� �������� �� ������ � ���������������
            IPolyhedron3DGraphNode prevNode = startNode;

            // ���� ��������������� ���� �� �������� � �������� �����
            while(currentNode != startNode)
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

            if(prevNode != finishConn)
            {
#warning ����� ����� ������������������ ����������
                throw new Exception("Error at construction of the graph's path !!!");
            }

            return shortestGraphPath;
        }
    }
}