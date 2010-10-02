using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Synergy;
using Synergy;

namespace SynergyTest
{
    public partial class Form1 : Form
    {
        List<RenderWindow> targets = new List<RenderWindow>();
        Synergy.TextureGPU test;
        Shader shader = null;

        Rect DesktopBounds = Graphics.GetTotalDesktopSize();

        Float2 Pos;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Synergy.Graphics.Initialize(Handle, new Int2(Width, Height));

            shader = ShaderCompiler.Compile(System.IO.File.ReadAllText("Default.fx"));
            test = new Synergy.TextureGPU(@"c:\test.png");
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            if (m.Msg != 0x14) base.OnNotifyMessage(m);
        }
        float dir = 0;
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            dir += 0.02f;
            Pos.X = ((float)MousePosition.X - (float)DesktopBounds.From.X) / (float)DesktopBounds.Size.X;
            Pos.Y = ((float)MousePosition.Y - (float)DesktopBounds.From.Y) / (float)DesktopBounds.Size.Y;
            Graphics.Clear(new Float4(1, 0, 1, 1));
            //Synergy.Graphics.ClearZBuffer(1);

            Float3x3 mat = Float3x3.Rotate(dir) * Float3x3.Translate(new Float2(-0.5f, -0.5f));


            shader.SetParameter("DiffuseMap", test);
            shader.SetParameter("View", mat);
            Graphics.SetAlphaBlending(true);
            //Graphics.SetDepthCheck(false);

            shader.Begin();
            Graphics.DrawRectangle(
                new Float2(0, 0),
                new Float2(1, 0),
                new Float2(0, 1),
                new Float2(1, 1), 0.5f);
            shader.End();

            Graphics.Present(Handle);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
