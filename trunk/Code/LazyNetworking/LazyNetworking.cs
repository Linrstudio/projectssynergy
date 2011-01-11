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
        string IP;
        ushort Port;
        Thread thread;
        public TCPListener(string _IP, ushort _Port)
        {
            IP = _IP;
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
            listener = new TcpListener(IPAddress.Parse(IP), Port);
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
#if false
        public bool Alive { get { return (Environment.TickCount - alivetimer) < 60000; } }//1 minute
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

        public void Kill()
        {
            try { socket.Close(); }
            catch { }
            try { thread.Abort(); }
            catch { }
        }

        void main()
        {
            try
            {
                string readbuffer = "";
                while (socket.Connected)
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
                            lock (ReceiveBuffer)
                            {
                                ReceiveBuffer.Enqueue(readbuffer);
                            }
                            Console.WriteLine("< " + readbuffer);
                            readbuffer = "";
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

                    alivetimer = Environment.TickCount;
                }
                Console.WriteLine("Connection lost");
            }
            catch (Exception ex) { Console.WriteLine("error in connection"); Console.WriteLine(ex.Message); }
        }
    }
}
