using System;

namespace LinearDiff3DGame.AdvMath.LinearEquationsSet
{
    internal class LESGaussZeidelSolver : ILinearEquationsSystemSolver
    {
        public Matrix.Matrix Solve(Matrix.Matrix matrixA, Matrix.Matrix matrixB)
        {
            throw new NotImplementedException();
        }

        public Matrix.Matrix Solve(Matrix.Matrix matrixA, Matrix.Matrix matrixB, out Matrix.Matrix matrixError)
        {
            throw new NotImplementedException();
        }


        public Matrix.Matrix Solve(Matrix.Matrix matrixA, Matrix.Matrix matrixB, Matrix.Matrix initial)
        {
            throw new NotImplementedException();
        }

        public Matrix.Matrix Solve(Matrix.Matrix matrixA,
                                   Matrix.Matrix matrixB,
                                   Matrix.Matrix initial,
                                   out Matrix.Matrix matrixError)
        {
            throw new NotImplementedException();
        }
    }
}