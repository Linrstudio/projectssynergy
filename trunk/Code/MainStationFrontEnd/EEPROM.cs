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
        static ushort size = 65535;//65536 byte
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
        public static List<ScheduleEntry> ScheduleEntries = new List<ScheduleEntry>();

        //static public List<CalendarEntry> 

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
            public bool Found = false;
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
                public string DefaultName { get { return eventtype != null ? eventtype.Name : null; } }
                public Device device;
                public ushort addr = 0;
                public ProductDataBase.Device.Event eventtype;
                public string Name;
                public Event(Device _Device, ProductDataBase.Device.Event _Event)
                {
                    device = _Device;
                    eventtype = _Event;
                    sequence = new KismetSequenceDeviceEvent(this, null);
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

        public static void AssambleSchedule()
        {
            KismetSequence target = Devices[0].Events[2].sequence;
            target.Clear();
            CodeBlock minute = new BlockGetMinute(target);
            CodeBlock hour = new BlockGetHour(target);
            target.codeblocks.Add(minute);
            target.codeblocks.Add(hour);
            target.Connect(target.root.Outputs[0], hour.Inputs[0]);
            target.Connect(target.root.Outputs[0], minute.Inputs[0]);

            foreach (ScheduleEntry entry in ScheduleEntries)
            {
                CodeBlock chour = new BlockMathConstant(target);
                CodeBlock cmin = new BlockMathConstant(target);

                cmin.SetValues(entry.Minutes.ToString());
                chour.SetValues(entry.Hours.ToString());

                CodeBlock eqhour = new BlockMathEquals(target);
                CodeBlock eqmin = new BlockMathEquals(target);
                CodeBlock and = new BlockBoolAnd(target);
                CodeBlock root = new BlockBoolIf(target);

                target.codeblocks.Add(eqhour);
                target.codeblocks.Add(eqmin);

                target.codeblocks.Add(chour);
                target.codeblocks.Add(cmin);

                target.codeblocks.Add(and);
                target.codeblocks.Add(root);

                target.Connect(hour.Outputs[0], eqhour.Inputs[0]);
                target.Connect(minute.Outputs[0], eqmin.Inputs[0]);
                target.Connect(chour.Outputs[0], eqhour.Inputs[1]);
                target.Connect(cmin.Outputs[0], eqmin.Inputs[1]);

                target.Connect(target.root.Outputs[0], chour.Inputs[0]);
                target.Connect(target.root.Outputs[0], cmin.Inputs[0]);

                target.Connect(eqhour.Outputs[0], and.Inputs[0]);
                target.Connect(eqmin.Outputs[0], and.Inputs[1]);

                target.Connect(and.Outputs[0], root.Inputs[0]);

                //copy all codeblocks into our new sequence
                Dictionary<CodeBlock, CodeBlock> map = new Dictionary<CodeBlock, CodeBlock>();
                map.Add(entry.sequence.root, root);
                foreach (CodeBlock block in entry.sequence.codeblocks)
                {
                    if (block == entry.sequence.root) continue;
                    string blocktype = block.GetType().FullName;
                    CodeBlock b = (CodeBlock)Type.GetType(blocktype).GetConstructor(new Type[] { typeof(KismetSequence) }).Invoke(new object[] { target });
                    b.SetValues(block.GetValues());
                    target.codeblocks.Add(b);
                    map.Add(block, b);
                }
                foreach (CodeBlock block in entry.sequence.codeblocks)
                {
                    if (block == entry.sequence.root) continue;

                    foreach (CodeBlock.Input input in block.Inputs)
                    {
                        if (input.Connected != null)
                        {
                            int inputidx = input.Owner.Inputs.IndexOf(input);
                            int outputidx = input.Connected.Owner.Outputs.IndexOf(input.Connected);

                            target.Connect(map[input.Connected.Owner].Outputs[outputidx], map[input.Owner].Inputs[inputidx]);
                        }
                    }
                }

                target.FixIndices();
            }
        }

        public static byte[] Assamble()
        {
            //add schedule entry
            Globals.Clear();
            AssambleSchedule();
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
                    if (e.eventtype == null) continue;
                    XElement method = new XElement("Event");
                    method.SetAttributeValue("ID", e.eventtype.ID);
                    method.SetAttributeValue("Name", e.Name);
                    e.sequence.Save(method);
                    device.Add(method);
                }
                file.Add(device);
            }

            foreach (ScheduleEntry e in ScheduleEntries)
            {
                XElement entry = new XElement("ScheduleEntry");
                entry.SetAttributeValue("Name", e.Name);
                entry.SetAttributeValue("Days", e.Days.ToString());
                entry.SetAttributeValue("Hours", e.Hours.ToString());
                entry.SetAttributeValue("Minutes", e.Minutes.ToString());
                entry.SetAttributeValue("Seconds", e.Seconds.ToString());
                e.sequence.Save(entry);
                file.Add(entry);
            }

            file.Save(_FileName);

        }

        public static void Clear()
        {
            Devices.Clear();
            Globals.Clear();
            ScheduleEntries.Clear();
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
                    if (!d.Events.ContainsKey(methodid)) continue;
                    Device.Event e = d.Events[methodid];
                    try
                    {
                        e.Name = method.Attribute("Name").Value;
                    }
                    catch { e.Name = ProductDataBase.GetDeviceByID(typeid).GetEventByID(methodid).Name; }
                    e.sequence = new KismetSequenceDeviceEvent(e, null);
                    e.sequence.Load(method);
                }
            }

            foreach (XElement e in file.Elements("ScheduleEntry"))
            {
                ScheduleEntry entry = new ScheduleEntry();
                entry.Name = e.Attribute("Name").Value;

                entry.Days = int.Parse(e.Attribute("Days").Value);
                entry.Hours = int.Parse(e.Attribute("Hours").Value);
                entry.Minutes = int.Parse(e.Attribute("Minutes").Value);
                entry.Seconds = int.Parse(e.Attribute("Seconds").Value);
                entry.sequence = new KismetSequenceScheduleEvent(entry);
                entry.sequence.Load(e);
                ScheduleEntries.Add(entry);
            }
            Utilities.Log("EEPROM loaded.");
        }

        public class ScheduleEntry
        {
            public string Name;
            public int Days;
            public int Hours;
            public int Minutes;
            public int Seconds;

            /// <summary>
            /// zero is dont repeat, 1 is dayly 7 is weekly etc etc
            /// </summary>
            public int repeat = 0;

            public KismetSequence sequence = null;

            public DateTime Moment
            {
                get
                {
                    DateTime dt = new DateTime(2000, 1, 1);
                    dt.Add(new TimeSpan(Days, Hours, Minutes, Seconds));
                    return dt;
                }
            }
        }
    }
}
