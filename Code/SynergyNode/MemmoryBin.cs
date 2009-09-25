using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

namespace SynergyNode
{
    public class MemoryBin
    {
        public virtual string GetState() { return ""; }
        public static MemoryBin GetBinForType(byte _Type)
        {
            switch (_Type)
            {
                case 0://DefaultMemoryBin
                    return new DefaultMemoryBin();
                case 10://DigitalOutput
                    return new DigitalMemoryBin();
                case 11://DigitalInput
                    return new DigitalMemoryBin();
                case 12://AnalogOutput
                    return new AnalogMemoryBin();
                case 13://AnalogInput
                    return new AnalogMemoryBin();
            }
            return new DefaultMemoryBin();
        }
        public static MemoryBin FromBytes(byte _Type,byte[] _Bytes)
        {
            MemoryBin b=GetBinForType(_Type);
            if (b == null) return null;
            Type t = b.GetType();
            int index = 0;
            foreach (FieldInfo i in t.GetFields())
            {
                string name = i.FieldType.Name;
                try
                {
                    if (name == typeof(byte).Name)
                    {
                        i.SetValue(b, _Bytes[index]);
                        index += sizeof(byte);
                    }
                    if (name == typeof(bool).Name)
                    {
                        i.SetValue(b, _Bytes[index] != 0);
                        index += sizeof(byte);
                    }
                }
                catch { }
            }
            return b;
        }
        public static byte[] ToBytes(MemoryBin _MemoryBin)
        {
            if (_MemoryBin == null) return new byte[] { };
            MemoryStream s = new MemoryStream();
            Type t = _MemoryBin.GetType();
            foreach (FieldInfo i in t.GetFields())
            {
                string name = i.FieldType.Name;
                try
                {
                    if (name == typeof(byte).Name)
                    {
                        s.WriteByte((byte)i.GetValue(_MemoryBin));
                    }
                    if (name == typeof(bool).Name)
                    {
                        s.WriteByte((byte)((bool)i.GetValue(_MemoryBin) ? 255 : 0));
                    }
                }
                catch { }
            }
            return s.GetBuffer();
        }
    }

    public class DefaultMemoryBin : MemoryBin 
    {
        public override string GetState()
        {
            return "Default";
        }
    }

    public class AnalogMemoryBin : MemoryBin
    {
        public byte Value;

        public override string GetState()
        {
            return (Value / 2.56).ToString() + "%";
        }
    }

    public class DigitalMemoryBin : MemoryBin
    {
        public bool On;
        public bool Inversed;

        public void Toggle()
        {
            On = !On;
        }
        public override string GetState()
        {
            return (On ? "On" : "Off") + " - " + (Inversed ? "" : "Not ") + "Inversed";
        }
    }
}
