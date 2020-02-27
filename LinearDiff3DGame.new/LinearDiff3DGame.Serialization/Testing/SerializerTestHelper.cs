using System;
using System.IO;

namespace LinearDiff3DGame.Serialization.Testing
{
    internal static class SerializerTestHelper
    {
        public static Byte[] Serialize<TSerializedObject>(TSerializedObject sourceObject,
                                                          ISerializer<TSerializedObject> serializer)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                ms.Seek(0, SeekOrigin.Begin);
                serializer.Serialize(ms, sourceObject);
                ms.Seek(0, SeekOrigin.Begin);
                Byte[] serializedData = new Byte[ms.Length];
                ms.Read(serializedData, 0, serializedData.Length);
                return serializedData;
            }
        }

        public static TSerializedObject Deserialize<TSerializedObject>(Byte[] serializedData,
                                                                       ISerializer<TSerializedObject> serializer)
        {
            using(MemoryStream ms = new MemoryStream(serializedData))
                return Deserialize(ms, serializer);
        }

        public static TSerializedObject Deserialize<TSerializedObject>(Stream serializedDataStream,
                                                                       ISerializer<TSerializedObject> serializer)
        {
            serializedDataStream.Seek(0, SeekOrigin.Begin);
            return serializer.Deserialize(serializedDataStream);
        }
    }
}