using System;
using System.Collections.Generic;
using System.Text;

using MathPostgraduateStudy.LinearDiff3DGame;

namespace InputDataTest
{
    class Program
    {
        static void Main(string[] args)
        {
            InputDataReader reader = new InputDataReader(m_InputDataFileName);

            Console.WriteLine("MatrixA : ");
            Matrix matrixA = reader.MatrixA;
            for (Int32 rowIndex = 1; rowIndex <= matrixA.RowCount; rowIndex++)
            {
                for (Int32 columnIndex = 1; columnIndex <= matrixA.ColumnCount; columnIndex++)
                {
                    Console.WriteLine("A[{0}, {1}] = {2}", rowIndex, columnIndex, matrixA[rowIndex, columnIndex]);
                }
            }
            Console.WriteLine();

            Console.WriteLine("MatrixB : ");
            Matrix matrixB = reader.MatrixB;
            for (Int32 rowIndex = 1; rowIndex <= matrixB.RowCount; rowIndex++)
            {
                for (Int32 columnIndex = 1; columnIndex <= matrixB.ColumnCount; columnIndex++)
                {
                    Console.WriteLine("B[{0}, {1}] = {2}", rowIndex, columnIndex, matrixB[rowIndex, columnIndex]);
                }
            }
            Console.WriteLine();

            Console.WriteLine("MatrixC : ");
            Matrix matrixC = reader.MatrixC;
            for (Int32 rowIndex = 1; rowIndex <= matrixC.RowCount; rowIndex++)
            {
                for (Int32 columnIndex = 1; columnIndex <= matrixC.ColumnCount; columnIndex++)
                {
                    Console.WriteLine("C[{0}, {1}] = {2}", rowIndex, columnIndex, matrixC[rowIndex, columnIndex]);
                }
            }

            Console.ReadLine();
        }

        /// <summary>
        /// 
        /// </summary>
        private const String m_InputDataFileName = "InputData.xml";
    }
}
