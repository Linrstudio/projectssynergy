using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynergyNode
{
    public class Event
    {
        public string format = "";

        public Event() { }
        public Event(string _Format) { format = _Format; }

        public void Parse(string _Value) { format = _Value; }
        public void Invoke()
        {
            foreach (string instruction in format.Split(';'))
            {
                string str = instruction.Replace("\n", "").Replace(" ", "");
                if (instruction != "")
                {
                    //try
                    {
                        string[] sdot = str.Split('.');
                        string[] sequals = sdot[1].Split('=');
                        ushort DeviceID = ushort.Parse(sdot[0]);
                        string Field = sequals[0];
                        string Value = sequals[1];
                        //try
                        {
                            Device dev = null;
                            if (NetworkNode.RemoteDevices.ContainsKey(DeviceID))
                                dev = NetworkNode.RemoteDevices[DeviceID];
                            if (NetworkNode.LocalDevices.ContainsKey(DeviceID))
                                dev = NetworkNode.LocalDevices[DeviceID];

                            if (dev != null)
                            {
                                dev.Memory.SetField(Field, Value);
                                Console.WriteLine("{0} executed", str);
                                dev.UpdateRemoteMemory();
                            }
                        }
                        //catch { }
                    }
                    //catch { Console.WriteLine("Syntax error in : {0}", str); }
                }
            }
        }
    }
}
