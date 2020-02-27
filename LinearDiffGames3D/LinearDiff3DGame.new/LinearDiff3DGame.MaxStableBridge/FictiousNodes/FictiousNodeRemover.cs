using System;
using System.Collections.Generic;
using System.Diagnostics;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.Corrector;
using LinearDiff3DGame.MaxStableBridge.SuspiciousConnections;

namespace LinearDiff3DGame.MaxStableBridge.FictiousNodes
{
    internal class FictiousNodeRemover
    {
        public IPolyhedron3DGraph Action(IPolyhedron3D polyhedron,
                                         IPolyhedron3DGraph graph,
                                         IBridgeGraphCorrector corrector)
        {
            Debug.Assert(polyhedron.SideList.Count == graph.NodeList.Count,
                         "polyhedron.SideList.Count must be equal graph.NodeList.Count");

            SuspiciousConnectionSet connSet = new SuspiciousConnectionSet();

            //for (Int32 sideIndex = 0; sideIndex < polyhedron.SideList.Count;)
            //{
            //    Debug.Assert(polyhedron.SideList.Count == graph.NodeList.Count, "polyhedron.SideList.Count must be equal graph.NodeList.Count");

            //    PolyhedronSide3D currsentSide = polyhedron.SideList[sideIndex];
            //    if (IsSideFictious(currsentSide))
            //    {
            //        Polyhedron3DGraphNode currentNode = graph.NodeList[sideIndex];
            //        corrector.RemoveNode(graph, currentNode, connSet);
            //        RemoveSide(polyhedron, currsentSide);
            //    }
            //    else
            //    {
            //        ++sideIndex;
            //    }
            //}

            List<IPolyhedron3DGraphNode> nodes4Remove = new List<IPolyhedron3DGraphNode>();
            for(Int32 sideIndex = 0; sideIndex < polyhedron.SideList.Count; ++sideIndex)
            {
                IPolyhedronSide3D currsentSide = polyhedron.SideList[sideIndex];
                if(IsSideFictious(currsentSide))
                {
                    nodes4Remove.Add(graph.NodeList[sideIndex]);
                }
            }

            for(Int32 nodeIndex = 0; nodeIndex < nodes4Remove.Count; ++nodeIndex)
            {
                corrector.RemoveNode(graph, nodes4Remove[nodeIndex], connSet);
            }

            corrector.CheckAndCorrectBridgeGraph(connSet, graph);

            //Debug.Assert(polyhedron.SideList.Count == graph.NodeList.Count, "polyhedron.SideList.Count must be equal graph.NodeList.Count");
            //for (Int32 sideIndex = 0; sideIndex < polyhedron.SideList.Count; ++sideIndex)
            //{
            //    polyhedron.SideList[sideIndex].ID = sideIndex;
            //    graph.NodeList[sideIndex].ID = sideIndex;
            //}
            //for (Int32 vertexIndex = 0; vertexIndex < polyhedron.VertexList.Count; ++vertexIndex)
            //{
            //    polyhedron.VertexList[vertexIndex].ID = vertexIndex;
            //    Debug.Assert(polyhedron.VertexList[vertexIndex].SideList.Count < 3, "Incorrect polyhedron structure"); 
            //}

            return graph;
        }

        private static Boolean IsSideFictious(IPolyhedronSide3D side)
        {
            return (side.VertexList.Count == 2);
        }

        private static void RemoveSide(IPolyhedron3D polyhedron, IPolyhedronSide3D removedSide)
        {
            //for (Int32 vertexIndex = 0; vertexIndex < removedSide.VertexList.Count; ++vertexIndex)
            //{
            //    PolyhedronVertex3D currentVertex = removedSide.VertexList[vertexIndex];

            //    currentVertex.SideList.Remove(removedSide);
            //    if (currentVertex.SideList.Count == 0)
            //    {
            //        polyhedron.VertexList.Remove(currentVertex);
            //    }
            //}

            polyhedron.SideList.Remove(removedSide);
        }
    }
}