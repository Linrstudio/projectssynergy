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
            return new byte[] { 0x01, _Reg, value[0], value[1] };//4
        }
        public static byte[] Load8(byte _Reg, byte _Value)
        {
            return new byte[] { 0x02, _Reg, _Value};//4
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

        public static byte[] GetHour(byte _RegA)
        {
            return new byte[] { 0x30, _RegA };
        }
        public static byte[] GetMinute(byte _RegA)
        {
            return new byte[] { 0x31, _RegA };
        }
        public static byte[] GetSecond(byte _RegA)
        {
            return new byte[] { 0x32, _RegA };
        }

        public static byte[] SetLED(byte _Reg)
        {
            return new byte[] { 0x0a, _Reg };//2
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_FromReg">Source register</param>
        /// <param name="_ToReg">Target register</param>
        /// <param name="_Amount">amount of Registers to copy</param>
        /// <returns></returns>
        public static byte[] Mov(byte _FromReg, byte _ToReg, byte _Amount)//move a register's value to another register
        {
            return new byte[] { 0x70, _FromReg, _ToReg, _Amount };//2
        }

        public static byte[] EPSend(ushort _DeviceID, byte _EPBufferSize)
        {
            byte[] value = Utilities.FromShort(_DeviceID);
            return new byte[] { 0x71, value[0], value[1], _EPBufferSize };
        }
    }
}
