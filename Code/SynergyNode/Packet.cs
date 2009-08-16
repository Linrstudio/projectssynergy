using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SynergyNode
{
    public class Packet
    {
        public static int DataOffset = 4 + 1 + 2;
        public uint PacketID;
        public byte Type;
        public byte[] Data;
        public Packet() { }
        public Packet(Stream _Stream)
        {
            byte[] buffer = new byte[4];
            _Stream.Read(buffer, 0, 4);
            PacketID = BitConverter.ToUInt32(buffer, 0);
            Type = (byte)_Stream.ReadByte();
            _Stream.Read(buffer,0,2);
            ushort length = BitConverter.ToUInt16(buffer, 0);
            Data = new byte[length];
            _Stream.Read(Data, 0, length);
        }
        public Packet(byte[] _PacketBytes) 
        {
            PacketID = BitConverter.ToUInt32(_PacketBytes, 0);
            Type = _PacketBytes[4];
            ushort length = BitConverter.ToUInt16(_PacketBytes, 5);
            Data = new byte[length];
            for (int i = 0; i < length; i++) Data[i] = _PacketBytes[i + 7];
        }
        public Packet(byte _Type,byte[] _Data)
        {
            Type = _Type;
            Data = _Data;
            PacketID = (uint)ConnectionManager.random.Next() + (uint)ConnectionManager.random.Next();
        }
        public byte[] GetPacketBytes()
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(BitConverter.GetBytes(PacketID), 0, 4);
            stream.Write(new byte[] { Type }, 0, 1);
            stream.Write(BitConverter.GetBytes((ushort)Data.Length), 0, 2);
            stream.Write(Data,0,Data.Length);
            return stream.ToArray();
        }
        public byte[] GetPiece(uint _Start,uint _Count)
        {
            byte[] ret = new byte[_Count];
            for (int i = 0; i < _Count; i++)
                ret[i] = Data[i + _Start];
            return ret;
        }
    }
}
