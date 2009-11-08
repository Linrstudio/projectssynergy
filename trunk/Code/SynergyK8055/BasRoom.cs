using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Speech.Synthesis;
using SynergyNode;

namespace SynergyK8055
{
    class BasRoom
    {
        public class CrossDigitalOutput : LocalDevice
        {
            [DllImport("k8055.dll")]
            private static extern void SetDigitalChannel(int Channel);
            [DllImport("k8055.dll")]
            private static extern void ClearDigitalChannel(int Channel);
            public int Channel;
            public ushort inputID;
            public CrossDigitalOutput(ushort _DeviceID) : base(_DeviceID, 10) { }
            public override void OnMemoryChanged()
            {
                DigitalMemoryBin bin = ((DigitalMemoryBin)Memory);
                DigitalMemoryBin inbin = ((DigitalMemoryBin)NetworkNode.Devices.GetDevice(inputID).Memory);
                
                SetState(bin.On == inbin.On);
            }
            public void SetState(bool _On)
            {
                if (_On) SetDigitalChannel(Channel); else ClearDigitalChannel(Channel);
            }
        }

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
