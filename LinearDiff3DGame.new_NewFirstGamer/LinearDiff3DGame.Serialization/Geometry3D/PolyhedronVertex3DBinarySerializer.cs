using System;
using System.IO;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Serialization.Common;

namespace LinearDiff3DGame.Serialization.Geometry3D
{
    public class PolyhedronVertex3DBinarySerializer : ISerializer<PolyhedronVertex3D>
    {
        public void Serialize(Stream storage, PolyhedronVertex3D serializableObject)
        {
            int32BinarySerializer.Serialize(storage, serializableObject.ID);
            doubleBinarySerializer.Serialize(storage, serializableObject.XCoord);
            doubleBinarySerializer.Serialize(storage, serializableObject.YCoord);
            doubleBinarySerializer.Serialize(storage, serializableObject.ZCoord);
        }

        public PolyhedronVertex3D Deserialize(Stream storage)
        {
            Int32 id = int32BinarySerializer.Deserialize(storage);
            Double xCoord = doubleBinarySerializer.Deserialize(storage);
            Double yCoord = doubleBinarySerializer.Deserialize(storage);
            Double zCoord = doubleBinarySerializer.Deserialize(storage);
            return new PolyhedronVertex3D(xCoord, yCoord, zCoord, id);
        }

        private readonly Int32BinarySerializer int32BinarySerializer = new Int32BinarySerializer();
        private readonly DoubleBinarySerializer doubleBinarySerializer = new DoubleBinarySerializer();
    }
}