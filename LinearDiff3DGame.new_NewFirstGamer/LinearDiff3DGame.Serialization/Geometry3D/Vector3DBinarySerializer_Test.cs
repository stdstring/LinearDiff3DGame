using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Serialization.Testing;
using NUnit.Framework;

namespace LinearDiff3DGame.Serialization.Geometry3D
{
    [TestFixture]
    public class Vector3DBinarySerializer_Test : SerializerTest<Vector3D>
    {
        public Vector3DBinarySerializer_Test()
            : base(data,
                   GetSerializedData(),
                   new Vector3DBinarySerializer(),
                   (obj1, obj2) => Equals(obj1, obj2))
        {
        }

        private static Byte[] GetSerializedData()
        {
            List<Byte> serializedData = new List<Byte>();
            serializedData.AddRange(BitConverter.GetBytes(data.XCoord));
            serializedData.AddRange(BitConverter.GetBytes(data.YCoord));
            serializedData.AddRange(BitConverter.GetBytes(data.ZCoord));
            return serializedData.ToArray();
        }

        private static Vector3D data = new Vector3D(1.1, 2.2, 3.3);
    }
}