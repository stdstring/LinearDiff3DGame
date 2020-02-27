using System;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.Geometry3D;

namespace LinearDiff3DGame.MaxStableBridge
{
    /// <summary>
    /// ��������������� �����; ������������ ������ ������ �, �� ������� �������������� ���������� ��������� ���������� ������������ ������� �������
    /// </summary>
    internal class SuspiciousConnectionSet
    {
        /// <summary>
        /// ���������� �����; ��� ���������� ���������, ������������� � ������ RemoveConnections ������ ������
        /// </summary>
        private class RemoveConnectionsPredicate
        {
            /// <summary>
            /// ����������� ������ RemoveConnectionsPredicate
            /// </summary>
            /// <param name="reasonNode">���� �����, ��� ������ �������� �������� ������ ���������</param>
            public RemoveConnectionsPredicate(Polyhedron3DGraphNode reasonNode)
            {
                m_ReasonNode = reasonNode;
            }

            /// <summary>
            /// ����� �������� ����� �� ������������ (��� ����� �������� ���������� � ������������ ����)
            /// </summary>
            /// <param name="obj">����������� �����</param>
            /// <returns>true, ���� ����� ����������; ����� - false</returns>
            public Boolean Match(GraphConnection obj)
            {
                return Object.ReferenceEquals(obj.Node1, m_ReasonNode) || Object.ReferenceEquals(obj.Node2, m_ReasonNode);
            }

            /// <summary>
            /// ���� �����, ��� ������ �������� �������� ������ ���������
            /// </summary>
            private Polyhedron3DGraphNode m_ReasonNode;
        }

        /// <summary>
        /// ����������� ������ SuspiciousConnectionSet
        /// </summary>
        public SuspiciousConnectionSet()
        {
            m_SuspiciousConnectionSet = new List<GraphConnection>();
        }

        /// <summary>
        /// ���������� ������ � ������ "��������������" ������
        /// </summary>
        public Int32 Count
        {
            get
            {
                return m_SuspiciousConnectionSet.Count;
            }
        }

        /// <summary>
        /// ���������� ��� ������� � ������ �� ������ "��������������" ������
        /// </summary>
        /// <param name="index">������ ����� � ������� �������������� ������</param>
        /// <returns>����� (� ���� ������� ����� �����) � ������� �������������� ������</returns>
        public Polyhedron3DGraphNode[] this[Int32 index]
        {
            get
            {
                Polyhedron3DGraphNode[] connNodes = new Polyhedron3DGraphNode[2];

                GraphConnection currentConn = m_SuspiciousConnectionSet[index];
                connNodes[0] = currentConn.Node1;
                connNodes[1] = currentConn.Node2;

                return connNodes;
            }
        }

        /// <summary>
        /// ���������� ����� (�������� ����� ������) � ������ "��������������" ������
        /// </summary>
        /// <param name="node1">���� 1 �����</param>
        /// <param name="node2">���� 2 �����</param>
        public void AddConnection(Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2)
        {
            GraphConnection conn = new GraphConnection(node1, node2);

            if (m_SuspiciousConnectionSet.IndexOf(conn) == -1)
            {
                m_SuspiciousConnectionSet.Add(conn);
            }
        }

        /// <summary>
        /// �������� ����� �� ������ "��������������" ������
        /// </summary>
        /// <param name="index">������ ��������� �����</param>
        public void RemoveConnection(Int32 index)
        {
            m_SuspiciousConnectionSet.RemoveAt(index);
        }

        /// <summary>
        /// �������� ����� (�������� ����� ������) �� ������ "��������������" ������
        /// </summary>
        /// <param name="node1">���� 1 �����</param>
        /// <param name="node2">���� 2 �����</param>
        public void RemoveConnection(Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2)
        {
            GraphConnection conn = new GraphConnection(node1, node2);
            m_SuspiciousConnectionSet.Remove(conn);
        }

        /// <summary>
        /// �������� ���� ������ �� ������ "��������������" ������, ������� �������� ���� node
        /// </summary>
        /// <param name="node">����, ��� �������� ���������� �������� ���� ������ �� ������ "��������������" ������</param>
        public void RemoveConnections(Polyhedron3DGraphNode node)
        {
            RemoveConnectionsPredicate predicate = new RemoveConnectionsPredicate(node);
            m_SuspiciousConnectionSet.RemoveAll(predicate.Match);
        }

        /// <summary>
        /// ������ "��������������" ������
        /// </summary>
        private List<GraphConnection> m_SuspiciousConnectionSet;
    }
}
