using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Serialization.Testing;
using NUnit.Framework;

namespace LinearDiff3DGame.Serialization.Geometry3D
{
    [TestFixture]
    public class PolyhedronSide3DBinarySerializer_Test : SerializerTest<PolyhedronSide3D>
    {
        public PolyhedronSide3DBinarySerializer_Test()
            : base(GetData(),
                   GetSerializedData(),
                   new PolyhedronSide3DBinarySerializer(GetVertexDictionary()),
                   PolyhedronSide3DEqualityTester.TestEquality)
        {
        }

        private static PolyhedronSide3D GetData()
        {
            IList<PolyhedronVertex3D> vertexList = new List<PolyhedronVertex3D>
                                                       {
                                                           new PolyhedronVertex3D(1.1, 0, 0, 10),
                                                           new PolyhedronVertex3D(2.1, 1.2, 0, 11),
                                                           new PolyhedronVertex3D(3.2, 0, 2.3, 15)
                                                       };
            Vector3D normal = new Vector3D(1.1, 2.3, 0.002);
            return new PolyhedronSide3D(vertexList, 123, normal);
        }

        private static IDictionary<Int32, PolyhedronVertex3D> GetVertexDictionary()
        {
            return new Dictionary<Int32, PolyhedronVertex3D>
                       {
                           {1, new PolyhedronVertex3D(-1, -2, 0, 1)},
                           {10, new PolyhedronVertex3D(1.1, 0, 0, 10)},
                           {11, new PolyhedronVertex3D(2.1, 1.2, 0, 11)},
                           {13, new PolyhedronVertex3D(-11, 1, 12, 13)},
                           {15, new PolyhedronVertex3D(3.2, 0, 2.3, 15)}
                       };
        }

        private static Byte[] GetSerializedData()
        {
            PolyhedronSide3D data = GetData();
            List<Byte> serializedData = new List<Byte>();
            serializedData.AddRange(BitConverter.GetBytes(data.ID));
            serializedData.AddRange(BitConverter.GetBytes(data.SideNormal.XCoord));
            serializedData.AddRange(BitConverter.GetBytes(data.SideNormal.YCoord));
            serializedData.AddRange(BitConverter.GetBytes(data.SideNormal.ZCoord));
            serializedData.AddRange(BitConverter.GetBytes(data.VertexList.Count));
            foreach(PolyhedronVertex3D vertex in data.VertexList)
                serializedData.AddRange(BitConverter.GetBytes(vertex.ID));
            return serializedData.ToArray();
        }
    }
}