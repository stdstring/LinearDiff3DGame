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
    public class Polyhedron3DGraphFactory_Test
    {
        public Polyhedron3DGraphFactory_Test()
        {
            m_ApproxComparer = new ApproxComp(Epsilon);
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

            Polyhedron3DFromPointsFactory cubeFactory = new Polyhedron3DFromPointsFactory(m_ApproxComparer);
            Polyhedron3D cube = cubeFactory.CreatePolyhedron(vertexes);

            Polyhedron3DGraphFactory cubeGraphFactory = new Polyhedron3DGraphFactory();
            Polyhedron3DGraph cubeGraph = cubeGraphFactory.CreatePolyhedronGraph(cube);

            IList<PolyhedronSide3D> sideList = cube.SideList;
            IList<Polyhedron3DGraphNode> nodeList = cubeGraph.NodeList;

            Assert.IsTrue(sideList.Count == nodeList.Count, "must sideList.Count == nodeList.Count");

            for (Int32 sideIndex = 0; sideIndex < sideList.Count; ++sideIndex)
            {
                Double angleValue = Vector3DUtils.AngleBetweenVectors(sideList[sideIndex].SideNormal,
                                                                      nodeList[sideIndex].NodeNormal);
                Assert.IsTrue(m_ApproxComparer.EQ(angleValue, 0), "side != graph node");
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

            Polyhedron3DFromPointsFactory pyramidFactory = new Polyhedron3DFromPointsFactory(m_ApproxComparer);
            Polyhedron3D pyramid = pyramidFactory.CreatePolyhedron(vertexes);

            Polyhedron3DGraphFactory pyramidGraphFactory = new Polyhedron3DGraphFactory();
            Polyhedron3DGraph pyramidGraph = pyramidGraphFactory.CreatePolyhedronGraph(pyramid);

            IList<PolyhedronSide3D> sideList = pyramid.SideList;
            IList<Polyhedron3DGraphNode> nodeList = pyramidGraph.NodeList;

            Assert.IsTrue(sideList.Count == nodeList.Count, "must sideList.Count == nodeList.Count");

            for (Int32 sideIndex = 0; sideIndex < sideList.Count; ++sideIndex)
            {
                Double angleValue = Vector3DUtils.AngleBetweenVectors(sideList[sideIndex].SideNormal,
                                                                      nodeList[sideIndex].NodeNormal);
                Assert.IsTrue(m_ApproxComparer.EQ(angleValue, 0), "side != graph node");
                Assert.IsTrue(sideList[sideIndex].ID == nodeList[sideIndex].ID, "side's ID != graph node's ID");
            }

            // узел 1
            Assert.IsTrue(nodeList[0].ConnectionList.Count == 4, "incorrect connection list for node with ID = 0");

            Assert.IsTrue(nodeList[0].ConnectionList[0].ID == 1, "incorrect connection list for node with ID = 0");
            Assert.IsTrue(nodeList[0].ConnectionList[1].ID == 2, "incorrect connection list for node with ID = 0");
            Assert.IsTrue(nodeList[0].ConnectionList[2].ID == 3, "incorrect connection list for node with ID = 0");
            Assert.IsTrue(nodeList[0].ConnectionList[3].ID == 4, "incorrect connection list for node with ID = 0");

            // узел 2
            Assert.IsTrue(nodeList[1].ConnectionList.Count == 3, "incorrect connection list for node with ID = 1");

            Assert.IsTrue(nodeList[1].ConnectionList[0].ID == 4, "incorrect connection list for node with ID = 1");
            Assert.IsTrue(nodeList[1].ConnectionList[1].ID == 2, "incorrect connection list for node with ID = 1");
            Assert.IsTrue(nodeList[1].ConnectionList[2].ID == 0, "incorrect connection list for node with ID = 1");

            // узел 3
            Assert.IsTrue(nodeList[2].ConnectionList.Count == 3, "incorrect connection list for node with ID = 2");

            Assert.IsTrue(nodeList[2].ConnectionList[0].ID == 1, "incorrect connection list for node with ID = 2");
            Assert.IsTrue(nodeList[2].ConnectionList[1].ID == 3, "incorrect connection list for node with ID = 2");
            Assert.IsTrue(nodeList[2].ConnectionList[2].ID == 0, "incorrect connection list for node with ID = 2");

            // узел 4
            Assert.IsTrue(nodeList[3].ConnectionList.Count == 3, "incorrect connection list for node with ID = 3");

            Assert.IsTrue(nodeList[3].ConnectionList[0].ID == 2, "incorrect connection list for node with ID = 3");
            Assert.IsTrue(nodeList[3].ConnectionList[1].ID == 4, "incorrect connection list for node with ID = 3");
            Assert.IsTrue(nodeList[3].ConnectionList[2].ID == 0, "incorrect connection list for node with ID = 3");

            // узел 5
            Assert.IsTrue(nodeList[4].ConnectionList.Count == 3, "incorrect connection list for node with ID = 4");

            Assert.IsTrue(nodeList[4].ConnectionList[0].ID == 3, "incorrect connection list for node with ID = 4");
            Assert.IsTrue(nodeList[4].ConnectionList[1].ID == 1, "incorrect connection list for node with ID = 4");
            Assert.IsTrue(nodeList[4].ConnectionList[2].ID == 0, "incorrect connection list for node with ID = 4");
        }

        /// <summary>
        /// точность сравнения действительных чисел
        /// </summary>
        private const Double Epsilon = 1e-9;

        /// <summary>
        /// сравниватель, для приближенного сравнения действительных чисел
        /// </summary>
        private readonly ApproxComp m_ApproxComparer;
    }
}