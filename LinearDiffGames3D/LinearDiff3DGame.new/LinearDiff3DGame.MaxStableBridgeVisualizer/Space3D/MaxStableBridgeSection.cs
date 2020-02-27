using System;

namespace LinearDiff3DGame.MaxStableBridgeVisualizer.Space3D
{
    /// <summary>
    /// представление среза максимального стабильного моста в некоторый момент времени
    /// </summary>
    internal class MaxStableBridgeSection
    {
        internal MaxStableBridgeSection(Double t, Polyhedron polyhedron)
        {
            T = t;
            Polyhedron = polyhedron;
        }


        public Double T { get; private set; }
        public Polyhedron Polyhedron { get; private set; }
    }
}
