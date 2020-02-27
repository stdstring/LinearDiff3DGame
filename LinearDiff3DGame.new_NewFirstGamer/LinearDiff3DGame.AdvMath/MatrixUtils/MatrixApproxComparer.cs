using System;
using LinearDiff3DGame.AdvMath.Common;

namespace LinearDiff3DGame.AdvMath.MatrixUtils
{
	public class MatrixApproxComparer
	{
		public MatrixApproxComparer(ApproxComp comparer)
		{
			this.comparer = comparer;
		}

		public Boolean Equals(Matrix matrix1, Matrix matrix2)
		{
			if (matrix1.RowCount != matrix2.RowCount || matrix1.ColumnCount != matrix2.ColumnCount)
				throw new ArgumentException("Не совпадают размеры матриц");
			for (Int32 rowIndex = 1; rowIndex <= matrix1.RowCount; ++rowIndex)
				for (Int32 columnIndex = 1; columnIndex <= matrix1.ColumnCount; ++columnIndex)
					if (comparer.NotEqual(matrix1[rowIndex, columnIndex], matrix2[rowIndex, columnIndex])) return false;
			return true;
		}

		private readonly ApproxComp comparer;
	}
}
