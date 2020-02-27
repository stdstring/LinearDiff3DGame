using System;
using System.IO;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Serialization.Common;

namespace LinearDiff3DGame.Serialization.Geometry3D
{
    internal class Vector3DBinarySerializer : ISerializer<Vector3D>
    {
        public void Serialize(Stream storage, Vector3D serializableObject)
        {
            doubleBinarySerializer.Serialize(storage, serializableObject.XCoord);
            doubleBinarySerializer.Serialize(storage, serializableObject.YCoord);
            doubleBinarySerializer.Serialize(storage, serializableObject.ZCoord);
        }

        public Vector3D Deserialize(Stream storage)
        {
            Double xCoord = doubleBinarySerializer.Deserialize(storage);
            Double yCoord = doubleBinarySerializer.Deserialize(storage);
            Double zCoord = doubleBinarySerializer.Deserialize(storage);
            return new Vector3D(xCoord, yCoord, zCoord);
        }

        private readonly DoubleBinarySerializer doubleBinarySerializer = new DoubleBinarySerializer();
    }
}