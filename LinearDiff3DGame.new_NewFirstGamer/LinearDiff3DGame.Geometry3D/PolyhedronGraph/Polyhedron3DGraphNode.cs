using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
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
        /// <param name="generationID">ID ���������</param>
        /// <param name="nodeNormal">"�������" ������� � �����, ������� ������������� ������� ���� �����</param>
        public Polyhedron3DGraphNode(Int32 nodeID, Int32 generationID, Vector3D nodeNormal)
        {
            ID = nodeID;
            GenerationID = generationID;
            NodeNormal = nodeNormal;

            m_NodeConnectionList = new List<Polyhedron3DGraphNode>();
        }

        /// <summary>
        /// ����������� ������ Polyhedron3DGraphNode
        /// </summary>
        /// <param name="nodeID">ID ����</param>
        /// <param name="generationID">ID ���������</param>
        /// <param name="nodeNormal">"�������" ������� � �����, ������� ������������� ������� ���� �����</param>
        /// <param name="nodeConnectionList">������ ������ ������� ����</param>
        public Polyhedron3DGraphNode(Int32 nodeID, Int32 generationID, Vector3D nodeNormal,
                                     IList<Polyhedron3DGraphNode> nodeConnectionList)
        {
            ID = nodeID;
            GenerationID = generationID;
            NodeNormal = nodeNormal;

            m_NodeConnectionList = new List<Polyhedron3DGraphNode>();
            for(Int32 connectionIndex = 0; connectionIndex < nodeConnectionList.Count; ++connectionIndex)
                m_NodeConnectionList.Add(nodeConnectionList[connectionIndex]);
        }

        /// <summary>
        /// ������ ������ ������� ����
        /// </summary>
        public IList<Polyhedron3DGraphNode> ConnectionList { get { return m_NodeConnectionList; } }

        /// <summary>
        /// ID - �������� ��� ������� (������/������) � ID ����
        /// </summary>
        public Int32 ID { get; set; }

        public Int32 GenerationID { get; private set; }

        /// <summary>
        /// NodeNormal - �������� ��� ������� (������) � "�������" ������� � �����, ������� ������������� ������� ���� �����
        /// </summary>
        public Vector3D NodeNormal { get; set; }

        /// <summary>
        /// �������� ������� ������� ��� ���� (�������������� ��������)
        /// </summary>
        public Double SupportFuncValue { get; set; }

        /// <summary>
        /// m_NodeConnectionList - ������ ������ ������� ����
        /// </summary>
        private readonly List<Polyhedron3DGraphNode> m_NodeConnectionList;
    }
}