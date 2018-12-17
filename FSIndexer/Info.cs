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
    public partial class Info : Form
    {
        public string Message
        {
            get { return rtbMessage.Text.TrimStart(); }
            set { rtbMessage.Text = value.TrimStart(); }
        }

        public Info()
        {
            InitializeComponent();
        }
    }
}
