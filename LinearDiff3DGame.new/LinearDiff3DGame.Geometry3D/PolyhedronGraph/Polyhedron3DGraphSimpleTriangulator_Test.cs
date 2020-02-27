using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Geometry3D.PolyhedronFactory;
using NUnit.Framework;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    [TestFixture]
    public class Polyhedron3DGraphSimpleTriangulator_Test
    {
        public Polyhedron3DGraphSimpleTriangulator_Test()
        {
            approxComparer = new ApproxComp(epsilon);
        }

        [Test]
        public void PolyhedronCubeTest()
        {
            // многогранник - куб
            Point3D[] vertexes = new[]
                                     {
                                         new Point3D(1.0, 1.0, 1.0),
                                         new Point3D(-1.0, 1.0, 1.0),
                                         new Point3D(-1.0, -1.0, 1.0),
                                         new Point3D(1.0, -1.0, 1.0),
                                         new Point3D(1.0, 1.0, -1.0),
                                         new Point3D(-1.0, 1.0, -1.0),
                                         new Point3D(-1.0, -1.0, -1.0),
                                         new Point3D(1.0, -1.0, -1.0)
                                     };

            Polyhedron3DFromPointsFactory cubeFactory = new Polyhedron3DFromPointsFactory(approxComparer);
            IPolyhedron3D cube = cubeFactory.CreatePolyhedron(vertexes);

            Polyhedron3DGraphFactory cubeGraphFactory = new Polyhedron3DGraphFactory();
            IPolyhedron3DGraph cubeGraph = cubeGraphFactory.CreatePolyhedronGraph(cube);

            Polyhedron3DGraphSimpleTriangulator triangulator = new Polyhedron3DGraphSimpleTriangulator();
            cubeGraph = triangulator.Triangulate(cubeGraph);

            IList<IPolyhedronSide3D> sideList = cube.SideList;
            IList<IPolyhedron3DGraphNode> nodeList = cubeGraph.NodeList;

            Assert.IsTrue(sideList.Count == nodeList.Count, "must sideList.Count == nodeList.Count");

            for(Int32 sideIndex = 0; sideIndex < sideList.Count; ++sideIndex)
            {
                Double angleValue = Vector3DUtils.AngleBetweenVectors(sideList[sideIndex].SideNormal,
                                                                      nodeList[sideIndex].NodeNormal);
                Assert.IsTrue(approxComparer.EQ(angleValue, 0), "side != graph node");
                Assert.IsTrue(sideList[sideIndex].ID == nodeList[sideIndex].ID, "side's ID != graph node's ID");
            }

            // узел 1
            Assert.IsTrue(nodeList[0].ConnectionList.Count == 4, "incorrect connection list for node with ID = 0");

            Assert.IsTrue(nodeList[0].ConnectionList[0].ID == 1, "incorrect connection list for node with ID = 0");
            Assert.IsTrue(nodeList[0].ConnectionList[1].ID == 2, "incorrect connection list for node with ID = 0");
            Assert.IsTrue(nodeList[0].ConnectionList[2].ID == 3, "incorrect connection list for node with ID = 0");
            Assert.IsTrue(nodeList[0].ConnectionList[3].ID == 4, "incorrect connection list for node with ID = 0");

            // узел 2
            Assert.IsTrue(nodeList[1].ConnectionList.Count == 4, "incorrect connection list for node with ID = 1");

            Assert.IsTrue(nodeList[1].ConnectionList[0].ID == 4, "incorrect connection list for node with ID = 1");
            Assert.IsTrue(nodeList[1].ConnectionList[1].ID == 5, "incorrect connection list for node with ID = 1");
            Assert.IsTrue(nodeList[1].ConnectionList[2].ID == 2, "incorrect connection list for node with ID = 1");
            Assert.IsTrue(nodeList[1].ConnectionList[3].ID == 0, "incorrect connection list for node with ID = 1");

            // узел 3
            Assert.IsTrue(nodeList[2].ConnectionList.Count == 4, "incorrect connection list for node with ID = 2");

            Assert.IsTrue(nodeList[2].ConnectionList[0].ID == 1, "incorrect connection list for node with ID = 2");
            Assert.IsTrue(nodeList[2].ConnectionList[1].ID == 5, "incorrect connection list for node with ID = 2");
            Assert.IsTrue(nodeList[2].ConnectionList[2].ID == 3, "incorrect connection list for node with ID = 2");
            Assert.IsTrue(nodeList[2].ConnectionList[3].ID == 0, "incorrect connection list for node with ID = 2");

            // узел 4
            Assert.IsTrue(nodeList[3].ConnectionList.Count == 4, "incorrect connection list for node with ID = 3");

            Assert.IsTrue(nodeList[3].ConnectionList[0].ID == 2, "incorrect connection list for node with ID = 3");
            Assert.IsTrue(nodeList[3].ConnectionList[1].ID == 5, "incorrect connection list for node with ID = 3");
            Assert.IsTrue(nodeList[3].ConnectionList[2].ID == 4, "incorrect connection list for node with ID = 3");
            Assert.IsTrue(nodeList[3].ConnectionList[3].ID == 0, "incorrect connection list for node with ID = 3");

            // узел 5
            Assert.IsTrue(nodeList[4].ConnectionList.Count == 4, "incorrect connection list for node with ID = 4");

            Assert.IsTrue(nodeList[4].ConnectionList[0].ID == 3, "incorrect connection list for node with ID = 4");
            Assert.IsTrue(nodeList[4].ConnectionList[1].ID == 5, "incorrect connection list for node with ID = 4");
            Assert.IsTrue(nodeList[4].ConnectionList[2].ID == 1, "incorrect connection list for node with ID = 4");
            Assert.IsTrue(nodeList[4].ConnectionList[3].ID == 0, "incorrect connection list for node with ID = 4");

            // узел 6
            Assert.IsTrue(nodeList[5].ConnectionList.Count == 4, "incorrect connection list for node with ID = 5");

            Assert.IsTrue(nodeList[5].ConnectionList[0].ID == 4, "incorrect connection list for node with ID = 5");
            Assert.IsTrue(nodeList[5].ConnectionList[1].ID == 3, "incorrect connection list for node with ID = 5");
            Assert.IsTrue(nodeList[5].ConnectionList[2].ID == 2, "incorrect connection list for node with ID = 5");
            Assert.IsTrue(nodeList[5].ConnectionList[3].ID == 1, "incorrect connection list for node with ID = 5");
        }

        [Test]
        public void PolyhedronPyramidTest()
        {
            // многогранник - пирамида, с квадратом в основании
            Point3D[] vertexes = new[]
                                     {
                                         new Point3D(1.0, 1.0, 0.0),
                                         new Point3D(-1.0, 1.0, 0.0),
                                         new Point3D(-1.0, -1.0, 0.0),
                                         new Point3D(1.0, -1.0, 0.0),
                                         new Point3D(0.0, 0.0, 1.0)
                                     };

            Polyhedron3DFromPointsFactory pyramidFactory = new Polyhedron3DFromPointsFactory(approxComparer);
            IPolyhedron3D pyramid = pyramidFactory.CreatePolyhedron(vertexes);

            Polyhedron3DGraphFactory pyramidGraphFactory = new Polyhedron3DGraphFactory();
            IPolyhedron3DGraph pyramidGraph = pyramidGraphFactory.CreatePolyhedronGraph(pyramid);

            Polyhedron3DGraphSimpleTriangulator triangulator = new Polyhedron3DGraphSimpleTriangulator();
            pyramidGraph = triangulator.Triangulate(pyramidGraph);

            IList<IPolyhedronSide3D> sideList = pyramid.SideList;
            IList<IPolyhedron3DGraphNode> nodeList = pyramidGraph.NodeList;

            Assert.IsTrue(sideList.Count == nodeList.Count, "must sideList.Count == nodeList.Count");

            for(Int32 sideIndex = 0; sideIndex < sideList.Count; ++sideIndex)
            {
                Double angleValue = Vector3DUtils.AngleBetweenVectors(sideList[sideIndex].SideNormal,
                                                                      nodeList[sideIndex].NodeNormal);
                Assert.IsTrue(approxComparer.EQ(angleValue, 0), "side != graph node");
                Assert.IsTrue(sideList[sideIndex].ID == nodeList[sideIndex].ID, "side's ID != graph node's ID");
            }

            // узел 1
            Assert.IsTrue(nodeList[0].ConnectionList.Count == 4, "incorrect connection list for node with ID = 0");

            Assert.IsTrue(nodeList[0].ConnectionList[0].ID == 1, "incorrect connection list for node with ID = 0");
            Assert.IsTrue(nodeList[0].ConnectionList[1].ID == 2, "incorrect connection list for node with ID = 0");
            Assert.IsTrue(nodeList[0].ConnectionList[2].ID == 3, "incorrect connection list for node with ID = 0");
            Assert.IsTrue(nodeList[0].ConnectionList[3].ID == 4, "incorrect connection list for node with ID = 0");

            // узел 2
            Assert.IsTrue(nodeList[1].ConnectionList.Count == 4, "incorrect connection list for node with ID = 1");

            Assert.IsTrue(nodeList[1].ConnectionList[0].ID == 4, "incorrect connection list for node with ID = 1");
            Assert.IsTrue(nodeList[1].ConnectionList[1].ID == 3, "incorrect connection list for node with ID = 1");
            Assert.IsTrue(nodeList[1].ConnectionList[2].ID == 2, "incorrect connection list for node with ID = 1");
            Assert.IsTrue(nodeList[1].ConnectionList[3].ID == 0, "incorrect connection list for node with ID = 1");

            // узел 3
            Assert.IsTrue(nodeList[2].ConnectionList.Count == 3, "incorrect connection list for node with ID = 2");

            Assert.IsTrue(nodeList[2].ConnectionList[0].ID == 1, "incorrect connection list for node with ID = 2");
            Assert.IsTrue(nodeList[2].ConnectionList[1].ID == 3, "incorrect connection list for node with ID = 2");
            Assert.IsTrue(nodeList[2].ConnectionList[2].ID == 0, "incorrect connection list for node with ID = 2");

            // узел 4
            Assert.IsTrue(nodeList[3].ConnectionList.Count == 4, "incorrect connection list for node with ID = 3");

            Assert.IsTrue(nodeList[3].ConnectionList[0].ID == 2, "incorrect connection list for node with ID = 3");
            Assert.IsTrue(nodeList[3].ConnectionList[1].ID == 1, "incorrect connection list for node with ID = 3");
            Assert.IsTrue(nodeList[3].ConnectionList[2].ID == 4, "incorrect connection list for node with ID = 3");
            Assert.IsTrue(nodeList[3].ConnectionList[3].ID == 0, "incorrect connection list for node with ID = 3");

            // узел 5
            Assert.IsTrue(nodeList[4].ConnectionList.Count == 3, "incorrect connection list for node with ID = 4");

            Assert.IsTrue(nodeList[4].ConnectionList[0].ID == 3, "incorrect connection list for node with ID = 4");
            Assert.IsTrue(nodeList[4].ConnectionList[1].ID == 1, "incorrect connection list for node with ID = 4");
            Assert.IsTrue(nodeList[4].ConnectionList[2].ID == 0, "incorrect connection list for node with ID = 4");
        }

        private const Double epsilon = 1e-9;

        private readonly ApproxComp approxComparer;
    }
}