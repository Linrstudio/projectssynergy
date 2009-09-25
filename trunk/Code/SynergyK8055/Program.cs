using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;

using SynergyNode;

namespace SynergyK8055
{
    public class DigitalOutput : LocalDevice
    {
        [DllImport("k8055.dll")]
        private static extern void SetDigitalChannel(int Channel);
        [DllImport("k8055.dll")]
        private static extern void ClearDigitalChannel(int Channel);
        public int Channel;
        public DigitalOutput(ushort _DeviceID):base(_DeviceID, 10) { }
        public override void  OnMemoryChanged()
        {
            DigitalMemoryBin bin = ((DigitalMemoryBin)Memory);
            if (bin != null)
            {
                SetState(bin.On != bin.Inversed);
            }
        }
        public void SetState(bool _On)
        {
            Console.Write("DigitalOut {0} -> {1}", Channel, _On);
            if (_On) SetDigitalChannel(Channel); else ClearDigitalChannel(Channel);
        }
    }
    public class DigitalInput : LocalDevice
    {
        [DllImport("k8055.dll")]
        private static extern int ReadDigitalChannel(int Channel);
        public int Channel;
        public DigitalInput(ushort _DeviceID) : base(_DeviceID, 11) { }
        public void Update()
        {
            DigitalMemoryBin bin = ((DigitalMemoryBin)Memory);

            bool last = bin.On;
            bool current = ReadDigitalChannel(Channel) != 0;
            if (last != current)
            {
                bin.On = current;
                Memory = bin;
                UpdateRemoteMemory();
                Console.WriteLine("{0} is now {1}", Channel, current);
            }
        }
    }
    public class AnalogOutput : LocalDevice
    {
        [DllImport("k8055.dll")]
        private static extern void OutputAnalogChannel(int Channel, int Data);
        public int Channel;
        public AnalogOutput(ushort _DeviceID) : base(_DeviceID, 12) { }
        public override void OnMemoryChanged()
        {
            AnalogMemoryBin bin = ((AnalogMemoryBin)Memory);
            SetState(bin.Value);
        }
        public void SetState(byte _Value)
        {
            Console.Write("AnalogOut {0} -> {1}", Channel, _Value);
            OutputAnalogChannel(Channel, _Value);
        }
    }

    class Program
    {
        [DllImport("k8055.dll")]
        private static extern int OpenDevice(int CardAddress);
        [DllImport("k8055.dll")]
        private static extern void CloseDevice();
        [DllImport("k8055.dll")]
        private static extern int ReadAnalogChannel(int Channel);
        [DllImport("k8055.dll")]
        private static extern void OutputAnalogChannel(int Channel, int Data);

        static void Main(string[] args)
        {
            ConnectionManager.Init();

            Type[] types=new Type[]
                {
                    typeof(DigitalInput), 
                    typeof(DigitalOutput),
                    typeof(MemoryBin) 
                };

            Dictionary<Type, List<object>> list = Utilities.LoadSettingsFile("Settings.xml", types );

            //Console.Read();
            Console.WriteLine("Connecting to K8055");
            OpenDevice(0);

            TCPListener.LoadSettingsFile("Connections.xml");
            TCPConnection.LoadSettingsFile("Connections.xml");

            DigitalInput[] digitalinputs = new DigitalInput[5];
            DigitalOutput[] digitaloutputs = new DigitalOutput[8];
            AnalogOutput[] analogoutputs = new AnalogOutput[2];

            for (int i = 0; i < 5; i++)
            {
                var dev = new DigitalInput(0);
                Device.LoadSettingsFile("Settings.xml", dev, "DigitalInput" + i.ToString());
                dev.Memory = MemoryBin.GetBinForType(dev.DeviceType);
                Device.LoadSettingsFile("Settings.xml", dev.Memory, "DigitalInput" + i.ToString() + "MemoryBin");
                digitalinputs[i] = dev;
                ConnectionManager.AddLocalDevice(dev);
            }
            
            for (int i = 0; i < 8; i++)
            {
                var dev = new DigitalOutput(0);
                Device.LoadSettingsFile("Settings.xml", dev, "DigitalOutput" + i.ToString());
                dev.Memory = MemoryBin.GetBinForType(dev.DeviceType);
                Device.LoadSettingsFile("Settings.xml", dev.Memory, "DigitalOutput" + i.ToString() + "MemoryBin");
                digitaloutputs[i] = dev;
                ConnectionManager.AddLocalDevice(dev);
            }
            
            for (int i = 0; i < 2; i++)
            {
                var dev = new AnalogOutput(0);
                Device.LoadSettingsFile("Settings.xml", dev, "AnalogOutput" + i.ToString());
                dev.Memory = MemoryBin.GetBinForType(dev.DeviceType);
                Device.LoadSettingsFile("Settings.xml", dev.Memory, "AnalogOutput" + i.ToString() + "MemoryBin");
                analogoutputs[i] = dev;
                ConnectionManager.AddLocalDevice(dev);
            }
            
            while (true)
            {
                foreach (DigitalInput i in digitalinputs) { i.Update(); }
                ConnectionManager.Update();
                Thread.Sleep(100);
            }
        }
    }
}
