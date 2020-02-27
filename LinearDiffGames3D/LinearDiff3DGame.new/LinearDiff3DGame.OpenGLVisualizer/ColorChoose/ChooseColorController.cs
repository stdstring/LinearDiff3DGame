using LinearDiff3DGame.OpenGLVisualizer.Objects3D;

namespace LinearDiff3DGame.OpenGLVisualizer.ColorChoose
{
    internal class ChooseColorController
    {
        public Polyhedron GetSamplePolyhedron()
        {
            PolyhedronSide side1 = new PolyhedronSide(new Vector(0, 0, 1),
                                                      new[] {new Point(1, 1, 1), new Point(-1, 1, 1), new Point(-1, -1, 1), new Point(1, -1, 1)});
            PolyhedronSide side2 = new PolyhedronSide(new Vector(1, 0, 0),
                                                      new[] {new Point(1, 1, -1), new Point(1, 1, 1), new Point(1, -1, 1), new Point(1, -1, -1)});
            PolyhedronSide side3 = new PolyhedronSide(new Vector(0, 1, 0),
                                                      new[] {new Point(-1, 1, -1), new Point(-1, 1, 1), new Point(1, 1, 1), new Point(1, 1, -1)});
            PolyhedronSide side4 = new PolyhedronSide(new Vector(0, 0, -1),
                                                      new[] {new Point(-1, 1, -1), new Point(1, 1, -1), new Point(1, -1, -1), new Point(-1, -1, -1)});
            PolyhedronSide side5 = new PolyhedronSide(new Vector(-1, 0, 0),
                                                      new[] {new Point(-1, -1, -1), new Point(-1, -1, 1), new Point(-1, 1, 1), new Point(-1, 1, -1)});
            PolyhedronSide side6 = new PolyhedronSide(new Vector(0, -1, 0),
                                                      new[] {new Point(1, -1, -1), new Point(1, -1, 1), new Point(-1, -1, 1), new Point(-1, -1, -1)});
            return new Polyhedron(new[] {side1, side2, side3, side4, side5, side6});
        }
    }
}