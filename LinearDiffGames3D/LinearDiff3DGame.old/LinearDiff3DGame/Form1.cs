using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MathPostgraduateStudy.LinearDiff3DGame
{
    public partial class Form1 : Form
    {
        private AlgorithmClass m_AC;

        public Form1()
        {
            InitializeComponent();

            m_AC = new AlgorithmClass();
        }

        private void nextIterationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_AC.NextSolutionIteration();
            MessageBox.Show("Next iteration complete");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}