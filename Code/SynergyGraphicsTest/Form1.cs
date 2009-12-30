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
        SynergyGraphics.TextureGPU test;
        Shader shader = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SynergyGraphics.Graphics.Initialize(Handle, new Int2(800, 600));

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
                new Float3(0.5f, 0.5f, 0),
                test);
            SynergyGraphics.Graphics.DrawRectangle(
                new Float3(-0.25f, -0.25f, 0),
                new Float3(0.75f, -0.25f, 0),
                new Float3(-0.25f, 0.75f, 0),
                new Float3(0.75f, 0.75f, 0),
                test);
            SynergyGraphics.Graphics.DrawRectangle(
                new Float3(0, 0, 0),
                new Float3(1, 0, 0),
                new Float3(0, 1, 0),
                new Float3(1, 1, 0),
                test);
            shader.End();

            SynergyGraphics.Graphics.Flush();
        }
    }
}
