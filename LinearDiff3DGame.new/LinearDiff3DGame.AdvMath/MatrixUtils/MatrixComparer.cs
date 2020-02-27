using System;
using LinearDiff3DGame.AdvMath.Common;

namespace LinearDiff3DGame.AdvMath.MatrixUtils
{
    public class MatrixComparer
    {
        public Boolean Equals(Matrix.Matrix matrix1, Matrix.Matrix matrix2)
        {
            if(matrix1.RowCount != matrix2.RowCount || matrix1.ColumnCount != matrix2.ColumnCount)
                throw new ArgumentException("Не совпадают размеры матриц");
            return Equals(matrix1, matrix2, (number1, number2) => number1.Equals(number2));
        }

        public Boolean Equals(ApproxComp comparer, Matrix.Matrix matrix1, Matrix.Matrix matrix2)
        {
            if (matrix1.RowCount != matrix2.RowCount || matrix1.ColumnCount != matrix2.ColumnCount)
                throw new ArgumentException("Не совпадают размеры матриц");
            return Equals(matrix1, matrix2, comparer.EQ);
        }

        private static Boolean Equals(Matrix.Matrix matrix1, Matrix.Matrix matrix2, Func<Double, Double, Boolean> itemComparer)
        {
            for(Int32 rowIndex = 1; rowIndex <= matrix1.RowCount; ++rowIndex)
                for(Int32 columnIndex = 1; columnIndex <= matrix1.ColumnCount; ++columnIndex)
                    if(!itemComparer(matrix1[rowIndex, columnIndex], matrix2[rowIndex, columnIndex])) return false;
            return true;
        }
    }
}