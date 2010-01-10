using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SynergyTemplate;

namespace Framework
{
    public class NetworkClassMaster : NetworkClass
    {
        public NetworkClassMaster(string _Name, string _TypeName)
            : base(_Name, _TypeName) { }

        public void InvokeSlaveMethod(string _FunctionName, params object[] _Parameters)
        {
            Connection.SendToAll("ExecuteSlaveCommand", Name, GetInvokeCommand(_FunctionName, _Parameters));
        }

        public void SetSlaveField(string _FieldName, object _Value)
        {
            Connection.SendToAll("ExecuteSlaveCommand", Name, GetSetFieldCommand(_FieldName, _Value));
        }

        public void UpdateSlaveField(string _FieldName)
        {
            SetSlaveField(_FieldName, GetField(_FieldName));
        }

        public object GetField(string _Name)
        {
            FieldInfo info = GetNetworkFieldInfo(_Name);
            if (info == null) return null;
            return GetNetworkFieldInfo(_Name).GetValue(this);
        }
    }
}
