using System;
using System.Collections.Generic;
using System.Text;

namespace MathPostgraduateStudy.LinearDiff3DGame
{
    /// <summary>
    /// ����� Matrix ������������ ��� �������� ������ NxM (��������� ������) � ����������� � ����
    /// </summary>
    public class Matrix : ICloneable
    {
        /// <summary>
        /// ������ ��� �������� ��������� �������
        /// </summary>
        Double[,] m_MatrixElements = null;
        /// <summary>
        /// ���������� �����
        /// </summary>
        Int32 m_RowCount = 0;
        /// <summary>
        /// ���������� ��������
        /// </summary>
        Int32 m_ColumnCount = 0;

        /// <summary>
        /// ����������� ������ Matrix
        /// </summary>
        /// <param name="RowCount">���������� �����</param>
        /// <param name="ColumnCount">���������� ��������</param>
        public Matrix(Int32 RowCount, Int32 ColumnCount)
        {
            m_MatrixElements = new Double[RowCount, ColumnCount];
            m_RowCount = RowCount;
            m_ColumnCount = ColumnCount;
        }

        /// <summary>
        /// �������� (����������) ��� ������� � ��������� �������
        /// ����� ������ ���������� � ��������� 1...RowCount
        /// ����� ������� ���������� � ��������� 1...ColumnCount
        /// </summary>
        /// <param name="RowIndex">����� (������) ������</param>
        /// <param name="ColumnIndex">����� (������) �������</param>
        /// <returns>������� �������</returns>
        public Double this[Int32 RowIndex, Int32 ColumnIndex]
        {
            get
            {
                if ((RowIndex < 1) || (RowIndex > m_RowCount))
                {
                    throw new ArgumentOutOfRangeException("Row index must be between 0 and row's count");
                }
                if ((ColumnIndex < 1) || (ColumnIndex > m_ColumnCount))
                {
                    throw new ArgumentOutOfRangeException("Column index must be between 0 and column's count");
                }

                return m_MatrixElements[(RowIndex - 1), (ColumnIndex - 1)];
            }
            set
            {
                if ((RowIndex < 1) || (RowIndex > m_RowCount))
                {
                    throw new ArgumentOutOfRangeException("Row index must be between 0 and row's count");
                }
                if ((ColumnIndex < 1) || (ColumnIndex > m_ColumnCount))
                {
                    throw new ArgumentOutOfRangeException("Column index must be between 0 and column's count");
                }

                m_MatrixElements[(RowIndex - 1), (ColumnIndex - 1)] = value;
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
        /// <param name="RowIndex">����� (������) ������������ ������</param>
        /// <returns>������������ ������ (� ���� �������)</returns>
        public Matrix GetMatrixRow(Int32 RowIndex)
        {
            if ((RowIndex < 1) || (RowIndex > m_RowCount))
            {
                throw new ArgumentOutOfRangeException("Row index must be between 0 and row's count");
            }

            Matrix RowMatrix = new Matrix(1, m_ColumnCount);

            for (Int32 ColumnIndex = 1; ColumnIndex <= m_ColumnCount; ColumnIndex++)
            {
                RowMatrix[1, ColumnIndex] = this[RowIndex, ColumnIndex];
            }

            return RowMatrix;
        }

        /// <summary>
        /// ����� SetMatrixRow ��������� ������ (�������� � ���� �������) � ������ �������� ������� � ������� (��������) RowIndex
        /// </summary>
        /// <param name="RowIndex">����� (������) ������ �������� �������</param>
        /// <param name="RowMatrix">������ (�������� � ���� �������), ������� ���� ��������� � �������� �������</param>
        public void SetMatrixRow(Int32 RowIndex, Matrix RowMatrix)
        {
            if ((RowIndex < 1) || (RowIndex > m_RowCount))
            {
                throw new ArgumentOutOfRangeException("Row index must be between 0 and row's count");
            }
            if (RowMatrix.RowCount > 1)
            {
                throw new ArgumentOutOfRangeException("RowMatrix isn't the row");
            }
            if (m_ColumnCount != RowMatrix.ColumnCount)
            {
                throw new ArgumentOutOfRangeException("Column's count of RowMatrix should be equaled to column's count of this matrix");
            }

            for (Int32 ColumnIndex = 1; ColumnIndex <= m_ColumnCount; ColumnIndex++)
            {
                this[RowIndex, ColumnIndex] = RowMatrix[1, ColumnIndex];
            }
        }

        /// <summary>
        /// ����� MatrixAddition ���������� ��������� �������� ������ Matrix1 � Matrix2
        /// </summary>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <param name="Matrix2">������� Matrix2</param>
        /// <returns>��������� �������� ������ Matrix1 � Matrix2</returns>
        public static Matrix MatrixAddition(Matrix Matrix1, Matrix Matrix2)
        {
            if (Matrix1.RowCount != Matrix2.RowCount)
            {
                throw new ArgumentOutOfRangeException("Row's count of the first matrix should be equaled to row's count of the second matrix");
            }
            if (Matrix1.ColumnCount != Matrix2.ColumnCount)
            {
                throw new ArgumentOutOfRangeException("Column's count of the first matrix should be equaled to column's count of the second matrix");
            }

            Matrix ResultMatrix = new Matrix(Matrix1.RowCount, Matrix1.ColumnCount);

            for (Int32 RowIndex = 1; RowIndex <= ResultMatrix.RowCount; RowIndex++)
            {
                for (Int32 ColumnIndex = 1; ColumnIndex <= ResultMatrix.ColumnCount; ColumnIndex++)
                {
                    ResultMatrix[RowIndex, ColumnIndex] = Matrix1[RowIndex, ColumnIndex] + Matrix2[RowIndex, ColumnIndex];
                }
            }

            return ResultMatrix;
        }

        /// <summary>
        /// ����� MatrixSubtraction ���������� ��������� �������� ������ Matrix1 � Matrix2
        /// </summary>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <param name="Matrix2">������� Matrix2</param>
        /// <returns>��������� �������� ������ Matrix1 � Matrix2</returns>
        public static Matrix MatrixSubtraction(Matrix Matrix1, Matrix Matrix2)
        {
            if (Matrix1.RowCount != Matrix2.RowCount)
            {
                throw new ArgumentOutOfRangeException("Row's count of the first matrix should be equaled to row's count of the second matrix");
            }
            if (Matrix1.ColumnCount != Matrix2.ColumnCount)
            {
                throw new ArgumentOutOfRangeException("Column's count of the first matrix should be equaled to column's count of the second matrix");
            }

            Matrix ResultMatrix = new Matrix(Matrix1.RowCount, Matrix1.ColumnCount);

            for (Int32 RowIndex = 1; RowIndex <= ResultMatrix.RowCount; RowIndex++)
            {
                for (Int32 ColumnIndex = 1; ColumnIndex <= ResultMatrix.ColumnCount; ColumnIndex++)
                {
                    ResultMatrix[RowIndex, ColumnIndex] = Matrix1[RowIndex, ColumnIndex] - Matrix2[RowIndex, ColumnIndex];
                }
            }

            return ResultMatrix;
        }

        /// <summary>
        /// ����� MatrixMultiplication ���������� ��������� ��������� ������ Matrix1 � Matrix2
        /// </summary>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <param name="Matrix2">������� Matrix2</param>
        /// <returns>��������� ��������� ������ Matrix1 � Matrix2</returns>
        public static Matrix MatrixMultiplication(Matrix Matrix1, Matrix Matrix2)
        {
            if (Matrix1.ColumnCount != Matrix2.RowCount)
            {
                throw new ArgumentOutOfRangeException("Column's count of the first matrix should be equaled to row's count of the second matrix");
            }

            Matrix ResultMatrix = new Matrix(Matrix1.RowCount, Matrix2.ColumnCount);

            for (Int32 RowIndex = 1; RowIndex <= ResultMatrix.RowCount; RowIndex++)
            {
                for (Int32 ColumnIndex = 1; ColumnIndex <= ResultMatrix.ColumnCount; ColumnIndex++)
                {
                    ResultMatrix[RowIndex, ColumnIndex] = 0;
                    for (Int32 InternalIndex = 1; InternalIndex <= Matrix1.ColumnCount; InternalIndex++)
                    {
                        ResultMatrix[RowIndex, ColumnIndex] += Matrix1[RowIndex, InternalIndex] * Matrix2[InternalIndex, ColumnIndex];
                    }
                }
            }

            return ResultMatrix;
        }

        /// <summary>
        /// ����� MatrixMultiplication ���������� ��������� ��������� ������� Matrix1 �� ����� DoubleNumber
        /// </summary>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <param name="DoubleNumber">����� DoubleNumber</param>
        /// <returns>��������� ��������� ������� Matrix1 �� ����� DoubleNumber</returns>
        public static Matrix MatrixMultiplication(Matrix Matrix1, Double DoubleNumber)
        {
            Matrix ResultMatrix = new Matrix(Matrix1.RowCount, Matrix1.ColumnCount);

            for (Int32 RowIndex = 1; RowIndex <= ResultMatrix.RowCount; RowIndex++)
            {
                for (Int32 ColumnIndex = 1; ColumnIndex <= ResultMatrix.ColumnCount; ColumnIndex++)
                {
                    ResultMatrix[RowIndex, ColumnIndex] = Matrix1[RowIndex, ColumnIndex] * DoubleNumber;
                }
            }

            return ResultMatrix;
        }

        /// <summary>
        /// ����� MatrixTransposing ���������� ��������� ���������������� ������� Matrix1
        /// </summary>
        /// <param name="Matrix1">�������� ������� Matrix1</param>
        /// <returns>��������� ���������������� ������� Matrix1</returns>
        public static Matrix MatrixTransposing(Matrix Matrix1)
        {
            Matrix ResultMatrix = new Matrix(Matrix1.ColumnCount, Matrix1.RowCount);

            for (Int32 RowIndex = 1; RowIndex <= ResultMatrix.RowCount; RowIndex++)
            {
                for (Int32 ColumnIndex = 1; ColumnIndex <= ResultMatrix.ColumnCount; ColumnIndex++)
                {
                    ResultMatrix[RowIndex, ColumnIndex] = Matrix1[ColumnIndex, RowIndex];
                }
            }

            return ResultMatrix;
        }

        /// <summary>
        /// �������� �������� ������ Matrix1 � Matrix2
        /// </summary>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <param name="Matrix2">������� Matrix2</param>
        /// <returns>��������� �������� ������ Matrix1 � Matrix2</returns>
        public static Matrix operator +(Matrix Matrix1, Matrix Matrix2)
        {
            return Matrix.MatrixAddition(Matrix1, Matrix2);
        }

        /// <summary>
        /// �������� ��������� ������ Matrix1 � Matrix2
        /// </summary>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <param name="Matrix2">������� Matrix2</param>
        /// <returns>��������� ��������� ������ Matrix1 � Matrix2</returns>
        public static Matrix operator -(Matrix Matrix1, Matrix Matrix2)
        {
            return Matrix.MatrixSubtraction(Matrix1, Matrix2);
        }

        /// <summary>
        /// �������� ��������� ������ Matrix1 � Matrix2
        /// </summary>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <param name="Matrix2">������� Matrix2</param>
        /// <returns>��������� ��������� ������ Matrix1 � Matrix2</returns>
        public static Matrix operator *(Matrix Matrix1, Matrix Matrix2)
        {
            return Matrix.MatrixMultiplication(Matrix1, Matrix2);
        }

        /// <summary>
        /// �������� ��������� ������� Matrix1 �� ����� DoubleNumber
        /// </summary>
        /// <param name="DoubleNumber">����� DoubleNumber</param>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <returns>��������� ��������� ������� Matrix1 �� ����� DoubleNumber</returns>
        public static Matrix operator *(Double DoubleNumber, Matrix Matrix1)
        {
            return Matrix.MatrixMultiplication(Matrix1, DoubleNumber);
        }

        /// <summary>
        /// ����� Clone ���������� ������ ����� (deep copy) ������ �������
        /// </summary>
        /// <returns>������ ����� (deep copy) ������ �������</returns>
        public Matrix Clone()
        {
            Matrix CloneMatrix = new Matrix(m_RowCount, m_ColumnCount);

            for (Int32 RowIndex = 1; RowIndex <= m_RowCount; RowIndex++)
            {
                for (Int32 ColumnIndex = 1; ColumnIndex <= m_ColumnCount; ColumnIndex++)
                {
                    CloneMatrix[RowIndex, ColumnIndex] = this[RowIndex, ColumnIndex];
                }
            }

            return CloneMatrix;
        }

        /// <summary>
        /// ����� Clone ���������� ������ ����� (deep copy) ������ ������� (����� ���������� ������ Clone ���������� ICloneable)
        /// </summary>
        /// <returns>������ ����� (deep copy) ������ �������</returns>
        Object ICloneable.Clone()
        {
            return this.Clone();
        }
    }

    /// <summary>
    /// ����� FundKoshiMatrix ������������� ���������� ��������������� ������� ����
    /// (� ������ �������, ������������ �� N ����� ������� ���� - ��. ��������)
    /// </summary>
    public class FundKoshiMatrix
    {
        /// <summary>
        /// DeltaT - ��� �� T ������� ����������������� ���������
        /// </summary>
        private const Double m_DeltaT = 0.001;
        /// <summary>
        /// ������� A, �� ������� ����������� ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����)
        /// </summary>
        private Matrix m_MatrixA = null;
        /// <summary>
        /// ����� ������� �����, ������������ ��� � ��������������� ������� ����
        /// (������ ������ ����������� ����������, ������� ��� ������ ��������� ��������������� ������� ���� �������)
        /// </summary>
        private Int32[] m_RowIndexes = null;
        /// <summary>
        /// �������� ����� (�.�. Theta - T) ���������� ���������� ��������������� ������� ����
        /// </summary>
        private Double m_LastInverseTime = Double.NaN;
        /// <summary>
        /// ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����), ����������� ��� ������� m_LastT
        /// </summary>
        private Matrix m_LastFundKoshiMatrix = null;

        /// <summary>
        /// ����� GetZeroFundKoshiMatrix ���������� ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����) ��� T = 0 (T - �������� �����)
        /// </summary>
        /// <returns>��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����) ��� T = 0 (T - �������� �����)</returns>
        private Matrix GetZeroTimeFundKoshiMatrix()
        {
            Matrix ZeroTimeFundKoshiMatrix = new Matrix(m_RowIndexes.Length, m_MatrixA.ColumnCount);

            for (Int32 RowIndex = 1; RowIndex <= ZeroTimeFundKoshiMatrix.RowCount; RowIndex++)
            {
                for (Int32 ColumnIndex = 1; ColumnIndex <= ZeroTimeFundKoshiMatrix.ColumnCount; ColumnIndex++)
                {
                    ZeroTimeFundKoshiMatrix[RowIndex, ColumnIndex] = (ColumnIndex == m_RowIndexes[RowIndex - 1] ? 1 : 0);
                }
            }

            return ZeroTimeFundKoshiMatrix;
        }

        /// <summary>
        /// ����� DeltaRowCalc ��������� ��������� ������� ��� ���� �� ������� m_DeltaT (��. ����� �����-�����)
        /// </summary>
        /// <param name="PreviousRow">���������� ������ (�� �������)</param>
        /// <returns>��������� ������� ��� ���� �� ������� m_DeltaT </returns>
        private Matrix DeltaRowCalc(Matrix PreviousRow)
        {
            Matrix nu1 = PreviousRow * m_MatrixA;
            Matrix nu2 = (PreviousRow + (m_DeltaT / 2) * nu1) * m_MatrixA;
            Matrix nu3 = (PreviousRow + (m_DeltaT / 2) * nu2) * m_MatrixA;
            Matrix nu4 = (PreviousRow + m_DeltaT * nu3) * m_MatrixA;
            Matrix DeltaRow = (m_DeltaT / 6) * (nu1 + 2 * nu2 + 2 * nu3 + nu4);

            return DeltaRow;
        }

        /// <summary>
        /// ����������� ������ FundKoshiMatrix
        /// </summary>
        /// <param name="MatrixA">������� A, �� ������� ����������� ��������������� ������� ����</param>
        /// <param name="RowIndexes">����� ������� �����, ������������ ��� � ��������������� ������� ����</param>
        public FundKoshiMatrix(Matrix MatrixA, Int32[] RowIndexes)
        {
            // ?? may be ������������ �� �����
            //m_MatrixA = (MatrixA.Clone() as Matrix);
            m_MatrixA = MatrixA;
            m_RowIndexes = RowIndexes;

            // ������ ����� � ������ RowIndexes ������ ���� � ������� �����������
            for (Int32 Index = 1; Index < RowIndexes.Length; Index++)
            {
                if (RowIndexes[Index - 1] >= RowIndexes[Index])
                {
                    // Some exception !!!!!
                }
            }
        }

        /// <summary>
        /// ����� FundKoshiMatrixCalc ��������� ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����)
        /// � ������ ��������� ������� InverseTime
        /// </summary>
        /// <param name="InverseTime">������ ��������� �������, ��� ������� ������������ ����������</param>
        /// <returns>�������, ������������ �� N ����� ��������������� ������� ���� � ������ ��������� ������� InverseTime</returns>
        public Matrix FundKoshiMatrixCalc(Double InverseTime)
        {
            if (m_LastInverseTime == InverseTime) return m_LastFundKoshiMatrix;

            Matrix FundKoshiMatrixValue = new Matrix(m_RowIndexes.Length, m_MatrixA.ColumnCount);

            // ���������� ���������� ������� ��������� ������� � ���������� �������� ����������� �������
            // ���� ������ ������� InverseTime, ��� �������� ��� ���� ��������� ������� ������, ��� �������� m_LastInverseTime,
            // �� ��������� �������� ��������� ������� ����� �������� m_LastInverseTime, � ��������� ��������� ����������� ������� - m_LastFundKoshiMatrix (�������� �������, ����������� ��� ������� ��������� m_LastInverseTime)
            // ����� ��������� �������� ��������� ������� ����� 0, ��������� ��������� ����������� ������� - ������� ������������ �� N ����� �� ���������
            Matrix InitialFundKoshiMatrix = (m_LastInverseTime < InverseTime ? m_LastFundKoshiMatrix : GetZeroTimeFundKoshiMatrix());
            Double CurrentInverseTime = (m_LastInverseTime < InverseTime ? m_LastInverseTime : 0);

            // �������������� ��������� ������� � ������ �����
            Matrix[] FundKoshiMatrixValueRows = new Matrix[FundKoshiMatrixValue.RowCount];
            for (Int32 RowIndex = 0; RowIndex < FundKoshiMatrixValueRows.Length; RowIndex++)
            {
                FundKoshiMatrixValueRows[RowIndex] = InitialFundKoshiMatrix.GetMatrixRow(RowIndex + 1);
            }

            // ���������� ��������� �������� �������, ������������ �� N ����� ��������������� ������� ����
            while (CurrentInverseTime < InverseTime)
            {
                for (Int32 RowIndex = 0; RowIndex < FundKoshiMatrixValueRows.Length; RowIndex++)
                {
                    FundKoshiMatrixValueRows[RowIndex] += DeltaRowCalc(FundKoshiMatrixValueRows[RowIndex]);
                }

                CurrentInverseTime += m_DeltaT;
            }

            // �������������� ������� ����� � �������� �������
            for (Int32 RowIndex = 0; RowIndex < FundKoshiMatrixValueRows.Length; RowIndex++)
            {
                FundKoshiMatrixValue.SetMatrixRow(RowIndex + 1, FundKoshiMatrixValueRows[RowIndex]);
            }

            m_LastInverseTime = InverseTime;
            m_LastFundKoshiMatrix = FundKoshiMatrixValue;

            return FundKoshiMatrixValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix GetCurrentFundKoshiMatrix()
        {
            return m_LastFundKoshiMatrix;
        }
    }
}
