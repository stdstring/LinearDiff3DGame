using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.Geometry3D;

namespace LinearDiff3DGame.UnitTest
{
    /// <summary>
    /// Summary description for Polyhedron3DGraphFactoryTest
    /// </summary>
    [TestClass]
    public class Polyhedron3DGraphFactoryTest
    {
        public Polyhedron3DGraphFactoryTest()
        {
            //
            // TODO: Add constructor logic here
            //
            m_ApproxComparer = new ApproxComp(Epsilon);
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void PolyhedronGraph1TestMethod()
        {
            //
            // TODO: Add test logic	here
            //

            // многогранник - куб
            Point3D[] vertexes = new Point3D[] { new Point3D(1.0, 1.0, 1.0),
                                                 new Point3D(-1.0, 1.0, 1.0),
                                                 new Point3D(-1.0, -1.0, 1.0),
                                                 new Point3D(1.0, -1.0, 1.0),
                                                 new Point3D(1.0, 1.0, -1.0),
                                                 new Point3D(-1.0, 1.0, -1.0),
                                                 new Point3D(-1.0, -1.0, -1.0),
                                                 new Point3D(1.0, -1.0, -1.0)};

            Polyhedron3DFromPointsFactory cubeFactory = new Polyhedron3DFromPointsFactory(m_ApproxComparer);
            Polyhedron3D cube = cubeFactory.CreatePolyhedron(vertexes);

            Polyhedron3DGraphFactory cubeGraphFactory = new Polyhedron3DGraphFactory();
            Polyhedron3DGraph cubeGraph = cubeGraphFactory.CreatePolyhedronGraph(cube);

            IList<PolyhedronSide3D> sideList = cube.SideList;
            IList<Polyhedron3DGraphNode> nodeList = cubeGraph.NodeList;

            Assert.IsTrue(sideList.Count == nodeList.Count, "must sideList.Count == nodeList.Count");

            for (Int32 sideIndex = 0; sideIndex < sideList.Count; ++sideIndex)
            {
                Double angleValue = Vector3D.AngleBetweenVectors(sideList[sideIndex].SideNormal, nodeList[sideIndex].NodeNormal);
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

        [TestMethod]
        public void PolyhedronGraph2TestMethod()
        {
            //
            // TODO: Add test logic	here
            //

            // многогранник - пирамида, с квадратом в основании
            Point3D[] vertexes = new Point3D[] { new Point3D(1.0, 1.0, 0.0),
                                                 new Point3D(-1.0, 1.0, 0.0),
                                                 new Point3D(-1.0, -1.0, 0.0),
                                                 new Point3D(1.0, -1.0, 0.0),
                                                 new Point3D(0.0, 0.0, 1.0)};

            Polyhedron3DFromPointsFactory pyramidFactory = new Polyhedron3DFromPointsFactory(m_ApproxComparer);
            Polyhedron3D pyramid = pyramidFactory.CreatePolyhedron(vertexes);

            Polyhedron3DGraphFactory pyramidGraphFactory = new Polyhedron3DGraphFactory();
            Polyhedron3DGraph pyramidGraph = pyramidGraphFactory.CreatePolyhedronGraph(pyramid);

            IList<PolyhedronSide3D> sideList = pyramid.SideList;
            IList<Polyhedron3DGraphNode> nodeList = pyramidGraph.NodeList;

            Assert.IsTrue(sideList.Count == nodeList.Count, "must sideList.Count == nodeList.Count");

            for (Int32 sideIndex = 0; sideIndex < sideList.Count; ++sideIndex)
            {
                Double angleValue = Vector3D.AngleBetweenVectors(sideList[sideIndex].SideNormal, nodeList[sideIndex].NodeNormal);
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
        private ApproxComp m_ApproxComparer;
    }
}
