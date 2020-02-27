using System;
using System.IO;
using System.Windows.Forms;

namespace LinearDiff3DGame.OpenGLVisualizer
{
    public partial class CalculateParamsDialog : Form
    {
        public CalculateParamsDialog()
        {
            InitializeComponent();
        }

        public String InputDataFile { get; set; }
        public Double FinishTime { get; set; }
        public String InitialDirectory { get; set; }

        private void BeforeShow()
        {
            tbInputDataFile.Text = InputDataFile;
            tbFinishTime.Text = FinishTime.ToString();
        }

        private Boolean ValidateData()
        {
            Boolean result = true;
            String fileName = tbInputDataFile.Text;
            if(File.Exists(fileName))
            {
                errorProvider1.SetError(tbInputDataFile, "");
                InputDataFile = fileName;
            }
            else
            {
                errorProvider1.SetError(tbInputDataFile, String.Format("File {0} does not exist.", fileName));
                result = false;
            }
            Double finishTime;
            if(Double.TryParse(tbFinishTime.Text, out finishTime))
            {
                FinishTime = finishTime;
                errorProvider1.SetError(tbFinishTime, "");
            }
            else
            {
                errorProvider1.SetError(tbFinishTime, String.Format("Value {0} is not double number.", tbFinishTime.Text));
                result = false;
            }
            return result;
        }

        private void btnChooseInputDataFile_Click(object sender, EventArgs e)
        {
            chooseInputDataFile.InitialDirectory = InitialDirectory;
            if(chooseInputDataFile.ShowDialog(this) == DialogResult.OK)
                tbInputDataFile.Text = InputDataFile = chooseInputDataFile.FileName;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(ValidateData())
                DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void CalculateParamsDialog_Shown(object sender, EventArgs e)
        {
            BeforeShow();
        }
    }
}