namespace LinearDiff3DGame.OpenGLVisualizer
{
    partial class CalculateParamsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.chooseInputDataFile = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbFinishTime = new System.Windows.Forms.TextBox();
            this.tbInputDataFile = new System.Windows.Forms.TextBox();
            this.btnChooseInputDataFile = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 117);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(450, 49);
            this.panel1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(363, 14);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(282, 14);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(22, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Input data\'s file";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(22, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 19);
            this.label2.TabIndex = 2;
            this.label2.Text = "Finish time";
            // 
            // tbFinishTime
            // 
            this.tbFinishTime.Location = new System.Drawing.Point(142, 51);
            this.tbFinishTime.Name = "tbFinishTime";
            this.tbFinishTime.Size = new System.Drawing.Size(245, 20);
            this.tbFinishTime.TabIndex = 3;
            // 
            // tbInputDataFile
            // 
            this.tbInputDataFile.Location = new System.Drawing.Point(142, 19);
            this.tbInputDataFile.Name = "tbInputDataFile";
            this.tbInputDataFile.Size = new System.Drawing.Size(245, 20);
            this.tbInputDataFile.TabIndex = 4;
            // 
            // btnChooseInputDataFile
            // 
            this.btnChooseInputDataFile.Location = new System.Drawing.Point(393, 19);
            this.btnChooseInputDataFile.Name = "btnChooseInputDataFile";
            this.btnChooseInputDataFile.Size = new System.Drawing.Size(24, 23);
            this.btnChooseInputDataFile.TabIndex = 5;
            this.btnChooseInputDataFile.Text = "...";
            this.btnChooseInputDataFile.UseVisualStyleBackColor = true;
            this.btnChooseInputDataFile.Click += new System.EventHandler(this.btnChooseInputDataFile_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // CalculateParamsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 166);
            this.Controls.Add(this.btnChooseInputDataFile);
            this.Controls.Add(this.tbInputDataFile);
            this.Controls.Add(this.tbFinishTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Name = "CalculateParamsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CalculateParamsDialog";
            this.Shown += new System.EventHandler(this.CalculateParamsDialog_Shown);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog chooseInputDataFile;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbFinishTime;
        private System.Windows.Forms.TextBox tbInputDataFile;
        private System.Windows.Forms.Button btnChooseInputDataFile;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}