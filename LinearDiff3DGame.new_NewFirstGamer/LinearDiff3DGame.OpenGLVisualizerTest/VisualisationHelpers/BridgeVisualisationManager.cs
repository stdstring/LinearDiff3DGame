using System;
using System.Collections.Generic;
using System.Drawing;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.OpenGLVisualizerTest.Objects3D;
using OpenGLControlTest;

namespace LinearDiff3DGame.OpenGLVisualizerTest.VisualisationHelpers
{
    internal class BridgeVisualisationManager : IDisposable
    {
        public BridgeVisualisationManager(IPolyhedronSideVisualisation sideVisualisation)
        {
            this.sideVisualisation = sideVisualisation;
            ListBase = OpenGLControl.GL_INVALID_VALUE;
            ListCount = 0;
        }

        public void Dispose()
        {
            ClearVisualisation();
        }

        public void CreateVisualisation(IList<Pair<Double, Polyhedron>> bridge)
        {
            ClearVisualisation();
            if((ListBase = OpenGLControl.glGenLists(bridge.Count)) == OpenGLControl.GL_INVALID_VALUE)
                throw new ApplicationException("Call glGenLists failed.");
            ListCount = bridge.Count;
            for(UInt32 sectionIndex = 0; sectionIndex < ListCount; ++sectionIndex)
            {
                OpenGLControl.glNewList(ListBase + sectionIndex, OpenGLControl.GL_COMPILE);
                CreatePolyhedronVisualisation(bridge[(Int32)sectionIndex].Item2);
                OpenGLControl.glEndList();
            }
        }

        public void ClearVisualisation()
        {
            if(ListBase != OpenGLControl.GL_INVALID_VALUE)
                OpenGLControl.glDeleteLists(ListBase, ListCount);
            ListBase = OpenGLControl.GL_INVALID_VALUE;
            ListCount = 0;
        }

        public void ApplyVisualisation(Int32 sectionIndex,
                                       Color polyhedronColor,
                                       Color contourColor,
                                       Single contourWidth)
        {
            OpenGLControl.glEnable(OpenGLControl.GL_NORMALIZE);
            OpenGLControl.glEnable(OpenGLControl.GL_LIGHTING);
            OpenGLControl.glEnable(OpenGLControl.GL_LIGHT0);
            OpenGLControl.glEnable(OpenGLControl.GL_COLOR_MATERIAL);
            OpenGLControl.glLightModeli(OpenGLControl.GL_LIGHT_MODEL_TWO_SIDE, (Byte)OpenGLControl.GL_TRUE);
            // многогранник
            OpenGLControl.glPolygonMode(OpenGLControl.GL_FRONT, OpenGLControl.GL_FILL);
            OpenGLControl.glEnable(OpenGLControl.GL_CULL_FACE);
            OpenGLControl.glCullFace(OpenGLControl.GL_BACK);
            OpenGLControl.glColor4d(polyhedronColor.R / 255.0,
                                    polyhedronColor.G / 255.0,
                                    polyhedronColor.B / 255.0,
                                    1.0);
            //polyhedronColor.A / 255.0);
            OpenGLControl.glCallList(ListBase + (UInt32)sectionIndex);
            // контур многогранника
            OpenGLControl.glPolygonMode(OpenGLControl.GL_FRONT, OpenGLControl.GL_LINE);
            OpenGLControl.glEnable(OpenGLControl.GL_CULL_FACE);
            OpenGLControl.glCullFace(OpenGLControl.GL_BACK);
            OpenGLControl.glColor4d(contourColor.R / 255.0,
                                    contourColor.G / 255.0,
                                    contourColor.B / 255.0,
                                    1.0);
            //contourColor.A / 255.0);
            OpenGLControl.glLineWidth(contourWidth);
            OpenGLControl.glCallList(ListBase + (UInt32)sectionIndex);
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