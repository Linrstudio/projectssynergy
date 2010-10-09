using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MainStationFrontEnd
{
    public partial class RenameDialog : Form
    {
        string oldname;
        public string NewName;
        public RenameDialog(string _OldName)
        {
            oldname = _OldName;
            InitializeComponent();
            textBox1.Text = oldname;
        }

        private void f_Rename_Load(object sender, EventArgs e)
        {

        }

        private void b_Rename_Click(object sender, EventArgs e)
        {
            NewName = textBox1.Text;
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }
    }
}
