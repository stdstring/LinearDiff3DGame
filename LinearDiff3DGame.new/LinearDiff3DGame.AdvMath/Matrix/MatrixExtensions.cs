using System;

namespace LinearDiff3DGame.AdvMath.Matrix
{
    public static class MatrixExtensions
    {
        public static Matrix GetMatrixRow(this Matrix source, Int32 rowIndex)
        {
            if((rowIndex < 1) || (rowIndex > source.RowCount))
                throw new ArgumentOutOfRangeException("rowIndex");
            Matrix rowMatrix = new Matrix(1, source.ColumnCount);
            for(Int32 columnIndex = 1; columnIndex <= source.ColumnCount; ++columnIndex)
                rowMatrix[1, columnIndex] = source[rowIndex, columnIndex];
            return rowMatrix;
        }

        public static Matrix SetMatrixRow(this Matrix source, Int32 rowIndex, Matrix rowMatrix)
        {
            if((rowIndex < 1) || (rowIndex > source.RowCount))
                throw new ArgumentOutOfRangeException("rowIndex");
            if(rowMatrix.RowCount > 1 || source.ColumnCount != rowMatrix.ColumnCount)
                throw new ArgumentOutOfRangeException("rowMatrix");
            for(Int32 columnIndex = 1; columnIndex <= source.ColumnCount; ++columnIndex)
                source[rowIndex, columnIndex] = rowMatrix[1, columnIndex];
            return source;
        }

        public static Matrix SwapRows(this Matrix source, Int32 rowIndex1, Int32 rowIndex2)
        {
            if(rowIndex1 < 1 || rowIndex1 > source.RowCount)
                throw new ArgumentOutOfRangeException("rowIndex1");
            if(rowIndex2 < 1 || rowIndex2 > source.RowCount)
                throw new ArgumentOutOfRangeException("rowIndex2");
            if(rowIndex1 == rowIndex2) return source;
            for(Int32 columnIndex = 1; columnIndex <= source.ColumnCount; ++columnIndex)
            {
                Swap(source, rowIndex1, columnIndex, rowIndex2, columnIndex);
            }
            return source;
        }

        public static Matrix SwapColumns(this Matrix source, Int32 columnIndex1, Int32 columnIndex2)
        {
            if(columnIndex1 < 1 || columnIndex1 > source.ColumnCount)
                throw new ArgumentOutOfRangeException("columnIndex1");
            if(columnIndex2 < 1 || columnIndex2 > source.ColumnCount)
                throw new ArgumentOutOfRangeException("columnIndex2");
            if(columnIndex1 == columnIndex2) return source;
            for(Int32 rowIndex = 1; rowIndex <= source.RowCount; ++rowIndex)
            {
                Swap(source, rowIndex, columnIndex1, rowIndex, columnIndex2);
            }
            return source;
        }

        public static Matrix Clone(this Matrix source)
        {
            Matrix clone = new Matrix(source.RowCount, source.ColumnCount);
            for(Int32 rowIndex = 1; rowIndex <= source.RowCount; ++rowIndex)
            {
                for(Int32 columnIndex = 1; columnIndex <= source.ColumnCount; ++columnIndex)
                    clone[rowIndex, columnIndex] = source[rowIndex, columnIndex];
            }
            return clone;
        }

        private static void Swap(Matrix source, Int32 rowIndex1, Int32 columnIndex1, Int32 rowIndex2, Int32 columnIndex2)
        {
            Double temp = source[rowIndex1, columnIndex1];
            source[rowIndex1, columnIndex1] = source[rowIndex2, columnIndex2];
            source[rowIndex2, columnIndex2] = temp;
        }
    }
}