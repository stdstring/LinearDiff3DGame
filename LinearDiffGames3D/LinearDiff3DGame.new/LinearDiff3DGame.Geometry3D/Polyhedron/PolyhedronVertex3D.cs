using System;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.Geometry3D.Polyhedron
{
    public interface IPolyhedronVertex3D
    {
        Int32 ID { get; }
        Double XCoord { get; }
        Double YCoord { get; }
        Double ZCoord { get; }
    }

    [Immutable]
    public class PolyhedronVertex3D : IPolyhedronVertex3D
    {
        public PolyhedronVertex3D(Double xCoord, Double yCoord, Double zCoord, Int32 vertexID)
        {
            id = vertexID;
            XCoord = xCoord;
            YCoord = yCoord;
            ZCoord = zCoord;
        }

        public PolyhedronVertex3D(Point3D vertex, Int32 vertexID)
            : this(vertex.X, vertex.Y, vertex.Z, vertexID)
        {
        }

        public Int32 ID
        {
            get { return id; }
        }

        public const Int32 Dimension = 3;

        public Double XCoord { get; private set; }

        public Double YCoord { get; private set; }

        public Double ZCoord { get; private set; }

        private readonly Int32 id;
    }
}