using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Speech.Synthesis;
using SynergyNode;

namespace SynergyK8055
{
    class RoenyRoom
    {

        static bool ringing = false;
        public static void Init()
        {
            //NetworkNode.Init();
            //NetworkNode.AddConnection(new TCPConnection("127.0.0.1", 1000, true));
            NetworkNode.OnDeviceMemoryChanged += OnDeviceMemoryChanged;
            //Thread.Sleep(1000);
            //NetworkNode.RequestNetworkMap();
            //while (true) Tick();
        }

        public static void Update() { }

        static void DoorBell()
        {
            if (ringing) return;
            ringing = true;
            ushort audioid = 2004;
            if (NetworkNode.LocalDevices.ContainsKey(audioid))
            {
                Device dev = NetworkNode.LocalDevices[audioid];
                bool originalstate = ((DigitalMemoryBin)dev.Memory).On;
                if (!originalstate)
                {
                    ((DigitalMemoryBin)dev.Memory).On = true;
                    dev.UpdateRemoteMemory();
                    Device door = NetworkNode.LocalDevices[2005];
                    ((DigitalMemoryBin)door.Memory).On = true;
                    door.UpdateRemoteMemory();
                    Thread.Sleep(2000);
                }
                SpeechSynthesizer sam = new SpeechSynthesizer();
                sam.Speak("Ding Dong, Someone is at the door.");
                if (!originalstate)
                {
                    ((DigitalMemoryBin)dev.Memory).On = false;
                    dev.UpdateRemoteMemory();
                }
            }
            ringing = false;
        }

        static void OnDeviceMemoryChanged(Device _Device)
        {
            if (_Device.ID == 1001)
            {
                if (((DigitalMemoryBin)_Device.Memory).On)
                {
                    Thread t = new Thread(new ThreadStart(DoorBell));
                    t.Start();
                }
            }
        }
    }
}
