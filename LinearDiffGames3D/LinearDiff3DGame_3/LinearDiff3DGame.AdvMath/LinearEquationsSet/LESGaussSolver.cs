using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.AdvMath
{
    /// <summary>
    /// 
    /// </summary>
    public class LESGaussSolver : ISolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <param name="matrixError"></param>
        /// <returns></returns>
        public Matrix Solve(Matrix matrixA, Matrix matrixB, out Matrix matrixError)
        {
            if (matrixA.RowCount != matrixA.ColumnCount)
            {
#warning ����� ����� ������������������ ����������
                throw new ArgumentException("matrixA must be square !!!");
            }
            if (matrixB.ColumnCount != 1)
            {
#warning ����� ����� ������������������ ����������
                throw new ArgumentException("matrixB must be column !!!");
            }
            if (matrixA.RowCount != matrixB.RowCount)
            {
#warning ����� ����� ������������������ ����������
                throw new ArgumentException("matrixA and matrixB must have equivalent row's count !!!");
            }

            throw new NotImplementedException("not implemented yet !!!");
        }
    }
}
