using System;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Serialization.Common;
using LinearDiff3DGame.Serialization.Geometry3D;

namespace LinearDiff3DGame.Serialization.Bridge
{
    public class LazyBridgeSerializer : BridgeSerializer
    {
        protected override EnumerableBinarySerializer<Pair<Double, IPolyhedron3D>> GetSerializer()
        {
            DoubleBinarySerializer doubleSerializer = new DoubleBinarySerializer();
            Polyhedron3DBinarySerializer polyhedronSerializer = new Polyhedron3DBinarySerializer();
            PairBinarySerializer<Double, IPolyhedron3D> pairSerializer =
                new PairBinarySerializer<Double, IPolyhedron3D>(doubleSerializer, polyhedronSerializer);
            return new LazyEnumerableBinarySerializer<Pair<Double, IPolyhedron3D>>(pairSerializer);
        }
    }
}