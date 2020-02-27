using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Serialization.Testing;
using NUnit.Framework;

namespace LinearDiff3DGame.Serialization.Geometry3D
{
    [TestFixture]
    public class Point3DBinarySerializer_Test : SerializerTest<Point3D>
    {
        public Point3DBinarySerializer_Test()
            : base(data,
                   GetSerializedData(),
                   new Point3DBinarySerializer(),
                   (obj1, obj2) => Equals(obj1, obj2))
        {
        }

        private static Byte[] GetSerializedData()
        {
            List<Byte> serializedData = new List<Byte>();
            serializedData.AddRange(BitConverter.GetBytes(data.X));
            serializedData.AddRange(BitConverter.GetBytes(data.Y));
            serializedData.AddRange(BitConverter.GetBytes(data.Z));
            return serializedData.ToArray();
        }

        private static Point3D data = new Point3D(1.1, 2.2, 3.3);
    }
}