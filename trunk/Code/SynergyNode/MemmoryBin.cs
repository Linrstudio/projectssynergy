using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SynergyNode
{
    public class MemoryBin
    {
        public ushort DeviceID;//Either virtual or real
        public byte[] Data;

        public MemoryBin(ushort _DeviceID, byte[] _Data) { DeviceID = _DeviceID; Data = _Data; }

        public byte[] GetData()
        {
            return Data;
        }
        public void UpdateMemoryBin()//note that if this is useless if this Memory bin is not a virtual bin
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(BitConverter.GetBytes(DeviceID), 0, 2);
            stream.Write(Data, 0, Data.Length);
            Packet p = new Packet(0, stream.ToArray());
            ConnectionManager.SendPacket(p);//that'll do
        }
    }
}
