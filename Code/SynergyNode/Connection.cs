using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

namespace SynergyNode
{
    public class Connection
    {
        public virtual void Update() { }
        public virtual void RequestDeviceList(uint _ActionID, bool _Broadcast) { }
        public virtual void SendDeviceMemoryBin(uint _ActionID, bool _Broadcast, Device _Device) { }

        public virtual void SendDeviceListElement(uint _ActionID, bool _Broadcast, LocalDevice _Device) { }

        public delegate void OnReceiveRequestDeviceListHandler();
        public virtual event OnReceiveRequestDeviceListHandler OnReceiveRequestDeviceList;

        //the connection should add the device to the device list
        public delegate void OnReceiveDeviceListElementHandler(RemoteDevice _Device);
        public virtual event OnReceiveDeviceListElementHandler OnReceiveDeviceListElement;

        //the connection itself should copy the new memorybin in the right device
        public delegate void OnReceiveDeviceMemoryBinHandler(Device _Device);
        public virtual event OnReceiveDeviceMemoryBinHandler OnReceiveDeviceMemoryBin;

        
    }
}
