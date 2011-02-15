using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public static class Utilities
    {
        public static byte[] FromShort(ushort _V)
        {
            byte[] buf = BitConverter.GetBytes(_V);
            return new byte[] { buf[0], buf[1] };
        }

        public static ushort ToShort(byte[] _Buffer, int _Index)
        {
            return BitConverter.ToUInt16(new byte[] { _Buffer[_Index], _Buffer[_Index + 1] }, 0);
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

        public static ushort GetDay(DateTime _Date)
        {
            return (ushort)TimeSpan.FromTicks(_Date.Ticks - new DateTime(2000, 1, 1).Ticks).Days;
        }
    }
}
