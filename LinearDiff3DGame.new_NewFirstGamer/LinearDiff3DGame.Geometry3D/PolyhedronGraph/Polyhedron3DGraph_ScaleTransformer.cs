using System;
using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    public class Polyhedron3DGraph_ScaleTransformer
    {
        // меняем сам исходный граф; копию не делаем
        public Polyhedron3DGraph Process(Polyhedron3DGraph graph, Matrix directTransformation, Matrix reverseTransformation)
        {
            Matrix normalTransformation = Matrix.MatrixTransposing(reverseTransformation);
            foreach (Polyhedron3DGraphNode node in graph.NodeList)
            {
                ProcessNode(node, directTransformation, normalTransformation);
            }
            return graph;
        }

        // меняем узел oldNode; копию не делаем
        // ReSharper disable UnusedMethodReturnValue
        private static Polyhedron3DGraphNode ProcessNode(Polyhedron3DGraphNode oldNode, Matrix transformation, Matrix normalTransformation)
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

        //private Polyhedron3DGraphNode ProcessNode(Polyhedron3DGraphNode oldNode, Matrix transformation)
        //{
        //    Vector3D[] x1x2x3Vectors = TransformX0X1X2(CalcX0X1X2Vectors(oldNode), transformation);
        //    Matrix matrixA = new Matrix(3, 3);
        //    matrixA[1, 1] = x1x2x3Vectors[0].XCoord;
        //    matrixA[1, 2] = x1x2x3Vectors[0].YCoord;
        //    matrixA[1, 3] = x1x2x3Vectors[0].ZCoord;
        //    matrixA[2, 1] = x1x2x3Vectors[1].XCoord;
        //    matrixA[2, 2] = x1x2x3Vectors[1].YCoord;
        //    matrixA[2, 3] = x1x2x3Vectors[1].ZCoord;
        //    matrixA[3, 1] = x1x2x3Vectors[2].XCoord;
        //    matrixA[3, 2] = x1x2x3Vectors[2].YCoord;
        //    matrixA[3, 3] = x1x2x3Vectors[2].ZCoord;
        //    Matrix matrixB = new Matrix(3, 1);
        //    matrixB[1, 1] = 1;
        //    matrixB[2, 1] = 1;
        //    matrixB[3, 1] = 1;
        //    Matrix errorMatrix = new Matrix(3, 1);
        //    Matrix solution = solver.Solve(matrixA, matrixB, out errorMatrix);
        //    Double length =
        //        Math.Sqrt(solution[1, 1]*solution[1, 1] + solution[2, 1]*solution[2, 1] + solution[3, 1]*solution[3, 1]);
        //    Vector3D newNormal = new Vector3D(solution[1, 1]/length, solution[2, 1]/length, solution[3, 1]/length);
        //    // Double d = node.SupportFuncValue; - если SupportFuncValue < 0, то что ???
        //    Double newSupportFuncValue = Math.Sign(oldNode.SupportFuncValue)*(1/length);
        //    oldNode.NodeNormal = newNormal;
        //    oldNode.SupportFuncValue = newSupportFuncValue;
        //    return oldNode;
        //}

        //private Vector3D[] CalcX0X1X2Vectors(Polyhedron3DGraphNode node)
        //{
        //    Double a = node.NodeNormal.XCoord;
        //    Double b = node.NodeNormal.YCoord;
        //    Double c = node.NodeNormal.ZCoord;
        //    Vector3D x0 = node.NodeNormal*node.SupportFuncValue;
        //    Pair<Vector3D, Vector3D> a1a2Vectors = CalcA1A2Pair(a, b, c);
        //    Vector3D[] vectorsX0X1X2 = new[] {x0, x0 + a1a2Vectors.Item1, x0 + a1a2Vectors.Item2};
        //    return vectorsX0X1X2;
        //}

        //private Pair<Vector3D, Vector3D> CalcA1A2Pair(Double a, Double b, Double c)
        //{
        //    if (approxComp.NE(a, 0) && approxComp.EQ(b, 0) && approxComp.EQ(c, 0))
        //        return new Pair<Vector3D, Vector3D>(new Vector3D(0, 1, 0), new Vector3D(0, 0, 1));
        //    if (approxComp.EQ(a, 0) && approxComp.NE(b, 0) && approxComp.EQ(c, 0))
        //        return new Pair<Vector3D, Vector3D>(new Vector3D(1, 0, 0), new Vector3D(0, 0, 1));
        //    if (approxComp.EQ(a, 0) && approxComp.EQ(b, 0) && approxComp.NE(c, 0))
        //        return new Pair<Vector3D, Vector3D>(new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
        //    if (approxComp.NE(a, 0) && approxComp.NE(b, 0) && approxComp.EQ(c, 0))
        //        return new Pair<Vector3D, Vector3D>(new Vector3D(-b, a, 0), new Vector3D(0, 0, 1));
        //    if (approxComp.NE(a, 0) && approxComp.EQ(b, 0) && approxComp.NE(c, 0))
        //        return new Pair<Vector3D, Vector3D>(new Vector3D(-c, 0, a), new Vector3D(0, 1, 0));
        //    if (approxComp.EQ(a, 0) && approxComp.NE(b, 0) && approxComp.NE(c, 0))
        //        return new Pair<Vector3D, Vector3D>(new Vector3D(0, -c, b), new Vector3D(1, 0, 0));
        //    Vector3D a1 = new Vector3D(-b, a, 0);
        //    a1.Normalize();
        //    Vector3D a2 = new Vector3D(-c, 0, a);
        //    a2.Normalize();
        //    Vector3D a3 = new Vector3D(0, -c, b);
        //    a3.Normalize();
        //    if (a >= b && a >= c)
        //        return new Pair<Vector3D, Vector3D>(a1, a2);
        //    if (b >= a && b >= c)
        //        return new Pair<Vector3D, Vector3D>(a1, a3);
        //    return new Pair<Vector3D, Vector3D>(a2, a3);
        //}

        //private static Vector3D[] TransformX0X1X2(Vector3D[] oldX0X1X2, Matrix transformation)
        //{
        //    return new[]
        //               {
        //                   TransformVector(oldX0X1X2[0], transformation),
        //                   TransformVector(oldX0X1X2[1], transformation),
        //                   TransformVector(oldX0X1X2[2], transformation)
        //               };
        //}

        //private static Vector3D TransformVector(Vector3D source, Matrix transformation)
        //{
        //    if (transformation.RowCount != 3 && transformation.ColumnCount != 3)
        //        throw new ArgumentException("Incorrect transformation size");
        //    return Matrix2Vector(transformation*Vector2Matrix(source));
        //}

        //private static Matrix Vector2Matrix(Vector3D source)
        //{
        //    Matrix dest = new Matrix(3, 1);
        //    dest[1, 1] = source.XCoord;
        //    dest[2, 1] = source.YCoord;
        //    dest[3, 1] = source.ZCoord;
        //    return dest;
        //}

        //private static Vector3D Matrix2Vector(Matrix source)
        //{
        //    if (source.RowCount != 3 && source.ColumnCount != 1)
        //        throw new ArgumentException("Matrix is not a 3D vector");
        //    return new Vector3D(source[1, 1], source[2, 1], source[3, 1]);
        //}
    }
}