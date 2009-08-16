using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SynergyNode
{
    public class TCPListener
    {
        TcpListener listener;
        Thread thread;
        int port;
        public TCPListener(int _Port)
        {
            port = _Port;
            thread = new Thread(new ThreadStart(main));
            thread.Start();
        }
        public void main()
        {
            try
            {
                listener = new TcpListener(System.Net.IPAddress.Any, port);
                listener.Start();
                Console.WriteLine("Listener started at port {0}", port);
            }
            catch { Console.WriteLine("Failed to start Listener at port {0}", port); return; }
            
            while (true)
            {
                if (listener.Pending())
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ConnectionManager.Connections.Add(new TCPConnection(client, false));
                    Console.WriteLine("TCPConnection with {0}",client.Client.RemoteEndPoint.ToString());
                }
                Thread.Sleep(100);
            }
        }
        ~TCPListener()
        {
            thread.Abort();
        }
    }
}
