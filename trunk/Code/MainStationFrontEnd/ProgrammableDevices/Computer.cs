using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Net;
using SynergySequence;
using DesktopCodeBlocks;
using WebInterface;

namespace MainStationFrontEnd
{
    public class ComputerSequenceManager : SequenceManager
    {
        public override CodeBlock CreateCodeBlock(Prototype _Prototype)
        {
            return (CodeBlock)Activator.CreateInstance(_Prototype.BlockType);
        }
    }

    public class Computer : ProgrammableDevice
    {
        public IPAddress IPAddress;
        public ushort Port;
        LazyNetworking.TCPConnection Connection;
        public Computer()
            : base()
        {
            Manager = new ComputerSequenceManager();

            DesktopCodeBlock.AddAllPrototypes(Manager);
            WebInterfaceCodeBlocks.AddAllPrototypes(Manager);
            K8055.K8055CodeBlocks.AddAllPrototypes(Manager);

            Manager.AddDataType(new SequenceManager.DataType("int", System.Drawing.Color.Blue));
            Manager.AddDataType(new SequenceManager.DataType("bool", System.Drawing.Color.Green));

            Sequence = new Sequence(Manager);
        }

        public override TreeNode GetTreeNode()
        {
            TreeNode node = new TreeNode(Name);
            node.Tag = this;
            foreach (MainStation m in MainStations)
            {
                node.Nodes.Add(m.GetTreeNode());
            }
            return node;
        }

        public override void Load(System.Xml.Linq.XElement _Data)
        {
            Sequence.Load(_Data.Element("Sequence"));
            Name = _Data.Attribute("Name").Value;
            try
            {
                IPAddress = IPAddress.Parse(_Data.Attribute("IPAddress").Value);
                Port = ushort.Parse(_Data.Attribute("Port").Value);
            }
            catch { }

            foreach (XElement element in _Data.Elements("MainStation"))
            {
                MainStation ms = new MainStation();
                ms.Load(element);
                mainstations.Add(ms);
            }

            Connection=new LazyNetworking.TCPConnection(new System.Net.Sockets.TcpClient(IPAddress.ToString(),Port));
        }

        public override void Save(System.Xml.Linq.XElement _Data)
        {
            _Data.SetAttributeValue("Name", Name);
            _Data.SetAttributeValue("IPAddress", IPAddress);
            _Data.SetAttributeValue("Port", Port);
            _Data.Add(Sequence.Save());
            foreach (MainStation mainstation in mainstations)
            {
                XElement element = new XElement("MainStation");
                mainstation.Save(element);
                _Data.Add(element);
            }

        }

        List<MainStation> mainstations = new List<MainStation>();
        public MainStation[] MainStations { get { return mainstations.ToArray(); } }

        public void Compile()
        {
            XElement file = new XElement("project");
            file.Add(Sequence.Save());

            XElement webinterface =new XElement("WebInterface");
            webinterface.SetAttributeValue("Port", "8080");
            XElement scene = new XElement("Scene");
            scene.SetAttributeValue("Name", "MyScene");
            XElement control = new XElement("Control");
            control.SetAttributeValue("Type", "Switch");
            control.SetAttributeValue("Name", "MySwitch1");
            control.SetAttributeValue("X", "0.5");
            control.SetAttributeValue("Y", "0.5");
            control.SetAttributeValue("Width", "0.1");
            control.SetAttributeValue("Height", "0.1");

            scene.Add(control);
            webinterface.Add(scene);
            file.Add(webinterface);

            Connection.Write("project " + file.ToString());
            file.Save("c:/sequence.xml");
        }
    }
}
