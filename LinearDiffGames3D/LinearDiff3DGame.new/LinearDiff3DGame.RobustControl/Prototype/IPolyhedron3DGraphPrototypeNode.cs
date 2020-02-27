using System;
using System.Collections.Generic;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.RobustControl.Prototype
{
    public interface IPolyhedron3DGraphPrototypeNode
    {
        Int32 ID { get; }
        IList<IPolyhedron3DGraphPrototypeNode> ConnectionList { get; }
        Vector3D NodeNormal { get; }
        Pair<Double> SupportFuncValues { get; set; }
    }
}