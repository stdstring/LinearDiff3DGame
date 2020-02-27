using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Serialization.Common;
using LinearDiff3DGame.Serialization.Geometry3D;

namespace LinearDiff3DGame.Serialization.Bridge
{
    public class BridgeSerializer : ISerializer<Pair<Int32, IEnumerable<Pair<Double, IPolyhedron3D>>>>
    {
        public void Serialize(Stream storage, Pair<Int32, IEnumerable<Pair<Double, IPolyhedron3D>>> bridge)
        {
            EnumerableBinarySerializer<Pair<Double, IPolyhedron3D>> sectionsSerializer = GetSerializer();
            Byte[] header = Encoding.ASCII.GetBytes(headerText);
            storage.Write(header, 0, header.Length);
            Pair<Int32, IEnumerable<Pair<Double, IPolyhedron3D>>> data =
                new Pair<Int32, IEnumerable<Pair<Double, IPolyhedron3D>>>(bridge.Item1, bridge.Item2);
            sectionsSerializer.Serialize(storage, data);
        }

        public Pair<Int32, IEnumerable<Pair<Double, IPolyhedron3D>>> Deserialize(Stream storage)
        {
            EnumerableBinarySerializer<Pair<Double, IPolyhedron3D>> sectionsSerializer = GetSerializer();
            Byte[] header = new Byte[headerText.Length];
            storage.Read(header, 0, header.Length);
            if(Encoding.ASCII.GetString(header) != headerText)
                throw new ApplicationException("Incorrect file");
            Pair<Int32, IEnumerable<Pair<Double, IPolyhedron3D>>> data =
                sectionsSerializer.Deserialize(storage);
            return data;
        }

        protected virtual EnumerableBinarySerializer<Pair<Double, IPolyhedron3D>> GetSerializer()
        {
            DoubleBinarySerializer doubleSerializer = new DoubleBinarySerializer();
            Polyhedron3DBinarySerializer polyhedronSerializer = new Polyhedron3DBinarySerializer();
            PairBinarySerializer<Double, IPolyhedron3D> pairSerializer =
                new PairBinarySerializer<Double, IPolyhedron3D>(doubleSerializer, polyhedronSerializer);
            return new EnumerableBinarySerializer<Pair<Double, IPolyhedron3D>>(pairSerializer);
        }

        private const String headerText = "LD3G";
    }
}