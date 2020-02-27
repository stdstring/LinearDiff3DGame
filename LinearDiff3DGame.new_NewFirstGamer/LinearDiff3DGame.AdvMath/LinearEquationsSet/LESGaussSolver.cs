using System;

namespace LinearDiff3DGame.AdvMath.LinearEquationsSet
{
	public class LESGaussSolver : ILinearEquationsSystemSolver
	{
		public Matrix Solve(Matrix matrixA, Matrix matrixB)
		{
			Matrix matrixError;
			return Solve(matrixA, matrixB, out matrixError);
		}

		public Matrix Solve(Matrix matrixA, Matrix matrixB, out Matrix matrixError)
		{
			if(matrixA.RowCount != matrixA.ColumnCount)
				throw new IncorrectMatrixSizeException("matrixA must be square.", "matrixA");
			if(matrixB.ColumnCount != 1)
				throw new IncorrectMatrixSizeException("matrixB must be column.", "matrixB");
			if(matrixA.RowCount != matrixB.RowCount)
				throw new IncorrectMatrixSizeException("matrixA and matrixB must have equivalent row's count.");

			throw new NotImplementedException("not implemented yet !!!");
		}
	}
}