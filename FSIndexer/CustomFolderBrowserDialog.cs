using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FSIndexer
{
    public partial class CustomFolderBrowserDialog : Form
    {
        private string InitialPath { get; set; }
        public string SelectedPath { get; set; }

        public CustomFolderBrowserDialog(string initialPath)
        {
            InitializeComponent();
            InitialPath = initialPath.Trim();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void CustomFolderBrowserDialog_Load(object sender, EventArgs e)
        {
            tbPath.Text = InitialPath;
            SelectedPath = InitialPath;
            this.ActiveControl = btnOK;
        }

        private void tbPath_TextChanged(object sender, EventArgs e)
        {
            SelectedPath = tbPath.Text.Trim();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog brow = new FolderBrowserDialog();
            brow.SelectedPath = tbPath.Text;

            if (brow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbPath.Text = brow.SelectedPath;
                SelectedPath = brow.SelectedPath;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedPath))
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }

            this.Close();
        }
    }
}
