using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using SynergySequence;
using MainStationCodeBlocks;
using Utilities;

namespace MainStation
{
    public class MainStationSequenceManager : SequenceManager
    {
        public override CodeBlock CreateCodeBlock(Prototype _Prototype)
        {
            return (CodeBlock)Activator.CreateInstance(_Prototype.BlockType);
        }
    }

    public static class MainStationCompiler
    {
        public const int HEADERSIZE = 1;
        static int TimerCount = 0;
        public class RegisterEntry
        {
            public byte index;
            public int size;
            //owner ?
        }

        //note that the first 16 registers will be used for UART transmission
        static List<RegisterEntry> UsedRegisters = new List<RegisterEntry>();
        public static bool[] GetRegisterMask()
        {
            bool[] mask = new bool[256];
            foreach (RegisterEntry e in UsedRegisters)
            {
                for (int i = 0; i < e.size; i++) mask[e.index + i] = true;
            }
            return mask;
        }
        public static RegisterEntry GetRegister(int _Size)
        {
            bool[] mask = GetRegisterMask();
            for (int i = 16; i < 256; i++)//first 16 registers are for UART communication
            {
                bool ok = true;
                if (i >= 255 - TimerCount * 4) ok = false;
                if (i < 128 && i + _Size >= 128) ok = false;//if the register is hanging over the edge between memory block a and b, dont create it

                for (int j = 0; j < _Size; j++)
                {
                    if (mask[i + j]) ok = false;
                }
                if (ok)
                {
                    RegisterEntry e = new RegisterEntry();
                    e.index = (byte)i;
                    e.size = _Size;
                    UsedRegisters.Add(e);
                    return e;
                }
            }
            throw new Exception("Out of registers!");
        }
        public static byte[] Compile(MainStation _MainStation)
        {
            TimerCount = 0;
            byte[] Buffer = new byte[0xffff];//2^16

            //global header
            //first 8 bits indicate the amount of timers

            //Event header
            List<MainStationDevice> devices = new List<MainStationDevice>();
            devices.Add(new MainStationDevice(_MainStation, "", null, 0));
            foreach (MainStationDevice d in _MainStation.Devices)
            {
                devices.Add(d);
            }
            List<BaseBlockEvent.Event> events = new List<BaseBlockEvent.Event>();
            foreach (CodeBlock c in _MainStation.Sequence.CodeBlocks)
            {
                if (c is MainStationCodeBlock)
                {
                    MainStationCodeBlock msc = (MainStationCodeBlock)c;
                    if (msc is BaseBlockEvent)
                    {
                        foreach (BaseBlockEvent.Event evnt in ((BaseBlockEvent)msc).Events)
                        {
                            //if (evnt.Output.Connected.Count > 0)//only add the event to the list when there is code attached to it
                            {
                                if (msc is BlockDelay)
                                    TimerCount++;
                                events.Add(evnt);
                            }
                        }
                    }
                }
                else throw new Exception("normal codeblock in sequence for mainstation found, whut !?");
            }
            //add empty events for RTC stuff, will be implemented later
            events.Add(new BaseBlockEvent.Event(0, 2, null));
            events.Add(new BaseBlockEvent.Event(0, 3, null));
            events.Add(new BaseBlockEvent.Event(0, 4, null));
            events.Add(new BaseBlockEvent.Event(0, 5, null));

            //Device Header
            int deviceheadersize = devices.Count * 4 + 2; // including the two blanks
            int eventheadersize = events.Count * 3 + 2;//2 blanks
            int currenteventaddr = HEADERSIZE + deviceheadersize + eventheadersize;
            int didx = 0;
            int eidx = 0;
            foreach (MainStationDevice d in devices)
            {
                byte[] did = Utilities.Utilities.FromShort(d.ID);
                byte[] eaddr = Utilities.Utilities.FromShort((ushort)(HEADERSIZE + deviceheadersize + eidx * 3));
                Buffer[HEADERSIZE + didx * 4 + 0] = did[0];
                Buffer[HEADERSIZE + didx * 4 + 1] = did[1];
                Buffer[HEADERSIZE + didx * 4 + 2] = eaddr[0];
                Buffer[HEADERSIZE + didx * 4 + 3] = eaddr[1];
                foreach (BaseBlockEvent.Event e in events)
                {
                    if (e.DeviceID != d.ID) continue;
                    int addr = HEADERSIZE + deviceheadersize + eidx * 3;
                    eaddr = Utilities.Utilities.FromShort((ushort)currenteventaddr);
                    Buffer[addr + 0] = e.EventID;
                    Buffer[addr + 1] = eaddr[0];
                    Buffer[addr + 2] = eaddr[1];
                    //clear the compiler before we use it
                    UsedRegisters.Clear();
                    BaseBlockEvent block = null;
                    foreach (CodeBlock c in _MainStation.Sequence.CodeBlocks)
                    {
                        if ((MainStationCodeBlock)c is BaseBlockEvent)
                        {
                            foreach (BaseBlockEvent.Event evnt in ((BaseBlockEvent)c).Events)
                            {
                                if (evnt.DeviceID == e.DeviceID && evnt.EventID == e.EventID)
                                {
                                    block = ((BaseBlockEvent)c);
                                }
                            }
                        }
                    }
                    byte[] blob = new byte[] { };
                    if (e.Output != null) blob = block.CompileEvent(e);
                    for (int i = 0; i < blob.Length; i++)
                    {
                        Buffer[currenteventaddr++] = blob[i];
                    }
                    Buffer[currenteventaddr++] = 0;//zero instruction, indicates the end of the event    
                    eidx++;
                }
                didx++;
            }
            //fill in header
            // for (int i = 0; i < 32; i++) Buffer[i] = (byte)i;
            Buffer[0] = (byte)TimerCount;


            //fill in the blanks
            Buffer[HEADERSIZE + deviceheadersize - 1] = 0xff;//tag the end each header with 0xffff
            Buffer[HEADERSIZE + deviceheadersize - 2] = 0xff;
            Buffer[HEADERSIZE + deviceheadersize + eventheadersize - 1] = 0xff;//tag the end each header with 0xffff
            Buffer[HEADERSIZE + deviceheadersize + eventheadersize - 2] = 0xff;
            //mark end of file ( doesnt do anything )
            Buffer[currenteventaddr + 0] = (byte)'E';
            Buffer[currenteventaddr + 1] = (byte)'O';
            Buffer[currenteventaddr + 2] = (byte)'F';

            MemoryStream stream = new MemoryStream();
            stream.Write(Buffer, 0, currenteventaddr + 3);//+3 for EOF ( temporaryu )

            return stream.ToArray();
        }
    }

    public class MainStation
    {
        public bool Found;

        public SequenceManager Manager;
        public Sequence Sequence;

        public MainStation()
            : base()
        {
            Manager = new MainStationSequenceManager();
            MainStationCodeBlock.AddAllPrototypes(Manager);

            Manager.AddDataType(new SequenceManager.DataType("int", System.Drawing.Color.Blue));
            Manager.AddDataType(new SequenceManager.DataType("bool", System.Drawing.Color.Green));

            Sequence = new Sequence(Manager);
        }

        List<MainStationDevice> devices = new List<MainStationDevice>();
        public MainStationDevice[] Devices { get { return devices.ToArray(); } }

        public void RemoveDevice(MainStationDevice _Device)
        {
            if (devices.Contains(_Device)) devices.Remove(_Device);
        }

        public void Load(System.Xml.Linq.XElement _Data)
        {
            Sequence.Load(_Data.Element("Sequence"));
            Sequence.CenterSequence();
            foreach (XElement element in _Data.Elements("Device"))
            {
                devices.Add(new MainStationDevice(element));
            }
            foreach (MainStationDevice d in devices) d.mainstation = this;
        }

        public void Save(System.Xml.Linq.XElement _Data)
        {
            _Data.Add(Sequence.Save());
            foreach (MainStationDevice d in devices)
            {
                XElement element = new XElement("Device");
                d.Save(element);
                _Data.Add(element);
            }
        }

        public MainStationDevice RegisterDevice(string _Name, ProductDataBase.Device _Device, ushort _DeviceID)
        {
            MainStationDevice d = new MainStationDevice(this, _Name, _Device, _DeviceID);
            foreach (ProductDataBase.Device.Event e in _Device.events)
            {
                d.Events.Add(e.ID, new MainStationDevice.Event(d, e));
            }
            devices.Add(d);
            return d;
        }

        public const byte EPBufferOffset = 0;
        public const byte KismetRegisterCount = 32;
        public const byte EPBufferSize = 16;

        public static void Connect()
        {
            HIDClass.MCHPHIDClass.USBHIDClassInit(0x4d8, 0x3f, 64);
        }

        public static bool Connected()
        {
            bool con = HIDClass.MCHPHIDClass.USBHIDIsConnected();
            return con;
        }

        public static void Write(byte[] _Buffer)
        {
            unsafe
            {
                fixed (byte* ptr = _Buffer) HIDClass.MCHPHIDClass.USBHIDWriteReport(ptr, (uint)_Buffer.Length);
            }
        }

        public static byte[] Read()
        {
            byte[] buffer = new byte[65];
            unsafe
            {
                fixed (byte* ptr = buffer) HIDClass.MCHPHIDClass.USBHIDReadReport(ptr);
            }
            return buffer;
        }

        public static bool SetTimer(byte _Timer, byte _Event, ushort _Time)
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x50;//Set Timer
            byte[] shrt = BitConverter.GetBytes((ushort)_Time);
            buffer[1] = _Timer;
            buffer[2] = _Event;
            buffer[3] = shrt[0];
            buffer[4] = shrt[1];
            Write(buffer);
            buffer = Read();
            return buffer[1] == 255;
        }

        public static bool Poll(ushort _DeviceID)
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x01;//send raw data to devices
            byte[] shrt = BitConverter.GetBytes((ushort)_DeviceID);
            buffer[1] = shrt[0];
            buffer[2] = shrt[1];
            Write(buffer);
            buffer = Read();
            return buffer[1] == 255;
        }

        public static void InvokeLocalEvent(ushort _DeviceID, byte _Event, ushort _Arguments)
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x06;//send raw data to devices
            byte[] shrt = BitConverter.GetBytes((ushort)_DeviceID);
            buffer[1] = shrt[0];
            buffer[2] = shrt[1];
            buffer[3] = _Event;
            Write(buffer);
            //#if false
            System.Threading.Thread.Sleep(10);
            byte[] result = Read();//wait for answer
            if (result[0] != 0x06)
            {
                MessageBox.Show("wtf");
            }
            //#endif
        }

        public static void InvokeRemoteEvent(ushort _DeviceID, byte _Event, ushort _Arguments)
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x02;//send raw data to devices
            byte[] shrt = BitConverter.GetBytes((ushort)_DeviceID);
            buffer[1] = shrt[0];
            buffer[2] = shrt[1];
            buffer[3] = 3;//buffer length
            buffer[4] = _Event;
            shrt = BitConverter.GetBytes((ushort)_Arguments);
            buffer[5] = shrt[0];
            buffer[6] = shrt[1];
            Write(buffer);
            Read();//wait for answer
        }

        public static void SendRaw(ushort _DeviceID, byte[] _Data)
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x02;//send raw data to devices
            byte[] shrt = BitConverter.GetBytes((ushort)_DeviceID);
            buffer[1] = shrt[0];
            buffer[2] = shrt[1];
            buffer[3] = (byte)_Data.Length;//buffer length
            for (int i = 0; i < _Data.Length; i++)
            {
                buffer[4 + i] = _Data[i];
            }
            Write(buffer);
        }

        public static bool EEPROMWriteVerify(byte[] _Data)
        {
            int begin = Environment.TickCount;
            OperationDisable();
            EEPROMWrite(_Data);
#if true
            byte[] read = new byte[_Data.Length];
            for (int i = 0; i < read.Length; i++)
            {
                read[i] = EEPROMRead((ushort)i);
            }
            for (int i = 0; i < read.Length; i++)
            {
                if (read[i] != _Data[i])
                {
                    Console.WriteLine("verify failed");
                    return false;
                }
            }
#endif
            OperationEnable();
            int time = Environment.TickCount - begin;
            Console.WriteLine("Time: {0}", time);
            return true;
        }

        public static void EEPROMWrite(byte[] _Data)
        {

            System.IO.File.WriteAllBytes(@"c:\eeprom.bin", _Data);
#if true//write page
            for (int i = 0; i < _Data.Length; i += 32)
            {
                byte[] buffer = new byte[32];
                for (int j = 0; j < 32; j++)
                {
                    if (i + j < _Data.Length)
                        buffer[j] = _Data[i + j];
                }
                EEPROMWritePage((ushort)i, buffer);
            }
#else
            for (int i = 0; i < _Data.Length; i++)
            {
                EEPROMWrite((ushort)i, _Data[i]);
            }
#endif
        }

        public static void EEPROMWrite(ushort _Address, byte _Data)
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x03;
            byte[] shrt = BitConverter.GetBytes((ushort)_Address);
            buffer[1] = shrt[0];
            buffer[2] = shrt[1];
            buffer[3] = _Data;
            Write(buffer);
            System.Threading.Thread.Sleep(10);
            Read();//wait for answer
        }

        public static void EEPROMWritePage(ushort _Address, byte[] _Data)
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x04;
            byte[] shrt = BitConverter.GetBytes((ushort)_Address);
            buffer[1] = shrt[0];
            buffer[2] = shrt[1];
            for (int i = 0; i < 32; i++)
            {
                buffer[3 + i] = _Data[i];
            }
            Write(buffer);
            System.Threading.Thread.Sleep(100);
            Read();//wait for answer
        }

        public static byte EEPROMRead(ushort _Address)
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x05;
            byte[] shrt = BitConverter.GetBytes((ushort)_Address);
            buffer[1] = shrt[0];
            buffer[2] = shrt[1];
            Write(buffer);
            System.Threading.Thread.Sleep(10);
            buffer = Read();
            return buffer[1];
        }

        public static void OperationEnable()
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x08;
            Write(buffer);
            Read();
        }

        public static void OperationDisable()
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x07;
            Write(buffer);
            Read();
        }

        public struct Time
        {
            TimeSpan daytime;
            [Browsable(true)]
            public TimeSpan DayTime { get { return daytime; } set { daytime = value; } }
            ushort day;
            [Browsable(true)]
            public ushort Day { get { return day; } set { day = value; } }

            public override string ToString()
            {
                return DayTime.Hours.ToString() + ":" + DayTime.Minutes.ToString();
            }
        }

        public static void KismetInvoke(ushort _DeviceID, byte _Event)
        {
            byte[] devid = Utilities.Utilities.FromShort(_DeviceID);
            byte[] buffer = new byte[65];
            buffer[0] = 0x10;
            buffer[1] = devid[0];
            buffer[2] = devid[1];
            buffer[3] = _Event;
            Write(buffer);
            System.Threading.Thread.Sleep(10);
            buffer = Read();
        }

        public static Time TimeRead()
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x40;
            Write(buffer);
            System.Threading.Thread.Sleep(10);
            buffer = Read();
            Time t = new Time();
            t.DayTime = new TimeSpan(buffer[3], buffer[2], buffer[1]);
            t.Day = Utilities.Utilities.ToShort(buffer, 4);
            return t;
        }

        public static void TimeWrite()
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x41;
            buffer[1] = (byte)DateTime.Now.Second;
            buffer[2] = (byte)DateTime.Now.Minute;
            buffer[3] = (byte)DateTime.Now.Hour;
            byte[] shrt = Utilities.Utilities.FromShort(Utilities.Utilities.GetDay(DateTime.Now));
            buffer[4] = shrt[0];
            buffer[5] = shrt[1];
            Write(buffer);
            System.Threading.Thread.Sleep(10);
            buffer = Read();
        }
    }

    public class MainStationDevice
    {
        public MainStationDevice(MainStation _MainStation, string _Name, ProductDataBase.Device _Device, ushort _ID)
        {
            mainstation = _MainStation;
            Name = _Name;
            device = _Device;
            ID = _ID;
        }
        public MainStation mainstation;
        public MainStationDevice(XElement _Data) { Load(_Data); }
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

        public void Load(XElement _Data)
        {
            Name = _Data.Attribute("Name").Value;
            ID = ushort.Parse(_Data.Attribute("ID").Value);
            device = ProductDataBase.GetDeviceByID(ushort.Parse(_Data.Attribute("Device").Value));
        }

        public void Save(XElement _Data)
        {
            _Data.SetAttributeValue("Name", Name);
            _Data.SetAttributeValue("ID", ID);
            _Data.SetAttributeValue("Device", device.ID);
        }

        public class Event
        {
            public string DefaultName { get { return eventtype != null ? eventtype.Name : null; } }
            public MainStationDevice device;

            public ProductDataBase.Device.Event eventtype;

            public string Name;
            public Event(MainStationDevice _Device, ProductDataBase.Device.Event _Event)
            {
                device = _Device;
                eventtype = _Event;
                Name = DefaultName;
            }
        }
    }
}
