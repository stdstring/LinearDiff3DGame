using System;
using System.Collections.Generic;
using System.Text;

namespace MathPostgraduateStudy.LinearDiff3DGame
{
    /// <summary>
    /// класс Matrix предназначен для хранения матриц NxM (элементов матриц) и манипуляций с ними
    /// </summary>
    public class Matrix : ICloneable
    {
        /// <summary>
        /// массив для хранения элементов матрицы
        /// </summary>
        Double[,] m_MatrixElements = null;
        /// <summary>
        /// количество строк
        /// </summary>
        Int32 m_RowCount = 0;
        /// <summary>
        /// количество столбцов
        /// </summary>
        Int32 m_ColumnCount = 0;

        /// <summary>
        /// конструктор класса Matrix
        /// </summary>
        /// <param name="RowCount">количество строк</param>
        /// <param name="ColumnCount">количество столбцов</param>
        public Matrix(Int32 RowCount, Int32 ColumnCount)
        {
            m_MatrixElements = new Double[RowCount, ColumnCount];
            m_RowCount = RowCount;
            m_ColumnCount = ColumnCount;
        }

        /// <summary>
        /// свойство (индексатор) для доступа к элементам матрицы
        /// номер строки изменяется в диапазоне 1...RowCount
        /// номер столбца изменяется в диапазоне 1...ColumnCount
        /// </summary>
        /// <param name="RowIndex">номер (индекс) строки</param>
        /// <param name="ColumnIndex">номер (индекс) столбца</param>
        /// <returns>элемент матрицы</returns>
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
        /// свойтво RowCount возвращает количество строк
        /// </summary>
        public Int32 RowCount
        {
            get
            {
                return m_RowCount;
            }
        }

        /// <summary>
        /// свойтво RowCount возвращает количество столбцов
        /// </summary>
        public Int32 ColumnCount
        {
            get
            {
                return m_ColumnCount;
            }
        }

        /// <summary>
        /// метод GetMatrixRow возвращает строку (в виде матрицы) матрицы
        /// </summary>
        /// <param name="RowIndex">номер (индекс) возвращаемой строки</param>
        /// <returns>возвращаемая строка (в виде матрицы)</returns>
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
        /// метод SetMatrixRow сохраняет строку (заданную в виде матрицы) в строке исходной матрицы с номером (индексом) RowIndex
        /// </summary>
        /// <param name="RowIndex">номер (индекс) строки исходной матрицы</param>
        /// <param name="RowMatrix">строка (заданная в виде матрицы), которую надо сохранить в исходной матрице</param>
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
        /// метод MatrixAddition возвращает результат сложения матриц Matrix1 и Matrix2
        /// </summary>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <param name="Matrix2">матрица Matrix2</param>
        /// <returns>результат сложения матриц Matrix1 и Matrix2</returns>
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
        /// метод MatrixSubtraction возвращает результат разности матриц Matrix1 и Matrix2
        /// </summary>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <param name="Matrix2">матрица Matrix2</param>
        /// <returns>результат разности матриц Matrix1 и Matrix2</returns>
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
        /// метод MatrixMultiplication возвращает результат умножения матриц Matrix1 и Matrix2
        /// </summary>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <param name="Matrix2">матрица Matrix2</param>
        /// <returns>результат умножения матриц Matrix1 и Matrix2</returns>
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
        /// метод MatrixMultiplication возвращает результат умножения матрицы Matrix1 на число DoubleNumber
        /// </summary>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <param name="DoubleNumber">число DoubleNumber</param>
        /// <returns>результат умножения матрицы Matrix1 на число DoubleNumber</returns>
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
        /// метод MatrixTransposing возвращает результат транспонирования матрицы Matrix1
        /// </summary>
        /// <param name="Matrix1">исходная матрица Matrix1</param>
        /// <returns>результат транспонирования матрицы Matrix1</returns>
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
        /// оператор сложения матриц Matrix1 и Matrix2
        /// </summary>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <param name="Matrix2">матрица Matrix2</param>
        /// <returns>результат сложения матриц Matrix1 и Matrix2</returns>
        public static Matrix operator +(Matrix Matrix1, Matrix Matrix2)
        {
            return Matrix.MatrixAddition(Matrix1, Matrix2);
        }

        /// <summary>
        /// оператор вычитания матриц Matrix1 и Matrix2
        /// </summary>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <param name="Matrix2">матрица Matrix2</param>
        /// <returns>результат вычитания матриц Matrix1 и Matrix2</returns>
        public static Matrix operator -(Matrix Matrix1, Matrix Matrix2)
        {
            return Matrix.MatrixSubtraction(Matrix1, Matrix2);
        }

        /// <summary>
        /// оператор умножения матриц Matrix1 и Matrix2
        /// </summary>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <param name="Matrix2">матрица Matrix2</param>
        /// <returns>результат умножения матриц Matrix1 и Matrix2</returns>
        public static Matrix operator *(Matrix Matrix1, Matrix Matrix2)
        {
            return Matrix.MatrixMultiplication(Matrix1, Matrix2);
        }

        /// <summary>
        /// оператор умножения матрицы Matrix1 на число DoubleNumber
        /// </summary>
        /// <param name="DoubleNumber">число DoubleNumber</param>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <returns>результат умножения матрицы Matrix1 на число DoubleNumber</returns>
        public static Matrix operator *(Double DoubleNumber, Matrix Matrix1)
        {
            return Matrix.MatrixMultiplication(Matrix1, DoubleNumber);
        }

        /// <summary>
        /// метод Clone возвращает полную копию (deep copy) данной матрицы
        /// </summary>
        /// <returns>полная копия (deep copy) данной матрицы</returns>
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
        /// метод Clone возвращает полную копию (deep copy) данной матрицы (явная реализация метода Clone интерфейса ICloneable)
        /// </summary>
        /// <returns>полная копия (deep copy) данной матрицы</returns>
        Object ICloneable.Clone()
        {
            return this.Clone();
        }
    }

    /// <summary>
    /// класс FundKoshiMatrix инкапсулирует вычисление фундаментальной матрицы Коши
    /// (а точнее матрицы, составленной из N строк матрицы Коши - см. алгоритм)
    /// </summary>
    public class FundKoshiMatrix
    {
        /// <summary>
        /// DeltaT - шаг по T решения дифференциального уравнения
        /// </summary>
        private const Double m_DeltaT = 0.001;
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
        /// обратное время (т.е. Theta - T) последнего вычисления фундаментальной матрицы Коши
        /// </summary>
        private Double m_LastInverseTime = Double.NaN;
        /// <summary>
        /// фундаментальная матрица Коши (а точнее матрица, составленная из N строк матрицы Коши), вычисленная для времени m_LastT
        /// </summary>
        private Matrix m_LastFundKoshiMatrix = null;

        /// <summary>
        /// метод GetZeroFundKoshiMatrix возвращает фундаментальную матрицу Коши (а точнее матрицу, составленную из N строк матрицы Коши) при T = 0 (T - обратное время)
        /// </summary>
        /// <returns>фундаментальная матрица Коши (а точнее матрица, составленная из N строк матрицы Коши) при T = 0 (T - обратное время)</returns>
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
        /// метод DeltaRowCalc вычисляет изменение вектора при шаге по времени m_DeltaT (см. метод Рунге-Кутта)
        /// </summary>
        /// <param name="PreviousRow">предыдущый вектор (по времени)</param>
        /// <returns>изменение вектора при шаге по времени m_DeltaT </returns>
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
        /// конструктор класса FundKoshiMatrix
        /// </summary>
        /// <param name="MatrixA">матрица A, по которой вычисляется фундаментальная матрица Коши</param>
        /// <param name="RowIndexes">набор номеров строк, интересующих нас в фундаментальной матрице Коши</param>
        public FundKoshiMatrix(Matrix MatrixA, Int32[] RowIndexes)
        {
            // ?? may be клонирование не нужно
            //m_MatrixA = (MatrixA.Clone() as Matrix);
            m_MatrixA = MatrixA;
            m_RowIndexes = RowIndexes;

            // номера строк в наборе RowIndexes должны идти в порядке возрастания
            for (Int32 Index = 1; Index < RowIndexes.Length; Index++)
            {
                if (RowIndexes[Index - 1] >= RowIndexes[Index])
                {
                    // Some exception !!!!!
                }
            }
        }

        /// <summary>
        /// метод FundKoshiMatrixCalc вычисляет фундаментальную матрицу Коши (а точнее матрицу, составленную из N строк матрицы Коши)
        /// в момент обратного времени InverseTime
        /// </summary>
        /// <param name="InverseTime">момент обратного времени, для котрого производятся вычисления</param>
        /// <returns>матрица, составленная из N строк фундаментальной матрицы Коши в момент обратного времени InverseTime</returns>
        public Matrix FundKoshiMatrixCalc(Double InverseTime)
        {
            if (m_LastInverseTime == InverseTime) return m_LastFundKoshiMatrix;

            Matrix FundKoshiMatrixValue = new Matrix(m_RowIndexes.Length, m_MatrixA.ColumnCount);

            // вычисление начального момента обратного времени и начального значения вычисляемой матрицы
            // если момент времени InverseTime, для которого нам надо вычислить матрицу больше, чем значение m_LastInverseTime,
            // то начальным моментом обратного времени будет значение m_LastInverseTime, а начальным значением вычисляемой матрицы - m_LastFundKoshiMatrix (значение матрицы, вычисленное для момента обратного m_LastInverseTime)
            // иначе начальным моментом обратного времени будет 0, начальным значением вычисляемой матрицы - матрица составленная из N строк из единичной
            Matrix InitialFundKoshiMatrix = (m_LastInverseTime < InverseTime ? m_LastFundKoshiMatrix : GetZeroTimeFundKoshiMatrix());
            Double CurrentInverseTime = (m_LastInverseTime < InverseTime ? m_LastInverseTime : 0);

            // преобразование начальной матрицы в массив строк
            Matrix[] FundKoshiMatrixValueRows = new Matrix[FundKoshiMatrixValue.RowCount];
            for (Int32 RowIndex = 0; RowIndex < FundKoshiMatrixValueRows.Length; RowIndex++)
            {
                FundKoshiMatrixValueRows[RowIndex] = InitialFundKoshiMatrix.GetMatrixRow(RowIndex + 1);
            }

            // вычисление конечного значения матрицы, составленной из N строк фундаментальной матрицы Коши
            while (CurrentInverseTime < InverseTime)
            {
                for (Int32 RowIndex = 0; RowIndex < FundKoshiMatrixValueRows.Length; RowIndex++)
                {
                    FundKoshiMatrixValueRows[RowIndex] += DeltaRowCalc(FundKoshiMatrixValueRows[RowIndex]);
                }

                CurrentInverseTime += m_DeltaT;
            }

            // преобразование массива строк в конечную матрицу
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
