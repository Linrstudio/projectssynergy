using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Framework;

namespace SynergyConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkManager.Initialize();
            new TCPListener(1111);
            NetworkManager.StartUpdateAsync();

            while (true)
            {
                switch (Console.ReadLine().ToLower())
                {
                    case "load":
                        {
                            PluginManager.LoadPlugin(@".\plugins\testplugin.cs");
                        } break;
                    case "try":
                        {
                            NetworkManager.LocalNode.Connections[0].SendCommand("analog out 1", NetworkClassLocal.GetSetFieldCommand("Value", 128));
                        } break;
                    case "connect":
                        {
                            Console.Write("IP:");
                            string ip = Console.ReadLine();
                            new TCPConnection(ip, 1111, true);
                        } break;
                    case "listen":
                        {
                            Console.Write("Port:");
                            int port = int.Parse(Console.ReadLine());
                            new TCPListener(port);
                        } break;
                    case "cow":
                        Console.WriteLine("Haha he said cow"); break;
                    case "break":
                        Debugger.Break();
                        break;
                    case "map":
                        NetworkManager.RequestNetworkMap(); break;
                    case "clear":
                        Console.Clear(); break;
                    case "exit":
                        Process.GetCurrentProcess().Kill(); break;
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            }
        }
    }
}
