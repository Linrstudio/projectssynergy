using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace DesktopClient
{
    public partial class Form1 : Form
    {
        Bitmap NotifyIcon = new Bitmap(16, 16);
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateIcon();
            notifyIcon1.ShowBalloonTip(1000, "", "test", ToolTipIcon.Info);
        }

        float iconpos = 0;
        public void UpdateIcon()
        {
            iconpos += 0.1f;
            float x = (float)Math.Cos(iconpos) * 7 + 8;
            float y = (float)Math.Sin(iconpos) * 7 + 8;

            Graphics g = Graphics.FromImage(NotifyIcon);
            g.Clear(Color.Transparent);
            g.DrawLine(Pens.Black, 0, 0, x, y);
            g.DrawLine(Pens.Black, 15, 0, x, y);
            g.DrawLine(Pens.Black, 0, 15, x, y);
            g.DrawLine(Pens.Black, 15, 15, x, y);
            try
            {
                notifyIcon1.Icon = Icon.FromHandle(NotifyIcon.GetHicon());
            }
            catch { }
        }

        private void t_UpdateIcon_Tick(object sender, EventArgs e)
        {

            t_UpdateIcon.Enabled = false;
            //UpdateIcon();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
