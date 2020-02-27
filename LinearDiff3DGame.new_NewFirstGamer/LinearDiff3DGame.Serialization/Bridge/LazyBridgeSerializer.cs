using System;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Serialization.Common;
using LinearDiff3DGame.Serialization.Geometry3D;

namespace LinearDiff3DGame.Serialization.Bridge
{
    public class LazyBridgeSerializer : BridgeSerializer
    {
        protected override EnumerableBinarySerializer<Pair<Double, Polyhedron3D>> GetSerializer()
        {
            DoubleBinarySerializer doubleSerializer = new DoubleBinarySerializer();
            Polyhedron3DBinarySerializer polyhedronSerializer = new Polyhedron3DBinarySerializer();
            PairBinarySerializer<Double, Polyhedron3D> pairSerializer =
                new PairBinarySerializer<Double, Polyhedron3D>(doubleSerializer, polyhedronSerializer);
            return new LazyEnumerableBinarySerializer<Pair<Double, Polyhedron3D>>(pairSerializer);
        }
    }
}