using System;
using System.Collections.Generic;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.OpenGLVisualizer.Objects3D;

namespace LinearDiff3DGame.OpenGLVisualizer.BridgeController
{
    internal interface IBridgeControllerAsync
    {
        void Run();
        void Cancel();
        event EventHandler OnSectionCompleted;
        event EventHandler<BridgeCompletedEventArgs> OnBridgeCompleted;
        IList<Pair<Double, Polyhedron>> Bridge { get; }
    }
}