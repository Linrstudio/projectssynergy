using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MainStationFrontEnd
{
    public abstract class ProgrammableDevice
    {
        public string Name;
        public KismetSequence Sequence = null;

        public abstract XElement Save();
        public abstract void Load(XElement _Data);
    }
}
