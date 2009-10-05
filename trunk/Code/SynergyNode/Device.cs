using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

namespace SynergyNode
{
    public class Device
    {
        public Device(bool _IsLocal) { islocal = _IsLocal; }
        private bool islocal;//do not change
        public ushort ID;
        public byte DeviceType;

        public MemoryBin Memory;

        public bool IsLocal() { return islocal; }

        public virtual void OnMemoryChanged() { }

        public virtual void UpdateRemoteMemory()
        {
            NetworkNode.SendDeviceMemoryBin(this);
        }

        public static bool LoadSettingsFile(string _Path, object _Device, string _SectionName)//fills the fields of this device
        {
            bool ret = false;
            try
            {
                XElement root = XElement.Load(_Path);
                Type t = _Device.GetType();

                foreach (XElement sect in root.Elements(_SectionName))//fetch the right section
                {
                    ret = true;
                    foreach (XElement f in sect.Elements())
                    {
                        foreach (FieldInfo i in t.GetFields())
                        {
                            if (i.Name == f.Name)
                            {
                                Console.WriteLine(i.FieldType.Name);
                                try
                                {
                                    object o = i.FieldType.GetMethod("Parse", new Type[] { f.Value.GetType() }).Invoke(i, new object[] { (object)f.Value });
                                    i.SetValue(_Device, o); Console.WriteLine("Field {0} set to value {1}", f.Name, f.Value);
                                }
                                catch { Console.Write("Could not Parse data."); }
                            }
                        }
                    }
                }
            }
            catch { Console.WriteLine("Could not open {0}", _Path); }
            return ret;
        }
    }

    public class LocalDevice : Device
    {
        public LocalDevice(ushort _DeviceID) : base(true)
        {
            ID = _DeviceID;
            Memory = new DefaultMemoryBin();
        }
        public LocalDevice(ushort _DeviceID,byte _Type) : base(true)
        {
            ID = _DeviceID;
            DeviceType = _Type;
            Memory = MemoryBin.GetBinForType(_Type);
        }
    }

    public class RemoteDevice : Device
    {
        public RemoteDevice(ushort _DeviceID) : base(false)
        {
            ID = _DeviceID;
            Memory = new DefaultMemoryBin();
        }
    }
}
