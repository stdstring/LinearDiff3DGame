using System;
using System.Collections.Generic;
using System.Text;

using MathPostgraduateStudy.LinearDiff3DGame;

namespace MathPostgraduateStudy.BuildRobustControl
{
    /// <summary>
    /// класс AdvMath содержит методы для решения продвинутых математических задач
    /// </summary>
    public class AdvMath
    {
        /// <summary>
        /// метод SolveEquationSystem3Gauss решает систему уравнений 3x3 методом Гаусса
        /// </summary>
        /// <param name="mA">матрица A</param>
        /// <param name="mB">матрица B</param>
        /// <param name="mError">матрица абсолютных ошибок</param>
        /// <returns>решение</returns>
        public static Matrix SolveEquationSystem3Gauss(Matrix mA, Matrix mB, out Matrix mError)
        {
            // по идее надо проверить правильность алгоритма !!!
            Matrix matrixA = mA.Clone();
            Matrix matrixB = mB.Clone();

            Int32 rowCount = matrixA.RowCount;

            for (Int32 rowIndex1 = 1; rowIndex1 <= rowCount; rowIndex1++)
            {
                //
                Double maxElementAbsValue = Math.Abs(matrixA[rowIndex1, rowIndex1]);
                Int32 maxElementRowIndex = rowIndex1;
                //
                for (Int32 rowIndex2 = rowIndex1 + 1; rowIndex2 <= rowCount; rowIndex2++)
                {
                    Double currentElementAbsValue = Math.Abs(matrixA[rowIndex2, rowIndex1]);
                    if (maxElementAbsValue < currentElementAbsValue)
                    {
                        maxElementAbsValue = currentElementAbsValue;
                        maxElementRowIndex = rowIndex2;
                    }
                }
                //
                if (maxElementRowIndex != rowIndex1)
                {
                    Matrix row1 = matrixA.GetMatrixRow(rowIndex1);
                    Matrix maxElementRow = matrixA.GetMatrixRow(maxElementRowIndex);
                    matrixA.SetMatrixRow(rowIndex1, maxElementRow);
                    matrixA.SetMatrixRow(maxElementRowIndex, row1);

                    Double tempValue = matrixB[rowIndex1, 1];
                    matrixB[rowIndex1, 1] = matrixB[maxElementRowIndex, 1];
                    matrixB[maxElementRowIndex, 1] = tempValue;
                }

                //
                Matrix matrixARow1 = matrixA.GetMatrixRow(rowIndex1);
                Double matrixBRow1 = matrixB[rowIndex1, 1];
                //
                for (Int32 rowIndex2 = rowIndex1 + 1; rowIndex2 <= rowCount; rowIndex2++)
                {
                    Double koeff = matrixA[rowIndex2, rowIndex1] / matrixA[rowIndex1, rowIndex1];

                    Matrix matrixACurrentRow = matrixA.GetMatrixRow(rowIndex2);
                    matrixA.SetMatrixRow(rowIndex2, matrixACurrentRow - koeff * matrixARow1);

                    matrixB[rowIndex2, 1] -= koeff * matrixBRow1;
                }
            }

            Matrix solutionMatrix = new Matrix(3, 1);

            //
            solutionMatrix[3, 1] = matrixB[3, 1] / matrixA[3, 3];
            solutionMatrix[2, 1] = (matrixB[2, 1] - matrixA[2, 3] * solutionMatrix[3, 1]) / matrixA[2, 2];
            solutionMatrix[1, 1] = (matrixB[1, 1] - matrixA[1, 3] * solutionMatrix[3, 1] - matrixA[1, 2] * solutionMatrix[2, 1]) / matrixA[1, 1];

            //
            mError = matrixA * solutionMatrix - matrixB;

            return solutionMatrix;
        }

        /// <summary>
        /// метод CalcDeterminant3 вычисляет определитель матрицы MatrixA размером 3x3
        /// </summary>
        /// <param name="matrixA"></param>
        /// <returns></returns>
        public static Double CalcDeterminant3(Matrix matrixA)
        {
            Double result = 0;

            result += matrixA[1, 1] * (matrixA[2, 2] * matrixA[3, 3] - matrixA[2, 3] * matrixA[3, 2]);
            result += matrixA[1, 2] * (matrixA[2, 3] * matrixA[3, 1] - matrixA[2, 1] * matrixA[3, 3]);
            result += matrixA[1, 3] * (matrixA[2, 1] * matrixA[3, 2] - matrixA[2, 2] * matrixA[3, 1]);

            return result;
        }

        /// <summary>
        /// метод SolveEquationSystem3Kramer решает систему лин. уравнений (3x3) методом Крамера
        /// </summary>
        /// <param name="mA">матрица A</param>
        /// <param name="mB">матрица B</param>
        /// <param name="mError">матрица абсолютных ошибок</param>
        /// <returns>решение</returns>
        public static Matrix SolveEquationSystem3Kramer(Matrix matrixA, Matrix matrixB, out Matrix mError)
        {
            if ((matrixA.ColumnCount != 3) && (matrixA.RowCount != 3))
            {
                throw new ArgumentException("MatrixA must be 3x3");
            }
            if ((matrixB.ColumnCount != 1) && (matrixB.RowCount != 3))
            {
                throw new ArgumentException("MatrixB must be 3x1");
            }

            Double delta = CalcDeterminant3(matrixA);

            Matrix matrixAX = matrixA.Clone();
            matrixAX[1, 1] = matrixB[1, 1];
            matrixAX[2, 1] = matrixB[2, 1];
            matrixAX[3, 1] = matrixB[3, 1];
            Double deltaX = CalcDeterminant3(matrixAX);

            Matrix matrixAY = matrixA.Clone();
            matrixAY[1, 2] = matrixB[1, 1];
            matrixAY[2, 2] = matrixB[2, 1];
            matrixAY[3, 2] = matrixB[3, 1];
            Double deltaY = CalcDeterminant3(matrixAY);

            Matrix matrixAZ = matrixA.Clone();
            matrixAZ[1, 3] = matrixB[1, 1];
            matrixAZ[2, 3] = matrixB[2, 1];
            matrixAZ[3, 3] = matrixB[3, 1];
            Double deltaZ = CalcDeterminant3(matrixAZ);

            Matrix solutionMatrix = new Matrix(3, 1);
            solutionMatrix[1, 1] = deltaX / delta;
            solutionMatrix[2, 1] = deltaY / delta;
            solutionMatrix[3, 1] = deltaZ / delta;

            //
            mError = matrixA * solutionMatrix - matrixB;

            return solutionMatrix;
        }

        /// <summary>
        /// метод SolveEquationSystem3 решает систему лин. уравнений (3x3) одним из методов
        /// </summary>
        /// <param name="mA">матрица A</param>
        /// <param name="mB">матрица B</param>
        /// <param name="mError">матрица абсолютных ошибок</param>
        /// <returns>решение</returns>
        public static Matrix SolveEquationSystem3(Matrix matrixA, Matrix matrixB, out Matrix mError)
        {
            return SolveEquationSystem3Kramer(matrixA, matrixB, out mError);
        }

        /// <summary>
        /// метод CalcPlaneLineCrossingPoint возвращает точку пересечения плоскости и прямой
        /// плоскость задается каноническим уравнением (Ax + By + Cz + D = 0)
        /// прямая задается 2-мя точками: linePoint1 и linePoint2
        /// </summary>
        /// <param name="approxComparer">сравниватель для операций приблизительного сравнения действительных чисел</param>
        /// <param name="plane">плоскость</param>
        /// <param name="linePoint1">1-я точка прямой</param>
        /// <param name="linePoint2">2-я точка прямой</param>
        /// <returns>точка пересечения плоскости и прямой</returns>
        public static Point3D CalcPlaneLineCrossingPoint(ApproxComparer approxComparer, PlaneClass plane, Point3D linePoint1, Point3D linePoint2)
        {
            Vector3D directionVector = new Vector3D(linePoint2.XCoord - linePoint1.XCoord,
                                                    linePoint2.YCoord - linePoint1.YCoord,
                                                    linePoint2.ZCoord - linePoint1.ZCoord);

            return CalcPlaneLineCrossingPoint(approxComparer, plane, linePoint1, directionVector);
        }

        /// <summary>
        /// метод CalcPlaneLineCrossingPoint возвращает точку пересечения плоскости и прямой
        /// плоскость задается каноническим уравнением (Ax + By + Cz + D = 0)
        /// прямая задается точкой linePoint0 и направляющим вектором directionVector
        /// </summary>
        /// <param name="approxComparer">сравниватель для операций приблизительного сравнения действительных чисел</param>
        /// <param name="plane">плоскость</param>
        /// <param name="linePoint0">точка прямой</param>
        /// <param name="directionVector">направляющий вектор прямой</param>
        /// <returns>точка пересечения плоскости и прямой</returns>
        public static Point3D CalcPlaneLineCrossingPoint(ApproxComparer approxComparer, PlaneClass plane, Point3D linePoint0, Vector3D directionVector)
        {
            Matrix matrixA = new Matrix(3, 3);
            Matrix matrixB = new Matrix(3, 1);

            matrixA[1, 1] = plane.AKoeff;
            matrixA[1, 2] = plane.BKoeff;
            matrixA[1, 3] = plane.CKoeff;
            matrixB[1, 1] = -plane.DKoeff;

            if (approxComparer.NE(directionVector.XCoord, 0) || approxComparer.NE(directionVector.YCoord, 0))
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

            if (approxComparer.NE(directionVector.XCoord, 0) || approxComparer.NE(directionVector.ZCoord, 0))
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
            Matrix solution = SolveEquationSystem3(matrixA, matrixB, out matrixError);

            return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
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

#warning !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// <summary>
        /// 
        /// </summary>
        private const Double MinLineDirectionLength = 1e-4;
    }
}
