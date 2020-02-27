using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.AdvMath
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISolver
    {
        Matrix Solve(Matrix matrixA, Matrix matrixB, out Matrix matrixError);
    }
}
