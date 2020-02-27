using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    // ���� �����, ��������������� 3-������ ������������
    // ���� ���� 1 � 2 �������� ��������, �� ������ �� ���� 2 ����� ��������� � ������ ������ ���� 1 � ������ �� ���� 1 - � ������ ������ ���� 2
    public class Polyhedron3DGraphNode : IPolyhedron3DGraphNode
    {
        public Polyhedron3DGraphNode(Int32 nodeID, Int32 generationID, Vector3D nodeNormal)
            : this(nodeID, generationID, nodeNormal, new List<IPolyhedron3DGraphNode>())
        {
        }


        public Polyhedron3DGraphNode(Int32 nodeID,
                                     Int32 generationID,
                                     Vector3D nodeNormal,
                                     IEnumerable<IPolyhedron3DGraphNode> nodeConnectionList)
        {
            ID = nodeID;
            GenerationID = generationID;
            NodeNormal = nodeNormal;
            connectionList = new List<IPolyhedron3DGraphNode>(nodeConnectionList);
        }

        // ������ ������ ������� ����
        public IList<IPolyhedron3DGraphNode> ConnectionList
        {
            get { return connectionList; }
        }

        // ID ����
        public Int32 ID { get; private set; }

        // ID ���������
        public Int32 GenerationID { get; private set; }

        // "�������" ������� � �����, ������� ������������� ������� ���� �����
        public Vector3D NodeNormal { get; set; }

        // �������� ������� ������� ��� ����
        public Double SupportFuncValue { get; set; }

        private readonly List<IPolyhedron3DGraphNode> connectionList;
    }
}