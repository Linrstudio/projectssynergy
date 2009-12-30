using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Framework
{
    public class NetworkClassRemote
    {
        public class Method
        {
            string methodname;
            public string MethodName { get { return methodname; } }

            NetworkClassRemote networkclass = null;
            string[] parameternames;

            public ushort ParameterCount { get { return (ushort)parameternames.Length; } }

            public Method(string _MethodName, NetworkClassRemote _NetworkClass, string[] _ParameterNames)
            {
                methodname = _MethodName;
                networkclass = _NetworkClass;
                parameternames = _ParameterNames;
            }

            public string ToString()
            {
                string param = "";
                foreach (string s in parameternames)
                {
                    param += s + " ";
                }
                return string.Format("[ MethodName:{0} Parameters: {1} ]", methodname, param);
            }
        }

        public class Field
        {
            string fieldname;
            Type fieldtype;
            object value;
            public string FieldName { get { return fieldname; } }
            public Type FieldType { get { return fieldtype; } }
            public object Value { get { return value; } }

            NetworkClassRemote networkclass = null;

            public Field(string _FieldName, NetworkClassRemote _NetworkClass, object _Value)
            {
                fieldname = _FieldName;
                networkclass = _NetworkClass;
                fieldtype = _Value.GetType();
                value = _Value;
            }

            public new string ToString()
            {
                string t = fieldtype == null ? "null" : fieldtype.Name;
                string v = Value == null ? "null" : Value.ToString();
                return string.Format("[ Name:{0} Type:{1} Value:{2} ]", fieldname, t, v);
            }
        }

 
        string name;

        internal Dictionary<string, Method> Methods = new Dictionary<string, Method>();
        internal Dictionary<string, Field> Fields = new Dictionary<string, Field>();

        public NetworkClassRemote.Method GetMethod(string _MethodName)
        {
            if (Methods.ContainsKey(_MethodName)) return Methods[_MethodName];
            return null;
        }

        public string Name { get { return name; } }
        public NetworkClassRemote(string _Name)
        {
            name = _Name;
        }
    }
}
