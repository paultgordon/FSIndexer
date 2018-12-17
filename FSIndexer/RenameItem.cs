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
    public partial class RenameItem : Form
    {
        public bool BlankAllowed { get; set; }

        public string TextName
        {
            get { return tbName.Text; }
            set { tbName.Text = value; }
        }

        public bool TextNameEnabled
        {
            get { return tbName.Enabled; }
            set { tbName.Enabled = value; }
        }

        public string ButtonText
        {
            get { return btnSave.Text; }
            set { btnSave.Text = string.IsNullOrEmpty(value) ? btnSave.Text : value; }
        }

        public bool Remember
        {
            get { return cbRemember.Enabled && cbRemember.Checked; }
        }

        public bool RememberDefaultValue
        {
            set { cbRemember.Checked = value; }
        }

        public bool RememberEnabled
        {
            get { return cbRemember.Enabled; }
            set 
            { 
                cbRemember.Enabled = value;
                cbRemember.Visible = value;
            }
        }

        public RenameItem(bool blankAllowed = false)
        {
            InitializeComponent();
            BlankAllowed = blankAllowed;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void RenameItem_Load(object sender, EventArgs e)
        {
            btnSave.Enabled = BlankAllowed || tbName.Text.Length > 0;
            tbName.Focus();
            tbName.SelectAll();
        }

        private void tbName_TextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = BlankAllowed || tbName.Text.Length > 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void tbName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && btnSave.Enabled)
            {
                btnSave_Click(null, null);
            }
        }
    }
}
