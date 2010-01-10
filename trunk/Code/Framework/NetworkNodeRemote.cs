using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SynergyTemplate;

namespace Framework
{
    public class NetworkNodeRemote : NetworkNode
    {
        public Dictionary<string, NetworkClassSlave> LocalDevices = new Dictionary<string, NetworkClassSlave>();

        internal void AddNetworkClass(string _Name, string _TypeName)
        {
            if (LocalDevices.ContainsKey(_Name)) return;

            NetworkClassSlave inst = NetworkClassSlave.CreateFromType(_Name, _TypeName);
            if (inst != null) LocalDevices.Add(inst.Name, inst);
        }

        public override void Update()
        {
            foreach (NetworkClass c in LocalDevices.Values) c.Update();
            base.Update();
        }

        public NetworkNodeRemote(ushort _NodeID)
            : base(_NodeID)
        {
        }
    }
}
