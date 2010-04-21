using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

using SynergyTemplate;

namespace Framework
{
    public class ExecFunctions
    {
        #region ParserCrap
        public static class Tokenizer
        {
            /// <summary>
            /// cuts away the matching regex from the _Source string
            /// </summary>
            /// <param name="_Source"></param>
            /// <param name="_Match"></param>
            public static void Cut(ref string _Source, Match _Match)
            {
                _Source = _Source.Remove(0, _Match.Index + _Match.Length);
            }

            public static string WhiteSpace(ref string _Source, bool _Cut)
            {
                Match m = Regex.Match(_Source, @"^[\s]*");
                if (_Cut) Cut(ref _Source, m);
                return m.Value;
            }

            public static string MethodName(ref string _Source, bool _Cut)
            {
                Match m = Regex.Match(_Source, @"^[-_a-zA-Z0-9]+");
                if (_Cut) Cut(ref _Source, m);
                return m.Value;
            }

            public static string String(ref string _Source, bool _Cut)
            {
                Match m = Regex.Match(_Source, "^\"(?<1>[-\\._a-zA-Z0-9]*)\"");
                if (_Cut) Cut(ref _Source, m);
                return m.Groups[1].Value;
            }

            public static string Number(ref string _Source, bool _Cut)
            {
                Match m = Regex.Match(_Source, @"^[0-9,.]+");
                if (_Cut) Cut(ref _Source, m);
                return m.Value;
            }

            public static string Comma(ref string _Source, bool _Cut)
            {
                Match m = Regex.Match(_Source, @"^[,]");
                if (_Cut) Cut(ref _Source, m);
                return m.Value;
            }

            public static string BracketOpen(ref string _Source, bool _Cut)
            {
                Match m = Regex.Match(_Source, @"^[(]");
                if (_Cut) Cut(ref _Source, m);
                return m.Value;
            }

            public static string BracketClose(ref string _Source, bool _Cut)
            {
                Match m = Regex.Match(_Source, @"^[)]");
                if (_Cut) Cut(ref _Source, m);
                return m.Value;
            }

            public static string SemiColumn(ref string _Source, bool _Cut)
            {
                Match m = Regex.Match(_Source, @"^[;]");
                if (_Cut) Cut(ref _Source, m);
                return m.Value;
            }

            public static string Bool(ref string _Source, bool _Cut)
            {
                Match m = Regex.Match(_Source, @"^(true|false)");
                if (_Cut) Cut(ref _Source, m);
                return m.Value;
            }
        }


        public static void Execute(string _Command)
        {
            while (true)
            {
                int length = _Command.Length;
                FunctionCall(DefaultExecFunctions, ref _Command);
                if (length == _Command.Length) break;
            }
        }

        public static object FunctionCall(object _Owner, ref string _Source)
        {
            Tokenizer.WhiteSpace(ref _Source, true);
            string methodname = Tokenizer.MethodName(ref _Source, true);
            Tokenizer.WhiteSpace(ref _Source, true);

            if (methodname.Length == 0) return null;

            MethodInfo info = null;
            try
            {
                info = _Owner.GetType().GetMethod(methodname);
                if (info.GetCustomAttributes(typeof(Exec), false).Length == 0) throw new Exception();
            }
            catch { Log.Write("Exec", "Failed to find method {0}", methodname); return null; }

            info.GetParameters();
            ParameterInfo[] parameters = info.GetParameters();
            object[] values = new object[parameters.Length];

            Tokenizer.BracketOpen(ref _Source, true); Tokenizer.WhiteSpace(ref _Source, true);
            for (int i = 0; i < values.Length; i++)
            {
                Tokenizer.WhiteSpace(ref _Source, true);
                values[i] = Value(_Owner, ref _Source, parameters[i].ParameterType);
                Tokenizer.WhiteSpace(ref _Source, true);
                Tokenizer.Comma(ref _Source, true);
            }
            Tokenizer.BracketClose(ref _Source, true); Tokenizer.WhiteSpace(ref _Source, true);
            Tokenizer.SemiColumn(ref _Source, true);
            object result = info.Invoke(_Owner, values);
            return result;
        }

        static object Value(object _Owner, ref string _Source, Type _Type)
        {
            MethodInfo[] methods = GetExecMethods(_Owner);
            Tokenizer.WhiteSpace(ref _Source, true);
            string readmethodname = Tokenizer.MethodName(ref _Source, false);
            MethodInfo foundmethod = null;
            foreach (MethodInfo i in methods)
            {
                if (i.Name == readmethodname) { foundmethod = i; break; }
            }

            if (foundmethod != null)//we found a method, so lets not parse this as a object
            {
                return FunctionCall(_Owner, ref _Source);
            }

            if (_Type == typeof(int) || _Type == typeof(ushort))// is any number
            {
                object number = _Type.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { Tokenizer.Number(ref _Source, false) });
                Tokenizer.Number(ref _Source, true);
                return number;
            }

            if (_Type == typeof(string))// string
            {
                object str = Tokenizer.String(ref _Source, true);
                return str;
            }

            if (_Type == typeof(bool))//not tested
            {
                object bl = Tokenizer.Bool(ref _Source, true);
            }
            return null;//cant make anything out of it
        }

        public static MethodInfo[] GetExecMethods(object _Owner)
        {
            List<MethodInfo> methods = new List<MethodInfo>();
            foreach (MethodInfo m in _Owner.GetType().GetMethods())
                if (m.GetCustomAttributes(typeof(Exec), false).Length > 0) methods.Add(m);
            return methods.ToArray();
        }

        static object DefaultExecFunctions = new ExecFunctions();

        public class Exec : Attribute { }
        #endregion

        [Exec]
        public static void run(string _Filename)
        {
            if (System.IO.File.Exists(_Filename)) Execute(System.IO.File.ReadAllText(_Filename));
        }

        [Exec]
        public NetworkClassMaster NetworkClass(string _ClassName)
        {
            if (NetworkManager.LocalNode.NetworkClasses.ContainsKey(_ClassName))
                return NetworkManager.LocalNode.NetworkClasses[_ClassName];
            return null;
        }

        [Exec]
        public void clear()
        {
            Log.Clear();
        }

        [Exec]
        public void initializeconverter()
        {
            Converter.Initialize();
        }

        [Exec]
        public void connect(string _IP, ushort _Port)
        {
            new TCPConnection(_IP, _Port, true);
        }

        [Exec]
        public void help(string _MethodName)
        {
            if (_MethodName == null || _MethodName == "")
            {
                foreach (MethodInfo m in GetExecMethods(this))
                    help(m.Name);
            }
            else
            {
                MethodInfo info = null;
                try
                {
                    info = GetType().GetMethod(_MethodName);
                }
                catch { Log.Write("Exec", "Failed to find {0}", _MethodName); }
                finally
                {
                    string text = info.Name + "(";
                    ParameterInfo[] parameters = info.GetParameters();
                    foreach (ParameterInfo i in parameters)
                    {
                        text += i.ParameterType.Name + " " + Regex.Match(i.Name, "_*(?<1>.*)").Groups[1].Value;
                        if (i != parameters[parameters.Length - 1]) text += ", ";
                    }
                    text += ")";
                    Log.Write("Exec", text);
                }
            }
        }

        [Exec]
        public void listen(int _Port)
        {
            new TCPListener(_Port);
        }
    }
}