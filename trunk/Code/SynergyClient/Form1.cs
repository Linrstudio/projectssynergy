﻿using System;
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
            Root.Size = new Float2(2, 2);
            Root.Position = new Float2(-1, -1);

            Root.OnRefresh += new Control.OnRefreshHandler(Root_OnRefresh);
            System.Windows.Forms.Application.Idle += new EventHandler(Application_Idle);
        }

        void Root_OnRefresh()
        {
            //draw a background of some sorts
            Graphics.defaultshader.Begin();

            Graphics.defaultshader.SetParameter("View", new Float2x3(2, 0, 0, 2, -1, -1));
            Graphics.defaultshader.SetParameter("DiffuseMap", ClientResources.Background);

            Graphics.DrawRectangle(new Float2(0, 0), new Float2(1, 0), new Float2(0, 1), new Float2(1, 1), 0.5f);

            Graphics.defaultshader.End();


            //Root.SetViewFit(Graphics.defaultshader);
            Root.Draw();
            SynergyGraphics.Graphics.Present(p_WorkingArea.Handle);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            //Update();
        }

        public void Tick()
        {
            Root.Resolution.X = Bounds.Width;
            Root.Resolution.Y = Bounds.Height;
            Root.Update();

            Root_OnRefresh();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            Bounds = new System.Drawing.Rectangle(0, 0, 800, 600);
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