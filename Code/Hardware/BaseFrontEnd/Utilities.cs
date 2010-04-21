using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace BaseFrontEnd
{
    public static class Utilities
    {
        public static PointF Add(PointF _A, PointF _B)
        {
            return new PointF(_A.X + _B.X, _A.Y + _B.Y);
        }

        public static PointF Avarage(PointF _A, PointF _B)
        {
            return new PointF(_A.X + _B.X / 2, _A.Y + _B.Y / 2);
        }

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
