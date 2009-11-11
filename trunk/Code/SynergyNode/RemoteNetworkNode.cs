using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynergyNode
{
    public class RemoteNetworkNode
    {
        public ushort NetworkNodeID;

        public Dictionary<ushort, RemoteDevice> LocalDevices;
        public Dictionary<ushort, RemoteNetworkNode> RemoteNodes;

        public void MakeCall(string _FunctionName, params object[] _Arguments)
        {

        }

        public RemoteNetworkNode(ushort _NodeID)
        {
            NetworkNodeID = _NodeID;
            LocalDevices = new Dictionary<ushort, RemoteDevice>();
            RemoteNodes = new Dictionary<ushort, RemoteNetworkNode>();
        }
    }
}
