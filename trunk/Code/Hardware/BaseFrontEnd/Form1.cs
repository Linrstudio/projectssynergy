using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BaseFrontEnd
{
    public partial class Form1 : Form
    {
        Base bas;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //bas = new Base();
            bas = new Base("192.168.1.74", 1001);


            byte[] b = BitConverter.GetBytes((ushort)512);
        }
    }
}
