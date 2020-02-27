using System;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.Geometry3D.Common
{
	[Immutable]
	public struct Vector3D
	{
		public Vector3D(Double coordX, Double coordY, Double coordZ) : this()
		{
			X = coordX;
			Y = coordY;
			Z = coordZ;
		}

		public Double X { get; private set; }

		public double Y { get; private set; }

		public double Z { get; private set; }

		public Double Length
		{
			get { return Math.Sqrt(X*X + Y*Y + Z*Z); }
		}

		public override string ToString()
		{
			return String.Format("({0} ; {1} ; {2})", X, Y, Z);
		}

		public static Vector3D VectorAddition(Vector3D a, Vector3D b)
		{
			return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public static Vector3D VectorSubtraction(Vector3D a, Vector3D b)
		{
			return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static Vector3D VectorMultiplication(Vector3D a, Double number)
		{
			return new Vector3D(number*a.X, number*a.Y, number*a.Z);
		}

		public static Vector3D ZeroVector3D
		{
			get { return new Vector3D(0, 0, 0); }
		}

		public static Vector3D operator +(Vector3D a, Vector3D b)
		{
			return VectorAddition(a, b);
		}

		public static Vector3D operator -(Vector3D a, Vector3D b)
		{
			return VectorSubtraction(a, b);
		}

		public static Vector3D operator *(Double number, Vector3D a)
		{
			return VectorMultiplication(a, number);
		}

		public static Vector3D operator *(Vector3D a, Double number)
		{
			return VectorMultiplication(a, number);
		}

		public static Double operator *(Vector3D a, Vector3D b)
		{
			return Vector3DUtils.ScalarProduct(a, b);
		}
	}
}