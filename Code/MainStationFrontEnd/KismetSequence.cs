﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MainStationFrontEnd
{

    public class KismetSequence
    {
        public EEPROM.Device.Event Event = null;
        public CodeBlock root = null;
        public List<CodeBlock> codeblocks = new List<CodeBlock>();
        public static float SpaceBetweenScopes = 10;
        public static float VecticalSpaceBetweenBlocks = 75;
        public static System.Drawing.Color ShadowColor = System.Drawing.Color.DarkGray;

        public List<Register> Registers = new List<Register>();

        public class Register
        {
            public Register(KismetSequence _Sequence)
            {
                Sequence = _Sequence;
            }
            KismetSequence Sequence;
            public byte Index = 0;
            public int Size = 0;
            public int references = 0;
        }

        public bool[] GetRegisterMask()
        {
            bool[] mask = new bool[MainStation.KismetRegisterCount];//registers used
            foreach (Register r in Registers)
            {
                for (int i = r.Index; i < r.Index + r.Size; i++)
                {
                    mask[i - MainStation.EPBufferSize] = true;
                }
            }
            return mask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Count">the amount of registers you want to alloc</param>
        /// <returns></returns>
        public Register GetRegister(int _Count)
        {
            bool[] mask = GetRegisterMask();
            for (int i = 0; i < mask.Length - _Count; i++)
            {
                bool found = true;
                for (int j = 0; j < _Count; j++)
                {
                    if (mask[i + j]) { found = false; break; }
                }
                if (found)
                {
                    Register r = new Register(this);
                    r.Size = _Count;
                    r.Index = (byte)(i + MainStation.EPBufferSize);
                    Registers.Add(r);
                    return r;
                }
            }
            throw new Exception("failed to alloc register");
        }

        public bool CheckForErrors()
        {
            foreach (CodeBlock c in codeblocks)
            {
                foreach (CodeBlock.Input i in c.Inputs)
                {
                    if (i.Connected == null) return false;
                }
            }
            return true;
        }

        public KismetSequence(EEPROM.Device.Event _Event)
        {
            Event = _Event;
            root = new BlockLocalEvent(this);
            root.SetValues(string.Format("{0} {1}", _Event.device.device.ID, _Event.eventtype.ID));
            if (!codeblocks.Contains(root)) codeblocks.Add(root);
        }

        public KismetSequence(EEPROM.Device.Event _Event, CodeBlock _Root)
        {
            Event = _Event;
            root = _Root;
            codeblocks.Add(root);
        }

        public void Connect(CodeBlock.Output _Out, CodeBlock.Input _In)
        {
            //check for illegal connect attempts ( for example between two parallel ifstatements )
            foreach (CodeBlock.Input i in _In.Owner.Inputs)
            {
                if (i == _In || i.Connected == null) continue;
                CodeBlock[] ascope = i.Connected.Owner.GetParentScopes();
                CodeBlock[] bscope = _Out.Owner.GetParentScopes();
                for (int idx = 0; idx < Math.Min(ascope.Length, bscope.Length); idx++)
                {
                    if (ascope[idx] != bscope[idx]) return;
                }
            }

            //only allow connections of the right datatypes
            if (_Out.datatype == null && _In.datatype != null) return;
            if (_Out.datatype != null && _In.datatype != null && _Out.datatype.ID != _In.datatype.ID) return;

            //remove any connection that will be overriden
            foreach (CodeBlock b in codeblocks) foreach (CodeBlock.Output o in b.Outputs) foreach (CodeBlock.Input i in o.Connected.ToArray()) if (i == _In) o.Connected.Remove(i);

            _Out.Connected.Add(_In);
            _In.Connected = _Out;
            if (!codeblocks.Contains(_Out.Owner)) codeblocks.Add(_Out.Owner);
            if (!codeblocks.Contains(_In.Owner)) codeblocks.Add(_In.Owner);
            _In.Owner.Sequence = _Out.Owner.Sequence = this;
            root.Scope = root;
            root.UpdateScope();
            FixIndices();
        }

        public CodeBlock[] GetChildrenInScope(CodeBlock _Scope)
        {
            List<CodeBlock> found = new List<CodeBlock>();
            foreach (CodeBlock c in codeblocks)
            {
                if (c.Scope == _Scope) found.Add(c);
            }
            return found.ToArray();
        }

        public void FixIndices()
        {
            root.index = 0;
            int idx = 1;
            root.FixIndices(ref idx);
        }

        public byte[] GetByteCode()
        {
            if (!CheckForErrors()) throw new Exception("failed to compile kismet sequence ( are all inputs connected ? )");
            FixIndices();
            CodeBlock[] blockssorted = new CodeBlock[codeblocks.Count];
            foreach (CodeBlock b in codeblocks) { blockssorted[b.index] = b; }

            //heuristic to determine register indices
            Registers.Clear();
            foreach (CodeBlock b in blockssorted)
            {
                foreach (CodeBlock.Output r in b.Outputs)
                {
                    if (r.datatype != null)
                    {
                        r.Register = GetRegister(r.datatype.RegistersNeeded);
                        r.Register.references = r.Connected.Count;
                    }
                }
                foreach (CodeBlock.Input r in b.Inputs)
                {
                    if (r.Connected != null)
                    {
                        if (r.Connected.Register != null)
                        {
                            r.Connected.Register.references--;
                            if (r.Connected.Register.references == 0)
                            {
                                Registers.Remove(r.Connected.Register);
                            }
                        }
                    }
                }
            }

            byte addr = 0;
            foreach (CodeBlock b in blockssorted)
            {
                b.Assamble();
                b.address = addr;
                addr += (byte)b.Code.Length;
            }

            List<byte> output = new List<byte>();
            foreach (CodeBlock b in blockssorted)
            {
                b.Assamble();//reassamble code ( so branching blocks know where to jump to )
                output.AddRange(b.Code);
            }
            output.Add(0);//add a zero instruction, this is a return aka stop the event
            Console.WriteLine("Sequence used {0} bytes", output.Count);
            return output.ToArray();
        }
    }
}
