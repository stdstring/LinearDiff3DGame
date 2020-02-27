using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MathPostgraduateStudy.BuildRobustControl
{
    public partial class OpenGLPaintBox : PopovYuri.Visualization.OpenGLControl
    {
        public OpenGLPaintBox()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // ���������� �������� OpenGL
            ActivateContext();

            base.OnPaint(e);

            // ����������� ������
            SwapBuffers();
            // ������������ �������� OpenGL
            DeactivateContext();
        }

        protected override void SetProjection()
        {
            glMatrixMode(GL_PROJECTION);
            glLoadIdentity();

            //glOrtho(-4, 4, -4, 4, -4, 4);
            glOrtho(-6, 6, -6, 6, 0, 12);
        }
    }
}

