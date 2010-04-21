using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SynergyTemplate;

namespace Framework
{
    public class NetworkClassMaster : NetworkClass
    {
        List<NetworkNodeRemote> SubscripedNodes = new List<NetworkNodeRemote>();
        public void AddSubscription(NetworkNodeRemote _Node)
        {
            System.Threading.Mutex mutex = new System.Threading.Mutex(true);
            if (!SubscripedNodes.Contains(_Node)) SubscripedNodes.Add(_Node);
            mutex.ReleaseMutex();
        }

        public void RemoveSubscription(NetworkNodeRemote _Node)
        {
            if (SubscripedNodes.Contains(_Node)) SubscripedNodes.Remove(_Node);
        }

        public NetworkClassMaster(string _Name, string _TypeName)
            : base(_Name, _TypeName) { }

        public void InvokeSlaveMethod(string _FunctionName, params object[] _Parameters)
        {
            foreach(NetworkNodeRemote node in SubscripedNodes)
                Connection.SendToAll("ExecuteSlaveCommand", Name, GetInvokeCommand(_FunctionName, _Parameters));
        }

        public void SetSlaveField(string _FieldName, object _Value)
        {
            Connection.SendToAll("ExecuteSlaveCommand", Name, GetSetFieldCommand(_FieldName, _Value));
        }

        public override void UpdateRemoteField(string _FieldName)
        {
            SetSlaveField(_FieldName, GetField(_FieldName));
        }

        public override object GetField(string _FieldName)
        {
            FieldInfo info = GetNetworkFieldInfo(_FieldName);
            if (info == null) return null;
            return GetNetworkFieldInfo(_FieldName).GetValue(this);
        }
    }
}
