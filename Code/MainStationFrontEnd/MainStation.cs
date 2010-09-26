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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Data">should be 2^16 byte long</param>
        public static void EEPROMWrite(byte[] _Data)
        {
            for (int i = 0; i < _Data.Length; i += 32)
            {
                byte[] buffer = new byte[32];
                for (int j = 0; j < 32; j++)
                {
                    buffer[j] = _Data[i + j];
                }
                EEPROMWrite((ushort)i, buffer);
            }
        }

        public static void EEPROMWrite(ushort _Address, byte[] _Data)
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0x03;
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
            buffer[0] = 0x04;
            byte[] shrt = BitConverter.GetBytes((ushort)_Address);
            buffer[1] = shrt[0];
            buffer[2] = shrt[1];
            Write(buffer);

            buffer = Read();
            return buffer[1];
        }
    }
}
