using System;

namespace LinearDiff3DGame.Geometry3D.Common
{
    public static class Vector3DUtils
    {
        public static Vector3D GetParallelComponent(Vector3D sourceVector, Vector3D directingVector)
        {
            Double scalarProductValue = sourceVector * directingVector;

            Double parallelCompX = scalarProductValue * directingVector.XCoord / directingVector.Length;
            Double parallelCompY = scalarProductValue * directingVector.YCoord / directingVector.Length;
            Double parallelCompZ = scalarProductValue * directingVector.ZCoord / directingVector.Length;

            return new Vector3D(parallelCompX, parallelCompY, parallelCompZ);
        }

        public static Vector3D GetPerpendicularComponent(Vector3D sourceVector, Vector3D directingVector)
        {
            Double scalarProductValue = sourceVector * directingVector;

            Double perpendicularCompX = sourceVector.XCoord -
                                        scalarProductValue * directingVector.XCoord / directingVector.Length;
            Double perpendicularCompY = sourceVector.YCoord -
                                        scalarProductValue * directingVector.YCoord / directingVector.Length;
            Double perpendicularCompZ = sourceVector.ZCoord -
                                        scalarProductValue * directingVector.ZCoord / directingVector.Length;

            return new Vector3D(perpendicularCompX, perpendicularCompY, perpendicularCompZ);
        }

        public static Double CosAngleBetweenVectors(Vector3D a, Vector3D b)
        {
            Double cosValue = (a * b) / (a.Length * b.Length);
            // из-за ошибок округления значение косинуса угла может стать > 1 (или < -1)
            if(cosValue > 1) return 1;
            if(cosValue < -1) return -1;
            return cosValue;
        }

        public static Double AngleBetweenVectors(Vector3D a, Vector3D b)
        {
            return Math.Acos(CosAngleBetweenVectors(a, b));
        }

        public static Vector3D NormalizeVector(Vector3D sourceVector)
        {
            if (sourceVector.XCoord == 0 && sourceVector.YCoord == 0 && sourceVector.ZCoord == 0)
                throw new ArgumentException("Can't normailze zero vector.", "sourceVector");

            Double length = sourceVector.Length;
            return new Vector3D(sourceVector.XCoord / length,
                                sourceVector.YCoord / length,
                                sourceVector.ZCoord / length);
        }
    }
}