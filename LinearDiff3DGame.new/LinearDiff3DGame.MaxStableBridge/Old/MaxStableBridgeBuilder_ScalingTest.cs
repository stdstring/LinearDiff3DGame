using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.Matrix;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Geometry3D.PolyhedronFactory;
using NUnit.Framework;

namespace LinearDiff3DGame.MaxStableBridge.Old
{
	[TestFixture]
	[Explicit]
	public class MaxStableBridgeBuilder_ScalingTest
	{
		[Test]
		[Explicit]
		public void CompareResultsWithAndWithoutScaling()
		{
			const Double maxInverseT = 1.0;

			builderWithScaling = new MaxStableBridgeBuilder_old();
			builderWithoutScaling = new MaxStableBridgeBuilderWithoutScaling();
			Polyhedron3DEqualityChecker checker = new Polyhedron3DEqualityChecker(approxComp);

			while (approxComp.LE(builderWithScaling.CurrentInverseTime, maxInverseT))
			{
				builderWithScaling.NextIteration();
				builderWithoutScaling.NextIteration();

				IPolyhedron3D polyhedronWithScaling = TransformPolyhedron(builderWithScaling.CurrentPolyhedron,
				                                                          builderWithScaling.ReverseTransformation);
				IPolyhedron3D polyhedronWithoutScaling = builderWithoutScaling.CurrentPolyhedron;

				Assert.IsTrue(checker.Equal(polyhedronWithScaling, polyhedronWithoutScaling));
			}
		}

		private IPolyhedron3D TransformPolyhedron(IPolyhedron3D polyhedron, Matrix transformation)
		{
			Point3D[] polyhedronVertexes = PolyhedronVertexes(polyhedron);
			List<Point3D> vertexesAfterTransformation = new List<Point3D>();
			foreach (Point3D vertex in polyhedronVertexes)
			{
				Point3D afterTranstormation =
					Geometry3DObjectFactory.CreatePoint(transformation*Geometry3DObjectFactory.CreateMatrix(vertex));
				vertexesAfterTransformation.Add(afterTranstormation);
			}
			return new Polyhedron3DFromPointsFactory(approxComp).CreatePolyhedron(vertexesAfterTransformation.ToArray());
		}

		private static Point3D[] PolyhedronVertexes(IPolyhedron3D polyhedron)
		{
			List<Point3D> points = new List<Point3D>();
			foreach (IPolyhedronVertex3D vertex in polyhedron.VertexList)
			{
				points.Add(new Point3D(vertex.XCoord, vertex.YCoord, vertex.ZCoord));
			}
			return points.ToArray();
		}

		private MaxStableBridgeBuilder_old builderWithScaling;
		private MaxStableBridgeBuilderWithoutScaling builderWithoutScaling;

		private readonly ApproxComp approxComp = new ApproxComp(1e-9);
	}
}