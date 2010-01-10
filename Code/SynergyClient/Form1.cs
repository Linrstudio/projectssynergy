using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SynergyGraphics;
using SynergyTemplate;

namespace SynergyClient
{
    public partial class Form1 : Form
    {
        Int2 TargetResolution = new Int2(800, 600);
        public Form1()
        {
            InitializeComponent();
            SynergyGraphics.Graphics.Initialize(this.Handle, TargetResolution);
            Application.Idle += new EventHandler(Application_Idle);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            SynergyGraphics.Graphics.Present(p_WorkingArea.Handle);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
