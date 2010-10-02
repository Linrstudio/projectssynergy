using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.CodeDom.Compiler;
using System.Reflection;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace Synergy
{
    public class PluginEntry : Attribute { }
    public class PluginTick : Attribute { }

    public class PluginManager
    {
        List<Plugin> Plugins = new List<Plugin>();
        public Plugin[] Loaded { get { return Plugins.ToArray(); } }
        string SourceDirectory;
        public PluginManager(string _SourceDirectory)
        {
            SourceDirectory = _SourceDirectory;
        }

        public void Load()
        {
            foreach (string s in System.IO.Directory.GetFiles(string.Format("Plugins/{0}", SourceDirectory)))
            {
                Plugin plugin = new Plugin(this, System.IO.Path.GetFileNameWithoutExtension(s), File.ReadAllText(s));
                Plugins.Add(plugin);
                if (OnLoad != null) OnLoad(plugin);
            }
        }

        public delegate void OnLoadHandler(Plugin _LoadedPlugin);
        public event OnLoadHandler OnLoad;

        public enum CompilerType { CSharp = 1, VisualBasic = 2 };

        public static Assembly Compile(string _AssemblyName, string _Filename)
        {
            string _PluginSource = File.ReadAllText(_Filename);
            if (Path.GetExtension(_Filename) == ".vb")
                return Compile(_PluginSource, _AssemblyName, PluginManager.CompilerType.VisualBasic);
            else
                return Compile(_PluginSource, _AssemblyName, PluginManager.CompilerType.CSharp);
        }

        public static Assembly Compile(string _AssemblyName, string _Source, PluginManager.CompilerType _Compiler)
        {
            CodeDomProvider compiler = new CSharpCodeProvider();

            CompilerParameters compilerParams = new CompilerParameters();
            compilerParams.GenerateInMemory = true;
            compilerParams.IncludeDebugInformation = true;
            compilerParams.ReferencedAssemblies.Add("system.dll");
            compilerParams.ReferencedAssemblies.Add("synergy.dll");
            compilerParams.GenerateExecutable = true;

            compilerParams.OutputAssembly = string.Format("Binaries/{0}.dll", _AssemblyName);
            CompilerResults results = compiler.CompileAssemblyFromSource(compilerParams, _Source);

            if (results.Errors.Count > 0)
            {
                foreach (CompilerError e in results.Errors)
                {
                    Log.Write("Plugin Compiler", Log.Line.Type.Error, e.ToString());
                }
                return null;
            }
            return results.CompiledAssembly;
        }

        public static void Update()
        {

        }

        public object CreateInstance(string _TypeName, params object[] _Parameters)
        {
            object o = null;
            foreach (Plugin p in Plugins)
            {
                o = p.CreateInstance(_TypeName, _Parameters);
                if (o != null) return o;
            }
            return null;
        }
    }

    public class Plugin
    {
        Assembly assembly;
        string pluginname;
        string source;
        PluginManager.CompilerType type;
        PluginManager manager;

        public string PluginName { get { return pluginname; } }
        public string Source { get { return source; } }
        public PluginManager.CompilerType Type { get { return type; } }
        public Type[] Types { get { if (assembly == null)return new Type[] { }; return assembly.GetTypes(); } }

        public Assembly Assambly { get { return assembly; } }

        public Plugin(PluginManager _Manager, string _PluginName, string _SourceCode)
        {
            type = PluginManager.CompilerType.CSharp;
            assembly = PluginManager.Compile(_PluginName, _SourceCode, type);
            pluginname = _PluginName;
            source = _SourceCode;
            manager = _Manager;
            if (assembly != null)
            {
                foreach (Type t in assembly.GetTypes())
                {
                    Log.Write("Plugin Compiler", "Type compiled {0}", t.FullName);
                }
            }
        }

        public object CreateInstance(string _TypeName, params object[] _Parameters)
        {
            if (assembly == null) return null;
            Type type = assembly.GetType(_TypeName, false, false);
            if (type == null) return null;
            Type[] types = new Type[_Parameters.Length];
            for (int i = 0; i < _Parameters.Length; i++) types[i] = _Parameters[i].GetType();
            object obj = null;
            try
            {
                obj = type.GetConstructor(types).Invoke(_Parameters);
            }
            catch { Log.Write("Plugin Manager", "Failed to create instance of type {0}", _TypeName); }
            return obj;
        }

        public virtual void Update()
        {

        }
    }
}
