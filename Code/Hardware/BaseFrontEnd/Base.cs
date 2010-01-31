using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;

namespace BaseFrontEnd
{
    public class Device
    {
        public ushort addr = 0;
        public ushort ID;
        public List<Event> events = new List<Event>();

        public class Event
        {
            public ushort addr = 0;
            public byte ID;
            public Method method = new Method();

            public Event(byte _ID)
            {
                ID = _ID;
            }

            public class Method
            {
                public Method() { ByteCode = new byte[] { 0 }; }
                public ushort addr;
                public byte[] ByteCode;
            }

            public virtual void Update()
            {

            }
        }
    }

    public class Base
    {
        SerialPort port;

        byte[] EEPROM = new byte[] { };

        public List<Device> devices = new List<Device>();


        public Base()
        {
            port = new SerialPort("COM2", 1200, Parity.None, 8, StopBits.One);
            port.Open();

            Write('h');//hello
            EEPROM = new byte[ReadShort()];

            AddSomeRandomStuff();

            BuildEEPROM();
            //UploadEEPROM();
            //DownloadEEPROM();

            //ExecuteRemoteEvent(devices[0].ID, devices[0].events[0].ID, 123);
        }

        public void AddSomeRandomStuff()
        {
            {
                Device d = new Device();
                d.ID = 1;
                {//dump registers
                    Device.Event e = new Device.Event(1);
                    e.method.ByteCode = new byte[] { 3, 0 };
                    d.events.Add(e);
                }
                {//dump registers
                    Device.Event e = new Device.Event(2);
                    e.method.ByteCode = new byte[] { 2, 0, 0 };
                    d.events.Add(e);
                }
                devices.Add(d);
            }
            {
                Device d = new Device();
                d.ID = 123;
                {
                    Device.Event e = new Device.Event(45);
                    e.method.ByteCode = new byte[] { 1, 4, 137, 2, 4, 0 };
                    d.events.Add(e);
                }
                {
                    Device.Event e = new Device.Event(46);
                    e.method.ByteCode = new byte[] { 2, 1, 0 };
                    d.events.Add(e);
                }
                //d.events.Add(new Device.Event(47));
                devices.Add(d);
            }
        }

        public void DownloadEEPROM()
        {
            for (ushort b = 0; b < EEPROM.Length; b += 16)
            {
                Write('r');
                WriteShort(b);
                for (int i = 0; i < 16; i++)
                {
                    EEPROM[i + b] = Read(1)[0];
                }
            }
        }

        public void CheckEEPROM()
        {
            DownloadEEPROM();
            int checksum = 0;
            foreach (byte b in EEPROM) checksum += b;
            DownloadEEPROM();
            int checksum2 = 0;
            foreach (byte b in EEPROM) checksum2 += b;
            if (checksum != checksum2) throw new Exception("EEPROM error detected");
        }

        public void UploadEEPROM()
        {
            for (ushort b = 0; b < EEPROM.Length; b += 16)
            {
                Write('w');
                WriteShort(b);
                for (int i = 0; i < 16; i++)
                {
                    Write(new byte[] { EEPROM[i + b] });
                }
                while (Read(1)[0] != (byte)'w') ;
            }
        }

        public void BuildEEPROMSequential()
        {
            for (int i = 0; i < EEPROM.Length; i++) EEPROM[i] = (byte)i;
        }

        public void ReadTime()
        {
            Write('t');
            Write('r');
            byte hour = Read(1)[0];
            byte minute = Read(1)[0];
            byte second = Read(1)[0];
            byte day = Read(1)[0];

            Console.WriteLine("[{0}:{1}:{2} day:{3}]", hour, minute, second,(DayOfWeek)day);
        }

        public void SetTime(DateTime _Time)
        {
            Write('t');
            Write('w');

            Write((byte)_Time.Hour);
            Write((byte)_Time.Minute);
            Write((byte)_Time.Second);
            Write((byte)((int)_Time.DayOfWeek));
        }

        public void BuildEEPROM()
        {
            byte[] buffer = new byte[EEPROM.Length];
            ushort idx = 0;
            ushort eventlistaddr = 0;
            foreach (Device d in devices)
            {
                d.addr = (ushort)(idx * 4);
                byte[] shrt = GetShort(d.ID);
                buffer[d.addr + 0] = shrt[0];
                buffer[d.addr + 1] = shrt[1];
                idx++;
                eventlistaddr = (ushort)(d.addr + 4);
            }
            eventlistaddr += 2;//add two
            ushort addr = eventlistaddr;
            foreach (Device d in devices)
            {
                foreach (Device.Event e in d.events)
                {
                    e.addr = addr;
                    buffer[e.addr] = e.ID;
                    buffer[e.addr + 1] = 1;
                    buffer[e.addr + 2] = 1;
                    addr += 3;
                }
            }
            addr += 2;
            foreach (Device d in devices)
            {
                foreach (Device.Event e in d.events)
                {
                    e.method.addr = addr;
                    for (int i = 0; i < e.method.ByteCode.Length; i++)
                        buffer[addr + i] = e.method.ByteCode[i];
                    addr += (ushort)e.method.ByteCode.Length;
                }
            }
            addr += 2;

            //fillin addresses
            foreach (Device d in devices)
            {
                {
                    byte[] shrt = GetShort(d.events[0].addr);
                    buffer[d.addr + 2] = shrt[0];
                    buffer[d.addr + 3] = shrt[1];
                }
                foreach (Device.Event e in d.events)
                {
                    byte[] shrt = GetShort(e.method.addr);
                    buffer[e.addr + 1] = shrt[0];
                    buffer[e.addr + 2] = shrt[1];
                }
            }

            Console.WriteLine("EEPROM used {0} out of {1} byte ({2}%)", addr, EEPROM.Length, ((float)addr / (float)EEPROM.Length) * 100);

            EEPROM = buffer;
        }

        public void ExecuteRemoteEvent(ushort _DeviceID, byte _EventID, byte _EventArgs)
        {
            Write('e');
            WriteShort(_DeviceID);
            Write(_EventID);
            Write(_EventArgs);
        }



        public byte[] GetShort(ushort _V)
        {
            byte[] buf = BitConverter.GetBytes(_V);
            return new byte[] { buf[1], buf[0] };
        }

        public ushort ReadShort()
        {
            byte[] buf = Read(2);
            return BitConverter.ToUInt16(new byte[] { buf[1], buf[0] }, 0);
        }

        public void WriteShort(ushort _Val)
        {
            byte[] buf = BitConverter.GetBytes(_Val);
            Write(new byte[] { buf[1], buf[0] });
        }

        public void Write(char _Char)
        {
            Write(new byte[] { (byte)_Char });
        }

        public void Write(byte _Byte)
        {
            Write(new byte[] { _Byte });
        }


        public void Write(byte[] _Buffer)
        {
            byte[] buf = _Buffer;
            port.Write(_Buffer, 0, _Buffer.Length);

            if (Console.ForegroundColor != ConsoleColor.Green) Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (byte b in buf)
            {
                Console.Write(b.ToString() + " ");
            }
        }

        public int Available() { return port.BytesToRead; }

        public byte[] Read(int _Count)
        {
            byte[] buf = new byte[_Count];
            port.Read(buf, 0, _Count);

            if (Console.ForegroundColor != ConsoleColor.Red) Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (byte b in buf)
            {
                Console.Write(b.ToString() + " ");
            }

            return buf;
        }
    }
}
