using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.AdvMath
{
    /// <summary>
    /// ����� Matrix ������������ ��� �������� ������ NxM � ������������ ����������� � ����
    /// </summary>
    public class Matrix : ICloneable
    {
        /// <summary>
        /// ����������� ������ Matrix
        /// </summary>
        /// <param name="rowCount">���������� �����</param>
        /// <param name="columnCount">���������� ��������</param>
        public Matrix(Int32 rowCount, Int32 columnCount)
        {
            m_MatrixElements = new Double[rowCount, columnCount];
            m_RowCount = rowCount;
            m_ColumnCount = columnCount;
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
                if ((rowIndex < 1) || (rowIndex > m_RowCount))
                {
#warning ����� ����� ������������������ ����������
                    throw new ArgumentOutOfRangeException("Row index must be between 0 and row's count");
                }
                if ((columnIndex < 1) || (columnIndex > m_ColumnCount))
                {
#warning ����� ����� ������������������ ����������
                    throw new ArgumentOutOfRangeException("Column index must be between 0 and column's count");
                }

                return m_MatrixElements[(rowIndex - 1), (columnIndex - 1)];
            }
            set
            {
                if ((rowIndex < 1) || (rowIndex > m_RowCount))
                {
#warning ����� ����� ������������������ ����������
                    throw new ArgumentOutOfRangeException("Row index must be between 0 and row's count");
                }
                if ((columnIndex < 1) || (columnIndex > m_ColumnCount))
                {
#warning ����� ����� ������������������ ����������
                    throw new ArgumentOutOfRangeException("Column index must be between 0 and column's count");
                }

                m_MatrixElements[(rowIndex - 1), (columnIndex - 1)] = value;
            }
        }

        /// <summary>
        /// ������� RowCount ���������� ���������� �����
        /// </summary>
        public Int32 RowCount
        {
            get
            {
                return m_RowCount;
            }
        }

        /// <summary>
        /// ������� RowCount ���������� ���������� ��������
        /// </summary>
        public Int32 ColumnCount
        {
            get
            {
                return m_ColumnCount;
            }
        }

        /// <summary>
        /// ����� GetMatrixRow ���������� ������ (� ���� �������) �������
        /// </summary>
        /// <param name="rowIndex">����� (������) ������������ ������</param>
        /// <returns>������������ ������ (� ���� �������)</returns>
        public Matrix GetMatrixRow(Int32 rowIndex)
        {
            if ((rowIndex < 1) || (rowIndex > m_RowCount))
            {
#warning ����� ����� ������������������ ����������
                throw new ArgumentOutOfRangeException("Row index must be between 0 and row's count");
            }

            Matrix rowMatrix = new Matrix(1, m_ColumnCount);

            for (Int32 columnIndex = 1; columnIndex <= m_ColumnCount; ++columnIndex)
            {
                rowMatrix[1, columnIndex] = this[rowIndex, columnIndex];
            }

            return rowMatrix;
        }

        /// <summary>
        /// ����� SetMatrixRow ��������� ������ (�������� � ���� �������) � ������ �������� ������� � ������� (��������) rowIndex
        /// </summary>
        /// <param name="rowIndex">����� (������) ������ �������� �������</param>
        /// <param name="rowMatrix">������ (�������� � ���� �������), ������� ���� ��������� � �������� �������</param>
        public void SetMatrixRow(Int32 rowIndex, Matrix rowMatrix)
        {
            if ((rowIndex < 1) || (rowIndex > m_RowCount))
            {
#warning ����� ����� ������������������ ����������
                throw new ArgumentOutOfRangeException("Row index must be between 0 and row's count");
            }
            if (rowMatrix.RowCount > 1)
            {
#warning ����� ����� ������������������ ����������
                throw new ArgumentOutOfRangeException("RowMatrix isn't the row");
            }
            if (m_ColumnCount != rowMatrix.ColumnCount)
            {
#warning ����� ����� ������������������ ����������
                throw new ArgumentOutOfRangeException("Column's count of RowMatrix should be equaled to column's count of this matrix");
            }

            for (Int32 columnIndex = 1; columnIndex <= m_ColumnCount; ++columnIndex)
            {
                this[rowIndex, columnIndex] = rowMatrix[1, columnIndex];
            }
        }

        /// <summary>
        /// ����� Clone ���������� ������ ����� (����) �������
        /// </summary>
        /// <returns>������ ����� (����) �������</returns>
        public Matrix Clone()
        {
            Matrix clone = new Matrix(m_RowCount, m_ColumnCount);

            for (Int32 rowIndex = 1; rowIndex <= m_RowCount; ++rowIndex)
            {
                for (Int32 columnIndex = 1; columnIndex <= m_ColumnCount; ++columnIndex)
                {
                    clone[rowIndex, columnIndex] = this[rowIndex, columnIndex];
                }
            }

            return clone;
        }

        #region ICloneable Members

        Object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

        /// <summary>
        /// ����� MatrixAddition ���������� ��������� �������� ������ matrix1 � matrix2
        /// </summary>
        /// <param name="matrix1">������� matrix1</param>
        /// <param name="matrix2">������� matrix2</param>
        /// <returns>��������� �������� ������ matrix1 � matrix2</returns>
        public static Matrix MatrixAddition(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.RowCount != matrix2.RowCount)
            {
#warning ����� ����� ������������������ ����������
                throw new ArgumentOutOfRangeException("Row's count of the first matrix should be equaled to row's count of the second matrix");
            }
            if (matrix1.ColumnCount != matrix2.ColumnCount)
            {
#warning ����� ����� ������������������ ����������
                throw new ArgumentOutOfRangeException("Column's count of the first matrix should be equaled to column's count of the second matrix");
            }

            Matrix resultMatrix = new Matrix(matrix1.RowCount, matrix1.ColumnCount);

            for (Int32 rowIndex = 1; rowIndex <= resultMatrix.RowCount; ++rowIndex)
            {
                for (Int32 columnIndex = 1; columnIndex <= resultMatrix.ColumnCount; ++columnIndex)
                {
                    resultMatrix[rowIndex, columnIndex] = matrix1[rowIndex, columnIndex] + matrix2[rowIndex, columnIndex];
                }
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
            if (matrix1.RowCount != matrix2.RowCount)
            {
#warning ����� ����� ������������������ ����������
                throw new ArgumentOutOfRangeException("Row's count of the first matrix should be equaled to row's count of the second matrix");
            }
            if (matrix1.ColumnCount != matrix2.ColumnCount)
            {
#warning ����� ����� ������������������ ����������
                throw new ArgumentOutOfRangeException("Column's count of the first matrix should be equaled to column's count of the second matrix");
            }

            Matrix resultMatrix = new Matrix(matrix1.RowCount, matrix1.ColumnCount);

            for (Int32 rowIndex = 1; rowIndex <= resultMatrix.RowCount; ++rowIndex)
            {
                for (Int32 columnIndex = 1; columnIndex <= resultMatrix.ColumnCount; ++columnIndex)
                {
                    resultMatrix[rowIndex, columnIndex] = matrix1[rowIndex, columnIndex] - matrix2[rowIndex, columnIndex];
                }
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
            if (matrix1.ColumnCount != matrix2.RowCount)
            {
#warning ����� ����� ������������������ ����������
                throw new ArgumentOutOfRangeException("Column's count of the first matrix should be equaled to row's count of the second matrix");
            }

            Matrix resultMatrix = new Matrix(matrix1.RowCount, matrix2.ColumnCount);

            for (Int32 rowIndex = 1; rowIndex <= resultMatrix.RowCount; ++rowIndex)
            {
                for (Int32 columnIndex = 1; columnIndex <= resultMatrix.ColumnCount; ++columnIndex)
                {
                    resultMatrix[rowIndex, columnIndex] = 0;
                    for (Int32 internalIndex = 1; internalIndex <= matrix1.ColumnCount; ++internalIndex)
                    {
                        resultMatrix[rowIndex, columnIndex] += matrix1[rowIndex, internalIndex] * matrix2[internalIndex, columnIndex];
                    }
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

            for (Int32 rowIndex = 1; rowIndex <= resultMatrix.RowCount; ++rowIndex)
            {
                for (Int32 columnIndex = 1; columnIndex <= resultMatrix.ColumnCount; ++columnIndex)
                {
                    resultMatrix[rowIndex, columnIndex] = matrix[rowIndex, columnIndex] * number;
                }
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

            for (Int32 rowIndex = 1; rowIndex <= resultMatrix.RowCount; ++rowIndex)
            {
                for (Int32 columnIndex = 1; columnIndex <= resultMatrix.ColumnCount; ++columnIndex)
                {
                    resultMatrix[rowIndex, columnIndex] = matrix[columnIndex, rowIndex];
                }
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
            return Matrix.MatrixAddition(matrix1, matrix2);
        }

        /// <summary>
        /// �������� ��������� ������ matrix1 � matrix2
        /// </summary>
        /// <param name="matrix1">������� matrix1</param>
        /// <param name="matrix2">������� matrix2</param>
        /// <returns>��������� ��������� ������ matrix1 � matrix2</returns>
        public static Matrix operator -(Matrix matrix1, Matrix matrix2)
        {
            return Matrix.MatrixSubtraction(matrix1, matrix2);
        }

        /// <summary>
        /// �������� ��������� ������ matrix1 � matrix2
        /// </summary>
        /// <param name="matrix1">������� matrix1</param>
        /// <param name="matrix2">������� matrix2</param>
        /// <returns>��������� ��������� ������ matrix1 � matrix2</returns>
        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            return Matrix.MatrixMultiplication(matrix1, matrix2);
        }

        /// <summary>
        /// �������� ��������� ������� matrix �� ����� number
        /// </summary>
        /// <param name="number">����� number</param>
        /// <param name="matrix">������� matrix</param>
        /// <returns>��������� ��������� ������� matrix �� ����� number</returns>
        public static Matrix operator *(Double number, Matrix matrix)
        {
            return Matrix.MatrixMultiplication(matrix, number);
        }

        /// <summary>
        /// ������ ��� �������� ��������� �������
        /// </summary>
        private Double[,] m_MatrixElements = null;
        /// <summary>
        /// ���������� �����
        /// </summary>
        private Int32 m_RowCount;
        /// <summary>
        /// ���������� ��������
        /// </summary>
        private Int32 m_ColumnCount;
    }
}
