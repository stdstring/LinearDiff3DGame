using System;
using System.Windows.Forms;
using LinearDiff3DGame.OpenGLVisualizerTest.BridgeController;

namespace LinearDiff3DGame.OpenGLVisualizerTest
{
    public partial class CalculationProgressForm : Form
    {
        public CalculationProgressForm(Int32 bridgeSectionCount)
        {
            InitializeComponent();
            pbCalculationProgress.Minimum = 0;
            pbCalculationProgress.Maximum = bridgeSectionCount;
            pbCalculationProgress.Value = 0;
            pbCalculationProgress.Step = 1;
        }

        public void SectionCalculated(Object sender, EventArgs e)
        {
            if(pbCalculationProgress.Value < pbCalculationProgress.Maximum)
                pbCalculationProgress.Value += 1;
        }

        public void BridgeCalculated(Object sender, BridgeCompletedEventArgs e)
        {
            DialogResult = e.Success ? DialogResult.OK : DialogResult.Cancel;
        }

        public event EventHandler OnCalculationStart;
        public event EventHandler OnCalculationCancel;

        private void CalculationProgressForm_Shown(object sender, EventArgs e)
        {
            if(OnCalculationStart != null)
                OnCalculationStart(this, new EventArgs());
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if(OnCalculationCancel != null)
                OnCalculationCancel(this, new EventArgs());
        }
    }
}