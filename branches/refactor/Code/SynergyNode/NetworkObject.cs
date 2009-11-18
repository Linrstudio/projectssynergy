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
        public bool PhysicsEnabled = true;
        public float Xs = 0, Ys = 0;
        public float X = 0.5f, Y = 0.5f, Size = 0.5f;
        public NetworkView Parent;
        public NetworkObject()
        {

        }
        public virtual void OnMouseChange(float _X, float _Y, bool _LMB, bool _LeftDown, bool _LeftUp)
        {
            if (_LeftDown || _LeftUp) Redraw();
        }

        public virtual void Update()
        {
            if (PhysicsEnabled)
            {
                X += Xs;
                Y += Ys;
                Xs *= 0.9f;
                Ys *= 0.9f;
            }
            else { Xs = Ys = 0; }
        }

        public void Push(PointF _Point, float _Distance, float _Force)
        {
            float dx = X - _Point.X;
            float dy = Y - _Point.Y;

            float len = (float)Math.Sqrt((double)(dx * dx + dy * dy));
            len -= _Distance;
            if (len > 0) return;
            dx *= len;
            dy *= len;

            Xs -= dx * _Force;
            Ys -= dy * _Force;
        }

        public void Interact(PointF _Point, float _Distance, float _Force)
        {
            float dx = X - _Point.X;
            float dy = Y - _Point.Y;

            float len = (float)Math.Sqrt((double)(dx * dx + dy * dy));
            dx *= len - _Distance;
            dy *= len - _Distance;

            Xs -= dx * _Force;
            Ys -= dy * _Force;
        }

        public void Gravity(PointF _Point, float _Force)
        {
            float dx = X - _Point.X;
            float dy = Y - _Point.Y;

            Xs -= dx * _Force;
            Ys -= dy * _Force;
        }

        public void Redraw()
        {
            if (Parent != null) Parent.Redraw();
        }

        public virtual void Draw(Graphics _Graphics)
        {
            //draw one fresh health debug line
            float x = X;
            float y = Y;
            float s = Size;
            x -= s * 0.5f;
            y -= s * 0.5f;

            _Graphics.DrawRectangle(new Pen(Brushes.Blue, 0.0025f), x, y, s, s);
        }
    }
}
