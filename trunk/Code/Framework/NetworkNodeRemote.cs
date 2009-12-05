using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class NetworkNodeRemote : NetworkNode
    {
        public ushort NetworkNodeID;

        public Dictionary<string, NetworkClassRemote> LocalDevices = new Dictionary<string, NetworkClassRemote>();

        public ushort GetNodeID() { return NetworkNodeID; }

        public NetworkNodeRemote(ushort _NodeID)
            : base(_NodeID)
        {
            NetworkNodeID = _NodeID;
        }
    }
}
