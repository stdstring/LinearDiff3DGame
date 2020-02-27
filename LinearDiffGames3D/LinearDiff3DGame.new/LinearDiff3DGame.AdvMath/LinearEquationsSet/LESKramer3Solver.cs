using System;
using LinearDiff3DGame.AdvMath.Matrix;

namespace LinearDiff3DGame.AdvMath.LinearEquationsSet
{
    // решение СЛАУ 3x3 методом Крамера
    public class LESKramer3Solver : ILinearEquationsSystemSolver
    {
        public Matrix.Matrix Solve(Matrix.Matrix matrixA, Matrix.Matrix matrixB)
        {
            Matrix.Matrix matrixError;
            return Solve(matrixA, matrixB, out matrixError);
        }

        public Matrix.Matrix Solve(Matrix.Matrix matrixA, Matrix.Matrix matrixB, out Matrix.Matrix matrixError)
        {
            if((matrixA.ColumnCount != 3) && (matrixA.RowCount != 3))
                throw new IncorrectMatrixSizeException("MatrixA must be 3x3.", "matrixA");
            if((matrixB.ColumnCount != 1) && (matrixB.RowCount != 3))
                throw new IncorrectMatrixSizeException("MatrixB must be 3x1.", "matrixB");

            Double delta = CalcDeterminant3(matrixA);

            Matrix.Matrix matrixAX = matrixA.Clone();
            matrixAX[1, 1] = matrixB[1, 1];
            matrixAX[2, 1] = matrixB[2, 1];
            matrixAX[3, 1] = matrixB[3, 1];
            Double deltaX = CalcDeterminant3(matrixAX);

            Matrix.Matrix matrixAY = matrixA.Clone();
            matrixAY[1, 2] = matrixB[1, 1];
            matrixAY[2, 2] = matrixB[2, 1];
            matrixAY[3, 2] = matrixB[3, 1];
            Double deltaY = CalcDeterminant3(matrixAY);

            Matrix.Matrix matrixAZ = matrixA.Clone();
            matrixAZ[1, 3] = matrixB[1, 1];
            matrixAZ[2, 3] = matrixB[2, 1];
            matrixAZ[3, 3] = matrixB[3, 1];
            Double deltaZ = CalcDeterminant3(matrixAZ);

            Matrix.Matrix solutionMatrix = new Matrix.Matrix(3, 1);
            solutionMatrix[1, 1] = deltaX / delta;
            solutionMatrix[2, 1] = deltaY / delta;
            solutionMatrix[3, 1] = deltaZ / delta;

            // матрица абсолютных ошибок
            matrixError = matrixA * solutionMatrix - matrixB;

            return solutionMatrix;
        }

        private static Double CalcDeterminant3(Matrix.Matrix matrix)
        {
            Double result = 0;

            result += matrix[1, 1] * (matrix[2, 2] * matrix[3, 3] - matrix[2, 3] * matrix[3, 2]);
            result += matrix[1, 2] * (matrix[2, 3] * matrix[3, 1] - matrix[2, 1] * matrix[3, 3]);
            result += matrix[1, 3] * (matrix[2, 1] * matrix[3, 2] - matrix[2, 2] * matrix[3, 1]);

            return result;
        }


        public Matrix.Matrix Solve(Matrix.Matrix matrixA, Matrix.Matrix matrixB, Matrix.Matrix initial)
        {
            throw new NotImplementedException();
        }

        public Matrix.Matrix Solve(Matrix.Matrix matrixA, Matrix.Matrix matrixB, Matrix.Matrix initial,
                                   out Matrix.Matrix matrixError)
        {
            throw new NotImplementedException();
        }
    }
}