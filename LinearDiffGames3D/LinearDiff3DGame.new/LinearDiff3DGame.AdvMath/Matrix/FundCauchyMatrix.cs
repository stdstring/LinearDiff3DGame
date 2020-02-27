using System;

namespace LinearDiff3DGame.AdvMath.Matrix
{
    // ���������� ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����)
    public class FundCauchyMatrix
    {
#warning �� ������� �� ������ �������� ��������� �����
        public FundCauchyMatrix(Matrix matrixA, Int32[] rowIndexes, Double deltaT)
        {
            this.deltaT = deltaT;
            // ?? may be ������������ �� �����
            //matrixA = matrixA.Clone();
            this.matrixA = matrixA;
            this.rowIndexes = rowIndexes;

            lastTime = 0;
            lastFundCauchyMatrix = GetZeroTimeFundCauchyMatrix();

            //lastTime = Double.NaN;
            //lastFundCauchyMatrix = null;

            // ������ ����� � ������ rowIndexes ������ ���� � ������� �����������
            for (Int32 index = 1; index < rowIndexes.Length; index++)
            {
                if(rowIndexes[index - 1] >= rowIndexes[index])
                {
#warning ����� ����� ������������������ ����������
                    throw new Exception("Must be following : rowIndexes[index - 1] >= rowIndexes[index]");
                }
            }
            // ������ ����� � ������ rowIndexes ������ ���� � ������� �����������
        }

        public Matrix Calculate(Double time)
        {
#warning ��������������, ��� time >= 0
            if(lastTime == time) return lastFundCauchyMatrix;

            Matrix fundCauchyMatrix = new Matrix(rowIndexes.Length,
                                                 matrixA.ColumnCount);

            // ���������� ���������� ������� ������� � ���������� �������� ����������� �������
            // ���� ������ ������� time, ��� �������� ��� ���� ��������� ������� ������, ��� �������� lastTime,
            // �� ��������� �������� ������� ����� �������� lastTime, � ��������� ��������� ����������� ������� - m_LastFundKoshiMatrix (�������� �������, ����������� ��� ������� ��������� m_LastInverseTime)
            // ����� ��������� �������� ������� ����� 0, ��������� ��������� ����������� ������� - ������� ������������ �� N ����� �� ���������
            Matrix initialFundCauchyMatrix = (lastTime < time
                                                  ? lastFundCauchyMatrix
                                                  : GetZeroTimeFundCauchyMatrix());
            Double currentTime = (lastTime < time ? lastTime : 0);

            // �������������� ��������� ������� � ������ �����
            Matrix[] fundCauchyMatrixRows = new Matrix[fundCauchyMatrix.RowCount];
            for (Int32 rowIndex = 0; rowIndex < fundCauchyMatrixRows.Length; ++rowIndex)
                fundCauchyMatrixRows[rowIndex] = initialFundCauchyMatrix.GetMatrixRow(rowIndex + 1);

            // ���������� ��������� �������� �������, ������������ �� N ����� ��������������� ������� ����
            while (currentTime < time)
            {
                for (Int32 rowIndex = 0; rowIndex < fundCauchyMatrixRows.Length; ++rowIndex)
                    fundCauchyMatrixRows[rowIndex] += CalcDeltaRow(fundCauchyMatrixRows[rowIndex]);

                currentTime += deltaT;
            }

            // �������������� ������� ����� � �������� �������
            for (Int32 rowIndex = 0; rowIndex < fundCauchyMatrixRows.Length; ++rowIndex)
                fundCauchyMatrix.SetMatrixRow(rowIndex + 1, fundCauchyMatrixRows[rowIndex]);

            lastTime = time;
            lastFundCauchyMatrix = fundCauchyMatrix;

            return fundCauchyMatrix;
        }

        public Matrix Current
        {
            get { return lastFundCauchyMatrix; }
        }

        // ���������� ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����) ��� T = 0
        private Matrix GetZeroTimeFundCauchyMatrix()
        {
            Matrix zeroTimeFundCauchyMatrix = new Matrix(rowIndexes.Length,
                                                         matrixA.ColumnCount);

            for (Int32 rowIndex = 1; rowIndex <= zeroTimeFundCauchyMatrix.RowCount; ++rowIndex)
            {
                for (Int32 columnIndex = 1; columnIndex <= zeroTimeFundCauchyMatrix.ColumnCount; ++columnIndex)
                    zeroTimeFundCauchyMatrix[rowIndex, columnIndex] = (columnIndex == rowIndexes[rowIndex - 1] ? 1 : 0);
            }

            return zeroTimeFundCauchyMatrix;
        }

        // ���������� ��������� ������� ��� ���� �� ������� deltaT (��. ����� �����-�����)
        private Matrix CalcDeltaRow(Matrix previousRow)
        {
            Matrix nu1 = previousRow*matrixA;
            Matrix nu2 = (previousRow + (deltaT/2)*nu1)*matrixA;
            Matrix nu3 = (previousRow + (deltaT/2)*nu2)*matrixA;
            Matrix nu4 = (previousRow + deltaT*nu3)*matrixA;
            Matrix deltaRow = (deltaT/6)*(nu1 + 2*nu2 + 2*nu3 + nu4);

            return deltaRow;
        }

        private readonly Double deltaT;

        // ������� A, �� ������� ����������� ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����)
        private readonly Matrix matrixA;

        // ����� ������� �����, ������������ ��� � ��������������� ������� ����
        // (������ ������ ����������� ����������, ������� ��� ������ ��������� ��������������� ������� ���� �������)
        private readonly Int32[] rowIndexes;

        // [��������] ����� [(�.�. Theta - T)] ���������� ���������� ��������������� ������� ����
        private Double lastTime;

        // ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����), ����������� ��� ������� lastTime
        private Matrix lastFundCauchyMatrix;
    }
}