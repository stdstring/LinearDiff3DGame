using System;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.RobustControl.Algorithms;
using LinearDiff3DGame.RobustControl.BridgeControl;

namespace LinearDiff3DGame.RobustControl
{
    public class RobustControlBuilder
    {
        public RobustControlBuilder(IBridgeFinder bridgeFinder, INearestPointFinder nearestPointFinder)
        {
            this.bridgeFinder = bridgeFinder;
            this.nearestPointFinder = nearestPointFinder;
        }

        public Tuple<Double, Point3D> Calculate(Double time, Point3D currentPos)
        {
            throw new NotImplementedException();
        }

        private readonly IBridgeFinder bridgeFinder;
        private readonly INearestPointFinder nearestPointFinder;
    }
}