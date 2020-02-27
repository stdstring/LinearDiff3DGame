using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.AdvMath
{
    /// <summary>
    /// класс FundCauchyMatrix предназначен для вычисления фундаментальной матрицы Коши (а точнее матрица, составленная из N строк матрицы Коши)
    /// </summary>
    public class FundCauchyMatrix
    {
#warning за решение ДУ должен отвечать отдельный класс
        /// <summary>
        /// конструктор класса FundCauchyMatrix
        /// </summary>
        /// <param name="MatrixA">матрица A, по которой вычисляется фундаментальная матрица Коши</param>
        /// <param name="RowIndexes">набор номеров строк, интересующих нас в фундаментальной матрице Коши</param>
        /// <param name="deltaT">...</param>
        public FundCauchyMatrix(Matrix matrixA, Int32[] rowIndexes, Double deltaT)
        {
            m_DeltaT = deltaT;
            // ?? may be клонирование не нужно
            //m_MatrixA = matrixA.Clone();
            m_MatrixA = matrixA;
            m_RowIndexes = rowIndexes;

            /*
            m_LastTime = 0;
            m_LastFundCauchyMatrix = GetZeroTimeFundCauchyMatrix();
            */
            m_LastTime = Double.NaN;
            m_LastFundCauchyMatrix = null;

            // номера строк в наборе rowIndexes должны идти в порядке возрастания
            for (Int32 index = 1; index < rowIndexes.Length; index++)
            {
                if (rowIndexes[index - 1] >= rowIndexes[index])
                {
#warning может более специализированное исключение
                    throw new Exception("Must be following : rowIndexes[index - 1] >= rowIndexes[index]");
                }
            }
            // номера строк в наборе rowIndexes должны идти в порядке возрастания
        }

        /// <summary>
        /// вычисление фундаментальной матрицы Коши (а точнее матрица, составленная из N строк матрицы Коши) в момент времени time
        /// </summary>
        /// <param name="time">момент времени, для которого производится вычисление</param>
        /// <returns>фундаментальная матрица Коши (а точнее матрица, составленная из N строк матрицы Коши) в момент времени time</returns>
        public Matrix CalcFundCauchyMatrix(Double time)
        {
#warning предполагается, что time >= 0
            if (m_LastTime == time) return m_LastFundCauchyMatrix;

            Matrix fundCauchyMatrix = new Matrix(m_RowIndexes.Length, m_MatrixA.ColumnCount);

            // вычисление начального момента времени и начального значения вычисляемой матрицы
            // если момент времени time, для которого нам надо вычислить матрицу больше, чем значение m_LastTime,
            // то начальным моментом времени будет значение m_LastTime, а начальным значением вычисляемой матрицы - m_LastFundKoshiMatrix (значение матрицы, вычисленное для момента обратного m_LastInverseTime)
            // иначе начальным моментом времени будет 0, начальным значением вычисляемой матрицы - матрица составленная из N строк из единичной
            Matrix initialFundCauchyMatrix = (m_LastTime < time ? m_LastFundCauchyMatrix : GetZeroTimeFundCauchyMatrix());
            Double currentTime = (m_LastTime < time ? m_LastTime : 0);

            // преобразование начальной матрицы в массив строк
            Matrix[] fundCauchyMatrixRows = new Matrix[fundCauchyMatrix.RowCount];
            for (Int32 rowIndex = 0; rowIndex < fundCauchyMatrixRows.Length; ++rowIndex)
            {
                fundCauchyMatrixRows[rowIndex] = initialFundCauchyMatrix.GetMatrixRow(rowIndex + 1);
            }

            // вычисление конечного значения матрицы, составленной из N строк фундаментальной матрицы Коши
            while (currentTime < time)
            {
                for (Int32 rowIndex = 0; rowIndex < fundCauchyMatrixRows.Length; ++rowIndex)
                {
                    fundCauchyMatrixRows[rowIndex] += CalcDeltaRow(fundCauchyMatrixRows[rowIndex]);
                }

                currentTime += m_DeltaT;
            }

            // преобразование массива строк в конечную матрицу
            for (Int32 rowIndex = 0; rowIndex < fundCauchyMatrixRows.Length; ++rowIndex)
            {
                fundCauchyMatrix.SetMatrixRow(rowIndex + 1, fundCauchyMatrixRows[rowIndex]);
            }

            m_LastTime = time;
            m_LastFundCauchyMatrix = fundCauchyMatrix;

            return fundCauchyMatrix;
        }

        /// <summary>
        /// возвращает последнее вычисленное значение фундаментальной матрицы Коши (а точнее матрицы, составленная из N строк матрицы Коши)
        /// </summary>
        /// <returns>последнее вычисленное значение фундаментальной матрицы Коши (а точнее матрицы, составленная из N строк матрицы Коши)</returns>
        public Matrix GetFundCauchyMatrix()
        {
            return m_LastFundCauchyMatrix;
        }

        /// <summary>
        /// метод GetZeroFundCauchyMatrix возвращает фундаментальную матрицу Коши (а точнее матрицу, составленную из N строк матрицы Коши) при T = 0
        /// </summary>
        /// <returns>фундаментальная матрица Коши (а точнее матрица, составленная из N строк матрицы Коши) при T = 0</returns>
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
        /// метод CalcDeltaRow вычисляет изменение вектора при шаге по времени m_DeltaT (см. метод Рунге-Кутта)
        /// </summary>
        /// <param name="previousRow">предыдущый вектор (по времени)</param>
        /// <returns>изменение вектора при шаге по времени m_DeltaT </returns>
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
        /// DeltaT - шаг по T решения дифференциального уравнения
        /// </summary>
        private readonly Double m_DeltaT;
        /// <summary>
        /// матрица A, по которой вычисляется фундаментальная матрица Коши (а точнее матрица, составленная из N строк матрицы Коши)
        /// </summary>
        private Matrix m_MatrixA = null;
        /// <summary>
        /// набор номеров строк, интересующих нас в фундаментальной матрице Коши
        /// (каждая строка вычисляется независимо, поэтому нет смысла вычислять фундаментальную матрицу Коши целиком)
        /// </summary>
        private Int32[] m_RowIndexes = null;
        /// <summary>
        /// [обратное] время [(т.е. Theta - T)] последнего вычисления фундаментальной матрицы Коши
        /// </summary>
        private Double m_LastTime = Double.NaN;
        /// <summary>
        /// фундаментальная матрица Коши (а точнее матрица, составленная из N строк матрицы Коши), вычисленная для времени m_LastTime
        /// </summary>
        private Matrix m_LastFundCauchyMatrix = null;
    }
}
