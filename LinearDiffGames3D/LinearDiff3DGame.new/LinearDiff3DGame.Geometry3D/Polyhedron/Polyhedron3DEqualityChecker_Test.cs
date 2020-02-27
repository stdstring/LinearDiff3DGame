using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronFactory;
using NUnit.Framework;

namespace LinearDiff3DGame.Geometry3D.Polyhedron
{
	[TestFixture]
	public class Polyhedron3DEqualityChecker_Test
	{
		[Test]
		public void PolyhedronsEquality()
		{
			IPolyhedron3D polyhedron1 = CreatePolyhedron(new Point3D(1, 1, 1),
			                                             new Point3D(1, -1, 1),
			                                             new Point3D(-1, 1, 1),
			                                             new Point3D(-1, -1, 1),
			                                             new Point3D(1, 1, -1),
			                                             new Point3D(1, -1, -1),
			                                             new Point3D(-1, 1, -1),
			                                             new Point3D(-1, -1, -1));
			IPolyhedron3D polyhedron2 = CreatePolyhedron(new Point3D(1, 1, 1.0001),
			                                             new Point3D(1, -1.0001, 1),
			                                             new Point3D(-1, 1.0001, 1),
			                                             new Point3D(-1.0001, -1, 1),
			                                             new Point3D(1, 1.0001, -1),
			                                             new Point3D(1.0001, -1, -1.0001),
			                                             new Point3D(-1, 1.0001, -1),
			                                             new Point3D(-1, -1, -1.0001));
			Assert.IsTrue(new Polyhedron3DEqualityChecker(approxComparer).Equal(polyhedron1, polyhedron2));
		}

		[Test]
		public void PolyhedronsNonEquality()
		{
			IPolyhedron3D polyhedron1 = CreatePolyhedron(new Point3D(1, 1, 1),
			                                             new Point3D(1, -1, 1),
			                                             new Point3D(-1, 1, 1),
			                                             new Point3D(-1, -1, 1),
			                                             new Point3D(1, 1, -1),
			                                             new Point3D(1, -1, -1),
			                                             new Point3D(-1, 1, -1),
			                                             new Point3D(-1, -1, -1));
			IPolyhedron3D polyhedron2 = CreatePolyhedron(new Point3D(1, 1, 1.1),
			                                             new Point3D(1, -1.0001, 1),
			                                             new Point3D(-1, 1.0001, 1),
			                                             new Point3D(-1.01, -1, 1),
			                                             new Point3D(1, 1.1, -1),
			                                             new Point3D(1.0001, -1, -1.0001),
			                                             new Point3D(-1, 1.1, -1),
			                                             new Point3D(-1, -1, -1.0001));
			Assert.IsFalse(new Polyhedron3DEqualityChecker(approxComparer).Equal(polyhedron1, polyhedron2));
		}

		private static IPolyhedron3D CreatePolyhedron(params Point3D[] vertexes)
		{
			return new Polyhedron3DFromPointsFactory(new ApproxComp(1e-9)).
				CreatePolyhedron(vertexes);
		}

		private readonly ApproxComp approxComparer = new ApproxComp(1e-2);
	}
}