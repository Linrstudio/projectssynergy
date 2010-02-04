using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BaseFrontEnd
{
    public partial class SequenceEditWindow : UserControl
    {
        public KismetSequence Sequence = null;
        public CodeBlock.Output Selected = null;

        public int MouseX;
        public int MouseY;

        public bool NeedsRecompile = true;

        public SequenceEditWindow()
        {
            InitializeComponent();
        }

        private void SequenceEditWindow_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.EnableNotifyMessage, true);
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        public Point GetInputPosition(CodeBlock.Input _Input)
        {
            float count = _Input.Owner.Inputs.Count + 1;
            float myidx = _Input.Owner.Inputs.IndexOf(_Input) + 1;

            return new Point(_Input.Owner.x, _Input.Owner.y + (int)((myidx / count) * (float)_Input.Owner.height));
        }

        public Point GetOutputPosition(CodeBlock.Output _Output)
        {
            float count = _Output.Owner.Outputs.Count + 1;
            float myidx = _Output.Owner.Outputs.IndexOf(_Output) + 1;

            return new Point(_Output.Owner.x + _Output.Owner.width, _Output.Owner.y + (int)((myidx / count) * (float)_Output.Owner.height));
        }

        public CodeBlock.Input GetNearestInput(Point _pos)
        {
            if (Sequence == null) return null;

            float bestdist = 20;
            CodeBlock.Input best = null;
            foreach (CodeBlock d in Sequence.codeblocks)
            {
                foreach (CodeBlock.Input o in d.Inputs)
                {
                    Point pos = GetInputPosition(o);
                    int dx = pos.X - _pos.X; int dy = pos.Y - _pos.Y;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (dist < bestdist) { bestdist = dist; best = o; }
                }
            }
            return best;
        }

        public CodeBlock.Output GetNearestOutput(Point _pos)
        {
            if (Sequence == null) return null;

            float bestdist = 20;
            CodeBlock.Output best = null;
            foreach (CodeBlock d in Sequence.codeblocks)
            {
                foreach (CodeBlock.Output o in d.Outputs)
                {
                    Point pos = GetOutputPosition(o);
                    int dx = pos.X - _pos.X; int dy = pos.Y - _pos.Y;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (dist < bestdist) { bestdist = dist; best = o; }
                }
            }
            return best;
        }

        public void DrawPwettyLine(Graphics g, Point A, Point B)
        {
            Point c = new Point((A.X + B.X) / 2, (A.Y + B.Y) / 2);
            g.DrawBezier(Pens.Black, A.X, A.Y, c.X, A.Y, c.X, B.Y, B.X, B.Y);
            g.FillPolygon(Brushes.Black, new Point[]{
                new Point(B.X,B.Y),
                new Point(B.X-15,B.Y-5),
                new Point(B.X-15,B.Y+5)
            }, System.Drawing.Drawing2D.FillMode.Alternate);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (Sequence != null)
            {
                foreach (CodeBlock b in Sequence.codeblocks)
                {
                    List<CodeBlock> dependencies = new List<CodeBlock>();
                    if (Selected != null) dependencies.AddRange(Selected.Owner.GetDependencies());

                    //e.Graphics.DrawRectangle(Pens.Red, b.x, b.y, 100, 50);
                    float p = 0;
                    foreach (CodeBlock.Input i in b.Inputs)
                    {
                        p += 1 / (float)(b.Inputs.Count + 1);
                        Point pos = GetInputPosition(i);
                        if (!dependencies.Contains(i.Owner))
                            e.Graphics.FillRectangle(Brushes.Black, new Rectangle(pos.X - 5, pos.Y - 5, 5, 10));

                        if (i.Connected != null)
                        {
                            Point x = GetInputPosition(i);
                            Point y = GetOutputPosition(i.Connected);
                            DrawPwettyLine(e.Graphics, y, x);
                        }
                    }
                    p = 0;
                    foreach (CodeBlock.Output i in b.Outputs)
                    {
                        p += 1 / (float)(b.Inputs.Count + 1);
                        Point pos = GetOutputPosition(i);
                        if (!dependencies.Contains(i.Owner))
                            e.Graphics.FillRectangle(Brushes.Black, new Rectangle(pos.X, pos.Y - 5, 5, 10));
                    }

                    //e.Graphics.DrawString(b.GetType().Name, Font, Brushes.Black, b.x, b.y);
                    if (b is ConstantByte)
                    {
                        //e.Graphics.DrawString(((ConstantByte)b).Value.ToString(), Font, Brushes.Black, b.x, b.y + 32);
                    }
                    b.Draw(e.Graphics);
                }
                if (Selected != null)
                {
                    DrawPwettyLine(e.Graphics, GetOutputPosition(Selected), new Point(MouseX, MouseY));
                }
            }
            base.OnPaint(e);
        }

        public void Format()
        {
            if (Sequence == null) return;
            //if (NeedsRecompile) eeprom.Assamble();
            NeedsRecompile = false;
            foreach (CodeBlock b in Sequence.codeblocks)
            {
                b.x = 100 + b.GetDepth() * 150;
                b.x -= b.width / 2;
                List<CodeBlock> siblings = new List<CodeBlock>(b.GetSibblings(Sequence.codeblocks.ToArray()));
                int idx = siblings.IndexOf(b);
                b.y = idx * 50;
                b.y -= b.height / 2;
                b.y -= siblings.Count * (50 / 2);
                b.y += Height / 2;
            }

            /*
            toolStripProgressBar1.Maximum = eeprom.Size;
            toolStripProgressBar1.Value = eeprom.BytesUsed;
            toolStripStatusLabel2.Text = string.Format("{0}/{1} Bytes used", eeprom.BytesUsed, eeprom.Size);
            */
            Refresh();
        }

        public void MouseEvent(MouseEventArgs e)
        {
            MouseX = e.X;
            MouseY = e.Y;

            if (Sequence == null) return;

            if (e.Button == MouseButtons.Left)
            {
                if (Selected == null)
                {
                    CodeBlock.Output nearest = GetNearestOutput(new Point(MouseX, MouseY));
                    if (nearest != null)
                    {
                        Selected = nearest;
                        //PropertyGrid.SelectedObject = Selected.Owner;
                        NeedsRecompile = true;
                    }
                }
                Format();
            }
            if (e.Button == MouseButtons.Right)
            {

                CodeBlock.Output output = GetNearestOutput(new Point(MouseX, MouseY));
                if (output != null)
                {
                    foreach (CodeBlock.Input i in output.Connected) i.Connected = null;
                    output.Connected.Clear();
                    NeedsRecompile = true;
                }
                else
                {
                    CodeBlock.Input input = GetNearestInput(new Point(MouseX, MouseY));
                    if (input != null)
                    {
                        if (input.Connected != null)
                        {
                            input.Connected.Connected.Remove(input);
                            input.Connected = null;
                            NeedsRecompile = true;
                        }
                        else
                        {
                            Sequence.codeblocks.Remove(input.Owner);
                        }
                    }
                    else
                    {
                        ShowContextMenu(MouseX, MouseY);
                    }
                }
                Format();
            }
            if (e.Button == MouseButtons.None)
            {
                if (Selected != null)
                {
                    CodeBlock.Input nearest = GetNearestInput(new Point(MouseX, MouseY));
                    if (nearest != null)
                    {
                        List<CodeBlock> dependencies = new List<CodeBlock>();
                        if (Selected != null) dependencies.AddRange(Selected.Owner.GetDependencies());
                        if (!dependencies.Contains(nearest.Owner))
                        {
                            Sequence.Connect(Selected, nearest);
                            NeedsRecompile = true;
                        }
                    }
                    Selected = null;
                    Format();
                }
            }
        }

        public void OnContextMenuItemClicked(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;

            foreach (Type t in CodeBlock.CodeBlocks.Values)
            {
                if (t.Name == item.Text)
                {
                    Sequence.codeblocks.Add((CodeBlock)t.GetConstructor(new Type[] { }).Invoke(new object[] { }));
                    NeedsRecompile = true;
                    Format();
                }
            }

        }

        public void ShowContextMenu(int x, int y)
        {
            List<MenuItem> menuitems = new List<MenuItem>();

            CodeBlock.Initialize();
            foreach (Type t in CodeBlock.CodeBlocks.Values)
            {
                if (t.FullName != typeof(PushEvent).FullName)
                    menuitems.Add(new MenuItem(t.Name, OnContextMenuItemClicked));
            }
            ContextMenu menu = new ContextMenu(menuitems.ToArray());

            menu.Show(this, new Point(x, y));
        }

        protected override void OnNotifyMessage(Message m)
        {
            if (m.Msg != 0x14)
                base.OnNotifyMessage(m);
        }

        private void SequenceEditWindow_MouseMove(object sender, MouseEventArgs e)
        {
            MouseEvent(e);
        }

        private void SequenceEditWindow_MouseDown(object sender, MouseEventArgs e)
        {
            MouseEvent(e);
        }
    }
}
