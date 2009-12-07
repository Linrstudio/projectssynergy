﻿//#define DEBUGNETWORK
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

namespace Framework
{
    public class TCPConnection : Connection
    {
        public int PACKETSIZE = 1024;
        public int lastsendtime = Environment.TickCount;
        Random random = new Random(Environment.TickCount);
        TcpClient client = null;
        Thread thread;
        string IP;
        ushort Port;
        bool AutoReconnect;
        int currentpacketsize = 0;

        ByteStream SendBuffer = new ByteStream();
        ByteStream ReceiveBuffer = new ByteStream();

        public override void Send(bool _BroadCast, uint _ActionID, string _FunctionName, params object[] _Parameters)
        {
            ByteStream s = new ByteStream();
            s.Write((byte)(_BroadCast ? 255 : 0));
            Converter.Write(_ActionID, s);
            s.Write(NetworkClassLocal.GetInvokeCommand(_FunctionName, _Parameters));
            //make it a packet
            byte[] data = s.ReadAll();

            Thread.BeginCriticalRegion();
            Converter.Write((ushort)data.Length, SendBuffer);
            SendBuffer.Write(data);
            Thread.EndCriticalRegion();
        }

        public override void Send(bool _BroadCast, string _FunctionName, params object[] _Parameters)
        {
            Send(_BroadCast, NetworkManager.ActionBlackList.GetRandomID(), _FunctionName, _Parameters);
        }

        public override void Send(ByteStream _RawData)
        {
            byte[] data = _RawData.ReadAll();

            Thread.BeginCriticalRegion();
            Converter.Write((ushort)data.Length, SendBuffer);
            SendBuffer.Write(data);
            Thread.EndCriticalRegion();
        }

        public override ushort GetRemoteNetworkNodeID()
        {
            return RemoteNodeID;
        }
        [Obsolete]
        public TCPConnection(TcpClient _Client, bool _AutoReconnect)
        {
            AutoReconnect = _AutoReconnect;
            IP = _Client.Client.RemoteEndPoint.ToString().Split(':')[0];
            Port = ushort.Parse(_Client.Client.RemoteEndPoint.ToString().Split(':')[1]);
            Log.Write("Networking", "TCPConnection created {0}:{1}", IP, Port);
            client = _Client;
            thread = new Thread(new ThreadStart(main));
            thread.Start();
        }
        public TCPConnection(string _IP, ushort _Port, bool _AutoReconnect)
        {
            AutoReconnect = _AutoReconnect;
            IP = _IP;
            Port = _Port;
            Log.Write("Networking", "TCPConnection created {0}:{1}", IP, Port);
            client = null;
            thread = new Thread(new ThreadStart(main));
            thread.Start();
        }

        public uint GetRandomPacketID() { return (uint)random.Next() + (uint)random.Next(); }

        public void main()
        {
            if (client == null)
            {
                //Console.WriteLine("no connection found, attempting to reconnect with {0}:{1}", IP, Port);
                try
                {
                    client = new TcpClient(IP, Port);
                }
                catch { OnConnectionLost(); }
            }//there is no connection passed by the constructor, we should try to connect now
            while (true)//only loops when reconnected
            {
#if !DEBUGNETWORK
                try
                {
#endif
                //Send our localnodeID
                Send(true, NetworkManager.ActionBlackList.GetRandomID(), "Hello", NetworkManager.LocalNode.NodeID);

                while (true)//loops continuous
                {
                    if (Environment.TickCount - lastsendtime > 10000)
                    {
                        //Console.WriteLine("Keep alive.");
                        Send(false, NetworkManager.ActionBlackList.GetRandomID(), "Sleep");
                        lastsendtime = Environment.TickCount;//not that this is usefull in any way
                    }
                    byte[] sendb = SendBuffer.Read(PACKETSIZE);
                    if (sendb.Length > 0)
                    {
                        client.Client.Send(sendb);
                        lastsendtime = Environment.TickCount;
                    }
                    if (client.Available > 0)
                    {
                        byte[] buffer = new byte[Math.Min(client.Available, PACKETSIZE)];
                        client.GetStream().Read(buffer, 0, buffer.Length);
                        ReceiveBuffer.Write(buffer);
                        //Console.WriteLine("{0} bytes received", buffer.Length);
                    }
                    Thread.Sleep(20);
                }
#if !DEBUGNETWORK
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                    OnConnectionLost();
                }
#endif
            }
        }

        private void OnConnectionLost()
        {
            currentpacketsize = 0;//we cant finish our packet, so lets reset this
            ReceiveBuffer.Clear(); SendBuffer.Clear();//trash any data that needs operating
            RemoteNodeID = 0;//we are not sure who is on the other end
            NetworkManager.RequestNetworkMap();//request a map of the network to check what got left out
            if (AutoReconnect)//attempt resurrection
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
                    catch { Thread.Sleep(10000); }
                }
            }
            else
            {
                Console.WriteLine("Error in transmission, connection will be killed");
                Kill();
                return;
            }
        }

        public void Kill()
        {
            NetworkManager.LocalNode.Connections.Remove(this);
            if (client != null) client.Close();
            thread.Abort();//final suicide action
        }

        public override void Update()
        {
            if (currentpacketsize == 0)
            {
                //read new packetype
                if (ReceiveBuffer.GetSize() < 2) return; //there is no data available so we cant ready any
                currentpacketsize = (ushort)Converter.Read(typeof(ushort), ReceiveBuffer);
            }
            if (currentpacketsize != 0)
            {
                if (ReceiveBuffer.GetSize() >= currentpacketsize)
                {
                    byte[] buffer = ReceiveBuffer.Read(currentpacketsize);
                    ByteStream packet = new ByteStream(buffer);
                    bool broadcast = packet.Read() != 0;
                    uint action = (uint)Converter.Read(typeof(uint), packet);
                    if (!NetworkManager.ActionBlackList.Contains(action))
                    {
                        NetworkManager.ActionBlackList.Add(action);
                        if (broadcast)
                        {
                            foreach (Connection c in NetworkManager.LocalNode.Connections)
                            {
                                if (c != this) c.Send(new ByteStream(buffer));
                            }
                        }
                        methods.ExecuteCommand(packet);
                    }
                    currentpacketsize = 0;
                }
            }
        }

        public static void LoadSettingsFile(string _Path)
        {
            try
            {
                XElement root = XElement.Load(_Path);
                foreach (XElement e in root.Elements("TCPConnection"))
                {
                    try
                    {
                        string IP = (string)e.Attribute("IP").Value;
                        ushort Port = ushort.Parse((string)e.Attribute("Port").Value);
                        bool Resurrect = bool.Parse((string)e.Attribute("Resurrect").Value);

                        new TCPConnection(IP, Port, Resurrect);
                    }
                    catch { Console.WriteLine("Could not parse TCPConnection in connections file"); }
                }
            }
            catch { Console.WriteLine("Could not open {0}", _Path); }
        }
    }
}
