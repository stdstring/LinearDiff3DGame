using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MathPostgraduateStudy.OpenGLColorChangeDialog
{
    public partial class OpenGLPaintBox : PopovYuri.Visualization.OpenGLControl
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
            glOrtho(-5, 5, -5, 5, 0, 10);
        }
    }
}

