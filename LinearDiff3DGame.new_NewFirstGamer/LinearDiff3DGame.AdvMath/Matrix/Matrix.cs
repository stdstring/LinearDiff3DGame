using System;

namespace LinearDiff3DGame.AdvMath
{
	/// <summary>
	/// класс Matrix предназначен для хранения матриц NxM и элементарных манипуляций с ними
	/// </summary>
	public class Matrix
	{
		/// <summary>
		/// конструктор класса Matrix
		/// </summary>
		/// <param name="rowCount">количество строк</param>
		/// <param name="columnCount">количество столбцов</param>
		public Matrix(Int32 rowCount, Int32 columnCount)
		{
			matrixElements = new Double[rowCount,columnCount];
			RowCount = rowCount;
			ColumnCount = columnCount;
		}

		/// <summary>
		/// метод GetMatrixRow возвращает строку (в виде матрицы) матрицы
		/// </summary>
		/// <param name="rowIndex">номер (индекс) возвращаемой строки</param>
		/// <returns>возвращаемая строка (в виде матрицы)</returns>
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
		/// метод SetMatrixRow сохраняет строку (заданную в виде матрицы) в строке исходной матрицы с номером (индексом) rowIndex
		/// </summary>
		/// <param name="rowIndex">номер (индекс) строки исходной матрицы</param>
		/// <param name="rowMatrix">строка (заданная в виде матрицы), которую надо сохранить в исходной матрице</param>
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
		/// меняем местами две строки в матрице
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
		/// меняем местами два столбца в матрице
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
		/// метод Clone возвращает полную копию (клон) матрицы
		/// </summary>
		/// <returns>полная копия (клон) матрицы</returns>
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
		/// единичная матрица размером (matrixSize * matrixSize)
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
		/// метод MatrixAddition возвращает результат сложения матриц matrix1 и matrix2
		/// </summary>
		/// <param name="matrix1">матрица matrix1</param>
		/// <param name="matrix2">матрица matrix2</param>
		/// <returns>результат сложения матриц matrix1 и matrix2</returns>
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
		/// метод MatrixSubtraction возвращает результат разности матриц matrix1 и matrix2
		/// </summary>
		/// <param name="matrix1">матрица matrix1</param>
		/// <param name="matrix2">матрица matrix2</param>
		/// <returns>результат разности матриц matrix1 и matrix2</returns>
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
		/// метод MatrixMultiplication возвращает результат умножения матриц matrix1 и matrix2
		/// </summary>
		/// <param name="matrix1">матрица matrix1</param>
		/// <param name="matrix2">матрица matrix2</param>
		/// <returns>результат умножения матриц matrix1 и matrix2</returns>
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
		/// метод MatrixMultiplication возвращает результат умножения матрицы matrix на число number
		/// </summary>
		/// <param name="matrix">матрица matrix</param>
		/// <param name="number">число number</param>
		/// <returns>результат умножения матрицы matrix на число number</returns>
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
		/// метод MatrixTransposing возвращает результат транспонирования матрицы matrix
		/// </summary>
		/// <param name="matrix">исходная матрица matrix</param>
		/// <returns>результат транспонирования матрицы matrix</returns>
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
		/// оператор сложения матриц matrix1 и matrix2
		/// </summary>
		/// <param name="matrix1">матрица matrix1</param>
		/// <param name="matrix2">матрица matrix2</param>
		/// <returns>результат сложения матриц matrix1 и matrix2</returns>
		public static Matrix operator +(Matrix matrix1, Matrix matrix2)
		{
			return MatrixAddition(matrix1, matrix2);
		}

		/// <summary>
		/// оператор вычитания матриц matrix1 и matrix2
		/// </summary>
		/// <param name="matrix1">матрица matrix1</param>
		/// <param name="matrix2">матрица matrix2</param>
		/// <returns>результат вычитания матриц matrix1 и matrix2</returns>
		public static Matrix operator -(Matrix matrix1, Matrix matrix2)
		{
			return MatrixSubtraction(matrix1, matrix2);
		}

		/// <summary>
		/// оператор умножения матриц matrix1 и matrix2
		/// </summary>
		/// <param name="matrix1">матрица matrix1</param>
		/// <param name="matrix2">матрица matrix2</param>
		/// <returns>результат умножения матриц matrix1 и matrix2</returns>
		public static Matrix operator *(Matrix matrix1, Matrix matrix2)
		{
			return MatrixMultiplication(matrix1, matrix2);
		}

		/// <summary>
		/// оператор умножения матрицы matrix на число number
		/// </summary>
		/// <param name="number">число number</param>
		/// <param name="matrix">матрица matrix</param>
		/// <returns>результат умножения матрицы matrix на число number</returns>
		public static Matrix operator *(Double number, Matrix matrix)
		{
			return MatrixMultiplication(matrix, number);
		}

		/// <summary>
		/// свойство (индексатор) для доступа к элементам матрицы
		/// номер строки изменяется в диапазоне 1...RowCount
		/// номер столбца изменяется в диапазоне 1...ColumnCount
		/// </summary>
		/// <param name="rowIndex">номер (индекс) строки</param>
		/// <param name="columnIndex">номер (индекс) столбца</param>
		/// <returns>элемент матрицы</returns>
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
		/// свойтво RowCount возвращает количество строк
		/// </summary>
		public int RowCount { get; private set; }

		/// <summary>
		/// свойтво RowCount возвращает количество столбцов
		/// </summary>
		public int ColumnCount { get; private set; }

		/// <summary>
		/// массив для хранения элементов матрицы
		/// </summary>
		private readonly Double[,] matrixElements;
	}
}