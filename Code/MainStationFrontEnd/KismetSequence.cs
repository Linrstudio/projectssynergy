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
        List<CodeBlock> codeblocks = new List<CodeBlock>();
        public CodeBlock[] CodeBlocks { get { return codeblocks.ToArray(); } }
        public void AddCodeBlock(CodeBlock _CodeBlock) { if (codeblocks.Contains(_CodeBlock))return; codeblocks.Add(_CodeBlock); _CodeBlock.Sequence = this; }
        public void RemoveCodeBlock(CodeBlock _CodeBlock) { if (codeblocks.Contains(_CodeBlock)) { _CodeBlock.DisconnectAllInputs(); _CodeBlock.DisconnectAllOutputs(); codeblocks.Remove(_CodeBlock); } }
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
            throw new Exception("failed to allocate register");
        }

        XElement CompileForDesktop()
        {
            XElement sequence = new XElement("Sequence");

            foreach (CodeBlock b in codeblocks)
            {
                if (b.IsEvent)
                {
                    XElement Event = new XElement("Event");
                    XElement data = new XElement("Data");
                    b.Save(data);
                    Event.Add(data);
                    
                }
            }

            return sequence;
        }

        public KismetSequence() { }

        public void Select(params CodeBlock[] _CodeBlocks)
        {
            foreach (CodeBlock b in codeblocks) b.Selected = false;
            foreach (CodeBlock b in _CodeBlocks) b.Selected = true;
        }

        public void Connect(CodeBlock.Output _Out, CodeBlock.Input _In)
        {
            if (_Out is CodeBlock.TriggerOutput && _In is CodeBlock.TriggerInput)
            {
                CodeBlock.TriggerOutput Out = (CodeBlock.TriggerOutput)_Out;
                CodeBlock.TriggerInput In = (CodeBlock.TriggerInput)_In;
                if (!Out.Connected.Contains(In)) Out.Connected.Add(In);
                if (!In.Connected.Contains(Out)) In.Connected.Add(Out);

                if (!codeblocks.Contains(Out.Owner)) codeblocks.Add(Out.Owner);
                if (!codeblocks.Contains(In.Owner)) codeblocks.Add(In.Owner);
                In.Owner.Sequence = Out.Owner.Sequence = this;
            }

            if (_Out is CodeBlock.DataOutput && _In is CodeBlock.DataInput)
            {
                CodeBlock.DataOutput Out = (CodeBlock.DataOutput)_Out;
                CodeBlock.DataInput In = (CodeBlock.DataInput)_In;
                if (!Out.Connected.Contains(In)) Out.Connected.Add(In);

                if (In.Connected != null) In.Connected.Connected.Remove(In);// override connection
                In.Connected = Out;
                if (!codeblocks.Contains(Out.Owner)) codeblocks.Add(Out.Owner);
                if (!codeblocks.Contains(In.Owner)) codeblocks.Add(In.Owner);
                In.Owner.Sequence = Out.Owner.Sequence = this;

                //remove any connection that will be overriden
                //foreach (CodeBlock b in codeblocks) foreach (CodeBlock.DataOutput o in b.DataOutputs) foreach (CodeBlock.DataInput i in o.Connected.ToArray()) if (i == _In) o.Connected.Remove(i);
            }
        }

        public void Load(XElement _Sequence)
        {
            codeblocks.Clear();
            //try
            {
                foreach (XElement block in _Sequence.Elements("Block"))
                {
                    string blocktype = (string)block.Attribute("Type").Value;
                    CodeBlock b = (CodeBlock)Activator.CreateInstance(null, "MainStationFrontEnd." + blocktype).Unwrap();
                    b.X = float.Parse(block.Attribute("X").Value);
                    b.Y = float.Parse(block.Attribute("Y").Value);
                    try
                    {
                        b.Load(block.Element("Data"));
                    }
                    catch { /*FIXME*/ }
                    codeblocks.Add(b);
                }
            }
            //catch{ /* FIXME */}
            //try
            {
                foreach (XElement connection in _Sequence.Elements("ConnectData"))
                {
                    int input = int.Parse(connection.Attribute("Input").Value);
                    int inputowner = int.Parse(connection.Attribute("InputOwner").Value);
                    int output = int.Parse(connection.Attribute("Output").Value);
                    int outputowner = int.Parse(connection.Attribute("OutputOwner").Value);
                    Connect(codeblocks[outputowner].DataOutputs[output], codeblocks[inputowner].DataInputs[input]);
                }
                foreach (XElement connection in _Sequence.Elements("ConnectTrigger"))
                {
                    int input = int.Parse(connection.Attribute("Input").Value);
                    int inputowner = int.Parse(connection.Attribute("InputOwner").Value);
                    int output = int.Parse(connection.Attribute("Output").Value);
                    int outputowner = int.Parse(connection.Attribute("OutputOwner").Value);
                    Connect(codeblocks[outputowner].TriggerOutputs[output], codeblocks[inputowner].TriggerInputs[input]);
                }
            }
            //catch { /* FIXME */ }
        }

        public XElement Save()
        {
            XElement sequence = new XElement("Sequence");
            foreach (CodeBlock b in codeblocks)
            {
                XElement block = null;
                block = new XElement("Block");
                block.SetAttributeValue("X", b.X);
                block.SetAttributeValue("Y", b.Y);
                block.SetAttributeValue("Type", CodeBlock.GetCodeBlock(b.GetType()).Type.Name);
                XElement data =new XElement("Data");
                b.Save(data);
                block.Add(data);
                sequence.Add(block);
            }
            foreach (CodeBlock b in codeblocks)
            {
                foreach (CodeBlock.DataInput i in b.DataInputs)
                {
                    if (i.Connected != null)
                    {
                        XElement connection = new XElement("ConnectData");
                        connection.SetAttributeValue("Input", i.Owner.DataInputs.IndexOf(i).ToString());
                        connection.SetAttributeValue("InputOwner", codeblocks.IndexOf(i.Owner).ToString());
                        connection.SetAttributeValue("Output", i.Connected.Owner.DataOutputs.IndexOf(i.Connected).ToString());
                        connection.SetAttributeValue("OutputOwner", codeblocks.IndexOf(i.Connected.Owner).ToString());
                        sequence.Add(connection);
                    }
                }
                foreach (CodeBlock.TriggerOutput o in b.TriggerOutputs)
                {
                    foreach (CodeBlock.TriggerInput i in o.Connected)
                    {
                        XElement connection = new XElement("ConnectTrigger");
                        connection.SetAttributeValue("Input", i.Owner.TriggerInputs.IndexOf(i).ToString());
                        connection.SetAttributeValue("InputOwner", codeblocks.IndexOf(i.Owner).ToString());
                        connection.SetAttributeValue("Output", o.Owner.TriggerOutputs.IndexOf(o).ToString());
                        connection.SetAttributeValue("OutputOwner", codeblocks.IndexOf(o.Owner).ToString());
                        sequence.Add(connection);
                    }
                }
            }
            return sequence;
        }
    }
}
