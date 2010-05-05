using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

using Framework;
using SynergyGraphics;
using SynergyTemplate;

namespace SynergyClient
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        Control Root = null;
        Int2 TargetResolution = new Int2(800, 600);
        public Form1()
        {
            InitializeComponent();
            SynergyGraphics.Graphics.Initialize(this.Handle, TargetResolution);
            ClientResources.Load();
            Root = new GenericBrowser(null);
            Root.Transformation = Float3x3.Scale(new Float2(2, -2)) * Float3x3.Translate(new Float2(-0.5f, -0.5f));

            System.Windows.Forms.Application.Idle += new EventHandler(Application_Idle);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            Tick();
        }

        public void Tick()
        {
            //Root.Transformation.X.X = Bounds.Width;
            //Root.Transformation.Y.Y = Bounds.Height;
            Root.Update();
            Root.Draw();

            SynergyGraphics.Graphics.Present(p_WorkingArea.Handle);
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            Bounds = new System.Drawing.Rectangle(0, 0, 800, 600);
            new FrontEnd().Show();
        }

        private void Form1_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void t_tick_Tick(object sender, EventArgs e)
        {
            Tick();
        }

        private void Form1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            UIController.CursorPos.X = e.X / TargetResolution.X;
            UIController.CursorPos.X = e.Y / TargetResolution.Y;
        }
    }
}
