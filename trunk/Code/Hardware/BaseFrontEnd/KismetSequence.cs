using System;
using System.Collections.Generic;
using System.Text;

namespace BaseFrontEnd
{
    public class EEPROM
    {
        ushort Size;
        Dictionary<ushort, Device> Devices = new Dictionary<ushort, Device>();

        public class Device
        {
            public Device(EEPROM _EEPROM, ushort _ID)
            {
                eeprom = _EEPROM;
            }
            public EEPROM eeprom;
            public ushort addr = 0;
            public ushort eventaddr = 0;
            public ushort ID;
            public Dictionary<byte, Event> events = new Dictionary<byte, Event>();

            public class Event
            {
                public ushort addr = 0;
                public byte ID;

                public Event(byte _ID)
                {
                    ID = _ID;
                }

                public ushort SequenceAddr;
                public KismetSequence sequence = null;
            }
        }

        public static EEPROM FromEEPROM(byte[] _EEPROM)
        {
            EEPROM eeprom = new EEPROM();
            {
                int idx = 0;
                while (true)
                {
                    ushort ID = Utilities.ToShort(_EEPROM, idx);
                    if (ID == 0) break;
                    eeprom.Devices.Add(ID, new Device(eeprom, ID));
                    eeprom.Devices[ID].addr = (ushort)idx;
                    eeprom.Devices[ID].eventaddr = Utilities.ToShort(_EEPROM, idx + 2);
                    idx += 4;
                }
            }
            foreach (Device d in eeprom.Devices.Values)
            {
                int idx = d.eventaddr;
                while (true)
                {
                    ushort sequenceaddr = Utilities.ToShort(_EEPROM, idx + 1);
                    byte id = _EEPROM[idx];
                    if (id == 0) break;
                    d.events.Add(id, new Device.Event(id));
                    d.events[id].SequenceAddr = sequenceaddr;
                    idx += 3;
                }
            }
            foreach (Device d in eeprom.Devices.Values)
            {
                foreach (Device.Event e in d.events.Values)
                {
                    e.sequence = KismetSequence.FromByteCode(Utilities.Cut(_EEPROM, e.SequenceAddr));
                }
            }

            return eeprom;
        }
    }

    public class KismetSequence
    {
        public CodeBlock root;
        public List<CodeBlock> codeblocks = new List<CodeBlock>();

        public void Connect(CodeBlock.Output _Out, CodeBlock.Input _In)
        {
            foreach (CodeBlock b in codeblocks) foreach (CodeBlock.Output o in b.Outputs) foreach (CodeBlock.Input i in o.Connected.ToArray()) if (i == _In) o.Connected.Remove(i);
            _Out.Connected.Add(_In);
            _In.Connected = _Out;
        }

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

        public static KismetSequence FromByteCode(byte[] _Code)
        {
            CodeBlock.Initialize();//compile a list with avaiable codeblocks
            KismetSequence sequence = new KismetSequence();
            int idx = 0;
            while (true)
            {
                if (_Code[idx] == 0) break;
                if (!CodeBlock.CodeBlocks.ContainsKey(_Code[idx])) break;

                Type t = CodeBlock.CodeBlocks[_Code[idx]];
                CodeBlock block = (CodeBlock)t.GetConstructor(new Type[] { }).Invoke(new object[] { });
                block.DisAssamble(Utilities.Cut(_Code, idx));
                sequence.codeblocks.Add(block);
                idx += block.Code.Length;
            }
            return sequence;
        }
    }
}
