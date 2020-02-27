using System;
using LinearDiff3DGame.Serialization.Testing;
using NUnit.Framework;

namespace LinearDiff3DGame.Serialization.Common
{
    [TestFixture]
    public class Int32BinarySerializer_Test : SerializerTest<Int32>
    {
        public Int32BinarySerializer_Test()
            : base(data, BitConverter.GetBytes(data), new Int32BinarySerializer(), (obj1, obj2) => Equals(obj1, obj2))
        {
        }

        private const Int32 data = 456;
    }
}