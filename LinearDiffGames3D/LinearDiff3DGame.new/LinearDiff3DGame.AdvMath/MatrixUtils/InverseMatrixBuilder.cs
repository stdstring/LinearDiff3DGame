using System;
using LinearDiff3DGame.AdvMath.Matrix;

namespace LinearDiff3DGame.AdvMath.MatrixUtils
{
    // построение обратной матрицы (через LUP - разложение)
    public class InverseMatrixBuilder
    {
        public Matrix.Matrix InverseMatrix(Matrix.Matrix matrixA)
        {
            if (matrixA.RowCount!=matrixA.ColumnCount)
            {
                throw new ArgumentException("A isn't the square matrix");
            }

            //n - размерность квадратной матрицы A
            Int32 n = matrixA.RowCount;
            //при инициализации задается размерность nxn
            Matrix.Matrix matrixX = new Matrix.Matrix(n, n);
            Matrix.Matrix matrixP;
            Matrix.Matrix matrixC;
            //предполагается что в результате следующего вызова матрица C = L + U - E
            LUPDecomposition(matrixA, out matrixC, out matrixP);
            for (Int32 k = n; k > 0; --k)
            {
                matrixX[k, k] = 1;
                for (Int32 j = n; j > k; --j)
                {
                    matrixX[k, k] -= matrixC[k, j] * matrixX[j, k];
                }
                matrixX[k, k] /= matrixC[k, k];

                for (Int32 i = k - 1; i > 0; --i)
                {
                    for (Int32 j = n; j > i; --j)
                    {
                        matrixX[i, k] -= matrixC[i, j] * matrixX[j, k];
                        matrixX[k, i] -= matrixC[j, i] * matrixX[k, j];
                    }
                    matrixX[i, k] /= matrixC[i, i];
                }
            }
            matrixX = matrixX * matrixP;
            return matrixX;
        }

        private static void LUPDecomposition(Matrix.Matrix matrixA, out Matrix.Matrix matrixC, out Matrix.Matrix matrixP)
        {
            Int32 n = matrixA.RowCount;

            matrixC = matrixA.Clone();
            matrixP = Matrix.Matrix.IdentityMatrix(n);

            for (Int32 i = 1; i <= n; ++i)
            {
                //поиск опорного элемента
                double pivotValue = 0;
                Int32 pivot = -1;
                for (Int32 row = i; row <= n; ++row)
                {
                    if (Math.Abs(matrixC[row, i]) > pivotValue)
                    {
                        pivotValue = Math.Abs(matrixC[row, i]);
                        pivot = row;
                    }
                }
                if (pivotValue == 0)
                {
                    throw new Exception("Матрица вырождена");
                }

                //меняем местами i-ю строку и строку с опорным элементом
                matrixP.SwapRows(pivot, i);
                matrixC.SwapRows(pivot, i);
                for (Int32 j = i + 1; j <= n; ++j)
                {
                    matrixC[j, i] /= matrixC[i, i];
                    for (Int32 k = i + 1; k <= n; ++k)
                    {
                        matrixC[j, k] -= matrixC[j, i] * matrixC[i, k];
                    }
                }
            }
        }
    }
}
