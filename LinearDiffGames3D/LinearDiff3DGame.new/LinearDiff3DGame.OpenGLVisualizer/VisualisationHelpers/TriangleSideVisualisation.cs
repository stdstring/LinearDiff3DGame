using System;
using LinearDiff3DGame.OpenGLVisualizer.Objects3D;
using OpenGLTools;

namespace LinearDiff3DGame.OpenGLVisualizer.VisualisationHelpers
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
                OpenGLImport.glBegin(OpenGLImport.GL_TRIANGLES);
                OpenGLImport.glNormal3d(side.Normal.X, side.Normal.Y, side.Normal.Z);
                OpenGLImport.glEdgeFlag(vertexIndex == 2 ? OpenGLImport.GL_TRUE : OpenGLImport.GL_FALSE);
                OpenGLImport.glVertex3d(vertex0.X, vertex0.Y, vertex0.Z);
                OpenGLImport.glEdgeFlag(OpenGLImport.GL_TRUE);
                OpenGLImport.glVertex3d(vertex1.X, vertex1.Y, vertex1.Z);
                OpenGLImport.glEdgeFlag(vertexIndex == side.VertexList.Count - 1 ? OpenGLImport.GL_TRUE : OpenGLImport.GL_FALSE);
                OpenGLImport.glVertex3d(vertex2.X, vertex2.Y, vertex2.Z);
                OpenGLImport.glEnd();
            }
        }
    }
}