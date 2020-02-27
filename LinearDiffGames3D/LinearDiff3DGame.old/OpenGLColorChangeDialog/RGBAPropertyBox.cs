using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MathPostgraduateStudy.OpenGLColorChangeDialog
{
    public partial class RGBAPropertyBox : UserControl
    {
        public RGBAPropertyBox()
        {
            InitializeComponent();

            m_ColorProperty = Color.FromArgb(255, 0, 0, 0);
            tbR.Text = "0";
            tbG.Text = "0";
            tbB.Text = "0";
            tbA.Text = "255";
        }

        /// <summary>
        /// 
        /// </summary>
        public Color ColorProperty
        {
            get
            {
                return m_ColorProperty;
            }
            set
            {
                m_ColorProperty = value;
                tbR.Text = m_ColorProperty.R.ToString();
                tbG.Text = m_ColorProperty.G.ToString();
                tbB.Text = m_ColorProperty.B.ToString();
                tbA.Text = m_ColorProperty.A.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Header
        {
            get
            {
                return lblHeader.Text;
            }
            set
            {
                lblHeader.Text = value;
            }
        }

        private void tbR_Validating(object sender, CancelEventArgs e)
        {
            Byte redValue;

            if (Byte.TryParse(tbR.Text, out redValue))
            {
                errorProvider1.SetError(tbR, "");
                m_ColorProperty = Color.FromArgb(m_ColorProperty.A, redValue, m_ColorProperty.G, m_ColorProperty.B);
            }
            else
            {
                errorProvider1.SetError(tbR, "Red value must be between 0 and 255");
            }
        }

        private void tbG_Validating(object sender, CancelEventArgs e)
        {
            Byte greenValue;

            if (Byte.TryParse(tbG.Text, out greenValue))
            {
                errorProvider1.SetError(tbG, "");
                m_ColorProperty = Color.FromArgb(m_ColorProperty.A, m_ColorProperty.R, greenValue, m_ColorProperty.B);
            }
            else
            {
                errorProvider1.SetError(tbG, "Green value must be between 0 and 255");
            }
        }

        private void tbB_Validating(object sender, CancelEventArgs e)
        {
            Byte blueValue;

            if (Byte.TryParse(tbB.Text, out blueValue))
            {
                errorProvider1.SetError(tbB, "");
                m_ColorProperty = Color.FromArgb(m_ColorProperty.A, m_ColorProperty.R, m_ColorProperty.G, blueValue);
            }
            else
            {
                errorProvider1.SetError(tbB, "Blue value must be between 0 and 255");
            }
        }

        private void tbA_Validating(object sender, CancelEventArgs e)
        {
            Byte alphaValue;

            if (Byte.TryParse(tbA.Text, out alphaValue))
            {
                errorProvider1.SetError(tbA, "");
                m_ColorProperty = Color.FromArgb(alphaValue, m_ColorProperty.R, m_ColorProperty.G, m_ColorProperty.B);
            }
            else
            {
                errorProvider1.SetError(tbA, "Alpha value must be between 0 and 255");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Color m_ColorProperty;
    }
}
