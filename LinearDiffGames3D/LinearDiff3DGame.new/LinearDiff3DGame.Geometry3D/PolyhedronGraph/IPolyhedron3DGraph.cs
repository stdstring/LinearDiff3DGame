using System.Collections.Generic;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    // граф, сопостовляемый выпуклому 3-мерному многограннику
    public interface IPolyhedron3DGraph
    {
        // список узлов графа (каждый узел содержит к тому же связи с другими узлами)
        IList<IPolyhedron3DGraphNode> NodeList { get; }
    }
}