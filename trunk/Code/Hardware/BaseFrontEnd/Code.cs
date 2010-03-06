﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace BaseFrontEnd
{
    public class CodeBlock
    {
        public CodeBlock(KismetSequence _Sequence, byte _BlockID)
        {
            Sequence = _Sequence;
            BlockID = _BlockID;
        }

        public KismetSequence Sequence;
        public bool NeedsTriggerIn;
        public byte BlockID;

        public List<Input> Inputs = new List<Input>();
        public List<Output> Outputs = new List<Output>();

        List<int> InputRegisters = new List<int>();

        /// <summary>
        /// indicates the codeblock that defines the scope this codeblock is in
        /// </summary>
        public CodeBlock Scope = null;

        //editor stuff
        public float targetX;
        public float targetY;
        public float X;
        public float Y;
        public float width = 200;
        public float height = 100;
        [Browsable(false)]
        public float Width { get { return width; } }
        [Browsable(false)]
        public float Height { get { return height; } }

        //used for assambling
        public byte[] Code = new byte[] { };
        public int index = 0;
        public byte address;
        /// <summary>
        /// branch this block is connected to ( events/ifstatements are branches of some kind )
        /// </summary>

        public bool IsScope = false;
        public int ScopeDepth = 0;

        public virtual void SetValues(string _Values)
        {

        }

        public virtual string GetValues()
        {
            return "";
        }

        public float GetAvarageParentHeight()
        {
            float h = 0;
            int c = 0;
            foreach (Input i in Inputs)
            {
                if (i.Connected != null) { h += i.Connected.Owner.Y; c++; }
            }
            if (c > 0)
                return h / c;
            else
                return Y;
        }

        public void UpdateScope()
        {
            int bestdepth = 0;
            CodeBlock bestscope = null;

            foreach (Input i in Inputs)
            {
                if (i.Connected != null)
                {
                    CodeBlock pscope = i.Connected.Owner.IsScope ? i.Connected.Owner : i.Connected.Owner.Scope;
                    if (pscope != null)
                    {
                        if (bestscope == null || pscope.ScopeDepth > bestdepth)
                        {
                            bestscope = pscope;
                        }
                    }
                }
            }

            Scope = bestscope;
            if (Scope != null) ScopeDepth = Scope.ScopeDepth;
            if (IsScope)
            {
                if (Scope != null)
                {
                    ScopeDepth = Scope.ScopeDepth + 1;
                }
            }

            foreach (Output o in Outputs) foreach (Input i in o.Connected) i.Owner.UpdateScope();
        }

        public CodeBlock GetChildWithHighestIndex()
        {
            CodeBlock c = this;
            foreach (Output o in Outputs)
            {
                foreach (Input i in o.Connected)
                {
                    CodeBlock f = i.Owner.GetChildWithHighestIndex();
                    if (f.index > c.index) c = f;
                }
            }
            return c;
        }

        public void Update()
        {
            X -= (X - targetX) / 2;
            Y -= (Y - targetY) / 2;
        }

        public void DrawShape(Graphics _Graphics, params PointF[] _Points)
        {
            for (int i = 0; i < _Points.Length; i++)
            {
                _Points[i].X += X;
                _Points[i].Y += Y;
            }
            _Graphics.FillPolygon(new SolidBrush(Color.FromArgb(150, 150, 255)), _Points);
            _Graphics.DrawPolygon(new Pen(Brushes.Black, 2), _Points);
        }

        public void DrawCircle(Graphics _Graphics, PointF _Position, PointF _Size)
        {
            _Graphics.FillEllipse(new SolidBrush(Color.FromArgb(150, 150, 255)), new RectangleF(X - _Size.X / 2, Y - _Size.Y / 2, _Size.X, _Size.Y));
            _Graphics.DrawEllipse(new Pen(Brushes.Black, 2), new RectangleF(X - _Size.X / 2, Y - _Size.Y / 2, _Size.X, _Size.Y));
        }

        public void DrawBranch(Graphics _Graphics)
        {
            _Graphics.DrawRectangle(new Pen(Brushes.Black, 2), X, Y - height / 2, width / 2, height);

            DrawShape(_Graphics,
                new Point(-50, 10),
                new Point(-50, -10),
                new Point(0, -20),
                new Point(50, -10),
                new Point(50, 10),
                new Point(0, 20));
        }

        public void UpdateConnectors()
        {
            foreach (Input i in Inputs) i.UpdatePosition();
            foreach (Output o in Outputs) o.UpdatePosition();
        }

        public virtual void Assamble()
        {

        }

        public int GetDepth()
        {
            return getdepth(0);
        }

        private int getdepth(int curdepth)
        {
            int depth = curdepth;
            foreach (Input i in Inputs)
            {
                if (i.Connected != null)
                    depth = Math.Max(depth, i.Connected.Owner.getdepth(curdepth + 1));
            }
            return depth;
        }

        private static int CompareCodeBlockByTargetHeight(CodeBlock A, CodeBlock B)
        {
            if (A.GetTargetHeight() < B.GetTargetHeight()) return -1; else return 1;
        }
#if true
        public CodeBlock[] GetSibblings(CodeBlock[] _BlocksInGraph)
        {
            List<CodeBlock> blocksfound = new List<CodeBlock>();
            int depth = GetDepth();
            foreach (CodeBlock b in _BlocksInGraph)
            {
                if (b.GetDepth() == depth) blocksfound.Add(b);
            }
            return blocksfound.ToArray();
        }
#else
        public CodeBlock[] GetSibblings(CodeBlock[] _BlocksInGraph)
        {
            List<CodeBlock> blocksfound = new List<CodeBlock>();
            int depth = GetDepth();

            foreach (CodeBlock b in _BlocksInGraph)
            {
                if (b.GetDepth() == depth) blocksfound.Add(b);
            }
            //add parents children
            foreach (Output o in Outputs)
            {
                foreach (Input i in o.Connected)
                {
                    foreach (Input c in i.Owner.Inputs)
                    {
                        if (c.Connected != null)
                        {
                            if (!blocksfound.Contains(c.Connected.Owner)) blocksfound.Add(c.Connected.Owner);
                        }
                    }
                }
            }

            blocksfound.Sort(CompareCodeBlockByTargetHeight);
            return blocksfound.ToArray();
        }
#endif
        public float GetTargetHeight()
        {
            float outcount = 0;
            float outheight = 0;
            foreach (Output o in Outputs)
            {
                foreach (Input c in o.Connected)
                {
                    outheight += c.GetPosition().Y;
                    outcount++;
                }
            }
            if (outcount > 0)
                return (outheight / outcount);
            else
                return 0;
        }

        public class Input
        {
            public Input(CodeBlock _Owner, string _Text)
            {
                Owner = _Owner;
                X = -Owner.width / 2;
                Text = _Text;
            }
            public string Text;

            public System.Windows.Forms.ToolTip tooptip = new System.Windows.Forms.ToolTip();

            //position in codeblock
            public float X;
            public float Y;
            public Output Connected = null;
            public CodeBlock Owner;

            public void UpdatePosition()
            {
                float cnt = Owner.Inputs.Count;
                float idx = (float)Owner.Inputs.IndexOf(this) - ((cnt - 1) / 2);
                Y = (int)((idx / cnt) * Owner.height);
            }

            public System.Drawing.PointF GetPosition()
            {
                return new System.Drawing.PointF(Owner.X + X, Owner.Y + Y);
            }
        }
        public class Output
        {
            public Output(CodeBlock _Owner, string _Text) { Owner = _Owner; X = Owner.width / 2; Text = _Text; }

            public string Text;
            public System.Windows.Forms.ToolTip tooptip = new System.Windows.Forms.ToolTip();

            //position in codeblock
            public float X;
            public float Y;
            public List<Input> Connected = new List<Input>();
            public CodeBlock Owner;
            public byte RegisterIndex;

            public void UpdatePosition()
            {
                float cnt = Owner.Outputs.Count;
                float idx = (float)Owner.Outputs.IndexOf(this) - ((cnt - 1) / 2);
                Y = (int)((idx / cnt) * Owner.height);
            }

            public System.Drawing.PointF GetPosition()
            {
                return new System.Drawing.PointF(Owner.X + X, Owner.Y + Y);
            }
        }

        public CodeBlock[] GetAllChildren()
        {
            List<CodeBlock> list = new List<CodeBlock>();

            foreach (Output i in Outputs)
            {
                foreach (Input b in i.Connected)
                {
                    if (!list.Contains(b.Owner)) list.Add(b.Owner);
                    foreach (CodeBlock c in b.Owner.GetAllChildren())
                    {
                        if (!list.Contains(c)) list.Add(c);
                    }
                }
            }

            return list.ToArray();
        }

        public void ConcatenateSizeWithChildren()
        {
            height = 50;
            width = 200;
            foreach (CodeBlock c in GetAllChildren())
            {
                height = Math.Max(height, (Math.Abs(c.Y - Y) + (c.height / 2)) * 2);
                width = Math.Max(width, (c.X - X + c.width / 2) * 2);
            }
            height += 10;
            width += 10;
        }

        /// <summary>
        /// returns a list of codeblocks this code block is dependent on, also this codeblock can never connect anything to one of these codeblocks
        /// </summary>
        /// <returns></returns>
        public CodeBlock[] GetDependencies()
        {
            List<CodeBlock> list = new List<CodeBlock>();
            list.Add(this);
            foreach (Input i in Inputs)
            {
                if (i.Connected != null)
                {
                    if (!list.Contains(i.Connected.Owner)) list.Add(i.Connected.Owner);
                    foreach (CodeBlock c in i.Connected.Owner.GetDependencies())
                    {
                        if (!list.Contains(c)) list.Add(c);
                    }
                }
            }

            return list.ToArray();
        }

        public virtual void Draw(System.Drawing.Graphics _Graphics)
        {
#if true
            _Graphics.DrawString("idx:" + index.ToString(), new System.Drawing.Font("Arial", 8), System.Drawing.Brushes.Black, X, Y - 25);
            _Graphics.DrawString("depth:" + GetDepth().ToString(), new System.Drawing.Font("Arial", 8), System.Drawing.Brushes.Black, X, Y - 35);
            if (Scope != null) _Graphics.DrawString("scope:" + Scope.ToString(), new System.Drawing.Font("Arial", 8), System.Drawing.Brushes.Black, X, Y - 45);
#endif
        }

        public class Prototype
        {
            public string BlockName;
            public string GroupName;
            public bool UserCanAdd;
            public Type Type;

            public Prototype(string _BlockName, string _GroupName, Type _Type, bool _UserCanAdd)
            {
                BlockName = _BlockName;
                GroupName = _GroupName;
                Type = _Type;
                UserCanAdd = _UserCanAdd;
            }
        }

        public static List<Prototype> CodeBlocks = null;
        public static void Initialize()
        {
            if (CodeBlocks == null)
            {
                CodeBlocks = new List<Prototype>();

                AddCodeBlock("Event", "", typeof(DefaultEvent), false);

                //Constants
                AddCodeBlock("weekday", "Contant", typeof(BlockConstantWeekDay));
                AddCodeBlock("byte", "Contant", typeof(BlockConstantByte));

                //Debug stuff
                AddCodeBlock("Set DebugLed 1", "Debug stuff", typeof(BlockSetDebugLed1));
                AddCodeBlock("Set DebugLed 2", "Debug stuff", typeof(BlockSetDebugLed2));

                //compare
                AddCodeBlock("Equals", "Compare", typeof(BlockEquals));
                AddCodeBlock("Differs", "Compare", typeof(BlockDiffers));
                AddCodeBlock("Smaller Than", "Compare", typeof(BlockSmallerThan));
                AddCodeBlock("Larget Than", "Compare", typeof(BlockLargerThan));

                //Time stuff
                AddCodeBlock("Get current hour", "Time", typeof(BlockGetHour));
                AddCodeBlock("Get current minute", "Time", typeof(BlockGetMinute));
                AddCodeBlock("Get current second", "Time", typeof(BlockGetSecond));
                AddCodeBlock("Get current weekday", "Time", typeof(BlockGetDay));

                //math
                AddCodeBlock("Add", "Math", typeof(BlockAdd));
                AddCodeBlock("Substract", "Math", typeof(BlockSubstract));
                AddCodeBlock("Multiply", "Math", typeof(BlockMultiply));
                AddCodeBlock("Divide", "Math", typeof(BlockDivide));
                AddCodeBlock("Bitwise And", "Math", typeof(BlockBitMask));

                //Variable
                AddCodeBlock("Set variable", "Variable", typeof(BlockSetVariable));
                AddCodeBlock("Get variable", "Variable", typeof(BlockGetVariable));

                //Branching
                AddCodeBlock("If", "Branches", typeof(BlockIf));
                AddCodeBlock("If not", "Branches", typeof(BlockIfNot));

            }
        }

        public static Prototype GetCodeBlock(Type _Type)
        {
            Initialize();
            foreach (Prototype p in CodeBlocks)
            {
                if (p.Type == _Type) return p;
            }
            return null;
        }

        private static void AddCodeBlock(string _BlockName, string _Group, Type _Type)
        {
            CodeBlocks.Add(new Prototype(_BlockName, _Group, _Type, true));
        }

        private static void AddCodeBlock(string _BlockName, string _Group, Type _Type, bool _UserCanAdd)
        {
            CodeBlocks.Add(new Prototype(_BlockName, _Group, _Type, _UserCanAdd));
        }
    }
}
