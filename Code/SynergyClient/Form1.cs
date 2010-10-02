using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

using Synergy;


namespace SynergyClient
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        Control Root = null;
        Int2 TargetResolution = new Int2(800, 600);
        bool MouseUpdated = false;
        public Form1()
        {
            InitializeComponent();

            Synergy.Graphics.Initialize(this.Handle, TargetResolution);
            ClientResources.Load();
            //Root = new GenericBrowser(null);
            Root = new UIButton("", null);
            ((UIButton)Root).Text = "my awesome button";
            //Root.Transformation = Float3x3.Scale(new Float2(2, -2)) * Float3x3.Translate(new Float2(-0.5f, -0.5f));
            System.Windows.Forms.Application.Idle += new EventHandler(Application_Idle);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            Tick();
        }

        public void Tick()
        {

            System.Drawing.Point mouse = PointToClient(MousePosition);
            UIController.Default.CursorPos.X = (float)mouse.X / (float)ClientRectangle.Width;
            UIController.Default.CursorPos.Y = (float)mouse.Y / (float)ClientRectangle.Height;
            UIController.Default.CursorPos = UIController.Default.CursorPos * 2 - new Float2(1, 1);
            bool yes = MouseButtons == System.Windows.Forms.MouseButtons.Left;
            bool no = MouseButtons == System.Windows.Forms.MouseButtons.Right;
            UIController.Default.YesDown = (yes && !UIController.Default.Yes);
            UIController.Default.NoDown = (no && !UIController.Default.No);
            UIController.Default.YesUp = (!yes && UIController.Default.Yes);
            UIController.Default.NoUp = (!no && UIController.Default.No);
            UIController.Default.Yes = yes;
            UIController.Default.No = no;
            if (!MouseUpdated) Root.HandleControllerInput(UIController.Default);
            MouseUpdated = true;

            Root.Update();
            Root.Draw();
            Synergy.Graphics.Present(p_WorkingArea.Handle);
            MouseUpdated = false;
            ResourceManager.Update();
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
    }
}
