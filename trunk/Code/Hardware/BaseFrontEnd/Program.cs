using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO.Ports;

namespace BaseFrontEnd
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Resources.Load();
            ProductDataBase.Load(@"Products.xml");
            Base b = new Base("COM1");

            /*
                        PushEvent root = new PushEvent();
                        Constant on = new Constant(); on.Value = 1;
                        SetDebugLed led = new SetDebugLed();
                        root.Outputs[0].Connected.Add(on.Inputs[0]);
                        on.Inputs[0].Connected = root.Outputs[0];

                        on.Outputs[0].Connected.Add(led.Inputs[0]);
                        led.Inputs[0].Connected = on.Outputs[0];

                        byte[] ledcode = CodeAssambler.Assamble(root);

                        b.devices[0].events[0].method.ByteCode = ledcode;
                        */
            //b.eeprom = EEPROM.FromFile("test.eeprom");
#if true
            {
                b.eeprom = EEPROM.FromFile("test.eeprom");
                var w = new MainWindow(b);
                w.ShowDialog();
            }
#endif

            while (true)
            {
                switch (Console.ReadLine().ToLower())
                {
                    case "save":
                        Console.Write("Filename:");
                        b.eeprom.Save(Console.ReadLine() + ".eeprom");
                        break;
                    case "load":
                        Console.Write("Filename:");
                        b.eeprom = EEPROM.FromFile(Console.ReadLine() + ".eeprom");
                        break;
                    case "downloadmemory":
                        b.DownloadEEPROM();
                        break;
                    case "build":
                        b.AssambleEEPROM();
                        break;
                    case "uploadmemory":
                        b.UploadEEPROM();
                        break;
                    case "uploadmemory2":
                        b.UploadEEPROMBruteForce();
                        break;
                    case "checkmemory":
                        Console.WriteLine("Check okay:{0}", b.CheckEEPROM());
                        break;
                    case "readtime":
                        b.ReadTime();
                        break;
                    case "synctime":
                        b.SetTime(DateTime.Now);
                        break;
                    case "readvariables":
                        b.ReadVariables();
                        break;
                    case "break":
                        System.Diagnostics.Debugger.Break();
                        break;
                    case "disablekismet":
                        b.KismetDisable();
                        break;
                    case "enabledkismet":
                        b.KismetEnable();
                        break;
                    case "plcwrite":
                        b.PLCWrite(byte.Parse(Console.ReadLine()));
                        break;
                    case "invoke":
                        {
                            Console.Write("DeviceID:");
                            ushort deviceid = ushort.Parse(Console.ReadLine());
                            Console.Write("EventID:");
                            byte eventid = byte.Parse(Console.ReadLine());
                            Console.Write("EventArgs:");
                            byte eventargs = byte.Parse(Console.ReadLine());
                            b.ExecuteRemoteEvent(deviceid, eventid, eventargs);
                        }
                        break;
                    case "w":
                        {
                            Console.WriteLine("Address:");
                            ushort addr = ushort.Parse(Console.ReadLine());
                            Console.WriteLine("Data:");
                            byte data = byte.Parse(Console.ReadLine());
                            b.Write('m'); b.WaitForY(); b.Write('x');
                            b.WriteShort(addr);
                            b.Write(data);
                        }
                        break;
                    case "r":
                        {
                            Console.WriteLine("Address:");
                            ushort addr = ushort.Parse(Console.ReadLine());
                            b.Write('m'); b.WaitForY(); b.Write('y');
                            b.WriteShort(addr);
                            Console.Write("[{0}]", b.Read(1)[0]);
                        }
                        break;
                    case "seteepromsize":
                        b.Write('s'); b.WaitForY(); b.Write('e');
                        b.WriteShort(2048);
                        break;
                    case "mainwindow":
                        {
                            var w = new MainWindow(b);
                            w.ShowDialog();
                            break;
                        }
                }
                b.Read(b.Available());
                Console.WriteLine("Done");
            }
        }
    }
}
