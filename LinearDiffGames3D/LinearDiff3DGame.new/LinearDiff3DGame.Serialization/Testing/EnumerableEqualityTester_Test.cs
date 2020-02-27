using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace LinearDiff3DGame.Serialization.Testing
{
    [TestFixture]
    public class EnumerableEqualityTester_Test
    {
        [Test]
        public void TestEqualEnumerables()
        {
            IEnumerable<Int32> enumerable1 = new List<Int32> {1, 2, 3};
            IEnumerable<Int32> enumerable2 = new List<Int32> {1, 2, 3};
            Assert.IsTrue(EnumerableEqualityTester.TestEquality(enumerable1, enumerable2, (item1, item2) => Equals(item1, item2)));
        }

        [Test]
        public void TestNonEqualEnumerables()
        {
            IEnumerable<Int32> enumerable1 = new List<Int32> { 1, 2, 3 };
            IEnumerable<Int32> enumerable2 = new List<Int32> { 1, 2, 4 };
            IEnumerable<Int32> enumerable3 = new List<Int32> { 1, 2, 3, 4 };
            Assert.IsFalse(EnumerableEqualityTester.TestEquality(enumerable1, enumerable2, (item1, item2) => Equals(item1, item2)));
            Assert.IsFalse(EnumerableEqualityTester.TestEquality(enumerable1, enumerable3, (item1, item2) => Equals(item1, item2)));
        }
    }
}