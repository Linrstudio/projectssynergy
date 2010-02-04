using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BaseFrontEnd
{
    public partial class f_AddEvent : Form
    {
        public ushort DeviceID;
        public byte EventID;
        public f_AddEvent()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DeviceID = (ushort)numericUpDown1.Value;
            EventID = (byte)numericUpDown2.Value;
            Close();
        }
    }
}
