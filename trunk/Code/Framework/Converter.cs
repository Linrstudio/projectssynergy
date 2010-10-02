using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//refactor me
//maybe plugin based ?

namespace Synergy
{
    public abstract class Converter
    {
        public abstract Type Converts { get; }
        public static Dictionary<Type, Converter> Converters = new Dictionary<Type, Converter>();
        static bool initialized = false;

        public static PluginManager pluginmanager = new PluginManager("Converters");

        /// <summary>
        /// Initializes the converter class, you dont have to invoke this method
        /// </summary>
        public static void Initialize()
        {
            if (!initialized)
            {
                pluginmanager.OnLoad += new PluginManager.OnLoadHandler(pluginmanager_OnLoad);
                pluginmanager.Load();
                Log.Write("Converter", "Converter initialized");
                initialized = true;
            }
        }

        static void pluginmanager_OnLoad(Plugin _LoadedPlugin)
        {
            foreach (Type t in _LoadedPlugin.Types)
            {
                if (t.BaseType == typeof(Converter))
                {
                    Converter converter = (Converter)_LoadedPlugin.CreateInstance(t.FullName);
                    if (converter != null)
                    {
                        AddConverter(converter, converter.Converts);
                    }
                }
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
                    Log.Write("Converter", "Added {0} Converter", t.Name);
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

        public abstract void WriteObject(object _Object, ByteStream _TargetStream);
        public abstract object ReadObject(ByteStream _Stream);
    }
}
