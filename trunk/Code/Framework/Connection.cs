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
        public ushort RemoteNodeID = 0;
        public ConnectionMethods methods = null;

        public virtual void Update() { }

        public void SendCommand(string _ClassName, ByteStream _Command)
        {
            Send(true, "ExecuteCommand", _ClassName, _Command);
        }

        public virtual void Send(bool _BroadCast, uint _ActionID, string _FunctionName, params object[] _Parameters) { }
        public virtual void Send(bool _BroadCast, string _FunctionName, params object[] _Parameters) { }
        public virtual void Send(ByteStream _RawData) { }
        public virtual ushort GetRemoteNetworkNodeID() { return RemoteNodeID; }

        public Connection()
        {
            methods = new ConnectionMethods(this);
            NetworkManager.LocalNode.Connections.Add(this);
        }

        public class ConnectionMethods : NetworkClassLocal
        {
            Connection connection = null;

            public ConnectionMethods(Connection _Connection)
                : base("ConnectionMethods")
            {
                connection = _Connection;
            }

            [NetworkMethod("Sleep")]
            public void Sleep()
            {
                //Console.WriteLine("Sleep!");
            }

            [NetworkMethod("Hello")]
            public void Hello(ushort _NodeID)
            {
                Log.Write("Networking", "Hello Node {0}", _NodeID);
                connection.RemoteNodeID = _NodeID;
                connection.Send(true, NetworkManager.ActionBlackList.GetRandomID(), "RequestNetworkMap");
            }

            [NetworkMethod("MapConnection")]
            public void AddConnection(ushort _NodeID1, ushort _NodeID2)
            {
                ushort node1 = _NodeID1;
                ushort node2 = _NodeID2;

                if (node1 == 0 || node2 == 0 || node1 == node2) return;
                if (!NetworkManager.RemoteNodes.ContainsKey(node1)) NetworkManager.RemoteNodes.Add(node1, new NetworkNodeRemote(node1));
                if (!NetworkManager.RemoteNodes.ContainsKey(node2)) NetworkManager.RemoteNodes.Add(node2, new NetworkNodeRemote(node2));
                if (!NetworkManager.RemoteNodes[node1].RemoteNodes.ContainsKey(node2)) NetworkManager.RemoteNodes[node1].RemoteNodes.Add(node2, NetworkManager.RemoteNodes[node2]);
                if (!NetworkManager.RemoteNodes[node2].RemoteNodes.ContainsKey(node1)) NetworkManager.RemoteNodes[node2].RemoteNodes.Add(node1, NetworkManager.RemoteNodes[node1]);
                Log.Write("Networking", "Connection added {0} <-> {1}", node1, node2);
            }

            [NetworkMethod("MapClass")]
            public void AddClass(ushort _NodeID, string _ClassName)
            {
                if (!NetworkManager.RemoteNodes.ContainsKey(_NodeID)) NetworkManager.AddRemoteNode(_NodeID);
                NetworkNodeRemote node = NetworkManager.RemoteNodes[_NodeID];
                if (!node.LocalDevices.ContainsKey(_ClassName)) node.LocalDevices.Add(_ClassName, new NetworkClassRemote(_ClassName));
                Log.Write("Networking", "Network class {0} added", _ClassName);
            }

            [NetworkMethod("MapClassMethod")]
            public void AddClassMethod(ushort _NodeID, string _ClassName, string _MethodName, string _ParameterNames)
            {
                if (!NetworkManager.RemoteNodes.ContainsKey(_NodeID)) NetworkManager.AddRemoteNode(_NodeID);
                NetworkNodeRemote node = NetworkManager.RemoteNodes[_NodeID];

                if (node.LocalDevices.ContainsKey(_ClassName))
                {
                    NetworkClassRemote parent = node.LocalDevices[_ClassName];
                    if (!parent.Methods.ContainsKey(_ClassName))
                    {
                        NetworkClassRemote.Method method = new NetworkClassRemote.Method(_MethodName, parent, _ParameterNames.Split(' '));
                        parent.Methods.Add(_MethodName, method);
                        Log.Write("Networking", "Network method added {0}", method.ToString());
                    }
                    else Log.Write("Networking", "{0} already has a method : {1}", _ClassName, _MethodName); 
                }
                else Log.Write("Networking", "Cant find NetworkClass {0}", _ClassName);
            }

            [NetworkMethod("MapClassField")]
            public void AddClassField(ushort _NodeID, string _ClassName, string _FieldName, string _FieldType, ByteStream _Value)
            {
                if (!NetworkManager.RemoteNodes.ContainsKey(_NodeID)) NetworkManager.AddRemoteNode(_NodeID);
                NetworkNodeRemote node = NetworkManager.RemoteNodes[_NodeID];

                if (node.LocalDevices.ContainsKey(_ClassName))
                {
                    NetworkClassRemote parent = node.LocalDevices[_ClassName];
                    if (!parent.Fields.ContainsKey(_FieldName))
                    {
                        object value = Converter.Read(_Value);

                        NetworkClassRemote.Field field = new NetworkClassRemote.Field(_FieldName, parent, value);
                        parent.Fields.Add(_FieldName, field);
                        Log.Write("Networking", "Network field added {0}", field.ToString());
                    }
                    else Log.Write("Networking", "{0} already has a field : {1}", _ClassName, _FieldName); 
                }
                else Log.Write("Networking", "Cant find NetworkClass {0}", _ClassName);
            }

            [NetworkMethod("ExecuteCommand")]
            public void ihateyou(string _TargetClass, ByteStream _Command)
            {
                if (NetworkManager.LocalNode.NetworkClasses.ContainsKey(_TargetClass))
                    NetworkManager.LocalNode.NetworkClasses[_TargetClass].EnqueueCommand(_Command);
                else
                {
                    Log.Write("Networking", "Network class {0} not found", _TargetClass);
                }
            }

            [NetworkMethod("RequestNetworkMap")]
            public void RequestNetworkMap()
            {
                Log.Write("Networking", "NetworkMap request received");
                //send connections
                foreach (Connection connection in NetworkManager.LocalNode.Connections)
                {
                    uint ID = NetworkManager.ActionBlackList.GetRandomID();
                    foreach (Connection c in NetworkManager.LocalNode.Connections)
                    {
                        c.Send(true, ID, "MapConnection", NetworkManager.LocalNode.NodeID, connection.GetRemoteNetworkNodeID());
                    }
                }
                //send classes
                foreach (NetworkClassLocal networkclass in NetworkManager.LocalNode.NetworkClasses.Values)
                {
                    {
                        uint ID = NetworkManager.ActionBlackList.GetRandomID();
                        foreach (Connection c in NetworkManager.LocalNode.Connections)
                        {
                            c.Send(true, ID, "MapClass", NetworkManager.LocalNode.NodeID, networkclass.Name);
                        }
                    }

                    //send methods
                    foreach (string methodname in networkclass.GetMethods())
                    {
                        string parameters = "";
                        foreach (ParameterInfo i in networkclass.GetMethodInfo(methodname).GetParameters())
                            parameters += i.Name + " ";
                        uint ID = NetworkManager.ActionBlackList.GetRandomID();
                        foreach (Connection c in NetworkManager.LocalNode.Connections)
                        {
                            c.Send(true, ID, "MapClassMethod", NetworkManager.LocalNode.NodeID, networkclass.Name, methodname, parameters);
                        }
                    }

                    //send fields
                    foreach (string fieldname in networkclass.GetFields())
                    {
                        FieldInfo info = networkclass.GetFieldInfo(fieldname);
                        object value=networkclass.GetField(fieldname);
                        if (value != null)
                        {
                            ByteStream valuestream = new ByteStream();
                            Converter.Write(value, valuestream);
                            uint ID = NetworkManager.ActionBlackList.GetRandomID();
                            foreach (Connection c in NetworkManager.LocalNode.Connections)
                            {
                                c.Send(true, ID, "MapClassField", NetworkManager.LocalNode.NodeID, networkclass.Name, fieldname, info.FieldType.FullName, valuestream);
                            }
                        }
                        else { Log.Write("Networking", Log.Line.Type.Error, "cant send field : {0}", fieldname); }
                    }
                }
            }
        }
    }
}
