using System;
using System.Collections.Generic;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.OpenGLVisualizerTest.Objects3D;

namespace LinearDiff3DGame.OpenGLVisualizerTest.BridgeController
{
    internal interface IBridgeControllerSync
    {
        IList<Pair<Double, Polyhedron>> CalculateBridge(String inputDataFile, Double finishTime);
        IList<Pair<Double, Polyhedron>> Bridge { get; }
    }
}