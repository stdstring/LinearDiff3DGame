namespace MathPostgraduateStudy.LinearDiff3DGame
{
    partial class SightParamForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbNX = new System.Windows.Forms.TextBox();
            this.tbNY = new System.Windows.Forms.TextBox();
            this.tbNZ = new System.Windows.Forms.TextBox();
            this.tbDistance = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(24, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "nx = ";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(24, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "ny =";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(24, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 23);
            this.label3.TabIndex = 2;
            this.label3.Text = "nz =";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(24, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(135, 23);
            this.label4.TabIndex = 3;
            this.label4.Text = "distance = ";
            // 
            // tbNX
            // 
            this.tbNX.Location = new System.Drawing.Point(186, 22);
            this.tbNX.Name = "tbNX";
            this.tbNX.Size = new System.Drawing.Size(183, 20);
            this.tbNX.TabIndex = 4;
            // 
            // tbNY
            // 
            this.tbNY.Location = new System.Drawing.Point(186, 64);
            this.tbNY.Name = "tbNY";
            this.tbNY.Size = new System.Drawing.Size(183, 20);
            this.tbNY.TabIndex = 5;
            // 
            // tbNZ
            // 
            this.tbNZ.Location = new System.Drawing.Point(186, 112);
            this.tbNZ.Name = "tbNZ";
            this.tbNZ.Size = new System.Drawing.Size(183, 20);
            this.tbNZ.TabIndex = 6;
            // 
            // tbDistance
            // 
            this.tbDistance.Location = new System.Drawing.Point(186, 152);
            this.tbDistance.Name = "tbDistance";
            this.tbDistance.Size = new System.Drawing.Size(183, 20);
            this.tbDistance.TabIndex = 7;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(186, 211);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(294, 211);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SightParamForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 256);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbDistance);
            this.Controls.Add(this.tbNZ);
            this.Controls.Add(this.tbNY);
            this.Controls.Add(this.tbNX);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "SightParamForm";
            this.Text = "SightParamForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbNX;
        private System.Windows.Forms.TextBox tbNY;
        private System.Windows.Forms.TextBox tbNZ;
        private System.Windows.Forms.TextBox tbDistance;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}