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
        public float zoom = 100;
        public Matrix projection = new Matrix();
        public Matrix view = new Matrix();

        public PointF MousePos;

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
            //zoom = Math.Max((Width + Height) * 0.5f, float.Epsilon);
            projection = new Matrix();
            projection.Scale(zoom, zoom);
            projection.Translate((Width - zoom) / (zoom * 2), (Height - zoom) / (zoom * 2));
        }

        private void NetworkView_Paint(object sender, PaintEventArgs e)
        {
            OnResize();
            Matrix viewproj = view.Clone(); viewproj.Multiply(projection);
            e.Graphics.Transform = viewproj;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            foreach (NetworkObject o in Objects.ToArray())
            {
                o.Parent = this;
                o.Draw(e.Graphics);
            }
        }

        public void Redraw()
        {
            //Refresh();
        }

        public void OnDeviceFound(Device _Device)
        {
            Redraw();
        }

        public void OnDeviceMemoryChanged(Device _Device)
        {
            Redraw();
        }

        private void UpdateMouse(float _windowX,float _windowY)
        {
            PointF[] points = new PointF[] { new PointF(_windowX, _windowY) };
            Matrix m = projection.Clone();
            m.Invert();
            m.TransformPoints(points);
            MousePos = points[0];
        }

        private void NetworkView_MouseMove(object sender, MouseEventArgs e)
        {
            OnResize();
            UpdateMouse(e.X, e.Y);
            foreach (NetworkObject o in Objects)
            {
                o.OnMouseChange(MousePos.X - o.X, MousePos.Y - o.Y, e.Button == MouseButtons.Left, false, false);
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
            UpdateMouse(e.X, e.Y);

            foreach (NetworkObject o in Objects.ToArray())
            {
                o.OnMouseChange(MousePos.X - o.X, MousePos.Y - o.Y, e.Button == MouseButtons.Left, e.Button == MouseButtons.Left, false);
            }
        }

        private void NetworkView_MouseUp(object sender, MouseEventArgs e)
        {
            OnResize();
            UpdateMouse(e.X, e.Y);

            foreach (NetworkObject o in Objects.ToArray())
            {
                o.OnMouseChange(MousePos.X - o.X, MousePos.Y - o.Y, e.Button == MouseButtons.Left, false, e.Button == MouseButtons.Left);
            }
        }
    }
}
