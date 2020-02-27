using System.Windows.Forms;
using OpenGLControlTest;

namespace LinearDiff3DGame.OpenGLVisualizerTest
{
    public partial class OpenGLPaintBox : OpenGLControl
    {
        public OpenGLPaintBox()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Активируем контекст OpenGL
            ActivateContext();

            base.OnPaint(e);

            // Переключаем буфера
            SwapBuffers();
            // Деактивируем контекст OpenGL
            DeactivateContext();
        }

        protected override void SetProjection()
        {
            glMatrixMode(GL_PROJECTION);
            glLoadIdentity();

            //glOrtho(-4, 4, -4, 4, -4, 4);
            //glOrtho(-20, 20, -20, 20, 0, 30);
            float dAspect = Width <= Height ? (float)Height / Width : (float)Width / Height;
            gluPerspective(45.0, dAspect, 0.01, 10000.0);
        }
    }
}