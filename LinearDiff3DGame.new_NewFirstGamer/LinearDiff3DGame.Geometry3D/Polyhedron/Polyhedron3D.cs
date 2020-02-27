using System.Collections.Generic;

namespace LinearDiff3DGame.Geometry3D.Polyhedron
{
    /// <summary>
    /// представляет 3-мерный многогранник
    /// </summary>
    public class Polyhedron3D
    {
        /// <summary>
        /// конструктор Polyhedron3D
        /// </summary>
        /// <param name="sideList">список граней многогранника</param>
        /// <param name="vertexList">список вершин многогранника</param>
        public Polyhedron3D(IEnumerable<PolyhedronSide3D> sideList, IEnumerable<PolyhedronVertex3D> vertexList)
        {
            m_SideList = new List<PolyhedronSide3D>(sideList);
            m_VertexList = new List<PolyhedronVertex3D>(vertexList);
        }

        /* это не есть хорошо  ... к тому же возможна несогласованность списка вершин и списка граней данного многогранника */
#warning Incorrect architecture

        /// <summary>
        /// список граней многогранника
        /// </summary>
        public IList<PolyhedronSide3D> SideList
        {
            get { return m_SideList; }
        }

        /// <summary>
        /// список вершин многогранника
        /// </summary>
        public IList<PolyhedronVertex3D> VertexList
        {
            get { return m_VertexList; }
        }

        /* это не есть хорошо  ... к тому же возможна несогласованность списка вершин и списка граней данного многогранника */

        /// <summary>
        /// список граней многогранника
        /// </summary>
        private readonly List<PolyhedronSide3D> m_SideList;

        /// <summary>
        /// список вершин многогранника
        /// </summary>
        private readonly List<PolyhedronVertex3D> m_VertexList;
    }
}