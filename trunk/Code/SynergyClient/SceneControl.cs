using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using SynergyNode;

namespace SynergyClient
{
    public class SceneControl : Control
    {
        SceneDevice CurrentDevice = null;
        int DeviceOperation = 0;

        public Scene scene;
        public float SceneSize = 0;

        ContextMenu SceneMenu;
        MenuItem SceneMenuAdd;
        ContextMenu DeviceMenu;
        protected override void OnCreateControl()
        {
            SceneMenuAdd = new MenuItem("Add");
            ConnectionManager.OnDeviceFound += OnDeviceFound;
            SceneMenu = new ContextMenu(new MenuItem[] { SceneMenuAdd });
            DeviceMenu = new ContextMenu(new MenuItem[] { new MenuItem("Move"), new MenuItem("Resize"), new MenuItem("Rename"), new MenuItem("Delete") });
            DeviceMenu.MenuItems[0].Click += OnMenuMoveClick;
            DeviceMenu.MenuItems[1].Click += OnMenuResizeClick;
            DeviceMenu.MenuItems[2].Click += OnMenuRenameClick;
            DeviceMenu.MenuItems[3].Click += OnMenuDeleteClick;
            SetStyle(ControlStyles.OptimizedDoubleBuffer,true);
            Resize();
            base.OnCreateControl();
        }

        public void OnDeviceFound(Device _D)
        {
            SceneMenuAdd.MenuItems.Clear();
            foreach (Device d in ConnectionManager.Devices.Values)
            {
                MenuItem item = new MenuItem(d.ID.ToString());
                SceneMenuAdd.MenuItems.Add(item);
                item.Click += OnDeviceAdd;
            }
        }

        public void Resize()
        {
            SceneSize = Math.Min(Width, Height);
        }

        protected override void OnResize(EventArgs e)
        {
            Resize();
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0 ) return;
            Graphics backbuffer = e.Graphics;
            
            backbuffer.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (scene != null)
            {
                backbuffer.DrawImage(scene.BackgroundImage, new RectangleF(0, 0, Width, Height));
                foreach (SceneDevice d in scene.Devices)
                {
                    d.OnDraw(backbuffer, SceneSize);
                }
            }
        
            base.OnPaint(e);
        }

        public void ReDraw()
        {
            Refresh();
        }

        private void OnDeviceAdd(object sender, EventArgs e)
        {
            var s = (MenuItem)sender;
            ushort ID = ushort.Parse(s.Text);
            Device D = ConnectionManager.Devices[ID];
            SceneDevice d = SceneDevice.GetDevice(D);
            if (d != null)
            {
                scene.Devices.Add(d);
                ReDraw();
            }
            else { MessageBox.Show("We do not yet have support for this Device type.\nTry to find an update of this software"); }
        }

        private void OnMenuMoveClick(object sender, EventArgs e)
        {
            DeviceOperation = 1;
        }

        private void OnMenuResizeClick(object sender, EventArgs e)
        {
            DeviceOperation = 2;
        }

        private void OnMenuRenameClick(object sender, EventArgs e)
        {
            GetDeviceName d = new GetDeviceName(CurrentDevice.Name);
            d.ShowDialog();
            CurrentDevice.Name = d.InsertedText;
            ReDraw();
        }

        private void OnMenuDeleteClick(object sender, EventArgs e)
        {
            scene.Devices.Remove(CurrentDevice);
            ReDraw();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {

            float X = (float)e.X / SceneSize;
            float Y = (float)e.Y / SceneSize;
            switch (DeviceOperation)
            {
                case 1:
                    CurrentDevice.X = X;
                    CurrentDevice.Y = Y;
                    ReDraw();
                    break;
                case 2:
                    {
                        float dx = CurrentDevice.X - X;
                        float dy = CurrentDevice.Y - Y;
                        CurrentDevice.Size = (float)Math.Sqrt(dx * dx + dy * dy) * 2;
                        ReDraw();
                    }
                    break;
            }
            base.OnMouseMove(e);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (DeviceOperation != 0)
                DeviceOperation = 0;
            else
            {
                SceneDevice d = scene.GetDevice((float)e.X / SceneSize, (float)e.Y / SceneSize);
                if (d != null)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        d.OnClick((float)e.X / SceneSize, (float)e.Y / SceneSize);
                        ReDraw();
                    }
                    if (e.Button == MouseButtons.Right)
                    {
                        CurrentDevice = d;
                        DeviceMenu.Show(this, e.Location);
                    }
                }
                else
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        SceneMenu.Show(this, e.Location);
                    }
                }
            }
            ReDraw();

            base.OnMouseClick(e);
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message ( reduce flickering )
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }
    }
}
