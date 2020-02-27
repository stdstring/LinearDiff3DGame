using System;
using NUnit.Framework;

namespace LinearDiff3DGame.Common
{
    [TestFixture]
    public class Tuple_Test
    {
        [Test]
        public void Tuple4()
        {
            Tuple<String, String, String, String> tuple1 = new Tuple<String, String, String, String>("blablabla", "blebleble", "blobloblo", "blublublu");
            Tuple<String, String, String, String> tuple2 = new Tuple<String, String, String, String>("blablabla", "blebleble", "blobloblo", "blublublu");
            Tuple<String, String, String, String> tuple3 = new Tuple<String, String, String, String>("111", "222", "333", "444");
            Assert.AreEqual(tuple1, tuple2);
            Assert.AreNotEqual(tuple1, tuple3);
        }

        [Test]
        public void Tuple3()
        {
            Tuple<String, String, String> tuple1 = new Tuple<String, String, String>("blablabla", "blebleble", "blobloblo");
            Tuple<String, String, String> tuple2 = new Tuple<String, String, String>("blablabla", "blebleble", "blobloblo");
            Tuple<String, String, String> tuple3 = new Tuple<String, String, String>("111", "222", "333");
            Assert.AreEqual(tuple1, tuple2);
            Assert.AreNotEqual(tuple1, tuple3);
        }

        [Test]
        public void Tuple2()
        {
            Tuple<String, String> tuple1 = new Tuple<String, String>("blablabla", "blebleble");
            Tuple<String, String> tuple2 = new Tuple<String, String>("blablabla", "blebleble");
            Tuple<String, String> tuple3 = new Tuple<String, String>("111", "222");
            Assert.AreEqual(tuple1, tuple2);
            Assert.AreNotEqual(tuple1, tuple3);
        }
    }
}
