using System;

namespace LinearDiff3DGame.AdvMath.MatrixUtils
{
    public class MatrixFactory
    {
        public Matrix CreateFromRawData(Int32 rowCount, Int32 columnCount, params Double[] values)
        {
            if(rowCount * columnCount != values.Length)
                throw new ArgumentException("RowCount*ColumnCount must be equal values length", "values");
            Matrix result = new Matrix(rowCount, columnCount);
            for(Int32 valueIndex = 0; valueIndex < values.Length; ++valueIndex)
            {
                Int32 rowIndex = (valueIndex / columnCount) + 1;
                Int32 columnIndex = (valueIndex % columnCount) + 1;
                result[rowIndex, columnIndex] = values[valueIndex];
            }
            return result;
        }
    }
}