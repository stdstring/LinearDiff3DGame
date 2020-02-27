using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace LinearDiff3DGame.Common
{
    [TestFixture]
    public class IListExtensions_Test
    {
        [Test]
        public void GetItemIndex()
        {
            Assert.AreEqual(1, list.GetItemIndex(1, 0));
            Assert.AreEqual(1, list.GetItemIndex("two", 0));
            Assert.AreEqual(2, list.GetItemIndex(1, 1));
            Assert.AreEqual(2, list.GetItemIndex("two", 1));
            Assert.AreEqual(3, list.GetItemIndex(1, 2));
            Assert.AreEqual(3, list.GetItemIndex("two", 2));
            Assert.AreEqual(0, list.GetItemIndex(1, 3));
            Assert.AreEqual(0, list.GetItemIndex("two", 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetItemIndex(1, 4));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetItemIndex("two", 4));
            Assert.AreEqual(0, list.GetItemIndex(1, -1));
            Assert.AreEqual(0, list.GetItemIndex("two", -1));
            Assert.AreEqual(3, list.GetItemIndex(1, -2));
            Assert.AreEqual(3, list.GetItemIndex("two", -2));
            Assert.AreEqual(2, list.GetItemIndex(1, -3));
            Assert.AreEqual(2, list.GetItemIndex("two", -3));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetItemIndex(1, -4));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetItemIndex("two", -4));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetItemIndex(111, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetItemIndex("twotwo", 0));
        }

        [Test]
        public void GetItem()
        {
            Assert.AreEqual("two", list.GetItem(1, 0));
            Assert.AreEqual("two", list.GetItem("two", 0));
            Assert.AreEqual("three", list.GetItem(1, 1));
            Assert.AreEqual("three", list.GetItem("two", 1));
            Assert.AreEqual("four", list.GetItem(1, 2));
            Assert.AreEqual("four", list.GetItem("two", 2));
            Assert.AreEqual("one", list.GetItem(1, 3));
            Assert.AreEqual("one", list.GetItem("two", 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetItem(1, 4));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetItem("two", 4));
            Assert.AreEqual("one", list.GetItem(1, -1));
            Assert.AreEqual("one", list.GetItem("two", -1));
            Assert.AreEqual("four", list.GetItem(1, -2));
            Assert.AreEqual("four", list.GetItem("two", -2));
            Assert.AreEqual("three", list.GetItem(1, -3));
            Assert.AreEqual("three", list.GetItem("two", -3));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetItem(1, -4));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetItem("two", -4));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetItem(111, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetItem("twotwo", 0));
        }

        [Test]
        public void GetNextItemIndex()
        {
            Assert.AreEqual(1, list.GetNextItemIndex(0));
            Assert.AreEqual(1, list.GetNextItemIndex("one"));
            Assert.AreEqual(2, list.GetNextItemIndex(1));
            Assert.AreEqual(2, list.GetNextItemIndex("two"));
            Assert.AreEqual(3, list.GetNextItemIndex(2));
            Assert.AreEqual(3, list.GetNextItemIndex("three"));
            Assert.AreEqual(0, list.GetNextItemIndex(3));
            Assert.AreEqual(0, list.GetNextItemIndex("four"));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetNextItemIndex(111));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetNextItemIndex("twotwo"));
        }

        [Test]
        public void GetNextItem()
        {
            Assert.AreEqual("two", list.GetNextItem(0));
            Assert.AreEqual("two", list.GetNextItem("one"));
            Assert.AreEqual("three", list.GetNextItem(1));
            Assert.AreEqual("three", list.GetNextItem("two"));
            Assert.AreEqual("four", list.GetNextItem(2));
            Assert.AreEqual("four", list.GetNextItem("three"));
            Assert.AreEqual("one", list.GetNextItem(3));
            Assert.AreEqual("one", list.GetNextItem("four"));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetNextItem(111));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetNextItem("twotwo"));
        }

        [Test]
        public void GetPrevItemIndex()
        {
            Assert.AreEqual(3, list.GetPrevItemIndex(0));
            Assert.AreEqual(3, list.GetPrevItemIndex("one"));
            Assert.AreEqual(0, list.GetPrevItemIndex(1));
            Assert.AreEqual(0, list.GetPrevItemIndex("two"));
            Assert.AreEqual(1, list.GetPrevItemIndex(2));
            Assert.AreEqual(1, list.GetPrevItemIndex("three"));
            Assert.AreEqual(2, list.GetPrevItemIndex(3));
            Assert.AreEqual(2, list.GetPrevItemIndex("four"));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetPrevItemIndex(111));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetPrevItemIndex("twotwo"));
        }

        [Test]
        public void GetPrevItem()
        {
            Assert.AreEqual("four", list.GetPrevItem(0));
            Assert.AreEqual("four", list.GetPrevItem("one"));
            Assert.AreEqual("one", list.GetPrevItem(1));
            Assert.AreEqual("one", list.GetPrevItem("two"));
            Assert.AreEqual("two", list.GetPrevItem(2));
            Assert.AreEqual("two", list.GetPrevItem("three"));
            Assert.AreEqual("three", list.GetPrevItem(3));
            Assert.AreEqual("three", list.GetPrevItem("four"));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetPrevItem(111));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetPrevItem("twotwo"));
        }

        private readonly IList<String> list = new List<String> {"one", "two", "three", "four"};
    }
}