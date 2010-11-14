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
        Random random = new Random(Environment.TickCount);
        Control Root = null;
        Int2 TargetResolution = new Int2(1024, 768);
        TextureGPU ScreenSaverBuffer = null;
        RenderTarget ScreenSaverWobbleTarget = null;
        RenderTarget ScreenSaverTarget = null;
        Shader ScreenSaverShader = null;
        int ScreenSaverStartTimer = 0;
        int ScreenSaverStopTimer = 0;
        bool ScreenSaverCustomDroplet = false;
        Float2 ScreenSaverDropletPos = new Float2(0, 0);

        int ScreenSaverDropTimeout = 0;
        TextureGPUResource ScreenSaverDroplet = null;
        bool ScreenSaverEnabled = true;

        public Form1()
        {
            InitializeComponent();

            Graphics.Initialize(this.Handle, TargetResolution);
            ClientResources.Load();
            ScreenSaverTarget = new RenderTarget(TargetResolution);
            ScreenSaverWobbleTarget = new RenderTarget(TargetResolution / 8, RenderTarget.SurfaceFormat.Float4Half);
            ScreenSaverShader = ShaderCompiler.Compile(System.IO.File.ReadAllText("screensaver.fx"));
            ScreenSaverBuffer = new TextureGPU(TargetResolution);

            ScreenSaverDroplet = new TextureGPUResource("./content/droplet.png");


            //Root = new GenericBrowser(null);
#if true
            Root = new UIBackground("", null);

            UIFloor floor4 = new UIFloor("floor4", Root); floor4.level = 3;
            UIFloor floor3 = new UIFloor("floor3", Root); floor3.level = 2;
            UIFloor floor2 = new UIFloor("floor2", Root); floor2.level = 1;
            UIFloor floor1 = new UIFloor("floor1", Root); floor1.level = 0;


            Control button1 = new UIButton("b1", floor4);
            button1.Transformation = Float3x3.Scale(0.1f) * Float3x3.Translate(new Float2(1, 1));

            Control button2 = new UIAnalog("b2", floor4);
            button2.Transformation = Float3x3.Scale(0.1f) * Float3x3.Translate(new Float2(0, 0));
#else
            Control button1 = new UIButton("b1", null);
            button1.Transformation = Float3x3.Translate(new Float2(0.5f, 0.5f)) * Float3x3.Scale(0.1f);
            Control button2 = new UIAnalog("b2", null);
            button2.Transformation = Float3x3.Translate(new Float2(0.5f, -0.5f)) * Float3x3.Scale(0.1f);
            Control button3 = new UIAnalog("b3", null);
            button3.Transformation = Float3x3.Translate(new Float2(-0.5f, 0.5f)) * Float3x3.Scale(0.1f);
            Control button4 = new UIAnalog("b4", null);
            button4.Transformation = Float3x3.Translate(new Float2(-0.5f, -0.5f)) * Float3x3.Scale(0.1f);

            ((UIButton)button1).Text = "button1";
            ((UIAnalog)button2).Text = "button2";
            ((UIAnalog)button3).Text = "button3";
            ((UIAnalog)button4).Text = "button4";

            Root = new UIBackground("", null);

            Root.AddChild(button1);
            Root.AddChild(button2);
            Root.AddChild(button3);
            Root.AddChild(button4);
#endif
            //Root.Transformation = Float3x3.Scale(new Float2(2, -2)) * Float3x3.Translate(new Float2(-0.5f, -0.5f));
            System.Windows.Forms.Application.Idle += new EventHandler(Application_Idle);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            Tick();
        }

        int presstime = 0;
        bool lastpress = false;
        bool frame1 = true;
        Float2 lastpos;
        public void Tick()
        {
            System.Drawing.Point mouse = PointToClient(MousePosition);
            Float2 cursorpos = new Float2(
                (float)mouse.X / (float)ClientRectangle.Width,
                (float)mouse.Y / (float)ClientRectangle.Height);

            cursorpos = cursorpos * 2 - new Float2(1, 1);
            cursorpos.Y = -cursorpos.Y;
            bool press = MouseButtons == System.Windows.Forms.MouseButtons.Left;
            if (press && !lastpress) presstime = Environment.TickCount;
            if ((press && lastpos != cursorpos) || lastpress != press)
                Root.HandleControllerInput(new UITouchEvent(cursorpos, press != lastpress && press == true, press != lastpress && press == false));
            if (press) ScreenSaverStartTimer = Environment.TickCount + 60000;

            if (!press && lastpress && ScreenSaverEnabled && Environment.TickCount - presstime < 250)
                ScreenSaverStopTimer = Environment.TickCount + 10000;
            if (press)
            {
                ScreenSaverDropletPos = cursorpos;
                ScreenSaverCustomDroplet = true;
            }
            lastpress = press;



            Root.Update();
            //optionally apply screensaver
            if (ScreenSaverEnabled)
                Graphics.SetRenderTarget(ScreenSaverTarget);

            Root.Draw();
            if (Environment.TickCount > ScreenSaverStartTimer && ScreenSaverStartTimer != 0) { ScreenSaverEnabled = true; ScreenSaverStartTimer = 0; ScreenSaverStopTimer = 0; frame1 = true; }
            if (ScreenSaverEnabled)
            {
                if (Environment.TickCount > ScreenSaverStopTimer && ScreenSaverStopTimer != 0) { ScreenSaverEnabled = false; ScreenSaverStopTimer = 0; }
                Graphics.SetRenderTarget(null);
                TextureGPU screenbuffer = ScreenSaverTarget.Resolve();
                Graphics.SetBlendMode(Graphics.BlendMode.None);
                Graphics.SetRenderTarget(ScreenSaverWobbleTarget);
                ScreenSaverShader.SetParameter("RenderSize", new Float2(TargetResolution.X, TargetResolution.Y));
                ScreenSaverShader.SetTechnique("Wobbler");
                ScreenSaverShader.Begin();
                Graphics.DrawRectangle(
                    new Float2(-1, 1),
                    new Float2(1, 1),
                    new Float2(-1, -1),
                    new Float2(1, -1), 0.5f);
                ScreenSaverShader.End();
                if (frame1) Graphics.Clear(new Float4(0, 0, 0, 0));
                if (ScreenSaverStopTimer == 0 && ScreenSaverDroplet.Get() != null)
                {
                    if (ScreenSaverCustomDroplet)
                    {
                        Graphics.SetBlendMode(Graphics.BlendMode.Alpha);
                        Graphics.defaultshader.SetParameter("View", new Float3x3(0.1f, 0, 0, 0, 0.1f, 0, ScreenSaverDropletPos.X, ScreenSaverDropletPos.Y, 1));
                        Graphics.defaultshader.SetParameter("DiffuseMap", (TextureGPU)ScreenSaverDroplet.Get());
                        Graphics.defaultshader.Begin();
                        Graphics.DrawRectangle(
                            new Float2(-1, 1),
                            new Float2(1, 1),
                            new Float2(-1, -1),
                            new Float2(1, -1), 0.5f);
                        Graphics.defaultshader.End();
                        Graphics.SetBlendMode(Graphics.BlendMode.None);
                        ScreenSaverCustomDroplet = false;
                    }
                    if (Environment.TickCount > ScreenSaverDropTimeout)
                    {
                        ScreenSaverDropTimeout = Environment.TickCount + random.Next(2000);
                        Graphics.SetBlendMode(Graphics.BlendMode.Add);
                        Graphics.defaultshader.SetParameter("View", new Float3x3(0.05f, 0, 0, 0, 0.05f, 0, (float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1, 1));
                        Graphics.defaultshader.SetParameter("DiffuseMap", (TextureGPU)ScreenSaverDroplet.Get());
                        Graphics.defaultshader.Begin();
                        Graphics.DrawRectangle(
                            new Float2(-1, 1),
                            new Float2(1, 1),
                            new Float2(-1, -1),
                            new Float2(1, -1), 0.5f);
                        Graphics.defaultshader.End();
                        Graphics.SetBlendMode(Graphics.BlendMode.None);
                    }
                }
                Graphics.SetRenderTarget(null);
                ScreenSaverBuffer = ScreenSaverWobbleTarget.Resolve();
                ScreenSaverShader.SetParameter("WobblerMap", ScreenSaverBuffer);
                ScreenSaverShader.SetParameter("DiffuseMap", screenbuffer);
                ScreenSaverShader.SetTechnique("Main");
                ScreenSaverShader.Begin();
                Graphics.DrawRectangle(
                    new Float2(-1, 1),
                    new Float2(1, 1),
                    new Float2(-1, -1),
                    new Float2(1, -1), 0.5f);
                ScreenSaverShader.End();
                Graphics.SetBlendMode(Graphics.BlendMode.Alpha);
            }
            Synergy.Graphics.Present(p_WorkingArea.Handle);
            ResourceManager.Update();
            frame1 = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Bounds = new System.Drawing.Rectangle(0, 0, TargetResolution.X, TargetResolution.Y);
            //new FrontEnd().Show();
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
