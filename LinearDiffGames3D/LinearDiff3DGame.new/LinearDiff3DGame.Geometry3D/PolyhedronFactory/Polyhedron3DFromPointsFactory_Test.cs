using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using NUnit.Framework;

namespace LinearDiff3DGame.Geometry3D.PolyhedronFactory
{
    [TestFixture]
    public class Polyhedron3DFromPointsFactory_Test
    {
        public Polyhedron3DFromPointsFactory_Test()
        {
            approxComparer = new ApproxComp(epsilon);
        }

        [Test]
        public void PolyhedronCubeTest()
        {
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

            IList<IPolyhedronSide3D> sideList = cube.SideList;
            IList<IPolyhedronVertex3D> vertexList = cube.VertexList;

            // проверка всех вершин
            for (Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
            {
                Assert.IsTrue(Compare2Vertexes(vertexList[vertexIndex], vertexes[vertexIndex]),
                              "incorrect vertex in vertex's list");
            }

            // первая грань
            Double angle1 = Vector3DUtils.AngleBetweenVectors(sideList[0].SideNormal, new Vector3D(0.0, 0.0, 1.0));
            Assert.IsTrue(approxComparer.EQ(angle1, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[0].ID == 0, "incorrect side's ID");

            // вторая грань
            Double angle2 = Vector3DUtils.AngleBetweenVectors(sideList[1].SideNormal, new Vector3D(0.0, 1.0, 0.0));
            Assert.IsTrue(approxComparer.EQ(angle2, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[1].ID == 1, "incorrect side's ID");

            // третья грань
            Double angle3 = Vector3DUtils.AngleBetweenVectors(sideList[2].SideNormal, new Vector3D(-1.0, 0.0, 0.0));
            Assert.IsTrue(approxComparer.EQ(angle3, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[2].ID == 2, "incorrect side's ID");

            // четвертая грань
            Double angle4 = Vector3DUtils.AngleBetweenVectors(sideList[3].SideNormal, new Vector3D(0.0, -1.0, 0.0));
            Assert.IsTrue(approxComparer.EQ(angle4, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[3].ID == 3, "incorrect side's ID");

            // пятая грань
            Double angle5 = Vector3DUtils.AngleBetweenVectors(sideList[4].SideNormal, new Vector3D(1.0, 0.0, 0.0));
            Assert.IsTrue(approxComparer.EQ(angle5, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[4].ID == 4, "incorrect side's ID");

            // шестая грань
            Double angle6 = Vector3DUtils.AngleBetweenVectors(sideList[5].SideNormal, new Vector3D(0.0, 0.0, -1.0));
            Assert.IsTrue(approxComparer.EQ(angle6, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[5].ID == 5, "incorrect side's ID");
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

            IList<IPolyhedronSide3D> sideList = pyramid.SideList;
            IList<IPolyhedronVertex3D> vertexList = pyramid.VertexList;

            // проверка всех вершин
            for (Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
            {
                Assert.IsTrue(Compare2Vertexes(vertexList[vertexIndex], vertexes[vertexIndex]),
                              "incorrect vertex in vertex's list");
            }

            // первая грань
            Double angle1 = Vector3DUtils.AngleBetweenVectors(sideList[0].SideNormal, new Vector3D(0.0, 0.0, -1.0));
            Assert.IsTrue(approxComparer.EQ(angle1, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[0].ID == 0, "incorrect side's ID");

            // вторая грань
            Double angle2 = Vector3DUtils.AngleBetweenVectors(sideList[1].SideNormal,
                                                              new Vector3D(0.70710678119, 0.0, 0.70710678119));
            Assert.IsTrue(approxComparer.EQ(angle2, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[1].ID == 1, "incorrect side's ID");

            // третья грань
            Double angle3 = Vector3DUtils.AngleBetweenVectors(sideList[2].SideNormal,
                                                              new Vector3D(0.0, -0.70710678119, 0.70710678119));
            Assert.IsTrue(approxComparer.EQ(angle3, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[2].ID == 2, "incorrect side's ID");

            // четвертая грань
            Double angle4 = Vector3DUtils.AngleBetweenVectors(sideList[3].SideNormal,
                                                              new Vector3D(-0.70710678119, 0.0, 0.70710678119));
            Assert.IsTrue(approxComparer.EQ(angle4, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[3].ID == 3, "incorrect side's ID");

            // пятая грань
            Double angle5 = Vector3DUtils.AngleBetweenVectors(sideList[4].SideNormal,
                                                              new Vector3D(0.0, 0.70710678119, 0.70710678119));
            Assert.IsTrue(approxComparer.EQ(angle5, 0), "incorrect side's normal");
            Assert.IsTrue(sideList[4].ID == 4, "incorrect side's ID");
        }

        /// <summary>
        /// сравнение двух вершин
        /// </summary>
        /// <param name="vertex1">вершина 1</param>
        /// <param name="vertex2">вершина 2</param>
        /// <returns>true, если вершины 1 и 2 совпадают (в пределах точности сравнения действительных чисел); иначе - false</returns>
        private Boolean Compare2Vertexes(IPolyhedronVertex3D vertex1, Point3D vertex2)
        {
            return approxComparer.EQ(vertex1.XCoord, vertex2.X) &&
                   approxComparer.EQ(vertex1.YCoord, vertex2.Y) &&
                   approxComparer.EQ(vertex1.ZCoord, vertex2.Z);
        }

        /// <summary>
        /// точность сравнения действительных чисел
        /// </summary>
        private const Double epsilon = 1e-9;

        /// <summary>
        /// сравниватель, для приближенного сравнения действительных чисел
        /// </summary>
        private readonly ApproxComp approxComparer;
    }
}