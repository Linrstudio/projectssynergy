using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;


namespace Synergy
{
    public class NetworkNodeRemote : NetworkNode
    {
        public Dictionary<string, NetworkClassSlave> LocalDevices = new Dictionary<string, NetworkClassSlave>();

        internal void AddNetworkClass(string _Name, string _TypeName)
        {
            if (LocalDevices.ContainsKey(_Name))
            {
                Log.Write("default", Log.Line.Type.Warning, "Could not add class, node already contains A class with this name");
                return;
            }

            NetworkClassSlave inst = NetworkClassSlave.CreateFromType(_Name, _TypeName);
            if (inst != null) LocalDevices.Add(inst.Name, inst);
        }

        internal void Send(string _FunctionName, params string[] _Parameters)
        {
            Connection.SendToAll(_FunctionName, _Parameters);
        }

        public override void Update()
        {
            foreach (NetworkClass c in LocalDevices.Values) c.Update();
            base.Update();
        }

        public NetworkNodeRemote(string _NodeID)
            : base(_NodeID)
        {
        }
    }
}
