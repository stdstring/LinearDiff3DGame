using System;
using System.IO;
using NUnit.Framework;

namespace LinearDiff3DGame.Serialization.Common
{
    [TestFixture]
    public class Int32BinarySerializer : ISerializer<Int32>
    {
        public void Serialize(Stream storage, int serializableObject)
        {
            Byte[] bytes = BitConverter.GetBytes(serializableObject);
            storage.Write(bytes, 0, bytes.Length);
        }

        public int Deserialize(Stream storage)
        {
            Byte[] bytes = new Byte[sizeof(Int32)];
            storage.Read(bytes, 0, bytes.Length);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}