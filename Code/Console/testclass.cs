using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace SynergyConsole
{
    class testclass : NetworkClassLocal
    {
        public testclass()
            : base("testclass")
        {
        }

        [Framework.NetworkField("myfield")]
        public int myint;

        [Framework.NetworkMethod("mymethod")]
        public void jkhdfkljdf()
        {
            System.Console.WriteLine("myfield = {0}", myint);
        }
    }
}
