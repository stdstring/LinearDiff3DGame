using LinearDiff3DGame.AdvMath.Matrix;
using LinearDiff3DGame.AdvMath.MatrixUtils;

namespace LinearDiff3DGame.Geometry3D.Common
{
    public static class Point3DExtensions
    {
        public static Vector3D ToVector3D(this Point3D point)
        {
            return new Vector3D(point.X, point.Y, point.Z);
        }

        public static Matrix ToMatrixColumn(this Point3D point)
        {
            return new MatrixFactory().CreateFromRawData(3, 1, point.X, point.Y, point.Z);
        }

        public static Point3D ToPoint3D(this Matrix matrixColumn)
        {
            if (matrixColumn.RowCount != 3 || matrixColumn.ColumnCount != 1)
                throw new IncorrectMatrixSizeException("matrixColumn must be 3x1");
            return new Point3D(matrixColumn[1, 1], matrixColumn[2, 1], matrixColumn[3, 1]);
        }
    }
}