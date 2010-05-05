using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SynergyTemplate;

//refactor me
//maybe plugin based ?

namespace Framework
{
    public class Converter
    {
        public static Dictionary<Type, Converter> Converters = new Dictionary<Type, Converter>();
        static bool initialized = false;

        /// <summary>
        /// Initializes the converter class, you dont have to invoke this method
        /// </summary>
        public static void Initialize()
        {
            if (!initialized)
            {
                AddConverter(new BoolConverter(), typeof(Boolean));
                AddConverter(new ByteConverter(), typeof(Byte));
                AddConverter(new ByteArrayConverter(), typeof(Byte[]));
                AddConverter(new IntConverter(), typeof(Int32));
                AddConverter(new UIntConverter(), typeof(UInt32));
                AddConverter(new UShortConverter(), typeof(UInt16));
                AddConverter(new FloatConverter(), typeof(Single));
                AddConverter(new StringConverter(), typeof(String));
                AddConverter(new StringArrayConverter(), typeof(String[]));
                AddConverter(new ByteStreamConverter(), typeof(ByteStream));
                Log.Write("Converter", "Converter initialized");
                initialized = true;
            }
        }

        static void AddConverter(Converter _Converter, params Type[] _Types)
        {
            foreach (Type t in _Types)
            {
                if (Converters.ContainsKey(t))
                    Log.Write("Converter", "Converter of Type {0} has already been added", t.Name);
                else
                {
                    Converters.Add(t, _Converter);
                    Log.Write("Converter", "Added {0} Converter", _Types[0].Name);
                }
            }
        }

        public static void Write(object _Object, ByteStream _TargetStream)
        {
            Initialize();
            Type t = _Object.GetType();
            if (Converters.ContainsKey(t))
            {
                Converters[typeof(string)].WriteObject(t.FullName, _TargetStream);
                Converters[t].WriteObject(_Object, _TargetStream);
            }
            else
                Log.Write("Converter", "Cant find converter for Type {0}", t.Name);
        }

        public static object Read(ByteStream _Stream)
        {
            Initialize();
            if (_Stream.Length > 0)
            {
                string typename = (string)Converters[typeof(string)].ReadObject(_Stream);
                Type t = Type.GetType(typename);
                if (t != null)
                {
                    if (Converters.ContainsKey(t)) return Converters[t].ReadObject(_Stream);
                }
                else { Log.Write("Converter", "Cant find Type {0}", typename); }
            }
            else { Log.Write("Converter", "Cant read from stream, stream is empty"); }
            return null;
        }

        public virtual void WriteObject(object _Object, ByteStream _TargetStream) { }
        public virtual object ReadObject(ByteStream _Stream) { return null; }

        public class ByteStreamConverter : Converter
        {
            public override void WriteObject(object _Object, ByteStream _TargetStream)
            {
                ByteStream obj = (ByteStream)_Object;
                Converter.Converters[typeof(ushort)].WriteObject((ushort)obj.Length, _TargetStream);
                _TargetStream.Write(((ByteStream)_Object).CopyAll());
            }
            public override object ReadObject(ByteStream _TargetStream)
            {
                ushort length = (ushort)Converter.Converters[typeof(ushort)].ReadObject(_TargetStream);
                return new ByteStream(_TargetStream.Read(length));
            }
        }

        public class IntConverter : Converter
        {
            public override void WriteObject(object _Object, ByteStream _TargetStream)
            {
                _TargetStream.Write(BitConverter.GetBytes((int)_Object));
            }
            public override object ReadObject(ByteStream _TargetStream)
            {
                return BitConverter.ToInt32(_TargetStream.Read(4), 0);
            }
        }

        public class BoolConverter : Converter
        {
            public override void WriteObject(object _Object, ByteStream _TargetStream)
            {
                _TargetStream.Write((byte)(((bool)_Object) ? 255 : 0));
            }
            public override object ReadObject(ByteStream _TargetStream)
            {
                return _TargetStream.Read() != 0;
            }
        }

        public class ByteConverter : Converter
        {
            public override void WriteObject(object _Object, ByteStream _TargetStream)
            {
                _TargetStream.Write((byte)_Object);
            }
            public override object ReadObject(ByteStream _TargetStream)
            {
                return _TargetStream.Read();
            }
        }

        public class ByteArrayConverter : Converter
        {
            public override void WriteObject(object _Object, ByteStream _TargetStream)
            {
                byte[] obj = (byte[])_Object;
                Converter.Converters[typeof(ushort)].WriteObject((ushort)obj.Length, _TargetStream);
                _TargetStream.Write((byte[])_Object);
            }
            public override object ReadObject(ByteStream _TargetStream)
            {
                ushort len = (ushort)Converter.Converters[typeof(ushort)].ReadObject(_TargetStream);
                return _TargetStream.Read(len);
            }
        }

        public class UIntConverter : Converter
        {
            public override void WriteObject(object _Object, ByteStream _TargetStream)
            {
                _TargetStream.Write(BitConverter.GetBytes((uint)_Object));
            }
            public override object ReadObject(ByteStream _TargetStream)
            {
                return BitConverter.ToUInt32(_TargetStream.Read(4), 0);
            }
        }

        public class UShortConverter : Converter
        {
            public override void WriteObject(object _Object, ByteStream _TargetStream)
            {
                ushort obj = (ushort)_Object;
                _TargetStream.Write(BitConverter.GetBytes((ushort)_Object));
            }
            public override object ReadObject(ByteStream _TargetStream)
            {
                return BitConverter.ToUInt16(_TargetStream.Read(2), 0);
            }
        }

        public class FloatConverter : Converter
        {
            public override void WriteObject(object _Object, ByteStream _TargetStream)
            {
                _TargetStream.Write(BitConverter.GetBytes((Single)_Object));
            }
            public override object ReadObject(ByteStream _TargetStream)
            {
                return BitConverter.ToSingle(_TargetStream.Read(4), 0);
            }
        }

        public class StringConverter : Converter
        {
            public override void WriteObject(object _Object, ByteStream _TargetStream)
            {
                string obj = (string)_Object;
                Converter.Converters[typeof(ushort)].WriteObject((ushort)obj.Length, _TargetStream);
                foreach (char c in (string)_Object) _TargetStream.Write((byte)c);
            }
            public override object ReadObject(ByteStream _TargetStream)
            {
                ushort len = (ushort)Converter.Converters[typeof(ushort)].ReadObject(_TargetStream);
                return System.Text.Encoding.ASCII.GetString(_TargetStream.Read(len));
            }
        }

        public class StringArrayConverter : Converter
        {
            public override void WriteObject(object _Object, ByteStream _TargetStream)
            {
                string[] strings = (string[])_Object;
                Converter.Converters[typeof(ushort)].WriteObject(strings.Length, _TargetStream);
                foreach (string s in strings)
                {
                    Converter.Write(s, _TargetStream);
                }
                foreach (char c in (string)_Object) _TargetStream.Write((byte)c);
            }
            public override object ReadObject(ByteStream _TargetStream)
            {
                ushort len = (ushort)Converter.Converters[typeof(ushort)].ReadObject(_TargetStream);
                string[] strings = new string[len];
                for (int i = 0; i < len; i++)
                {
                    strings[i] = (string)Converter.Read(_TargetStream);
                }
                return strings;
            }
        }
    }
}
