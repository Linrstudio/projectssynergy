using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace MainStation
{
    public static class ProductDataBase
    {
        public static List<Device> Devices = new List<Device>();
        public static void Load(string _FileName)
        {
            Devices.Clear();
            XElement file = XElement.Load(_FileName);
            foreach (XElement device in file.Elements("Device"))
            {
                Device d = null;
                {
                    string name = device.Attribute("Name").Value;
                    ushort id = ushort.Parse(device.Attribute("ID").Value);
                    string description = device.Attribute("Description").Value;
                    d = new Device(name, id, description);
                }
                foreach (XElement evnt in device.Elements("Event"))
                {
                    string name = evnt.Attribute("Name").Value;
                    byte id = byte.Parse(evnt.Attribute("ID").Value);
                    string description = evnt.Attribute("Description").Value;
                    Device.Event newevent = new Device.Event(name, id, description);
                    d.events.Add(newevent);
                    foreach (XElement output in evnt.Elements("Output"))
                    {
                        newevent.Outputs.Add(new Device.Event.Output(output.Attribute("Name").Value, output.Attribute("Type").Value));
                    }
                }
                foreach (XElement evnt in device.Elements("RemoteEvent"))
                {
                    string name = evnt.Attribute("Name").Value;
                    byte id = byte.Parse(evnt.Attribute("ID").Value);
                    string description = evnt.Attribute("Description").Value;
                    Device.RemoteEvent newevent = new Device.RemoteEvent(name, id, description);
                    d.remoteevents.Add(newevent);
                    foreach (XElement input in evnt.Elements("Input"))
                    {
                        newevent.Inputs.Add(new Device.RemoteEvent.Input(input.Attribute("Name").Value,input.Attribute("Type").Value));
                    }
                    foreach (XElement output in evnt.Elements("Output"))
                    {
                        newevent.Outputs.Add(new Device.RemoteEvent.Output(output.Attribute("Name").Value, output.Attribute("Type").Value));
                    }
                }
            }
            //Utilities.Log("Product database loaded.");
        }

        public static Device GetDeviceByID(ushort _ID)
        {
            foreach (Device d in Devices)
            {
                if (d.ID == _ID) return d;
            }
            return null;
        }

        public class Device
        {
            public Device(string _Name, ushort _ID, string _Description)
            {
                name = _Name;
                id = _ID;
                description = _Description;
                ProductDataBase.Devices.Add(this);
            }
            string name;
            public string Name { get { return name; } }
            ushort id;
            public ushort ID { get { return id; } }
            string description;
            public string Description { get { return description; } }

            public List<Event> events = new List<Event>();
            public List<RemoteEvent> remoteevents = new List<RemoteEvent>();

            public Event GetEventByID(byte _ID)
            {
                foreach (Event e in events)
                {
                    if (e.ID == _ID) return e;
                }
                return null;
            }

            public RemoteEvent GetRemoteEventByID(byte _ID)
            {
                foreach (RemoteEvent e in remoteevents)
                {
                    if (e.ID == _ID) return e;
                }
                return null;
            }

            public class RemoteEvent
            {
                public RemoteEvent(string _Name, byte _ID, string _Description)
                {
                    name = _Name;
                    id = _ID;
                    description = _Description;
                }
                byte id;
                public byte ID { get { return id; } }
                string name;
                public string Name { get { return name; } }
                string description;
                public string Description { get { return description; } }

                public List<Input> Inputs = new List<Input>();
                public List<Output> Outputs = new List<Output>();

                public class Input
                {
                    public Input(string _Name, string _Type)
                    {
                        Name = _Name;
                        Type = _Type;
                    }
                    public string Name;
                    public string Type;
                }

                public class Output
                {
                    public Output(string _Name, string _Type)
                    {
                        Name = _Name;
                        Type = _Type;
                    }
                    public string Name;
                    public string Type;
                }
            }

            public class Event
            {
                public Event(string _Name, byte _ID, string _Description)
                {
                    name = _Name;
                    id = _ID;
                    description = _Description;
                }
                byte id;
                public byte ID { get { return id; } }
                string name;
                public string Name { get { return name; } }
                string description;
                public string Description { get { return description; } }

                public List<Output> Outputs = new List<Output>();

                public class Output
                {
                    public Output(string _Name, string _Type)
                    {
                        Name = _Name;
                        Type = _Type;
                    }
                    public string Name;
                    public string Type;
                }
            }
        }
    }
}
