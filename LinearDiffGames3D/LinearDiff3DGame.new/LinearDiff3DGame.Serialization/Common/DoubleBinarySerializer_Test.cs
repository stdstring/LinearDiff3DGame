using System;
using LinearDiff3DGame.Serialization.Testing;
using NUnit.Framework;

namespace LinearDiff3DGame.Serialization.Common
{
    [TestFixture]
    public class DoubleBinarySerializer_Test : SerializerTest<Double>
    {
        public DoubleBinarySerializer_Test()
            : base(data, BitConverter.GetBytes(data), new DoubleBinarySerializer(), (obj1, obj2) => Equals(obj1, obj2))
        {
        }

        private const Double data = 3.1415926;
    }
}