using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;

namespace LinearDiff3DGame.MaxStableBridge.SuspiciousConnections
{
    // ������ ������ �, �� ������� �������������� ���������� ��������� ���������� ������������ ������� �������
    internal class SuspiciousConnectionSet
    {
        public SuspiciousConnectionSet()
        {
            suspiciousConnectionSet = new List<GraphConnection>();
        }

        public Int32 Count
        {
            get { return suspiciousConnectionSet.Count; }
        }

        // ���������� ��� ������� � ������ �� ������ "��������������" ������; ���������� ����� (� ���� ������� ����� �����) � ������� �������������� ������
        public IPolyhedron3DGraphNode[] this[Int32 index]
        {
            get
            {
                IPolyhedron3DGraphNode[] connNodes = new IPolyhedron3DGraphNode[2];

                GraphConnection currentConn = suspiciousConnectionSet[index];
                connNodes[0] = currentConn.Node1;
                connNodes[1] = currentConn.Node2;

                return connNodes;
            }
        }

        // ���������� ����� (�������� ����� ������) � ������ "��������������" ������
        public void AddConnection(IPolyhedron3DGraphNode node1, IPolyhedron3DGraphNode node2)
        {
            GraphConnection conn = new GraphConnection(node1, node2);
            if (suspiciousConnectionSet.IndexOf(conn) == -1)
                suspiciousConnectionSet.Add(conn);
        }

        // �������� ����� �� ������ "��������������" ������
        public void RemoveConnection(Int32 index)
        {
            suspiciousConnectionSet.RemoveAt(index);
        }

        // �������� ����� (�������� ����� ������) �� ������ "��������������" ������
        public void RemoveConnection(IPolyhedron3DGraphNode node1, IPolyhedron3DGraphNode node2)
        {
            GraphConnection conn = new GraphConnection(node1, node2);
            suspiciousConnectionSet.Remove(conn);
        }

        // �������� ���� ������ �� ������ "��������������" ������, ������� �������� ���� node
        public void RemoveConnections(IPolyhedron3DGraphNode node)
        {
            suspiciousConnectionSet.RemoveAll(
                conn => ReferenceEquals(conn.Node1, node) || ReferenceEquals(conn.Node2, node));
        }

        // ������ "��������������" ������
        private readonly List<GraphConnection> suspiciousConnectionSet;
    }
}