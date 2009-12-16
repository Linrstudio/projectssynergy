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

    public class NetworkClassLocal
    {
        Queue<ByteStream> CommandQueue = new Queue<ByteStream>();

        string name;
        public string Name { get { return name; } }
        public NetworkClassLocal(string _Name)
        {
            name = _Name;
        }

        public void UpdateRemote()
        {

        }

        public virtual void Update()
        {
            lock (CommandQueue)
            {
                while (CommandQueue.Count > 0)
                {
                    ByteStream c = CommandQueue.Dequeue();
                    ExecuteCommand(c);
                }
            }
        }

        public string[] GetMethods()
        {
            List<string> methods = new List<string>();
            foreach (MethodInfo i in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                NetworkMethod[] attributes = (NetworkMethod[])i.GetCustomAttributes(typeof(NetworkMethod), true);
                if (attributes.Length > 0)
                    methods.Add(attributes[0].Name);
            }
            return methods.ToArray();
        }

        public string[] GetFields()
        {
            List<string> fields = new List<string>();
            foreach (FieldInfo i in this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                NetworkField[] attributes = (NetworkField[])i.GetCustomAttributes(typeof(NetworkField), true);
                if (attributes.Length > 0)
                    fields.Add(attributes[0].Name);
            }
            return fields.ToArray();
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
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e.InnerException, true);

                    
                    Log.Write("NetworkClass", Log.Line.Type.Error, "BAD error! File:{0} Line:{1} Message:{2}",
                        System.IO.Path.GetFileName(trace.GetFrame(0).GetFileName()), 
                        trace.GetFrame(0).GetFileLineNumber(),
                        e.InnerException.Message);

                    //throw e.InnerException;
                }
                catch (TargetParameterCountException)
                {
                    Log.Write("NetworkClass", Log.Line.Type.Error, "Method {0} does not have {1} parameters", _Name, _Parameters.Length);
                }
            }
            else { Log.Write("NetworkClass", Log.Line.Type.Error, "Method {0} does not exits", _Name); }
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
                    else Log.Write("NetworkClass", Log.Line.Type.Error, "this field is readonly!");
                }
                else Log.Write("NetworkClass", Log.Line.Type.Error, "Value is of wrong type");
            }
            else Log.Write("NetworkClass", Log.Line.Type.Error, "Cant find member with name {0}", _Name);
        }

        public object GetField(string _Name)
        {
            FieldInfo info = GetFieldInfo(_Name);
            if (info == null) return null;
            return GetFieldInfo(_Name).GetValue(this);
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
                        try
                        {
                            i.Invoke(this, new object[] { _OldVal, _NewVal });
                        }
                        catch (ArgumentException) { Log.Write("NetworkClass", "Cant invoke field change event, arguments are of wrong type "); }
                        catch (Exception e) { throw e.InnerException; }
                    }
                }
            }
        }

        public void UpdateRemoteField(string _FieldName)
        {
            FieldInfo info = GetFieldInfo(_FieldName);
            if (info == null) Log.Write("NetworkClass", Log.Line.Type.Error, "Cant find field with name : {0}", _FieldName);
            
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

        public void EnqueueCommand(ByteStream _Stream)
        {
            lock(CommandQueue)
            {
                CommandQueue.Enqueue(_Stream);
            }
        }

        public void ExecuteCommand(ByteStream _Stream)
        {
            byte command = _Stream.Read();
            switch (command)
            {
                case 1://invoke method
                    {
                        string methodname = (string)Converter.Read( _Stream);
                        MethodInfo info = GetMethodInfo(methodname);
                        if (info != null)
                        {
                            ParameterInfo[] parametersinfo = info.GetParameters();
                            List<object> parameters = new List<object>();
                            foreach (ParameterInfo i in parametersinfo)
                            {
                                parameters.Add(Converter.Read( _Stream));
                            }
                            InvokeMethod(methodname, parameters.ToArray());
                        }
                        else Log.Write("NetworkClass", Log.Line.Type.Error, "Cant find method with name {0}", methodname);
                    }
                    break;
                case 2://set field
                    {
                        string fieldname = (string)Converter.Read( _Stream);
                        FieldInfo info = GetFieldInfo(fieldname);
                        if (info != null)
                        {
                            object value = Converter.Read(_Stream);
                            SetField(fieldname, value);
                        }
                        else Log.Write("NetworkClass", Log.Line.Type.Error, "Cant find field with name {0}", fieldname);
                    }
                    break;
            }
        }

        public MethodInfo GetMethodInfo(string _Name)
        {
            foreach (MethodInfo i in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
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

        public FieldInfo GetFieldInfo(string _Name)
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
