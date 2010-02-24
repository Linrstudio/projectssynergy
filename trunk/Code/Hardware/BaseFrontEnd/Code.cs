using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

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

        //indicates weather this codeblock will jump to the next block by itself
        public bool WillJump = false;

        public List<Input> Inputs = new List<Input>();
        public List<Output> Outputs = new List<Output>();

        List<int> InputRegisters = new List<int>();

        //editor stuff
        public int x;
        public int y;
        public int width = 200;
        public int height = 100;
        [Browsable(false)]
        public int Width { get { return width; } }
        [Browsable(false)]
        public int Height { get { return height; } }

        //used for assambling
        public byte[] Code = new byte[] { };
        public int index = 0;
        public byte address;

        public virtual void SetValues(string _Values)
        {

        }

        public virtual string GetValues()
        {
            return "";
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
            if (A.GetTargetHeight() + A.height < B.GetTargetHeight() + B.height) return -1; else return 1;
        }

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

        public int GetTargetHeight()
        {
            int outcount = 0;
            int outheight = 0;
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
                x = -Owner.width / 2;
                Text = _Text;
            }
            public string Text;

            public System.Windows.Forms.ToolTip tooptip = new System.Windows.Forms.ToolTip();

            //position in codeblock
            public int x;
            public int y;
            public Output Connected = null;
            public CodeBlock Owner;

            public void UpdatePosition()
            {
                float cnt = Owner.Inputs.Count;
                float idx = (float)Owner.Inputs.IndexOf(this) - ((cnt - 1) / 2);
                y = (int)((idx / cnt) * Owner.height);
            }

            public System.Drawing.Point GetPosition()
            {
                return new System.Drawing.Point(Owner.x + x, Owner.y + y);
            }
        }
        public class Output
        {
            public Output(CodeBlock _Owner, string _Text) { Owner = _Owner; x = Owner.width / 2; Text = _Text; }

            public string Text;
            public System.Windows.Forms.ToolTip tooptip = new System.Windows.Forms.ToolTip();

            //position in codeblock
            public int x;
            public int y;
            public List<Input> Connected = new List<Input>();
            public CodeBlock Owner;
            public byte RegisterIndex;

            public void UpdatePosition()
            {
                float cnt = Owner.Outputs.Count;
                float idx = (float)Owner.Outputs.IndexOf(this) - ((cnt - 1) / 2);
                y = (int)((idx / cnt) * Owner.height);
            }

            public System.Drawing.Point GetPosition()
            {
                return new System.Drawing.Point(Owner.x + x, Owner.y + y);
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

        public CodeBlock[] GetDependencies()
        {
            List<CodeBlock> list = new List<CodeBlock>();

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
            //_Graphics.DrawString(index.ToString(), new System.Drawing.Font("Arial", 8), System.Drawing.Brushes.Black, x, y - 25);
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

                AddCodeBlock("Event", "", typeof(PushEvent), false);

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
