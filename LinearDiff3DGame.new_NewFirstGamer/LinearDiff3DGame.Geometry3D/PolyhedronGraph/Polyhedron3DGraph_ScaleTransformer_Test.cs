using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.LinearEquationsSet;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Geometry3D.PolyhedronFactory;
using NUnit.Framework;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    [TestFixture]
    public class Polyhedron3DGraph_ScaleTransformer_Test
    {
        [Test]
        public void TransformCube()
        {
            Polyhedron3D cube = CreatePolyhedron(new Point3D(1, 1, 1),
                                                 new Point3D(1, -1, 1),
                                                 new Point3D(-1, 1, 1),
                                                 new Point3D(-1, -1, 1),
                                                 new Point3D(1, 1, -1),
                                                 new Point3D(1, -1, -1),
                                                 new Point3D(-1, 1, -1),
                                                 new Point3D(-1, -1, -1));
            Vector3D direction = Vector3DUtils.NormalizeVector(new Vector3D(2, 1, 3));
            TestScaleTransformer(cube, direction, 2.5);
            Vector3D direction2 = new Vector3D(0, 0, 1);
            TestScaleTransformer(cube, direction2, 2.5);
        }

        [Test]
        public void TransformPyramid()
        {
            Polyhedron3D pyramid = CreatePolyhedron(new Point3D(1, 1, 1),
                                                    new Point3D(1, 1, 0),
                                                    new Point3D(-1, 1, 0),
                                                    new Point3D(1, -1, 0),
                                                    new Point3D(-1, -1, 0));
            Vector3D direction = Vector3DUtils.NormalizeVector(new Vector3D(2, 1, 3));
            TestScaleTransformer(pyramid, direction, 2.5);
            Vector3D direction2 = new Vector3D(0, 0, 1);
            TestScaleTransformer(pyramid, direction2, 2.5);
        }

        private Polyhedron3D CreatePolyhedron(params Point3D[] vertexes)
        {
            return new Polyhedron3DFromPointsFactory(approxComp).CreatePolyhedron(vertexes);
        }

        private void TestScaleTransformer(Polyhedron3D sourcePolyhedron, Vector3D direction, Double scalingRatio)
        {
            Matrix directTransform = ScalingTransformation3D.GetTransformationMatrix(direction, scalingRatio);
            Matrix reverseTransform = ScalingTransformation3D.GetTransformationMatrix(direction, 1 / scalingRatio);
            // polyhedron 2 graph :
            Polyhedron3DGraph graph = new Polyhedron3DGraphFactory().CreatePolyhedronGraph(sourcePolyhedron);
            new Polyhedron3DGraphSimpleTriangulator().Triangulate(graph);
            // direct transform graph :
            new Polyhedron3DGraph_ScaleTransformer().Process(graph, directTransform, reverseTransform);
            // graph 2 polyhedron
            Polyhedron3D intermediatePolyhedron = new Polyhedron3DFromGraphFactory(approxComp, new LESKramer3Solver()).CreatePolyhedron(graph);
            // reverse transform polyhedron
            Polyhedron3D destPolyhedron = TransformPolyhedron(intermediatePolyhedron, reverseTransform);
            Assert.IsTrue(new Polyhedron3DEqualityChecker(approxComp).Equal(sourcePolyhedron, destPolyhedron));
        }

        private Polyhedron3D TransformPolyhedron(Polyhedron3D polyhedron, Matrix transformation)
        {
            Point3D[] polyhedronVertexes = PolyhedronVertexes(polyhedron);
            List<Point3D> vertexesAfterTransformation = new List<Point3D>();
            foreach(Point3D vertex in polyhedronVertexes)
                vertexesAfterTransformation.Add(Matrix2Point(transformation * Point2Matrix(vertex)));
            return new Polyhedron3DFromPointsFactory(approxComp).CreatePolyhedron(vertexesAfterTransformation.ToArray());
        }

        private static Matrix Point2Matrix(Point3D point)
        {
            Matrix matrix = new Matrix(3, 1);
            matrix[1, 1] = point.XCoord;
            matrix[2, 1] = point.YCoord;
            matrix[3, 1] = point.ZCoord;
            return matrix;
        }

        private static Point3D Matrix2Point(Matrix matrix)
        {
            return new Point3D(matrix[1, 1], matrix[2, 1], matrix[3, 1]);
        }

        private static Point3D[] PolyhedronVertexes(Polyhedron3D polyhedron)
        {
            List<Point3D> points = new List<Point3D>();
            foreach(PolyhedronVertex3D vertex in polyhedron.VertexList)
                points.Add(new Point3D(vertex.XCoord, vertex.YCoord, vertex.ZCoord));
            return points.ToArray();
        }

        private readonly ApproxComp approxComp = new ApproxComp(1e-9);
    }
}