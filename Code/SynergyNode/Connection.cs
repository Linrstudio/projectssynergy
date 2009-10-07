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

        public virtual ushort GetRemoteNetworkNodeID() { return 0; }

        public virtual void SendConnection(uint _ActionID, bool _Broadcast, Connection _Connection) { }

        public virtual void RequestNetworkMap(uint _ActionID, bool _Broadcast) { }
        public virtual void SendDeviceMemoryBin(uint _ActionID, bool _Broadcast, Device _Device) { }

        public virtual void SendDeviceListElement(uint _ActionID, bool _Broadcast, LocalDevice _Device) { }

        public delegate void OnReceiveRequestNetworkMapHandler();
        public virtual event OnReceiveRequestNetworkMapHandler OnReceiveRequestNetworkMap;

        //the connection should add the device to the device list
        public delegate void OnReceiveConnectionHandler(ushort _NetworkNodeA,ushort _NetworkNodeB);
        public virtual event OnReceiveConnectionHandler OnReceiveConnection;

        //the connection should add the device to the device list
        public delegate void OnReceiveDeviceListElementHandler(RemoteDevice _Device);
        public virtual event OnReceiveDeviceListElementHandler OnReceiveDeviceListElement;

        //the connection should add the network node to the nodelist
        public delegate void OnReceiveNetworkNodeHandler(RemoteNetworkNode _NetworkNode);
        public virtual event OnReceiveNetworkNodeHandler OnReceiveNetworkNode;

        //the connection itself should copy the new memorybin in the right device
        public delegate void OnReceiveDeviceMemoryBinHandler(Device _Device);
        public virtual event OnReceiveDeviceMemoryBinHandler OnReceiveDeviceMemoryBin;
    }
}
