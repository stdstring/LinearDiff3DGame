using System;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.Geometry3D
{
    /// <summary>
    /// ���� �����, ��������������� 3-������ ������������
    /// ����, ����������� �� �������� ����� ����� ��������� �����������: ���� ���� ���� �� ���� 1 � ���� 2, �� �� �����������, ��� ���� ���� �� ���� 2 � ���� 1
    /// </summary>
    public class Polyhedron3DGraphNode
    {
        /// <summary>
        /// ����������� ������ Polyhedron3DGraphNode
        /// </summary>
        /// <param name="nodeID">ID ����</param>
        /// <param name="nodeNormal">"�������" ������� � �����, ������� ������������� ������� ���� �����</param>
        public Polyhedron3DGraphNode(Int32 nodeID, Vector3D nodeNormal)
        {
            m_ID = nodeID;
            m_NodeNormal = nodeNormal;

            m_NodeConnectionList = new CyclicList<Polyhedron3DGraphNode>();
        }

        /// <summary>
        /// ����������� ������ Polyhedron3DGraphNode
        /// </summary>
        /// <param name="nodeID">ID ����</param>
        /// <param name="nodeNormal">"�������" ������� � �����, ������� ������������� ������� ���� �����</param>
        /// <param name="nodeConnectionList">������ ������ ������� ����</param>
        public Polyhedron3DGraphNode(Int32 nodeID, Vector3D nodeNormal, IList<Polyhedron3DGraphNode> nodeConnectionList)
        {
            m_ID = nodeID;
            m_NodeNormal = nodeNormal;

            m_NodeConnectionList = new CyclicList<Polyhedron3DGraphNode>();
            for (Int32 connectionIndex = 0; connectionIndex < nodeConnectionList.Count; ++connectionIndex)
            {
                m_NodeConnectionList.Add(nodeConnectionList[connectionIndex]);
            }
        }

        /// <summary>
        /// ������ ������ ������� ����
        /// </summary>
        public ICyclicList<Polyhedron3DGraphNode> ConnectionList
        {
            get
            {
                return m_NodeConnectionList;
            }
        }

        /// <summary>
        /// ID - �������� ��� ������� (������/������) � ID ����
        /// </summary>
        public Int32 ID
        {
            get
            {
                return m_ID;
            }
            set
            {
                m_ID = value;
            }
        }

        /// <summary>
        /// NodeNormal - �������� ��� ������� (������) � "�������" ������� � �����, ������� ������������� ������� ���� �����
        /// </summary>
        public Vector3D NodeNormal
        {
            get
            {
                return m_NodeNormal;
            }
        }

#warning ����� ��������, �.�. � ������� �������� ������������� ���� ����� ���������� ����������
        /// <summary>
        /// �������� ������� ������� ��� ���� (�������������� ��������)
        /// </summary>
        public Double SupportFuncValue
        {
            get
            {
                return m_SupportFuncValue;
            }
            set
            {
                m_SupportFuncValue = value;
            }
        }

        /// <summary>
        /// m_ID - ���������� ������������� ���� (��������� � ID �����, ������� ������� ���� �������������)
        /// </summary>
        private Int32 m_ID;
        /// <summary>
        /// m_NodeNormal - "�������" ������� � �����, ������� ������������� ������� ���� �����
        /// </summary>
        private Vector3D m_NodeNormal;
        /// <summary>
        /// m_NodeConnectionList - ������ ������ ������� ����
        /// </summary>
        private CyclicList<Polyhedron3DGraphNode> m_NodeConnectionList;

#warning ����� ��������, �.�. � ������� �������� ������������� ���� ����� ���������� ����������
        /// <summary>
        /// �������� ������� ������� ��� ���� (�������������� ��������)
        /// </summary>
        private Double m_SupportFuncValue;
    }
}
