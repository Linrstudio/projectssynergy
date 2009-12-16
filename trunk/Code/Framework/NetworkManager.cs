using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Framework
{
    public static class NetworkManager
    {
        public static string Revision = "6.000";
        public static void Initialize()
        {
            Log.Write("default", "Framework initialized Version:{0}", Revision);
        }
        public static Random random = new Random(Environment.TickCount);
        public static NetworkNodeLocal LocalNode = new NetworkNodeLocal();
        public static Dictionary<ushort, NetworkNodeRemote> RemoteNodes = new Dictionary<ushort, NetworkNodeRemote>();

        public static void AddRemoteNode(ushort _NodeID)
        {
            if (!NetworkManager.RemoteNodes.ContainsKey(_NodeID)) RemoteNodes.Add(_NodeID, new NetworkNodeRemote(_NodeID));
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

        public static void Update()
        {
            LocalNode.Update();
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
                    Console.WriteLine("ZOMG! the ID already existed, this was a one out of 43.000.000 chance!");
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
