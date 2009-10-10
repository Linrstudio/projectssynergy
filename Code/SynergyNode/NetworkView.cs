using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SynergyNode
{
    public partial class NetworkView : Control
    {
        public List<NetworkObject> Objects = new List<NetworkObject>();
        public float zoom = 1;
        public NetworkView()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            OnResize();
            NetworkNode.OnDeviceFound += OnDeviceFound;
            NetworkNode.OnDeviceMemoryChanged += OnDeviceMemoryChanged;
        }

        public void OnResize()
        {
            zoom = Math.Max(Math.Min(Width, Height), float.Epsilon);
        }

        private void NetworkView_Paint(object sender, PaintEventArgs e)
        {
            OnResize();
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            float camx = 0;
            float camy = 0;
            foreach (NetworkObject o in Objects)
            {
                o.Parent = this;
                o.Draw(e.Graphics, camx, camy, zoom);
            }
        }

        public void Redraw()
        {
            Refresh();
        }

        public void OnDeviceFound(Device _Device)
        {
            Redraw();
        }

        public void OnDeviceMemoryChanged(Device _Device)
        {
            Redraw();
        }

        private void NetworkView_MouseMove(object sender, MouseEventArgs e)
        {
            OnResize();
            float x = e.X / zoom;
            float y = e.Y / zoom;

            foreach (NetworkObject o in Objects)
            {
                o.OnMouseChange(x - o.X, y - o.Y, e.Button == MouseButtons.Left, false, false);
            }
        }

        protected override  void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message ( reduce flickering )
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }

        private void NetworkView_MouseDown(object sender, MouseEventArgs e)
        {
            OnResize();
            float x = e.X / zoom;
            float y = e.Y / zoom;

            foreach (NetworkObject o in Objects)
            {
                o.OnMouseChange(x - o.X, y - o.Y, e.Button == MouseButtons.Left, e.Button == MouseButtons.Left, false);
            }
        }

        private void NetworkView_MouseUp(object sender, MouseEventArgs e)
        {
            OnResize();
            float x = e.X / zoom;
            float y = e.Y / zoom;

            foreach (NetworkObject o in Objects)
            {
                o.OnMouseChange(x - o.X, y - o.Y, e.Button == MouseButtons.Left, false, e.Button == MouseButtons.Left);
            }
        }
    }
}
