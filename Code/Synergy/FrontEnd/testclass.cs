using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using SynergyTemplate;

namespace K8055
{
    public class testclass : NetworkClassSlave
    {
        public testclass(string _Name, string _TypeName)
            : base(_Name, _TypeName)
        {
            Log.Write("K8055", "slave reporting");
        }

        [NetworkClass.Method("testfunction")]
        public void ihateyou()
        {
            Log.Write("K8055", "slave bob");
        }

        public int myint;
    }
}
