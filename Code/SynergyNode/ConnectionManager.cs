using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace SynergyNode
{
    public static class ConnectionManager
    {
        public static string Revision =  "2.160";
        public static Random random;
        public static List<Connection> Connections;
        private static Queue<Packet> ReceiveQueue;
        private static Queue<Packet> SendQueue;

        public static Dictionary<uint, Device> Devices;
        
        public static void AddDevice(Device _Device)//you can also do this yourself
        {
            Devices.Add(_Device.ID, _Device);
        }

        public static void Init()
        {
            random = new Random(Environment.TickCount);
            Connections = new List<Connection>();
            ReceiveQueue = new Queue<Packet>();
            SendQueue = new Queue<Packet>();
            Devices = new Dictionary<uint, Device>();
            Console.WriteLine("ConnectionManager Initialized");
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
                Thread.Sleep(10);
            }
        }

        public static void SetRemoteMemory(ushort _DeviceID, byte[] _Memory)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(BitConverter.GetBytes(_DeviceID), 0, 2);
            stream.Write(_Memory, 0, _Memory.Length);
            Packet p = new Packet(0, stream.ToArray());
            SendQueue.Enqueue(p);
        }

        public static void Whois()
        {
            Packet p = new Packet(1, new byte[] { 0 });
            SendQueue.Enqueue(p);
        }

        public static void AddReceivePacket(Packet _Packet) { Thread.BeginCriticalRegion(); ReceiveQueue.Enqueue(_Packet); Thread.EndCriticalRegion(); }
        public static void /* */ SendPacket(Packet _Packet) { Thread.BeginCriticalRegion(); SendQueue.Enqueue(_Packet); Thread.EndCriticalRegion(); }

        public static void Update()
        {
            //main parsing of packets
            if (SendQueue.Count > 0)
            {
                Packet p = SendQueue.Dequeue();
                foreach (Connection c in Connections) c.SendPacket(p);
            }

            while (ReceiveQueue.Count > 0)
            {
                Thread.BeginCriticalRegion();
                Packet packet = ReceiveQueue.Dequeue();
                Thread.EndCriticalRegion();
                if (packet != null)
                {
                    Console.WriteLine("Parsing packet");
                    switch (packet.Type)//swap the banana with the apple
                    {
                        case 0://packets as they all should be
                            {
                                ushort ReceivedID = BitConverter.ToUInt16(packet.Data, 0);
                                byte[] Data = packet.GetPiece(2, (uint)packet.Data.Length - 2);
                                foreach (Device d in Devices.Values)
                                {
                                    if (d.ID == ReceivedID)
                                    {
                                        d.SetMemory(Data, false);
                                        Console.WriteLine("{0} received MemoryBin", d.ID);
                                    }
                                }
                            }
                            break;
                        case 1://WhoIs
                            {
                                Console.WriteLine("returning device info");
                                foreach (Device d in Devices.Values)
                                {
                                    if (d.IsLocal())
                                    {
                                        MemoryStream stream = new MemoryStream();
                                        stream.WriteByte(d.DeviceType);
                                        stream.Write(BitConverter.GetBytes(d.ID), 0, 2);
                                        stream.Write(d.Memory, 0, d.Memory.Length);
                                        SendQueue.Enqueue(new Packet(2, stream.ToArray()));
                                        Console.WriteLine("returning device {0}", d.ID);
                                    }
                                }
                            }
                            break;
                        case 2://WhoIsResult
                            {
                                byte type = packet.Data[0];
                                ushort device = BitConverter.ToUInt16(packet.Data, 1);
                                if (!Devices.ContainsKey(device))
                                {
                                    RemoteDevice d = new RemoteDevice(device);
                                    d.SetMemory(packet.GetPiece(3, (uint)packet.Data.Length - 3), false);
                                    AddDevice(d);
                                    Console.WriteLine("whois result added");
                                }
                                else { Console.WriteLine("we already have this device {0}", device); }
                            }
                            break;
                    }
                }
                else { Console.WriteLine("Error in dequeueing packet, note: the exact reasons for this behaviour is unknown"); }
            }
            foreach (Connection c in Connections) c.Update();
        }
    }
}
