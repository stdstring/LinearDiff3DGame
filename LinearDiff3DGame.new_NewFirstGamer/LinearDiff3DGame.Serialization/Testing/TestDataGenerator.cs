using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.Serialization.Testing
{
    internal static class TestDataGenerator
    {
        public static Polyhedron3D GetPolyhedron(Double scaleCoeff)
        {
            IList<PolyhedronVertex3D> vertexes =
                new List<PolyhedronVertex3D>
                    {
                        new PolyhedronVertex3D(scaleCoeff, scaleCoeff, scaleCoeff, 0),
                        new PolyhedronVertex3D(-scaleCoeff, scaleCoeff, scaleCoeff, 1),
                        new PolyhedronVertex3D(scaleCoeff, -scaleCoeff, scaleCoeff, 2),
                        new PolyhedronVertex3D(-scaleCoeff, -scaleCoeff, scaleCoeff, 3),
                        new PolyhedronVertex3D(scaleCoeff, scaleCoeff, -scaleCoeff, 4),
                        new PolyhedronVertex3D(-scaleCoeff, scaleCoeff, -scaleCoeff, 5),
                        new PolyhedronVertex3D(scaleCoeff, -scaleCoeff, -scaleCoeff, 6),
                        new PolyhedronVertex3D(-scaleCoeff, -scaleCoeff, -scaleCoeff, 7)
                    };
            IList<PolyhedronSide3D> sides =
                new List<PolyhedronSide3D>
                    {
                        new PolyhedronSide3D(new List<PolyhedronVertex3D> {vertexes[0], vertexes[1], vertexes[2], vertexes[3]}, 0, new Vector3D(0, 0, 1)),
                        new PolyhedronSide3D(new List<PolyhedronVertex3D> {vertexes[0], vertexes[2], vertexes[4], vertexes[6]}, 1, new Vector3D(1, 0, 0)),
                        new PolyhedronSide3D(new List<PolyhedronVertex3D> {vertexes[0], vertexes[1], vertexes[4], vertexes[5]}, 2, new Vector3D(0, 1, 0)),
                        new PolyhedronSide3D(new List<PolyhedronVertex3D> {vertexes[4], vertexes[5], vertexes[6], vertexes[7]}, 3, new Vector3D(0, 0, -1)),
                        new PolyhedronSide3D(new List<PolyhedronVertex3D> {vertexes[1], vertexes[3], vertexes[5], vertexes[7]}, 4, new Vector3D(-1, 0, 0)),
                        new PolyhedronSide3D(new List<PolyhedronVertex3D> {vertexes[2], vertexes[3], vertexes[6], vertexes[7]}, 5, new Vector3D(0, -1, 0))
                    };
            return new Polyhedron3D(sides, vertexes);
        }
    }
}