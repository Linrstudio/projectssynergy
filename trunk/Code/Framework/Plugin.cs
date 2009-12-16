using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.CodeDom.Compiler;
using System.Reflection;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace Framework
{
    public class PluginEntry : Attribute { }
    public class PluginTick : Attribute { }

    public static class PluginManager
    {
        public enum CompilerType { CSharp=1, VisualBasic=2 };

        public static List<Plugin> plugins = new List<Plugin>();

        public static Plugin LoadPlugin(string _Filename)
        {
            if (Path.GetExtension(_Filename) == ".vb")
                return LoadPlugin(_Filename, CompilerType.VisualBasic);
            else
                return LoadPlugin(_Filename, CompilerType.CSharp); 
        }

        public static Plugin LoadPlugin(string _Filename,CompilerType _Compiler)
        {
            CodeDomProvider compiler=null;
            switch (_Compiler)
            {
                case CompilerType.VisualBasic: compiler = new VBCodeProvider(); break;
                case CompilerType.CSharp: compiler = new CSharpCodeProvider(); break;
            }
            
            CompilerParameters compilerParams = new CompilerParameters();
            compilerParams.GenerateInMemory = true;
            compilerParams.ReferencedAssemblies.Add("system.dll");
            compilerParams.ReferencedAssemblies.Add("framework.dll");
            compilerParams.GenerateExecutable = false;

            CompilerResults results = compiler.CompileAssemblyFromFile(compilerParams, _Filename);
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError e in results.Errors)
                {
                    Log.Write("Plugin Compiler", Log.Line.Type.Error, e.ToString());
                }
                return null;
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
}
