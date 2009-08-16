using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynergyNode
{
    public static class ConnectionManager
    {
        public static List<Connection> Connections;
        public static Queue<Packet> ReceiveQueue;
        public static Queue<Packet> SendQueue;

        public static void Update()
        {
            Packet p = SendQueue.Dequeue();
            foreach (Connection c in Connections) c.SendPacket(p);
            foreach (Connection c in Connections) c.Update();
        }
    }
}
