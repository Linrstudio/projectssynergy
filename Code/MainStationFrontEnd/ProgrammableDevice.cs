using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
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

        public abstract TreeNode GetTreeNode();
        public abstract ContextMenu GetContextMenu();

        public abstract void Save(XElement _Data);
        public abstract void Load(XElement _Data);
    }
}
