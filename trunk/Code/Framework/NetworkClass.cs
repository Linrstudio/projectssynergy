using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;


namespace Synergy
{
    public class NetworkClass
    {
        public class Method : Attribute
        {
            public string Name;
            public Method(string _Name) { Name = _Name; }
        }

        public class Field : Attribute
        {
            public string Name;
            public bool ReadOnly;
            public Field(string _Name) { Name = _Name; ReadOnly = false; }
            public Field(string _Name, bool _ReadOnly) { Name = _Name; ReadOnly = _ReadOnly; }
        }

        public class FieldChanged : Attribute
        {
            public string FieldName;
            public FieldChanged(string _FieldName) { FieldName = _FieldName; }
        }

        Queue<ByteStream> CommandQueue = new Queue<ByteStream>();

        string name;
        public string Name { get { return name; } }
        string typename = "";
        public string TypeName { get { return typename; } }

        public NetworkClass(string _Name, string _TypeName)
        {
            name = _Name;
            typename = _TypeName;
        }

        internal void EnqueueCommand(ByteStream _Stream)
        {
            lock (CommandQueue)
            {
                CommandQueue.Enqueue(_Stream);
            }
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

        internal string[] GetNetworkMethods()
        {
            List<string> methods = new List<string>();
            foreach (MethodInfo i in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                Method[] attributes = (Method[])i.GetCustomAttributes(typeof(Method), true);
                if (attributes.Length > 0)
                    methods.Add(attributes[0].Name);
            }
            return methods.ToArray();
        }

        internal string[] GetNetworkFields()
        {
            List<string> fields = new List<string>();
            foreach (FieldInfo i in this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                Field[] attributes = (Field[])i.GetCustomAttributes(typeof(Field), true);
                if (attributes.Length > 0)
                    fields.Add(attributes[0].Name);
            }
            return fields.ToArray();
        }

        internal MethodInfo GetNetworkMethodInfo(string _Name)
        {
            foreach (MethodInfo i in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                Method[] attributes = (Method[])System.Attribute.GetCustomAttributes(i, typeof(Method));

                foreach (Method a in attributes)
                {
                    if (a.Name == _Name)
                    {
                        return i;
                    }
                }
            }
            return null;
        }

        internal FieldInfo GetNetworkFieldInfo(string _Name)
        {
            foreach (FieldInfo i in this.GetType().GetFields())
            {
                Field[] attributes = (Field[])i.GetCustomAttributes(typeof(Field), true);

                foreach (Field a in attributes)
                {
                    if (a.Name == _Name)
                    {
                        return i;
                    }
                }
            }
            return null;
        }

        internal void TriggerFieldChanged(string _FieldName, object _OldVal, object _NewVal)
        {
            foreach (MethodInfo i in this.GetType().GetMethods())
            {
                FieldChanged[] attributes = (FieldChanged[])System.Attribute.GetCustomAttributes(i, typeof(FieldChanged));
                foreach (FieldChanged a in attributes)
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
        #region FieldStuff
        public void SetField(string _Name, object _Value)
        {
            FieldInfo info = GetNetworkFieldInfo(_Name);
            if (info != null)
            {
                if (_Value.GetType() == info.FieldType)
                {
                    Field[] attributes = (Field[])info.GetCustomAttributes(typeof(Field), true);
                    if (!attributes[0].ReadOnly)
                    {
                        object oldvalue = info.GetValue(this);
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
        public virtual object GetField(string _FieldName)
        {
            return null;
        }
        /// <summary>
        /// Updates the remote instance of this field
        /// </summary>
        /// <param name="_FieldName">Name of the field</param>
        public virtual void UpdateRemoteField(string _FieldName)
        {

        }
        #endregion
        internal object InvokeMethod(string _Name, params object[] _Parameters)
        {
            MethodInfo method = GetNetworkMethodInfo(_Name);
            object result = null;
            if (method != null)
            {
                try
                {
                    SafeInvoke invoke = new SafeInvoke(method);
                    result = invoke.Invoke(this, _Parameters);
                }
                catch (TargetInvocationException e)
                {
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e.InnerException, true);

                    Log.Write("NetworkClass", Log.Line.Type.Error, @"BAD error! File:{0} Line:{1} Message:{2}",
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

        internal virtual void ExecuteCommand(ByteStream _Stream)
        {
            byte command = _Stream.Read();
            switch (command)
            {
                case 1://invoke method
                    {
                        string methodname = (string)Converter.Read(_Stream);
                        MethodInfo info = GetNetworkMethodInfo(methodname);
                        if (info != null)
                        {
                            int paramcount = (ushort)Converter.Read(_Stream);
                            object[] parameters = new object[paramcount];
                            for (int i = 0; i < paramcount; i++) 
                                parameters[i] = Converter.Read(_Stream);
                            InvokeMethod(methodname, parameters);
                        }
                        else Log.Write("NetworkClass", Log.Line.Type.Error, "Cant find method with name {0}", methodname);
                    }
                    break;
                case 2://set field
                    {
                        string fieldname = (string)Converter.Read(_Stream);
                        FieldInfo info = GetNetworkFieldInfo(fieldname);
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
        #region Commands
        public static ByteStream GetInvokeCommand(string _Name, params object[] _Parameters)
        {
            ByteStream stream = new ByteStream();
            stream.Write(1);
            Converter.Write(_Name, stream);
            Converter.Write((ushort)_Parameters.Length, stream);
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
        #endregion
    }
}
