namespace LinearDiff3DGame.AdvMath.LinearEquationsSet
{
	/// <summary>
	/// 
	/// </summary>
	public interface ILinearEquationsSystemSolver
	{
		Matrix Solve(Matrix matrixA, Matrix matrixB);
		Matrix Solve(Matrix matrixA, Matrix matrixB, out Matrix matrixError);
	}
}