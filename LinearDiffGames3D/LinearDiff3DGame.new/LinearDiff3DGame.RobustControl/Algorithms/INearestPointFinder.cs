using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.RobustControl.Algorithms
{
    public interface INearestPointFinder
    {
        IPolyhedronSide3D Search(IPolyhedron3D polyhedron, Point3D point);
    }
}