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

        private void timer1_Tick(object sender, EventArgs e)
        {
            Text = MainStation.Connected() ? "Connected" : "Not Connected";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainStation.Poll((ushort)numericUpDown1.Value);
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            MainStation.InvokeEvent((ushort)numericUpDown1.Value,(byte)n_event.Value,(ushort)n_arguments.Value);
            //MainStation.SendRaw((ushort)numericUpDown1.Value, new byte[] { 1, 1 });
        }

        private void b_test_Click(object sender, EventArgs e)
        {
            MainStation.SendRaw((ushort)numericUpDown1.Value, new byte[] {2 });
        }

        private void n_event_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            MainStation.InvokeEvent(1234, 1, 0);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MainStation.InvokeEvent(1234, 1, 1);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MainStation.InvokeEvent(1234, 1, 2);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            MainStation.InvokeEvent(1234, 2, 0);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            MainStation.InvokeEvent(1234, 2, 1);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            MainStation.InvokeEvent(1234, 2, 2);
        }
    }
}
