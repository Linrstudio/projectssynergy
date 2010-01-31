using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace BaseFrontEnd
{
    public class CodeBlock
    {
        public CodeBlock(byte _CodeSize)
        {
            Code = new byte[_CodeSize];
        }
        public bool NeedsTriggerIn;

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
    }

    public static class CodeAssambler
    {
        public static byte[] Assamble(CodeBlock _Head)
        {
            List<CodeBlock> blocks = new List<CodeBlock>(_Head.GetAllChildren());
            blocks.Add(_Head);
            for (int i = 0; i < blocks.Count; i++) blocks[i].index = i + 1;
            _Head.index = 0;

            bool changed = true;
            while (changed)
            {
                changed = false;

                foreach (CodeBlock b in blocks)
                {

                    foreach (CodeBlock d in b.GetDependencies())
                    {
                        if (d.index > b.index)
                        {
                            int t = d.index;
                            d.index = b.index;
                            b.index = t;
                        }
                    }

                }
            }
            CodeBlock[] blockssorted = new CodeBlock[blocks.Count];
            foreach (CodeBlock b in blocks) { blockssorted[b.index] = b; }
            byte addr = 0;
            foreach (CodeBlock b in blockssorted)
            {
                b.Assamble();
                b.address = addr;
                addr += (byte)b.Code.Length;
            }


            //heuristic to determine register indices
            byte registeridx = 0;
            foreach (CodeBlock b in blockssorted)
            {
                foreach (CodeBlock.Output r in b.Outputs)
                {
                    r.RegisterIndex = registeridx;
                    registeridx++;
                }
            }

            foreach (CodeBlock b in blockssorted)
            {
                b.Assamble();
            }

            List<byte> output = new List<byte>();

            foreach (CodeBlock b in blockssorted)
            {
                output.AddRange(b.Code);
            }
            output.Add(0);//add a zero instruction, this is a return aka stop the event

            return output.ToArray();
        }

    }
}
