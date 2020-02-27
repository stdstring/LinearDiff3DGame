using System;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.Serialization.Testing
{
	internal static class PolyhedronVertex3DEqualityTester
	{
		public static Boolean TestEquality(IPolyhedronVertex3D vertex1, IPolyhedronVertex3D vertex2)
		{
			return Equals(vertex1.ID, vertex2.ID) &&
			       Equals(vertex1.XCoord, vertex2.XCoord) &&
			       Equals(vertex1.YCoord, vertex2.YCoord) &&
			       Equals(vertex1.ZCoord, vertex2.ZCoord);
		}
	}
}