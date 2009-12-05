using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

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
                //Console.WriteLine("Hello {0}", _NodeID);
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
                //Console.WriteLine("Connection added {0} <-> {1}", node1, node2);
            }

            [NetworkMethod("MapClass")]
            public void AddClass(ushort _NodeID, string _ClassName)
            {
                if (!NetworkManager.RemoteNodes.ContainsKey(_NodeID)) NetworkManager.AddRemoteNode(_NodeID);
                NetworkNodeRemote node = NetworkManager.RemoteNodes[_NodeID];
                if (!node.LocalDevices.ContainsKey(_ClassName)) node.LocalDevices.Add(_ClassName, new NetworkClassRemote(_ClassName));
                //Console.WriteLine("Network class {0} added", _ClassName);
            }

            [NetworkMethod("ExecuteCommand")]
            public void ihateyou(string _TargetClass, ByteStream _Command)
            {
                if (NetworkManager.LocalNode.NetworkClasses.ContainsKey(_TargetClass))
                    NetworkManager.LocalNode.NetworkClasses[_TargetClass].ExecuteCommand(_Command);
                else
                {
                    //Console.WriteLine("Network class {0} not found", _TargetClass);
                }
            }

            [NetworkMethod("RequestNetworkMap")]
            public void RequestNetworkMap()
            {
                //Console.WriteLine("NetworkMap request received");
                //send connections
                foreach (Connection connection in NetworkManager.LocalNode.Connections)
                {
                    uint ID = NetworkManager.ActionBlackList.GetRandomID();
                    foreach (Connection c in NetworkManager.LocalNode.Connections)
                    {
                        if (connection.GetRemoteNetworkNodeID() != 0)
                            c.Send(true, NetworkManager.ActionBlackList.GetRandomID(), "MapConnection", NetworkManager.LocalNode.NodeID, connection.GetRemoteNetworkNodeID());
                    }
                }
                //send classes
                foreach (NetworkClassLocal networkclass in NetworkManager.LocalNode.NetworkClasses.Values)
                {
                    uint ID = NetworkManager.ActionBlackList.GetRandomID();
                    foreach (Connection c in NetworkManager.LocalNode.Connections)
                    {
                        c.Send(true, NetworkManager.ActionBlackList.GetRandomID(), "MapClass", NetworkManager.LocalNode.NodeID, networkclass.Name);
                    }
                }
            }
        }
    }
}
