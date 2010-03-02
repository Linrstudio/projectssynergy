//#define CONSOLESTUFF

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
                port = new SerialPort(_PortName, 19200, Parity.None, 8, StopBits.One);
                port.Open();

                Write('h');//hello?
                WaitForY();
                eepromsize = ReadShort();
            }
            catch { port = null; Console.WriteLine("Failed to open port : {0}", _PortName); }
            eeprom = new EEPROM(eepromsize);
            eepromdata = new byte[eepromsize];
            Console.WriteLine("EEPROM Size : {0}", eeprom.Size);
        }

        public void SendCommand(char _Category, char _Command)
        {
            Write(_Category);
            WaitForY();
            Write(_Command);
        }

        public void KismetEnable()
        {
            SendCommand('k', 'e');
        }
        public void KismetDisable()
        {
            SendCommand('k', 'd');
        }

        public void ReadVariables()
        {
            SendCommand('k', 'v');
            Console.WriteLine();
            for (int i = 0; i < 64; i++)
            {
                Console.Write(ReadShort().ToString("00000 "));
            }
        }

        public void PLCWrite(byte _b)
        {
            SendCommand('p', 'w');
            Write(_b);
        }

        public byte PLCRead()
        {
            SendCommand('p', 'r');
            return Read(1)[0];
        }

        public void AssambleEEPROM()
        {
            eepromdata = eeprom.Assamble();
        }

        public void PrintEEPROMData()
        {
            for (int i = 0; i < eepromdata.Length; i++)
            {
                Console.Write("{0} ", eepromdata[i].ToString("000"));
            }
        }

        public bool CheckEEPROM()
        {
            Random random = new Random(Environment.TickCount);

            int size = eeprom.Size;//size=256;
            byte[] from = new byte[size];
            byte[] to = new byte[size];
            //fill memory randomly
            for (int i = 0; i < from.Length; i++)
                from[i] = (byte)random.Next(256);

            from[0] = from[1] = 0;

            //disable kismet execution
            KismetDisable();

            //upload eeprom
            for (ushort b = 0; b < from.Length; b += 16)
            {
                SendCommand('m', 'w');
                WriteShort(b);
                for (int i = 0; i < 16; i++)
                {
                    Write(new byte[] { from[i + b] });
                }
            }
            //download eeprom
            for (ushort b = 0; b < to.Length; b += 16)
            {
                SendCommand('m', 'r');
                WriteShort(b);
                for (int i = 0; i < 16; i++)
                {
                    to[i + b] = Read(1)[0];
                }
            }

            //enable kismet execution
            KismetEnable();
            return Compare(to, from);
        }

        public bool Compare(byte[] a, byte[] b)
        {
            bool okay = true;
            //check if memory is still sequential
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) okay = false;
                Console.ForegroundColor = (a[i] == b[i]) ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine("{0} > {1} | {2}", i.ToString("000"), a[i].ToString("000"), b[i].ToString("000"));
            }
            return okay;
        }

        public void DownloadEEPROM()
        {
            byte[] buf = new byte[eeprom.Size];
            for (ushort b = 0; b < eepromdata.Length; b += 16)
            {
                SendCommand('m', 'r');
                WriteShort(b);
                for (int i = 0; i < 16; i++)
                {
                    buf[i + b] = Read(1)[0];
                }
            }
            Compare(eepromdata, buf);
            //eeprom = EEPROM.FromEEPROM(eepromdata);
        }

        public void UploadEEPROM()
        {
            AssambleEEPROM();
            KismetDisable();
            for (ushort b = 0; b < eeprom.BytesUsed; b += 16)
            {
                SendCommand('m', 'w');
                WriteShort(b);
                for (int i = 0; i < 16; i++)
                {
                    Write(new byte[] { eepromdata[i + b] });
                }
                //while (Read(1)[0] != (byte)'w') ;
            }
            KismetEnable();
        }

        public void UploadEEPROMBruteForce()
        {
            AssambleEEPROM();
            KismetDisable();
            for (ushort b = 0; b < eeprom.Size; b += 16)
            {
                SendCommand('m', 'w');
                WriteShort(b);
                for (int i = 0; i < 16; i++)
                {
                    Write(new byte[] { eepromdata[i + b] });
                }
                //while (Read(1)[0] != (byte)'w') ;
            }
            KismetEnable();
        }

        public string ReadTime()
        {
            SendCommand('t', 'r');
            byte hour = Read(1)[0];
            byte minute = Read(1)[0];
            byte second = Read(1)[0];
            byte day = Read(1)[0];

            Console.WriteLine("[{0}:{1}:{2} day:{3}]", hour, minute, second, (DayOfWeek)day);
            return string.Format("{3} {0}:{1}:{2}", hour.ToString("00"), minute.ToString("00"), second.ToString("00"), (DayOfWeek)day);
        }

        public void SetTime(DateTime _Time)
        {
            SendCommand('t', 'w');

            Write((byte)_Time.Hour);
            Write((byte)_Time.Minute);
            Write((byte)_Time.Second);
            Write((byte)((int)_Time.DayOfWeek));
        }

        public void ExecuteRemoteEvent(ushort _DeviceID, byte _EventID, ushort _EventArgs)
        {
            SendCommand('k', 'x');
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
            byte[] buf = _Buffer;
            port.Write(_Buffer, 0, _Buffer.Length);
#if CONSOLESTUFF
            if (Console.ForegroundColor != ConsoleColor.Green) Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (byte b in buf)
            {
                Console.Write(b.ToString() + " ");
            }
#endif
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
            while (Available() < _Count) ;
            byte[] buf = new byte[_Count];
            if (port != null) port.Read(buf, 0, _Count);
#if CONSOLESTUFF
            if (Console.ForegroundColor != ConsoleColor.Red) Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (byte b in buf)
            {
                Console.Write(b.ToString() + " ");
            }
#endif
            return buf;
        }
    }
}
