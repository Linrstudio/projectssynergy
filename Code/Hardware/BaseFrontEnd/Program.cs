using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
            Base b = new Base("COM69");

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
            while (true)
            {
                switch (Console.ReadLine().ToLower())
                {
                    case "save":
                        b.eeprom.Save("test.eeprom");
                        break;
                    case "load":
                        b.eeprom = EEPROM.FromFile("test.eeprom");
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
                    case "edit":
                        {
                            KismetEditor editor = new KismetEditor(b.eeprom);
                            editor.ShowDialog();
                        }
                        break;
                    case "invoke":
                        Console.Write("DeviceID:");
                        ushort deviceid = ushort.Parse(Console.ReadLine());
                        Console.Write("EventID:");
                        byte eventid = byte.Parse(Console.ReadLine());
                        Console.Write("EventArgs:");
                        byte eventargs = byte.Parse(Console.ReadLine());
                        b.ExecuteRemoteEvent(deviceid, eventid, eventargs);
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
                    case "t":
                        b.Write('0'); b.WaitForY();
                        b.WriteShort(1220);
                        //b.Read(1);
                        Console.WriteLine("[{0}]",b.ReadShort());
                        //Console.WriteLine();

                        break;
                }
                b.Read(b.Available());
                Console.WriteLine("Done");
            }
        }
    }
}
