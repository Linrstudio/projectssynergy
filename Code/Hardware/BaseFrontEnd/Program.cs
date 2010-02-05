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
                    case "downloadeeprom":
                        b.DownloadEEPROM();
                        break;
                    case "uploadeeprom":
                        b.UploadEEPROM();
                        break;
                    case "t":
                        b.ExecuteRemoteEvent(123, 45, 243);
                        break;
                    case "t2":
                        b.ExecuteRemoteEvent(123, 46, 0);
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
                }
                b.Read(b.Available());
                Console.WriteLine("Done");
            }
        }
    }
}
