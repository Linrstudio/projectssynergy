using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using SynergyNode;

namespace SynergyTerminal
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionManager.Init();
            new Thread(new ThreadStart(main)).Start();
            while (true)
            {
                switch (Console.ReadLine().ToLower())
                {
                    case "show":
                        foreach (Device d in ConnectionManager.RemoteDevices.Values)
                        {
                            Console.WriteLine("Remote:True ID:{0} Type:{1} Memmory:{2}", d.ID, d.DeviceType, d.Memory.GetState());
                        }
                        foreach (Device d in ConnectionManager.LocalDevices.Values)
                        {
                            Console.WriteLine("Remote:False ID:{0} Type:{1} Memmory:{2}", d.ID, d.DeviceType, d.Memory.GetState());
                        }
                        break;
                    case "add local device":
                        Console.Write("Device ID:");
                        ConnectionManager.AddLocalDevice(new LocalDevice(ushort.Parse(Console.ReadLine()), byte.Parse(Console.ReadLine())));
                        break;
                    case "add remote device":
                        Console.Write("Device ID:");
                        ConnectionManager.AddRemoteDevice(new RemoteDevice(ushort.Parse(Console.ReadLine())));
                        break;
                    case "connect":
                        {
                            Console.Write("IP:");
                            string ip = Console.ReadLine();
                            Console.Write("Port:");
                            ushort port = ushort.Parse(Console.ReadLine());
                            ConnectionManager.AddConnection(new TCPConnection(ip, port, true));
                        }break;
                    case "listen":
                        {
                            Console.Write("Port:");
                            int port = int.Parse(Console.ReadLine());
                            new TCPListener(port);
                        }break;
                    case "cow":
                        Console.WriteLine("Haha he said cow"); break;
                    case "break":
                        System.Diagnostics.Debugger.Break();
                        break;
                    case "whois":
                        ConnectionManager.RequestDeviceList(); break;
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            }
        }
        public static void main()
        {
            while (true)
            {
                ConnectionManager.Update();
                Thread.Sleep(50);
            }
        }
    }
}
