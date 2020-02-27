using LinearDiff3DGame.AdvMath.Algorithms;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.RobustControl.Algorithms
{
	public class NearestNormalSideFinder : INearestNormalSideFinder
	{
		public NearestNormalSideFinder()
		{
			finder = new ConvexSystemExtremumFinder();
		}

		public IPolyhedronSide3D Search(IPolyhedron3D polyhedron, Vector3D direction)
		{
			// т.к. все нормали имеют одну длину, можно не требовать нормализации вектора direction
			return finder.SearchMin(polyhedron.SideList[0],
			                        side => polyhedron.GetNeighbours4Side(side),
			                        side => side.SideNormal*direction);
		}

		private readonly ConvexSystemExtremumFinder finder;
	}
}