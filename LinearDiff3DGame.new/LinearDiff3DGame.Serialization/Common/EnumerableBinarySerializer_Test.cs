using System;
using System.Collections.Generic;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Serialization.Testing;
using NUnit.Framework;

namespace LinearDiff3DGame.Serialization.Common
{
    [TestFixture]
    public class EnumerableBinarySerializer_Test : SerializerTest<Pair<Int32, IEnumerable<Int32>>>
    {
        public EnumerableBinarySerializer_Test()
            : base(data,
                   GetSerializedData(),
                   new EnumerableBinarySerializer<Int32>(new Int32BinarySerializer()),
                   (obj1, obj2) => Equals(obj1.Item1, obj2.Item1) && EnumerableEqualityTester.TestEquality(obj1.Item2, obj2.Item2, (item1, item2) => Equals(item1, item2)))
        {
        }

        private static Byte[] GetSerializedData()
        {
            List<Byte> serializedData = new List<Byte>();
            serializedData.AddRange(BitConverter.GetBytes(data.Item1));
            foreach(Int32 item in data.Item2)
                serializedData.AddRange(BitConverter.GetBytes(item));
            return serializedData.ToArray();
        }

        private static readonly Pair<Int32, IEnumerable<Int32>> data =
            new Pair<Int32, IEnumerable<Int32>>(3, new[] {1, 2, 3});
    }
}