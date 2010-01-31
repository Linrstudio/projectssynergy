using System;
using System.Collections.Generic;
using System.Text;

namespace BaseFrontEnd
{
    public class KismetSequence
    {
        public CodeBlock root;
        public List<CodeBlock> codeblocks = new List<CodeBlock>();

        public byte[] GetByteCode()
        {
            List<CodeBlock> blocks = new List<CodeBlock>(root.GetAllChildren());
            blocks.Add(root);
            for (int i = 0; i < blocks.Count; i++) blocks[i].index = i + 1;
            root.index = 0;

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

        public KismetSequence FromByteCode(byte[] _Code)
        {
            return null;
        }
    }
}
