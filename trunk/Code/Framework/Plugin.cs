using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.CodeDom.Compiler;
using System.Reflection;
using SynergyTemplate;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace Framework
{
    public class PluginEntry : Attribute { }
    public class PluginTick : Attribute { }

    public static class PluginManager
    {
        public enum CompilerType { CSharp = 1, VisualBasic = 2 };

        public static List<Plugin> plugins = new List<Plugin>();
        public static Dictionary<string, PluginData> CollectedPluginData = new Dictionary<string, PluginData>();

        public static Plugin LoadPlugin(string _Filename)
        {
            string[] _PluginSource = File.ReadAllLines(_Filename);
            if (Path.GetExtension(_Filename) == ".vb")
                return LoadPlugin(_PluginSource, CompilerType.VisualBasic);
            else
                return LoadPlugin(_PluginSource, CompilerType.CSharp);
        }

        public static Plugin LoadPlugin(string[] _Source, CompilerType _Compiler)
        {
            CodeDomProvider compiler = null;
            switch (_Compiler)
            {
                case CompilerType.VisualBasic: compiler = new VBCodeProvider(); break;
                case CompilerType.CSharp: compiler = new CSharpCodeProvider(); break;
            }

            CompilerParameters compilerParams = new CompilerParameters();
            compilerParams.GenerateInMemory = true;
            compilerParams.IncludeDebugInformation = true;
            compilerParams.ReferencedAssemblies.Add("system.dll");
            compilerParams.ReferencedAssemblies.Add("framework.dll");
            compilerParams.GenerateExecutable = false;
            CompilerResults results = compiler.CompileAssemblyFromSource(compilerParams, _Source);

            if (results.Errors.Count > 0)
            {
                foreach (CompilerError e in results.Errors)
                {
                    Log.Write("Plugin Compiler", Log.Line.Type.Error, e.ToString());
                }
                return null;
            }

            foreach (Type t in results.CompiledAssembly.GetTypes())
            {
                Log.Write("Plugin Compiler", Log.Line.Type.Message, "Type added {0}", t.FullName);
            }

            Plugin p = new Plugin(results.CompiledAssembly);
            plugins.Add(p);
            return p;
        }

        public static void Update()
        {
            foreach (Plugin p in plugins) p.Update();
        }
    }

    public class Plugin
    {
        Assembly assambly;
        public Plugin(Assembly _Assambly)
        {
            assambly = _Assambly;
            foreach (Type t in assambly.GetTypes())
            {
                Console.WriteLine(t.Name);
            }
            InvokeEntries();
        }
        public void InvokeEntries()
        {
            foreach (Type t in assambly.GetTypes())
            {
                foreach (MethodInfo field in t.GetMethods())
                {
                    if (field.IsStatic)
                    {
                        PluginEntry[] attributes = (PluginEntry[])System.Attribute.GetCustomAttributes(field, typeof(PluginEntry));
                        if (attributes.Length > 0) field.Invoke(null, null);
                    }
                }
            }
        }
        public void Update()
        {
            foreach (Type t in assambly.GetTypes())
            {
                foreach (MethodInfo field in t.GetMethods())
                {
                    if (field.IsStatic)
                    {
                        PluginTick[] attributes = (PluginTick[])System.Attribute.GetCustomAttributes(field, typeof(PluginTick));
                        if (attributes.Length > 0) field.Invoke(null, null);
                    }
                }
            }
        }
    }

    public class PluginData
    {
        string pluginname;
        string[] source;
        PluginManager.CompilerType type;

        public PluginData(string _PluginName,string[] _Source, PluginManager.CompilerType _Type)
        {
            pluginname = _PluginName;
            source = _Source;
            type = _Type;
            PluginManager.CollectedPluginData.Add(_PluginName, this);
        }
        
        public string PluginName { get { return pluginname; } }
        public string[] Source { get { return source; } }
        public PluginManager.CompilerType Type { get { return type; } }

        public Plugin Load()
        {
            return PluginManager.LoadPlugin(source, type);
        }
    }
}
