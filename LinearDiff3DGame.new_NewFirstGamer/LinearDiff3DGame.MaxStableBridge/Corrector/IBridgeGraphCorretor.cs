using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.SuspiciousConnections;

namespace LinearDiff3DGame.MaxStableBridge.Corrector
{
    public interface IBridgeGraphCorrector
    {
        Polyhedron3DGraph CheckAndCorrectBridgeGraph(SuspiciousConnectionSet connSet, Polyhedron3DGraph graph);
        void RemoveNode(Polyhedron3DGraph graph, Polyhedron3DGraphNode removedNode, SuspiciousConnectionSet connSet);
    }
}