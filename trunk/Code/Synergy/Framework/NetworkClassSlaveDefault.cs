using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Synergy
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
