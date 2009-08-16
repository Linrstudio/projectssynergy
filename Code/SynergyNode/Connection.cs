using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynergyNode
{
    public class Connection
    {
        public virtual void Update(){}
        public virtual void SendPacket(Packet _Packet) { }
    }
}
