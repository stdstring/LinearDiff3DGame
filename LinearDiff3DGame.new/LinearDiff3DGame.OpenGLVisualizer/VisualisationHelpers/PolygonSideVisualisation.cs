using LinearDiff3DGame.OpenGLVisualizer.Objects3D;
using OpenGLTools;

namespace LinearDiff3DGame.OpenGLVisualizer.VisualisationHelpers
{
    internal class PolygonSideVisualization : IPolyhedronSideVisualisation
    {
        public void CreateVisualisation(PolyhedronSide side)
        {
            OpenGLImport.glBegin(OpenGLImport.GL_POLYGON);
            OpenGLImport.glNormal3d(side.Normal.X, side.Normal.Y, side.Normal.Z);
            foreach(Point vertex in side.VertexList)
            {
                OpenGLImport.glEdgeFlag(OpenGLImport.GL_TRUE);
                OpenGLImport.glVertex3d(vertex.X, vertex.Y, vertex.Z);
            }
            OpenGLImport.glEnd();
        }
    }
}