using System;
using LinearDiff3DGame.OpenGLVisualizerTest.Objects3D;
using OpenGLControlTest;

namespace LinearDiff3DGame.OpenGLVisualizerTest.VisualisationHelpers
{
    internal class TriangleSideVisualization : IPolyhedronSideVisualisation
    {
        public void CreateVisualisation(PolyhedronSide side)
        {
            Point vertex0 = side.VertexList[0];
            for(Int32 vertexIndex = 2; vertexIndex < side.VertexList.Count; ++vertexIndex)
            {
                Point vertex1 = side.VertexList[vertexIndex - 1];
                Point vertex2 = side.VertexList[vertexIndex];
                OpenGLControl.glBegin(OpenGLControl.GL_TRIANGLES);
                OpenGLControl.glNormal3d(side.Normal.X, side.Normal.Y, side.Normal.Z);
                OpenGLControl.glEdgeFlag(vertexIndex == 2 ? (Byte)OpenGLControl.GL_TRUE : (Byte)OpenGLControl.GL_FALSE);
                OpenGLControl.glVertex3d(vertex0.X, vertex0.Y, vertex0.Z);
                OpenGLControl.glEdgeFlag((Byte)OpenGLControl.GL_TRUE);
                OpenGLControl.glVertex3d(vertex1.X, vertex1.Y, vertex1.Z);
                OpenGLControl.glEdgeFlag(vertexIndex == side.VertexList.Count - 1 ? (Byte)OpenGLControl.GL_TRUE : (Byte)OpenGLControl.GL_FALSE);
                OpenGLControl.glVertex3d(vertex2.X, vertex2.Y, vertex2.Z);
                OpenGLControl.glEnd();
            }
        }
    }
}