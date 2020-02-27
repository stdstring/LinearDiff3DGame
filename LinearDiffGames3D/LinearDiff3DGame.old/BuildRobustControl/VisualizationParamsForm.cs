using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MathPostgraduateStudy.BuildRobustControl
{
    public partial class VisualizationParamsForm : Form
    {
        public VisualizationParamsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean ShowOuterBridgeWithIndex
        {
            get
            {
                return m_ShowOuterBridgeWithIndex;
            }
            set
            {
                m_ShowOuterBridgeWithIndex = value;
                rbShowBridgeWithIndex.Checked = m_ShowOuterBridgeWithIndex;
                rbShowBottomNearestBridge.Checked = !m_ShowOuterBridgeWithIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 OuterBridgeIndex
        {
            get
            {
                return m_OuterBridgeIndex;
            }
            set
            {
                m_OuterBridgeIndex = value;
                txtBridgeIndex.Text = m_OuterBridgeIndex.ToString();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void rbShowBridgeWithIndex_CheckedChanged(object sender, EventArgs e)
        {
            txtBridgeIndex.Enabled = rbShowBridgeWithIndex.Checked;
            m_ShowOuterBridgeWithIndex = rbShowBridgeWithIndex.Checked;
        }

        private void rbShowBottomNearestBridge_CheckedChanged(object sender, EventArgs e)
        {
            txtBridgeIndex.Enabled = !rbShowBottomNearestBridge.Checked;
            m_ShowOuterBridgeWithIndex = !rbShowBottomNearestBridge.Checked;
        }

        private void txtBridgeIndex_Validating(object sender, CancelEventArgs e)
        {
            Int32 bridgeIndex;
            if (Int32.TryParse(txtBridgeIndex.Text, out bridgeIndex))
            {
                m_OuterBridgeIndex = bridgeIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Boolean m_ShowOuterBridgeWithIndex;
        /// <summary>
        /// 
        /// </summary>
        private Int32 m_OuterBridgeIndex;
    }
}