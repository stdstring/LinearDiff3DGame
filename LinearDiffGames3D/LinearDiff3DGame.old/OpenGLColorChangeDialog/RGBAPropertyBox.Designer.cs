namespace MathPostgraduateStudy.OpenGLColorChangeDialog
{
    partial class RGBAPropertyBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblR = new System.Windows.Forms.Label();
            this.tbR = new System.Windows.Forms.TextBox();
            this.tbG = new System.Windows.Forms.TextBox();
            this.lblG = new System.Windows.Forms.Label();
            this.tbB = new System.Windows.Forms.TextBox();
            this.lblB = new System.Windows.Forms.Label();
            this.tbA = new System.Windows.Forms.TextBox();
            this.lblA = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.Location = new System.Drawing.Point(13, 11);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(254, 23);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "none";
            // 
            // lblR
            // 
            this.lblR.Location = new System.Drawing.Point(13, 45);
            this.lblR.Name = "lblR";
            this.lblR.Size = new System.Drawing.Size(65, 23);
            this.lblR.TabIndex = 1;
            this.lblR.Text = "R";
            // 
            // tbR
            // 
            this.tbR.Location = new System.Drawing.Point(84, 45);
            this.tbR.Name = "tbR";
            this.tbR.Size = new System.Drawing.Size(163, 20);
            this.tbR.TabIndex = 2;
            this.tbR.Validating += new System.ComponentModel.CancelEventHandler(this.tbR_Validating);
            // 
            // tbG
            // 
            this.tbG.Location = new System.Drawing.Point(84, 68);
            this.tbG.Name = "tbG";
            this.tbG.Size = new System.Drawing.Size(163, 20);
            this.tbG.TabIndex = 4;
            this.tbG.Validating += new System.ComponentModel.CancelEventHandler(this.tbG_Validating);
            // 
            // lblG
            // 
            this.lblG.Location = new System.Drawing.Point(13, 68);
            this.lblG.Name = "lblG";
            this.lblG.Size = new System.Drawing.Size(65, 23);
            this.lblG.TabIndex = 3;
            this.lblG.Text = "G";
            // 
            // tbB
            // 
            this.tbB.Location = new System.Drawing.Point(84, 91);
            this.tbB.Name = "tbB";
            this.tbB.Size = new System.Drawing.Size(163, 20);
            this.tbB.TabIndex = 6;
            this.tbB.Validating += new System.ComponentModel.CancelEventHandler(this.tbB_Validating);
            // 
            // lblB
            // 
            this.lblB.Location = new System.Drawing.Point(13, 91);
            this.lblB.Name = "lblB";
            this.lblB.Size = new System.Drawing.Size(65, 23);
            this.lblB.TabIndex = 5;
            this.lblB.Text = "B";
            // 
            // tbA
            // 
            this.tbA.Location = new System.Drawing.Point(84, 114);
            this.tbA.Name = "tbA";
            this.tbA.Size = new System.Drawing.Size(163, 20);
            this.tbA.TabIndex = 8;
            this.tbA.Validating += new System.ComponentModel.CancelEventHandler(this.tbA_Validating);
            // 
            // lblA
            // 
            this.lblA.Location = new System.Drawing.Point(13, 114);
            this.lblA.Name = "lblA";
            this.lblA.Size = new System.Drawing.Size(65, 23);
            this.lblA.TabIndex = 7;
            this.lblA.Text = "A";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // RGBAPropertyBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbA);
            this.Controls.Add(this.lblA);
            this.Controls.Add(this.tbB);
            this.Controls.Add(this.lblB);
            this.Controls.Add(this.tbG);
            this.Controls.Add(this.lblG);
            this.Controls.Add(this.tbR);
            this.Controls.Add(this.lblR);
            this.Controls.Add(this.lblHeader);
            this.Name = "RGBAPropertyBox";
            this.Size = new System.Drawing.Size(280, 152);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblR;
        private System.Windows.Forms.TextBox tbR;
        private System.Windows.Forms.TextBox tbG;
        private System.Windows.Forms.Label lblG;
        private System.Windows.Forms.TextBox tbB;
        private System.Windows.Forms.Label lblB;
        private System.Windows.Forms.TextBox tbA;
        private System.Windows.Forms.Label lblA;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}
