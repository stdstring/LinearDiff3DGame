using System;

namespace LinearDiff3DGame.MaxStableBridgeVisualizer.Space3D
{
    /// <summary>
    /// представление 3D объекта, такого как вектор или точка
    /// </summary>
    internal struct Object3D
    {
        public Object3D(Double x, Double y, Double z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Double X { get; set; }
        public Double Y { get; set; }
        public Double Z { get; set; }
    }
}
