using System;
using LinearDiff3DGame.AdvMath;

namespace LinearDiff3DGame.Geometry3D.Common
{
    public static class ScalingTransformation3D
    {
        public static Matrix GetTransformationMatrix(Vector3D direction, Double scalingRatio)
        {
            Double m = scalingRatio - 1;
            Matrix transformation = new Matrix(3, 3);
            transformation[1, 1] = m*direction.XCoord*direction.XCoord + 1;
            transformation[1, 2] = m*direction.XCoord*direction.YCoord;
            transformation[1, 3] = m*direction.XCoord*direction.ZCoord;
            transformation[2, 1] = m*direction.YCoord*direction.XCoord;
            transformation[2, 2] = m*direction.YCoord*direction.YCoord + 1;
            transformation[2, 3] = m*direction.YCoord*direction.ZCoord;
            transformation[3, 1] = m*direction.ZCoord*direction.XCoord;
            transformation[3, 2] = m*direction.ZCoord*direction.YCoord;
            transformation[3, 3] = m*direction.ZCoord*direction.ZCoord + 1;
            return transformation;
        }

        //// формирование матрицы маштабирования вдоль направления direction (dx, dy, dz) :
        //// мы знаем матрицу маштабирования вдоль осей OX, OY, OZ
        //// для формирования матрицы маштабирования вдоль произвольной оси будем поворачивать эту ось к OZ
        //// 1) поворачиваем вокруг оси OZ на угол a1 (direction -> YOZ)
        //// 2) поворачиваем вокруг оси OX на угол a2 (direction -> OZ)
        //// 3) масштабирование вдоль оси OZ
        //// 4) поворачиваем вокруг оси OX на угол -a2
        //// 5) поворачиваем вокруг оси OZ на угол -a1
        //// a1 = arcsin(dx/Sqrt(dx*dx+dy*dy)) в первой четверти
        //// a2 = arcsin(dy/Sqrt(dy*dy+dz*dz)) в первой четверти
        //// результат = (1)*(2)*(3)*(4)*(5)
        //public static Matrix GetTransformationMatrix(Vector3D direction, Double scalingRatio)
        //{
        //    Double a1 = 0; // ...
        //    Double a2 = 0; // ...
        //    throw new NotImplementedException();
        //}

        //internal static Matrix GetTransformationXYMatrix(Double a1, Double a2, Double scalingRatio)
        //{
        //    Matrix transformation1 = CoordTransformation3D.RxMatrix(a1);
        //    Matrix transformation2 = CoordTransformation3D.RyMatrix(a2);
        //    Matrix transformation3 = CoordTransformation3D.DMatrix(1, 1, scalingRatio);
        //    Matrix transformation4 = CoordTransformation3D.RyMatrix(-a2);
        //    Matrix transformation5 = CoordTransformation3D.RxMatrix(-a1);
        //    return transformation1 * transformation2 * transformation3 * transformation4 * transformation5;
        //}

        //internal static Matrix GetTransformationZXMatrix(Double a1, Double a2, Double scalingRatio)
        //{
        //    Matrix transformation1 = CoordTransformation3D.RzMatrix(a1);
        //    Matrix transformation2 = CoordTransformation3D.RxMatrix(a2);
        //    Matrix transformation3 = CoordTransformation3D.DMatrix(1, 1, scalingRatio);
        //    Matrix transformation4 = CoordTransformation3D.RxMatrix(-a2);
        //    Matrix transformation5 = CoordTransformation3D.RzMatrix(-a1);
        //    return transformation1 * transformation2 * transformation3 * transformation4 * transformation5;
        //}

        //internal static Matrix GetTransformationZYMatrix(Double a1, Double a2, Double scalingRatio)
        //{
        //    Matrix transformation1 = CoordTransformation3D.RzMatrix(a1);
        //    Matrix transformation2 = CoordTransformation3D.RyMatrix(a2);
        //    Matrix transformation3 = CoordTransformation3D.DMatrix(1, 1, scalingRatio);
        //    Matrix transformation4 = CoordTransformation3D.RyMatrix(-a2);
        //    Matrix transformation5 = CoordTransformation3D.RzMatrix(-a1);
        //    return transformation1 * transformation2 * transformation3 * transformation4 * transformation5;
        //}
    }
}