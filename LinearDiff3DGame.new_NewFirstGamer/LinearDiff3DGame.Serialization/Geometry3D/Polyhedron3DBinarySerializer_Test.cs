using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Serialization.Testing;
using NUnit.Framework;

namespace LinearDiff3DGame.Serialization.Geometry3D
{
    [TestFixture]
    public class Polyhedron3DBinarySerializer_Test : SerializerTest<Polyhedron3D>
    {
        public Polyhedron3DBinarySerializer_Test()
            : base(TestDataGenerator.GetPolyhedron(scaleCoeff),
                   GetSerializedData(),
                   new Polyhedron3DBinarySerializer(),
                   Polyhedron3DEqualityTester.TestEquality)
        {
        }

        private static Byte[] GetSerializedData()
        {
            Polyhedron3D data = TestDataGenerator.GetPolyhedron(scaleCoeff);
            List<Byte> serializedData = new List<Byte>();
            serializedData.AddRange(BitConverter.GetBytes(data.VertexList.Count));
            foreach(PolyhedronVertex3D vertex in data.VertexList)
            {
                serializedData.AddRange(BitConverter.GetBytes(vertex.ID));
                serializedData.AddRange(BitConverter.GetBytes(vertex.XCoord));
                serializedData.AddRange(BitConverter.GetBytes(vertex.YCoord));
                serializedData.AddRange(BitConverter.GetBytes(vertex.ZCoord));
            }
            serializedData.AddRange(BitConverter.GetBytes(data.SideList.Count));
            foreach(PolyhedronSide3D side in data.SideList)
            {
                serializedData.AddRange(BitConverter.GetBytes(side.ID));
                serializedData.AddRange(BitConverter.GetBytes(side.SideNormal.XCoord));
                serializedData.AddRange(BitConverter.GetBytes(side.SideNormal.YCoord));
                serializedData.AddRange(BitConverter.GetBytes(side.SideNormal.ZCoord));
                serializedData.AddRange(BitConverter.GetBytes(side.VertexList.Count));
                foreach(PolyhedronVertex3D vertex in side.VertexList)
                    serializedData.AddRange(BitConverter.GetBytes(vertex.ID));
            }
            return serializedData.ToArray();
        }

        private const Double scaleCoeff = 1.34;
    }
}