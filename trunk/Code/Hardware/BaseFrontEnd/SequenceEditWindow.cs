﻿using System;
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
            B.X -= 15;
            PointF c = new PointF((A.X + B.X) / 2, (A.Y + B.Y) / 2);
            g.DrawBezier(new Pen(Brushes.Black, 2), A.X, A.Y, c.X, A.Y, c.X, B.Y, B.X, B.Y);
            g.FillPolygon(Brushes.Black, new PointF[]{
                new PointF(B.X + 15, B.Y),
                new PointF(B.X, B.Y - 5),
                new PointF(B.X, B.Y + 5)
            }, System.Drawing.Drawing2D.FillMode.Alternate);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Resources.Background != null)//to keep visual studio from crashing on us
            {
                e.Graphics.DrawImage(Resources.Background, new Rectangle(0, 0, Width, Height));
            }
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (Sequence != null)
            {
                foreach (CodeBlock b in Sequence.codeblocks)
                {
                    List<CodeBlock> dependencies = new List<CodeBlock>();
                    if (Selected != null) dependencies.AddRange(Selected.Owner.GetDependencies());

                    CodeBlock.Input input = GetNearestInput(new Point(MouseX, MouseY));
                    CodeBlock.Output output = GetNearestOutput(new Point(MouseX, MouseY));

                    //e.Graphics.DrawRectangle(Pens.Red, b.x, b.y, 100, 50);
                    float p = 0;
                    foreach (CodeBlock.Input i in b.Inputs)
                    {
                        p += 1 / (float)(b.Inputs.Count + 1);
                        PointF pos = i.GetPosition();
                        if (!dependencies.Contains(i.Owner))
                            e.Graphics.FillRectangle(Brushes.Black, new RectangleF(pos.X - 5, pos.Y - 5, 5, 10));

                        if (i.Connected != null)
                        {
                            PointF x = i.GetPosition();
                            PointF y = i.Connected.GetPosition();
                            DrawPwettyLine(e.Graphics, y, x);
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
                            e.Graphics.FillRectangle(Brushes.Black, new RectangleF(pos.X, pos.Y - 5, 5, 10));

                        if (i == output)
                        {
                            i.tooptip.Show(i.Text, this, new Point((int)pos.X, (int)pos.Y), 1000);
                        }
                    }
                    b.Draw(e.Graphics);
                }
                if (Selected != null)
                {
                    DrawPwettyLine(e.Graphics, Selected.GetPosition(), new Point(MouseX, MouseY));
                }
            }
            base.OnPaint(e);
        }

        public void Format()
        {
            if (Sequence == null) return;
            //if (NeedsRecompile) eeprom.Assamble();
            NeedsRecompile = false;
#if true   
            foreach (CodeBlock c in Sequence.codeblocks)
            {
                c.targetX = 100;
                c.targetY = 100;
            }
            Sequence.root.targetY = 500;
            Sequence.root.targetX = 150;
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
            Width = (int)Math.Max(Parent.Bounds.Width, Sequence.root.width + 20);
            Height = (int)Math.Max(Parent.Bounds.Height, Sequence.root.height + 100);

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
                CodeBlock.Output nearestout = GetNearestOutput(new Point(MouseX, MouseY));
                CodeBlock.Input nearestin = GetNearestInput(new Point(MouseX, MouseY));
                if (Selected == null)
                {
                    if (nearestout != null)
                    {
                        Selected = nearestout;
                        //PropertyGrid.SelectedObject = Selected.Owner;
                        NeedsRecompile = true;
                    }
                }
                //select nearest codeblock

                SelectedBlock = null;
                if (nearestout != null && SelectedBlock != nearestout.Owner) { SelectedBlock = nearestout.Owner; if (OnBlockSelect != null)OnBlockSelect(SelectedBlock); }
                if (nearestin != null && SelectedBlock != nearestin.Owner) { SelectedBlock = nearestin.Owner; if (OnBlockSelect != null)OnBlockSelect(SelectedBlock); }
            }
            if (e.Button == MouseButtons.None)
            {
                CodeBlock.Input nearest = GetNearestInput(new Point(MouseX, MouseY));
                if (Selected != null)
                {
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
                }
            }
            Format();
        }

        public void OnContextMenuItemClicked(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;

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

        public void ShowContextMenu(int x, int y)
        {
            Dictionary<string, MenuItem> menuitems = new Dictionary<string, MenuItem>();

            CodeBlock.Initialize();
            foreach (CodeBlock.Prototype p in CodeBlock.CodeBlocks)
            {
                if (p.Type.BaseType != typeof(BaseBlockEvent))
                {
                    if (!menuitems.ContainsKey(p.GroupName))
                        menuitems.Add(p.GroupName, new MenuItem(p.GroupName));

                    MenuItem item = new MenuItem(p.BlockName, OnContextMenuItemClicked);
                    item.Tag = p.Type;
                    menuitems[p.GroupName].MenuItems.Add(item);
                }
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
            MouseEvent(e);
        }

        private void SequenceEditWindow_MouseDown(object sender, MouseEventArgs e)
        {
            MouseEvent(e);

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
                            foreach (CodeBlock.Output o in input.Owner.Outputs) o.Connected.Clear();
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
            foreach (CodeBlock c in Sequence.codeblocks) c.Update();
            Format();
        }
    }
}