using System;
using LinearDiff3DGame.AdvMath.Matrix;

namespace LinearDiff3DGame.AdvMath.LinearEquationsSet
{
    public class LESGaussSolver : ILinearEquationsSystemSolver
    {
        public Matrix.Matrix Solve(Matrix.Matrix matrixA, Matrix.Matrix matrixB)
        {
            Matrix.Matrix matrixError;
            return Solve(matrixA, matrixB, out matrixError);
        }

        public Matrix.Matrix Solve(Matrix.Matrix matrixA, Matrix.Matrix matrixB, out Matrix.Matrix matrixError)
        {
            if(matrixA.RowCount != matrixA.ColumnCount)
                throw new IncorrectMatrixSizeException("matrixA must be square.", "matrixA");
            if(matrixB.ColumnCount != 1)
                throw new IncorrectMatrixSizeException("matrixB must be column.", "matrixB");
            if(matrixA.RowCount != matrixB.RowCount)
                throw new IncorrectMatrixSizeException("matrixA and matrixB must have equivalent row's count.");

            throw new NotImplementedException("not implemented yet !!!");
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