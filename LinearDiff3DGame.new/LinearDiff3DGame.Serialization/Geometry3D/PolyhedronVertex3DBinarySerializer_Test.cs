using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Serialization.Testing;
using NUnit.Framework;

namespace LinearDiff3DGame.Serialization.Geometry3D
{
	[TestFixture]
	public class PolyhedronVertex3DBinarySerializer_Test : SerializerTest<IPolyhedronVertex3D>
	{
		public PolyhedronVertex3DBinarySerializer_Test()
			: base(data,
			       GetSerializedData(),
			       new PolyhedronVertex3DBinarySerializer(),
			       PolyhedronVertex3DEqualityTester.TestEquality)
		{
		}

		private static Byte[] GetSerializedData()
		{
			List<Byte> serializedData = new List<Byte>();
			serializedData.AddRange(BitConverter.GetBytes(data.ID));
			serializedData.AddRange(BitConverter.GetBytes(data.XCoord));
			serializedData.AddRange(BitConverter.GetBytes(data.YCoord));
			serializedData.AddRange(BitConverter.GetBytes(data.ZCoord));
			return serializedData.ToArray();
		}

		private static readonly IPolyhedronVertex3D data = new PolyhedronVertex3D(1.11, 2.22, 3.33, 12);
	}
}