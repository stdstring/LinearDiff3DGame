using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.MaxStableBridge;
using LinearDiff3DGame.OpenGLVisualizerTest.Objects3D;

namespace LinearDiff3DGame.OpenGLVisualizerTest.BridgeController
{
    internal class BridgeControllerSync : IBridgeControllerSync
    {
        public BridgeControllerSync()
        {
            Bridge = new ReadOnlyCollection<Pair<Double, Polyhedron>>(new List<Pair<Double, Polyhedron>>());
        }

        public IList<Pair<Double, Polyhedron>> CalculateBridge(String inputDataFile, Double finishTime)
        {
            BridgeBuildController bridgeBuilder = new BridgeBuildController(inputDataFile);
            IList<Pair<Double, Polyhedron3D>> bridge = bridgeBuilder.GenerateBridge(finishTime);
            return Bridge = new ReadOnlyCollection<Pair<Double, Polyhedron>>
                                (
                                bridge
                                    .Select(source => new Pair<Double, Polyhedron>(source.Item1, new Polyhedron(source.Item2)))
                                    .ToList()
                                );
        }

        public IList<Pair<Double, Polyhedron>> Bridge { get; private set; }
    }
}