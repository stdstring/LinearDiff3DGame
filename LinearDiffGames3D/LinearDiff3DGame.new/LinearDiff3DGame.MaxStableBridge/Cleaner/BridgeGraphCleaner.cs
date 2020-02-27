using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.Corrector;
using LinearDiff3DGame.MaxStableBridge.SuspiciousConnections;

namespace LinearDiff3DGame.MaxStableBridge.Cleaner
{
    internal class BridgeGraphCleaner
    {
        public IPolyhedron3DGraph Action(Double minAngle,
                                         IPolyhedron3DGraph graph,
                                         IBridgeGraphCorrector corrector,
                                         Int32 generationID4Clean)
        {
            SuspiciousConnectionSet connSet = new SuspiciousConnectionSet();
            List<IPolyhedron3DGraphNode> nodes4Clearance = new List<IPolyhedron3DGraphNode>();

            for(Int32 nodeIndex = graph.NodeList.Count - 1; nodeIndex >= 0; --nodeIndex)
            {
                IPolyhedron3DGraphNode currentNode = graph.NodeList[nodeIndex];
                if(currentNode.GenerationID != generationID4Clean)
                    continue;

                for(Int32 connIndex = 0; connIndex < currentNode.ConnectionList.Count; ++connIndex)
                {
                    IPolyhedron3DGraphNode currentConn = currentNode.ConnectionList[connIndex];

                    if(Vector3DUtils.AngleBetweenVectors(currentNode.NodeNormal, currentConn.NodeNormal) < minAngle &&
                       currentConn.GenerationID != currentNode.GenerationID)
                    {
                        nodes4Clearance.Add(currentNode);
                        break;
                    }
                }
            }

            for(Int32 nodeIndex = 0; nodeIndex < nodes4Clearance.Count; ++nodeIndex)
            {
                corrector.RemoveNode(graph, nodes4Clearance[nodeIndex], connSet);
            }
            corrector.CheckAndCorrectBridgeGraph(connSet, graph);

            return graph;
        }
    }
}