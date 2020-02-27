using System;

namespace LinearDiff3DGame.AdvMath
{
	/// <summary>
	/// ����� Matrix ������������ ��� �������� ������ NxM � ������������ ����������� � ����
	/// </summary>
	public class Matrix
	{
		/// <summary>
		/// ����������� ������ Matrix
		/// </summary>
		/// <param name="rowCount">���������� �����</param>
		/// <param name="columnCount">���������� ��������</param>
		public Matrix(Int32 rowCount, Int32 columnCount)
		{
			matrixElements = new Double[rowCount,columnCount];
			RowCount = rowCount;
			ColumnCount = columnCount;
		}

		/// <summary>
		/// ����� GetMatrixRow ���������� ������ (� ���� �������) �������
		/// </summary>
		/// <param name="rowIndex">����� (������) ������������ ������</param>
		/// <returns>������������ ������ (� ���� �������)</returns>
		public Matrix GetMatrixRow(Int32 rowIndex)
		{
			if((rowIndex < 1) || (rowIndex > RowCount))
				throw new ArgumentOutOfRangeException("Row index must be between 0 and row's count");

			Matrix rowMatrix = new Matrix(1, ColumnCount);

			for(Int32 columnIndex = 1; columnIndex <= ColumnCount; ++columnIndex)
				rowMatrix[1, columnIndex] = this[rowIndex, columnIndex];

			return rowMatrix;
		}

		/// <summary>
		/// ����� SetMatrixRow ��������� ������ (�������� � ���� �������) � ������ �������� ������� � ������� (��������) rowIndex
		/// </summary>
		/// <param name="rowIndex">����� (������) ������ �������� �������</param>
		/// <param name="rowMatrix">������ (�������� � ���� �������), ������� ���� ��������� � �������� �������</param>
		public void SetMatrixRow(Int32 rowIndex, Matrix rowMatrix)
		{
			if((rowIndex < 1) || (rowIndex > RowCount))
				throw new ArgumentOutOfRangeException("Row index must be between 0 and row's count");
			if(rowMatrix.RowCount > 1)
				throw new ArgumentOutOfRangeException("RowMatrix isn't the row");
			if(ColumnCount != rowMatrix.ColumnCount)
				throw new ArgumentOutOfRangeException("Column's count of RowMatrix should be equaled to column's count of this matrix");

			for(Int32 columnIndex = 1; columnIndex <= ColumnCount; ++columnIndex)
				this[rowIndex, columnIndex] = rowMatrix[1, columnIndex];
		}

		/// <summary>
		/// ������ ������� ��� ������ � �������
		/// </summary>
		/// <param name="rowIndex1"></param>
		/// <param name="rowIndex2"></param>
		public void SwapRows(Int32 rowIndex1, Int32 rowIndex2)
		{
			if(rowIndex1 == rowIndex2) return;

			for(Int32 columnIndex = 1; columnIndex <= ColumnCount; ++columnIndex)
			{
				Double temp = this[rowIndex1, columnIndex];
				this[rowIndex1, columnIndex] = this[rowIndex2, columnIndex];
				this[rowIndex2, columnIndex] = temp;
			}
		}

		/// <summary>
		/// ������ ������� ��� ������� � �������
		/// </summary>
		/// <param name="columnIndex1"></param>
		/// <param name="columnIndex2"></param>
		public void SwapColumns(Int32 columnIndex1, Int32 columnIndex2)
		{
			if(columnIndex1 == columnIndex2) return;

			for(Int32 rowIndex = 1; rowIndex <= ColumnCount; ++rowIndex)
			{
				Double temp = this[rowIndex, columnIndex1];
				this[rowIndex, columnIndex1] = this[rowIndex, columnIndex2];
				this[rowIndex, columnIndex2] = temp;
			}
		}

		/// <summary>
		/// ����� Clone ���������� ������ ����� (����) �������
		/// </summary>
		/// <returns>������ ����� (����) �������</returns>
		public Matrix Clone()
		{
			Matrix clone = new Matrix(RowCount, ColumnCount);

			for(Int32 rowIndex = 1; rowIndex <= RowCount; ++rowIndex)
			{
				for(Int32 columnIndex = 1; columnIndex <= ColumnCount; ++columnIndex)
					clone[rowIndex, columnIndex] = this[rowIndex, columnIndex];
			}

			return clone;
		}

		/// <summary>
		/// ��������� ������� �������� (matrixSize * matrixSize)
		/// </summary>
		/// <param name="matrixSize"></param>
		/// <returns></returns>
		public static Matrix IdentityMatrix(Int32 matrixSize)
		{
			Matrix identityMatrix = new Matrix(matrixSize, matrixSize);

			for(Int32 rowIndex = 1; rowIndex <= matrixSize; ++rowIndex)
			{
				for(Int32 columnIndex = 1; columnIndex <= matrixSize; ++columnIndex)
					identityMatrix[rowIndex, columnIndex] = (rowIndex == columnIndex ? 1.0 : 0.0);
			}

			return identityMatrix;
		}

		/// <summary>
		/// ����� MatrixAddition ���������� ��������� �������� ������ matrix1 � matrix2
		/// </summary>
		/// <param name="matrix1">������� matrix1</param>
		/// <param name="matrix2">������� matrix2</param>
		/// <returns>��������� �������� ������ matrix1 � matrix2</returns>
		public static Matrix MatrixAddition(Matrix matrix1, Matrix matrix2)
		{
			if(matrix1.RowCount != matrix2.RowCount || matrix1.ColumnCount != matrix2.ColumnCount)
				throw new IncorrectMatrixSizeException("Size of matrix1 and matrix2 must be equals");

			Matrix resultMatrix = new Matrix(matrix1.RowCount, matrix1.ColumnCount);

			for(Int32 rowIndex = 1; rowIndex <= resultMatrix.RowCount; ++rowIndex)
			{
				for(Int32 columnIndex = 1; columnIndex <= resultMatrix.ColumnCount; ++columnIndex)
					resultMatrix[rowIndex, columnIndex] = matrix1[rowIndex, columnIndex] + matrix2[rowIndex, columnIndex];
			}

			return resultMatrix;
		}

		/// <summary>
		/// ����� MatrixSubtraction ���������� ��������� �������� ������ matrix1 � matrix2
		/// </summary>
		/// <param name="matrix1">������� matrix1</param>
		/// <param name="matrix2">������� matrix2</param>
		/// <returns>��������� �������� ������ matrix1 � matrix2</returns>
		public static Matrix MatrixSubtraction(Matrix matrix1, Matrix matrix2)
		{
			if(matrix1.RowCount != matrix2.RowCount || matrix1.ColumnCount != matrix2.ColumnCount)
				throw new IncorrectMatrixSizeException("Size of matrix1 and matrix2 must be equals");

			Matrix resultMatrix = new Matrix(matrix1.RowCount, matrix1.ColumnCount);

			for(Int32 rowIndex = 1; rowIndex <= resultMatrix.RowCount; ++rowIndex)
			{
				for(Int32 columnIndex = 1; columnIndex <= resultMatrix.ColumnCount; ++columnIndex)
					resultMatrix[rowIndex, columnIndex] = matrix1[rowIndex, columnIndex] - matrix2[rowIndex, columnIndex];
			}

			return resultMatrix;
		}

		/// <summary>
		/// ����� MatrixMultiplication ���������� ��������� ��������� ������ matrix1 � matrix2
		/// </summary>
		/// <param name="matrix1">������� matrix1</param>
		/// <param name="matrix2">������� matrix2</param>
		/// <returns>��������� ��������� ������ matrix1 � matrix2</returns>
		public static Matrix MatrixMultiplication(Matrix matrix1, Matrix matrix2)
		{
			if(matrix1.ColumnCount != matrix2.RowCount)
				throw new IncorrectMatrixSizeException("matrix1 and matrix2 column's count must be equals");

			Matrix resultMatrix = new Matrix(matrix1.RowCount, matrix2.ColumnCount);

			for(Int32 rowIndex = 1; rowIndex <= resultMatrix.RowCount; ++rowIndex)
			{
				for(Int32 columnIndex = 1; columnIndex <= resultMatrix.ColumnCount; ++columnIndex)
				{
					resultMatrix[rowIndex, columnIndex] = 0;
					for(Int32 internalIndex = 1; internalIndex <= matrix1.ColumnCount; ++internalIndex)
						resultMatrix[rowIndex, columnIndex] += matrix1[rowIndex, internalIndex] * matrix2[internalIndex, columnIndex];
				}
			}

			return resultMatrix;
		}

		/// <summary>
		/// ����� MatrixMultiplication ���������� ��������� ��������� ������� matrix �� ����� number
		/// </summary>
		/// <param name="matrix">������� matrix</param>
		/// <param name="number">����� number</param>
		/// <returns>��������� ��������� ������� matrix �� ����� number</returns>
		public static Matrix MatrixMultiplication(Matrix matrix, Double number)
		{
			Matrix resultMatrix = new Matrix(matrix.RowCount, matrix.ColumnCount);

			for(Int32 rowIndex = 1; rowIndex <= resultMatrix.RowCount; ++rowIndex)
			{
				for(Int32 columnIndex = 1; columnIndex <= resultMatrix.ColumnCount; ++columnIndex)
					resultMatrix[rowIndex, columnIndex] = matrix[rowIndex, columnIndex] * number;
			}

			return resultMatrix;
		}

		/// <summary>
		/// ����� MatrixTransposing ���������� ��������� ���������������� ������� matrix
		/// </summary>
		/// <param name="matrix">�������� ������� matrix</param>
		/// <returns>��������� ���������������� ������� matrix</returns>
		public static Matrix MatrixTransposing(Matrix matrix)
		{
			Matrix resultMatrix = new Matrix(matrix.ColumnCount, matrix.RowCount);

			for(Int32 rowIndex = 1; rowIndex <= resultMatrix.RowCount; ++rowIndex)
			{
				for(Int32 columnIndex = 1; columnIndex <= resultMatrix.ColumnCount; ++columnIndex)
					resultMatrix[rowIndex, columnIndex] = matrix[columnIndex, rowIndex];
			}

			return resultMatrix;
		}

		/// <summary>
		/// �������� �������� ������ matrix1 � matrix2
		/// </summary>
		/// <param name="matrix1">������� matrix1</param>
		/// <param name="matrix2">������� matrix2</param>
		/// <returns>��������� �������� ������ matrix1 � matrix2</returns>
		public static Matrix operator +(Matrix matrix1, Matrix matrix2)
		{
			return MatrixAddition(matrix1, matrix2);
		}

		/// <summary>
		/// �������� ��������� ������ matrix1 � matrix2
		/// </summary>
		/// <param name="matrix1">������� matrix1</param>
		/// <param name="matrix2">������� matrix2</param>
		/// <returns>��������� ��������� ������ matrix1 � matrix2</returns>
		public static Matrix operator -(Matrix matrix1, Matrix matrix2)
		{
			return MatrixSubtraction(matrix1, matrix2);
		}

		/// <summary>
		/// �������� ��������� ������ matrix1 � matrix2
		/// </summary>
		/// <param name="matrix1">������� matrix1</param>
		/// <param name="matrix2">������� matrix2</param>
		/// <returns>��������� ��������� ������ matrix1 � matrix2</returns>
		public static Matrix operator *(Matrix matrix1, Matrix matrix2)
		{
			return MatrixMultiplication(matrix1, matrix2);
		}

		/// <summary>
		/// �������� ��������� ������� matrix �� ����� number
		/// </summary>
		/// <param name="number">����� number</param>
		/// <param name="matrix">������� matrix</param>
		/// <returns>��������� ��������� ������� matrix �� ����� number</returns>
		public static Matrix operator *(Double number, Matrix matrix)
		{
			return MatrixMultiplication(matrix, number);
		}

		/// <summary>
		/// �������� (����������) ��� ������� � ��������� �������
		/// ����� ������ ���������� � ��������� 1...RowCount
		/// ����� ������� ���������� � ��������� 1...ColumnCount
		/// </summary>
		/// <param name="rowIndex">����� (������) ������</param>
		/// <param name="columnIndex">����� (������) �������</param>
		/// <returns>������� �������</returns>
		public Double this[Int32 rowIndex, Int32 columnIndex]
		{
			get
			{
				if((rowIndex < 1) || (rowIndex > RowCount))
					throw new ArgumentOutOfRangeException("Row index must be between 1 and row's count-1");
				if((columnIndex < 1) || (columnIndex > ColumnCount))
					throw new ArgumentOutOfRangeException("Column index must be between 1 and column's count-1");

				return matrixElements[(rowIndex - 1), (columnIndex - 1)];
			}
			set
			{
				if((rowIndex < 1) || (rowIndex > RowCount))
					throw new ArgumentOutOfRangeException("Row index must be between 1 and row's count-1");
				if((columnIndex < 1) || (columnIndex > ColumnCount))
					throw new ArgumentOutOfRangeException("Column index must be between 1 and column's count-1");

				matrixElements[(rowIndex - 1), (columnIndex - 1)] = value;
			}
		}

		/// <summary>
		/// ������� RowCount ���������� ���������� �����
		/// </summary>
		public int RowCount { get; private set; }

		/// <summary>
		/// ������� RowCount ���������� ���������� ��������
		/// </summary>
		public int ColumnCount { get; private set; }

		/// <summary>
		/// ������ ��� �������� ��������� �������
		/// </summary>
		private readonly Double[,] matrixElements;
	}
}