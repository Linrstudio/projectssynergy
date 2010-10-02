using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MainStationFrontEnd
{
    public class CodeInstructions
    {
        public static byte[] Load(byte _Reg, ushort _Value)
        {
            byte[] value = Utilities.FromShort(_Value);
            return new byte[] { 0x01, _Reg, value[1], value[0] };//4
        }
        public static byte[] Equals(byte _RegA, byte _RegB, byte _RegAnswer)
        {
            return new byte[] { 0x0b, _RegA, _RegB, _RegAnswer };//4
        }
        public static byte[] Differs(byte _RegA, byte _RegB, byte _RegAnswer)
        {
            return new byte[] { 0x0c, _RegA, _RegB, _RegAnswer };//4
        }
        public static byte[] And(byte _RegA, byte _RegB, byte _RegAnswer)
        {
            return new byte[] { 0x0d, _RegA, _RegB, _RegAnswer };//4
        }
        public static byte[] Or(byte _RegA, byte _RegB, byte _RegAnswer)
        {
            return new byte[] { 0x0e, _RegA, _RegB, _RegAnswer };//4
        }
        public static byte[] Xor(byte _RegA, byte _RegB, byte _RegAnswer)
        {
            return new byte[] { 0x0e, _RegA, _RegB, _RegAnswer };//4
        }
        public static byte[] Add(byte _RegA, byte _RegB, byte _RegAnswer)
        {
            return new byte[] { 0x20, _RegA, _RegB, _RegAnswer };//4
        }
        public static byte[] Substract(byte _RegA, byte _RegB, byte _RegAnswer)
        {
            return new byte[] { 0x21, _RegA, _RegB, _RegAnswer };//4
        }
        public static byte[] Multiply(byte _RegA, byte _RegB, byte _RegAnswer)
        {
            return new byte[] { 0x22, _RegA, _RegB, _RegAnswer };//4
        }
        public static byte[] Divide(byte _RegA, byte _RegB, byte _RegAnswer)
        {
            return new byte[] { 0x23, _RegA, _RegB, _RegAnswer };//4
        }
        public static byte[] SetLED(byte _Reg)
        {
            return new byte[] { 0x0a, _Reg };//2
        }
        public static byte[] MovKismetEP(byte _Reg, byte _EPBufferIdx)//move a register's value to the EP send buffer
        {
            return new byte[] { 0x70, _Reg, _EPBufferIdx };//2
        }
        public static byte[] MovEPKismet(byte _Reg, byte _EPBufferIdx)//move a register's value to the EP send buffer
        {
            return new byte[] { 0x71, _Reg, _EPBufferIdx };//2
        }
    }
}
