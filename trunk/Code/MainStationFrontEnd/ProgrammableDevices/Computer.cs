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
        string name;
        public string Name { get { return name; } set { name = value; } }

        Sequence sequence;
        public Sequence Sequence { get { return sequence; } set { sequence = value; } }

        SequenceManager manager;
        public SequenceManager Manager { get { return manager; } set { manager = value; } }

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

        public void Reconnect()
        {
            if (Connection != null) Connection.Kill();
            Connection = new LazyNetworking.TCPConnection(IPAddress, Port);
            Connection.OnStateChange += new LazyNetworking.TCPConnection.StateChange(Connection_OnStateChange);
        }

        void Connection_OnStateChange(bool _Connected)
        {
            MainWindow.mainwindow.ScheduleUpdateTree();
        }

        public TreeNode GetTreeNode()
        {
            int icon = (int)(Connected() ? MainWindow.Icons.ComputerOnline : MainWindow.Icons.ComputerOffline);
            TreeNode node = new TreeNode(Name, icon, icon);
            node.Tag = this;
            foreach (FrontEndMainStation m in MainStations)
            {
                node.Nodes.Add(m.GetTreeNode());
            }
            return node;
        }

        public bool Connected()
        {
            return Connection != null && Connection.Alive;
        }

        public ContextMenu GetContextMenu()
        {
            ContextMenu menu = new ContextMenu();
            menu.MenuItems.Add("Edit", new EventHandler(ContextMenuEdit));
            menu.MenuItems.Add("Add MainStation", new EventHandler(ContextMenuAddMainStation));
            return menu;
        }

        public void ContextMenuEdit(object sender, EventArgs e)
        {
            new EditComputerDialog(this).ShowDialog();
            Reconnect();
        }

        public void ContextMenuAddMainStation(object sender, EventArgs e)
        {
            mainstations.Add(new FrontEndMainStation());
        }

        public void Load(System.Xml.Linq.XElement _Data)
        {
            Sequence.Load(_Data.Element("Sequence"));
            Name = _Data.Attribute("Name").Value;
            try
            {
                IPAddress = IPAddress.Parse(_Data.Attribute("IPAddress").Value);
                Port = ushort.Parse(_Data.Attribute("Port").Value);
                Reconnect();
            }
            catch { }

            foreach (XElement element in _Data.Elements("MainStation"))
            {
                FrontEndMainStation ms = new FrontEndMainStation();
                ms.Load(element);
                mainstations.Add(ms);
            }
        }

        public void Save(System.Xml.Linq.XElement _Data)
        {
            _Data.SetAttributeValue("Name", Name);
            _Data.SetAttributeValue("IPAddress", IPAddress);
            _Data.SetAttributeValue("Port", Port);
            _Data.Add(Sequence.Save());

            foreach (MainStation.MainStation mainstation in mainstations)
            {
                XElement element = new XElement("MainStation");
                mainstation.Save(element);
                _Data.Add(element);
            }

        }

        List<MainStation.MainStation> mainstations = new List<MainStation.MainStation>();
        public MainStation.MainStation[] MainStations { get { return mainstations.ToArray(); } }

        public void Compile()
        {
            XElement file = new XElement("project");
            file.Add(Sequence.Save());

            XElement webinterface = new XElement("WebInterface");
            webinterface.SetAttributeValue("Port", "8080");
            XElement scene = new XElement("Scene");
            scene.SetAttributeValue("Name", "MyScene");
            {
                XElement control = new XElement("Control");
                control.SetAttributeValue("Type", "Switch");
                control.SetAttributeValue("Name", "lampkamer");
                control.SetAttributeValue("X", "0.5");
                control.SetAttributeValue("Y", "0.7");
                control.SetAttributeValue("Width", "0.2");
                control.SetAttributeValue("Height", "0.2");
                scene.Add(control);
            }
            {
                XElement control = new XElement("Control");
                control.SetAttributeValue("Type", "Switch");
                control.SetAttributeValue("Name", "lampgang");
                control.SetAttributeValue("X", "0.5");
                control.SetAttributeValue("Y", "0.2");
                control.SetAttributeValue("Width", "0.2");
                control.SetAttributeValue("Height", "0.2");
                scene.Add(control);
            }
            {
                XElement control = new XElement("Control");
                control.SetAttributeValue("Type", "Switch");
                control.SetAttributeValue("Name", "wcd1");
                control.SetAttributeValue("X", "0.9");
                control.SetAttributeValue("Y", "0.25");
                control.SetAttributeValue("Width", "0.1");
                control.SetAttributeValue("Height", "0.1");
                scene.Add(control);
            }
            {
                XElement control = new XElement("Control");
                control.SetAttributeValue("Type", "Switch");
                control.SetAttributeValue("Name", "wcd2");
                control.SetAttributeValue("X", "0.9");
                control.SetAttributeValue("Y", "0.75");
                control.SetAttributeValue("Width", "0.1");
                control.SetAttributeValue("Height", "0.1");
                scene.Add(control);
            }
            webinterface.Add(scene);
            file.Add(webinterface);
            try
            {
                if (Connection == null) Connection = new LazyNetworking.TCPConnection(new System.Net.Sockets.TcpClient(IPAddress.ToString(), Port));
                Connection.Write("project " + file.ToString());
            }
            catch { MessageBox.Show("Failed to connect to desktopclient"); }
            file.Save("c:/sequence.xml");
        }
    }
}
