﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WiimoteLib;
using SynergyNode;

namespace SynergyWiimote
{
    class Program
    {
public static bool[] buttons=new bool[16];
        static void Main(string[] args)
        {
            ConnectionManager.Init();
            ConnectionManager.Connections.Add(new TCPConnection(new System.Net.Sockets.TcpClient("192.168.1.123", 1000), true));
            //ConnectionManager.OnDeviceMemoryChanged += OnMemoryChanged;
            ConnectionManager.Whois();
            
            var wm = new Wiimote();
            wm.WiimoteChanged += new WiimoteChangedEventHandler(OnWiimoteChanged);
            wm.Connect();
            wm.SetReportType(Wiimote.InputReport.Buttons | Wiimote.InputReport.IRExtensionAccel | Wiimote.InputReport.Status, true);
            
            ConnectionManager.UpdateAsync();

            while(true)
            {
                byte power=wm.WiimoteState.Battery;
                wm.SetLEDs(power>32,power>64,power>128,power>255-64);

                System.Threading.Thread.Sleep(10000);
            }

            Console.ReadLine();
        }

        public static void DoDevice(int _Device, ushort _DeviceID, bool _Button)
        {
            if (ConnectionManager.Devices.ContainsKey(_DeviceID))
            {
                if (_Button)
                {
                    if (!buttons[_Device])
                    {
                        buttons[_Device] = true;
                        Device d = ConnectionManager.Devices[_DeviceID];
                        d.ToggleDigital();
                        d.UpdateRemoteMemory();
                    }
                }
                else { buttons[_Device] = false; }
            }
        }
        public static void OnWiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            DoDevice(1, 2002, args.WiimoteState.ButtonState.Plus);
            DoDevice(2, 2001, args.WiimoteState.ButtonState.One);
            DoDevice(3, 2004, args.WiimoteState.ButtonState.Two);
            DoDevice(4, 2005, args.WiimoteState.ButtonState.A);

            DoDevice(5, 2000, args.WiimoteState.ButtonState.Up);
            DoDevice(6, 2003, args.WiimoteState.ButtonState.Down);

            //Console.WriteLine((int)(args.WiimoteState.AccelState.X*128)+128);

            if (ConnectionManager.Devices.ContainsKey(3000))
            {
                if (args.WiimoteState.ButtonState.Home)
                {
                    if (!buttons[10])
                    {
                        buttons[0] = true;
                        Device d = ConnectionManager.Devices[2000];
                        d.SetDigitalState(true);
                        d.UpdateRemoteMemory();
                        d = ConnectionManager.Devices[2001];
                        d.SetDigitalState(false);
                        d.UpdateRemoteMemory();
                        d = ConnectionManager.Devices[2002];
                        d.SetDigitalState(false);
                        d.UpdateRemoteMemory();
                        d = ConnectionManager.Devices[2003];
                        d.SetDigitalState(false);
                        d.UpdateRemoteMemory();
                        d = ConnectionManager.Devices[2004];
                        d.SetDigitalState(false);
                        d.UpdateRemoteMemory();
                        d = ConnectionManager.Devices[2005];
                        d.SetDigitalState(false);
                        d.UpdateRemoteMemory();
                        d = ConnectionManager.Devices[3000];
                        d.SetAnalogState(1);
                        d.UpdateRemoteMemory();
                    }
                }
                else { buttons[10] = false; }
            }


            if (ConnectionManager.Devices.ContainsKey(3000))
            {
                if (args.WiimoteState.ButtonState.B)
                {
                    if (!buttons[0])
                    {
                        buttons[0] = true;
                        Device d = ConnectionManager.Devices[3000];
                        d.SetAnalogState((byte)((args.WiimoteState.AccelState.X * 128) + 128));
                        d.UpdateRemoteMemory();
                    }
                }
                else { buttons[0] = false; }
            }
        }
    }
}
