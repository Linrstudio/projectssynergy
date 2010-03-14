using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SynergyTemplate;

namespace Framework
{
    public class NetworkNodeLocal : NetworkNode
    {
        public List<Connection> Connections = new List<Connection>();
        public Dictionary<string, NetworkClassMaster> NetworkClasses = new Dictionary<string, NetworkClassMaster>();

        public NetworkNodeLocal()
            : base(Environment.MachineName + "-" + Environment.UserName)
        {
        }

        public override void Update()
        {
            Log.Write(new Log.Variable("LocalNode", "ID", NodeID));
            foreach (Connection c in Connections.ToArray()) c.Update();
            foreach (NetworkClassMaster c in NetworkClasses.Values) c.Update();

            base.Update();
        }

        public void AddNetworkClass(NetworkClassMaster _NetworkClass)
        {
            NetworkClasses.Add(_NetworkClass.Name, _NetworkClass);
            Log.Write(new Log.Variable("LocalNode", "Network Classes", NetworkClasses.Count));
        }
    }
}
