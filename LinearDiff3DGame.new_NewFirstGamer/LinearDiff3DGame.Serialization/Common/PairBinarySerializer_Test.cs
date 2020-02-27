using System;
using System.Collections.Generic;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Serialization.Testing;
using NUnit.Framework;

namespace LinearDiff3DGame.Serialization.Common
{
    [TestFixture]
    public class PairBinarySerializer_Test : SerializerTest<Pair<Int32, Double>>
    {
        public PairBinarySerializer_Test()
            : base(data,
                   GetSerializedData(),
                   new PairBinarySerializer<Int32, Double>(new Int32BinarySerializer(), new DoubleBinarySerializer()),
                   PairEqualityTester.TestEquality)
        {
        }

        private static Byte[] GetSerializedData()
        {
            List<Byte> serializedData = new List<Byte>();
            serializedData.AddRange(BitConverter.GetBytes(data.Item1));
            serializedData.AddRange(BitConverter.GetBytes(data.Item2));
            return serializedData.ToArray();
        }

        private static readonly Pair<Int32, Double> data = new Pair<Int32, Double>(111, 45.345);
    }
}