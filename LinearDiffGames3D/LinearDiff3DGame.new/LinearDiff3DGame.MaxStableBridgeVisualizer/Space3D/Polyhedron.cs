using System.Collections.Generic;
using System.Collections.ObjectModel;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.MaxStableBridgeVisualizer.Space3D
{
	// представление многогранника
	internal class Polyhedron
	{
		internal Polyhedron(IPolyhedron3D polyhedron)
		{
			IList<PolyhedronSide> sideList = new List<PolyhedronSide>(polyhedron.SideList.Count);
			foreach (IPolyhedronSide3D side in polyhedron.SideList)
			{
				sideList.Add(new PolyhedronSide(side));
			}
			this.sideList = new ReadOnlyCollection<PolyhedronSide>(sideList);
		}

		public IList<PolyhedronSide> SideList
		{
			get { return sideList; }
		}

		private readonly ReadOnlyCollection<PolyhedronSide> sideList;
	}
}