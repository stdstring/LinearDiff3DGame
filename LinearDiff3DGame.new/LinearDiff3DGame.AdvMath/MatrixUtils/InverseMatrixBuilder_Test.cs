using System;
using LinearDiff3DGame.AdvMath.Common;
using NUnit.Framework;

namespace LinearDiff3DGame.AdvMath.MatrixUtils
{
    [TestFixture]
    public class InverseMatrixBuilder_Test
    {
        [Test]
        public void TestInverseMatrixBuilding()
        {
            const Double epsilon = 1e-9;
            ApproxComp ac = new ApproxComp(epsilon);

            Matrix.Matrix identityMatrix = Matrix.Matrix.IdentityMatrix(3);

            Matrix.Matrix matrixA1 = new Matrix.Matrix(3, 3);
            matrixA1[1, 1] = 2;
            matrixA1[1, 2] = 1;
            matrixA1[1, 3] = -1;
            matrixA1[2, 1] = 5;
            matrixA1[2, 2] = 2;
            matrixA1[2, 3] = 4;
            matrixA1[3, 1] = 7;
            matrixA1[3, 2] = 3;
            matrixA1[3, 3] = 2;

            Matrix.Matrix inverseMatrix1 = new InverseMatrixBuilder().InverseMatrix(matrixA1);

            Matrix.Matrix deltaMatrix1 = (matrixA1 * inverseMatrix1) - identityMatrix;
            Double euclidNorm1 = CalcEuclidNorm(deltaMatrix1);

            Assert.IsTrue(ac.LE(euclidNorm1, 0));

            Matrix.Matrix matrixA2 = new Matrix.Matrix(3, 3);
            matrixA2[1, 1] = 20;
            matrixA2[1, 2] = 11;
            matrixA2[1, 3] = -1;
            matrixA2[2, 1] = 51;
            matrixA2[2, 2] = 12;
            matrixA2[2, 3] = 43;
            matrixA2[3, 1] = 70;
            matrixA2[3, 2] = 13;
            matrixA2[3, 3] = 211;

            Matrix.Matrix inverseMatrix2 = new InverseMatrixBuilder().InverseMatrix(matrixA2);

            Matrix.Matrix deltaMatrix2 = (matrixA2 * inverseMatrix2) - identityMatrix;
            Double euclidNorm2 = CalcEuclidNorm(deltaMatrix2);

            Assert.IsTrue(ac.LE(euclidNorm2, 0));
        }

        private static Double CalcEuclidNorm(Matrix.Matrix source)
        {
            Double euclidNorm = 0;

            for (Int32 rowIndex = 1; rowIndex <= source.RowCount; ++rowIndex)
            {
                for (Int32 columnIndex = 1; columnIndex <= source.ColumnCount; ++columnIndex)
                {
                    euclidNorm += source[rowIndex, columnIndex] * source[rowIndex, columnIndex];
                }
            }

            return Math.Sqrt(euclidNorm);
        }
    }
}
