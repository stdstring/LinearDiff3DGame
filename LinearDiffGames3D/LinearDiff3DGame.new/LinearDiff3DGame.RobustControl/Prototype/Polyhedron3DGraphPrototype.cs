using System;
using System.Collections.Generic;
using System.Linq;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;

namespace LinearDiff3DGame.RobustControl.Prototype
{
    internal class Polyhedron3DGraphPrototype : IPolyhedron3DGraphPrototype
    {
        public Polyhedron3DGraphPrototype(IPolyhedron3DGraph graph)
        {
            NodeList = new List<IPolyhedron3DGraphPrototypeNode>();
            CopyFrom(graph, this);
        }

        public IList<IPolyhedron3DGraphPrototypeNode> NodeList { get; private set; }

        private static void CopyFrom(IPolyhedron3DGraph graph, IPolyhedron3DGraphPrototype prototype)
        {
            prototype.NodeList.Clear();
            // nodes
            foreach(IPolyhedron3DGraphNode node in graph.NodeList)
            {
                Pair<Double> supportFuncValus = new Pair<Double>(node.SupportFuncValue, 0);
                prototype.NodeList.Add(new Polyhedron3DGraphPrototypeNode(node.ID, node.NodeNormal)
                                           {SupportFuncValues = supportFuncValus});
            }
            // connections
            for(Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
            {
                IPolyhedron3DGraphNode source = graph.NodeList[nodeIndex];
                IPolyhedron3DGraphPrototypeNode dest = prototype.NodeList[nodeIndex];
                foreach(IPolyhedron3DGraphNode conn in source.ConnectionList)
                {
                    IPolyhedron3DGraphNode sourceConn = conn;
                    IPolyhedron3DGraphPrototypeNode destConn = prototype.NodeList.First(item => item.ID == sourceConn.ID);
                    dest.ConnectionList.Add(destConn);
                }
            }
        }
    }
}