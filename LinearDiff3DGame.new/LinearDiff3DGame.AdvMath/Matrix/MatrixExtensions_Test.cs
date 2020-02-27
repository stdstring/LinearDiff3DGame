using System;
using LinearDiff3DGame.AdvMath.MatrixUtils;
using NUnit.Framework;

namespace LinearDiff3DGame.AdvMath.Matrix
{
    [TestFixture]
    public class MatrixExtensions_Test
    {
        [SetUp]
        public void SetUp()
        {
            source = new MatrixFactory().CreateFromRawData(2, 3, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0);
        }

        [Test]
        public void GetMatrixRow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => source.GetMatrixRow(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.GetMatrixRow(3));
            MatrixComparer comparer = new MatrixComparer();
            Matrix expected = new MatrixFactory().CreateFromRawData(1, 3, 1.0, 2.0, 3.0);
            Assert.IsTrue(comparer.Equals(expected, source.GetMatrixRow(1)));
        }

        [Test]
        public void SetMatrixRow()
        {
            Matrix row1 = new MatrixFactory().CreateFromRawData(1, 3, 1.1, 2.1, 3.1);
            Matrix row2 = new MatrixFactory().CreateFromRawData(1, 4, 1.1, 2.1, 3.1, 4.1);
            Matrix square = new MatrixFactory().CreateFromRawData(2, 2, 1.1, 2.1, 3.1, 4.1);
            Assert.Throws<ArgumentOutOfRangeException>(() => source.SetMatrixRow(0, row1));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.SetMatrixRow(3, row1));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.SetMatrixRow(1, row2));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.SetMatrixRow(2, square));
            MatrixComparer comparer = new MatrixComparer();
            Matrix expected = new MatrixFactory().CreateFromRawData(2, 3, 1.1, 2.1, 3.1, 4.0, 5.0, 6.0);
            Assert.IsTrue(comparer.Equals(expected, source.SetMatrixRow(1, row1)));
        }

        [Test]
        public void SwapRows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => source.SwapRows(0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.SwapRows(3, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.SwapRows(1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.SwapRows(1, 3));
            MatrixComparer comparer = new MatrixComparer();
            Matrix expected1 = new MatrixFactory().CreateFromRawData(2, 3, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0);
            Assert.IsTrue(comparer.Equals(expected1, source.SwapRows(1, 1)));
            Matrix expected2 = new MatrixFactory().CreateFromRawData(2, 3, 4.0, 5.0, 6.0, 1.0, 2.0, 3.0);
            Assert.IsTrue(comparer.Equals(expected2, source.SwapRows(2, 1)));
        }

        [Test]
        public void SwapColumns()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => source.SwapColumns(0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.SwapColumns(4, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.SwapColumns(1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.SwapColumns(1, 4));
            MatrixComparer comparer = new MatrixComparer();
            Matrix expected1 = new MatrixFactory().CreateFromRawData(2, 3, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0);
            Assert.IsTrue(comparer.Equals(expected1, source.SwapColumns(1, 1)));
            Matrix expected2 = new MatrixFactory().CreateFromRawData(2, 3, 3.0, 2.0, 1.0, 6.0, 5.0, 4.0);
            Assert.IsTrue(comparer.Equals(expected2, source.SwapColumns(1, 3)));
        }

        [Test]
        public void Clone()
        {
            Matrix clone = source.Clone();
            Assert.AreNotEqual(source, clone);
            MatrixComparer comparer = new MatrixComparer();
            Assert.IsTrue(comparer.Equals(source, clone));
            clone[1, 1] = 101;
            Assert.IsFalse(comparer.Equals(source, clone));
        }

        private Matrix source;
    }
}