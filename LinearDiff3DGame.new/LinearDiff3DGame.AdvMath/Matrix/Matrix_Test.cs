using System;
using LinearDiff3DGame.AdvMath.MatrixUtils;
using NUnit.Framework;

namespace LinearDiff3DGame.AdvMath.Matrix
{
    [TestFixture]
    public class Matrix_Test
    {
        [Test]
        public void Elements()
        {
            Matrix matrix = new Matrix(2, 3);
            Assert.AreEqual(2, matrix.RowCount);
            Assert.AreEqual(3, matrix.ColumnCount);
            Assert.AreEqual(0, matrix[1, 1]);
            matrix[1, 1] = 10;
            Assert.AreEqual(10, matrix[1, 1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => matrix[0, 1] = 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => matrix[3, 1] = 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => matrix[1, 0] = 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => matrix[1, 4] = 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => Assert.AreEqual(0, matrix[0, 1]));
            Assert.Throws<ArgumentOutOfRangeException>(() => Assert.AreEqual(0, matrix[3, 1]));
            Assert.Throws<ArgumentOutOfRangeException>(() => Assert.AreEqual(0, matrix[1, 0]));
            Assert.Throws<ArgumentOutOfRangeException>(() => Assert.AreEqual(0, matrix[1, 4]));
        }

        [Test]
        public void IdentityMatrix()
        {
            Matrix identityMatrix = new MatrixFactory().CreateFromRawData(2, 2, 1.0, 0.0, 0.0, 1.0);
            Assert.IsTrue(comparer.Equals(identityMatrix, Matrix.IdentityMatrix(2)));
        }

        [Test]
        public void MatrixAddition()
        {
            Assert.Throws<IncorrectMatrixSizeException>(() => Matrix.MatrixAddition(matrix1, matrix2));
            Matrix expected = new MatrixFactory().CreateFromRawData(2, 3, 2.0, 4.0, 6.0, 8.0, 10.0, 12.0);
            Assert.IsTrue(comparer.Equals(expected, Matrix.MatrixAddition(matrix1, matrix1)));
            Assert.IsTrue(comparer.Equals(expected, matrix1 + matrix1));
        }

        [Test]
        public void MatrixSubtraction()
        {
            Assert.Throws<IncorrectMatrixSizeException>(() => Matrix.MatrixSubtraction(matrix1, matrix2));
            Matrix expected = new MatrixFactory().CreateFromRawData(2, 3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            Assert.IsTrue(comparer.Equals(expected, Matrix.MatrixSubtraction(matrix1, matrix1)));
            Assert.IsTrue(comparer.Equals(expected, matrix1 - matrix1));
        }

        [Test]
        public void MatrixMultiplication()
        {
            Assert.Throws<IncorrectMatrixSizeException>(() => Matrix.MatrixMultiplication(matrix1, matrix1));
            Matrix expected1 = new MatrixFactory().CreateFromRawData(2, 2, 14.0, 32.0, 32.0, 77.0);
            Assert.IsTrue(comparer.Equals(expected1, Matrix.MatrixMultiplication(matrix1, matrix2)));
            Assert.IsTrue(comparer.Equals(expected1, matrix1*matrix2));
            Matrix expected2 = new MatrixFactory().CreateFromRawData(2, 3, 2.0, 4.0, 6.0, 8.0, 10.0, 12.0);
            Assert.IsTrue(comparer.Equals(expected2, Matrix.MatrixMultiplication(matrix1, 2)));
            Assert.IsTrue(comparer.Equals(expected2, 2*matrix1));
        }

        [Test]
        public void MatrixTransposing()
        {
            Assert.IsTrue(comparer.Equals(matrix2, Matrix.MatrixTransposing(matrix1)));
        }

        private readonly MatrixComparer comparer = new MatrixComparer();
        private readonly Matrix matrix1 = new MatrixFactory().CreateFromRawData(2, 3, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0);
        private readonly Matrix matrix2 = new MatrixFactory().CreateFromRawData(3, 2, 1.0, 4.0, 2.0, 5.0, 3.0, 6.0);
    }
}