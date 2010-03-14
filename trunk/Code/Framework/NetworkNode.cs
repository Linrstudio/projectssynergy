using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SynergyTemplate;

namespace Framework
{
    public class NetworkNode
    {
        internal string nodeid;
        public string NodeID { get { return nodeid; } }

        public Dictionary<string, NetworkNodeRemote> RemoteNodes = new Dictionary<string, NetworkNodeRemote>();

        public NetworkNode(string _NodeID)
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
