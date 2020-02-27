using System;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.Geometry3D.Common
{
    [Immutable]
    public struct Point3D
    {
      
        public Point3D(Double x, Double y, Double z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; private set; }

        public double Y { get; private set; }

        public double Z { get; private set; }
    }
}