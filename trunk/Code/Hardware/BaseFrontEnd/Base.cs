using System;
using System.Collections.Generic;
using System.Text;
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
            public Method method;

            public Event(byte _ID)
            {
                ID = _ID;
            }

            public class Method
            {
                public ushort addr;
            }
        }
    }

    public class Base
    {
        TcpClient client;

        byte[] EEPROM = new byte[] { };

        public List<Device> devices = new List<Device>();


        public Base(string _IP, ushort _Port)
        {
            client = new TcpClient(_IP, _Port);

            Write('h');//hello
            EEPROM = new byte[ReadShort()];

            AddSomeRandomStuff();

            BuildEEPROM();
            //UploadEEPROM();
            //DownloadEEPROM();

            ExecuteRemoteEvent(devices[0].ID, devices[0].events[0].ID, 123);
        }

        public Base()
        {
            EEPROM = new byte[256];

            BuildEEPROM();
        }

        public void AddSomeRandomStuff()
        {
            Device d = new Device();
            d.ID = 123;
            d.events.Add(new Device.Event(45));
            d.events.Add(new Device.Event(46));
            d.events.Add(new Device.Event(47));
            devices.Add(d);
        }

        public void DownloadEEPROM()
        {
            for (ushort b = 0; b < EEPROM.Length; b += 16)
            {
                Write('r');
                if (Available() > 0) Read(Available());
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



            //fillin addresses
            foreach (Device d in devices)
            {
                byte[] shrt = GetShort(d.events[0].addr);
                buffer[d.addr + 2] = shrt[0];
                buffer[d.addr + 3] = shrt[1];
                foreach (Device.Event e in d.events)
                {

                }
            }

            EEPROM = buffer;
        }

        public void ExecuteRemoteEvent(ushort _DeviceID, byte _EventID, ushort _EventArgs)
        {
            Write('e');
            WriteShort(_DeviceID);
            Write(_EventID);
            WriteShort(_EventArgs);
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
            client.GetStream().Write(_Buffer, 0, _Buffer.Length);
        }

        public int Available() { return client.Available; }

        public byte[] Read(int _Count)
        {
            byte[] buf = new byte[_Count];
            client.GetStream().Read(buf, 0, _Count);
            return buf;
        }
    }
}
