using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class NetworkNodeLocal : NetworkNode
    {
        public List<Connection> Connections = new List<Connection>();
        public Dictionary<string, NetworkClassLocal> NetworkClasses = new Dictionary<string, NetworkClassLocal>();

        public NetworkNodeLocal()
            : base((ushort)((NetworkManager.random.Next() % ushort.MaxValue - 1) + 1))
        {
        }

        public override void Update()
        {
            foreach (Connection c in Connections.ToArray()) c.Update();

            base.Update();
        }

        public void AddLocalNetworkClass(NetworkClassLocal _NetworkClass)
        {
            NetworkClasses.Add(_NetworkClass.Name, _NetworkClass);
        }
    }
}
