namespace LinearDiff3DGame.MaxStableBridgeVisualizer.old
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveDatatoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveData2toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.nextIterationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChangeSightParamstoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblInfo = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.actionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(292, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveDatatoolStripMenuItem,
            this.SaveData2toolStripMenuItem1,
            this.nextIterationToolStripMenuItem,
            this.ChangeSightParamstoolStripMenuItem,
            this.exitToolStripMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.actionsToolStripMenuItem.Text = "Actions";
            // 
            // SaveDatatoolStripMenuItem
            // 
            this.SaveDatatoolStripMenuItem.Name = "SaveDatatoolStripMenuItem";
            this.SaveDatatoolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.SaveDatatoolStripMenuItem.Text = "Save polyhedron\'s data";
            this.SaveDatatoolStripMenuItem.Click += new System.EventHandler(this.SaveDatatoolStripMenuItem_Click);
            // 
            // SaveData2toolStripMenuItem1
            // 
            this.SaveData2toolStripMenuItem1.Name = "SaveData2toolStripMenuItem1";
            this.SaveData2toolStripMenuItem1.Size = new System.Drawing.Size(207, 22);
            this.SaveData2toolStripMenuItem1.Text = "Save polyhedron\'s data 2";
            this.SaveData2toolStripMenuItem1.Click += new System.EventHandler(this.SaveData2toolStripMenuItem1_Click);
            // 
            // nextIterationToolStripMenuItem
            // 
            this.nextIterationToolStripMenuItem.Name = "nextIterationToolStripMenuItem";
            this.nextIterationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.nextIterationToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.nextIterationToolStripMenuItem.Text = "Next iteration";
            this.nextIterationToolStripMenuItem.Click += new System.EventHandler(this.nextIterationToolStripMenuItem_Click);
            // 
            // ChangeSightParamstoolStripMenuItem
            // 
            this.ChangeSightParamstoolStripMenuItem.Name = "ChangeSightParamstoolStripMenuItem";
            this.ChangeSightParamstoolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.ChangeSightParamstoolStripMenuItem.Text = "Change sight params";
            this.ChangeSightParamstoolStripMenuItem.Click += new System.EventHandler(this.ChangeSightParamstoolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(28, 54);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(0, 13);
            this.lblInfo.TabIndex = 1;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Data\'s files|*.dat";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextIterationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ChangeSightParamstoolStripMenuItem;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.ToolStripMenuItem SaveDatatoolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem SaveData2toolStripMenuItem1;
    }
}

