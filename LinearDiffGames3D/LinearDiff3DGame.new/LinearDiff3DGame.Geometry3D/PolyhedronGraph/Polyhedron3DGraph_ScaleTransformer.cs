using System;
using LinearDiff3DGame.AdvMath.Matrix;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    public class Polyhedron3DGraph_ScaleTransformer
    {
        // меняем сам исходный граф; копию не делаем
        public IPolyhedron3DGraph Process(IPolyhedron3DGraph graph, Matrix directTransformation, Matrix reverseTransformation)
        {
            Matrix normalTransformation = Matrix.MatrixTransposing(reverseTransformation);
            foreach (IPolyhedron3DGraphNode node in graph.NodeList)
            {
                ProcessNode(node, directTransformation, normalTransformation);
            }
            return graph;
        }

        // меняем узел oldNode; копию не делаем
        // ReSharper disable UnusedMethodReturnValue
        private static IPolyhedron3DGraphNode ProcessNode(IPolyhedron3DGraphNode oldNode, Matrix transformation, Matrix normalTransformation)
        // ReSharper restore UnusedMethodReturnValue
        {
            Matrix normalMatrix = normalTransformation*Geometry3DObjectFactory.CreateMatrix(oldNode.NodeNormal);
            Vector3D newNormal = Vector3DUtils.NormalizeVector(Geometry3DObjectFactory.CreateVector(normalMatrix));
            Vector3D oldPoint = oldNode.SupportFuncValue*oldNode.NodeNormal;
            Matrix pointMatrix = transformation*Geometry3DObjectFactory.CreateMatrix(oldPoint);
            Vector3D newPoint = Geometry3DObjectFactory.CreateVector(pointMatrix);
            Double newSupportFuncValue = newPoint*newNormal;
            oldNode.NodeNormal = newNormal;
            oldNode.SupportFuncValue = newSupportFuncValue;
            return oldNode;
        }
    }
}