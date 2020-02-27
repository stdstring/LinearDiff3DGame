using System;

namespace LinearDiff3DGame.AdvMath.Matrix
{
    public class Matrix
    {
        public Matrix(Int32 rowCount, Int32 columnCount)
        {
            matrixElements = new Double[rowCount,columnCount];
        }

        // номер строки изменяется в диапазоне 1...RowCount
        // номер столбца изменяется в диапазоне 1...ColumnCount
        public Double this[Int32 rowIndex, Int32 columnIndex]
        {
            get
            {
                if((rowIndex < 1) || (rowIndex > RowCount))
                    throw new ArgumentOutOfRangeException("rowIndex");
                if((columnIndex < 1) || (columnIndex > ColumnCount))
                    throw new ArgumentOutOfRangeException("columnIndex");
                return matrixElements[(rowIndex - 1), (columnIndex - 1)];
            }
            set
            {
                if((rowIndex < 1) || (rowIndex > RowCount))
                    throw new ArgumentOutOfRangeException("rowIndex");
                if((columnIndex < 1) || (columnIndex > ColumnCount))
                    throw new ArgumentOutOfRangeException("columnIndex");
                matrixElements[(rowIndex - 1), (columnIndex - 1)] = value;
            }
        }

        public int RowCount
        {
            get { return matrixElements.GetLength(0); }
        }

        public int ColumnCount
        {
            get { return matrixElements.GetLength(1); }
        }

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

        public static Matrix MatrixAddition(Matrix matrix1, Matrix matrix2)
        {
            if(matrix1.RowCount != matrix2.RowCount || matrix1.ColumnCount != matrix2.ColumnCount)
                throw new IncorrectMatrixSizeException("Size of matrix1 and matrix2 must be equals");
            Matrix resultMatrix = new Matrix(matrix1.RowCount, matrix1.ColumnCount);
            for(Int32 rowIndex = 1; rowIndex <= resultMatrix.RowCount; ++rowIndex)
            {
                for(Int32 columnIndex = 1; columnIndex <= resultMatrix.ColumnCount; ++columnIndex)
                    resultMatrix[rowIndex, columnIndex] = matrix1[rowIndex, columnIndex] +
                                                          matrix2[rowIndex, columnIndex];
            }
            return resultMatrix;
        }

        public static Matrix MatrixSubtraction(Matrix matrix1, Matrix matrix2)
        {
            if(matrix1.RowCount != matrix2.RowCount || matrix1.ColumnCount != matrix2.ColumnCount)
                throw new IncorrectMatrixSizeException("Size of matrix1 and matrix2 must be equals");
            Matrix resultMatrix = new Matrix(matrix1.RowCount, matrix1.ColumnCount);
            for(Int32 rowIndex = 1; rowIndex <= resultMatrix.RowCount; ++rowIndex)
            {
                for(Int32 columnIndex = 1; columnIndex <= resultMatrix.ColumnCount; ++columnIndex)
                    resultMatrix[rowIndex, columnIndex] = matrix1[rowIndex, columnIndex] -
                                                          matrix2[rowIndex, columnIndex];
            }
            return resultMatrix;
        }

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
                        resultMatrix[rowIndex, columnIndex] += matrix1[rowIndex, internalIndex]*
                                                               matrix2[internalIndex, columnIndex];
                }
            }
            return resultMatrix;
        }

        public static Matrix MatrixMultiplication(Matrix matrix, Double number)
        {
            Matrix resultMatrix = new Matrix(matrix.RowCount, matrix.ColumnCount);
            for(Int32 rowIndex = 1; rowIndex <= resultMatrix.RowCount; ++rowIndex)
            {
                for(Int32 columnIndex = 1; columnIndex <= resultMatrix.ColumnCount; ++columnIndex)
                    resultMatrix[rowIndex, columnIndex] = matrix[rowIndex, columnIndex]*number;
            }
            return resultMatrix;
        }

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

        public static Matrix operator +(Matrix matrix1, Matrix matrix2)
        {
            return MatrixAddition(matrix1, matrix2);
        }

        public static Matrix operator -(Matrix matrix1, Matrix matrix2)
        {
            return MatrixSubtraction(matrix1, matrix2);
        }

        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            return MatrixMultiplication(matrix1, matrix2);
        }

        public static Matrix operator *(Double number, Matrix matrix)
        {
            return MatrixMultiplication(matrix, number);
        }

        public static Matrix operator *(Matrix matrix, Double number)
        {
            return MatrixMultiplication(matrix, number);
        }

        private readonly Double[,] matrixElements;
    }
}