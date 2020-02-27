using System;
using System.Collections.Generic;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    /// <summary>
    /// граф, сопостовляемый выпуклому 3-мерному многограннику (см. алгоритм построения максимальных стабильных мостов ...)
    /// </summary>
    public class Polyhedron3DGraph
    {
        public Polyhedron3DGraph()
        {
            m_PGNodeList = new List<Polyhedron3DGraphNode>();
        }

        /// <summary>
        /// конструктор класса Polyhedron3DGraph
        /// </summary>
        /// <param name="nodeList">список узлов графа (каждый узел содержит к тому же связи с другими узлами)</param>
        public Polyhedron3DGraph(IEnumerable<Polyhedron3DGraphNode> nodeList)
        {
            m_PGNodeList = new List<Polyhedron3DGraphNode>(nodeList);
        }

        public Polyhedron3DGraph(Polyhedron3DGraph otherGraph)
        {
            m_PGNodeList = new List<Polyhedron3DGraphNode>();
            CopyFrom(this, otherGraph);
        }

        /// <summary>
        /// список узлов графа (каждый узел содержит к тому же связи с другими узлами)
        /// </summary>
        public IList<Polyhedron3DGraphNode> NodeList
        {
            get { return m_PGNodeList; }
        }

        public Polyhedron3DGraph Clone()
        {
            Polyhedron3DGraph graph = new Polyhedron3DGraph();
            CopyFrom(graph, this);
            return graph;
        }

        private static void CopyFrom(Polyhedron3DGraph destGraph, Polyhedron3DGraph sourceGraph)
        {
            destGraph.m_PGNodeList.Clear();
            // nodes
            for (Int32 nodeIndex = 0; nodeIndex < sourceGraph.m_PGNodeList.Count; ++nodeIndex)
            {
                Polyhedron3DGraphNode sourceNode = sourceGraph.m_PGNodeList[nodeIndex];
                Polyhedron3DGraphNode destNode = new Polyhedron3DGraphNode(nodeIndex,
                                                                           sourceNode.GenerationID,
                                                                           sourceNode.NodeNormal)
                                                     {SupportFuncValue = sourceNode.SupportFuncValue};
                destGraph.m_PGNodeList.Add(destNode);
            }
            // connections
            for (Int32 nodeIndex = 0; nodeIndex < sourceGraph.m_PGNodeList.Count; ++nodeIndex)
            {
                Polyhedron3DGraphNode sourceNode = sourceGraph.m_PGNodeList[nodeIndex];
                Polyhedron3DGraphNode destNode = destGraph.m_PGNodeList[nodeIndex];
                foreach (Polyhedron3DGraphNode connection in sourceNode.ConnectionList)
                {
                    Int32 connectionIndex = sourceGraph.m_PGNodeList.IndexOf(connection);
                    if (connectionIndex == -1)
                        throw new AlgorithmException("Exception occur : Не найден узел в списке узлов графа");
                    destNode.ConnectionList.Add(destGraph.m_PGNodeList[connectionIndex]);
                }
            }
        }

        /// <summary>
        /// список узлов графа (каждый узел содержит к тому же связи с другими узлами)
        /// </summary>
        private readonly List<Polyhedron3DGraphNode> m_PGNodeList;
    }
}