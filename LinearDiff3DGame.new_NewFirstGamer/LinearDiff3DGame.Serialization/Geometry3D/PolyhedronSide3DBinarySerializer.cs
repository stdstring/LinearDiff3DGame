using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Serialization.Common;

namespace LinearDiff3DGame.Serialization.Geometry3D
{
    public class PolyhedronSide3DBinarySerializer : ISerializer<PolyhedronSide3D>
    {
        public PolyhedronSide3DBinarySerializer(IDictionary<Int32, PolyhedronVertex3D> vertexDictionary)
        {
            this.vertexDictionary = vertexDictionary;
        }

        public void Serialize(Stream storage, PolyhedronSide3D serializableObject)
        {
            int32BinarySerializer.Serialize(storage, serializableObject.ID);
            normalSerializer.Serialize(storage, serializableObject.SideNormal);
            Int32 vertexCount = serializableObject.VertexList.Count;
            IEnumerable<Int32> vertexIDList = serializableObject.VertexList.Select(vertex => vertex.ID);
            Pair<Int32, IEnumerable<Int32>> data = new Pair<Int32, IEnumerable<Int32>>(vertexCount, vertexIDList);
            sideVertexesSerializer.Serialize(storage, data);
        }

        public PolyhedronSide3D Deserialize(Stream storage)
        {
            Int32 id = int32BinarySerializer.Deserialize(storage);
            Vector3D normal = normalSerializer.Deserialize(storage);
            Pair<Int32, IEnumerable<Int32>> sideVertexesDescription = sideVertexesSerializer.Deserialize(storage);
            IList<PolyhedronVertex3D> sideVertexes = new List<PolyhedronVertex3D>(sideVertexesDescription.Item1);
            foreach(Int32 vertexID in sideVertexesDescription.Item2)
                sideVertexes.Add(vertexDictionary[vertexID]);
            return new PolyhedronSide3D(sideVertexes, id, normal);
        }

        private readonly IDictionary<Int32, PolyhedronVertex3D> vertexDictionary;

        private readonly Int32BinarySerializer int32BinarySerializer = new Int32BinarySerializer();

        private readonly Vector3DBinarySerializer normalSerializer =
            new Vector3DBinarySerializer();

        private readonly EnumerableBinarySerializer<Int32> sideVertexesSerializer =
            new EnumerableBinarySerializer<Int32>(new Int32BinarySerializer());
    }
}