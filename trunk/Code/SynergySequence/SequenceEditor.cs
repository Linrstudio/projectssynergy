using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SynergySequence
{
    public partial class SequenceEditWindow : UserControl
    {
        public Sequence Sequence = null;
        public List<CodeBlock> FloatingBlocks = new List<CodeBlock>();
        public int SelectedStartedAt = 0;
        public CodeBlock.Connector ConnectFrom = null;
        public CodeBlock.Connector ConnectTo = null;
        public CodeBlock SelectedBlock = null;
        bool MoveSelectedBlock = false;

        public delegate void OnBlockSelectHandler(CodeBlock _SelectedBlock);
        public event OnBlockSelectHandler OnBlockSelect = null;

        System.Drawing.Drawing2D.Matrix View;
        bool ViewStraving = false;
        Point LastMousePos;
        PointF MousePos;

        public SequenceEditWindow()
        {
            InitializeComponent();
            View = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, 0, 0);
            AllowDrop = true;
        }

        private void SequenceEditWindow_Load(object sender, EventArgs e)
        {
            MouseWheel += new MouseEventHandler(SequenceEditWindow_MouseWheel);
            AllowDrop = true;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.EnableNotifyMessage | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        public CodeBlock.Input GetNearestInput(PointF _pos)
        {
            if (Sequence == null) return null;

            float bestdist = 20;
            CodeBlock.Input best = null;
            foreach (CodeBlock d in Sequence.CodeBlocks)
            {
                foreach (CodeBlock.Input o in d.Inputs)
                {
                    PointF pos = o.GetPosition();
                    float dx = pos.X - _pos.X; float dy = pos.Y - _pos.Y;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (dist < bestdist) { bestdist = dist; best = o; }
                }
            }
            return best;
        }

        private CodeBlock.Output GetNearestOutput(PointF _pos)
        {
            if (Sequence == null) return null;

            float bestdist = 20;
            CodeBlock.Output best = null;
            foreach (CodeBlock d in Sequence.CodeBlocks)
            {
                foreach (CodeBlock.Output o in d.Outputs)
                {
                    PointF pos = o.GetPosition();
                    float dx = pos.X - _pos.X; float dy = pos.Y - _pos.Y;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (dist < bestdist) { bestdist = dist; best = o; }
                }
            }
            return best;
        }

        CodeBlock GetCodeBlock(PointF _Pos)
        {
            if (Sequence == null) return null;
            foreach (CodeBlock c in Sequence.CodeBlocks)
                if (c.Intersect(_Pos)) return c;
            return null;
        }

        private void DrawPwettyLine(Graphics g, PointF A, PointF B)
        {
            DrawPwettyLine(g, A, B, 1.5f, System.Drawing.Color.Black, true);
        }

        private void DrawPwettyLineShadow(Graphics g, PointF A, PointF B)
        {
            DrawPwettyLine(g, A, B, 1.5f, Sequence.ShadowColor, true);
        }

        private void DrawPwettyLine(Graphics g, PointF A, PointF B, float _Width, System.Drawing.Color _Color, bool _IsVertical)
        {
            if (_IsVertical)
            {
                B.Y -= 15;
                PointF c = new PointF((A.X + B.X) / 2, (A.Y + B.Y) / 2);
                g.DrawBezier(new Pen(new SolidBrush(_Color), _Width), A.X, A.Y, A.X, c.Y, B.X, c.Y, B.X, B.Y);
                g.FillPolygon(new SolidBrush(_Color), new PointF[]{
                    new PointF(B.X, B.Y+15),
                    new PointF(B.X-5, B.Y),
                    new PointF(B.X+5, B.Y)
                }, System.Drawing.Drawing2D.FillMode.Alternate);
            }
            else
            {
                B.X -= 15;
                PointF c = new PointF((A.X + B.X) / 2, (A.Y + B.Y) / 2);
                g.DrawBezier(new Pen(new SolidBrush(_Color), _Width), A.X, A.Y, c.X, A.Y, c.X, B.Y, B.X, B.Y);
                g.FillPolygon(new SolidBrush(_Color), new PointF[]{
                    new PointF(B.X+15, B.Y),
                    new PointF(B.X, B.Y+5),
                    new PointF(B.X, B.Y-5)
                }, System.Drawing.Drawing2D.FillMode.Alternate);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Matrix view = View.Clone();
            e.Graphics.Transform = view;
            e.Graphics.TranslateTransform(Width / 2, Height / 2, MatrixOrder.Append);

            //if (Resources.Background != null)//to keep visual studio from crashing on us
            {
                //e.Graphics.DrawImage(Resources.Background, new Rectangle(0, 0, Width, Height));
            }
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (Sequence != null)
            {
                foreach (CodeBlock b in Sequence.CodeBlocks)
                {
                    b.DrawShadow(e.Graphics);
                    foreach (CodeBlock.DataInput i in b.DataInputs)
                    {
                        if (i.Connected != null)
                        {
                            PointF x = i.GetPosition();
                            x.X += i.Owner.GetShadowOffset().X;
                            x.Y += i.Owner.GetShadowOffset().Y;
                            PointF y = i.Connected.GetPosition();
                            y.X += i.Connected.Owner.GetShadowOffset().X;
                            y.Y += i.Connected.Owner.GetShadowOffset().Y;
                            DrawPwettyLineShadow(e.Graphics, y, x);
                        }
                    }
                }

                CodeBlock.Input input = GetNearestInput(new PointF(MousePos.X, MousePos.Y));
                CodeBlock.Output output = GetNearestOutput(new PointF(MousePos.X, MousePos.Y));
                CodeBlock.Connector connector = null;
                if (ConnectFrom is CodeBlock.Input) connector = input;
                if (ConnectFrom is CodeBlock.Output) connector = output;
                foreach (CodeBlock b in Sequence.CodeBlocks)
                {
                    List<CodeBlock> dependencies = new List<CodeBlock>();

                    if (ConnectFrom != null) dependencies.AddRange(ConnectFrom.Owner.GetDependencies());

                    foreach (CodeBlock.DataInput i in b.DataInputs)
                    {
                        PointF pos = i.GetPosition();
                        if (!dependencies.Contains(i.Owner)) e.Graphics.FillRectangle(new SolidBrush(Sequence.Manager.GetDataType(i.datatype).Color), new RectangleF(pos.X - 5, pos.Y - 5, 10, 5));

                        if (i.Connected != null)
                        {
                            PointF x = i.GetPosition();
                            PointF y = i.Connected.GetPosition();
                            DrawPwettyLine(e.Graphics, y, x, 1.5f, Sequence.Manager.GetDataType(i.datatype).Color, true);
                        }
                    }

                    foreach (CodeBlock.DataOutput i in b.DataOutputs)
                    {
                        PointF pos = i.GetPosition();
                        if (!dependencies.Contains(i.Owner)) e.Graphics.FillRectangle(new SolidBrush(Sequence.Manager.GetDataType(i.datatype).Color), new RectangleF(pos.X - 5, pos.Y, 10, 5));
                    }

                    foreach (CodeBlock.TriggerInput i in b.TriggerInputs)
                    {
                        PointF pos = i.GetPosition();

                        e.Graphics.FillRectangle(new SolidBrush(Color.Black), new RectangleF(pos.X - 5, pos.Y - 5, 5, 10));
                    }

                    foreach (CodeBlock.TriggerOutput o in b.TriggerOutputs)
                    {
                        PointF pos = o.GetPosition();

                        foreach (CodeBlock.TriggerInput i in o.Connected)
                        {
                            DrawPwettyLine(e.Graphics, pos, i.GetPosition(), 1.5f, Color.Black, false);
                        }

                        e.Graphics.FillRectangle(new SolidBrush(Color.Black), new RectangleF(pos.X, pos.Y - 5, 5, 10));
                    }
                }

                if (ConnectFrom != null)
                {
                    if (ConnectTo != null)
                    {
                        if (ConnectFrom is CodeBlock.Output)
                        {
                            if (ConnectFrom is CodeBlock.DataOutput)
                                DrawPwettyLine(e.Graphics, ConnectFrom.GetPosition(), ConnectTo.GetPosition(), 2, Sequence.Manager.GetDataType(((CodeBlock.DataOutput)ConnectFrom).datatype).Color, true);
                            else
                                DrawPwettyLine(e.Graphics, ConnectFrom.GetPosition(), ConnectTo.GetPosition(), 2, Color.Black, false);
                        }
                        else
                        {
                            if (ConnectFrom is CodeBlock.DataInput)
                                DrawPwettyLine(e.Graphics, ConnectTo.GetPosition(), ConnectFrom.GetPosition(), 2, Sequence.Manager.GetDataType(((CodeBlock.DataInput)ConnectFrom).datatype).Color, true);
                            else
                                DrawPwettyLine(e.Graphics, ConnectTo.GetPosition(), ConnectFrom.GetPosition(), 2, Color.Black, false);
                        }
                    }
                    else
                    {
                        if (ConnectFrom is CodeBlock.Output)
                        {
                            if (ConnectFrom is CodeBlock.DataOutput)
                                DrawPwettyLine(e.Graphics, ConnectFrom.GetPosition(), new PointF(MousePos.X, MousePos.Y), 2, Sequence.Manager.GetDataType(((CodeBlock.DataOutput)ConnectFrom).datatype).Color, true);
                            else
                                DrawPwettyLine(e.Graphics, ConnectFrom.GetPosition(), new PointF(MousePos.X, MousePos.Y), 2, Color.Black, false);
                        }
                        else
                        {
                            if (ConnectFrom is CodeBlock.DataInput)
                                DrawPwettyLine(e.Graphics, new PointF(MousePos.X, MousePos.Y), ConnectFrom.GetPosition(), 2, Sequence.Manager.GetDataType(((CodeBlock.DataInput)ConnectFrom).datatype).Color, true);
                            else
                                DrawPwettyLine(e.Graphics, new PointF(MousePos.X, MousePos.Y), ConnectFrom.GetPosition(), 2, Color.Black, false);
                        }
                    }
                }

                //draw blocks 
                foreach (CodeBlock b in Sequence.CodeBlocks)
                {
                    b.Draw(e.Graphics);
                    b.DrawText(e.Graphics);
                }

            }
            base.OnPaint(e);
        }

        int doublerightclicktimer = 0;
        bool doublerightclicktimervar = false;
        public void MouseEvent(MouseEventArgs e, bool _LClick)
        {
            if (!doublerightclicktimervar && e.Button == MouseButtons.Right)
            {
                if (doublerightclicktimer > Environment.TickCount)
                {
                    CodeBlock b = GetCodeBlock(new PointF(MousePos.X, MousePos.Y));
                    if (b != null)
                    {
                        Sequence.RemoveCodeBlock(b);
                        Invalidate();
                    }
                }
                doublerightclicktimer = Environment.TickCount + 500;
            }

            doublerightclicktimervar = e.Button == MouseButtons.Right;

            if (Sequence == null) return;

            if (e.Button == MouseButtons.Left)
            {

            }

            //Format();
        }

        public void OnContextMenuItemClicked(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;

            if (item.Tag is string)
            {
                //CodeBlock b = new BlockRemoteEvent(Sequence);
                //b.SetValues((string)item.Tag);
                //Sequence.codeblocks.Add(b);
            }
            if (item.Tag is Type)
            {
                foreach (SequenceManager.Prototype p in Sequence.Manager.Prototypes)
                {
                    if (p.BlockType == item.Tag)
                    {
                        CodeBlock block = (CodeBlock)p.Create();
                        Sequence.AddCodeBlock(block);
                        //Format();
                    }
                }
            }
        }

        [Obsolete]
        public void ShowContextMenu(int x, int y)
        {
            Dictionary<string, MenuItem> menuitems = new Dictionary<string, MenuItem>();

            foreach (SequenceManager.Prototype p in Sequence.Manager.Prototypes)
            {
                if (!p.UserCanAdd) continue;
                if (!menuitems.ContainsKey(p.GroupName))
                    menuitems.Add(p.GroupName, new MenuItem(p.GroupName));

                MenuItem item = new MenuItem(p.Name, OnContextMenuItemClicked);
                item.Tag = p.BlockType;
                menuitems[p.GroupName].MenuItems.Add(item);
            }

            ContextMenu menu = new ContextMenu(menuitems.Values.ToArray());

            menu.Show(this, new Point(x, y));
        }

        protected override void OnNotifyMessage(Message m)
        {
            if (m.Msg != 0x14)
                base.OnNotifyMessage(m);
        }

        private void SequenceEditWindow_MouseMove(object sender, MouseEventArgs e)
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


            if (SelectedBlock != null)
            {
                if (MoveSelectedBlock)
                {
                    SelectedBlock.X = MousePos.X;
                    SelectedBlock.Y = MousePos.Y;
                    Invalidate();
                }
            }

            if (ConnectFrom != null)
            {
                if (ConnectFrom is CodeBlock.Input)
                {
                    ConnectTo = GetNearestOutput(MousePos);

                }
                else
                {
                    ConnectTo = GetNearestInput(MousePos);

                }
                if (ConnectTo != null && ConnectTo is CodeBlock.TriggerInput && ConnectFrom is CodeBlock.DataOutput) ConnectTo = null;
                if (ConnectTo != null && ConnectTo is CodeBlock.DataInput && ConnectFrom is CodeBlock.TriggerOutput) ConnectTo = null;
                if (ConnectTo != null && ConnectTo is CodeBlock.TriggerOutput && ConnectFrom is CodeBlock.DataInput) ConnectTo = null;
                if (ConnectTo != null && ConnectTo is CodeBlock.DataOutput && ConnectFrom is CodeBlock.TriggerInput) ConnectTo = null;

                Invalidate();
            }

            if (Sequence == null) return;
            //MouseEvent(e, false);

            LastMousePos = pos;
        }

        private void SequenceEditWindow_MouseDown(object sender, MouseEventArgs e)
        {

            //select nearest codeblock
            if (e.Button == MouseButtons.Left)
            {
                if (!MoveSelectedBlock)//only allow to connect lines when not moving blocks
                {
                    CodeBlock.Output nearestout = GetNearestOutput(new PointF(MousePos.X, MousePos.Y));
                    CodeBlock.Input nearestin = GetNearestInput(new PointF(MousePos.X, MousePos.Y));

                    if (nearestout != null) ConnectFrom = nearestout;
                    if (nearestin != null) ConnectFrom = nearestin;
                }

                CodeBlock selected = GetCodeBlock(new PointF(MousePos.X, MousePos.Y));
                if (selected != SelectedBlock)
                {
                    SelectedBlock = selected;
                    OnBlockSelect(SelectedBlock);
                    Sequence.Select(SelectedBlock);

                    Invalidate();
                }
                else
                {
                    if (SelectedBlock != null && ConnectFrom == null)
                    {
                        MoveSelectedBlock = true;
                    }
                }

            }


            if (e.Button == MouseButtons.Middle)
            {
                ViewStraving = true;
            }
            if (Sequence == null) return;
            //MouseEvent(e, true);

            if (e.Button == MouseButtons.Right)
            {
                //if (SelectedBlock != null)//not sure if this is a good idea
                {
                    CodeBlock.Output output = GetNearestOutput(new PointF(MousePos.X, MousePos.Y));
                    CodeBlock.Input input = GetNearestInput(new PointF(MousePos.X, MousePos.Y));

                    if (output != null)
                    {
                        if (output is CodeBlock.DataOutput)
                        {
                            foreach (CodeBlock.DataInput i in ((CodeBlock.DataOutput)output).Connected) i.Connected = null;
                            ((CodeBlock.DataOutput)output).Connected.Clear();
                        }
                        if (output is CodeBlock.TriggerOutput)
                        {
                            CodeBlock.TriggerOutput o = (CodeBlock.TriggerOutput)output;
                            foreach (CodeBlock.TriggerInput i in o.Connected) i.Connected.Remove(o);
                            o.Connected.Clear();
                        }
                        Invalidate();
                    }

                    if (input != null)
                    {
                        if (input is CodeBlock.DataInput)
                        {
                            if (((CodeBlock.DataInput)input).Connected != null)
                            {
                                ((CodeBlock.DataInput)input).Connected.Connected.Remove(((CodeBlock.DataInput)input));
                                ((CodeBlock.DataInput)input).Connected = null;
                            }
                        }
                        if (input is CodeBlock.TriggerInput)
                        {
                            CodeBlock.TriggerInput i = (CodeBlock.TriggerInput)input;
                            foreach (CodeBlock.TriggerOutput o in i.Connected) o.Connected.Clear();
                            i.Connected.Clear();
                        }
                        Invalidate();
                    }
                }
            }
        }

        private void t_Update_Tick(object sender, EventArgs e)
        {
            if (Sequence == null) return;
            foreach (CodeBlock c in Sequence.CodeBlocks) c.Update();
            //Format();
        }
        CodeBlock Dragging = null;
        private void SequenceEditWindow_DragDrop(object sender, DragEventArgs e)
        {
            Dragging = null;
        }

        private void SequenceEditWindow_DragEnter(object sender, DragEventArgs e)
        {
            if (Dragging == null)
            {
                object[] data = (object[])(e.Data.GetData(typeof(object[])));

                if (data[0] is SequenceManager.Prototype)
                {
                    SequenceManager.Prototype p = (SequenceManager.Prototype)data[0];
                    CodeBlock b = Sequence.Manager.CreateCodeBlock(p);
                    Sequence.AddCodeBlock(b);
                    //Format();

                    Dragging = b;

                    e.Effect = DragDropEffects.Copy;
                }
                else e.Effect = DragDropEffects.None;
            }
        }

        private void SequenceEditWindow_DragLeave(object sender, EventArgs e)
        {
            //TODO does this make sense ?
            if (Dragging != null)
            {
                Sequence.RemoveCodeBlock(Dragging);
                Dragging = null;
            }
        }

        private void SequenceEditWindow_DragOver(object sender, DragEventArgs e)
        {
            Point pos = new Point(e.X - PointToScreen(new Point(0, 0)).X, e.Y - PointToScreen(new Point(0, 0)).Y);
            MousePos.X = pos.X - Width / 2;
            MousePos.Y = pos.Y - Height / 2;
            PointF[] points = new PointF[] { MousePos };
            Matrix m = View.Clone();
            m.Invert();
            m.TransformPoints(points);
            MousePos = points[0];

            if (Dragging != null)
            {
                Dragging.X = MousePos.X;
                Dragging.Y = MousePos.Y;

                Invalidate();
            }
        }

        private void SequenceEditWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                ViewStraving = false;
            }
            if (e.Button == MouseButtons.Left)
            {
                MoveSelectedBlock = false;
                //if (OnSelectedItemChanged != null) OnSelectedItemChanged(SelectedControl);


                if (ConnectFrom != null && ConnectTo != null)
                {
                    //FIXME
                    //List<CodeBlock> dependencies = new List<CodeBlock>();
                    //if (SelectedOutput != null) dependencies.AddRange(SelectedOutput.Owner.GetDependencies());
                    //if (!dependencies.Contains(SelectedInput.Owner))
                    {
                        Sequence.Connect(ConnectFrom, ConnectTo);
                    }
                }
                ConnectFrom = null;
                ConnectTo = null;
                Invalidate();
            }
        }

        void SequenceEditWindow_MouseWheel(object sender, MouseEventArgs e)
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
