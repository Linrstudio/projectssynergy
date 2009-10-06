using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynergyNode
{
    public class RemoteNetworkNode
    {
        public static Dictionary<ushort,RemoteDevice> LocalDevices;
        public static Dictionary<ushort, RemoteNetworkNode> RemoteNodes;
    }
}
