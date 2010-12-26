using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using SynergySequence;

namespace MainStationFrontEnd
{
    public abstract class ProgrammableDevice
    {
        public string Name;
        public Sequence Sequence = null;
        public SequenceManager Manager = null;

        public ProgrammableDevice()
        {
            Sequence = new Sequence();
        }

        public abstract XElement Save();
        public abstract void Load(XElement _Data);
    }
}
