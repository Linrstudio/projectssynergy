using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class Converter
    {
        public static Dictionary<Type, Converter> Converters = new Dictionary<Type, Converter>();
        static bool initialized = false;
        static void Initialize()
        {
            if (!initialized)
            {
                AddConverter(new ByteConverter(), typeof(Byte));
                AddConverter(new ByteArrayConverter(), typeof(Byte[]));
                AddConverter(new IntConverter(), typeof(Int32));
                AddConverter(new UIntConverter(), typeof(UInt32));
                AddConverter(new UShortConverter(), typeof(UInt16));
                AddConverter(new FloatConverter(), typeof(Single));
                AddConverter(new StringConverter(), typeof(String));
                AddConverter(new ByteStreamConverter(), typeof(ByteStream));
                Console.WriteLine("Converter initialized");
                initialized = true;
            }
        }

        static void AddConverter(Converter _Converter, params Type[] _Types)
        {
            foreach (Type t in _Types)
            {
                if (Converters.ContainsKey(t))
                    Console.WriteLine("Converter of Type {0} has already been added", t.Name);
                else
                    Converters.Add(t, _Converter);
            }
        }

        public static void Write(object _Object, ByteStream _TargetStream)
        {
            Initialize();
            Type t = _Object.GetType();
            if (Converters.ContainsKey(t))
                Converters[t].WriteObject(_Object, _TargetStream);
            else
                Console.WriteLine("Cant find converter for Type {0}", t.Name);
        }

        public static object Read(Type _Type, ByteStream _Stream)
        {
            Initialize();
            if (Converters.ContainsKey(_Type)) return Converters[_Type].ReadObject(_Stream);
            return null;
        }

        public virtual void WriteObject(object _Object, ByteStream _TargetStream) { }
        public virtual object ReadObject(ByteStream _Stream) { return null; }


        public class ByteStreamConverter : Converter
        {
            public override void WriteObject(object _Object, ByteStream _TargetStream)
            {
                _TargetStream.Write(BitConverter.GetBytes((ushort)(((ByteStream)_Object).GetSize())));
                _TargetStream.Write(((ByteStream)_Object).ReadAll());
            }
            public override object ReadObject(ByteStream _TargetStream)
            {
                ushort len = BitConverter.ToUInt16(_TargetStream.Read(2), 0);
                return new ByteStream(_TargetStream.Read(len));
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
                _TargetStream.Write(BitConverter.GetBytes((ushort)(((byte[])_Object).Length)));
                _TargetStream.Write((byte[])_Object);
            }
            public override object ReadObject(ByteStream _TargetStream)
            {
                ushort len = BitConverter.ToUInt16(_TargetStream.Read(2), 0);
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
                _TargetStream.Write(BitConverter.GetBytes((ushort)((string)_Object).Length));
                foreach (char c in (string)_Object) _TargetStream.Write((byte)c);
            }
            public override object ReadObject(ByteStream _TargetStream)
            {
                int len = BitConverter.ToUInt16(_TargetStream.Read(2), 0);
                return System.Text.Encoding.ASCII.GetString(_TargetStream.Read(len));
            }
        }
    }
}
