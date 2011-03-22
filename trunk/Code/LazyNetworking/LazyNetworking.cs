using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace LazyNetworking
{
    public class TCPListener
    {
        public static List<TCPListener> Listeners = new List<TCPListener>();
        TcpListener listener = null;
        List<TCPConnection> connections = new List<TCPConnection>();
        public TCPConnection[] Connections { get { lock (connections) { return connections.ToArray(); } } }
        ushort Port;
        Thread thread;
        public TCPListener(ushort _Port)
        {
            Port = _Port;
            if (!Listeners.Contains(this)) Listeners.Add(this);

            thread = new Thread(new ThreadStart(main));
            thread.Start();
        }

        ~TCPListener()
        {
            try
            {
                thread.Abort();
            }
            catch { }
        }

        public void main()
        {
            listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            while (true)
            {
                if (listener.Pending())
                {
                    TCPConnection c = new TCPConnection(listener.AcceptTcpClient());
                    Console.WriteLine("Connection Added");
                    connections.Add(c);
                }
                foreach (TCPConnection c in Connections)
                {
                    if (!c.Alive)
                    {
                        Console.WriteLine("Connection Lost");
                        c.Kill();
                        connections.Remove(c);
                        break;
                    }
                    if (c.TimedOut)
                    {
                        Console.WriteLine("Connection TimedOut");
                        c.Kill();
                        connections.Remove(c);
                        break;
                    }
                }
                Thread.Sleep(20);
            }
        }
    }

    public class TCPConnection
    {
        TcpClient socket;
        Thread thread;
        int alivetimer = Environment.TickCount;
        int timeouttimer = Environment.TickCount;
        Queue<string> ReceiveBuffer = new Queue<string>();
        Queue<string> SendBuffer = new Queue<string>();
        IPAddress IP;
        ushort Port;

        public delegate void StateChange(bool _Connected);
        public event StateChange OnStateChange = null;
        bool laststateconnected = false;
        void ChangeState(bool _Connected)
        {
            if (laststateconnected != _Connected)
            {
                laststateconnected = _Connected;
                if (OnStateChange != null) OnStateChange(_Connected);
            }
        }

        public string Read()
        {
            lock (ReceiveBuffer)
            {
                if (ReceiveBuffer.Count > 0)
                    return ReceiveBuffer.Dequeue();
            }
            return null;
        }

        public void Write(string _Message)
        {
            lock (SendBuffer)
            {
                SendBuffer.Enqueue(_Message);
            }
        }
#if true
        public bool Alive { get { return (Environment.TickCount - alivetimer) < 60000 && socket != null && socket.Connected; } }//1 minute
        public bool TimedOut { get { return (Environment.TickCount - timeouttimer) > 600000; } }//1 hour
#else
        public bool Alive { get { return true; } }//1 minute
        public bool TimedOut { get { return false; } }//1 hour
#endif
        public TCPConnection(TcpClient _Socket)
        {
            socket = _Socket;
            thread = new Thread(new ThreadStart(main));
            thread.Start();
        }

        public TCPConnection(IPAddress _IP, ushort _Port)
        {
            IP = _IP;
            Port = _Port;
            thread = new Thread(new ThreadStart(main));
            thread.Start();
        }

        public void Kill()
        {
            try { socket.Close(); }
            catch { }
            try { thread.Abort(); }
            catch { }
        }
        void main()
        {
            while (true)
            {
#if !DEBUG
                try
#endif
                {
                    if (socket == null)
                    {
                        try
                        {
                            socket = new TcpClient(IP.ToString(), Port);
                            ChangeState(true);
                        }
                        catch (Exception ex)
                        {
                            socket = null;
                            ChangeState(false);
                            //Console.WriteLine("Failed To Connect");
                            //Console.WriteLine(ex.Message);
                            Thread.Sleep(5000);
                        }
                    }
                   if(socket!=null)
                    {
                        string readbuffer = "";
                        while (socket.Connected && Alive)
                        {
                            int available = (int)socket.Available;
                            if (available > 0)
                            {
                                char chr = (char)socket.GetStream().ReadByte();
                                if ((byte)chr != 0)
                                {
                                    readbuffer += chr;
                                }
                                else
                                {
                                    if (readbuffer.Length != 0)
                                    {
                                        lock (ReceiveBuffer)
                                        {
                                            ReceiveBuffer.Enqueue(readbuffer);
                                        }
                                        Console.WriteLine("< " + readbuffer);
                                        readbuffer = "";
                                    }
                                    else
                                    {
                                        //receive ping
                                    }
                                }
                                timeouttimer = Environment.TickCount;//reset timeout timer
                            }
                            else if (SendBuffer.Count > 0)
                            {
                                string buffer;
                                lock (SendBuffer)
                                {
                                    buffer = SendBuffer.Dequeue();
                                    buffer += (char)(byte)0;
                                }
                                Console.WriteLine("> " + buffer);
                                byte[] data = System.Text.Encoding.ASCII.GetBytes(buffer);
                                socket.GetStream().Write(data, 0, data.Length);
                            }
                            else Thread.Sleep(10);

                            if (Environment.TickCount - timeouttimer > 10000)
                            {
                                socket.GetStream().Write(new byte[] { 0 }, 0, 1);
                            }

                            alivetimer = Environment.TickCount;
                        }
                        Console.WriteLine("Connection lost");
                        socket = null;
                        ChangeState(false);
                        //Kill();
                    }
                }
#if !DEBUG
                catch (Exception ex) { Console.WriteLine("error in connection"); Console.WriteLine(ex.Message); }
#endif
                socket = null;
                ChangeState(false);
            }
        }
    }
}
