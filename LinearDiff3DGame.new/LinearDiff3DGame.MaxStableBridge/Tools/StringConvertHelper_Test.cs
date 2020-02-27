using System;
using System.Globalization;
using NUnit.Framework;

namespace LinearDiff3DGame.MaxStableBridge.Tools
{
    [TestFixture]
    public class StringConvertHelper_Test
    {
        [Test]
        public void String2DoubleArray()
        {
            Assert.AreEqual(new Double[] {1, 2, 3.1415926, 7},
                            StringConvertHelper.ToDoubleArray("1 2\t \t3.1415926  7", CultureInfo.InvariantCulture));
        }

        [Test]
        public void String2Int32Array()
        {
            Assert.AreEqual(new[] {1, 2, 0, 7, 9},
                            StringConvertHelper.ToInt32Array("1 2\t \t0  7 9", CultureInfo.InvariantCulture));
        }
    }
}