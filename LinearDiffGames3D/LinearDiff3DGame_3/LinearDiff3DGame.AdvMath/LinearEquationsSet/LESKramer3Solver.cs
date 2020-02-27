using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.AdvMath
{
    /// <summary>
    /// класс для инкапсуляции решения СЛАУ 3x3 методом Крамера
    /// </summary>
    public class LESKramer3Solver : ISolver
    {
        /// <summary>
        /// конструктор класса LESKramer3Solver
        /// </summary>
        public LESKramer3Solver()
        {
        }

        /// <summary>
        /// метод Solve решает СЛАУ 3x3 методом Крамера
        /// </summary>
        /// <param name="matrixA">матрица A системы</param>
        /// <param name="matrixB">матрица B системы</param>
        /// <param name="matrixError">матрица абсолютных ошибок решения</param>
        /// <returns>матрица-решение СЛАУ</returns>
        public Matrix Solve(Matrix matrixA, Matrix matrixB, out Matrix matrixError)
        {
            if ((matrixA.ColumnCount != 3) && (matrixA.RowCount != 3))
            {
#warning может более специализированное исключение
                throw new ArgumentException("MatrixA must be 3x3");
            }
            if ((matrixB.ColumnCount != 1) && (matrixB.RowCount != 3))
            {
#warning может более специализированное исключение
                throw new ArgumentException("MatrixB must be 3x1");
            }

            Double delta = CalcDeterminant3(matrixA);

            Matrix matrixAX = matrixA.Clone();
            matrixAX[1, 1] = matrixB[1, 1];
            matrixAX[2, 1] = matrixB[2, 1];
            matrixAX[3, 1] = matrixB[3, 1];
            Double deltaX = CalcDeterminant3(matrixAX);

            Matrix matrixAY = matrixA.Clone();
            matrixAY[1, 2] = matrixB[1, 1];
            matrixAY[2, 2] = matrixB[2, 1];
            matrixAY[3, 2] = matrixB[3, 1];
            Double deltaY = CalcDeterminant3(matrixAY);

            Matrix matrixAZ = matrixA.Clone();
            matrixAZ[1, 3] = matrixB[1, 1];
            matrixAZ[2, 3] = matrixB[2, 1];
            matrixAZ[3, 3] = matrixB[3, 1];
            Double deltaZ = CalcDeterminant3(matrixAZ);

            Matrix solutionMatrix = new Matrix(3, 1);
            solutionMatrix[1, 1] = deltaX / delta;
            solutionMatrix[2, 1] = deltaY / delta;
            solutionMatrix[3, 1] = deltaZ / delta;

            // матрица абсолютных ошибок
            matrixError = matrixA * solutionMatrix - matrixB;

            return solutionMatrix;
        }

        /// <summary>
        /// метод CalcDeterminant3 вычисляет определитель матрицы matrix размером 3x3
        /// </summary>
        /// <param name="matrix">исходная матрица</param>
        /// <returns>определитель матрицы matrix</returns>
        private Double CalcDeterminant3(Matrix matrix)
        {
            Double result = 0;

            result += matrix[1, 1] * (matrix[2, 2] * matrix[3, 3] - matrix[2, 3] * matrix[3, 2]);
            result += matrix[1, 2] * (matrix[2, 3] * matrix[3, 1] - matrix[2, 1] * matrix[3, 3]);
            result += matrix[1, 3] * (matrix[2, 1] * matrix[3, 2] - matrix[2, 2] * matrix[3, 1]);

            return result;
        }
    }
}
