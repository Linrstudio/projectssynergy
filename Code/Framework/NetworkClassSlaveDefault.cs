using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using SynergyTemplate;

namespace Framework
{
    public class NetworkClassSlaveDefault:NetworkClassSlave
    {
        public NetworkClassSlaveDefault(string _Name, string _Type)
            : base(_Name, _Type)
        {

        }

        public NetworkClassSlaveDefault(string _Name)
            : base(_Name, typeof(NetworkClassSlaveDefault).FullName)
        {

        }
    }
}
