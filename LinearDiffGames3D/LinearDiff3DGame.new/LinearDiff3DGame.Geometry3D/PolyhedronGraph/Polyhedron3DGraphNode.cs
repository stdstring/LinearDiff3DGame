using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    // узел графа, представляющего 3-мерный многогранник
    // если узлы 1 и 2 являются соседями, то ссылку на узел 2 нужно добавлять в список связей узла 1 и ссылку на узел 1 - в список связей узла 2
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

        // список связей данного узла
        public IList<IPolyhedron3DGraphNode> ConnectionList
        {
            get { return connectionList; }
        }

        // ID узла
        public Int32 ID { get; private set; }

        // ID поколения
        public Int32 GenerationID { get; private set; }

        // "внешняя" нормаль к грани, которая соответствует данному узлу графа
        public Vector3D NodeNormal { get; set; }

        // значение опорной функции для узла
        public Double SupportFuncValue { get; set; }

        private readonly List<IPolyhedron3DGraphNode> connectionList;
    }
}