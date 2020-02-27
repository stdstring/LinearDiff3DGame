using System;
using LinearDiff3DGame.OpenGLVisualizerTest.Objects3D;
using OpenGLControlTest;

namespace LinearDiff3DGame.OpenGLVisualizerTest.VisualisationHelpers
{
    internal class PolygonSideVisualization : IPolyhedronSideVisualisation
    {
        public void CreateVisualisation(PolyhedronSide side)
        {
            OpenGLControl.glBegin(OpenGLControl.GL_LINE_LOOP);
            OpenGLControl.glNormal3d(side.Normal.X, side.Normal.Y, side.Normal.Z);
            foreach(Point vertex in side.VertexList)
            {
                OpenGLControl.glEdgeFlag((Byte)OpenGLControl.GL_TRUE);
                OpenGLControl.glVertex3d(vertex.X, vertex.Y, vertex.Z);
            }
            OpenGLControl.glEnd();
        }
    }
}