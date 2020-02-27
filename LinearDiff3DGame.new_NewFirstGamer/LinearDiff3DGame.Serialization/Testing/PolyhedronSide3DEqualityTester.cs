using System;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.Serialization.Testing
{
    internal static class PolyhedronSide3DEqualityTester
    {
        public static Boolean TestEquality(PolyhedronSide3D side1, PolyhedronSide3D side2)
        {
            if(!Equals(side1.ID, side2.ID)) return false;
            if(!Equals(side1.SideNormal, side2.SideNormal)) return false;
            if(!Equals(side1.VertexList.Count, side2.VertexList.Count)) return false;
            return EnumerableEqualityTester.TestEquality(side1.VertexList,
                                                         side2.VertexList,
                                                         PolyhedronVertex3DEqualityTester.TestEquality);
        }
    }
}