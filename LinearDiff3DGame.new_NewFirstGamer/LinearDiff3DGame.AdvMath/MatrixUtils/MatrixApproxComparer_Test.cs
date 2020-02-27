using System;
using LinearDiff3DGame.AdvMath.Common;
using NUnit.Framework;

namespace LinearDiff3DGame.AdvMath.MatrixUtils
{
	[TestFixture]
	public class MatrixApproxComparer_Test
	{
		public MatrixApproxComparer_Test()
		{
			matrixComparer = new MatrixApproxComparer(comparer);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void MatrixRowCountNotEqual()
		{
			Matrix matrix1 = new Matrix(1, 2);
			Matrix matrix2 = new Matrix(2, 2);
			matrixComparer.Equals(matrix1, matrix2);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void MatrixColumnCountNotEqual()
		{
			Matrix matrix1 = new Matrix(2, 1);
			Matrix matrix2 = new Matrix(2, 2);
			matrixComparer.Equals(matrix1, matrix2);
		}

		[Test]
		public void CompareEqualMatrix()
		{
			Matrix matrix1 = new Matrix(2, 2);
			matrix1[1, 1] = 1;
			matrix1[1, 2] = 1;
			matrix1[2, 1] = 1;
			matrix1[2, 2] = 1;
			Matrix matrix2 = new Matrix(2, 2);
			matrix2[1, 1] = 1.0001;
			matrix2[1, 2] = 0.9991;
			matrix2[2, 1] = 1.004;
			matrix2[2, 2] = 0.995;
			Assert.IsTrue(matrixComparer.Equals(matrix1, matrix2));
		}

		[Test]
		public void CompareNotEqualMatrix()
		{
			Matrix matrix1 = new Matrix(2, 2);
			matrix1[1, 1] = 1;
			matrix1[1, 2] = 1;
			matrix1[2, 1] = 1;
			matrix1[2, 2] = 1;
			Matrix matrix2 = new Matrix(2, 2);
			matrix2[1, 1] = 1.1;
			matrix2[1, 2] = 0.91;
			matrix2[2, 1] = 1.04;
			matrix2[2, 2] = 0.95;
			Assert.IsFalse(matrixComparer.Equals(matrix1, matrix2));
		}

		private const double epsilon = 1e-2;
		private readonly ApproxComp comparer = new ApproxComp(epsilon);
		private readonly MatrixApproxComparer matrixComparer;
	}
}
