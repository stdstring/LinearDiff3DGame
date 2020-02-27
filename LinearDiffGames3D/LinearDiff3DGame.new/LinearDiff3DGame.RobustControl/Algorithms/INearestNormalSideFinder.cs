using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.RobustControl.Algorithms
{
    public interface INearestNormalSideFinder
    {
        IPolyhedronSide3D Search(IPolyhedron3D polyhedron, Vector3D direction);
    }
}