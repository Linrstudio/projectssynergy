using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace SynergyNode
{
    public static class NetworkNode
    {
        public delegate void OnDeviceFoundHandler(Device _Device);
        public static  event OnDeviceFoundHandler OnDeviceFound;
        public delegate void OnDeviceMemoryChangedHandler(Device _Device);
        public static  event OnDeviceMemoryChangedHandler OnDeviceMemoryChanged;

        public static string Revision =  "4.200";

        public static List<Connection> Connections;
        public static Dictionary<ushort, LocalDevice> LocalDevices;
        public static Dictionary<ushort, RemoteDevice> RemoteDevices;

        public static void AddConnection(Connection _Connection)
        {
            Connections.Add(_Connection);
            _Connection.OnReceiveRequestDeviceList += TriggerOnRequestDeviceList;
            _Connection.OnReceiveDeviceListElement += TriggerOnDeviceFound;
            _Connection.OnReceiveDeviceMemoryBin += TriggerOnDeviceMemoryChanged;
        }

        public static void AddLocalDevice(LocalDevice _Device)
        {
            LocalDevices.Add(_Device.ID, _Device);
        }
        public static void AddRemoteDevice(RemoteDevice _Device)
        {
            RemoteDevices.Add(_Device.ID, _Device);
        }

        public static void Init()
        {
            
            Connections = new List<Connection>();
            LocalDevices = new Dictionary<ushort, LocalDevice>();
            RemoteDevices = new Dictionary<ushort, RemoteDevice>();
            Console.WriteLine("NetworkNode Initialized");
            Console.WriteLine("Version:{0}", Revision);
        }

        public static void UpdateAsync()
        {
            new Thread(new ThreadStart(main)).Start();
        }

        static void main()
        {
            while (true)
            {
                Update();
                Thread.Sleep(50);
            }
        }

        public static void RequestDeviceList()
        {
            uint ID = ActionBlackList.GetRandomID();
            Console.WriteLine("device list requested ActionID:{0}", ID);
            foreach (Connection c in Connections) c.RequestDeviceList(ID, true);
        }

        private static void TriggerOnRequestDeviceList()
        {
            Console.WriteLine("Device list request received");
            foreach (LocalDevice d in LocalDevices.Values)
            {
                uint ID = ActionBlackList.GetRandomID();
                foreach (Connection c in Connections) c.SendDeviceListElement(ID, true, d);
            }
            Console.WriteLine("Devices Returned:{0}", LocalDevices.Values.Count);
        }

        private static void TriggerOnDeviceMemoryChanged(Device _Device)
        {
            Console.WriteLine("Device memory received");
            if (OnDeviceMemoryChanged != null) OnDeviceMemoryChanged(_Device);
        }

        private static void TriggerOnDeviceFound(RemoteDevice _Device)
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
            foreach (Connection c in Connections) c.Update();
        }

        public static class ActionBlackList
        {
            public static Random random= new Random(Environment.TickCount);
            private static Queue<uint> blacklist = new Queue<uint>();
            public static uint Size=100;
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
