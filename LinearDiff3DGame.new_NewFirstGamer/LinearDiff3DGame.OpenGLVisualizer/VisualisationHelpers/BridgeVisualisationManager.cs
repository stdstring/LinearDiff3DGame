using System;
using System.Collections.Generic;
using System.Drawing;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.OpenGLVisualizer.Objects3D;
using OpenGLTools;

namespace LinearDiff3DGame.OpenGLVisualizer.VisualisationHelpers
{
    internal class BridgeVisualisationManager : IDisposable
    {
        public BridgeVisualisationManager(IPolyhedronSideVisualisation sideVisualisation)
        {
            this.sideVisualisation = sideVisualisation;
            ListBase = OpenGLImport.GL_INVALID_VALUE;
            ListCount = 0;
        }

        public void Dispose()
        {
            ClearVisualisation();
        }

        public void CreateVisualisation(IList<Pair<Double, Polyhedron>> bridge)
        {
            ClearVisualisation();
            if((ListBase = OpenGLImport.glGenLists(bridge.Count)) == OpenGLImport.GL_INVALID_VALUE)
                throw new ApplicationException("Call glGenLists failed.");
            ListCount = bridge.Count;
            for(UInt32 sectionIndex = 0; sectionIndex < ListCount; ++sectionIndex)
            {
                OpenGLImport.glNewList(ListBase + sectionIndex, OpenGLImport.GL_COMPILE);
                CreatePolyhedronVisualisation(bridge[(Int32)sectionIndex].Item2);
                OpenGLImport.glEndList();
            }
        }

        public void ClearVisualisation()
        {
            if(ListBase != OpenGLImport.GL_INVALID_VALUE)
                OpenGLImport.glDeleteLists(ListBase, ListCount);
            ListBase = OpenGLImport.GL_INVALID_VALUE;
            ListCount = 0;
        }

        public void ApplyVisualisation(Int32 sectionIndex,
                                       Color polyhedronColor,
                                       Color contourColor,
                                       Single contourWidth)
        {
            OpenGLImport.glEnable(OpenGLImport.GL_NORMALIZE);
            OpenGLImport.glEnable(OpenGLImport.GL_LIGHTING);
            OpenGLImport.glEnable(OpenGLImport.GL_LIGHT0);
            OpenGLImport.glEnable(OpenGLImport.GL_COLOR_MATERIAL);
            OpenGLImport.glLightModeli(OpenGLImport.GL_LIGHT_MODEL_TWO_SIDE, OpenGLImport.GL_TRUE);
            // многогранник
            OpenGLImport.glPolygonMode(OpenGLImport.GL_FRONT, OpenGLImport.GL_FILL);
            OpenGLImport.glEnable(OpenGLImport.GL_CULL_FACE);
            OpenGLImport.glCullFace(OpenGLImport.GL_BACK);
            OpenGLImport.glColor4d(polyhedronColor.R / 255.0,
                                   polyhedronColor.G / 255.0,
                                   polyhedronColor.B / 255.0,
                                   1.0);
            //polyhedronColor.A / 255.0);
            OpenGLImport.glCallList(ListBase + (UInt32)sectionIndex);
            // контур многогранника
            OpenGLImport.glPolygonMode(OpenGLImport.GL_FRONT, OpenGLImport.GL_LINE);
            OpenGLImport.glEnable(OpenGLImport.GL_CULL_FACE);
            OpenGLImport.glCullFace(OpenGLImport.GL_BACK);
            OpenGLImport.glColor4d(contourColor.R / 255.0,
                                   contourColor.G / 255.0,
                                   contourColor.B / 255.0,
                                   1.0);
            //contourColor.A / 255.0);
            OpenGLImport.glLineWidth(contourWidth);
            OpenGLImport.glCallList(ListBase + (UInt32)sectionIndex);
        }

        public UInt32 ListBase { get; private set; }
        public Int32 ListCount { get; private set; }
        public Boolean IsEmpty { get { return ListCount == 0; } }

        private void CreatePolyhedronVisualisation(Polyhedron polyhedron)
        {
            foreach(PolyhedronSide side in polyhedron.SideList)
                sideVisualisation.CreateVisualisation(side);
        }

        private readonly IPolyhedronSideVisualisation sideVisualisation;

        //~BridgeVisualisationManager()
        //{
        //    Dispose();
        //}
    }
}