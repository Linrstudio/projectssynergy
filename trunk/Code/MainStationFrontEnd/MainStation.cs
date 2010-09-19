using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

using USBHIDDRIVER.USB;

namespace MainStationFrontEnd
{
    class MainStation
    {
        static string Vid = "vid_04d8", Pid = "pid_003f";

        static USBHIDDRIVER.USBInterface deviceinterface = null;
        //static USBHIDDRIVER.USB.HIDUSBDevice device = null;
        public static void Connect()
        {

            deviceinterface = new USBHIDDRIVER.USBInterface(Vid.ToLower(),Pid);

            string[] devices = deviceinterface.getDeviceList();

            foreach (string s in devices)
            {
                System.Diagnostics.Debugger.Log(1, "bleh", s);
            }

            if (deviceinterface.Connect())
            {
                System.Diagnostics.Debugger.Log(1, "MainStation", "MainStation connected");

                deviceinterface.enableUsbBufferEvent(new System.EventHandler(EventCatcher));
            }
            else
            {
                System.Diagnostics.Debugger.Log(1, "MainStation", "Failed to connect to MainStation");
            }

            

            //deviceinterface.startRead();

            

        }
        public static void Write(byte[] _Buffer)
        {
            if (deviceinterface == null) return;

            bool uhm = deviceinterface.write(_Buffer);

        }

        public static void Read()
        {

        }

        private static void EventCatcher(object sender, EventArgs args)
        {
            USBHIDDRIVER.List.ListWithEvent list = (USBHIDDRIVER.List.ListWithEvent)sender;
            if (list.Count > 0)
            {
                Byte[] b = (Byte[])list[0];
                for (int i = 0; i < b.Length; i++)
                {

                }
                list.Clear();
            }
        }
    }
}
