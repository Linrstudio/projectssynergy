using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SynergyTemplate;
using SynergyGraphics;

namespace SynergyGraphicsTest
{
    public partial class Form1 : Form
    {
        List<RenderWindow> targets = new List<RenderWindow>();
        SynergyGraphics.TextureGPU test;
        Shader shader = null;

        Rect DesktopBounds = Graphics.GetTotalDesktopSize();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                RenderWindow windoh = new RenderWindow();
                windoh.screen = screen;
                targets.Add(windoh);
            }
            foreach (RenderWindow w in targets)
            {
                w.Show();
                w.Bounds = w.screen.Bounds;
            }

            SynergyGraphics.Graphics.Initialize(Handle, DesktopBounds.Size);

            shader = ShaderCompiler.Compile(System.IO.File.ReadAllText("Default.fx"));
            test = new SynergyGraphics.TextureGPU(@"c:\test.png");
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            if (m.Msg != 0x14) base.OnNotifyMessage(m);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            SynergyGraphics.Graphics.Clear(new Float4(0, 0, 0, 1));
            shader.SetParameter("DiffuseMap", test);

            Graphics.SetAlphaBlending(true);

            shader.Begin();
            SynergyGraphics.Graphics.DrawRectangle(
                new Float3(-0.5f, -0.5f, 0),
                new Float3(0.5f, -0.5f, 0),
                new Float3(-0.5f, 0.5f, 0),
                new Float3(0.5f, 0.5f, 0));
            SynergyGraphics.Graphics.DrawRectangle(
                new Float3(-0.25f, -0.25f, 0),
                new Float3(0.75f, -0.25f, 0),
                new Float3(-0.25f, 0.75f, 0),
                new Float3(0.75f, 0.75f, 0));
            SynergyGraphics.Graphics.DrawRectangle(
                new Float3(0, 0, 0),
                new Float3(1, 0, 0),
                new Float3(0, 1, 0),
                new Float3(1, 1, 0));
            shader.End();

            float totalw = DesktopBounds.Size.X;
            float totalh = DesktopBounds.Size.Y;
            foreach (RenderWindow w in targets)
            {
                Float2 from = new Float2((float)(w.Bounds.Left - DesktopBounds.From.X) / totalw, (float)(w.Bounds.Top - DesktopBounds.From.Y) / totalh);
                Float2 to = new Float2((float)(w.Bounds.Right - DesktopBounds.From.X) / totalw, (float)(w.Bounds.Bottom - DesktopBounds.From.Y) / totalh);
                SynergyGraphics.Graphics.Present(w.Handle,
                    from,
                    to,
                    new Int2(0, 0),
                    new Int2(w.Bounds.Width, w.Bounds.Height)
                    );
            }
        }
    }
}
