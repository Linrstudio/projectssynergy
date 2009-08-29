using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SynergyClient
{
    public partial class GetDeviceName : Form
    {
        public string InsertedText = "";
        public GetDeviceName(string _DefaultText)
        {
            InitializeComponent();
            textBox1.Text = _DefaultText;
        }

        private void GetDeviceName_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            InsertedText = textBox1.Text;
            if (InsertedText.Contains("\n"))
            {
                InsertedText=InsertedText.Replace("\n","");
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
