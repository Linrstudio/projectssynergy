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
        public DigitalOutput(ushort _DeviceID, int _Channel):base(_DeviceID,0){Channel = _Channel;}
        public override void  OnMemoryChanged()
        {
            SetState(GetDigitalState());
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
        public DigitalInput(ushort _DeviceID, int _Channel) : base(_DeviceID, 1) { Channel = _Channel; }
        public void Update()
        {
            bool last = GetDigitalState();
            bool current = ReadDigitalChannel(Channel) != 0;
            if (last != current)
            {
                SetDigitalState(current);
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
        public AnalogOutput(ushort _DeviceID, int _Channel) : base(_DeviceID,2) { Channel = _Channel; }
        public override void OnMemoryChanged()
        {
            SetState(GetAnalogState());
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
            DigitalInput[] digitalinputs = new DigitalInput[5];
            DigitalOutput[] digitaloutputs = new DigitalOutput[8];
            AnalogOutput[] analogoutputs = new AnalogOutput[2];
            XElement file = XElement.Load("Settings.xml");
            for (int i = 0; i < 5; i++)//0
            {
                XElement element = file.Element("DigitalInput" + i.ToString());
                digitalinputs[i] = new DigitalInput(ushort.Parse(element.Element("DeviceID").Value), i + 1);
                ConnectionManager.AddDevice(digitalinputs[i]);
            }
            for (int i = 0; i < 8; i++)//1
            {
                XElement element = file.Element("DigitalOutput" + i.ToString());
                digitaloutputs[i] = new DigitalOutput(ushort.Parse(element.Element("DeviceID").Value), i + 1);
                ConnectionManager.AddDevice(digitaloutputs[i]);
            }
            for (int i = 0; i < 2; i++)//2
            {
                XElement element = file.Element("AnalogOutput" + i.ToString());
                analogoutputs[i] = new AnalogOutput(ushort.Parse(element.Element("DeviceID").Value), i + 1);
                ConnectionManager.AddDevice(analogoutputs[i]);
            }

            Console.WriteLine("Connecting to K8055");
            Console.Write("insert card address:");
            OpenDevice(int.Parse(Console.ReadLine()));
            Console.WriteLine("TCPListener Setup");
            Console.Write("Port:");
            TCPListener listner = new TCPListener(int.Parse(Console.ReadLine()));
            while (true)
            {
                foreach (DigitalInput i in digitalinputs) { i.Update(); }
                ConnectionManager.Update();
                Thread.Sleep(100);
            }
        }
    }
}
