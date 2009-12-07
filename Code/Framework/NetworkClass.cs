using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Framework
{
    public class NetworkMethod : Attribute
    {
        public string Name;
        public NetworkMethod(string _Name) { Name = _Name; }
    }
    public class NetworkField : Attribute
    {
        public string Name;
        public bool ReadOnly;
        public NetworkField(string _Name) { Name = _Name; ReadOnly = false; }
        public NetworkField(string _Name, bool _ReadOnly) { Name = _Name; ReadOnly = _ReadOnly; }
    }

    public class NetworkFieldChanged : Attribute
    {
        public string FieldName;
        public NetworkFieldChanged(string _FieldName) { FieldName = _FieldName; }
    }

    public class NetworkClassRemote
    {
        string name;
        public string Name { get { return name; } }
        public NetworkClassRemote(string _Name)
        {
            name = _Name;
        }

    }

    public class NetworkClassLocal
    {
        string name;
        public string Name { get { return name; } }
        public NetworkClassLocal(string _Name)
        {
            name = _Name;
        }

        public virtual void Update()
        {
        }

        public object InvokeMethod(string _Name, params object[] _Parameters)
        {
            MethodInfo method = GetMethodInfo(_Name);
            object result = null;
            if (method != null)
            {
                try
                {
                    result = method.Invoke(this, _Parameters);
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException;
                }
                catch (TargetParameterCountException)
                {
                    Log.Write("NetworkClass", "Method {0} does not have {1} parameters", _Name, _Parameters.Length);
                }
            }
            else { Log.Write("NetworkClass", "Method {0} does not exits", _Name); }
            return result;
        }

        public void SetField(string _Name, object _Value)
        {
            FieldInfo info = GetFieldInfo(_Name);
            if (info != null)
            {
                if (_Value.GetType() == info.FieldType)
                {
                    NetworkField[] attributes = (NetworkField[])info.GetCustomAttributes(typeof(NetworkField), true);
                    if (!attributes[0].ReadOnly)
                    {
                        object oldvalue = info.GetValue((object)this);
                        info.SetValue((object)this, _Value);
                        //trigger any OnChanged 'events'
                        TriggerFieldChanged(_Name, oldvalue, _Value);
                    }
                    else Log.Write("NetworkClass", "this field is readonly!");
                }
                else Log.Write("NetworkClass", "Value is of wrong type");
            }
            else Log.Write("NetworkClass", "Cant find member with name {0}", _Name);
        }

        public void TriggerFieldChanged(string _FieldName, object _OldVal, object _NewVal)
        {
            foreach (MethodInfo i in this.GetType().GetMethods())
            {
                NetworkFieldChanged[] attributes = (NetworkFieldChanged[])System.Attribute.GetCustomAttributes(i, typeof(NetworkFieldChanged));

                foreach (NetworkFieldChanged a in attributes)
                {
                    if (a.FieldName == _FieldName)
                    {
                        i.Invoke(this, new object[] { _OldVal, _NewVal });
                    }
                }
            }
        }

        public static ByteStream GetInvokeCommand(string _Name, params object[] _Parameters)
        {
            ByteStream stream = new ByteStream();
            stream.Write(1);
            Converter.Write(_Name, stream);
            foreach (object o in _Parameters)
            {
                Converter.Write(o, stream);
            }
            return stream;
        }

        public static ByteStream GetSetFieldCommand(string _FieldName, object _Value)
        {
            ByteStream stream = new ByteStream();
            stream.Write(2);
            Converter.Write(_FieldName, stream);
            Converter.Write(_Value, stream);
            return stream;
        }

        public void ExecuteCommand(ByteStream _Stream)
        {
            byte command = _Stream.Read();
            switch (command)
            {
                case 1://invoke method
                    {
                        string methodname = (string)Converter.Read(typeof(string), _Stream);
                        MethodInfo info = GetMethodInfo(methodname);
                        if (info != null)
                        {
                            ParameterInfo[] parametersinfo = info.GetParameters();
                            List<object> parameters = new List<object>();
                            foreach (ParameterInfo i in parametersinfo)
                            {
                                parameters.Add(Converter.Read(i.ParameterType, _Stream));
                            }
                            InvokeMethod(methodname, parameters.ToArray());
                        }
                        else Log.Write("NetworkClass", "Cant find method with name {0}", methodname);
                    }
                    break;
                case 2://set field
                    {
                        string fieldname = (string)Converter.Read(typeof(string), _Stream);
                        FieldInfo info = GetFieldInfo(fieldname);
                        if (info != null)
                        {
                            object value = Converter.Read(info.FieldType, _Stream);
                            SetField(fieldname, value);
                        }
                        else Log.Write("NetworkClass", "Cant find field with name {0}", fieldname);
                    }
                    break;
            }
        }

        MethodInfo GetMethodInfo(string _Name)
        {
            foreach (MethodInfo i in this.GetType().GetMethods())
            {
                NetworkMethod[] attributes = (NetworkMethod[])System.Attribute.GetCustomAttributes(i, typeof(NetworkMethod));

                foreach (NetworkMethod a in attributes)
                {
                    if (a.Name == _Name)
                    {
                        return i;
                    }
                }
            }
            return null;
        }

        FieldInfo GetFieldInfo(string _Name)
        {
            foreach (FieldInfo i in this.GetType().GetFields())
            {
                NetworkField[] attributes = (NetworkField[])i.GetCustomAttributes(typeof(NetworkField), true);

                foreach (NetworkField a in attributes)
                {
                    if (a.Name == _Name)
                    {
                        return i;
                    }
                }
            }
            return null;
        }
    }
}
