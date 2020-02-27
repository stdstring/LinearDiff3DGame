using System;
using System.Collections.Generic;
using System.IO;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Serialization.Common;

namespace LinearDiff3DGame.Serialization.Geometry3D
{
	public class Polyhedron3DBinarySerializer : ISerializer<IPolyhedron3D>
	{
		public void Serialize(Stream storage, IPolyhedron3D serializableObject)
		{
			Pair<Int32, IEnumerable<IPolyhedronVertex3D>> vertexes =
				new Pair<Int32, IEnumerable<IPolyhedronVertex3D>>(serializableObject.VertexList.Count, serializableObject.VertexList);
			vertexesSerializer.Serialize(storage, vertexes);
			IDictionary<Int32, IPolyhedronVertex3D> vertexDict = new Dictionary<Int32, IPolyhedronVertex3D>();
			EnumerableBinarySerializer<IPolyhedronSide3D> sidesSerializer =
				new EnumerableBinarySerializer<IPolyhedronSide3D>(new PolyhedronSide3DBinarySerializer(vertexDict));
			Pair<Int32, IEnumerable<IPolyhedronSide3D>> sides =
				new Pair<Int32, IEnumerable<IPolyhedronSide3D>>(serializableObject.SideList.Count, serializableObject.SideList);
			sidesSerializer.Serialize(storage, sides);
		}

		public IPolyhedron3D Deserialize(Stream storage)
		{
			Pair<Int32, IEnumerable<IPolyhedronVertex3D>> vertexes = vertexesSerializer.Deserialize(storage);
			IDictionary<Int32, IPolyhedronVertex3D> vertexDict = new Dictionary<Int32, IPolyhedronVertex3D>(vertexes.Item1);
			foreach (IPolyhedronVertex3D vertex in vertexes.Item2)
				vertexDict.Add(vertex.ID, vertex);
			EnumerableBinarySerializer<IPolyhedronSide3D> sidesSerializer =
				new EnumerableBinarySerializer<IPolyhedronSide3D>(new PolyhedronSide3DBinarySerializer(vertexDict));
			Pair<Int32, IEnumerable<IPolyhedronSide3D>> sides = sidesSerializer.Deserialize(storage);
			return new Polyhedron3D(sides.Item2, vertexes.Item2);
		}

		private readonly EnumerableBinarySerializer<IPolyhedronVertex3D> vertexesSerializer =
			new EnumerableBinarySerializer<IPolyhedronVertex3D>(new PolyhedronVertex3DBinarySerializer());
	}
}