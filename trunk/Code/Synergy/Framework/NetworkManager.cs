using System;
using System.Collections.Generic;

using System.Text;
using System.Text.RegularExpressions;
using System.Threading;


namespace Synergy
{
    public static class NetworkManager
    {
        public static string Revision = "6.000";
        public static void Initialize()
        {
            Log.Write("Default", "Framework initialized Version:{0}", Revision);
        }
        public static Random random = new Random(Environment.TickCount);
        public static NetworkNodeLocal LocalNode = new NetworkNodeLocal();
        public static Dictionary<string, NetworkNodeRemote> RemoteNodes = new Dictionary<string, NetworkNodeRemote>();

        public static void AddRemoteNode(string _NodeID)
        {
            if (!NetworkManager.RemoteNodes.ContainsKey(_NodeID)) RemoteNodes.Add(_NodeID, new NetworkNodeRemote(_NodeID));
        }

        public static void AddConnection(string _NodeID1, string _NodeID2)
        {
            if (_NodeID1 == "" || _NodeID2 == "")
            {
                Log.Write("Network Manager", "Connections to non initialized Nodes not allowed");
                return;
            }

            if (_NodeID1 == _NodeID2)
            {
                Log.Write("Network Manager", "Connection is illegal {0} <-> {1} ", _NodeID1, _NodeID2);
                return;
            }
            AddRemoteNode(_NodeID1);
            AddRemoteNode(_NodeID2);
            if (!NetworkManager.RemoteNodes[_NodeID1].RemoteNodes.ContainsKey(_NodeID2)) NetworkManager.RemoteNodes[_NodeID1].RemoteNodes.Add(_NodeID2, NetworkManager.RemoteNodes[_NodeID2]);
            if (!NetworkManager.RemoteNodes[_NodeID2].RemoteNodes.ContainsKey(_NodeID1)) NetworkManager.RemoteNodes[_NodeID2].RemoteNodes.Add(_NodeID1, NetworkManager.RemoteNodes[_NodeID1]);
            Log.Write("Networking", "Connection added {0} <-> {1}", _NodeID1, _NodeID2);
        }

        private static Thread ThreadUpdateAsync = null;
        public static void StartUpdateAsync()
        {
            if (ThreadUpdateAsync == null)
            {
                ThreadUpdateAsync = new Thread(new ThreadStart(main));
                ThreadUpdateAsync.Start();
                Console.WriteLine("Async Update started");
            }
        }

        public static void StopUpdateAsync()
        {
            if (ThreadUpdateAsync != null)//that'll do :)
            {
                ThreadUpdateAsync.Abort();
                ThreadUpdateAsync = null;
            }
        }

        static void main()
        {
            while (true)
            {
                Update();
                Thread.Sleep(20);
            }
        }

        public static void InvokeCommand(string _Command)
        {
            string[] split1 = _Command.Split('.');
            string nodename = split1[0].Trim();
            string classname = split1[1].Trim();
            string[] split2 = split1[2].Split('=');
            string fieldname = split2[0].Trim();
            string fieldvalue = split2[1].Trim();
            if (!RemoteNodes.ContainsKey(nodename)) { Log.Write("NetworkManager", Log.Line.Type.Error, "Failed to find remotenode : {0}", nodename); return; }
            NetworkNodeRemote node = RemoteNodes[nodename];
            if (!node.LocalDevices.ContainsKey(classname)) { Log.Write("NetworkManager", Log.Line.Type.Error, "Failed to find class : {0}", classname); return; }
            NetworkClassSlave slave = node.LocalDevices[classname];
            if (!slave.Fields.ContainsKey(fieldname)) { Log.Write("NetworkManager", Log.Line.Type.Error, "Failed to find field : {0}", fieldname); return; }
            object value = slave.Fields[fieldname].FieldType.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { fieldvalue });
            if (value == null) { Log.Write("NetworkManager", "Failed to parse value : {0}", Log.Line.Type.Error, fieldvalue); return; }
            slave.Fields[fieldname].Value = value;
        }

        public static void Update()
        {
            Log.Write(new Log.Variable("NetworkManager", "RemoteNodes", RemoteNodes.Count));
            LocalNode.Update();
            foreach (NetworkNode node in RemoteNodes.Values) node.Update();
            PluginManager.Update();
            Log.Update();
        }

        public static void RequestNetworkMap()
        {

        }

        internal static class ActionBlackList
        {
            private static Queue<uint> blacklist = new Queue<uint>();
            public static uint Size = 100;
            public static uint GetRandomID()
            {
                uint newid = (uint)random.Next() + (uint)random.Next();
                if (Contains(newid))
                {
                    Log.Write("Default", "ZOMG! the ID already existed, this was a one out of 43.000.000 chance!");
                    newid = GetRandomID();
                }
                return newid;
            }

            public static void Add(uint _ActionID)
            {
                //Console.WriteLine("{0} added to blacklist", _PacketID);
                blacklist.Enqueue(_ActionID);
                while (blacklist.Count > Size) blacklist.Dequeue();//trim the end
            }
            public static bool Contains(uint _ActionID)
            {
                foreach (uint v in blacklist.ToArray()) if (v == _ActionID) return true;
                return false;
            }
        }
    }
}
