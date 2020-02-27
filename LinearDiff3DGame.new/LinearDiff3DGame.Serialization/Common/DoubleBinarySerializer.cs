using System;
using System.IO;

namespace LinearDiff3DGame.Serialization.Common
{
    public class DoubleBinarySerializer : ISerializer<Double>
    {
        public void Serialize(Stream storage, Double serializableObject)
        {
            Byte[] bytes = BitConverter.GetBytes(serializableObject);
            storage.Write(bytes, 0, bytes.Length);
        }

        public Double Deserialize(Stream storage)
        {
            Byte[] bytes = new Byte[sizeof(Double)];
            storage.Read(bytes, 0, bytes.Length);
            return BitConverter.ToDouble(bytes, 0);
        }
    }
}