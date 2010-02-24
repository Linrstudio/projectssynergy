﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace BaseFrontEnd
{
    public class EEPROM
    {
        ushort size;
        ushort bytesused;
        public ushort Size { get { return size; } }
        public ushort BytesUsed { get { return bytesused; } }
        public List<string> Globals = new List<string>();//index == address;

        public EEPROM(ushort _EEPROMSize)
        {
            size = _EEPROMSize;
        }

        public SortedDictionary<ushort, Device> Devices = new SortedDictionary<ushort, Device>();

        public class Device
        {
            public Device(EEPROM _EEPROM, ushort _ID)
            {
                eeprom = _EEPROM;
                ID = _ID;
            }
            public EEPROM eeprom;
            public ushort addr = 0;
            public ushort eventaddr = 0;
            public ushort ID;
            public SortedDictionary<byte, Event> Events = new SortedDictionary<byte, Event>();

            public class Event
            {
                public Device device;
                public ushort addr = 0;
                public byte ID;

                public Event(Device _Device, byte _ID)
                {
                    device = _Device;
                    ID = _ID;
                    sequence = new KismetSequence(this);
                }

                public ushort SequenceAddr;
                public KismetSequence sequence;
            }
        }

        public byte GetVariableAddress(string _GlobalName)
        {
            if (!Globals.Contains(_GlobalName)) Globals.Add(_GlobalName);
            return (byte)Globals.IndexOf(_GlobalName);
        }

        public byte[] Assamble()
        {
            Globals.Clear();
            byte[] buffer = new byte[Size];
            ushort idx = 0;
            ushort eventlistaddr = 0;
            foreach (Device d in Devices.Values)
            {
                d.addr = (ushort)(idx * 4);
                byte[] shrt = Utilities.FromShort(d.ID);
                buffer[d.addr + 0] = shrt[0];
                buffer[d.addr + 1] = shrt[1];
                idx++;
                eventlistaddr = (ushort)(d.addr + 4);
            }
            eventlistaddr += 2;//add two blanks, indicating a null address
            ushort addr = eventlistaddr;
            foreach (Device d in Devices.Values)
            {
                d.eventaddr = addr;
                foreach (Device.Event e in d.Events.Values)
                {
                    e.addr = addr;
                    buffer[e.addr] = e.ID;
                    buffer[e.addr + 1] = 1;
                    buffer[e.addr + 2] = 1;
                    addr += 3;
                }
            }
            addr += 2;//add two blanks, indicating a null event
            foreach (Device d in Devices.Values)
            {
                foreach (Device.Event e in d.Events.Values)
                {
                    e.SequenceAddr = addr;
                    byte[] seqcode = e.sequence.GetByteCode();
                    for (int i = 0; i < seqcode.Length; i++)
                        buffer[addr + i] = seqcode[i];
                    addr += (ushort)seqcode.Length;
                }
            }
            addr += 2;
            bytesused = addr;

            //fillin addresses
            foreach (Device d in Devices.Values)
            {
                {
                    byte[] shrt = Utilities.FromShort(d.eventaddr);
                    buffer[d.addr + 2] = shrt[0];
                    buffer[d.addr + 3] = shrt[1];
                }
                foreach (Device.Event e in d.Events.Values)
                {
                    byte[] shrt = Utilities.FromShort(e.SequenceAddr);
                    buffer[e.addr + 1] = shrt[0];
                    buffer[e.addr + 2] = shrt[1];
                }
            }

            Console.WriteLine("EEPROM used {0} out of {1} byte ({2}%)", addr, Size, (int)(((float)addr / (float)Size) * 100));

            return buffer;
        }

        public void Save(string _FileName)
        {
            XElement file = new XElement("EEPROM");

            foreach (Device d in Devices.Values)
            {
                XElement device = new XElement("Device");
                device.SetAttributeValue("ID", d.ID.ToString());
                foreach (Device.Event e in d.Events.Values)
                {
                    XElement method = new XElement("Event");
                    method.SetAttributeValue("ID", e.ID);
                    foreach (CodeBlock b in e.sequence.codeblocks)
                    {
                        XElement block = null;
                        if (b == e.sequence.root) block = new XElement("Root"); else block = new XElement("Block");
                        block.SetAttributeValue("GUID", b.index);
                        block.SetAttributeValue("Type", CodeBlock.GetCodeBlock(b.GetType()).Type.FullName);
                        block.SetAttributeValue("Values", b.GetValues());
                        method.Add(block);
                    }
                    foreach (CodeBlock b in e.sequence.codeblocks)
                    {
                        foreach (CodeBlock.Input i in b.Inputs)
                        {
                            XElement connection = new XElement("Connect");
                            connection.SetAttributeValue("Input", i.Owner.Inputs.IndexOf(i).ToString());
                            connection.SetAttributeValue("InputOwner", e.sequence.codeblocks.IndexOf(i.Owner).ToString());
                            connection.SetAttributeValue("Output", i.Connected.Owner.Outputs.IndexOf(i.Connected).ToString());
                            connection.SetAttributeValue("OutputOwner", e.sequence.codeblocks.IndexOf(i.Connected.Owner).ToString());
                            method.Add(connection);
                        }
                    }
                    device.Add(method);
                }

                file.Add(device);
            }
            file.Save(_FileName);
        }

        public static EEPROM FromFile(string _FileName)
        {
            CodeBlock.Initialize();
            EEPROM eeprom = new EEPROM(2048);
            XElement file = XElement.Load(_FileName);
            foreach (XElement device in file.Elements("Device"))
            {
                ushort deviceid = ushort.Parse(device.Attribute("ID").Value);
                Device d = new Device(eeprom, deviceid);
                eeprom.Devices.Add(deviceid, d);

                foreach (XElement method in device.Elements("Event"))
                {
                    byte methodid = byte.Parse(method.Attribute("ID").Value);
                    Device.Event e = new Device.Event(d, methodid);
                    {
                        XElement block = method.Element("Root");
                        string blocktype = (string)block.Attribute("Type").Value;
                        byte index = byte.Parse(block.Attribute("GUID").Value);
                        CodeBlock b = (CodeBlock)Type.GetType(blocktype).GetConstructor(new Type[] { typeof(KismetSequence) }).Invoke(new object[] { e.sequence });
                        b.index = index;
                        b.SetValues(block.Attribute("Values").Value);
                        e.sequence = new KismetSequence(e, b);
                    }

                    foreach (XElement block in method.Elements("Block"))
                    {
                        string blocktype = (string)block.Attribute("Type").Value;
                        byte index = byte.Parse(block.Attribute("GUID").Value);
                        CodeBlock b = (CodeBlock)Type.GetType(blocktype).GetConstructor(new Type[] { typeof(KismetSequence) }).Invoke(new object[] { e.sequence });
                        b.index = index;
                        b.SetValues(block.Attribute("Values").Value);
                        e.sequence.codeblocks.Add(b);
                    }

                    foreach (XElement connection in method.Elements("Connect"))
                    {
                        int input = int.Parse(connection.Attribute("Input").Value);
                        int inputowner = int.Parse(connection.Attribute("InputOwner").Value);
                        int output = int.Parse(connection.Attribute("Output").Value);
                        int outputowner = int.Parse(connection.Attribute("OutputOwner").Value);

                        e.sequence.Connect(e.sequence.codeblocks[outputowner].Outputs[output], e.sequence.codeblocks[inputowner].Inputs[input]);
                    }

                    d.Events.Add(methodid, e);
                }
            }

            return eeprom;
        }
    }

    public class KismetSequence
    {
        public EEPROM.Device.Event Event = null;
        public CodeBlock root = null;
        public List<CodeBlock> codeblocks = new List<CodeBlock>();

        public KismetSequence(EEPROM.Device.Event _Event)
        {
            Event = _Event;
            root = new PushEvent(this);
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
                            changed = true;
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
            Console.WriteLine("Sequence used {0} registers", registeridx);
            foreach (CodeBlock b in blockssorted)
            {
                b.Assamble();
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
