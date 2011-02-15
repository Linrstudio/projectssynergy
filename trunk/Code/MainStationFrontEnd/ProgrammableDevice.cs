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
    public interface ProgrammableDevice
    {
        string Name { get; set; }
        Sequence Sequence { get; set; }
        SequenceManager Manager { get; set; }

        TreeNode GetTreeNode();
        ContextMenu GetContextMenu();

        void Save(XElement _Data);
        void Load(XElement _Data);
    }
}
