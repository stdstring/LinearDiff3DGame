using System;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.OpenGLVisualizerTest.Objects3D
{
    [Immutable]
    internal struct Point
    {
        public Point(Double x, Double y, Double z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Double X { get; private set; }
        public Double Y { get; private set; }
        public Double Z { get; private set; }
    }
}