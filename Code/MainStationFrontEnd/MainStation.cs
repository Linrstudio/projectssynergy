using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace MainStationFrontEnd
{
    class MainStation
    {
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

        public static void Poll(ushort _DeviceID)
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x01;//send raw data to devices
            byte[] shrt = BitConverter.GetBytes((ushort)_DeviceID);
            buffer[1] = shrt[0];
            buffer[2] = shrt[1];
            Write(buffer);
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
            public TimeSpan DayTime;
            public DayOfWeek Day;
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
            t.Day = (DayOfWeek)(int)buffer[4];
            return t;
        }

        public static void TimeWrite()
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x41;
            buffer[1] = (byte)DateTime.Now.Second;
            buffer[2] = (byte)DateTime.Now.Minute;
            buffer[3] = (byte)DateTime.Now.Hour;
            buffer[4] = (byte)(int)DateTime.Now.DayOfWeek;
            Write(buffer);
            System.Threading.Thread.Sleep(10);
            buffer = Read();
        }
    }
}
