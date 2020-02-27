using System;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.RobustControl.BridgeControl
{
    public interface IBridgeFinder
    {
        Tuple<IPolyhedron3D, Double> FindTSection(Double time, Point3D currentPos);
    }
}