using System;
using System.Collections.Generic;
using LinearDiff3DGame.Common;
using NUnit.Framework;

namespace LinearDiff3DGame.AdvMath.Algorithms
{
    [TestFixture]
    public class ConvexSystemExtremumFinder_Test
    {
        [Test]
        public void FindPoligonPointWithMinDistance()
        {
            Point origin = new Point(0, 0);
            Point expectedPoint = new Point(1, 0);
            ConvexSystemExtremumFinder finder = new ConvexSystemExtremumFinder();
            Point point = finder.SearchMin(polygon[4],
                                           p => new List<Point> { polygon.GetPrevItem(p), polygon.GetNextItem(p) },
                                           p => Math.Sqrt((p.X - origin.X) * (p.X - origin.X) + (p.Y - origin.Y) * (p.Y - origin.Y)));
            Assert.AreEqual(expectedPoint, point);
        }

        [Test]
        public void FindPoligonPointWithMaxDistance()
        {
            Point origin = new Point(0, 0);
            Point expectedPoint = new Point(5, 0);
            ConvexSystemExtremumFinder finder = new ConvexSystemExtremumFinder();
            Point point = finder.SearchMax(polygon[0],
                                           p => new List<Point> { polygon.GetPrevItem(p), polygon.GetNextItem(p) },
                                           p => Math.Sqrt((p.X - origin.X) * (p.X - origin.X) + (p.Y - origin.Y) * (p.Y - origin.Y)));
            Assert.AreEqual(expectedPoint, point);
        }

        private readonly Point[] polygon = new[]
		                                   	{
		                                   		new Point(1, 0),
		                                   		new Point(2, 1),
		                                   		new Point(3, 1.5),
		                                   		new Point(4, 1),
		                                   		new Point(5, 0),
		                                   		new Point(4, -1),
		                                   		new Point(3, -2),
		                                   		new Point(2, -0.5)
		                                   	};

        private class Point : Tuple<Double, Double>
        {
            public Point(Double x, Double y)
                : base(x, y)
            {
            }

            public Double X
            {
                get { return Item1; }
            }

            public Double Y
            {
                get { return Item2; }
            }
        }
    }
}
