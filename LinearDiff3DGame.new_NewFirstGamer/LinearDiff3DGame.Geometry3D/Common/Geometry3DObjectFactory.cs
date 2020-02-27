using System;
using LinearDiff3DGame.AdvMath;

namespace LinearDiff3DGame.Geometry3D.Common
{
    public static class Geometry3DObjectFactory
    {
        public static Point3D CreatePoint(Vector3D vector)
        {
            return new Point3D(vector.XCoord, vector.YCoord, vector.ZCoord);
        }

        public static Point3D CreatePoint(Matrix matrix)
        {
            if (matrix.RowCount != 3 && matrix.ColumnCount != 1) throw new ArgumentException("matrix");
            return new Point3D(matrix[1, 1], matrix[2, 1], matrix[3, 1]);
        }

        public static Vector3D CreateVector(Point3D point)
        {
            return new Vector3D(point.XCoord, point.YCoord, point.ZCoord);
        }

        public static Vector3D CreateVector(Matrix matrix)
        {
            if (matrix.RowCount != 3 && matrix.ColumnCount != 1) throw new ArgumentException("matrix");
            return new Vector3D(matrix[1, 1], matrix[2, 1], matrix[3, 1]);
        }

        public static Matrix CreateMatrix(Point3D point)
        {
            Matrix matrix = new Matrix(3, 1);
            matrix[1, 1] = point.XCoord;
            matrix[2, 1] = point.YCoord;
            matrix[3, 1] = point.ZCoord;
            return matrix;
        }

        public static Matrix CreateMatrix(Vector3D vector)
        {
            Matrix matrix = new Matrix(3, 1);
            matrix[1, 1] = vector.XCoord;
            matrix[2, 1] = vector.YCoord;
            matrix[3, 1] = vector.ZCoord;
            return matrix;
        }
    }
}