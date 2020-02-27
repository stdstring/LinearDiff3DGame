namespace MathPostgraduateStudy.BuildRobustControl
{
    partial class RobustControlViewerForm
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
            this.lblTime = new System.Windows.Forms.Label();
            this.timeBar = new System.Windows.Forms.TrackBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.miActions = new System.Windows.Forms.ToolStripMenuItem();
            this.miCalcRobustControl = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpenBridgeSystemDataFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveBridgeSystemDataFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.miSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.miColorOfPolyhedrons = new System.Windows.Forms.ToolStripMenuItem();
            this.miVisualizationParams = new System.Windows.Forms.ToolStripMenuItem();
            this.openDataFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveDataFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.PaintBox = new MathPostgraduateStudy.BuildRobustControl.OpenGLPaintBox();
            this.miSaveRobustControlData = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeBar)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.lblTime);
            this.panel1.Controls.Add(this.timeBar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 545);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(794, 85);
            this.panel1.TabIndex = 0;
            // 
            // lblTime
            // 
            this.lblTime.Location = new System.Drawing.Point(630, 21);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(133, 23);
            this.lblTime.TabIndex = 1;
            // 
            // timeBar
            // 
            this.timeBar.Location = new System.Drawing.Point(12, 21);
            this.timeBar.Name = "timeBar";
            this.timeBar.Size = new System.Drawing.Size(583, 42);
            this.timeBar.TabIndex = 0;
            this.timeBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.timeBar.ValueChanged += new System.EventHandler(this.timeBar_ValueChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miActions,
            this.miSettings});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(794, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // miActions
            // 
            this.miActions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCalcRobustControl,
            this.miSaveRobustControlData,
            this.miOpenBridgeSystemDataFile,
            this.miSaveBridgeSystemDataFile,
            this.miExit});
            this.miActions.Name = "miActions";
            this.miActions.Size = new System.Drawing.Size(54, 20);
            this.miActions.Text = "Actions";
            // 
            // miCalcRobustControl
            // 
            this.miCalcRobustControl.Name = "miCalcRobustControl";
            this.miCalcRobustControl.Size = new System.Drawing.Size(253, 22);
            this.miCalcRobustControl.Text = "Calc. robust control";
            this.miCalcRobustControl.Click += new System.EventHandler(this.miCalcRobustControl_Click);
            // 
            // miOpenBridgeSystemDataFile
            // 
            this.miOpenBridgeSystemDataFile.Name = "miOpenBridgeSystemDataFile";
            this.miOpenBridgeSystemDataFile.Size = new System.Drawing.Size(253, 22);
            this.miOpenBridgeSystemDataFile.Text = "Open data\'s file with bridge system";
            this.miOpenBridgeSystemDataFile.Click += new System.EventHandler(this.miOpenBridgeSystemDataFile_Click);
            // 
            // miSaveBridgeSystemDataFile
            // 
            this.miSaveBridgeSystemDataFile.Name = "miSaveBridgeSystemDataFile";
            this.miSaveBridgeSystemDataFile.Size = new System.Drawing.Size(253, 22);
            this.miSaveBridgeSystemDataFile.Text = "Save data\'s file with bridge system";
            this.miSaveBridgeSystemDataFile.Click += new System.EventHandler(this.miSaveBridgeSystemDataFile_Click);
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(253, 22);
            this.miExit.Text = "Exit";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // miSettings
            // 
            this.miSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miColorOfPolyhedrons,
            this.miVisualizationParams});
            this.miSettings.Name = "miSettings";
            this.miSettings.Size = new System.Drawing.Size(58, 20);
            this.miSettings.Text = "Settings";
            // 
            // miColorOfPolyhedrons
            // 
            this.miColorOfPolyhedrons.Name = "miColorOfPolyhedrons";
            this.miColorOfPolyhedrons.Size = new System.Drawing.Size(185, 22);
            this.miColorOfPolyhedrons.Text = "Color of polyhedrons";
            this.miColorOfPolyhedrons.Click += new System.EventHandler(this.miColorOfPolyhedrons_Click);
            // 
            // miVisualizationParams
            // 
            this.miVisualizationParams.Name = "miVisualizationParams";
            this.miVisualizationParams.Size = new System.Drawing.Size(185, 22);
            this.miVisualizationParams.Text = "Visualization params";
            this.miVisualizationParams.Click += new System.EventHandler(this.miVisualizationParams_Click);
            // 
            // openDataFileDialog
            // 
            this.openDataFileDialog.DefaultExt = "dat";
            // 
            // saveDataFileDialog
            // 
            this.saveDataFileDialog.DefaultExt = "dat";
            // 
            // PaintBox
            // 
            this.PaintBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PaintBox.Location = new System.Drawing.Point(0, 24);
            this.PaintBox.Name = "PaintBox";
            this.PaintBox.Size = new System.Drawing.Size(794, 521);
            this.PaintBox.TabIndex = 1;
            this.PaintBox.Paint += new System.Windows.Forms.PaintEventHandler(this.PaintBox_Paint);
            this.PaintBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PaintBox_KeyDown);
            // 
            // miSaveRobustControlData
            // 
            this.miSaveRobustControlData.Name = "miSaveRobustControlData";
            this.miSaveRobustControlData.Size = new System.Drawing.Size(253, 22);
            this.miSaveRobustControlData.Text = "Save robust control data";
            this.miSaveRobustControlData.Click += new System.EventHandler(this.miSaveRobustControlData_Click);
            // 
            // RobustControlViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 630);
            this.Controls.Add(this.PaintBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RobustControlViewerForm";
            this.Text = "RobustControlViewer";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeBar)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TrackBar timeBar;
        private OpenGLPaintBox PaintBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem miActions;
        private System.Windows.Forms.ToolStripMenuItem miCalcRobustControl;
        private System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.ToolStripMenuItem miSettings;
        private System.Windows.Forms.ToolStripMenuItem miColorOfPolyhedrons;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.ToolStripMenuItem miVisualizationParams;
        private System.Windows.Forms.OpenFileDialog openDataFileDialog;
        private System.Windows.Forms.SaveFileDialog saveDataFileDialog;
        private System.Windows.Forms.ToolStripMenuItem miOpenBridgeSystemDataFile;
        private System.Windows.Forms.ToolStripMenuItem miSaveBridgeSystemDataFile;
        private System.Windows.Forms.ToolStripMenuItem miSaveRobustControlData;
    }
}

