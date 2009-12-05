using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            testclass test = new testclass();
            NetworkManager.LocalNode.AddLocalNetworkClass(test);

            while (true)
            {
                switch (Console.ReadLine().ToLower())
                {
                    case "try":
                        {
                            NetworkManager.LocalNode.Connections[0].SendCommand(test.Name, NetworkClassLocal.GetSetFieldCommand("myfield", 12));
                            NetworkManager.LocalNode.Connections[0].SendCommand(test.Name, NetworkClassLocal.GetInvokeCommand("mymethod"));
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
                        System.Diagnostics.Debugger.Break();
                        break;
                    case "whois":
                        NetworkManager.RequestNetworkMap(); break;
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            }
        }
    }
}
