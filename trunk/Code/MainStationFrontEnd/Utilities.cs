using System;
using System.Collections.Generic;
using System.Text;

namespace MainStationFrontEnd
{
    public static class Utilities
    {
        public static byte[] FromShort(ushort _V)
        {
            byte[] buf = BitConverter.GetBytes(_V);
            return new byte[] { buf[1], buf[0] };
        }

        public static ushort ToShort(byte[] _Buffer, int _Index)
        {
            return BitConverter.ToUInt16(new byte[] { _Buffer[_Index + 1], _Buffer[_Index] }, 0);
        }

        public static byte[] Cut(byte[] _Buffer, int _Index)
        {
            byte[] buffer = new byte[_Buffer.Length - _Index];
            for (int i = 0; i < _Index; i++)
            {
                buffer[i] = _Buffer[i + _Index];
            }
            return buffer;
        }
    }
}
