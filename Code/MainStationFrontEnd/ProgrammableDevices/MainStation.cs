using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using SynergySequence;
using MainStationCodeBlocks;


namespace MainStationFrontEnd
{
    public class MainStationSequenceManager : SequenceManager
    {
        public override CodeBlock CreateCodeBlock(Prototype _Prototype)
        {
            return (CodeBlock)Activator.CreateInstance(_Prototype.BlockType);
        }
    }

    public class MainStation : ProgrammableDevice
    {
        public MainStation()
            : base()
        {
            Manager = new MainStationSequenceManager();
            MainStationCodeBlock.AddAllPrototypes(Manager);

            Manager.AddDataType(new SequenceManager.DataType("int", System.Drawing.Color.Blue));
            Manager.AddDataType(new SequenceManager.DataType("bool", System.Drawing.Color.Green));

            Sequence = new Sequence(Manager);
        }

        List<Device> devices = new List<Device>();
        public Device[] Devices { get { return devices.ToArray(); } }

        public override System.Windows.Forms.TreeNode GetTreeNode()
        {
            TreeNode node = new TreeNode("mainstation");
            node.Tag = this;
            return node;
        }

        public override void Load(System.Xml.Linq.XElement _Data)
        {
            Sequence.Load(_Data.Element("Sequence"));
        }

        public override void Save(System.Xml.Linq.XElement _Data)
        {
            _Data.Add(Sequence.Save());
        }

        public Device RegisterDevice(string _Name, ProductDataBase.Device _Device, ushort _DeviceID)
        {
            Device d = new Device(_Name, _Device, _DeviceID);
            foreach (ProductDataBase.Device.Event e in _Device.events)
            {
                d.Events.Add(e.ID, new Device.Event(d, e));
            }
            devices.Add(d);
            return d;
        }

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

            public class Event
            {
                public string DefaultName { get { return eventtype != null ? eventtype.Name : null; } }
                public Device device;

                public ProductDataBase.Device.Event eventtype;

                public string Name;
                public Event(Device _Device, ProductDataBase.Device.Event _Event)
                {
                    device = _Device;
                    eventtype = _Event;
                    Name = DefaultName;
                }
            }
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
                fixed (byte* ptr = _Buffer)
                    HIDClass.MCHPHIDClass.USBHIDWriteReport(ptr, (uint)_Buffer.Length);
            }
        }

        public static byte[] Read()
        {
            byte[] buffer = new byte[65];
            unsafe
            {
                fixed (byte* ptr = buffer)
                    HIDClass.MCHPHIDClass.USBHIDReadReport(ptr);
            }
            return buffer;
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
            System.Threading.Thread.Sleep(1000);
            byte[] result = Read();//wait for answer
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
            OperationDisable();
            EEPROMWrite(_Data);
            byte[] read = new byte[_Data.Length];
            for (int i = 0; i < read.Length; i++)
            {
                read[i] = EEPROMRead((ushort)i);
            }
            for (int i = 0; i < read.Length; i++)
            {
                if (EEPROMRead((ushort)i) != _Data[i])
                    return false;
            }
            OperationEnable();
            return true;
        }

        public static void EEPROMWrite(byte[] _Data)
        {
            System.IO.File.WriteAllBytes("c:\\eeprom.bin", _Data);
#if false//write page
            for (int i = 0; i < _Data.Length; i += 32)
            {
                byte[] buffer = new byte[32];
                for (int j = 0; j < 32; j++)
                {
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
            public ushort Day{ get { return day; } set { day = value; } }

            public override string ToString()
            {
                return DayTime.Hours.ToString() + ":" + DayTime.Minutes.ToString();
            }
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
            t.Day = Utilities.ToShort(buffer, 4);
            return t;
        }

        public static void TimeWrite()
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x41;
            buffer[1] = (byte)DateTime.Now.Second;
            buffer[2] = (byte)DateTime.Now.Minute;
            buffer[3] = (byte)DateTime.Now.Hour;
            byte[] shrt = Utilities.FromShort(Utilities.GetDay(DateTime.Now));
            buffer[4] = shrt[0];
            buffer[5] = shrt[1];
            Write(buffer);
            System.Threading.Thread.Sleep(10);
            buffer = Read();
        }
    }
}
