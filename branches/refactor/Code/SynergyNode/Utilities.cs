using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

namespace SynergyNode
{
    public class Utilities
    {
        
        public static Dictionary<Type,List<object>> LoadSettingsFile(string _Path, Type[] _Types)
        {
            var dictionary = new Dictionary<Type, List<object>>();
            XElement root = XElement.Load(_Path);
            foreach (XElement sect in root.Elements())//loop over all sections
            {
                string name = sect.Name.ToString();
                Type type = null;
                foreach (Type t in _Types) if (t.Name == name) type = t;
                if (type != null)
                {
                    Console.WriteLine("Match for type {0} found!", name);
                    Object instance = null;
                    try
                    {
                        instance = type.GetConstructor(new Type[] { }).Invoke(new object[] { });//create an instance of our type using its default contructor
                    }
                    catch { }
                    if (instance != null)
                    {
                        foreach (XElement f in sect.Elements())
                        {
                            foreach (FieldInfo i in type.GetFields())
                            {
                                if (i.Name == f.Name)
                                {
                                    try
                                    {
                                        object o = i.FieldType.GetMethod("Parse", new Type[] { f.Value.GetType() }).Invoke(i, new object[] { (object)f.Value });
                                        i.SetValue(instance, o);
                                        Console.WriteLine("Field {0} set to value {1}", f.Name, f.Value);
                                    }
                                    catch { Console.WriteLine("Could not Parse data. for {0}", f.Name); }
                                }
                            }
                        }

                        if (!dictionary.ContainsKey(type)) dictionary.Add(type, new List<object>());
                        dictionary[type].Add(instance);
                    }
                    else { Console.WriteLine("Could not find default constructor"); }
                }
                else 
                {
                    Console.Write("could not find {0} type in type list :", name);
                    foreach (Type t in _Types) Console.Write(t.Name + " ");
                    Console.WriteLine();
                }
            }
            return dictionary;
        }
    }
}
