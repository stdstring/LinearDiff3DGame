using System;
using System.IO;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Serialization.Common;

namespace LinearDiff3DGame.Serialization.Geometry3D
{
    internal class Point3DBinarySerializer : ISerializer<Point3D>
    {
        public void Serialize(Stream storage, Point3D serializableObject)
        {
            doubleBinarySerializer.Serialize(storage, serializableObject.XCoord);
            doubleBinarySerializer.Serialize(storage, serializableObject.YCoord);
            doubleBinarySerializer.Serialize(storage, serializableObject.ZCoord);
        }

        public Point3D Deserialize(Stream storage)
        {
            Double xCoord = doubleBinarySerializer.Deserialize(storage);
            Double yCoord = doubleBinarySerializer.Deserialize(storage);
            Double zCoord = doubleBinarySerializer.Deserialize(storage);
            return new Point3D(xCoord, yCoord, zCoord);
        }

        private readonly DoubleBinarySerializer doubleBinarySerializer = new DoubleBinarySerializer();
    }
}