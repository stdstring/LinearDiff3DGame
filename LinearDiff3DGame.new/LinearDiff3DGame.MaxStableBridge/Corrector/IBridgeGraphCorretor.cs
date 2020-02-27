using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.SuspiciousConnections;

namespace LinearDiff3DGame.MaxStableBridge.Corrector
{
    internal interface IBridgeGraphCorrector
    {
        IPolyhedron3DGraph CheckAndCorrectBridgeGraph(SuspiciousConnectionSet connSet, IPolyhedron3DGraph graph);
        void RemoveNode(IPolyhedron3DGraph graph, IPolyhedron3DGraphNode removedNode, SuspiciousConnectionSet connSet);
    }
}