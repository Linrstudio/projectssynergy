using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SynergyTemplate;

namespace Framework
{
    public class NetworkClassSlave : NetworkClass
    {
        //static crap
        public static List<Type> AllowedTypes = new List<Type>();

        /// <summary>
        /// Creates a NetworkClassSlave , if the Type is illegal, a NetworkClassSlaveDefault will be created instead
        /// </summary>
        /// <param name="_Name">Name of the class</param>
        /// <param name="_Type">Type of the class</param>
        /// <returns>Shit</returns>
        public static NetworkClassSlave CreateFromType(string _Name, string _Type)
        {
            Type type = typeof(NetworkClassSlaveDefault);
            foreach (Type t in AllowedTypes)
            {
                if (t.FullName == _Type && t.BaseType == typeof(NetworkClassSlave))
                { type = t; break; }
            }
            if (type == typeof(NetworkClassSlaveDefault))
            {
                Log.Write("NetworkClass", Log.Line.Type.Warning, "Type not found or illegal, wil use fallbacktype, OldType : {0} NewType : {1}", _Type, typeof(NetworkClassSlaveDefault).FullName);
            }
            ConstructorInfo ctr = type.GetConstructor(new Type[] { typeof(string), typeof(string) });
            if (ctr != null)
            {
                object instance = ctr.Invoke(new object[] { _Name, _Type });
                return (NetworkClassSlave)instance;
            }
            else Log.Write("NetworkClass", Log.Line.Type.Error, "Failed to find usefull constructor for class {0}", type.FullName);
            return null;
        }

        public class Method
        {
            string methodname;
            public string MethodName { get { return methodname; } }

            NetworkClassSlave networkclass = null;
            string[] parameternames;
            public string[] ParameterNames { get { return parameternames; } }

            public ushort ParameterCount { get { return (ushort)parameternames.Length; } }

            public Method(string _MethodName, NetworkClassSlave _NetworkClass, string[] _ParameterNames)
            {
                methodname = _MethodName;
                networkclass = _NetworkClass;
                parameternames = _ParameterNames;
            }

            public new string ToString()
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
            public object Value
            {
                get { return value; }
                set { this.value = value; networkclass.UpdateRemoteField(fieldname); }
            }

            NetworkClassSlave networkclass = null;

            public Field(string _FieldName, NetworkClassSlave _NetworkClass, object _Value)
            {
                fieldname = _FieldName;
                networkclass = _NetworkClass;
                fieldtype = _Value.GetType();
                value = _Value;
            }

            public void SetValue(object _Value)
            {
                networkclass.SetMasterField(fieldname, _Value);
            }

            public new string ToString()
            {
                string t = fieldtype == null ? "null" : fieldtype.Name;
                string v = Value == null ? "null" : Value.ToString();
                return string.Format("[ Name:{0} Type:{1} Value:{2} ]", fieldname, t, v);
            }
        }

        internal Dictionary<string, Method> Methods = new Dictionary<string, Method>();
        internal Dictionary<string, Field> Fields = new Dictionary<string, Field>();

        public NetworkClassSlave(string _Name, string _TypeName)
            : base(_Name, _TypeName) { }

        public void InvokeMasterMethod(string _FunctionName, params object[] _Parameters)
        {
            Connection.SendToAll("ExecuteMasterCommand", Name, GetInvokeCommand(_FunctionName, _Parameters));
        }

        public void SetMasterField(string _FieldName, object _Value)
        {
            Connection.SendToAll("ExecuteMasterCommand", Name, GetSetFieldCommand(_FieldName, _Value));
        }

        public override void UpdateRemoteField(string _FieldName)
        {
            SetMasterField(_FieldName, GetField(_FieldName));
        }

        public NetworkClassSlave.Method GetMethod(string _MethodName)
        {
            if (Methods.ContainsKey(_MethodName)) return Methods[_MethodName];
            return null;
        }

        public override object GetField(string _FieldName)
        {
            if (Fields.ContainsKey(_FieldName)) return Fields[_FieldName].Value;
            return null;
        }
    }
}
