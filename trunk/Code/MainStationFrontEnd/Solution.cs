using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using SynergySequence;

namespace MainStationFrontEnd
{
    public static class Solution
    {
        static List<ProgrammableDevice> programmabledevices = new List<ProgrammableDevice>();
        public static ProgrammableDevice[] ProgrammableDevices { get { return programmabledevices.ToArray(); } }

        public static void AddComputer(Computer _Computer)
        {
            if (!programmabledevices.Contains(_Computer))
            {
                programmabledevices.Add(_Computer);
            }
        }

        public static void Load(string _File)
        {
            programmabledevices.Clear();
            XElement file = XElement.Load(_File);
            foreach (XElement element in file.Elements("ProgrammableDevice"))
            {
                string typename = element.Attribute("Type").Value;
                ProgrammableDevice d;
                d = (ProgrammableDevice)Activator.CreateInstance(null, "MainStationFrontEnd." + typename).Unwrap();
                d.Load(element);
                programmabledevices.Add(d);
            }
        }

        public static void Save(string _File)
        {
            XElement file = new XElement("Solution");
            foreach (ProgrammableDevice d in ProgrammableDevices)
            {
                XElement element = new XElement("ProgrammableDevice");
                element.SetAttributeValue("Type", d.GetType().Name);
                d.Save(element);
                file.Add(element);
            }
            file.Save(_File);
        }
    }
}
