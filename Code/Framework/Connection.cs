using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SynergyTemplate;

namespace Framework
{
    public class Connection
    {
        internal string remotenodeid = "";
        public string RemoteNodeID { get { return remotenodeid; } }
        public ConnectionMethods methods = null;

        public virtual void Update() { }

        public static void Send(NetworkNodeRemote _Node, string _FunctioName, params object[] _Parameters)
        {

        }

        public static void SendToAll(string _FunctionName, params object[] _Parameters)
        {
            uint ID = NetworkManager.ActionBlackList.GetRandomID();
            foreach (Connection c in NetworkManager.LocalNode.Connections)
            {
                c.Send(true, ID, _FunctionName, _Parameters);
            }
        }

        public void Send(string _FunctionName, params object[] _Parameters) { Send("", _FunctionName, _Parameters); }
        public virtual void Send(string _TargetNode, string _FunctionName, params object[] _Parameters) { }
        public virtual void Send(bool _BroadCast, uint _ActionID, string _FunctionName, params object[] _Parameters) { }
        public virtual void Send(bool _BroadCast, string _FunctionName, params object[] _Parameters) { }
        public virtual void Send(ByteStream _RawData) { }

        public Connection()
        {
            methods = new ConnectionMethods(this);
            NetworkManager.LocalNode.Connections.Add(this);
        }

        public class ConnectionMethods : NetworkClassMaster
        {
            Connection connection = null;

            public ConnectionMethods(Connection _Connection)
                : base("ConnectionMethods", "")
            {
                connection = _Connection;
            }

            [Method("Sleep")]
            public void Sleep()
            {
                //Console.WriteLine("Sleep!");
            }

            [Method("Hello")]
            public void Hello(string _NodeID)
            {
                Log.Write("Networking", "Hello Node {0}", _NodeID);
                connection.remotenodeid = _NodeID;
                connection.Send(true, NetworkManager.ActionBlackList.GetRandomID(), "RequestNetworkMap");
            }

            [Method("MapConnection")]
            public void AddConnection(string _NodeID1, string _NodeID2)
            {
                NetworkManager.AddConnection(_NodeID1, _NodeID2);
            }

            [Method("MapClass")]
            public void AddClass(string _NodeID, string _ClassName, string _Type)
            {
                NetworkManager.AddRemoteNode(_NodeID);
                NetworkNodeRemote node = NetworkManager.RemoteNodes[_NodeID];
                node.AddNetworkClass(_ClassName, _Type);
                Log.Write("Networking", "NetworkClass added [{0}]", _ClassName, _Type);
            }

            [Method("MapClassMethod")]
            public void AddClassMethod(string _NodeID, string _ClassName, string _MethodName, string _ParameterNames)
            {
                NetworkManager.AddRemoteNode(_NodeID);
                NetworkNodeRemote node = NetworkManager.RemoteNodes[_NodeID];

                if (node.LocalDevices.ContainsKey(_ClassName))
                {
                    NetworkClassSlave parent = node.LocalDevices[_ClassName];
                    if (!parent.Methods.ContainsKey(_ClassName))
                    {
                        NetworkClassSlave.Method method = new NetworkClassSlave.Method(_MethodName, parent, _ParameterNames.Split(' '));
                        parent.Methods.Add(_MethodName, method);
                        Log.Write("Networking", "Network method added {0}", method.ToString());
                    }
                    else Log.Write("Networking", "{0} already has a method : {1}", _ClassName, _MethodName);
                }
                else Log.Write("Networking", "Cant find NetworkClass {0}", _ClassName);
            }

            [Method("Subscribe")]
            public void Subscribe(string _ClassName, string _NodeID)
            {
                //find local object with name classname
                if (!NetworkManager.RemoteNodes.ContainsKey(_NodeID)) Log.Write("Networking", "Cant add registration to class {0}, class doesnt exists", _ClassName);
                if (!NetworkManager.LocalNode.NetworkClasses.ContainsKey(_ClassName)) Log.Write("Networking", "Cant add registration to class {0}, class doesnt exists", _ClassName);
                NetworkClassMaster obj = NetworkManager.LocalNode.NetworkClasses[_ClassName];
                obj.AddSubscription(NetworkManager.RemoteNodes[_NodeID]);
            }

            [Method("MapClassField")]
            public void AddClassField(string _NodeID, string _ClassName, string _FieldName, string _FieldType, ByteStream _Value)
            {
                NetworkManager.AddRemoteNode(_NodeID);
                NetworkNodeRemote node = NetworkManager.RemoteNodes[_NodeID];

                if (node.LocalDevices.ContainsKey(_ClassName))
                {
                    NetworkClassSlave parent = node.LocalDevices[_ClassName];
                    if (!parent.Fields.ContainsKey(_FieldName))
                    {
                        object value = Converter.Read(_Value);

                        NetworkClassSlave.Field field = new NetworkClassSlave.Field(_FieldName, parent, value);
                        parent.Fields.Add(_FieldName, field);
                        Log.Write("Networking", "Network field added {0}", field.ToString());
                    }
                    else Log.Write("Networking", Log.Line.Type.Warning, "{0} already has a field : {1}", _ClassName, _FieldName);
                }
                else Log.Write("Networking", Log.Line.Type.Warning, "Cant find NetworkClass {0}", _ClassName);
            }

            [Method("ExecuteMasterCommand")]
            public void ExecuteLocalCommand(string _TargetClass, ByteStream _Command)
            {
                if (NetworkManager.LocalNode.NetworkClasses.ContainsKey(_TargetClass))
                    NetworkManager.LocalNode.NetworkClasses[_TargetClass].EnqueueCommand(_Command);
                else
                    Log.Write("Networking", Log.Line.Type.Warning, "Network class {0} not found", _TargetClass);
            }

            [Method("ExecuteSlaveCommand")]
            public void ExecuteRemoteCommand(string _TargetClass, ByteStream _Command)
            {
                bool found = false;
                foreach (NetworkNodeRemote node in NetworkManager.RemoteNodes.Values)
                {
                    foreach (NetworkClassSlave clas in node.LocalDevices.Values)
                    {
                        if (clas.Name == _TargetClass)
                        {
                            clas.EnqueueCommand(_Command);
                            found = true;
                        }
                    }
                }
                if (!found) Log.Write("Networking", Log.Line.Type.Warning, "not NetworkClassSlave found with name {0}, try to refresh network map", _TargetClass);
            }

            [Method("RequestNetworkMap")]
            public void RequestNetworkMap()
            {
                Log.Write("Networking", "NetworkMap request received");
                //send connections
                foreach (Connection connection in NetworkManager.LocalNode.Connections)
                {
                    SendToAll("MapConnection", NetworkManager.LocalNode.NodeID, connection.RemoteNodeID);
                }
                //send classes
                foreach (NetworkClassMaster networkclass in NetworkManager.LocalNode.NetworkClasses.Values)
                {
                    {
                        SendToAll("MapClass", NetworkManager.LocalNode.NodeID, networkclass.Name, networkclass.TypeName);
                    }

                    //send methods
                    foreach (string methodname in networkclass.GetNetworkMethods())
                    {
                        string parameters = "";
                        foreach (ParameterInfo i in networkclass.GetNetworkMethodInfo(methodname).GetParameters())
                            parameters += i.Name + " ";
                        SendToAll("MapClassMethod", NetworkManager.LocalNode.NodeID, networkclass.Name, methodname, parameters);
                    }

                    //send fields
                    foreach (string fieldname in networkclass.GetNetworkFields())
                    {
                        FieldInfo info = networkclass.GetNetworkFieldInfo(fieldname);
                        object value = networkclass.GetField(fieldname);
                        if (value != null)
                        {
                            ByteStream valuestream = new ByteStream();
                            Converter.Write(value, valuestream);
                            SendToAll("MapClassField", NetworkManager.LocalNode.NodeID, networkclass.Name, fieldname, info.FieldType.FullName, valuestream);
                        }
                        else { Log.Write("Networking", Log.Line.Type.Error, "cant send field : {0}", fieldname); }
                    }
                }
            }
        }
    }
}
