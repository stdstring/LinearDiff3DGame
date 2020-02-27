using System;
using System.Diagnostics;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;

namespace LinearDiff3DGame.MaxStableBridge.SuspiciousConnections
{
    // ��������������� ���������, �������������� ����� ����� ����� ������ �����
    // ��� ���������� ������ ��������� ���� ����������� ���, ����� ID 1-�� ���� ��� ������ ID 2-�� ����
    internal struct GraphConnection
    {
        public GraphConnection(IPolyhedron3DGraphNode node1, IPolyhedron3DGraphNode node2)
        {
            Debug.Assert(node1.ConnectionList.Contains(node2));
            Debug.Assert(node2.ConnectionList.Contains(node1));
            if (ReferenceEquals(node1, node2))
            {
                throw new ArgumentException("node1 and node2 must be different");
            }

            Node1 = (node1.ID < node2.ID ? node1 : node2);
            Node2 = (node2.ID < node1.ID ? node1 : node2);
        }

        // ���� 1 �����
        public readonly IPolyhedron3DGraphNode Node1;

        // ���� 2 �����
        public readonly IPolyhedron3DGraphNode Node2;
    }
}