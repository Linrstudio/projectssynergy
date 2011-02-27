using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WebInterface
{
    public partial class SceneEditor : UserControl
    {
        public delegate void SelectedItemChanged(Control _NewItem);
        public event SelectedItemChanged OnSelectedItemChanged = null;

        System.Drawing.Drawing2D.Matrix View;
        bool ViewStraving = false;

        Scene scene = null;
        public SceneEditor()
        {
            InitializeComponent();
            float smallestedge = Math.Min(Width, Height);
            View = new System.Drawing.Drawing2D.Matrix(smallestedge, 0, 0, smallestedge, -smallestedge / 2, -smallestedge / 2);
        }

        public void SetScene(Scene _Scene)
        {
            scene = _Scene;
        }

        private void SceneEditor_Load(object sender, EventArgs e)
        {
            MouseWheel += new MouseEventHandler(SceneEditor_MouseWheel);
            DoubleBuffered = true;
            AllowDrop = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.EnableNotifyMessage | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        private void SceneEditor_Paint(object sender, PaintEventArgs e)
        {
            Matrix view = View.Clone();
            e.Graphics.Transform = view;
            e.Graphics.TranslateTransform(Width / 2, Height / 2, MatrixOrder.Append);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen outline = new Pen(Brushes.Black, 0.005f);
            Pen outlineselected = new Pen(Brushes.Red, 0.01f);
            e.Graphics.Clear(Color.CornflowerBlue);
            e.Graphics.FillRectangle(Brushes.LightGray, new Rectangle(0, 0, 1, 1));

            if (scene == null) return;
            foreach (Control c in scene.Controls)
            {
                if (c == SelectedControl)
                    e.Graphics.DrawRectangle(outlineselected, c.X - c.Width / 2, c.Y - c.Height / 2, c.Width, c.Height);
                else
                    e.Graphics.DrawRectangle(outline, c.X - c.Width / 2, c.Y - c.Height / 2, c.Width, c.Height);
            }
        }
        Control SelectedControl = null;
        bool MoveSelectedControl = false;
        bool ResizeSelectedControlH = false;
        bool ResizeSelectedControlV = false;
        Point LastMousePos;
        PointF MousePos;

        private void SceneEditor_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                ViewStraving = false;
            }
            if (e.Button == MouseButtons.Left)
            {
                MoveSelectedControl = false;
                ResizeSelectedControlH = false;
                ResizeSelectedControlV = false;
                if (OnSelectedItemChanged != null) OnSelectedItemChanged(SelectedControl);
            }
        }

        private void SceneEditor_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                ViewStraving = true;
            }
            if (e.Button == MouseButtons.Left)
            {
                bool anyselected = false;
                foreach (Control c in scene.Controls)
                {
                    if (MousePos.X > c.X - c.Width / 2 && MousePos.Y > c.Y - c.Height / 2 && MousePos.X < c.X + c.Width / 2 && MousePos.Y < c.Y + c.Height / 2)
                    {
                        anyselected = true;
                        if (SelectedControl != c)//we clicked a control that was not yet selected
                        {
                            SelectedControl = c;
                            if (OnSelectedItemChanged != null) OnSelectedItemChanged(SelectedControl);
                            Invalidate();
                        }
                        else //we clicked on the control that is already selected
                        {
                            //see where we clicked the control to determine if we want to resize or move
                            ResizeSelectedControlV = (MousePos.Y < c.Y - (c.Height / 2) + (c.Height / 8) || MousePos.Y > c.Y + (c.Height / 2) - (c.Height / 8));
                            ResizeSelectedControlH = (MousePos.X < c.X - (c.Width / 2) + (c.Width / 8) || MousePos.X > c.X + (c.Width / 2) - (c.Width / 8));

                            if (!ResizeSelectedControlH && !ResizeSelectedControlV)//we're not resizing, lets move the control
                            {
                                MoveSelectedControl = true;
                            }
                        }
                        break;
                    }
                }
                if (!anyselected && SelectedControl != null)//unselected
                {
                    SelectedControl = null;
                    if (OnSelectedItemChanged != null) OnSelectedItemChanged(SelectedControl);
                    Invalidate();
                }
            }
        }

        private void SceneEditor_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = new Point(e.X, e.Y);
            MousePos.X = pos.X - Width / 2;
            MousePos.Y = pos.Y - Height / 2;
            PointF[] points = new PointF[] { MousePos };
            Matrix m = View.Clone();
            m.Invert();
            m.TransformPoints(points);
            MousePos = points[0];

            if (ViewStraving)
            {
                View.Translate((float)(pos.X - LastMousePos.X), (float)(pos.Y - LastMousePos.Y), MatrixOrder.Append);
                Invalidate();
            }

            if (SelectedControl != null)
            {
                if (MoveSelectedControl)
                {
                    SelectedControl.X = MousePos.X;
                    SelectedControl.Y = MousePos.Y;
                    Invalidate();
                }

                if (ResizeSelectedControlV)
                {
                    SelectedControl.Height = Math.Abs(SelectedControl.Y - MousePos.Y) * 2;
                    Invalidate();
                }
                if (ResizeSelectedControlH)
                {
                    SelectedControl.Width = Math.Abs(SelectedControl.X - MousePos.X) * 2;
                    Invalidate();
                }
            }

            LastMousePos = pos;

        }

        void SceneEditor_MouseWheel(object sender, MouseEventArgs e)
        {
            float s = 1 + ((float)e.Delta / 1000.0f);

            float tx = -(e.X - (Width / 2));
            float ty = -(e.Y - (Height / 2));

            View.Translate(tx, ty, MatrixOrder.Append);

            Matrix scale = new Matrix(s, 0, 0, s, 0, 0);
            scale.Multiply(View);
            View = scale;

            View.Translate(-tx, -ty, MatrixOrder.Append);

            Invalidate();
        }
    }
}
