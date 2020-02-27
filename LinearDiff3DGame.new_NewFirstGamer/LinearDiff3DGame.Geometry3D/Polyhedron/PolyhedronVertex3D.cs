using System;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.Geometry3D.Polyhedron
{
    public class PolyhedronVertex3D
    {
        public PolyhedronVertex3D(Double xCoord, Double yCoord, Double zCoord, Int32 vertexID)
        {
            ID = vertexID;

            XCoord = xCoord;
            YCoord = yCoord;
            ZCoord = zCoord;
        }

        public PolyhedronVertex3D(Point3D vertex, Int32 vertexID)
            : this(vertex.XCoord, vertex.YCoord, vertex.ZCoord, vertexID)
        {
        }

        public readonly Int32 ID;

        public const Int32 Dimension = 3;

        public Double XCoord { get; set; }

        public Double YCoord { get; set; }

        public Double ZCoord { get; set; }
    }
}