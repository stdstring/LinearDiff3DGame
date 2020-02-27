namespace LinearDiff3DGame.OpenGLVisualizer.ColorChoose
{
    partial class RGBPropertyBox
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
            this.tbBComponent = new System.Windows.Forms.TextBox();
            this.tbGComponent = new System.Windows.Forms.TextBox();
            this.tbRComponent = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblHeader = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // tbBComponent
            // 
            this.tbBComponent.Location = new System.Drawing.Point(50, 79);
            this.tbBComponent.Name = "tbBComponent";
            this.tbBComponent.Size = new System.Drawing.Size(100, 20);
            this.tbBComponent.TabIndex = 13;
            // 
            // tbGComponent
            // 
            this.tbGComponent.Location = new System.Drawing.Point(50, 56);
            this.tbGComponent.Name = "tbGComponent";
            this.tbGComponent.Size = new System.Drawing.Size(100, 20);
            this.tbGComponent.TabIndex = 12;
            // 
            // tbRComponent
            // 
            this.tbRComponent.Location = new System.Drawing.Point(50, 33);
            this.tbRComponent.Name = "tbRComponent";
            this.tbRComponent.Size = new System.Drawing.Size(100, 20);
            this.tbRComponent.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 23);
            this.label4.TabIndex = 10;
            this.label4.Text = "B";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 23);
            this.label3.TabIndex = 9;
            this.label3.Text = "G";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 23);
            this.label2.TabIndex = 8;
            this.label2.Text = "R";
            // 
            // lblHeader
            // 
            this.lblHeader.Location = new System.Drawing.Point(3, 9);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(100, 23);
            this.lblHeader.TabIndex = 7;
            this.lblHeader.Text = "none:";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // RGBPropertyBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbBComponent);
            this.Controls.Add(this.tbGComponent);
            this.Controls.Add(this.tbRComponent);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblHeader);
            this.Name = "RGBPropertyBox";
            this.Size = new System.Drawing.Size(193, 113);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbBComponent;
        private System.Windows.Forms.TextBox tbGComponent;
        private System.Windows.Forms.TextBox tbRComponent;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}