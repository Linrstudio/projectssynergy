using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SynergyTemplate;

namespace Framework
{
    public class NetworkNode
    {
        internal ushort nodeid;
        public ushort NodeID { get { return nodeid; } }

        public Dictionary<ushort, NetworkNodeRemote> RemoteNodes = new Dictionary<ushort, NetworkNodeRemote>();

        public NetworkNode(ushort _NodeID)
        {
            nodeid = _NodeID;
        }

        public virtual void Update()
        {

        }

        public override string ToString()
        {
            return string.Format("NodeID:{0}", nodeid);
        }
    }
}
