using System;
using System.Collections.Generic;
using System.Text;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Serialization.Testing;
using NUnit.Framework;

namespace LinearDiff3DGame.Serialization.Bridge
{
    [TestFixture]
    public class BridgeSerializer_Test : SerializerTest<Pair<Int32, IEnumerable<Pair<Double, Polyhedron3D>>>>
    {
        public BridgeSerializer_Test()
            :
                base(GetData(),
                     GetSerializedData(),
                     new BridgeSerializer(),
                     BridgeEqualityTester)
        {
        }

        private static Pair<Int32, IEnumerable<Pair<Double, Polyhedron3D>>> GetData()
        {
            IList<Pair<Double, Polyhedron3D>> bridge =
                new List<Pair<Double, Polyhedron3D>>
                    {
                        new Pair<Double, Polyhedron3D>(0, TestDataGenerator.GetPolyhedron(1.1)),
                        new Pair<Double, Polyhedron3D>(0.52, TestDataGenerator.GetPolyhedron(2.3))
                    };
            return new Pair<Int32, IEnumerable<Pair<Double, Polyhedron3D>>>(bridge.Count, bridge);
        }

        private static Byte[] GetSerializedData()
        {
            Pair<Int32, IEnumerable<Pair<Double, Polyhedron3D>>> data = GetData();
            List<Byte> serializedData = new List<Byte>();
            serializedData.AddRange(Encoding.ASCII.GetBytes("LD3G"));
            serializedData.AddRange(BitConverter.GetBytes(data.Item1));
            foreach(Pair<Double, Polyhedron3D> section in data.Item2)
            {
                serializedData.AddRange(BitConverter.GetBytes(section.Item1));
                Polyhedron3D polyhedron = section.Item2;
                serializedData.AddRange(BitConverter.GetBytes(polyhedron.VertexList.Count));
                foreach(PolyhedronVertex3D vertex in polyhedron.VertexList)
                {
                    serializedData.AddRange(BitConverter.GetBytes(vertex.ID));
                    serializedData.AddRange(BitConverter.GetBytes(vertex.XCoord));
                    serializedData.AddRange(BitConverter.GetBytes(vertex.YCoord));
                    serializedData.AddRange(BitConverter.GetBytes(vertex.ZCoord));
                }
                serializedData.AddRange(BitConverter.GetBytes(polyhedron.SideList.Count));
                foreach(PolyhedronSide3D side in polyhedron.SideList)
                {
                    serializedData.AddRange(BitConverter.GetBytes(side.ID));
                    serializedData.AddRange(BitConverter.GetBytes(side.SideNormal.XCoord));
                    serializedData.AddRange(BitConverter.GetBytes(side.SideNormal.YCoord));
                    serializedData.AddRange(BitConverter.GetBytes(side.SideNormal.ZCoord));
                    serializedData.AddRange(BitConverter.GetBytes(side.VertexList.Count));
                    foreach(PolyhedronVertex3D vertex in side.VertexList)
                        serializedData.AddRange(BitConverter.GetBytes(vertex.ID));
                }
            }
            return serializedData.ToArray();
        }

        private static Boolean BridgeEqualityTester(Pair<Int32, IEnumerable<Pair<Double, Polyhedron3D>>> bridge1,
                                                    Pair<Int32, IEnumerable<Pair<Double, Polyhedron3D>>> bridge2)
        {
            if(!Equals(bridge1.Item1, bridge2.Item1)) return false;
            return EnumerableEqualityTester.TestEquality(bridge1.Item2,
                                                         bridge2.Item2,
                                                         (pair1, pair2) => PairEqualityTester.TestEquality(pair1, pair2, (time1, time2) => Equals(time1, time2), Polyhedron3DEqualityTester.TestEquality));
        }
    }
}