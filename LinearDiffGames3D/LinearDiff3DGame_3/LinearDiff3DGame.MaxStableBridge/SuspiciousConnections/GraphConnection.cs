using System;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.Geometry3D;

namespace LinearDiff3DGame.MaxStableBridge
{
    /// <summary>
    /// ��������������� ���������, �������������� ����� ����� ����� ������ �����
    /// ��� ���������� ������ ��������� ���� ����������� ���, ����� ID 1-�� ���� ��� ������ ID 2-�� ����
    /// </summary>
    internal struct GraphConnection
    {
        /// <summary>
        /// ����������� ��������� GraphConnection
        /// </summary>
        /// <param name="node1">���� 1 �����</param>
        /// <param name="node2">���� 2 �����</param>
        public GraphConnection(Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2)
        {
#warning ����� �� �����������, ��� ���� node1 � node2 �� ����� ���� �������� �����

            if (Object.ReferenceEquals(node1, node2))
            {
#warning ����� ����� ������������������ ����������
                throw new Exception("node1 and node2 must be different !!!");
            }

            Node1 = (node1.ID < node2.ID ? node1 : node2);
            Node2 = (node2.ID < node1.ID ? node1 : node2);
        }

        /// <summary>
        /// ���� 1 �����
        /// </summary>
        public readonly Polyhedron3DGraphNode Node1;
        /// <summary>
        /// ���� 2 �����
        /// </summary>
        public readonly Polyhedron3DGraphNode Node2;
    }
}
