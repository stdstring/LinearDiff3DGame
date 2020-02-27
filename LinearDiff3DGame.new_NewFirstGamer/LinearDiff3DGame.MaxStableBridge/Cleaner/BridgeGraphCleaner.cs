using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.Corrector;
using LinearDiff3DGame.MaxStableBridge.SuspiciousConnections;

namespace LinearDiff3DGame.MaxStableBridge.Cleaner
{
    public class BridgeGraphCleaner
    {
        public BridgeGraphCleaner(ApproxComp approxComparer)
        {
            m_ApproxComparer = approxComparer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minAngle"></param>
        /// <param name="graph"></param>
        /// <param name="corrector"></param>
        /// <param name="generationID4Clean"></param>
        /// <returns></returns>
        public Polyhedron3DGraph Action(Double minAngle, Polyhedron3DGraph graph, IBridgeGraphCorrector corrector,
                                        Int32 generationID4Clean)
        {
            SuspiciousConnectionSet connSet = new SuspiciousConnectionSet();
            List<Polyhedron3DGraphNode> nodes4Clearance = new List<Polyhedron3DGraphNode>();

            for (Int32 nodeIndex = graph.NodeList.Count - 1; nodeIndex >= 0; --nodeIndex)
            {
                Polyhedron3DGraphNode currentNode = graph.NodeList[nodeIndex];
                if (currentNode.GenerationID != generationID4Clean)
                {
                    continue;
                }

                for (Int32 connIndex = 0; connIndex < currentNode.ConnectionList.Count; ++connIndex)
                {
                    Polyhedron3DGraphNode currentConn = currentNode.ConnectionList[connIndex];

                    if (Vector3DUtils.AngleBetweenVectors(currentNode.NodeNormal, currentConn.NodeNormal) < minAngle &&
                        currentConn.GenerationID != currentNode.GenerationID)
                    {
                        nodes4Clearance.Add(currentNode);
                        break;
                    }
                }
            }

            for (Int32 nodeIndex = 0; nodeIndex < nodes4Clearance.Count; ++nodeIndex)
            {
                corrector.RemoveNode(graph, nodes4Clearance[nodeIndex], connSet);
            }
            corrector.CheckAndCorrectBridgeGraph(connSet, graph);

            return graph;
        }

        /// <summary>
        /// сравниватель для приближенного сравнения действительных чисел
        /// </summary>
        private readonly ApproxComp m_ApproxComparer;
    }
}