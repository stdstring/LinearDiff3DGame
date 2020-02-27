using System;
using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.AdvMath.Common;

namespace LinearDiff3DGame.Geometry3D.Common
{
    public static class CoordTransformation3D
    {
        // матрица поворота вокруг оси OX на угол angle
        public static Matrix RxMatrix(Double angle)
        {
            AdvTrigonometry triginometry = new AdvTrigonometry();
            Double sinValue = triginometry.Sin(angle);
            Double cosValue = triginometry.Cos(angle);
            Matrix matrix = Create4x4Matrix();
            matrix[1, 1] = 1;
            matrix[2, 2] = cosValue;
            matrix[2, 3] = -sinValue;
            matrix[3, 2] = sinValue;
            matrix[3, 3] = cosValue;
            return matrix;
        }

        // матрица поворота вокруг оси OY на угол angle
        public static Matrix RyMatrix(Double angle)
        {
            AdvTrigonometry triginometry = new AdvTrigonometry();
            Double sinValue = triginometry.Sin(angle);
            Double cosValue = triginometry.Cos(angle);
            Matrix matrix = Create4x4Matrix();
            matrix[1, 1] = cosValue;
            matrix[1, 3] = sinValue;
            matrix[2, 2] = 1;
            matrix[3, 1] = -sinValue;
            matrix[3, 3] = cosValue;
            return matrix;
        }

        // матрица поворота вокруг оси OZ на угол angle
        public static Matrix RzMatrix(Double angle)
        {
            AdvTrigonometry triginometry = new AdvTrigonometry();
            Double sinValue = triginometry.Sin(angle);
            Double cosValue = triginometry.Cos(angle);
            Matrix matrix = Create4x4Matrix();
            matrix[1, 1] = cosValue;
            matrix[1, 2] = -sinValue;
            matrix[2, 1] = sinValue;
            matrix[2, 2] = cosValue;
            matrix[3, 3] = 1;
            return matrix;
        }

        // матрица масштабирования
        public static Matrix DMatrix(Double dx, Double dy, Double dz)
        {
            Matrix matrix = Create4x4Matrix();
            matrix[1, 1] = dx;
            matrix[2, 2] = dy;
            matrix[3, 3] = dz;
            return matrix;
        }

        // матрица отражения относительно плоскости xOy
        public static Matrix Mx0yMatrix()
        {
            Matrix matrix = Create4x4Matrix();
            matrix[1, 1] = 1;
            matrix[2, 2] = 1;
            matrix[3, 3] = -1;
            return matrix;
        }

        // матрица отражения относительно плоскости zOx
        public static Matrix Mz0xMatrix()
        {
            Matrix matrix = Create4x4Matrix();
            matrix[1, 1] = 1;
            matrix[2, 2] = -1;
            matrix[3, 3] = 1;
            return matrix;
        }

        // матрица отражения относительно плоскости yOz
        public static Matrix My0zMatrix()
        {
            Matrix matrix = Create4x4Matrix();
            matrix[1, 1] = -1;
            matrix[2, 2] = 1;
            matrix[3, 3] = 1;
            return matrix;
        }

        // матрица переноса
        public static Matrix TMatrix(Vector3D shiftVector)
        {
            Matrix matrix = Create4x4Matrix();
            matrix[1, 1] = 1;
            matrix[2, 2] = 1;
            matrix[3, 3] = 1;
            matrix[1, 4] = shiftVector.XCoord;
            matrix[2, 4] = shiftVector.YCoord;
            matrix[3, 4] = shiftVector.ZCoord;
            return matrix;
        }

        private static Matrix Create4x4Matrix()
        {
            Matrix matrixBillets = new Matrix(4, 4);
            matrixBillets[4, 4] = 1;
            return matrixBillets;
        }
    }
}
