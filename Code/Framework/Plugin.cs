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
        /// <summary>
        /// Plugins that can be sent to clients to interact with the local plugin on the server
        /// </summary>
        public static Dictionary<string, SlavePlugin> SlavePlugins = new Dictionary<string, SlavePlugin>();


        public static Assembly Compile(string _Filename)
        {
            string _PluginSource = File.ReadAllText(_Filename);
            if (Path.GetExtension(_Filename) == ".vb")
                return Compile(_PluginSource, PluginManager.CompilerType.VisualBasic);
            else
                return Compile(_PluginSource, PluginManager.CompilerType.CSharp);
        }

        public static Assembly Compile(string _Source, PluginManager.CompilerType _Compiler)
        {
            CodeDomProvider compiler = null;
            switch (_Compiler)
            {
                case PluginManager.CompilerType.VisualBasic: compiler = new VBCodeProvider(); break;
                case PluginManager.CompilerType.CSharp: compiler = new CSharpCodeProvider(); break;
            }

            CompilerParameters compilerParams = new CompilerParameters();
            compilerParams.GenerateInMemory = true;
            compilerParams.IncludeDebugInformation = true;
            compilerParams.ReferencedAssemblies.Add("system.dll");
            compilerParams.ReferencedAssemblies.Add("framework.dll");
            compilerParams.ReferencedAssemblies.Add("template.dll");
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
            return results.CompiledAssembly;
        }

        public static void Update()
        {
            foreach (Plugin p in plugins) p.Update();
        }
    }

    public class Plugin
    {
        Assembly assambly;
        string pluginname;
        string source;
        PluginManager.CompilerType type;

        public string PluginName { get { return pluginname; } }
        public string Source { get { return source; } }
        public PluginManager.CompilerType Type { get { return type; } }
        public Type[] Types { get { return assambly.GetTypes(); } }

        public Assembly Assambly { get { return assambly; } }

        public Plugin(string _PluginName, string _SourceCode)
        {
            type = PluginManager.CompilerType.CSharp;
            assambly = PluginManager.Compile(_SourceCode, type);
            pluginname = _PluginName;
            source = _SourceCode;
            foreach (Type t in assambly.GetTypes())
            {
                Log.Write("Plugin Compiler", "Type compiled {0}", t.FullName);
            }
            PluginManager.plugins.Add(this);
            CheckPluginAttributes();
        }

        public Plugin(string _PluginName, string _SourceCode, PluginManager.CompilerType _CompilerType)
        {
            type = _CompilerType;
            assambly = PluginManager.Compile(_SourceCode, type);
            pluginname = _PluginName;
            source = _SourceCode;
            foreach (Type t in assambly.GetTypes())
            {
                Log.Write("Plugin Compiler", "Type compiled {0}", t.FullName);
            }
            PluginManager.plugins.Add(this);
            CheckPluginAttributes();
        }

        public void CheckPluginAttributes()
        {
            foreach (Type t in Assambly.GetTypes())
            {
                foreach (MethodInfo method in t.GetMethods())
                {
                    if (System.Attribute.GetCustomAttributes(method, typeof(PluginEntry)).Length > 0)
                    {
                        if (!method.IsStatic) Log.Write("Plugin Compiler", "Method {0} should be static", method.Name);
                    }
                    if (System.Attribute.GetCustomAttributes(method, typeof(PluginTick)).Length > 0)
                    {
                        if (!method.IsStatic) Log.Write("Plugin Compiler", "Method {0} should be static", method.Name);
                    }
                }
            }
        }

        public virtual void Update()
        {

        }
    }

    public class MasterPlugin : Plugin
    {
        public MasterPlugin(string _PluginName, string _SourceCode)
            : base(_PluginName, _SourceCode)
        {
            InvokeEntries();
        }

        public void InvokeEntries()
        {
            foreach (Type t in Assambly.GetTypes())
            {
                foreach (MethodInfo field in t.GetMethods())
                {
                    if (field.IsStatic)
                    {
                        PluginEntry[] attributes = (PluginEntry[])System.Attribute.GetCustomAttributes(field, typeof(PluginEntry));
                        if (attributes.Length > 0)
                        {
                            SafeInvoke invoke = new SafeInvoke(field);
                            invoke.Invoke(null, null);
                        }
                    }
                }
            }
        }

        public override void Update()
        {
            foreach (Type t in Assambly.GetTypes())
            {
                foreach (MethodInfo field in t.GetMethods())
                {
                    if (field.IsStatic)
                    {
                        PluginTick[] attributes = (PluginTick[])System.Attribute.GetCustomAttributes(field, typeof(PluginTick));
                        if (attributes.Length > 0)
                        {
                            SafeInvoke invoke = new SafeInvoke(field);
                            invoke.Invoke(null, null);
                        }
                    }
                }
            }
        }
    }

    public class SlavePlugin : Plugin
    {
        public SlavePlugin(string _PluginName, string _SourceCode)
            : base(_PluginName, _SourceCode)
        {

        }
    }
}
