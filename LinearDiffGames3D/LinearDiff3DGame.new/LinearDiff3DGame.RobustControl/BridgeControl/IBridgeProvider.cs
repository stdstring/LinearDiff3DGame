using System;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.RobustControl.BridgeControl
{
    public interface IBridgeProvider
    {
        IPolyhedron3D GetTSection(Double time, Double k);
    }
}