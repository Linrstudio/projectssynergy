using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MainStationFrontEnd
{
    public class KismetSequence
    {
        public List<CodeBlock> codeblocks = new List<CodeBlock>();
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

        public void Clear()
        {
            codeblocks.Clear();
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

        public KismetSequence() { }

        public void Select(params CodeBlock[] _CodeBlocks)
        {
            foreach (CodeBlock b in codeblocks) b.Selected = false;
            foreach (CodeBlock b in _CodeBlocks) b.Selected = true;
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

        public void Remove(CodeBlock _CodeBlock)
        {
            _CodeBlock.DisconnectAllInputs();
            _CodeBlock.DisconnectAllOutputs();
            codeblocks.Remove(_CodeBlock);
        }

        public void Load(XElement _Sequence)
        {
            codeblocks.Clear();
            try
            {
                foreach (XElement block in _Sequence.Elements("Block"))
                {
                    string blocktype = (string)block.Attribute("Type").Value;
                    byte index = byte.Parse(block.Attribute("GUID").Value);
                    CodeBlock b = (CodeBlock)Type.GetType(blocktype).GetConstructor(new Type[] { typeof(KismetSequence) }).Invoke(new object[] { this });
                    b.index = index;
                    b.X = float.Parse(block.Attribute("X").Value);
                    b.Y = float.Parse(block.Attribute("Y").Value);
                    b.SetValues(block.Attribute("Values").Value);
                    codeblocks.Add(b);
                }

                foreach (XElement connection in _Sequence.Elements("Connect"))
                {
                    int input = int.Parse(connection.Attribute("Input").Value);
                    int inputowner = int.Parse(connection.Attribute("InputOwner").Value);
                    int output = int.Parse(connection.Attribute("Output").Value);
                    int outputowner = int.Parse(connection.Attribute("OutputOwner").Value);
                    Connect(codeblocks[outputowner].Outputs[output], codeblocks[inputowner].Inputs[input]);
                }
            }
            catch { //FIXME 
            }
        }

        public XElement Save()
        {
            XElement sequence = new XElement("Sequence");
            foreach (CodeBlock b in codeblocks)
            {
                XElement block = null;
                block = new XElement("Block");
                block.SetAttributeValue("GUID", b.index);
                block.SetAttributeValue("X", b.X);
                block.SetAttributeValue("Y", b.Y);
                block.SetAttributeValue("Type", CodeBlock.GetCodeBlock(b.GetType()).Type.FullName);
                block.SetAttributeValue("Values", b.GetValues());
                sequence.Add(block);
            }
            foreach (CodeBlock b in codeblocks)
            {
                foreach (CodeBlock.Input i in b.Inputs)
                {
                    if (i.Connected != null)
                    {
                        XElement connection = new XElement("Connect");
                        connection.SetAttributeValue("Input", i.Owner.Inputs.IndexOf(i).ToString());
                        connection.SetAttributeValue("InputOwner", codeblocks.IndexOf(i.Owner).ToString());
                        connection.SetAttributeValue("Output", i.Connected.Owner.Outputs.IndexOf(i.Connected).ToString());
                        connection.SetAttributeValue("OutputOwner", codeblocks.IndexOf(i.Connected.Owner).ToString());
                        sequence.Add(connection);
                    }
                }
            }
            return sequence;
        }
    }
}
