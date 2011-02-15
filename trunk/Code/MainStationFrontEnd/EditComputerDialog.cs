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
    public partial class EditComputerDialog : Form
    {
        Computer computer;
        public EditComputerDialog(Computer _Computer)
        {
            computer = _Computer;
            InitializeComponent();
            textBox1.Text = computer.Name;
            try
            {
                textBox2.Text = computer.IPAddress.ToString() + ":" + computer.Port.ToString();
            }
            catch { }
        }

        private void EditComputerDialog_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                computer.Name = textBox1.Text;
                string[] split = textBox2.Text.Split(':');
                computer.IPAddress = System.Net.IPAddress.Parse(split[0]);
                computer.Port = ushort.Parse(split[1]);
            }
            catch { return; }
            Close();
        }
    }
}
