using System;
using System.Collections.Generic;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    // граф, сопостовл€емый выпуклому 3-мерному многограннику (см. алгоритм построени€ максимальных стабильных мостов ...)
    public class Polyhedron3DGraph : IPolyhedron3DGraph
    {
        public Polyhedron3DGraph()
            : this(new List<IPolyhedron3DGraphNode>())
        {
        }

        public Polyhedron3DGraph(IEnumerable<IPolyhedron3DGraphNode> nodeList)
        {
            NodeList = new List<IPolyhedron3DGraphNode>(nodeList);
        }

        public Polyhedron3DGraph(IPolyhedron3DGraph otherGraph)
        {
            NodeList = new List<IPolyhedron3DGraphNode>();
            CopyFrom(this, otherGraph);
        }

        // список узлов графа (каждый узел содержит к тому же св€зи с другими узлами)
        public IList<IPolyhedron3DGraphNode> NodeList { get; private set; }

        public IPolyhedron3DGraph Clone()
        {
            IPolyhedron3DGraph graph = new Polyhedron3DGraph();
            CopyFrom(graph, this);
            return graph;
        }

        private static void CopyFrom(IPolyhedron3DGraph destGraph, IPolyhedron3DGraph sourceGraph)
        {
            destGraph.NodeList.Clear();
            // nodes
            for(Int32 nodeIndex = 0; nodeIndex < sourceGraph.NodeList.Count; ++nodeIndex)
            {
                IPolyhedron3DGraphNode sourceNode = sourceGraph.NodeList[nodeIndex];
                IPolyhedron3DGraphNode destNode = new Polyhedron3DGraphNode(nodeIndex,
                                                                            sourceNode.GenerationID,
                                                                            sourceNode.NodeNormal)
                                                      {SupportFuncValue = sourceNode.SupportFuncValue};
                destGraph.NodeList.Add(destNode);
            }
            // connections
            for(Int32 nodeIndex = 0; nodeIndex < sourceGraph.NodeList.Count; ++nodeIndex)
            {
                IPolyhedron3DGraphNode sourceNode = sourceGraph.NodeList[nodeIndex];
                IPolyhedron3DGraphNode destNode = destGraph.NodeList[nodeIndex];
                foreach(IPolyhedron3DGraphNode connection in sourceNode.ConnectionList)
                {
                    Int32 connectionIndex = sourceGraph.NodeList.IndexOf(connection);
                    if(connectionIndex == -1)
                        throw new AlgorithmException("Exception occur : Ќе найден узел в списке узлов графа");
                    destNode.ConnectionList.Add(destGraph.NodeList[connectionIndex]);
                }
            }
        }
    }
}