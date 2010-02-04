using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;

namespace BaseFrontEnd
{
    public partial class f_Connect : Form
    {
        Base b = null;
        public f_Connect()
        {
            InitializeComponent();
        }

        private void f_Connect_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            SerialPort p = null;
            string[] ports = SerialPort.GetPortNames();
            foreach (string s in ports)
            {
                try
                {
                    p = new SerialPort(s, 1200, Parity.None, 8, StopBits.One);
                    p.Open();
                    p.Write(new byte[] { (byte)'h' }, 0, 1);
                    for (int i = 0; i < 10; i++)
                    {
                        if (p.BytesToRead > 0) if (p.ReadByte() == (byte)'y')
                            {
                                p.ReadByte(); p.ReadByte();
                                break;
                            }
                        System.Threading.Thread.Sleep(100);//wait one second
                    }
                    p.Close();
                    p = null;
                }
                catch { p = null; }
            }
            if (p != null)
            {
                b = new Base(p.PortName);
            }
        }
    }
}
