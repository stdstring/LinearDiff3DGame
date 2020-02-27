namespace LinearDiff3DGame.MaxStableBridgeVisualizer
{
    partial class Form1
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
            this.Canvas = new OpenGLPaintControl.OpenGLCanvas();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.miActions = new System.Windows.Forms.ToolStripMenuItem();
            this.miCalculate = new System.Windows.Forms.ToolStripMenuItem();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblCurrentTime = new System.Windows.Forms.Label();
            this.tbTimeLine = new System.Windows.Forms.TrackBar();
            this.mainMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbTimeLine)).BeginInit();
            this.SuspendLayout();
            // 
            // Canvas
            // 
            this.Canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Canvas.Location = new System.Drawing.Point(0, 24);
            this.Canvas.Name = "Canvas";
            this.Canvas.Size = new System.Drawing.Size(792, 549);
            this.Canvas.TabIndex = 0;
            this.Canvas.Text = "openGLCanvas1";
            this.Canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.Canvas_Paint);
            this.Canvas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Canvas_KeyDown);
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miActions});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(792, 24);
            this.mainMenu.TabIndex = 1;
            this.mainMenu.Text = "menuStrip1";
            // 
            // miActions
            // 
            this.miActions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCalculate,
            this.miExit});
            this.miActions.Name = "miActions";
            this.miActions.Size = new System.Drawing.Size(54, 20);
            this.miActions.Text = "Actions";
            // 
            // miCalculate
            // 
            this.miCalculate.Name = "miCalculate";
            this.miCalculate.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.miCalculate.Size = new System.Drawing.Size(148, 22);
            this.miCalculate.Text = "Calculate";
            this.miCalculate.Click += new System.EventHandler(this.miCalculate_Click);
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(148, 22);
            this.miExit.Text = "Exit";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblCurrentTime);
            this.panel1.Controls.Add(this.tbTimeLine);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 505);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(792, 68);
            this.panel1.TabIndex = 2;
            // 
            // lblCurrentTime
            // 
            this.lblCurrentTime.Location = new System.Drawing.Point(537, 14);
            this.lblCurrentTime.Name = "lblCurrentTime";
            this.lblCurrentTime.Size = new System.Drawing.Size(230, 42);
            this.lblCurrentTime.TabIndex = 1;
            // 
            // tbTimeLine
            // 
            this.tbTimeLine.Location = new System.Drawing.Point(12, 14);
            this.tbTimeLine.Name = "tbTimeLine";
            this.tbTimeLine.Size = new System.Drawing.Size(519, 42);
            this.tbTimeLine.TabIndex = 0;
            this.tbTimeLine.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.tbTimeLine.ValueChanged += new System.EventHandler(this.tbTimeLine_ValueChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Canvas);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "Form1";
            this.Text = "Form1";
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbTimeLine)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenGLPaintControl.OpenGLCanvas Canvas;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem miActions;
        private System.Windows.Forms.ToolStripMenuItem miCalculate;
        private System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.Label lblCurrentTime;
        private System.Windows.Forms.TrackBar tbTimeLine;

    }
}

