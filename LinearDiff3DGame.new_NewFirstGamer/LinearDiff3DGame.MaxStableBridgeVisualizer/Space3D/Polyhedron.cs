using System.Collections.Generic;
using System.Collections.ObjectModel;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.MaxStableBridgeVisualizer.Space3D
{
    /// <summary>
    /// представление многогранника
    /// </summary>
    internal class Polyhedron
    {
        internal Polyhedron(Polyhedron3D polyhedron)
        {
            IList<PolyhedronSide> sideList = new List<PolyhedronSide>(polyhedron.SideList.Count);
            foreach (PolyhedronSide3D side in polyhedron.SideList)
            {
                sideList.Add(new PolyhedronSide(side));
            }
            m_SideList = new ReadOnlyCollection<PolyhedronSide>(sideList);
        }

        public IList<PolyhedronSide> SideList
        {
            get { return m_SideList; }
        }

        private readonly ReadOnlyCollection<PolyhedronSide> m_SideList;
    }
}
