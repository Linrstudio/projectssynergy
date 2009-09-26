﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

namespace SynergyNode
{
    public class TCPConnection:Connection
    {
        public const uint PACKETSIZE = 1024;
        public const byte REQUESTDEVICELISTID = 1;
        public const byte SENDDEVICELISTELEMENTID = 2;
        public const byte UPDATEREMOTEMEMORYID = 3;
        Random random = new Random(Environment.TickCount);
        TcpClient client = null;
        Thread thread;
        string IP;
        ushort Port;
        bool AutoReconnect;

        Queue<byte> SendBuffer = new Queue<byte>();
        Queue<byte> ReceiveBuffer = new Queue<byte>();

        ushort currentpacketsize = 0;

        //fix weirdness in event inheriting
        public override event Connection.OnReceiveDeviceListElementHandler OnReceiveDeviceListElement;
        public override event OnReceiveRequestDeviceListHandler OnReceiveRequestDeviceList;
        public override event Connection.OnReceiveDeviceMemoryBinHandler OnReceiveDeviceMemoryBin;

        public override void SendDeviceListElement(uint _ActionID, bool _Broadcast, LocalDevice _Device)
        {
            MemoryStream stream = new MemoryStream();
            stream.WriteByte(SENDDEVICELISTELEMENTID);//ID is always first
            stream.WriteByte((byte)(_Broadcast ? 255 : 0));

            stream.Write(BitConverter.GetBytes(_ActionID), 0, 4);//Action ID

            stream.WriteByte(_Device.DeviceType);//Type
            stream.Write(BitConverter.GetBytes(_Device.ID), 0, 2);//ID
            byte[] devicememory = ToBytes(_Device.Memory);
            stream.Write(BitConverter.GetBytes((ushort)devicememory.Length), 0, 2);//memorybin size
            stream.Write(devicememory, 0, devicememory.Length);//memorybin
            SendData(stream.ToArray());
        }
        public override void  SendDeviceMemoryBin(uint _ActionID, bool _Broadcast, Device _Device)
        {
            MemoryStream stream = new MemoryStream();
            stream.WriteByte(UPDATEREMOTEMEMORYID);//ID is always first
            stream.WriteByte((byte)(_Broadcast ? 255 : 0));

            stream.Write(BitConverter.GetBytes(_ActionID), 0, 4);//Action ID

            stream.WriteByte(_Device.DeviceType);//Type
            stream.Write(BitConverter.GetBytes(_Device.ID), 0, 2);//ID
            byte[] devicememory = ToBytes(_Device.Memory);
            stream.Write(BitConverter.GetBytes((ushort)devicememory.Length), 0, 2);//memorybin size
            stream.Write(devicememory, 0, devicememory.Length);//memorybin
            SendData(stream.ToArray());
        }

        public override void RequestDeviceList(uint _ActionID, bool _Broadcast)
        {
            MemoryStream stream = new MemoryStream();
            stream.WriteByte(REQUESTDEVICELISTID);//ID is always first 
            stream.WriteByte((byte)(_Broadcast ? 255 : 0));
            stream.Write(BitConverter.GetBytes(_ActionID), 0, 4);//Action ID
            SendData(stream.ToArray());
        }

        private void ParseData(byte[] _Data)
        {
            try
            {
                switch (_Data[0])
                {
                    case REQUESTDEVICELISTID://ReceiveDeviceList
                        {
                            Console.WriteLine("REQUESTDEVICELISTID");
                            bool BroadCast = _Data[1] != 0;
                            uint ActionID = BitConverter.ToUInt32(_Data, 2);
                            if (!ConnectionManager.ActionBlackList.Contains(ActionID))
                            {
                                ConnectionManager.ActionBlackList.Add(ActionID);
                                //broadcast
                                if (BroadCast) foreach (Connection c in ConnectionManager.Connections) if (c != this) c.RequestDeviceList(ActionID, true);

                                if (OnReceiveRequestDeviceList != null) OnReceiveRequestDeviceList();
                            }
                        }
                        break;
                    case SENDDEVICELISTELEMENTID://ReceiveDevice
                        {
                            Console.WriteLine("SENDDEVICELISTELEMENTID");
                            bool BroadCast = _Data[1] != 0;
                            uint ActionID = BitConverter.ToUInt32(_Data, 2);

                            if (!ConnectionManager.ActionBlackList.Contains(ActionID))
                            {
                                ConnectionManager.ActionBlackList.Add(ActionID);
                                byte type = _Data[6];
                                ushort ID = BitConverter.ToUInt16(_Data, 7);
                                ushort membinsize = BitConverter.ToUInt16(_Data, 9);
                                byte[] membin = new byte[membinsize];
                                for (int i = 0; i < membinsize; i++)
                                    membin[i] = _Data[i + 11];
                                RemoteDevice remotedevice = new RemoteDevice(ID);
                                LocalDevice localdevice = new LocalDevice(ID);
                                remotedevice.DeviceType = localdevice.DeviceType = type;
                                remotedevice.Memory = localdevice.Memory = FromBytes(type, membin);
                                //add device to devicelist if it is not present ( or local )
                                if (!ConnectionManager.RemoteDevices.ContainsKey(ID) && !ConnectionManager.LocalDevices.ContainsKey(ID))
                                {
                                    Console.WriteLine("Device added ---------------");
                                    ConnectionManager.AddRemoteDevice(remotedevice);
                                }
                                else
                                {
                                    if (remotedevice.DeviceType != ConnectionManager.RemoteDevices[ID].DeviceType)
                                    {
                                        Console.WriteLine("Device with ID:{0} already added using another memorybin type ( conflicting devices? )", ID);
                                    }
                                }
                                //broadcast
                                if (BroadCast) foreach (Connection c in ConnectionManager.Connections) if (c != this) c.SendDeviceListElement(ActionID, true, localdevice);
                                //only trigger this if we dont have a device with this ID
                                if (!ConnectionManager.RemoteDevices.ContainsKey(ID) && !ConnectionManager.LocalDevices.ContainsKey(ID))
                                {
                                    if (OnReceiveDeviceListElement != null) OnReceiveDeviceListElement(remotedevice);
                                }
                            }
                        }
                        break;
                    case UPDATEREMOTEMEMORYID:
                        {
                            Console.WriteLine("UPDATEREMOTEMEMORYID");
                            bool BroadCast = _Data[1] != 0;
                            uint ActionID = BitConverter.ToUInt32(_Data, 2);

                            if (!ConnectionManager.ActionBlackList.Contains(ActionID))
                            {
                                ConnectionManager.ActionBlackList.Add(ActionID);
                                byte type = _Data[6];
                                ushort ID = BitConverter.ToUInt16(_Data, 7);
                                ushort membinsize = BitConverter.ToUInt16(_Data, 9);
                                byte[] membin = new byte[membinsize];
                                for (int i = 0; i < membinsize; i++)
                                    membin[i] = _Data[i + 11];
                                MemoryBin bin = FromBytes(type, membin);

                                Device device = null;
                                RemoteDevice remdev = null;
                                if (ConnectionManager.LocalDevices.ContainsKey(ID)) device = ConnectionManager.LocalDevices[ID];
                                if (ConnectionManager.RemoteDevices.ContainsKey(ID)) { device = remdev = ConnectionManager.RemoteDevices[ID]; }
                                if (device == null)
                                {
                                    Console.WriteLine("memory for unknown device found ID:{0}", ID);
                                    Console.WriteLine("note: it is possible to add this device ( kick Roeny in the face to fix this)");
                                }
                                else
                                {
                                    if (device.DeviceType != type) Console.WriteLine("Device type does not match type received, something is fishy here.");
                                    device.Memory = bin;
                                    device.OnMemoryChanged();
                                    //broadcast
                                    if (BroadCast) foreach (Connection c in ConnectionManager.Connections) if (c != this) c.SendDeviceMemoryBin(ActionID, true, device);
                                    if (OnReceiveDeviceMemoryBin != null) OnReceiveDeviceMemoryBin(device);
                                }
                            }
                        }
                        break;
                }
            }
            catch { Console.WriteLine("Invalid packet receive, data was trashed and connection survived ( not sure whats wrong tho )"); }
        }

        public void SendData(byte[] _Data)
        {
            Thread.BeginCriticalRegion();
            byte[] length = BitConverter.GetBytes((ushort)_Data.Length);
            foreach (byte b in length) SendBuffer.Enqueue(b);
            foreach (byte b in _Data) SendBuffer.Enqueue(b);
            Thread.EndCriticalRegion();
        }

        public byte[] ReadData(uint _Amount)
        {
            byte[] data = new byte[_Amount];
            for (int i = 0; i < _Amount; i++) data[i] = ReceiveBuffer.Dequeue();
            return data;
        }

        public byte[] ReadData()
        {
            byte[] buffer = new byte[] { ReceiveBuffer.Dequeue(), ReceiveBuffer.Dequeue() };
            ushort length = BitConverter.ToUInt16(buffer,0);
            byte[] data = new byte[length];
            for (int i = 0; i < length; i++) data[i] = ReceiveBuffer.Dequeue();
            return data;
        }

        public TCPConnection(TcpClient _Client, bool _AutoReconnect)
        {
            AutoReconnect = _AutoReconnect;
            IP = _Client.Client.RemoteEndPoint.ToString().Split(':')[0];
            Port = ushort.Parse(_Client.Client.RemoteEndPoint.ToString().Split(':')[1]);
            //Console.WriteLine("TCPConnection created {0}:{1}", IP, Port);
            client = _Client;
            thread = new Thread(new ThreadStart(main));
            thread.Start();
        }
        public TCPConnection(string _IP, ushort _Port, bool _AutoReconnect)
        {
            AutoReconnect = _AutoReconnect;
            IP = _IP;
            Port = _Port;
            //Console.WriteLine("TCPConnection created {0}:{1}", IP, Port);
            client = null;
            thread = new Thread(new ThreadStart(main));
            thread.Start();
        }

        public uint GetRandomPacketID() { return (uint)random.Next() + (uint)random.Next(); }
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
                        if (SendBuffer.Count>0)
                        {
                            MemoryStream stream = new MemoryStream();
                            Thread.BeginCriticalRegion();
                            while (SendBuffer.Count > 0) stream.WriteByte(SendBuffer.Dequeue());
                            Thread.EndCriticalRegion();
                            byte[] buffer = stream.ToArray();
                            client.GetStream().Write(buffer, 0, buffer.Length);
                        }
                        if (client.Available > 0)
                        {
                            byte[] buffer = new byte[Math.Min(client.Available,PACKETSIZE)];
                            client.GetStream().Read(buffer, 0, buffer.Length);
                            Thread.BeginCriticalRegion();
                            foreach (byte b in buffer) ReceiveBuffer.Enqueue(b);
                            Thread.EndCriticalRegion();
                            Console.WriteLine("{0} bytes received", buffer.Length);
                        }
                        Thread.Sleep(10);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    OnConnectionLost();
                }
            }
        }
        private void OnConnectionLost()
        {
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

        public void Kill()
        {
            ConnectionManager.Connections.Remove(this);
            client.Close();
            thread.Abort();//final suicide action
        }

        public override void Update()
        {
            if (currentpacketsize == 0)
            {
                //read new packetype
                if (ReceiveBuffer.Count <2) return; //there is no data available so we cant ready any
                Thread.BeginCriticalRegion();
                currentpacketsize = BitConverter.ToUInt16(ReadData(2), 0);
                Thread.EndCriticalRegion();
            }
            if (ReceiveBuffer.Count >= currentpacketsize)
            {
                byte[] packetdata = ReadData(currentpacketsize);
                ParseData(packetdata);
                currentpacketsize = 0;
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
                        string IP = (string)e.Element("IP").Value;
                        ushort Port = ushort.Parse((string)e.Element("Port").Value);
                        bool Resurrect = bool.Parse((string)e.Element("Resurrect").Value);

                        ConnectionManager.AddConnection(new TCPConnection(IP, Port, Resurrect));
                    }
                    catch { Console.WriteLine("Could not parse TCPConnection in connections file"); }
                }
            }
            catch { Console.WriteLine("Could not open {0}",_Path); }
        }

        public byte[] ToBytes(MemoryBin _MemoryBin)
        {
            if (_MemoryBin == null) return new byte[] { };
            MemoryStream s = new MemoryStream();
            Type t = _MemoryBin.GetType();
            foreach (FieldInfo i in t.GetFields())
            {
                string name = i.FieldType.Name;
                try
                {
                    if (name == typeof(byte).Name)
                    {
                        s.WriteByte((byte)i.GetValue(_MemoryBin));
                    }
                    if (name == typeof(bool).Name)
                    {
                        s.WriteByte((byte)((bool)i.GetValue(_MemoryBin) ? 255 : 0));
                    }
                }
                catch { }
            }
            return s.GetBuffer();
        }
        public MemoryBin FromBytes(byte _Type, byte[] _Bytes)
        {
            MemoryBin b = MemoryBin.GetBinForType(_Type);
            if (b == null) return null;
            Type t = b.GetType();
            int index = 0;
            foreach (FieldInfo i in t.GetFields())
            {
                string name = i.FieldType.Name;
                try
                {
                    if (name == typeof(byte).Name)
                    {
                        i.SetValue(b, _Bytes[index]);
                        index += sizeof(byte);
                    }
                    if (name == typeof(bool).Name)
                    {
                        i.SetValue(b, _Bytes[index] != 0);
                        index += sizeof(byte);
                    }
                }
                catch { }
            }
            return b;
        }
    }
}
