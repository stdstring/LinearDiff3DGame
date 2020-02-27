using System;

namespace LinearDiff3DGame.AdvMath.Matrix
{
    // вычисление фундаментальной матрицы Коши (а точнее матрица, составленная из N строк матрицы Коши)
    public class FundCauchyMatrix
    {
#warning за решение ДУ должен отвечать отдельный класс
        public FundCauchyMatrix(Matrix matrixA, Int32[] rowIndexes, Double deltaT)
        {
            this.deltaT = deltaT;
            // ?? may be клонирование не нужно
            //matrixA = matrixA.Clone();
            this.matrixA = matrixA;
            this.rowIndexes = rowIndexes;

            lastTime = 0;
            lastFundCauchyMatrix = GetZeroTimeFundCauchyMatrix();

            //lastTime = Double.NaN;
            //lastFundCauchyMatrix = null;

            // номера строк в наборе rowIndexes должны идти в порядке возрастания
            for (Int32 index = 1; index < rowIndexes.Length; index++)
            {
                if(rowIndexes[index - 1] >= rowIndexes[index])
                {
#warning может более специализированное исключение
                    throw new Exception("Must be following : rowIndexes[index - 1] >= rowIndexes[index]");
                }
            }
            // номера строк в наборе rowIndexes должны идти в порядке возрастания
        }

        public Matrix Calculate(Double time)
        {
#warning предполагается, что time >= 0
            if(lastTime == time) return lastFundCauchyMatrix;

            Matrix fundCauchyMatrix = new Matrix(rowIndexes.Length,
                                                 matrixA.ColumnCount);

            // вычисление начального момента времени и начального значения вычисляемой матрицы
            // если момент времени time, для которого нам надо вычислить матрицу больше, чем значение lastTime,
            // то начальным моментом времени будет значение lastTime, а начальным значением вычисляемой матрицы - m_LastFundKoshiMatrix (значение матрицы, вычисленное для момента обратного m_LastInverseTime)
            // иначе начальным моментом времени будет 0, начальным значением вычисляемой матрицы - матрица составленная из N строк из единичной
            Matrix initialFundCauchyMatrix = (lastTime < time
                                                  ? lastFundCauchyMatrix
                                                  : GetZeroTimeFundCauchyMatrix());
            Double currentTime = (lastTime < time ? lastTime : 0);

            // преобразование начальной матрицы в массив строк
            Matrix[] fundCauchyMatrixRows = new Matrix[fundCauchyMatrix.RowCount];
            for (Int32 rowIndex = 0; rowIndex < fundCauchyMatrixRows.Length; ++rowIndex)
                fundCauchyMatrixRows[rowIndex] = initialFundCauchyMatrix.GetMatrixRow(rowIndex + 1);

            // вычисление конечного значения матрицы, составленной из N строк фундаментальной матрицы Коши
            while (currentTime < time)
            {
                for (Int32 rowIndex = 0; rowIndex < fundCauchyMatrixRows.Length; ++rowIndex)
                    fundCauchyMatrixRows[rowIndex] += CalcDeltaRow(fundCauchyMatrixRows[rowIndex]);

                currentTime += deltaT;
            }

            // преобразование массива строк в конечную матрицу
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

        // вычисление фундаментальной матрицы Коши (а точнее матрицы, составленной из N строк матрицы Коши) при T = 0
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

        // вычисление изменения вектора при шаге по времени deltaT (см. метод Рунге-Кутта)
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

        // матрица A, по которой вычисляется фундаментальная матрица Коши (а точнее матрица, составленная из N строк матрицы Коши)
        private readonly Matrix matrixA;

        // набор номеров строк, интересующих нас в фундаментальной матрице Коши
        // (каждая строка вычисляется независимо, поэтому нет смысла вычислять фундаментальную матрицу Коши целиком)
        private readonly Int32[] rowIndexes;

        // [обратное] время [(т.е. Theta - T)] последнего вычисления фундаментальной матрицы Коши
        private Double lastTime;

        // фундаментальная матрица Коши (а точнее матрица, составленная из N строк матрицы Коши), вычисленная для времени lastTime
        private Matrix lastFundCauchyMatrix;
    }
}