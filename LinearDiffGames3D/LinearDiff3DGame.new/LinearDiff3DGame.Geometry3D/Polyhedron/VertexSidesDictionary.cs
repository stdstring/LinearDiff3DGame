using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LinearDiff3DGame.Geometry3D.Polyhedron
{
    internal class VertexSidesDictionary
    {
        public VertexSidesDictionary()
        {
            vertexSidesDictionary = new Dictionary<IPolyhedronVertex3D, List<IPolyhedronSide3D>>();
        }

        public static VertexSidesDictionary Create(IPolyhedron3D polyhedron)
        {
            VertexSidesDictionary dict = new VertexSidesDictionary();

            foreach (IPolyhedronSide3D side in polyhedron.SideList)
            {
                foreach (IPolyhedronVertex3D vertex in side.VertexList)
                {
                    dict.AddSide4Vertex(vertex, side);
                }
            }

            return dict;
        }

        public void AddSide4Vertex(IPolyhedronVertex3D vertex, IPolyhedronSide3D side)
        {
            List<IPolyhedronSide3D> sideList;
            if(!vertexSidesDictionary.TryGetValue(vertex, out sideList))
            {
                vertexSidesDictionary[vertex] = sideList = new List<IPolyhedronSide3D>();
            }
            sideList.Add(side);
        }

        public void Clear()
        {
            vertexSidesDictionary.Clear();
        }

        public IList<IPolyhedronSide3D> GetSideList4Vertex(IPolyhedronVertex3D vertex)
        {
            List<IPolyhedronSide3D> sideList;
            if(!vertexSidesDictionary.TryGetValue(vertex, out sideList))
            {
                sideList = new List<IPolyhedronSide3D>();
            }
            return new ReadOnlyCollection<IPolyhedronSide3D>(sideList);
        }

        public void RemoveSide4Vertex(IPolyhedronVertex3D vertex, IPolyhedronSide3D side)
        {
            List<IPolyhedronSide3D> sideList;
            if(vertexSidesDictionary.TryGetValue(vertex, out sideList))
            {
                sideList.Remove(side);
            }
        }

        private readonly Dictionary<IPolyhedronVertex3D, List<IPolyhedronSide3D>> vertexSidesDictionary;
    }
}