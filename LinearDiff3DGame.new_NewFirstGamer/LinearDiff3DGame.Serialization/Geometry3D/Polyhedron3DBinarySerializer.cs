using System;
using System.Collections.Generic;
using System.IO;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Serialization.Common;

namespace LinearDiff3DGame.Serialization.Geometry3D
{
    public class Polyhedron3DBinarySerializer : ISerializer<Polyhedron3D>
    {
        public void Serialize(Stream storage, Polyhedron3D serializableObject)
        {
            Pair<Int32, IEnumerable<PolyhedronVertex3D>> vertexes =
                new Pair<Int32, IEnumerable<PolyhedronVertex3D>>(serializableObject.VertexList.Count, serializableObject.VertexList);
            vertexesSerializer.Serialize(storage, vertexes);
            IDictionary<Int32, PolyhedronVertex3D> vertexDict = new Dictionary<Int32, PolyhedronVertex3D>();
            EnumerableBinarySerializer<PolyhedronSide3D> sidesSerializer =
                new EnumerableBinarySerializer<PolyhedronSide3D>(new PolyhedronSide3DBinarySerializer(vertexDict));
            Pair<Int32, IEnumerable<PolyhedronSide3D>> sides =
                new Pair<Int32, IEnumerable<PolyhedronSide3D>>(serializableObject.SideList.Count, serializableObject.SideList);
            sidesSerializer.Serialize(storage, sides);
        }

        public Polyhedron3D Deserialize(Stream storage)
        {
            Pair<Int32, IEnumerable<PolyhedronVertex3D>> vertexes = vertexesSerializer.Deserialize(storage);
            IDictionary<Int32, PolyhedronVertex3D> vertexDict = new Dictionary<Int32, PolyhedronVertex3D>(vertexes.Item1);
            foreach(PolyhedronVertex3D vertex in vertexes.Item2)
                vertexDict.Add(vertex.ID, vertex);
            EnumerableBinarySerializer<PolyhedronSide3D> sidesSerializer =
               new EnumerableBinarySerializer<PolyhedronSide3D>(new PolyhedronSide3DBinarySerializer(vertexDict));
            Pair<Int32, IEnumerable<PolyhedronSide3D>> sides = sidesSerializer.Deserialize(storage);
            return new Polyhedron3D(sides.Item2, vertexes.Item2);
        }

        private readonly EnumerableBinarySerializer<PolyhedronVertex3D> vertexesSerializer =
            new EnumerableBinarySerializer<PolyhedronVertex3D>(new PolyhedronVertex3DBinarySerializer());
    }
}