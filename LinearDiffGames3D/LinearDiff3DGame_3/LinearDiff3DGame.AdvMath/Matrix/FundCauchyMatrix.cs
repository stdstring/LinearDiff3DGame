using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.AdvMath
{
    /// <summary>
    /// ����� FundCauchyMatrix ������������ ��� ���������� ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����)
    /// </summary>
    public class FundCauchyMatrix
    {
#warning �� ������� �� ������ �������� ��������� �����
        /// <summary>
        /// ����������� ������ FundCauchyMatrix
        /// </summary>
        /// <param name="MatrixA">������� A, �� ������� ����������� ��������������� ������� ����</param>
        /// <param name="RowIndexes">����� ������� �����, ������������ ��� � ��������������� ������� ����</param>
        /// <param name="deltaT">...</param>
        public FundCauchyMatrix(Matrix matrixA, Int32[] rowIndexes, Double deltaT)
        {
            m_DeltaT = deltaT;
            // ?? may be ������������ �� �����
            //m_MatrixA = matrixA.Clone();
            m_MatrixA = matrixA;
            m_RowIndexes = rowIndexes;

            /*
            m_LastTime = 0;
            m_LastFundCauchyMatrix = GetZeroTimeFundCauchyMatrix();
            */
            m_LastTime = Double.NaN;
            m_LastFundCauchyMatrix = null;

            // ������ ����� � ������ rowIndexes ������ ���� � ������� �����������
            for (Int32 index = 1; index < rowIndexes.Length; index++)
            {
                if (rowIndexes[index - 1] >= rowIndexes[index])
                {
#warning ����� ����� ������������������ ����������
                    throw new Exception("Must be following : rowIndexes[index - 1] >= rowIndexes[index]");
                }
            }
            // ������ ����� � ������ rowIndexes ������ ���� � ������� �����������
        }

        /// <summary>
        /// ���������� ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����) � ������ ������� time
        /// </summary>
        /// <param name="time">������ �������, ��� �������� ������������ ����������</param>
        /// <returns>��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����) � ������ ������� time</returns>
        public Matrix CalcFundCauchyMatrix(Double time)
        {
#warning ��������������, ��� time >= 0
            if (m_LastTime == time) return m_LastFundCauchyMatrix;

            Matrix fundCauchyMatrix = new Matrix(m_RowIndexes.Length, m_MatrixA.ColumnCount);

            // ���������� ���������� ������� ������� � ���������� �������� ����������� �������
            // ���� ������ ������� time, ��� �������� ��� ���� ��������� ������� ������, ��� �������� m_LastTime,
            // �� ��������� �������� ������� ����� �������� m_LastTime, � ��������� ��������� ����������� ������� - m_LastFundKoshiMatrix (�������� �������, ����������� ��� ������� ��������� m_LastInverseTime)
            // ����� ��������� �������� ������� ����� 0, ��������� ��������� ����������� ������� - ������� ������������ �� N ����� �� ���������
            Matrix initialFundCauchyMatrix = (m_LastTime < time ? m_LastFundCauchyMatrix : GetZeroTimeFundCauchyMatrix());
            Double currentTime = (m_LastTime < time ? m_LastTime : 0);

            // �������������� ��������� ������� � ������ �����
            Matrix[] fundCauchyMatrixRows = new Matrix[fundCauchyMatrix.RowCount];
            for (Int32 rowIndex = 0; rowIndex < fundCauchyMatrixRows.Length; ++rowIndex)
            {
                fundCauchyMatrixRows[rowIndex] = initialFundCauchyMatrix.GetMatrixRow(rowIndex + 1);
            }

            // ���������� ��������� �������� �������, ������������ �� N ����� ��������������� ������� ����
            while (currentTime < time)
            {
                for (Int32 rowIndex = 0; rowIndex < fundCauchyMatrixRows.Length; ++rowIndex)
                {
                    fundCauchyMatrixRows[rowIndex] += CalcDeltaRow(fundCauchyMatrixRows[rowIndex]);
                }

                currentTime += m_DeltaT;
            }

            // �������������� ������� ����� � �������� �������
            for (Int32 rowIndex = 0; rowIndex < fundCauchyMatrixRows.Length; ++rowIndex)
            {
                fundCauchyMatrix.SetMatrixRow(rowIndex + 1, fundCauchyMatrixRows[rowIndex]);
            }

            m_LastTime = time;
            m_LastFundCauchyMatrix = fundCauchyMatrix;

            return fundCauchyMatrix;
        }

        /// <summary>
        /// ���������� ��������� ����������� �������� ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����)
        /// </summary>
        /// <returns>��������� ����������� �������� ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����)</returns>
        public Matrix GetFundCauchyMatrix()
        {
            return m_LastFundCauchyMatrix;
        }

        /// <summary>
        /// ����� GetZeroFundCauchyMatrix ���������� ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����) ��� T = 0
        /// </summary>
        /// <returns>��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����) ��� T = 0</returns>
        private Matrix GetZeroTimeFundCauchyMatrix()
        {
            Matrix zeroTimeFundCauchyMatrix = new Matrix(m_RowIndexes.Length, m_MatrixA.ColumnCount);

            for (Int32 rowIndex = 1; rowIndex <= zeroTimeFundCauchyMatrix.RowCount; ++rowIndex)
            {
                for (Int32 columnIndex = 1; columnIndex <= zeroTimeFundCauchyMatrix.ColumnCount; ++columnIndex)
                {
                    zeroTimeFundCauchyMatrix[rowIndex, columnIndex] = (columnIndex == m_RowIndexes[rowIndex - 1] ? 1 : 0);
                }
            }

            return zeroTimeFundCauchyMatrix;
        }

        /// <summary>
        /// ����� CalcDeltaRow ��������� ��������� ������� ��� ���� �� ������� m_DeltaT (��. ����� �����-�����)
        /// </summary>
        /// <param name="previousRow">���������� ������ (�� �������)</param>
        /// <returns>��������� ������� ��� ���� �� ������� m_DeltaT </returns>
        private Matrix CalcDeltaRow(Matrix previousRow)
        {
            Matrix nu1 = previousRow * m_MatrixA;
            Matrix nu2 = (previousRow + (m_DeltaT / 2) * nu1) * m_MatrixA;
            Matrix nu3 = (previousRow + (m_DeltaT / 2) * nu2) * m_MatrixA;
            Matrix nu4 = (previousRow + m_DeltaT * nu3) * m_MatrixA;
            Matrix deltaRow = (m_DeltaT / 6) * (nu1 + 2 * nu2 + 2 * nu3 + nu4);

            return deltaRow;
        }

        /// <summary>
        /// DeltaT - ��� �� T ������� ����������������� ���������
        /// </summary>
        private readonly Double m_DeltaT;
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
        /// [��������] ����� [(�.�. Theta - T)] ���������� ���������� ��������������� ������� ����
        /// </summary>
        private Double m_LastTime = Double.NaN;
        /// <summary>
        /// ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����), ����������� ��� ������� m_LastTime
        /// </summary>
        private Matrix m_LastFundCauchyMatrix = null;
    }
}
