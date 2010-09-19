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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            MainStation.Connect();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainStation.Connect();

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[64];
            buffer[0] = 0x80;
            MainStation.Write(buffer);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[64];
            buffer[0] = 0x81;
            MainStation.Write(buffer);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[64];
            buffer[0] = 0x82;
            MainStation.Write(buffer);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MainStation.Read();
        }
    }
}
