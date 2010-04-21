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

        public NetworkNode[] FindPathTo(NetworkNode _Target)
        {
            return FindPathTo(_Target, 10);
        }

        public NetworkNode[] FindPathTo(NetworkNode _Target, int _MaxDepth)
        {
            if (this == _Target) return new NetworkNode[] { this };//we found him
            //otherwise we ask any connected nodes
            int bestlength = 1000000;
            NetworkNode[] best = null;
            foreach (NetworkNode node in RemoteNodes.Values)
            {
                NetworkNode[] results = node.FindPathTo(_Target, _MaxDepth - 1);
                if (results != null && results.Length < bestlength)
                {
                    bestlength = results.Length; best = results;
                }
            }
            if (best == null) return null;//we found nothing, return null

            List<NetworkNode> list = new List<NetworkNode>(best); list.Add(this);//add our self and return the list
            return list.ToArray();
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
