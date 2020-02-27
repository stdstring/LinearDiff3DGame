using NUnit.Framework;

namespace LinearDiff3DGame.Serialization.Common
{
    [TestFixture]
    public class LazyEnumerableBinarySerializer_Test
    {
        //[Test]
        //public void Serialize()
        //{
        //    Byte[] reallySerializedData = SerializerTestHelper.Serialize(data, serializer);
        //    Assert.AreEqual(GetSerializedData(), reallySerializedData);
        //}

        //[Test]
        //public void Deserialize()
        //{
        //    using(MemoryStream ms = new MemoryStream(GetSerializedData()))
        //    {
        //        Pair<Int32, IEnumerable<Int32>> reallyData = SerializerTestHelper.Deserialize(ms, serializer);
        //        Assert.IsTrue(ms.Position < ms.Length - 1);
        //        Assert.AreEqual(data.Item1, reallyData.Item1);
        //        IEnumerator<Int32> dataEnum = data.Item2.GetEnumerator();
        //        IEnumerator<Int32> reallyDataEnum = reallyData.Item2.GetEnumerator();
        //    }
        //}

        //private Byte[] GetSerializedData()
        //{
        //    List<Byte> serializedData = new List<Byte>();
        //    serializedData.AddRange(BitConverter.GetBytes(data.Item1));
        //    foreach(Int32 item in data.Item2)
        //        serializedData.AddRange(BitConverter.GetBytes(item));
        //    return serializedData.ToArray();
        //}

        //private readonly Pair<Int32, IEnumerable<Int32>> data =
        //    new Pair<Int32, IEnumerable<Int32>>(4, new[] {1, 2, 3, 2});
        //private readonly ISerializer<Pair<Int32, IEnumerable<Int32>>> serializer =
        //    new LazyEnumerableBinarySerializer<Int32>(new Int32BinarySerializer());
    }
}