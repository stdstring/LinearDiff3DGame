using System;
using NUnit.Framework;

namespace LinearDiff3DGame.Serialization.Testing
{
    [TestFixture]
    public class SerializerTest<TSerializedObject>
    {
        public SerializerTest(TSerializedObject data,
                              Byte[] serializedData,
                              ISerializer<TSerializedObject> serializer,
                              Func<TSerializedObject, TSerializedObject, Boolean> equalityTester)
        {
            this.data = data;
            this.serializedData = serializedData;
            this.serializer = serializer;
            this.equalityTester = equalityTester;
        }

        [Test]
        public void Serialize()
        {
            Byte[] reallySerializedData = SerializerTestHelper.Serialize(data, serializer);
            Assert.AreEqual(serializedData, reallySerializedData);
        }

        [Test]
        public void Deserialize()
        {
            TSerializedObject reallyData = SerializerTestHelper.Deserialize(serializedData, serializer);
            Assert.IsTrue(equalityTester(data, reallyData));
        }

        private readonly TSerializedObject data;
        private readonly Byte[] serializedData;
        private readonly ISerializer<TSerializedObject> serializer;
        private readonly Func<TSerializedObject, TSerializedObject, Boolean> equalityTester;
    }
}