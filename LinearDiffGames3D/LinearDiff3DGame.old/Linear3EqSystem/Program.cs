using System;
using System.Collections.Generic;
using System.Text;
using MathPostgraduateStudy.LinearDiff3DGame;

namespace MathPostgraduateStudy.Linear3EqSystem
{
    class Program
    {
        private static Matrix SolveLinear3EqSystem(Matrix MA, Matrix MB, out Matrix MError)
        {
            Matrix MatrixA = MA.Clone();
            Matrix MatrixB = MB.Clone();

            Int32 RowCount = MatrixA.RowCount;

            for (Int32 RowIndex1 = 1; RowIndex1 <= RowCount; RowIndex1++)
            {
                //
                Double MaxElementAbsValue = Math.Abs(MatrixA[RowIndex1, RowIndex1]);
                Int32 MaxElementRowIndex = RowIndex1;
                //
                for (Int32 RowIndex2 = RowIndex1 + 1; RowIndex2 <= RowCount; RowIndex2++)
                {
                    Double CurrentElementAbsValue = Math.Abs(MatrixA[RowIndex2, RowIndex1]);
                    if (MaxElementAbsValue < CurrentElementAbsValue)
                    {
                        MaxElementAbsValue = CurrentElementAbsValue;
                        MaxElementRowIndex = RowIndex2;
                    }
                }
                //
                if (MaxElementRowIndex != RowIndex1)
                {
                    Matrix Row1 = MatrixA.GetMatrixRow(RowIndex1);
                    Matrix MaxElementRow = MatrixA.GetMatrixRow(MaxElementRowIndex);
                    MatrixA.SetMatrixRow(RowIndex1, MaxElementRow);
                    MatrixA.SetMatrixRow(MaxElementRowIndex, Row1);

                    Double TempValue = MatrixB[RowIndex1, 1];
                    MatrixB[RowIndex1, 1] = MatrixB[MaxElementRowIndex, 1];
                    MatrixB[MaxElementRowIndex, 1] = TempValue;
                }

                //
                Matrix MatrixARow1 = MatrixA.GetMatrixRow(RowIndex1);
                Double MatrixBRow1 = MatrixB[RowIndex1, 1];
                //
                for (Int32 RowIndex2 = RowIndex1 + 1; RowIndex2 <= RowCount; RowIndex2++)
                {
                    Double Koeff = MatrixA[RowIndex2, RowIndex1] / MatrixA[RowIndex1, RowIndex1];

                    Matrix MatrixACurrentRow = MatrixA.GetMatrixRow(RowIndex2);
                    MatrixA.SetMatrixRow(RowIndex2, MatrixACurrentRow - Koeff * MatrixARow1);

                    MatrixB[RowIndex2, 1] -= Koeff * MatrixBRow1;
                }
            }

            Matrix SolutionMatrix = new Matrix(3, 1);

            //
            SolutionMatrix[3, 1] = MatrixB[3, 1] / MatrixA[3, 3];
            SolutionMatrix[2, 1] = (MatrixB[2, 1] - MatrixA[2, 3] * SolutionMatrix[3, 1]) / MatrixA[2, 2];
            SolutionMatrix[1, 1] = (MatrixB[1, 1] - MatrixA[1, 3] * SolutionMatrix[3, 1] - MatrixA[1, 2] * SolutionMatrix[2, 1]) / MatrixA[1, 1];

            //
            MError = MatrixA * SolutionMatrix - MatrixB;

            return SolutionMatrix;
        }

        static void Main(string[] args)
        {
            Matrix MA = new Matrix(3, 3);
            MA[1, 1] = 2;
            MA[1, 2] = -9;
            MA[1, 3] = 5;
            MA[2, 1] = 1.2;
            MA[2, 2] = -5.3999;
            MA[2, 3] = 6;
            MA[3, 1] = 1;
            MA[3, 2] = -1;
            MA[3, 3] = -7.5;
            Matrix MB = new Matrix(3, 1);
            MB[1, 1] = -4;
            MB[2, 1] = 0.6001;
            MB[3, 1] = -8.5;

            Matrix MError = null;
            Matrix Solution = SolveLinear3EqSystem(MA, MB, out MError);

            Double Delta1 = Math.Abs(MError[1, 1]) + Math.Abs(MError[2, 1]) + Math.Abs(MError[3, 1]);
            Delta1 /= (Math.Abs(Solution[1, 1]) + Math.Abs(Solution[2, 1]) + Math.Abs(Solution[3, 1]));
            Double Delta2 = Math.Sqrt(MError[1, 1] * MError[1, 1] + MError[2, 1] * MError[2, 1] + MError[3, 1] * MError[3, 1]);
            Delta2 /= Math.Sqrt(Solution[1, 1] * Solution[1, 1] + Solution[2, 1] * Solution[2, 1] + Solution[3, 1] * Solution[3, 1]);

            Console.WriteLine("Solution :");
            Console.WriteLine("x1 = {0}", Solution[1, 1]);
            Console.WriteLine("x2 = {0}", Solution[2, 1]);
            Console.WriteLine("x3 = {0}", Solution[3, 1]);

            Console.WriteLine("Error :");
            Console.WriteLine("Delta1 = {0}", Delta1);
            Console.WriteLine("Delta2 = {0}", Delta2);

            Console.ReadLine();
        }
    }
}
