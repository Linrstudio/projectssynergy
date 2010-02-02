using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BaseFrontEnd
{
    public partial class KismetEditor : Form
    {
        KismetSequence sequence = new KismetSequence();

        int mouseX;
        int mouseY;

        public CodeBlock.Output selected = null;

        public KismetEditor()
        {
            InitializeComponent();

        }

        private void KismetEditor_Load(object sender, EventArgs e)
        {
            sequence.root = new PushEvent();
            sequence.codeblocks.Add(sequence.root);
            sequence.codeblocks.Add(new SetDebugLed());
            sequence.codeblocks.Add(new Compare());
            sequence.codeblocks.Add(new ConstantWeekDay());
            sequence.codeblocks.Add(new ConstantByte());
            sequence.codeblocks.Add(new GetHour());
            sequence.codeblocks.Add(new GetMinute());
            sequence.codeblocks.Add(new GetSecond());
            sequence.codeblocks.Add(new GetDay());

            Format();
        }

        public void Format()
        {
            byte[] bytecode = sequence.GetByteCode();

            foreach (CodeBlock b in sequence.codeblocks)
            {
                b.x = 100 + b.GetDepth() * 200;
                b.x -= b.width / 2;
                List<CodeBlock> siblings = new List<CodeBlock>(b.GetSibblings(sequence.codeblocks.ToArray()));
                int idx = siblings.IndexOf(b);
                b.y = 100 + (int)((float)idx * 50);
                b.y -= b.height / 2;
            }


            toolStripProgressBar1.Maximum = 256;
            toolStripProgressBar1.Value = bytecode.Length;
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

        public void DrawPwettyLine(Graphics g, Point A, Point B)
        {
            Point c = new Point((A.X + B.X) / 2, (A.Y + B.Y) / 2);
            g.DrawBezier(Pens.Black, A.X, A.Y, c.X, A.Y, c.X, B.Y, B.X, B.Y);
        }

        private void KismetEditor_Paint(object sender, PaintEventArgs e)
        {
            foreach (CodeBlock b in sequence.codeblocks)
            {
                List<CodeBlock> dependencies = new List<CodeBlock>();
                if (selected != null) dependencies.AddRange(selected.Owner.GetDependencies());

                //e.Graphics.DrawRectangle(Pens.Red, b.x, b.y, 100, 50);
                float p = 0;
                foreach (CodeBlock.Input i in b.Inputs)
                {
                    p += 1 / (float)(b.Inputs.Count + 1);
                    Point pos = GetInputPosition(i);
                    if (!dependencies.Contains(i.Owner))
                        e.Graphics.FillRectangle(Brushes.Black, new Rectangle(pos.X - 10, pos.Y - 5, 10, 10));

                    if (i.Connected != null)
                    {
                        Point x = GetInputPosition(i);
                        Point y = GetOutputPosition(i.Connected);
                        DrawPwettyLine(e.Graphics, x, y);
                    }
                }
                p = 0;
                foreach (CodeBlock.Output i in b.Outputs)
                {
                    p += 1 / (float)(b.Inputs.Count + 1);
                    Point pos = GetOutputPosition(i);
                    if (!dependencies.Contains(i.Owner))
                        e.Graphics.FillRectangle(Brushes.Black, new Rectangle(pos.X, pos.Y - 5, 10, 10));
                }

                //e.Graphics.DrawString(b.GetType().Name, Font, Brushes.Black, b.x, b.y);
                if (b is ConstantByte)
                {
                    //e.Graphics.DrawString(((ConstantByte)b).Value.ToString(), Font, Brushes.Black, b.x, b.y + 32);
                }

                if (selected != null)
                {
                    DrawPwettyLine(e.Graphics, GetOutputPosition(selected), new Point(mouseX, mouseY));
                }

                b.Draw(e.Graphics);
            }
        }

        public byte[] GetCode()
        {
            return sequence.GetByteCode();
        }

        private void KismetEditor_ResizeEnd(object sender, EventArgs e)
        {
            Format();
            Refresh();
        }

        private void KismetEditor_MouseMove(object sender, MouseEventArgs e)
        {
            MouseEvent(e);
        }

        public void MouseEvent(MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                if (selected == null)
                {
                    CodeBlock.Output nearest = GetNearestOutput(new Point(mouseX, mouseY));
                    if (nearest != null)
                    {
                        selected = nearest;
                        PropertyGrid.SelectedObject = selected.Owner;
                    }
                }
                Refresh();
            }
            if (e.Button == MouseButtons.Right)
            {
                {
                    CodeBlock.Output nearest = GetNearestOutput(new Point(mouseX, mouseY));
                    if (nearest != null)
                    {
                        foreach (CodeBlock.Input i in nearest.Connected) i.Connected = null;
                        nearest.Connected.Clear();
                        Format();
                    }
                }
                {
                    CodeBlock.Input nearest = GetNearestInput(new Point(mouseX, mouseY));
                    if (nearest != null && nearest.Connected != null)
                    {
                        nearest.Connected.Connected.Remove(nearest);
                        nearest.Connected = null;
                        Format();
                    }
                }
                Refresh();
            }
            if (e.Button == MouseButtons.None)
            {
                if (selected != null)
                {
                    CodeBlock.Input nearest = GetNearestInput(new Point(mouseX, mouseY));
                    if (nearest != null) { sequence.Connect(selected, nearest); Format(); }
                    selected = null;
                    Refresh();
                }
            }
        }

        public CodeBlock.Input GetNearestInput(Point _pos)
        {
            foreach (CodeBlock d in sequence.codeblocks)
            {
                foreach (CodeBlock.Input o in d.Inputs)
                {
                    Point pos = GetInputPosition(o);
                    int dx = pos.X - _pos.X; int dy = pos.Y - _pos.Y;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (dist < 20) return o;
                }
            }
            return null;
        }

        public CodeBlock.Output GetNearestOutput(Point _pos)
        {
            foreach (CodeBlock d in sequence.codeblocks)
            {
                foreach (CodeBlock.Output o in d.Outputs)
                {
                    Point pos = GetOutputPosition(o);
                    int dx = pos.X - _pos.X; int dy = pos.Y - _pos.Y;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (dist < 20) return o;
                }
            }
            return null;
        }

        private void KismetEditor_MouseDown(object sender, MouseEventArgs e)
        {
            MouseEvent(e);
        }
    }
}
