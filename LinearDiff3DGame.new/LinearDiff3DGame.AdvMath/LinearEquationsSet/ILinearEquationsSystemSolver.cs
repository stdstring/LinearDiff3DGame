namespace LinearDiff3DGame.AdvMath.LinearEquationsSet
{
    public interface ILinearEquationsSystemSolver
    {
        Matrix.Matrix Solve(Matrix.Matrix matrixA, Matrix.Matrix matrixB);
        Matrix.Matrix Solve(Matrix.Matrix matrixA, Matrix.Matrix matrixB, out Matrix.Matrix matrixError);
        Matrix.Matrix Solve(Matrix.Matrix matrixA, Matrix.Matrix matrixB, Matrix.Matrix initial);

        Matrix.Matrix Solve(Matrix.Matrix matrixA,
                            Matrix.Matrix matrixB,
                            Matrix.Matrix initial,
                            out Matrix.Matrix matrixError);
    }
}