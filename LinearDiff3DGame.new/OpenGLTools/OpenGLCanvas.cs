using System;
using System.Windows.Forms;

namespace OpenGLTools
{
    public partial class OpenGLCanvas : Control
    {
        public OpenGLCanvas()
        {
            InitializeComponent();
            openGLHelper = new OpenGLHelper(Handle, Width, Height);
            Disposed += DisposeCanvas;
        }

        public void DisposeCanvas(Object sender, EventArgs ea)
        {
            openGLHelper.Dispose();
        }

        protected override void OnResize(EventArgs e)
        {
            openGLHelper.SetViewport(Width, Height);
            openGLHelper.SetDefaultProjection(Width, Height);
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            openGLHelper.SwapBuffers();
        }

        private readonly OpenGLHelper openGLHelper;
    }
}