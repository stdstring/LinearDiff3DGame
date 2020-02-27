using System;
using LinearDiff3DGame.AdvMath.Matrix;

namespace LinearDiff3DGame.Geometry3D.Common
{
    public static class ScalingTransformation3D
    {
        public static Matrix GetTransformationMatrix(Vector3D direction, Double scalingRatio)
        {
            Double m = scalingRatio - 1;
            Matrix transformation = new Matrix(3, 3);
            transformation[1, 1] = m*direction.X*direction.X + 1;
            transformation[1, 2] = m*direction.X*direction.Y;
            transformation[1, 3] = m*direction.X*direction.Z;
            transformation[2, 1] = m*direction.Y*direction.X;
            transformation[2, 2] = m*direction.Y*direction.Y + 1;
            transformation[2, 3] = m*direction.Y*direction.Z;
            transformation[3, 1] = m*direction.Z*direction.X;
            transformation[3, 2] = m*direction.Z*direction.Y;
            transformation[3, 3] = m*direction.Z*direction.Z + 1;
            return transformation;
        }
    }
}