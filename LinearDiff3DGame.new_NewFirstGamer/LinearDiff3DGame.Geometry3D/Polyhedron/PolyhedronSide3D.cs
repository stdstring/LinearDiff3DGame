using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.Geometry3D.Polyhedron
{
    public class PolyhedronSide3D
    {
        public PolyhedronSide3D(Int32 sideID, Vector3D sideNormal)
        {
            ID = sideID;
            vertexList = new List<PolyhedronVertex3D>();
            this.sideNormal = sideNormal;
        }

        public PolyhedronSide3D(IList<PolyhedronVertex3D> vertexList, Int32 sideID, Vector3D sideNormal)
        {
            ID = sideID;

            this.vertexList = new List<PolyhedronVertex3D>();
            for(Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
                this.vertexList.Add(vertexList[vertexIndex]);

            this.sideNormal = sideNormal;
        }

        public readonly Int32 ID;

        public Vector3D SideNormal { get { return sideNormal; } }

        public IList<PolyhedronVertex3D> VertexList { get { return vertexList; } }

        [Obsolete]
        public Boolean HasVertex(PolyhedronVertex3D vertex)
        {
            return (vertexList.IndexOf(vertex) != -1);
        }

        private readonly List<PolyhedronVertex3D> vertexList;
        private readonly Vector3D sideNormal;
    }
}