using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SynergyNode
{
    public class NetworkObject
    {
        public float X = 0.5f, Y = 0.5f, Size = 0.5f;
        public NetworkView Parent;
        public virtual void OnMouseChange(float _X, float _Y, bool _LMB, bool _LeftDown, bool _LeftUp)
        {
            if (_LeftDown || _LeftUp) Redraw();
        }

        public void Redraw()
        {
            if (Parent != null) Parent.Redraw();
        }

        public virtual void Draw(Graphics _Graphics, float _CameraX, float _CameraY, float _Zoom)
        {
            float s = Size * _Zoom;
            float x = (X - _CameraX) * _Zoom;
            float y = (Y - _CameraY) * _Zoom;
            x -= s * 0.5f;
            y -= s * 0.5f;
            
            _Graphics.DrawRectangle(Pens.Red, x, y, s, s);
        }
    }
}
