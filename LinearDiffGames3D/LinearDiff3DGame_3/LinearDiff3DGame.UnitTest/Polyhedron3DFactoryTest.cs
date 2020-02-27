using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.Geometry3D;

namespace LinearDiff3DGame.UnitTest
{
    /// <summary>
    /// Summary description for Polyhedron3DFactoryTest
    /// </summary>
    [TestClass]
    public class Polyhedron3DFactoryTest
    {
        public Polyhedron3DFactoryTest()
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
        public void Polyhedron1TestMethod()
        {
            //
            // TODO: Add test logic	here
            //

            // ������������ - ���
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

            IList<PolyhedronSide3D> sideList = cube.SideList;
            IList<PolyhedronVertex3D> vertexList = cube.VertexList;

            // �������� ���� ������
            for (Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
            {
                Assert.IsTrue(Compare2Vertexes(vertexList[vertexIndex], vertexes[vertexIndex]), "incorrect vertex in vertex's list");
            }

            // ������ �����
            Double angle1 = Vector3D.AngleBetweenVectors(sideList[0].SideNormal, new Vector3D(0.0, 0.0, 1.0));
            Assert.IsTrue(m_ApproxComparer.EQ(angle1, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[0].ID == 0, "incorrect side's ID");

            // ������ �����
            Double angle2 = Vector3D.AngleBetweenVectors(sideList[1].SideNormal, new Vector3D(0.0, 1.0, 0.0));
            Assert.IsTrue(m_ApproxComparer.EQ(angle2, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[1].ID == 1, "incorrect side's ID");

            // ������ �����
            Double angle3 = Vector3D.AngleBetweenVectors(sideList[2].SideNormal, new Vector3D(-1.0, 0.0, 0.0));
            Assert.IsTrue(m_ApproxComparer.EQ(angle3, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[2].ID == 2, "incorrect side's ID");

            // ��������� �����
            Double angle4 = Vector3D.AngleBetweenVectors(sideList[3].SideNormal, new Vector3D(0.0, -1.0, 0.0));
            Assert.IsTrue(m_ApproxComparer.EQ(angle4, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[3].ID == 3, "incorrect side's ID");

            // ����� �����
            Double angle5 = Vector3D.AngleBetweenVectors(sideList[4].SideNormal, new Vector3D(1.0, 0.0, 0.0));
            Assert.IsTrue(m_ApproxComparer.EQ(angle5, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[4].ID == 4, "incorrect side's ID");

            // ������ �����
            Double angle6 = Vector3D.AngleBetweenVectors(sideList[5].SideNormal, new Vector3D(0.0, 0.0, -1.0));
            Assert.IsTrue(m_ApproxComparer.EQ(angle6, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[5].ID == 5, "incorrect side's ID");
        }

        [TestMethod]
        public void Polyhedron2TestMethod()
        {
            //
            // TODO: Add test logic	here
            //

            // ������������ - ��������, � ��������� � ���������
            Point3D[] vertexes = new Point3D[] { new Point3D(1.0, 1.0, 0.0),
                                                 new Point3D(-1.0, 1.0, 0.0),
                                                 new Point3D(-1.0, -1.0, 0.0),
                                                 new Point3D(1.0, -1.0, 0.0),
                                                 new Point3D(0.0, 0.0, 1.0)};

            Polyhedron3DFromPointsFactory pyramidFactory = new Polyhedron3DFromPointsFactory(m_ApproxComparer);
            Polyhedron3D pyramid = pyramidFactory.CreatePolyhedron(vertexes);

            IList<PolyhedronSide3D> sideList = pyramid.SideList;
            IList<PolyhedronVertex3D> vertexList = pyramid.VertexList;

            // �������� ���� ������
            for (Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
            {
                Assert.IsTrue(Compare2Vertexes(vertexList[vertexIndex], vertexes[vertexIndex]), "incorrect vertex in vertex's list");
            }

            // ������ �����
            Double angle1 = Vector3D.AngleBetweenVectors(sideList[0].SideNormal, new Vector3D(0.0, 0.0, -1.0));
            Assert.IsTrue(m_ApproxComparer.EQ(angle1, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[0].ID == 0, "incorrect side's ID");

            // ������ �����
            Double angle2 = Vector3D.AngleBetweenVectors(sideList[1].SideNormal, new Vector3D(0.70710678119, 0.0, 0.70710678119));
            Assert.IsTrue(m_ApproxComparer.EQ(angle2, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[1].ID == 1, "incorrect side's ID");

            // ������ �����
            Double angle3 = Vector3D.AngleBetweenVectors(sideList[2].SideNormal, new Vector3D(0.0, -0.70710678119, 0.70710678119));
            Assert.IsTrue(m_ApproxComparer.EQ(angle3, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[2].ID == 2, "incorrect side's ID");

            // ��������� �����
            Double angle4 = Vector3D.AngleBetweenVectors(sideList[3].SideNormal, new Vector3D(-0.70710678119, 0.0, 0.70710678119));
            Assert.IsTrue(m_ApproxComparer.EQ(angle4, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[3].ID == 3, "incorrect side's ID");

            // ����� �����
            Double angle5 = Vector3D.AngleBetweenVectors(sideList[4].SideNormal, new Vector3D(0.0, 0.70710678119, 0.70710678119));
            Assert.IsTrue(m_ApproxComparer.EQ(angle5, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[4].ID == 4, "incorrect side's ID");
        }

        /// <summary>
        /// ��������� ���� ������
        /// </summary>
        /// <param name="vertex1">������� 1</param>
        /// <param name="vertex2">������� 2</param>
        /// <returns>true, ���� ������� 1 � 2 ��������� (� �������� �������� ��������� �������������� �����); ����� - false</returns>
        private Boolean Compare2Vertexes(PolyhedronVertex3D vertex1, Point3D vertex2)
        {
            return m_ApproxComparer.EQ(vertex1.XCoord, vertex2.XCoord) &&
                   m_ApproxComparer.EQ(vertex1.YCoord, vertex2.YCoord) &&
                   m_ApproxComparer.EQ(vertex1.ZCoord, vertex2.ZCoord);
        }

        /// <summary>
        /// �������� ��������� �������������� �����
        /// </summary>
        private const Double Epsilon = 1e-9;
        /// <summary>
        /// ������������, ��� ������������� ��������� �������������� �����
        /// </summary>
        private ApproxComp m_ApproxComparer;
    }
}
