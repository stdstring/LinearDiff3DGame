using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearDiff3DGame.AdvMath
{
    /// <summary>
    /// класс Matrix предназначен для хранения матриц NxM и элементарных манипуляций с ними
    /// </summary>
    public sealed partial class Matrix : IElementaryOp
    {
        /// <summary>
        /// конструктор класса Matrix
        /// </summary>
        /// <param name="rowCount">количество строк</param>
        /// <param name="columnCount">количество столбцов</param>
        public Matrix(Int32 rowCount, Int32 columnCount)
        {
            m_MatrixElements = new Double[rowCount, columnCount];
            m_RowCount = rowCount;
            m_ColumnCount = columnCount;
        }

        /// <summary>
        /// свойство (индексатор) для доступа к элементам матрицы
        /// номер строки изменяется в диапазоне 1...RowCount
        /// номер столбца изменяется в диапазоне 1...ColumnCount
        /// </summary>
        /// <param name="rowIndex">номер строки</param>
        /// <param name="columnIndex">номер столбца</param>
        /// <returns>элемент матрицы</returns>
        public Double this[Int32 rowIndex, Int32 columnIndex]
        {
            get
            {
                if ((rowIndex < 1) || (rowIndex > m_RowCount))
                {
                    throw new ArgumentOutOfRangeException("Row index must be between 1 and row's count");
                }
                if ((columnIndex < 1) || (columnIndex > m_ColumnCount))
                {
                    throw new ArgumentOutOfRangeException("Column index must be between 1 and column's count");
                }

                return m_MatrixElements[(rowIndex - 1), (columnIndex - 1)];
            }
            set
            {
                if ((rowIndex < 1) || (rowIndex > m_RowCount))
                {
                    throw new ArgumentOutOfRangeException("Row index must be between 1 and row's count");
                }
                if ((columnIndex < 1) || (columnIndex > m_ColumnCount))
                {
                    throw new ArgumentOutOfRangeException("Column index must be between 1 and column's count");
                }

                m_MatrixElements[(rowIndex - 1), (columnIndex - 1)] = value;
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
        /// массив для хранения элементов матрицы
        /// </summary>
        private Double[,] m_MatrixElements;
        /// <summary>
        /// количество строк
        /// </summary>
        private readonly Int32 m_RowCount;
        /// <summary>
        /// количество столбцов
        /// </summary>
        private readonly Int32 m_ColumnCount;
    }
}
