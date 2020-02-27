using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MathPostgraduateStudy.LinearDiff3DGame
{
    public partial class SightParamForm : Form
    {
        private Double m_nx;
        private Double m_ny;
        private Double m_nz;
        private Double m_Distance;

        public SightParamForm()
        {
            InitializeComponent();
        }

        public Double nx
        {
            get
            {
                return m_nx;
            }
            set
            {
                m_nx = value;
                tbNX.Text = m_nx.ToString();
            }
        }

        public Double ny
        {
            get
            {
                return m_ny;
            }
            set
            {
                m_ny = value;
                tbNY.Text = m_ny.ToString();
            }
        }

        public Double nz
        {
            get
            {
                return m_nz;
            }
            set
            {
                m_nz = value;
                tbNZ.Text = m_nz.ToString();
            }
        }

        public Double Distance
        {
            get
            {
                return m_Distance;
            }
            set
            {
                m_Distance = value;
                tbDistance.Text = m_Distance.ToString();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Double NewNX;
            Double NewNY;
            Double NewNZ;
            Double NewDistance;

            if (Double.TryParse(tbNX.Text, out NewNX))
            {
                m_nx = NewNX;
            }
            if (Double.TryParse(tbNY.Text, out NewNY))
            {
                m_ny = NewNY;
            }
            if (Double.TryParse(tbNZ.Text, out NewNZ))
            {
                m_nz = NewNZ;
            }
            if (Double.TryParse(tbDistance.Text, out NewDistance))
            {
                m_Distance = NewDistance;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}