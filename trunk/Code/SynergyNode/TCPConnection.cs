﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace SynergyNode
{
    public class TCPConnection:Connection
    {
        TcpClient client = null;
        Thread thread;
        string IP;
        ushort Port;
        bool AutoConnect;
        Queue<Packet> SendQueue = new Queue<Packet>();
        Queue<uint> BlackList = new Queue<uint>();
        public TCPConnection(TcpClient _Client, bool _AutoConnect)
        {
            AutoConnect = _AutoConnect;
            IP = _Client.Client.RemoteEndPoint.ToString().Split(':')[0];
            Port = ushort.Parse(_Client.Client.RemoteEndPoint.ToString().Split(':')[1]);
            //Console.WriteLine("TCPConnection created {0}:{1}", IP, Port);
            client = _Client;
            thread = new Thread(new ThreadStart(main));
            thread.Start();
        }
        public TCPConnection(string _IP,ushort _Port, bool _AutoConnect)
        {
            AutoConnect = _AutoConnect;
            IP = _IP;
            Port = _Port;
            //Console.WriteLine("TCPConnection created {0}:{1}", IP, Port);
            client = null;
            thread = new Thread(new ThreadStart(main));
            thread.Start();
        }
        public void AddToBlackList(uint _PacketID)
        {
            //Console.WriteLine("{0} added to blacklist", _PacketID);
            BlackList.Enqueue(_PacketID);
            while (BlackList.Count > 100) BlackList.Dequeue();//trim the end
        }
        public bool InBlackList(uint _PacketID)
        {
            foreach (uint v in BlackList.ToArray()) if (v == _PacketID) return true;
            return false;
        }
        public void main()
        {
            if (client == null)
            {
                Console.WriteLine("no connection found, attempting to reconnect with {0}:{1}", IP, Port);
                try
                {
                    client = new TcpClient(IP, Port);
                }
                catch { OnConnectionLost(); }
            }//there is no connection passed by the constructor, we should try to connect now
            while (true)//only loops when reconnected
            {
                try
                {
                    while (true)//loops continuous
                    {
                        if (SendQueue.Count > 0)
                        {
                            Thread.BeginCriticalRegion();
                            Packet p = SendQueue.Dequeue();
                            Thread.EndCriticalRegion();
                            if (!InBlackList(p.PacketID))
                            {
                                AddToBlackList(p.PacketID);
                                byte[] buffer = p.GetPacketBytes();
                                client.GetStream().Write(buffer, 0, buffer.Length);
                                //Console.WriteLine(" > {0}", System.Text.Encoding.ASCII.GetString(buffer));
                                Console.WriteLine("Packet sent ( {2} ) PacketID:{0} Data:{1}", p.PacketID, System.Text.Encoding.ASCII.GetString(p.Data), client.Client.RemoteEndPoint.ToString());
                            }
                            else 
                            { 
                                //Console.WriteLine("packet blocked by black list");
                            }
                        }
                        if (client.Available > 0)
                        {
                            Packet p = null;
                            //try
                            {
                                p = new Packet((Stream)client.GetStream());
                            }
                            //catch { Console.WriteLine("invalid packet at TCPConnection {0}:{1}", IP, Port); }
                            if (p != null)
                            {
                                if (!InBlackList(p.PacketID))//if it is not on the blacklist add it and spread around the other connections
                                {
                                    Console.WriteLine("Packet received ( {2} ) PacketID:{0} Data:{1}", p.PacketID, System.Text.Encoding.ASCII.GetString(p.Data), client.Client.RemoteEndPoint.ToString());
                                    AddToBlackList(p.PacketID);
                                    ConnectionManager.AddReceivePacket(p);
                                    ConnectionManager.SendPacket(p);
                                }
                            }
                        }
                        Thread.Sleep(10);
                    }
                }
                catch
                {
                    OnConnectionLost();
                }
            }
        }
        private void OnConnectionLost()
        {
            if (AutoConnect)//attempt resurrection if allowed by family
            {
                Console.WriteLine("Error in transmission, connection will be resurrected");
                client = null;
                while (client == null)
                {
                    try
                    {
                        client = new TcpClient(IP, Port);
                        Console.WriteLine("TCPConnection with {0}:{1} was resurrected successfully", IP, Port);
                    }
                    catch { Console.WriteLine("Failed to resurrect TCPConnection with {0}:{1}  -  reattempt in 10s", IP, Port); Thread.Sleep(10000); }
                }
            }
            else
            {
                Console.WriteLine("Error in transmission, connection will be killed");
                Kill();
                return;
            }
        }
        public override void SendPacket(Packet _Packet)
        {
            Thread.BeginCriticalRegion();
            SendQueue.Enqueue(_Packet);
            Thread.EndCriticalRegion();
        }
        public void Kill()
        {
            ConnectionManager.Connections.Remove(this);
            client.Close();
            thread.Abort();//final suicide action
        }
        public override void Update()
        {

        }
        public static void LoadConnectionFile(string _Path)
        {
            try
            {
                XElement root = XElement.Load(_Path);
                foreach (XElement e in root.Elements("TCPConnection"))
                {
                    try
                    {
                        string IP = (string)e.Element("IP").Value;
                        ushort Port = ushort.Parse((string)e.Element("Port").Value);
                        bool Resurrect = bool.Parse((string)e.Element("Resurrect").Value);

                        SynergyNode.ConnectionManager.Connections.Add(new TCPConnection(IP, Port, Resurrect));
                    }
                    catch { Console.WriteLine("Could not parse TCPConnection in connections file"); }
                }
            }
            catch { Console.WriteLine("Could not open {0}",_Path); }
        }
    }
}
