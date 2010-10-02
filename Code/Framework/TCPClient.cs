// #define DEBUGNETWORK
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using Synergy;

namespace Synergy
{
    public class TCPConnection : Connection
    {
        public Mutex mutex = new Mutex();
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
            s.Write(NetworkClassMaster.GetInvokeCommand(_FunctionName, _Parameters));

            Send(s);
        }

        public override void Send(bool _BroadCast, string _FunctionName, params object[] _Parameters)
        {
            Send(_BroadCast, NetworkManager.ActionBlackList.GetRandomID(), _FunctionName, _Parameters);
        }

        public override void Send(ByteStream _RawData)
        {
            byte[] data = _RawData.ReadAll();
            mutex.WaitOne();
            SendBuffer.Write(BitConverter.GetBytes((ushort)data.Length));
            SendBuffer.Write(data);
            mutex.ReleaseMutex();
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

            client.ReceiveBufferSize = 65536;
            while (true)//only loops when reconnected
            {
#if !DEBUGNETWORK
                try
                {
#endif
                    //Send our localnodeID
                    Send(true, "Hello", NetworkManager.LocalNode.NodeID);

                    while (true)//loops continuous
                    {
                        if (Environment.TickCount - lastsendtime > 10000)
                        {
                            //Console.WriteLine("Keep alive.");
                            Send(false, NetworkManager.ActionBlackList.GetRandomID(), "Sleep");
                            lastsendtime = Environment.TickCount;//not that this is usefull in any way
                        }
                        mutex.WaitOne();
                        byte[] sendb = SendBuffer.Read(PACKETSIZE);
                        mutex.ReleaseMutex();
                        if (sendb.Length > 0)
                        {
                            client.Client.Send(sendb);
                            lastsendtime = Environment.TickCount;
                        }
                        if (client.Available > 0)
                        {
                            byte[] buffer = new byte[Math.Min(client.Available, PACKETSIZE)];
                            client.GetStream().Read(buffer, 0, buffer.Length);
                            mutex.WaitOne();
                            ReceiveBuffer.Write(buffer);
                            mutex.ReleaseMutex();
                            //Console.WriteLine("{0} bytes received", buffer.Length);
                        }
                        Thread.Sleep(50);
                    }
#if !DEBUGNETWORK
                }
                catch (Exception)
                {
                    OnConnectionLost();
                }
#endif
            }
        }

        private void OnConnectionLost()
        {
            currentpacketsize = 0;//we cant finish our packet, so lets reset this
            ReceiveBuffer.Clear(); SendBuffer.Clear();//trash any data that needs operating
            remotenodeid = "";//we are not sure who is on the other end
            NetworkManager.RequestNetworkMap();//request a map of the network to check what got left out
            if (AutoReconnect)//attempt resurrection
            {
                Log.Write("Networking", Log.Line.Type.Error, "Error in transmission with node {0}, connection will be resurrected", remotenodeid);
                client = null;
                while (client == null)
                {
                    try
                    {
                        client = new TcpClient(IP, Port);
                        Log.Write("Networking", Log.Line.Type.Message, "Transmission with {0}:{1} was resurrected successfully,  connection will be resurrected", IP, Port);
                    }
                    catch { Thread.Sleep(10000); }
                }
            }
            else
            {
                Log.Write("Networking", Log.Line.Type.Error, "Error in transmission with  node, connection will killed", remotenodeid);
                Kill();
                return;
            }
        }

        public void Kill()
        {
            NetworkManager.LocalNode.Connections.Remove(this);
            if (client != null) client.Close();
            thread.Abort();//final suicide action
            if (NetworkManager.LocalNode.Connections.Contains(this))
                NetworkManager.LocalNode.Connections.Remove(this);
            else
                Log.Write("Networking", Log.Line.Type.Warning, "Connection with node {0} lost", remotenodeid);
        }

        public override void Update()
        {
            while (true)
            {
                if (currentpacketsize == 0)
                {
                    //read new packetype
                    if (ReceiveBuffer.Length < 2) return; //there is no data available so we cant read any
                    currentpacketsize = BitConverter.ToUInt16(ReceiveBuffer.Read(2), 0);
                }
                if (currentpacketsize != 0)
                {
                    if (ReceiveBuffer.Length >= currentpacketsize)
                    {
                        byte[] buffer = ReceiveBuffer.Read(currentpacketsize);
                        ByteStream packet = new ByteStream(buffer);
                        bool broadcast = packet.Read() != 0;
                        uint action = (uint)Converter.Read(packet);
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
                    else return;
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
