using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace SynergyClient
{
    public struct ConnectionListItem
    {
        public ConnectionListItem(string _IP, ushort _Port) { IP = _IP; Port = _Port; }
        public string IP;
        public ushort Port;
    }
    public static class ConnectionList
    {
        public static List<ConnectionListItem> Items=new List<ConnectionListItem>();
        public static void Load(string _Path)
        {
            XElement root = XElement.Load(_Path);
            foreach(XElement e in root.Elements("Connection"))
            {
                Items.Add(new ConnectionListItem((string)e.Element("IP").Value, ushort.Parse((string)e.Element("Port").Value)));
            }
        }
        public static void Save(string _Path)
        {
            XElement root = new XElement("Connections");
            foreach(ConnectionListItem e in Items)
            {
                XElement c = new XElement("Connection");
                c.Add(new XElement("IP",e.IP));
                c.Add(new XElement("Port",e.Port.ToString()));
                root.Add(c);
            }
            root.Save(_Path);
        }
    }
}
