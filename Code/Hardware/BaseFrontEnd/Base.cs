using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;

namespace BaseFrontEnd
{
    public class Base
    {
        SerialPort port = null;

        public EEPROM eeprom = null;

        byte[] eepromdata;

        public Base(string _PortName)
        {
            ushort eepromsize = 256;
            try
            {
                port = new SerialPort(_PortName, 1200, Parity.None, 8, StopBits.One);
                port.Open();

                Write('h');//hello?
                WaitForY();
                eepromsize = ReadShort();
            }
            catch { port = null; Console.WriteLine("Failed to open port:{0}", _PortName); }
            eeprom = new EEPROM(eepromsize);
            eepromdata = new byte[eepromsize];
        }

        public void KismetEnable()
        {
            Write('k');
            WaitForY();
            Write('e');
        }
        public void KismetDisable()
        {
            Write('k');
            WaitForY();
            Write('d');
        }

        public void DownloadEEPROM()
        {
            for (ushort b = 0; b < eepromdata.Length; b += 16)
            {
                Write('m');
                WaitForY();
                Write('r');
                WriteShort(b);
                for (int i = 0; i < 16; i++)
                {
                    eepromdata[i + b] = Read(1)[0];
                }
                eeprom = EEPROM.FromEEPROM(eepromdata);
            }
        }

        public void UploadEEPROM()
        {
            eepromdata = eeprom.Assamble();
            KismetDisable();
            for (ushort b = 0; b < eeprom.BytesUsed; b += 16)
            {
                Write('m');
                WaitForY();
                Write('w');
                WriteShort(b);
                for (int i = 0; i < 16; i++)
                {
                    Write(new byte[] { eepromdata[i + b] });
                }
                while (Read(1)[0] != (byte)'w') ;
            }
            KismetEnable();
        }

        public void ReadTime()
        {
            Write('t');
            WaitForY();
            Write('r');
            byte hour = Read(1)[0];
            byte minute = Read(1)[0];
            byte second = Read(1)[0];
            byte day = Read(1)[0];

            Console.WriteLine("[{0}:{1}:{2} day:{3}]", hour, minute, second, (DayOfWeek)day);
        }

        public void SetTime(DateTime _Time)
        {
            Write('t');
            WaitForY();
            Write('w');

            Write((byte)_Time.Hour);
            Write((byte)_Time.Minute);
            Write((byte)_Time.Second);
            Write((byte)((int)_Time.DayOfWeek));
        }

        public void ExecuteRemoteEvent(ushort _DeviceID, byte _EventID, byte _EventArgs)
        {
            Write('k');
            WaitForY();
            Write('x');
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

        public void WaitForY()
        {
            while (Read(1)[0] != 121) ;
        }

        public int Available() 
        {
            if (port == null) return 0;
            return port.BytesToRead; 
        }

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
