using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace MainStationFrontEnd
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
                    d.events.Add(new Device.Event(name, id, description));
                }
            }
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

            public Event GetEventByID(byte _ID)
            {
                foreach (Event e in events)
                {
                    if (e.ID == _ID) return e;
                }
                return null;
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
            }
        }
    }
}
