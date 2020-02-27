using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LinearDiff3DGame.Geometry3D.Polyhedron
{
    internal class VertexSidesDictionary
    {
        public VertexSidesDictionary()
        {
            m_VertexSidesDictionary = new Dictionary<PolyhedronVertex3D, List<PolyhedronSide3D>>();
        }

        public static VertexSidesDictionary Create(Polyhedron3D polyhedron)
        {
            VertexSidesDictionary dict = new VertexSidesDictionary();

            foreach (PolyhedronSide3D side in polyhedron.SideList)
            {
                foreach (PolyhedronVertex3D vertex in side.VertexList)
                {
                    dict.AddSide4Vertex(vertex, side);
                }
            }

            return dict;
        }

        public void AddSide4Vertex(PolyhedronVertex3D vertex, PolyhedronSide3D side)
        {
            List<PolyhedronSide3D> sideList;
            if (!m_VertexSidesDictionary.TryGetValue(vertex, out sideList))
            {
                m_VertexSidesDictionary[vertex] = sideList = new List<PolyhedronSide3D>();
            }
            sideList.Add(side);
        }

        public void Clear()
        {
            m_VertexSidesDictionary.Clear();
        }

        public IList<PolyhedronSide3D> GetSideList4Vertex(PolyhedronVertex3D vertex)
        {
            List<PolyhedronSide3D> sideList;
            if (!m_VertexSidesDictionary.TryGetValue(vertex, out sideList))
            {
                sideList = new List<PolyhedronSide3D>();
            }
            return new ReadOnlyCollection<PolyhedronSide3D>(sideList);
        }

        public void RemoveSide4Vertex(PolyhedronVertex3D vertex, PolyhedronSide3D side)
        {
            List<PolyhedronSide3D> sideList;
            if (m_VertexSidesDictionary.TryGetValue(vertex, out sideList))
            {
                sideList.Remove(side);
            }
        }

        private readonly Dictionary<PolyhedronVertex3D, List<PolyhedronSide3D>> m_VertexSidesDictionary;
    }
}