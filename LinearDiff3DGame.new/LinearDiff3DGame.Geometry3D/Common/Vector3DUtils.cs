using System;

namespace LinearDiff3DGame.Geometry3D.Common
{
    public static class Vector3DUtils
    {
        public static Vector3D GetParallelComponent(Vector3D sourceVector, Vector3D directingVector)
        {
            Vector3D direction = NormalizeVector(directingVector);
            Double scalarProductValue = sourceVector * direction;
            Double parallelCompX = scalarProductValue * direction.X;
            Double parallelCompY = scalarProductValue * direction.Y;
            Double parallelCompZ = scalarProductValue * direction.Z;
            return new Vector3D(parallelCompX, parallelCompY, parallelCompZ);
        }

        public static Vector3D GetPerpendicularComponent(Vector3D sourceVector, Vector3D directingVector)
        {
            Vector3D direction = NormalizeVector(directingVector);
            Double scalarProductValue = sourceVector * direction;
            Double perpendicularCompX = sourceVector.X - scalarProductValue * direction.X;
            Double perpendicularCompY = sourceVector.Y - scalarProductValue * direction.Y;
            Double perpendicularCompZ = sourceVector.Z - scalarProductValue * direction.Z;
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
            if(sourceVector.X == 0 && sourceVector.Y == 0 && sourceVector.Z == 0)
                throw new ArgumentException("Can't normailze zero vector.", "sourceVector");

            Double length = sourceVector.Length;
            return new Vector3D(sourceVector.X / length,
                                sourceVector.Y / length,
                                sourceVector.Z / length);
        }

        public static Double ScalarProduct(Vector3D a, Vector3D b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static Vector3D VectorProduct(Vector3D a, Vector3D b)
        {
            Double coordX = a.Y * b.Z - a.Z * b.Y;
            Double coordY = a.Z * b.X - a.X * b.Z;
            Double coordZ = a.X * b.Y - a.Y * b.X;

            return new Vector3D(coordX, coordY, coordZ);
        }

        public static Double MixedProduct(Vector3D a, Vector3D b, Vector3D c)
        {
            return ScalarProduct(a, VectorProduct(b, c));
        }
    }
}