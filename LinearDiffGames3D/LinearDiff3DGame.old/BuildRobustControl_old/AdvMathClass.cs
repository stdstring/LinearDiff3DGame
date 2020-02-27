using System;
using System.Collections.Generic;
using MathPostgraduateStudy.LinearDiff3DGame;

namespace MathPostgraduateStudy.BuildRobustControl
{
    /// <summary>
    /// класс AdvMathClass содержит методы для решения продвинутых математических задач
    /// </summary>
    public class AdvMathClass
    {
        /// <summary>
        /// метод SolveEquationSystem3Gauss решает систему уравнений 3x3 методом Гаусса
        /// </summary>
        /// <param name="MA">матрица A</param>
        /// <param name="MB">матрица B</param>
        /// <param name="MError">матрица абсолютных ошибок</param>
        /// <returns>решение</returns>
        public static Matrix SolveEquationSystem3Gauss(Matrix MA, Matrix MB, out Matrix MError)
        {
            // по идее надо проверить правильность алгоритма !!!
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

        /// <summary>
        /// метод CalcDeterminant3 вычисляет определитель матрицы MatrixA размером 3x3
        /// </summary>
        /// <param name="MatrixA"></param>
        /// <returns></returns>
        public static Double CalcDeterminant3(Matrix MatrixA)
        {
            Double Result = 0;

            Result += MatrixA[1, 1] * (MatrixA[2, 2] * MatrixA[3, 3] - MatrixA[2, 3] * MatrixA[3, 2]);
            Result += MatrixA[1, 2] * (MatrixA[2, 3] * MatrixA[3, 1] - MatrixA[2, 1] * MatrixA[3, 3]);
            Result += MatrixA[1, 3] * (MatrixA[2, 1] * MatrixA[3, 2] - MatrixA[2, 2] * MatrixA[3, 1]);

            return Result;
        }

        /// <summary>
        /// метод SolveEquationSystem3Kramer решает систему лин. уравнений (3x3) методом Крамера
        /// </summary>
        /// <param name="MatrixA"></param>
        /// <param name="MatrixB"></param>
        /// <returns></returns>
        public static Matrix SolveEquationSystem3Kramer(Matrix MatrixA, Matrix MatrixB)
        {
            if ((MatrixA.ColumnCount != 3) && (MatrixA.RowCount != 3))
            {
                throw new ArgumentException("MatrixA must be 3x3");
            }
            if ((MatrixB.ColumnCount != 1) && (MatrixB.RowCount != 3))
            {
                throw new ArgumentException("MatrixB must be 3x1");
            }

            Double Delta = CalcDeterminant3(MatrixA);

            Matrix MatrixAX = MatrixA.Clone();
            MatrixAX[1, 1] = MatrixB[1, 1];
            MatrixAX[2, 1] = MatrixB[2, 1];
            MatrixAX[3, 1] = MatrixB[3, 1];
            Double DeltaX = CalcDeterminant3(MatrixAX);

            Matrix MatrixAY = MatrixA.Clone();
            MatrixAY[1, 2] = MatrixB[1, 1];
            MatrixAY[2, 2] = MatrixB[2, 1];
            MatrixAY[3, 2] = MatrixB[3, 1];
            Double DeltaY = CalcDeterminant3(MatrixAY);

            Matrix MatrixAZ = MatrixA.Clone();
            MatrixAZ[1, 3] = MatrixB[1, 1];
            MatrixAZ[2, 3] = MatrixB[2, 1];
            MatrixAZ[3, 3] = MatrixB[3, 1];
            Double DeltaZ = CalcDeterminant3(MatrixAZ);

            Matrix SolutionMatrix = new Matrix(3, 1);
            SolutionMatrix[1, 1] = DeltaX / Delta;
            SolutionMatrix[2, 1] = DeltaY / Delta;
            SolutionMatrix[3, 1] = DeltaZ / Delta;

            return SolutionMatrix;
        }

        /// <summary>
        /// метод CalcPlaneLineCrossingPoint возвращает точку пересечения плоскости и прямой
        /// плоскость задается каноническим уравнением (Ax + By + Cz + D = 0)
        /// прямая задается 2-мя точками: linePoint1 и linePoint2
        /// </summary>
        /// <param name="plane">плоскость</param>
        /// <param name="linePoint1">1-я точка прямой</param>
        /// <param name="linePoint2">2-я точка прямой</param>
        /// <returns>точка пересечения плоскости и прямой</returns>
        public static Point3D CalcPlaneLineCrossingPoint(PlaneClass plane, Point3D linePoint1, Point3D linePoint2)
        {
            Vector3D directionVector = new Vector3D(linePoint2.XCoord - linePoint1.XCoord,
                                                    linePoint2.YCoord - linePoint1.YCoord,
                                                    linePoint2.ZCoord - linePoint1.ZCoord);

            return CalcPlaneLineCrossingPoint(plane, linePoint1, directionVector);
        }

        /// <summary>
        /// метод CalcPlaneLineCrossingPoint возвращает точку пересечения плоскости и прямой
        /// плоскость задается каноническим уравнением (Ax + By + Cz + D = 0)
        /// прямая задается точкой linePoint0 и направляющим вектором directionVector
        /// </summary>
        /// <param name="plane">плоскость</param>
        /// <param name="linePoint0">точка прямой</param>
        /// <param name="directionVector">направляющий вектор прямой</param>
        /// <returns>точка пересечения плоскости и прямой</returns>
        public static Point3D CalcPlaneLineCrossingPoint(PlaneClass plane, Point3D linePoint0, Vector3D directionVector)
        {
            Matrix matrixA = new Matrix(3, 3);
            Matrix matrixB = new Matrix(3, 1);

            matrixA[1, 1] = plane.AKoeff;
            matrixA[1, 2] = plane.BKoeff;
            matrixA[1, 3] = plane.CKoeff;
            matrixB[1, 1] = -plane.DKoeff;

            if ((Math.Abs(directionVector.XCoord) > m_Epsilon) || (Math.Abs(directionVector.YCoord) > m_Epsilon))
            {
                matrixA[2, 1] = directionVector.YCoord;
                matrixA[2, 2] = -directionVector.XCoord;
                matrixA[2, 3] = 0;
                matrixB[2, 1] = directionVector.YCoord * linePoint0.XCoord - directionVector.XCoord * linePoint0.YCoord;
            }
            else
            {
                matrixA[2, 1] = 0;
                matrixA[2, 2] = directionVector.ZCoord;
                matrixA[2, 3] = -directionVector.YCoord;
                matrixB[2, 1] = directionVector.ZCoord * linePoint0.YCoord - directionVector.YCoord * linePoint0.ZCoord;
            }

            if ((Math.Abs(directionVector.XCoord) > m_Epsilon) || (Math.Abs(directionVector.ZCoord) > m_Epsilon))
            {
                matrixA[3, 1] = directionVector.ZCoord;
                matrixA[3, 2] = 0;
                matrixA[3, 3] = -directionVector.XCoord;
                matrixB[3, 1] = directionVector.ZCoord * linePoint0.XCoord - directionVector.XCoord * linePoint0.ZCoord;
            }
            else
            {
                matrixA[3, 1] = 0;
                matrixA[3, 2] = directionVector.ZCoord;
                matrixA[3, 3] = -directionVector.YCoord;
                matrixB[3, 1] = directionVector.ZCoord * linePoint0.YCoord - directionVector.YCoord * linePoint0.ZCoord;
            }

            Matrix matrixError = null;
            Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
            //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);

            return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
        }

        /// <summary>
        /// метод CalcLineLineCrossingPoint возвращает точку пересечения двух прямых
        /// обе прямые задаются двумя точками: line1Point1, line1Point2 и line2Point1, line2Point2 соответственно
        /// </summary>
        /// <param name="line1Point1">1-я точка 1-й прямой</param>
        /// <param name="line1Point2">2-я точка 1-й прямой</param>
        /// <param name="line2Point1">1-я точка 2-й прямой</param>
        /// <param name="line2Point2">2-я точка 2-й прямой</param>
        /// <returns>точка пересечения двух прямых</returns>
        public static Point3D CalcLineLineCrossingPoint(Point3D line1Point1, Point3D line1Point2, Point3D line2Point1, Point3D line2Point2)
        {
            Vector3D line1DirectionVector = new Vector3D(line1Point2.XCoord - line1Point1.XCoord,
                                                         line1Point2.YCoord - line1Point1.YCoord,
                                                         line1Point2.ZCoord - line1Point1.ZCoord);

            return CalcLineLineCrossingPoint(line1Point1, line1DirectionVector, line2Point1, line2Point2);
        }

        /// <summary>
        /// метод CalcLineLineCrossingPoint возвращает точку пересечения двух прямых
        /// 1-я прямая задается точкой line1Point0 и направляющим вектором line1DirectionVector
        /// 2-я прямая задается двумя точками: line2Point1 и line2Point2
        /// </summary>
        /// <param name="line1Point0">точка 1-й прямой</param>
        /// <param name="line1DirectionVector">направляющий вектор 1-й прямой</param>
        /// <param name="line2Point1">1-я точка 2-й прямой</param>
        /// <param name="line2Point2">2-я точка 2-й прямой</param>
        /// <returns>точка пересечения двух прямых</returns>
        public static Point3D CalcLineLineCrossingPoint(Point3D line1Point0, Vector3D line1DirectionVector, Point3D line2Point1, Point3D line2Point2)
        {
            // пока не придумал чегото более умного просто перебираю все возможные варианты построения матрицы A
            Matrix matrixA = new Matrix(3, 3);
            Double matrixADet = 0;

            Matrix matrixB = new Matrix(3, 1);

            Matrix line1Row1 = new Matrix(1, 3);
            Matrix line1Row2 = new Matrix(1, 3);
            Matrix line1Row3 = new Matrix(1, 3);
            Matrix line2Row1 = new Matrix(1, 3);
            Matrix line2Row2 = new Matrix(1, 3);
            Matrix line2Row3 = new Matrix(1, 3);

            Double line1B1;
            Double line1B2;
            Double line1B3;
            Double line2B1;
            Double line2B2;
            Double line2B3;

            line1Row1[1, 1] = line1DirectionVector.YCoord;
            line1Row1[1, 2] = -line1DirectionVector.XCoord;
            line1Row1[1, 3] = 0;
            line1B1 = line1DirectionVector.YCoord * line1Point0.XCoord - line1DirectionVector.XCoord * line1Point0.YCoord;

            line1Row2[1, 1] = line1DirectionVector.ZCoord;
            line1Row2[1, 2] = 0;
            line1Row2[1, 3] = -line1DirectionVector.XCoord;
            line1B2 = line1DirectionVector.ZCoord * line1Point0.XCoord - line1DirectionVector.XCoord * line1Point0.ZCoord;

            line1Row3[1, 1] = 0;
            line1Row3[1, 2] = line1DirectionVector.ZCoord;
            line1Row3[1, 3] = -line1DirectionVector.YCoord;
            line1B3 = line1DirectionVector.ZCoord * line1Point0.YCoord - line1DirectionVector.YCoord * line1Point0.ZCoord;

            Double deltaX = line2Point2.XCoord - line2Point1.XCoord;
            Double deltaY = line2Point2.YCoord - line2Point1.YCoord;
            Double deltaZ = line2Point2.ZCoord - line2Point1.ZCoord;

            line2Row1[1, 1] = deltaY;
            line2Row1[1, 2] = -deltaX;
            line2Row1[1, 3] = 0;
            line2B1 = deltaY * line2Point1.XCoord - deltaX * line2Point1.YCoord;

            line2Row2[1, 1] = deltaZ;
            line2Row2[1, 2] = 0;
            line2Row2[1, 3] = -deltaX;
            line2B2 = deltaZ * line2Point1.XCoord - deltaX * line2Point1.ZCoord;

            line2Row3[1, 1] = 0;
            line2Row3[1, 2] = deltaZ;
            line2Row3[1, 3] = -deltaY;
            line2B3 = deltaZ * line2Point1.YCoord - deltaY * line2Point1.ZCoord;

            // вариант 1
            matrixA.SetMatrixRow(1, line1Row1);
            matrixA.SetMatrixRow(2, line1Row2);
            matrixA.SetMatrixRow(3, line2Row1);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B1;
                matrixB[2, 1] = line1B2;
                matrixB[3, 1] = line2B1;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 2
            matrixA.SetMatrixRow(1, line1Row1);
            matrixA.SetMatrixRow(2, line1Row2);
            matrixA.SetMatrixRow(3, line2Row2);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B1;
                matrixB[2, 1] = line1B2;
                matrixB[3, 1] = line2B2;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 3
            matrixA.SetMatrixRow(1, line1Row1);
            matrixA.SetMatrixRow(2, line1Row2);
            matrixA.SetMatrixRow(3, line2Row3);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B1;
                matrixB[2, 1] = line1B2;
                matrixB[3, 1] = line2B3;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 4
            matrixA.SetMatrixRow(1, line1Row1);
            matrixA.SetMatrixRow(2, line1Row3);
            matrixA.SetMatrixRow(3, line2Row1);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B1;
                matrixB[2, 1] = line1B3;
                matrixB[3, 1] = line2B1;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 5
            matrixA.SetMatrixRow(1, line1Row1);
            matrixA.SetMatrixRow(2, line1Row3);
            matrixA.SetMatrixRow(3, line2Row2);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B1;
                matrixB[2, 1] = line1B3;
                matrixB[3, 1] = line2B2;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 6
            matrixA.SetMatrixRow(1, line1Row1);
            matrixA.SetMatrixRow(2, line1Row3);
            matrixA.SetMatrixRow(3, line2Row3);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B1;
                matrixB[2, 1] = line1B3;
                matrixB[3, 1] = line2B3;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 7
            matrixA.SetMatrixRow(1, line1Row2);
            matrixA.SetMatrixRow(2, line1Row3);
            matrixA.SetMatrixRow(3, line2Row1);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B2;
                matrixB[2, 1] = line1B3;
                matrixB[3, 1] = line2B1;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 8
            matrixA.SetMatrixRow(1, line1Row2);
            matrixA.SetMatrixRow(2, line1Row3);
            matrixA.SetMatrixRow(3, line2Row2);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B2;
                matrixB[2, 1] = line1B3;
                matrixB[3, 1] = line2B2;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 9
            matrixA.SetMatrixRow(1, line1Row2);
            matrixA.SetMatrixRow(2, line1Row3);
            matrixA.SetMatrixRow(3, line2Row3);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B2;
                matrixB[2, 1] = line1B3;
                matrixB[3, 1] = line2B3;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 10
            matrixA.SetMatrixRow(1, line1Row1);
            matrixA.SetMatrixRow(2, line2Row1);
            matrixA.SetMatrixRow(3, line2Row2);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B1;
                matrixB[2, 1] = line2B1;
                matrixB[3, 1] = line2B2;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 11
            matrixA.SetMatrixRow(1, line1Row1);
            matrixA.SetMatrixRow(2, line2Row1);
            matrixA.SetMatrixRow(3, line2Row3);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B1;
                matrixB[2, 1] = line2B1;
                matrixB[3, 1] = line2B3;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 12
            matrixA.SetMatrixRow(1, line1Row1);
            matrixA.SetMatrixRow(2, line2Row2);
            matrixA.SetMatrixRow(3, line2Row3);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B1;
                matrixB[2, 1] = line2B2;
                matrixB[3, 1] = line2B3;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 13
            matrixA.SetMatrixRow(1, line1Row2);
            matrixA.SetMatrixRow(2, line2Row1);
            matrixA.SetMatrixRow(3, line2Row2);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B2;
                matrixB[2, 1] = line2B1;
                matrixB[3, 1] = line2B2;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 14
            matrixA.SetMatrixRow(1, line1Row2);
            matrixA.SetMatrixRow(2, line2Row1);
            matrixA.SetMatrixRow(3, line2Row3);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B2;
                matrixB[2, 1] = line2B1;
                matrixB[3, 1] = line2B3;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 15
            matrixA.SetMatrixRow(1, line1Row2);
            matrixA.SetMatrixRow(2, line2Row2);
            matrixA.SetMatrixRow(3, line2Row3);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B2;
                matrixB[2, 1] = line2B2;
                matrixB[3, 1] = line2B3;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 16
            matrixA.SetMatrixRow(1, line1Row3);
            matrixA.SetMatrixRow(2, line2Row1);
            matrixA.SetMatrixRow(3, line2Row2);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B3;
                matrixB[2, 1] = line2B1;
                matrixB[3, 1] = line2B2;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 17
            matrixA.SetMatrixRow(1, line1Row3);
            matrixA.SetMatrixRow(2, line2Row1);
            matrixA.SetMatrixRow(3, line2Row3);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B3;
                matrixB[2, 1] = line2B1;
                matrixB[3, 1] = line2B3;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            // вариант 18
            matrixA.SetMatrixRow(1, line1Row3);
            matrixA.SetMatrixRow(2, line2Row2);
            matrixA.SetMatrixRow(3, line2Row3);
            matrixADet = CalcDeterminant3(matrixA);
            if (Math.Abs(matrixADet) > m_Epsilon)
            {
                matrixB[1, 1] = line1B3;
                matrixB[2, 1] = line2B2;
                matrixB[3, 1] = line2B3;

                Matrix matrixError = null;
                Matrix solution = SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
                //Matrix solution = SolveEquationSystem3Kramer(matrixA, matrixB);
                return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
            }

            throw new Exception("PEZDEC ... Can't find crossing point for two line !!!");
        }

        /// <summary>
        /// метод CalcPerpendicularLinesCrossingPoint возвращает точку пересечения двух перпендикулярных прямых
        /// через нахождения минимума расстояния от точки line1Point0 до любой точки на прямой (см. док.)
        /// </summary>
        /// <param name="line1point0"></param>
        /// <param name="line2Point1"></param>
        /// <param name="line2Point2"></param>
        /// <returns></returns>
        public static Point3D CalcPerpendicularLinesCrossingPoint(Point3D line1Point0, Point3D line2Point1, Point3D line2Point2)
        {
            Vector3D lineDirection = new Vector3D(line2Point2.XCoord - line2Point1.XCoord,
                                                  line2Point2.YCoord - line2Point1.YCoord,
                                                  line2Point2.ZCoord - line2Point1.ZCoord);

            if (lineDirection.Length <= MinLineDirectionLength)
            {
                return new Point3D((line2Point2.XCoord + line2Point1.XCoord) / 2,
                                   (line2Point2.YCoord + line2Point1.YCoord) / 2,
                                   (line2Point2.ZCoord + line2Point1.ZCoord) / 2);
            }
            else
            {
                Vector3D r0 = new Vector3D(line1Point0.XCoord, line1Point0.YCoord, line1Point0.ZCoord);
                Vector3D r1 = new Vector3D(line2Point1.XCoord, line2Point1.YCoord, line2Point1.ZCoord);

                Double paramt = (Vector3D.ScalarProduct(lineDirection, r0) - Vector3D.ScalarProduct(lineDirection, r1)) / Vector3D.ScalarProduct(lineDirection, lineDirection);

                return new Point3D(line2Point1.XCoord + paramt * lineDirection.XCoord,
                                   line2Point1.YCoord + paramt * lineDirection.YCoord,
                                   line2Point1.ZCoord + paramt * lineDirection.ZCoord);
            }
        }

        /// <summary>
        /// метод DistanceBetween2Points возвращает расстояние между двумя точками point1 и point2
        /// </summary>
        /// <param name="point1">первая точка</param>
        /// <param name="point2">вторая точка</param>
        /// <returns>расстояние между двумя точками point1 и point2</returns>
        public static Double DistanceBetween2Points(Point3D point1, Point3D point2)
        {
            return Math.Sqrt((point1.XCoord - point2.XCoord) * (point1.XCoord - point2.XCoord) +
                             (point1.YCoord - point2.YCoord) * (point1.YCoord - point2.YCoord) +
                             (point1.ZCoord - point2.ZCoord) * (point1.ZCoord - point2.ZCoord));
        }

        /// <summary>
        /// Epsilon - ...
        /// </summary>
        private const Double m_Epsilon = 1e-9;
        /// <summary>
        /// 
        /// </summary>
        private const Double MinLineDirectionLength = 1e-4;
    }
}
