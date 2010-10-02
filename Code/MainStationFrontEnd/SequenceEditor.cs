using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MainStationFrontEnd
{
    public partial class SequenceEditWindow : UserControl
    {
        public KismetSequence Sequence = null;
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
        }

        private void SequenceEditWindow_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.EnableNotifyMessage | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        public CodeBlock.Input GetNearestInput(PointF _pos)
        {
            if (Sequence == null) return null;

            float bestdist = 20;
            CodeBlock.Input best = null;
            foreach (CodeBlock d in Sequence.codeblocks)
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
            foreach (CodeBlock d in Sequence.codeblocks)
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

        private void DrawPwettyLine(Graphics g, PointF A, PointF B)
        {
            DrawPwettyLine(g, A, B, 1.5f, System.Drawing.Color.Black);
        }

        private void DrawPwettyLineShadow(Graphics g, PointF A, PointF B)
        {
            DrawPwettyLine(g, A, B, 1.5f, KismetSequence.ShadowColor);
        }

        private void DrawPwettyLine(Graphics g, PointF A, PointF B, float _Width, System.Drawing.Color _Color)
        {
            B.X -= 15;
            PointF c = new PointF((A.X + B.X) / 2, (A.Y + B.Y) / 2);
            g.DrawBezier(new Pen(new SolidBrush(_Color), _Width), A.X, A.Y, c.X, A.Y, c.X, B.Y, B.X, B.Y);
            g.FillPolygon(new SolidBrush(_Color), new PointF[]{
                new PointF(B.X + 15, B.Y),
                new PointF(B.X, B.Y - 5),
                new PointF(B.X, B.Y + 5)
            }, System.Drawing.Drawing2D.FillMode.Alternate);
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
                foreach (CodeBlock b in Sequence.codeblocks)
                {
                    b.DrawShadow(e.Graphics);
                    foreach (CodeBlock.Input i in b.Inputs)
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
                foreach (CodeBlock b in Sequence.codeblocks)
                {
                    List<CodeBlock> dependencies = new List<CodeBlock>();
                    if (SelectedOutput != null) dependencies.AddRange(SelectedOutput.Owner.GetDependencies());

                    CodeBlock.Input input = GetNearestInput(new Point(MouseX, MouseY));
                    CodeBlock.Output output = GetNearestOutput(new Point(MouseX, MouseY));

                    //e.Graphics.DrawRectangle(Pens.Red, b.x, b.y, 100, 50);
                    float p = 0;
                    foreach (CodeBlock.Input i in b.Inputs)
                    {
                        p += 1 / (float)(b.Inputs.Count + 1);
                        PointF pos = i.GetPosition();
                        if (!dependencies.Contains(i.Owner))
                            e.Graphics.FillRectangle(new SolidBrush(i.datatype == null ? Color.Black : i.datatype.Color), new RectangleF(pos.X - 5, pos.Y - 5, 5, 10));

                        if (i.Connected != null)
                        {
                            PointF x = i.GetPosition();
                            PointF y = i.Connected.GetPosition();
                            DrawPwettyLine(e.Graphics, y, x, 1.5f, i.datatype == null ? Color.Black : i.datatype.Color);
                        }

                        if (i == input)
                        {
                            i.tooptip.Show(i.Text, this, new Point((int)pos.X, (int)pos.Y), 1000);
                        }
                    }
                    p = 0;
                    foreach (CodeBlock.Output i in b.Outputs)
                    {
                        p += 1 / (float)(b.Inputs.Count + 1);
                        PointF pos = i.GetPosition();
                        if (!dependencies.Contains(i.Owner))
                            e.Graphics.FillRectangle(new SolidBrush(i.datatype != null ? i.datatype.Color : Color.Black), new RectangleF(pos.X, pos.Y - 5, 5, 10));

                        if (i == output)
                        {
                            i.tooptip.Show(i.Text, this, new Point((int)pos.X, (int)pos.Y), 1000);
                        }
                    }
                    b.Draw(e.Graphics);
                }
                if (SelectedOutput != null && SelectedInput != null)

                    DrawPwettyLine(e.Graphics, SelectedOutput.GetPosition(), SelectedInput.GetPosition(), 2, SelectedInput.datatype == null ? Color.Black : SelectedInput.datatype.Color);
                else
                {
                    if (SelectedInput != null)//input to mouse
                        DrawPwettyLine(e.Graphics, new Point(MouseX, MouseY), SelectedInput.GetPosition(), 2, SelectedInput.datatype == null ? Color.Black : SelectedInput.datatype.Color);
                    if (SelectedOutput != null)//output to mouse
                        DrawPwettyLine(e.Graphics, SelectedOutput.GetPosition(), new Point(MouseX, MouseY), 2, SelectedOutput.datatype == null ? Color.Black : SelectedOutput.datatype.Color);
                }
            }
            base.OnPaint(e);
        }

        public void UpdateFloatingBlocks()
        {
            FloatingBlocks.Clear();
            foreach (CodeBlock b in Sequence.codeblocks)
            {
                if (b == Sequence.root) continue;
                bool found = true;
                foreach (CodeBlock.Input i in b.Inputs)
                {
                    if (i.Connected != null) found = false;
                }
                foreach (CodeBlock.Output o in b.Outputs)
                {
                    if (o.Connected.Count > 0) found = false;
                }
                if (found) FloatingBlocks.Add(b);
            }
        }

        public void Format()
        {
            if (Sequence == null) return;


            //if (NeedsRecompile) eeprom.Assamble();
            NeedsRecompile = false;
#if true
            foreach (CodeBlock c in Sequence.codeblocks)
            {
                if (c == Sequence.root) continue;
                //c.targetX = 100;
                //c.targetY = 100;
            }
            Sequence.root.targetX = 100;
            Sequence.root.UpdateScope();
            Sequence.root.UpdateLayout();
#else
            int siblingdist = 150;
            foreach (CodeBlock b in Sequence.codeblocks)
            {
                if (b == Sequence.root) continue;
                b.targetX = 100 + b.GetDepth() * 150;
                List<CodeBlock> siblings = new List<CodeBlock>(b.GetSibblings(Sequence.codeblocks.ToArray()));
                int idx = siblings.IndexOf(b);
                b.targetY = idx * siblingdist;
                b.targetY -= (float)(siblings.Count-1) * (float)(siblingdist / 2);
                b.targetY += b.GetAvarageParentHeight();
            }
#endif
            Sequence.root.UpdateLayout();
            Width = (int)Math.Max(Parent.Bounds.Width, Sequence.root.targetX + Sequence.root.width / 2 + 20);
            Height = (int)Math.Max(Parent.Bounds.Height, Sequence.root.height + 100);
            Sequence.root.targetY = Height / 2;

            UpdateFloatingBlocks();
            int floatx = 100;
            foreach (CodeBlock b in FloatingBlocks)
            {
                b.targetX = floatx;
                b.targetY = 100;
                floatx += 150;
            }

            Refresh();
        }

        public void MouseEvent(MouseEventArgs e, bool _LClick)
        {
            MouseX = e.X;
            MouseY = e.Y;

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
                    if (nearestout != null && SelectedBlock != nearestout.Owner) { SelectedBlock = nearestout.Owner; if (OnBlockSelect != null)OnBlockSelect(SelectedBlock); }
                    if (nearestin != null && SelectedBlock != nearestin.Owner) { SelectedBlock = nearestin.Owner; if (OnBlockSelect != null)OnBlockSelect(SelectedBlock); }
                }
                else { SelectedBlock = null; }
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
            }
            //Format();
        }

        public void OnContextMenuItemClicked(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;

            if (item.Tag is string)
            {
                CodeBlock b = new BlockRemoteEvent(Sequence);
                b.SetValues((string)item.Tag);
                Sequence.codeblocks.Add(b);
            }
            if (item.Tag is Type)
            {
                foreach (CodeBlock.Prototype p in CodeBlock.CodeBlocks)
                {
                    if (p.Type == item.Tag)
                    {
                        Sequence.codeblocks.Add((CodeBlock)p.Type.GetConstructor(new Type[] { typeof(KismetSequence) }).Invoke(new object[] { Sequence }));
                        NeedsRecompile = true;
                        Format();
                    }
                }
            }
        }

        public void ShowContextMenu(int x, int y)
        {
            Dictionary<string, MenuItem> menuitems = new Dictionary<string, MenuItem>();

            CodeBlock.Initialize();
            foreach (CodeBlock.Prototype p in CodeBlock.CodeBlocks)
            {
                if (p.UserCanAdd)
                {
                    if (!menuitems.ContainsKey(p.GroupName))
                        menuitems.Add(p.GroupName, new MenuItem(p.GroupName));

                    MenuItem item = new MenuItem(p.BlockName, OnContextMenuItemClicked);
                    item.Tag = p.Type;
                    menuitems[p.GroupName].MenuItems.Add(item);
                }
            }

            //add remoteevent blocks
            MenuItem eventitem = new MenuItem("Remote events");
            menuitems.Add("Remote events", eventitem);
            foreach (ProductDataBase.Device d in ProductDataBase.Devices)
            {
                bool worthadding = false;
                MenuItem ditem = new MenuItem(d.Name);
                foreach (ProductDataBase.Device.RemoteEvent e in d.remoteevents)
                {
                    MenuItem eitem = new MenuItem(e.Name, OnContextMenuItemClicked);
                    eitem.Tag = string.Format("{0} {1} {2}", d.ID, e.ID, 0);
                    ditem.MenuItems.Add(eitem);
                    worthadding = true;
                }
                if (worthadding) eventitem.MenuItems.Add(ditem);
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
                            foreach (CodeBlock.Output o in input.Owner.Outputs) o.DisconnectAll();
                            Sequence.codeblocks.Remove(input.Owner);
                        }
                    }
                    else
                    {
                        ShowContextMenu(MouseX, MouseY);
                    }
                }
            }
        }

        private void t_Update_Tick(object sender, EventArgs e)
        {
            if (Sequence == null) return;
            foreach (CodeBlock c in Sequence.codeblocks) c.Update();
            Format();
        }
    }
}
