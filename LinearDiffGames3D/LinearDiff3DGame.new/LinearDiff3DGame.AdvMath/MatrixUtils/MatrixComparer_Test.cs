using System;
using LinearDiff3DGame.AdvMath.Common;
using NUnit.Framework;

namespace LinearDiff3DGame.AdvMath.MatrixUtils
{
    [TestFixture]
    public class MatrixComparer_Test
    {
        public MatrixComparer_Test()
        {
            matrixComparer = new MatrixComparer();
        }

        [Test]
        public void MatrixRowCountNotEqual()
        {
            Matrix.Matrix matrix1 = new Matrix.Matrix(1, 2);
            Matrix.Matrix matrix2 = new Matrix.Matrix(2, 2);
            Assert.Throws<ArgumentException>(() => matrixComparer.Equals(matrix1, matrix2));
            Assert.Throws<ArgumentException>(() => matrixComparer.Equals(comparer, matrix1, matrix2));
        }

        [Test]
        public void MatrixColumnCountNotEqual()
        {
            Matrix.Matrix matrix1 = new Matrix.Matrix(2, 1);
            Matrix.Matrix matrix2 = new Matrix.Matrix(2, 2);
            Assert.Throws<ArgumentException>(() => matrixComparer.Equals(matrix1, matrix2));
            Assert.Throws<ArgumentException>(() => matrixComparer.Equals(comparer, matrix1, matrix2));
        }

        [Test]
        public void CompareEqualMatrix()
        {
            Matrix.Matrix matrix1 = new Matrix.Matrix(2, 2);
            matrix1[1, 1] = 1.1;
            matrix1[1, 2] = 1.2;
            matrix1[2, 1] = 1.3;
            matrix1[2, 2] = 1.4;
            Matrix.Matrix matrix2 = new Matrix.Matrix(2, 2);
            matrix2[1, 1] = 1.1;
            matrix2[1, 2] = 1.2;
            matrix2[2, 1] = 1.3;
            matrix2[2, 2] = 1.4;
            Assert.IsTrue(matrixComparer.Equals(matrix1, matrix2));
        }

        [Test]
        public void CompareNotEqualMatrix()
        {
            Matrix.Matrix matrix1 = new Matrix.Matrix(2, 2);
            matrix1[1, 1] = 1.1;
            matrix1[1, 2] = 1.2;
            matrix1[2, 1] = 1.3;
            matrix1[2, 2] = 1.4;
            Matrix.Matrix matrix2 = new Matrix.Matrix(2, 2);
            matrix2[1, 1] = 1.1;
            matrix2[1, 2] = 1.2;
            matrix2[2, 1] = 1.3;
            matrix2[2, 2] = 1.39999999;
            Assert.IsFalse(matrixComparer.Equals(matrix1, matrix2));
        }

        [Test]
        public void ApproxCompareEqualMatrix()
        {
            Matrix.Matrix matrix1 = new Matrix.Matrix(2, 2);
            matrix1[1, 1] = 1;
            matrix1[1, 2] = 1;
            matrix1[2, 1] = 1;
            matrix1[2, 2] = 1;
            Matrix.Matrix matrix2 = new Matrix.Matrix(2, 2);
            matrix2[1, 1] = 1.0001;
            matrix2[1, 2] = 0.9991;
            matrix2[2, 1] = 1.004;
            matrix2[2, 2] = 0.995;
            Assert.IsTrue(matrixComparer.Equals(comparer, matrix1, matrix2));
        }

        [Test]
        public void ApproxCompareNotEqualMatrix()
        {
            Matrix.Matrix matrix1 = new Matrix.Matrix(2, 2);
            matrix1[1, 1] = 1;
            matrix1[1, 2] = 1;
            matrix1[2, 1] = 1;
            matrix1[2, 2] = 1;
            Matrix.Matrix matrix2 = new Matrix.Matrix(2, 2);
            matrix2[1, 1] = 1.1;
            matrix2[1, 2] = 0.91;
            matrix2[2, 1] = 1.04;
            matrix2[2, 2] = 0.95;
            Assert.IsFalse(matrixComparer.Equals(comparer, matrix1, matrix2));
        }

        private const double epsilon = 1e-2;
        private readonly ApproxComp comparer = new ApproxComp(epsilon);
        private readonly MatrixComparer matrixComparer;
    }
}