using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace SynergyNode
{
    public static class NetworkNode
    {
        public static Random random = new Random(Environment.TickCount);


        private static ushort NetworkNodeID;

        public delegate void OnDeviceFoundHandler(Device _Device);
        public static  event OnDeviceFoundHandler OnDeviceFound;
        public delegate void OnDeviceMemoryChangedHandler(Device _Device);
        public static  event OnDeviceMemoryChangedHandler OnDeviceMemoryChanged;

        public static string Revision =  "4.205";

        public static List<Connection> Connections;
        public static DeviceList<Device> Devices;
        public static Dictionary<ushort, LocalDevice> LocalDevices;
        public static Dictionary<ushort, RemoteDevice> RemoteDevices;

        public static Dictionary<ushort, RemoteNetworkNode> RemoteNodes;

        public static Dictionary<string, LocalNetworkClass> LocalNetworkClasses = new Dictionary<string, LocalNetworkClass>();

        public static Device GetDevice(ushort _Device)
        {
            lock (LocalDevices)
            {
                if (LocalDevices.ContainsKey(_Device)) return LocalDevices[_Device];
            }
            lock (RemoteDevices)
            {
                if (RemoteDevices.ContainsKey(_Device)) return RemoteDevices[_Device];
            }
            return null;//we dont have this device, return null
        }

        public static void Init()
        {
            Connections = new List<Connection>();
            Devices = new DeviceList<Device>();
            LocalDevices = new Dictionary<ushort, LocalDevice>();
            RemoteDevices = new Dictionary<ushort, RemoteDevice>();
            RemoteNodes = new Dictionary<ushort, RemoteNetworkNode>();
            Console.WriteLine("NetworkNode Initialized");
            Console.WriteLine("Version:{0}", Revision);
            Random random = new Random(Environment.TickCount);
            NetworkNodeID = (ushort)(Math.Abs(random.Next()) % ushort.MaxValue);//generate random ID
            RemoteNodes.Add(GetID(), new RemoteNetworkNode(GetID()));
        }

        public static void AddConnection(Connection _Connection)
        {
            lock (Connections)
            {
                Connections.Add(_Connection);
                _Connection.OnReceiveRequestNetworkMap += TriggerOnRequestNetworkMap;
                _Connection.OnReceiveDeviceListElement += TriggerOnDeviceFound;
                _Connection.OnReceiveDeviceMemoryBin += TriggerOnDeviceMemoryChanged;
                _Connection.OnReceiveConnection += TriggerOnReceiveConnection;
            }
        }

        public static void AddLocalDevice(LocalDevice _Device)
        {
            Devices.AddDevice(_Device);
            LocalDevices.Add(_Device.ID, _Device);
        }

        public static void AddRemoteDevice(RemoteDevice _Device,ushort _NodeID)
        {
            Devices.AddDevice(_Device);
            if (!RemoteNodes.ContainsKey(_NodeID)) RemoteNodes.Add(_NodeID, new RemoteNetworkNode(_NodeID));
            RemoteDevices.Add(_Device.ID, _Device);
            if(!RemoteNodes[_NodeID].LocalDevices.ContainsKey(_Device.ID))
                RemoteNodes[_NodeID].LocalDevices.Add(_Device.ID, _Device);
        }

        public static ushort GetID() { return NetworkNodeID; }

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
                Thread.Sleep(50);
            }
        }
        public static void RequestNetworkMap()
        {
            uint ID = ActionBlackList.GetRandomID();
            Console.WriteLine("network map requested ActionID:{0}", ID);
            foreach (Connection c in Connections) c.RequestNetworkMap(ID, true);
        }

        internal static void TriggerOnReceiveConnection(ushort _NodeA, ushort _NodeB)
        {
            if (_NodeA == 0 || _NodeB == 0) return;
            if (!RemoteNodes.ContainsKey(_NodeA)) RemoteNodes.Add(_NodeA, new RemoteNetworkNode(_NodeA));
            if (!RemoteNodes.ContainsKey(_NodeB)) RemoteNodes.Add(_NodeB, new RemoteNetworkNode(_NodeB));
            if (!RemoteNodes[_NodeA].RemoteNodes.ContainsKey(_NodeB)) RemoteNodes[_NodeA].RemoteNodes.Add(_NodeB, RemoteNodes[_NodeB]);
            if (!RemoteNodes[_NodeB].RemoteNodes.ContainsKey(_NodeA)) RemoteNodes[_NodeB].RemoteNodes.Add(_NodeA, RemoteNodes[_NodeA]);
            Console.WriteLine("Connection added to remote");
        }

        private static void TriggerOnRequestNetworkMap()
        {
            Console.WriteLine("Device list request received");
            foreach (LocalDevice d in LocalDevices.Values)
            {
                uint ID = ActionBlackList.GetRandomID();
                foreach (Connection c in Connections) c.SendDeviceListElement(ID, true, d);
            }
            foreach (Connection connection in Connections)
            {
                uint ID = ActionBlackList.GetRandomID();
                foreach (Connection c in Connections) c.SendConnection(ID, true, connection);
            }
            Console.WriteLine("Devices Returned:{0}", LocalDevices.Values.Count);
        }

        private static void TriggerOnDeviceMemoryChanged(Device _Device)
        {
            Console.WriteLine("Device memory received");
            if (OnDeviceMemoryChanged != null) OnDeviceMemoryChanged(_Device);
        }

        internal static void TriggerOnDeviceFound(RemoteDevice _Device)
        {
            if (OnDeviceFound != null) OnDeviceFound(_Device);
        }

        public static void SendDeviceMemoryBin(Device _Device)
        {
            Console.WriteLine("Update Remote memorybin");
            uint ID = ActionBlackList.GetRandomID();
            foreach (Connection c in Connections) c.SendDeviceMemoryBin(ID, true, _Device);
        }

        public static void Update()
        {
            foreach (Connection c in new List<Connection>(Connections.ToArray())) c.Update();
        }

        public static class ActionBlackList
        {
            private static Queue<uint> blacklist = new Queue<uint>();
            public static uint Size = 100;
            public static uint GetRandomID() { return (uint)random.Next() + (uint)random.Next(); }

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
