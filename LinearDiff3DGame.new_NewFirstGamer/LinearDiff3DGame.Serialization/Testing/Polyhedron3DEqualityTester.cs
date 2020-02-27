using System;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.Serialization.Testing
{
    internal static class Polyhedron3DEqualityTester
    {
        public static Boolean TestEquality(Polyhedron3D polyhedron1, Polyhedron3D polyhedron2)
        {
            if(!Equals(polyhedron1.VertexList.Count, polyhedron2.VertexList.Count)) return false;
            if(!EnumerableEqualityTester.TestEquality(polyhedron1.VertexList,
                                                      polyhedron2.VertexList,
                                                      PolyhedronVertex3DEqualityTester.TestEquality))
                return false;
            if(!Equals(polyhedron1.SideList.Count, polyhedron2.SideList.Count)) return false;
            return EnumerableEqualityTester.TestEquality(polyhedron1.SideList,
                                                         polyhedron2.SideList,
                                                         PolyhedronSide3DEqualityTester.TestEquality);
        }
    }
}