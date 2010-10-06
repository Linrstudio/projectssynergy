using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace MainStationFrontEnd
{
    public class EEPROM
    {
        static ushort size = 65535;
        static ushort bytesused;
        static public ushort Size { get { return size; } }
        static public ushort BytesUsed { get { return bytesused; } }
        static public List<string> Globals = new List<string>();//index == address;

        public delegate void OnAssambleHandler();
        static public event OnAssambleHandler OnAssamble;

        static public Device RegisterDevice(string _Name, ProductDataBase.Device _Device, ushort _DeviceID)
        {
            Device d = new Device(_Name, _Device, _DeviceID);
            foreach (ProductDataBase.Device.Event e in _Device.events)
            {
                d.Events.Add(e.ID, new Device.Event(d, e));
            }
            Devices.Add(_DeviceID, d);
            return d;
        }

        static public SortedDictionary<ushort, Device> Devices = new SortedDictionary<ushort, Device>();

        public class Device
        {
            public Device(string _Name, ProductDataBase.Device _Device, ushort _ID)
            {
                Name = _Name;
                device = _Device;
                ID = _ID;
            }
            public string Name;
            public ushort addr = 0;
            /// <summary>
            /// address in memory
            /// </summary>
            public ushort eventaddr = 0;
            public ushort ID;
            public ProductDataBase.Device device;
            public SortedDictionary<byte, Event> Events = new SortedDictionary<byte, Event>();

            public bool WorthCompiling()
            {
                //if (ID == 0) return false;
                bool found = false;
                foreach (Event e in Events.Values)
                {
                    foreach (CodeBlock.Output output in e.sequence.root.Outputs)
                    {
                        if (output.Connected.Count > 0) found = true;
                    }
                }
                if (!found) return false;
                return true;
            }

            public class Event
            {
                public string DefaultName { get { return eventtype.Name; } }
                public Device device;
                public ushort addr = 0;
                public ProductDataBase.Device.Event eventtype;
                public string Name;
                public Event(Device _Device, ProductDataBase.Device.Event _Event)
                {
                    device = _Device;
                    eventtype = _Event;
                    sequence = new KismetSequence(this);
                    Name = DefaultName;
                }

                public bool WorthCompiling()
                {
                    foreach (CodeBlock.Output i in sequence.root.Outputs)
                    {
                        if (i.Connected.Count > 0) return true;
                    }
                    return false;
                }

                public ushort SequenceAddr;
                public KismetSequence sequence;
            }
        }

        public static byte GetVariableAddress(string _GlobalName)
        {
            if (!Globals.Contains(_GlobalName)) Globals.Add(_GlobalName);
            return (byte)Globals.IndexOf(_GlobalName);
        }


        public static byte[] Assamble()
        {
            Globals.Clear();
            byte[] buffer = new byte[Size];
            ushort idx = 0;
            ushort eventlistaddr = 0;
            foreach (Device d in Devices.Values)
            {
                if (!d.WorthCompiling()) continue;
                d.addr = (ushort)(idx * 4);
                byte[] shrt = Utilities.FromShort(d.ID);
                buffer[d.addr + 0] = shrt[0];
                buffer[d.addr + 1] = shrt[1];
                idx++;
                eventlistaddr = (ushort)(d.addr + 4);
            }
            buffer[eventlistaddr++] = 0xff;
            buffer[eventlistaddr++] = 0xff;//add two blanks, indicating there are no more devices

            ushort addr = eventlistaddr;
            foreach (Device d in Devices.Values)
            {
                if (!d.WorthCompiling()) continue;
                d.eventaddr = addr;
                foreach (Device.Event e in d.Events.Values)
                {
                    if (!e.WorthCompiling()) continue;
                    e.addr = addr;
                    buffer[e.addr] = e.eventtype.ID;
                    buffer[e.addr + 1] = 0;
                    buffer[e.addr + 2] = 0;
                    addr += 3;
                }
            }
            buffer[addr++] = 0xff;
            buffer[addr++] = 0xff;//add two blanks, indicating there are no more events

            foreach (Device d in Devices.Values)
            {
                if (!d.WorthCompiling()) continue;
                foreach (Device.Event e in d.Events.Values)
                {
                    if (!e.WorthCompiling()) continue;
                    if (!e.sequence.CheckForErrors())
                    {
                        System.Windows.Forms.MessageBox.Show("Failed to connect KismetSequence, are all inputs connected ?");
                        continue;
                    }
                    e.SequenceAddr = addr;
                    byte[] seqcode = e.sequence.GetByteCode();
                    for (int i = 0; i < seqcode.Length; i++)
                        buffer[addr + i] = seqcode[i];
                    addr += (ushort)seqcode.Length;
                }
            }
            //addr += 2;
            bytesused = addr;

            //fillin addresses
            foreach (Device d in Devices.Values)
            {
                if (!d.WorthCompiling()) continue;
                {
                    byte[] shrt = Utilities.FromShort(d.eventaddr);
                    buffer[d.addr + 2] = shrt[0];
                    buffer[d.addr + 3] = shrt[1];
                }
                foreach (Device.Event e in d.Events.Values)
                {
                    if (!e.WorthCompiling()) continue;
                    byte[] shrt = Utilities.FromShort(e.SequenceAddr);
                    buffer[e.addr + 1] = shrt[0];
                    buffer[e.addr + 2] = shrt[1];
                }
            }

            Console.WriteLine("EEPROM used {0} out of {1} byte ({2}%)", addr, Size, (int)(((float)addr / (float)Size) * 100));

            if (OnAssamble != null) OnAssamble();
            byte[] newbuffer = new byte[BytesUsed];
            for (int i = 0; i < newbuffer.Length; i++) newbuffer[i] = buffer[i];
            return newbuffer;
        }

        public static void Save(string _FileName)
        {
            XElement file = new XElement("EEPROM");

            foreach (Device d in Devices.Values)
            {
                XElement device = new XElement("Device");
                device.SetAttributeValue("TypeID", d.device.ID.ToString());
                device.SetAttributeValue("DeviceID", d.ID.ToString());
                device.SetAttributeValue("DeviceName", d.Name.ToString());
                foreach (Device.Event e in d.Events.Values)
                {
                    XElement method = new XElement("Event");
                    method.SetAttributeValue("ID", e.eventtype.ID);
                    method.SetAttributeValue("Name", e.Name);
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
                            if (i.Connected != null)
                            {
                                XElement connection = new XElement("Connect");
                                connection.SetAttributeValue("Input", i.Owner.Inputs.IndexOf(i).ToString());
                                connection.SetAttributeValue("InputOwner", e.sequence.codeblocks.IndexOf(i.Owner).ToString());
                                connection.SetAttributeValue("Output", i.Connected.Owner.Outputs.IndexOf(i.Connected).ToString());
                                connection.SetAttributeValue("OutputOwner", e.sequence.codeblocks.IndexOf(i.Connected.Owner).ToString());
                                method.Add(connection);
                            }
                        }
                    }
                    device.Add(method);
                }

                file.Add(device);
            }
            file.Save(_FileName);
        }

        public static void Clear()
        {
            EEPROM.Devices.Clear();
            EEPROM.Globals.Clear();
        }

        public static void FromFile(string _FileName)
        {
            Clear();
            CodeBlock.Initialize();
            XElement file = XElement.Load(_FileName);
            foreach (XElement device in file.Elements("Device"))
            {
                ushort typeid = ushort.Parse(device.Attribute("TypeID").Value);
                ushort deviceid = ushort.Parse(device.Attribute("DeviceID").Value);
                string devicename = device.Attribute("DeviceName").Value;
                Device d = RegisterDevice(devicename, ProductDataBase.GetDeviceByID(typeid), deviceid);

                foreach (XElement method in device.Elements("Event"))
                {
                    byte methodid = byte.Parse(method.Attribute("ID").Value);
                    Device.Event e = d.Events[methodid];
                    try
                    {
                        e.Name = method.Attribute("Name").Value;
                    }
                    catch { e.Name = ProductDataBase.GetDeviceByID(typeid).GetEventByID(methodid).Name; }
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
                }
            }
        }
    }
}
