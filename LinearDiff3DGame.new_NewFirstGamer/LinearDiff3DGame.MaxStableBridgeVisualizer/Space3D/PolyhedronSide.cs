using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.MaxStableBridgeVisualizer.Space3D
{
    /// <summary>
    /// представление грани многогранника
    /// </summary>
    internal class PolyhedronSide
    {
        internal PolyhedronSide(PolyhedronSide3D side)
        {
            // init normal
            m_Normal = new Object3D(side.SideNormal.XCoord,
                side.SideNormal.YCoord,
                side.SideNormal.ZCoord);
            // init vertex list
            IList<Object3D> vertexList = new List<Object3D>(side.VertexList.Count);
            for(Int32 vertexIndex=0; vertexIndex<side.VertexList.Count; ++vertexIndex)
            {
                PolyhedronVertex3D currentVertex = side.VertexList[vertexIndex];
                vertexList.Add(new Object3D(currentVertex.XCoord,
                    currentVertex.YCoord,
                    currentVertex.ZCoord));
            }
            m_VertexList = new ReadOnlyCollection<Object3D>(vertexList);
        }

        public Object3D Normal
        {
            get { return m_Normal; }
        }

        public IList<Object3D> VertexList
        {
            get { return m_VertexList; }
        }

        private readonly Object3D m_Normal;
        private readonly ReadOnlyCollection<Object3D> m_VertexList;
    }
}
