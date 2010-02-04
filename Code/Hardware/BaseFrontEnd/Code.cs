using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace BaseFrontEnd
{
    public class CodeBlock
    {
        public CodeBlock(byte _CodeSize, byte _BlockID)
        {
            Code = new byte[_CodeSize];
            BlockID = _BlockID;
        }
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
        public byte[] Code = null;
        public int index = 0;
        public byte address;

        public virtual void SetValues(string _Values)
        {

        }

        public virtual string GetValues()
        {
            return "";
        }

        public virtual void DisAssamble(byte[] _ByteCode)
        {

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

        public class Input
        {
            public Input(CodeBlock _Owner) { Owner = _Owner; }
            public Output Connected = null;
            public CodeBlock Owner;
        }
        public class Output
        {
            public Output(CodeBlock _Owner) { Owner = _Owner; }
            public List<Input> Connected = new List<Input>();
            public CodeBlock Owner;
            public byte RegisterIndex;
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

        }

        public static Dictionary<byte, Type> CodeBlocks = null;
        public static void Initialize()
        {
            if (CodeBlocks == null)
            {
                CodeBlocks = new Dictionary<byte, Type>();
                CodeBlocks.Add(0, typeof(PushEvent));
                CodeBlocks.Add(1, typeof(ConstantByte));
                CodeBlocks.Add(2, typeof(SetDebugLed1));
                CodeBlocks.Add(3, typeof(SetDebugLed2));
                CodeBlocks.Add(5, typeof(Compare));

                CodeBlocks.Add(6, typeof(GetHour));
                CodeBlocks.Add(7, typeof(GetMinute));
                CodeBlocks.Add(8, typeof(GetSecond));
                CodeBlocks.Add(9, typeof(GetDay));

                CodeBlocks.Add(10, typeof(BlockAdd));
                CodeBlocks.Add(11, typeof(BlockSubstract));
                CodeBlocks.Add(12, typeof(BlockMultiply));
                CodeBlocks.Add(13, typeof(BlockDivide));

                CodeBlocks.Add(14, typeof(ConstantWeekDay));

                CodeBlocks.Add(15, typeof(BlockBitMask));
            }
        }
    }
}
