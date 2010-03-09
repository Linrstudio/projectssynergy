using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ClientBas
{
    public partial class Form1 : Form
    {
        [DllImport("k8055.dll")]
        private static extern int OpenDevice(int CardAddress);
        [DllImport("k8055.dll")]
        private static extern void SetDigitalChannel(int Channel);
        [DllImport("k8055.dll")]
        private static extern void ClearDigitalChannel(int Channel);

        public bool[] DigitalOut = new bool[8];

        public Form1()
        {
            InitializeComponent();
            OpenDevice(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void Toggle(int idx)
        {
            DigitalOut[idx-1] = !DigitalOut[idx-1];
            if (DigitalOut[idx-1])
                SetDigitalChannel(idx);
            else
                ClearDigitalChannel(idx);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '1':
                    Toggle(1);
                    break;
                case '2':
                    Toggle(2);
                    break;
                case '3':
                    Toggle(3);
                    break;
                case '4':
                    Toggle(4);
                    break;
                case '5':
                    Toggle(5);
                    break;
                case '6':
                    Toggle(6);
                    break;
                case '7':
                    Toggle(7);
                    break;
                case '8':
                    Toggle(8);
                    break;
            }
        }
    }
}
