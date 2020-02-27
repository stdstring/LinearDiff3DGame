namespace MathPostgraduateStudy.BuildRobustControl
{
    partial class VisualizationParamsForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbShowBridgeWithIndex = new System.Windows.Forms.RadioButton();
            this.rbShowBottomNearestBridge = new System.Windows.Forms.RadioButton();
            this.txtBridgeIndex = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtBridgeIndex);
            this.groupBox1.Controls.Add(this.rbShowBottomNearestBridge);
            this.groupBox1.Controls.Add(this.rbShowBridgeWithIndex);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(327, 129);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Outer bridge options";
            // 
            // rbShowBridgeWithIndex
            // 
            this.rbShowBridgeWithIndex.AutoSize = true;
            this.rbShowBridgeWithIndex.Location = new System.Drawing.Point(15, 29);
            this.rbShowBridgeWithIndex.Name = "rbShowBridgeWithIndex";
            this.rbShowBridgeWithIndex.Size = new System.Drawing.Size(134, 17);
            this.rbShowBridgeWithIndex.TabIndex = 0;
            this.rbShowBridgeWithIndex.TabStop = true;
            this.rbShowBridgeWithIndex.Text = "Show bridge with index";
            this.rbShowBridgeWithIndex.UseVisualStyleBackColor = true;
            this.rbShowBridgeWithIndex.CheckedChanged += new System.EventHandler(this.rbShowBridgeWithIndex_CheckedChanged);
            // 
            // rbShowBottomNearestBridge
            // 
            this.rbShowBottomNearestBridge.AutoSize = true;
            this.rbShowBottomNearestBridge.Location = new System.Drawing.Point(15, 76);
            this.rbShowBottomNearestBridge.Name = "rbShowBottomNearestBridge";
            this.rbShowBottomNearestBridge.Size = new System.Drawing.Size(157, 17);
            this.rbShowBottomNearestBridge.TabIndex = 1;
            this.rbShowBottomNearestBridge.TabStop = true;
            this.rbShowBottomNearestBridge.Text = "Show bottom nearest bridge";
            this.rbShowBottomNearestBridge.UseVisualStyleBackColor = true;
            this.rbShowBottomNearestBridge.CheckedChanged += new System.EventHandler(this.rbShowBottomNearestBridge_CheckedChanged);
            // 
            // txtBridgeIndex
            // 
            this.txtBridgeIndex.AcceptsReturn = true;
            this.txtBridgeIndex.Location = new System.Drawing.Point(184, 29);
            this.txtBridgeIndex.Name = "txtBridgeIndex";
            this.txtBridgeIndex.Size = new System.Drawing.Size(104, 20);
            this.txtBridgeIndex.TabIndex = 2;
            this.txtBridgeIndex.Validating += new System.ComponentModel.CancelEventHandler(this.txtBridgeIndex_Validating);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(170, 207);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(261, 207);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // VisualizationParamsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 248);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "VisualizationParamsForm";
            this.Text = "VisualizationParamsForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBridgeIndex;
        private System.Windows.Forms.RadioButton rbShowBottomNearestBridge;
        private System.Windows.Forms.RadioButton rbShowBridgeWithIndex;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}