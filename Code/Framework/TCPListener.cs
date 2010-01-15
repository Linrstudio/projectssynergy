﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using SynergyTemplate;

namespace Framework
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
                Log.Write("Networking", "Listener started at port {0}", port);
            }
            catch
            {
                Log.Write("Networking", "Failed to start Listener at port {0}", port);
                return;
            }

            while (true)
            {
                if (listener.Pending())
                {
                    TcpClient client = listener.AcceptTcpClient();
                    new TCPConnection(client, false);
                }
                Thread.Sleep(100);
            }
        }

        public static void LoadSettingsFile(string _Path)
        {
            try
            {
                XElement root = XElement.Load(_Path);
                foreach (XElement e in root.Elements("TCPListener"))
                {
                    try
                    {
                        ushort Port = ushort.Parse((string)e.Element("Port").Value);
                        new TCPListener(Port);
                    }
                    catch { Log.Write("IO", "Could not parse TCPListner in connections file"); }
                }
            }
            catch { Log.Write("IO", "Could not open {0}", _Path); }
        }

        ~TCPListener()
        {
            thread.Abort();
        }
    }
}