using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        public CodeBlock.Output SelectedOutput = null;
        public CodeBlock.Input SelectedInput = null;
        public CodeBlock SelectedBlock = null;

        public int MouseX;
        public int MouseY;

        public bool NeedsRecompile = true;

        public delegate void OnBlockSelectHandler(CodeBlock _SelectedBlock);
        public event OnBlockSelectHandler OnBlockSelect = null;

        public SequenceEditWindow()
        {
            InitializeComponent();
            AllowDrop = true;
        }

        private void SequenceEditWindow_Load(object sender, EventArgs e)
        {
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

        CodeBlock GetCodeBlock(Point _Pos)
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
                foreach (CodeBlock b in Sequence.CodeBlocks)
                {
                    List<CodeBlock> dependencies = new List<CodeBlock>();
                    if (SelectedOutput != null) dependencies.AddRange(SelectedOutput.Owner.GetDependencies());

                    CodeBlock.Input input = GetNearestInput(new Point(MouseX, MouseY));
                    CodeBlock.Output output = GetNearestOutput(new Point(MouseX, MouseY));

                    //e.Graphics.DrawRectangle(Pens.Red, b.x, b.y, 100, 50);
                    foreach (CodeBlock.DataInput i in b.DataInputs)
                    {
                        PointF pos = i.GetPosition();
                        if (!dependencies.Contains(i.Owner))
                            e.Graphics.FillRectangle(new SolidBrush(Sequence.Manager.GetDataType(i.datatype).Color), new RectangleF(pos.X - 5, pos.Y - 5, 10, 5));

                        if (i.Connected != null)
                        {
                            PointF x = i.GetPosition();
                            PointF y = i.Connected.GetPosition();
                            DrawPwettyLine(e.Graphics, y, x, 1.5f, Sequence.Manager.GetDataType(i.datatype).Color, true);
                        }

                        if (i == input)
                        {
                            i.tooptip.Show(i.Text, this, new Point((int)pos.X, (int)pos.Y), 1000);
                        }
                    }

                    foreach (CodeBlock.DataOutput i in b.DataOutputs)
                    {
                        PointF pos = i.GetPosition();
                        if (!dependencies.Contains(i.Owner))
                            e.Graphics.FillRectangle(new SolidBrush(Sequence.Manager.GetDataType(i.datatype).Color), new RectangleF(pos.X - 5, pos.Y, 10, 5));

                        if (i == output)
                        {
                            i.tooptip.Show(i.Text, this, new Point((int)pos.X, (int)pos.Y), 1000);
                        }
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
                    b.Draw(e.Graphics);
                    b.DrawText(e.Graphics);
                }
                if (SelectedOutput != null && SelectedInput != null)
                {
                    if (SelectedOutput is CodeBlock.DataOutput && SelectedInput is CodeBlock.DataInput)
                    {
                        DrawPwettyLine(e.Graphics, SelectedOutput.GetPosition(), SelectedInput.GetPosition(), 2, Sequence.Manager.GetDataType(((CodeBlock.DataInput)SelectedInput).datatype).Color, true);
                    }
                    else
                        DrawPwettyLine(e.Graphics, SelectedOutput.GetPosition(), SelectedInput.GetPosition(), 2, Color.Black, false);
                }
                else
                {
                    if (SelectedOutput != null)
                    {
                        if (SelectedOutput is CodeBlock.DataOutput)
                            DrawPwettyLine(e.Graphics, SelectedOutput.GetPosition(), new PointF(MouseX, MouseY), 2, Sequence.Manager.GetDataType( ((CodeBlock.DataOutput)SelectedOutput).datatype).Color, true);
                        else
                            DrawPwettyLine(e.Graphics, SelectedOutput.GetPosition(), new PointF(MouseX, MouseY), 2, Color.Black, false);
                    }

                    if (SelectedInput != null)
                    {
                        if (SelectedInput is CodeBlock.DataInput)
                            DrawPwettyLine(e.Graphics, new PointF(MouseX, MouseY), SelectedInput.GetPosition(), 2, Sequence.Manager.GetDataType( ((CodeBlock.DataInput)SelectedInput).datatype).Color, true);
                        else
                            DrawPwettyLine(e.Graphics, new PointF(MouseX, MouseY), SelectedInput.GetPosition(), 2, Color.Black, false);
                    }
                }
            }
            base.OnPaint(e);
        }

        public void UpdateFloatingBlocks()
        {
            FloatingBlocks.Clear();
            foreach (CodeBlock b in Sequence.CodeBlocks)
            {
                bool found = true;
                foreach (CodeBlock.Input i in b.Inputs)
                {
                    if (i.AnyConnected) found = false;
                }
                foreach (CodeBlock.Output o in b.Outputs)
                {
                    if (o.AnyConnected) found = false;
                }
                if (found) FloatingBlocks.Add(b);
            }
        }

        public void Format()
        {
            if (Sequence == null) return;


            //if (NeedsRecompile) eeprom.Assamble();
            NeedsRecompile = false;

            float minX = 0;
            float minY = 0;
            float maxX = 0;
            float maxY = 0;

            foreach (CodeBlock b in Sequence.CodeBlocks)
            {
                minX = Math.Min(minX, b.X);
                minY = Math.Min(minY, b.Y);
            }

            foreach (CodeBlock b in Sequence.CodeBlocks)
            {
                b.X -= minX;
                b.Y -= minY;

                maxX = Math.Max(maxX, b.X + b.Width);
                maxY = Math.Max(maxY, b.Y + b.Height);
            }

            Width = (int)Math.Max(Parent.Bounds.Width, maxX + 20);
            Height = (int)Math.Max(Parent.Bounds.Height, maxY + 20);
            /*
            UpdateFloatingBlocks();
            int floatx = 100;
            foreach (CodeBlock b in FloatingBlocks)
            {
                b.X = floatx;
                b.Y = 100;
                floatx += 150;
            }
*/
            Refresh();
        }
        int doublerightclicktimer = 0;
        bool doublerightclicktimervar = false;
        public void MouseEvent(MouseEventArgs e, bool _LClick)
        {
            MouseX = e.X;
            MouseY = e.Y;

            if (!doublerightclicktimervar && e.Button == MouseButtons.Right)
            {
                if (doublerightclicktimer > Environment.TickCount)
                {
                    CodeBlock b = GetCodeBlock(new Point(MouseX, MouseY));
                    if (b != null) Sequence.RemoveCodeBlock(b);
                }
                doublerightclicktimer = Environment.TickCount + 500;
            }

            doublerightclicktimervar = e.Button == MouseButtons.Right;

            if (Sequence == null) return;

            if (e.Button == MouseButtons.Left)
            {
                CodeBlock.Output nearestout = GetNearestOutput(new Point(MouseX, MouseY));
                CodeBlock.Input nearestin = GetNearestInput(new Point(MouseX, MouseY));

                if (nearestout != null)
                {
                    if (SelectedOutput == null)
                    {
                        SelectedOutput = nearestout;
                        if (SelectedInput == null) SelectedStartedAt = 1;
                    }
                }
                else if (SelectedStartedAt == 2) SelectedOutput = null;

                if (nearestin != null)
                {
                    if (SelectedInput == null)
                    {
                        SelectedInput = nearestin;
                        if (SelectedOutput == null) SelectedStartedAt = 2;
                    }
                }
                else if (SelectedStartedAt == 1) SelectedInput = null;


                //select nearest codeblock
                if (_LClick)
                {
                    SelectedBlock = GetCodeBlock(new Point(MouseX, MouseY));
                    OnBlockSelect(SelectedBlock);
                    if (SelectedBlock != null) Sequence.Select(SelectedBlock);
                }
                else { SelectedBlock = null; }


            }

            if (e.Button == MouseButtons.Right)
            {
                if (Dragging == null)
                {
                    Dragging = GetCodeBlock(new Point(MouseX, MouseY));
                }
                else
                {
                    Dragging.X = MouseX;
                    Dragging.Y = MouseY;
                }
            }

            if (e.Button == MouseButtons.None)
            {
                if (SelectedOutput != null && SelectedInput != null)
                {
                    List<CodeBlock> dependencies = new List<CodeBlock>();
                    if (SelectedOutput != null) dependencies.AddRange(SelectedOutput.Owner.GetDependencies());
                    if (!dependencies.Contains(SelectedInput.Owner))
                    {
                        Sequence.Connect(SelectedOutput, SelectedInput);
                        NeedsRecompile = true;
                    }
                }
                SelectedOutput = null; SelectedInput = null;

                Dragging = null;
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
                        NeedsRecompile = true;
                        Format();
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
            if (Sequence == null) return;
            MouseEvent(e, false);
        }

        private void SequenceEditWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (Sequence == null) return;
            MouseEvent(e, true);

            if (e.Button == MouseButtons.Right)
            {

                CodeBlock.Output output = GetNearestOutput(new Point(MouseX, MouseY));
                if (output != null)
                {
                    if (output is CodeBlock.DataOutput)
                    {
                        foreach (CodeBlock.DataInput i in ((CodeBlock.DataOutput)output).Connected) i.Connected = null;
                        ((CodeBlock.DataOutput)output).Connected.Clear();
                        NeedsRecompile = true;
                    }
                    if (output is CodeBlock.TriggerOutput)
                    {
                        CodeBlock.TriggerOutput o = (CodeBlock.TriggerOutput)output;
                        foreach (CodeBlock.TriggerInput i in o.Connected) i.Connected.Remove(o);
                        o.Connected.Clear();
                        NeedsRecompile = true;
                    }
                }
                else
                {
                    CodeBlock.Input input = GetNearestInput(new Point(MouseX, MouseY));
                    if (input != null)
                    {
                        if (input is CodeBlock.DataInput)
                        {
                            if (((CodeBlock.DataInput)input).Connected != null)
                            {
                                ((CodeBlock.DataInput)input).Connected.Connected.Remove(((CodeBlock.DataInput)input));
                                ((CodeBlock.DataInput)input).Connected = null;
                                NeedsRecompile = true;
                            }
                        }
                    }
                    else
                    {
                        //ShowContextMenu(MouseX, MouseY);
                    }
                }
            }
        }

        private void t_Update_Tick(object sender, EventArgs e)
        {
            if (Sequence == null) return;
            foreach (CodeBlock c in Sequence.CodeBlocks) c.Update();
            Format();
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

                    NeedsRecompile = true;
                    Format();

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
            if (Dragging != null)
            {
                Dragging.X = e.X - PointToScreen(new Point(0, 0)).X;
                Dragging.Y = e.Y - PointToScreen(new Point(0, 0)).Y;
            }
        }
    }
}
