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
            Base b = new Base();

            
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
           
            
            while (true)
            {
                switch (Console.ReadLine().ToLower())
                {
                    case "downloadeeprom":
                        b.DownloadEEPROM();
                        break;
                    case "uploadeeprom":
                        b.UploadEEPROM();
                        break;
                    case "build":
                        b.BuildEEPROM();
                        break;
                    case "t":
                        b.ExecuteRemoteEvent(123, 45, 243);
                        break;
                    case "t2":
                        b.ExecuteRemoteEvent(123, 46, 0);
                        break;
                    case "dumpregisters":
                        b.ExecuteRemoteEvent(1, 1, 0);
                        break;
                    case "ledon":
                        b.ExecuteRemoteEvent(1, 2, 255);
                        break;
                    case "ledoff":
                        b.ExecuteRemoteEvent(1, 2, 0);
                        break;
                    case "break":
                        System.Diagnostics.Debugger.Break();
                        break;
                    case "edit":
                        KismetEditor editor = new KismetEditor();
                        editor.ShowDialog();
                        b.devices[0].events[0].method.ByteCode = editor.GetCode();
                        break;
                    case "test":
                        Console.Write("eventargs:");
                        b.ExecuteRemoteEvent(b.devices[0].ID, b.devices[0].events[0].ID, byte.Parse(Console.ReadLine()));
                        break;
                }
                b.Read(b.Available());
                Console.WriteLine("Done");
            }
        }
    }
}
