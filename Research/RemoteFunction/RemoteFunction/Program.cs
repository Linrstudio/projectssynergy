using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace RemoteFunction
{
    public class Converter
    {
        public static Dictionary<Type, Converter> Converters = new Dictionary<Type, Converter>();
        public virtual void Write(object _Object, MemoryStream _TargetStream) 
        {
            Type t = _Object.GetType();
            if (Converters.ContainsKey(t)) Converters[t].Write(_Object, _TargetStream);
        }
        public virtual object Read(Type _Type, MemoryStream _Stream)
        {
            if (Converters.ContainsKey(_Type)) return Converters[_Type].Read(_Type, _Stream);
            return null;
        }
    }

    public class IntConverter:Converter
    {
        public override void Write(object _Object, MemoryStream _TargetStream)
        {
            
        }
    }

    public static class Container
    {
        public static void Call(string _FunctionName, object[] _Arguments)
        {
            MethodInfo method = typeof(Container).GetMethod(_FunctionName);
            if (method == null)
                Console.WriteLine("Method {0} does not exists", _FunctionName);
            else
                method.Invoke(null, _Arguments);
        }

        public static byte[] GetCall(string _FunctionName, object[] _Arguments)
        {
            MemoryStream stream = new MemoryStream();
            MemberInfo[] members = typeof(LocalFunctions).GetMethods();
            int functionindex=-1;
            for (int index = 0; index < members.Length; index++)
            {
                if (members[index].Name == _FunctionName)
                {
                    functionindex = index;
                    break;
                }
            }
            if (functionindex == -1)
            {
                Console.WriteLine("no function with name {0} found", _FunctionName);
                return new byte[]{};
            }
            stream.Write(BitConverter.GetBytes((ushort)functionindex), 0, 2);//write function index
            foreach (object o in _Arguments)
            {

            }

            return stream.ToArray();
        }

        public static byte[] ToBytes(object _Instance)
        {
            MemoryStream stream = new MemoryStream();
            Type t = _Instance.GetType();
            switch (t.FullName)
            {
                case "System.Byte":
                {
                    stream.WriteByte(1);
                    stream.WriteByte((byte)_Instance);
                    break;
                }
                case "System.int":
                {
                    stream.WriteByte(2);
                    stream.WriteByte((byte)_Instance);
                    break;
                }
            }
            return stream.ToArray();
        }

        public static class LocalFunctions
        {
            public static void Myfunction(float _MyArgument)
            {
                Console.WriteLine(_MyArgument);
            }
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            var func = typeof(Program).GetMethod("Main");
            ParameterInfo[] arguments=func.GetParameters();
            Console.WriteLine(typeof(float).GUID);
            Container.Call("Myfunction", new object[]{34.0f});
            Console.Read();
        }
    }
}
