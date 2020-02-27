namespace MathPostgraduateStudy.OpenGLColorChangeDialog
{
    partial class OpenGLColorChangeForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.STCubeContourColorPB = new MathPostgraduateStudy.OpenGLColorChangeDialog.RGBAPropertyBox();
            this.STCubeColorPB = new MathPostgraduateStudy.OpenGLColorChangeDialog.RGBAPropertyBox();
            this.OCubeContourColorPB = new MathPostgraduateStudy.OpenGLColorChangeDialog.RGBAPropertyBox();
            this.OCubeColorPB = new MathPostgraduateStudy.OpenGLColorChangeDialog.RGBAPropertyBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.PaintBox = new MathPostgraduateStudy.OpenGLColorChangeDialog.OpenGLPaintBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.STCubeContourColorPB);
            this.panel1.Controls.Add(this.STCubeColorPB);
            this.panel1.Controls.Add(this.OCubeContourColorPB);
            this.panel1.Controls.Add(this.OCubeColorPB);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(630, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(317, 630);
            this.panel1.TabIndex = 1;
            // 
            // STCubeContourColorPB
            // 
            this.STCubeContourColorPB.ColorProperty = System.Drawing.Color.Empty;
            this.STCubeContourColorPB.Header = "Color of semi-transparent cube\'s contour";
            this.STCubeContourColorPB.Location = new System.Drawing.Point(14, 429);
            this.STCubeContourColorPB.Name = "STCubeContourColorPB";
            this.STCubeContourColorPB.Size = new System.Drawing.Size(280, 152);
            this.STCubeContourColorPB.TabIndex = 3;
            // 
            // STCubeColorPB
            // 
            this.STCubeColorPB.ColorProperty = System.Drawing.Color.Empty;
            this.STCubeColorPB.Header = "Color of semi-transparent cube";
            this.STCubeColorPB.Location = new System.Drawing.Point(14, 286);
            this.STCubeColorPB.Name = "STCubeColorPB";
            this.STCubeColorPB.Size = new System.Drawing.Size(280, 152);
            this.STCubeColorPB.TabIndex = 2;
            // 
            // OCubeContourColorPB
            // 
            this.OCubeContourColorPB.ColorProperty = System.Drawing.Color.Empty;
            this.OCubeContourColorPB.Header = "Color of opaque cube\'s contour";
            this.OCubeContourColorPB.Location = new System.Drawing.Point(14, 143);
            this.OCubeContourColorPB.Name = "OCubeContourColorPB";
            this.OCubeContourColorPB.Size = new System.Drawing.Size(280, 152);
            this.OCubeContourColorPB.TabIndex = 1;
            // 
            // OCubeColorPB
            // 
            this.OCubeColorPB.ColorProperty = System.Drawing.Color.Empty;
            this.OCubeColorPB.Header = "Color of opaque cube";
            this.OCubeColorPB.Location = new System.Drawing.Point(14, 3);
            this.OCubeColorPB.Name = "OCubeColorPB";
            this.OCubeColorPB.Size = new System.Drawing.Size(280, 152);
            this.OCubeColorPB.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.PaintBox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(630, 630);
            this.panel2.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.btnApply);
            this.panel3.Controls.Add(this.btnOK);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 530);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(630, 100);
            this.panel3.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(352, 40);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(271, 40);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(190, 40);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // PaintBox
            // 
            this.PaintBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PaintBox.Location = new System.Drawing.Point(0, 0);
            this.PaintBox.Name = "PaintBox";
            this.PaintBox.Size = new System.Drawing.Size(630, 630);
            this.PaintBox.TabIndex = 0;
            this.PaintBox.Paint += new System.Windows.Forms.PaintEventHandler(this.PaintBox_Paint);
            // 
            // OpenGLColorChangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(947, 630);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "OpenGLColorChangeForm";
            this.Text = "OpenGLColorChanger";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private RGBAPropertyBox STCubeContourColorPB;
        private RGBAPropertyBox STCubeColorPB;
        private RGBAPropertyBox OCubeContourColorPB;
        private RGBAPropertyBox OCubeColorPB;
        private System.Windows.Forms.Panel panel2;
        private OpenGLPaintBox PaintBox;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
    }
}

