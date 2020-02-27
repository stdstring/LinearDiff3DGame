using LinearDiff3DGame.AdvMath.Matrix;
using LinearDiff3DGame.AdvMath.MatrixUtils;

namespace LinearDiff3DGame.Geometry3D.Common
{
    public static class Vector3DExtensions
    {
        public static Point3D ToPoint3D(this Vector3D vector)
        {
            return new Point3D(vector.X, vector.Y, vector.Z);
        }

        public static Matrix ToMatrixColumn(this Vector3D vector)
        {
            return new MatrixFactory().CreateFromRawData(3, 1, vector.X, vector.Y, vector.Z);
        }

        public static Vector3D ToVector3D(this Matrix matrixColumn)
        {
            if (matrixColumn.RowCount != 3 || matrixColumn.ColumnCount != 1)
                throw new IncorrectMatrixSizeException("matrixColumn must be 3x1");
            return new Vector3D(matrixColumn[1, 1], matrixColumn[2, 1], matrixColumn[3, 1]);
        }
    }
}