﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.OpenGLVisualizerTest.Objects3D
{
    [Immutable]
    internal class PolyhedronSide
    {
        public PolyhedronSide(PolyhedronSide3D side)
        {
            Normal = new Vector(side.SideNormal.XCoord,
                                side.SideNormal.YCoord,
                                side.SideNormal.ZCoord);
            List<Point> vertexList = new List<Point>(side.VertexList.Count);
            foreach(PolyhedronVertex3D vertex in side.VertexList)
                vertexList.Add(new Point(vertex.XCoord, vertex.YCoord, vertex.ZCoord));
            VertexList = new ReadOnlyCollection<Point>(vertexList);
        }

        public PolyhedronSide(Vector normal, IEnumerable<Point> vertexList)
        {
            Normal = normal;
            VertexList = new ReadOnlyCollection<Point>(new List<Point>(vertexList));
        }

        public Vector Normal { get; private set; }
        public IList<Point> VertexList { get; private set; }
    }
}