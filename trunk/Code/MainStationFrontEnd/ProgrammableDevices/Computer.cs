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

        List<MainStation.MainStation> mainstations = new List<MainStation.MainStation>();
        public MainStation.MainStation[] MainStations { get { return mainstations.ToArray(); } }

        List<FrontEndWebInterface> webinterfaces = new List<FrontEndWebInterface>();
        public FrontEndWebInterface[] WebInterfaces { get { return webinterfaces.ToArray(); } }

        public IPAddress IPAddress;
        public ushort Port;
        LazyNetworking.TCPConnection Connection;
        public Computer()
            : base()
        {
            Manager = new ComputerSequenceManager();

            DesktopCodeBlock.AddAllPrototypes(Manager);

            Manager.AddDataType(new SequenceManager.DataType("int", System.Drawing.Color.Blue));
            Manager.AddDataType(new SequenceManager.DataType("bool", System.Drawing.Color.Green));

            Sequence = new Sequence(Manager);
        }

        public void Update()
        {
            string read = Connection.Read();
            if (read != null)
            {
                try
                {
                    string[] split = read.Split(' ');
                    if (split[0] == "mainstation")
                    {
                        if (split[1] == "found")
                        {
                            foreach (FrontEndMainStation m in mainstations)
                            {
                                m.Found = true;
                            }
                            MainWindow.mainwindow.UpdateTree();
                        }
                        if (split[1] == "notfound")
                        {
                            foreach (FrontEndMainStation m in mainstations)
                            {
                                m.Found = false;
                            }
                            MainWindow.mainwindow.UpdateTree();
                        }
                    }
                }
                catch { }
            }
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
            foreach (WebInterface.WebInterface i in webinterfaces)
            {
                node.Nodes.Add(GetTreeNode(i));
            }
            return node;
        }

        public TreeNode GetTreeNode(WebInterface.WebInterface _WebInterface)
        {
            int icon = (int)MainWindow.Icons.WebInterface;
            TreeNode node = new TreeNode("Web interface", icon, icon);
            node.Tag = _WebInterface;
            foreach (Scene s in _WebInterface.scenes)
            {
                node.Nodes.Add(GetTreeNode(s));
            }
            return node;
        }

        public TreeNode GetTreeNode(WebInterface.Scene _Scene)
        {
            int icon = (int)MainWindow.Icons.WebInterface;
            TreeNode node = new TreeNode(_Scene.Name, icon, icon);
            node.Tag = _Scene;

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
            MainWindow.mainwindow.UpdateTree();
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

            foreach (XElement element in _Data.Elements("WebInterface"))
            {
                FrontEndWebInterface wi = new FrontEndWebInterface(element);
                webinterfaces.Add(wi);
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

            foreach (WebInterface.WebInterface wi in webinterfaces)
            {
                XElement element = new XElement("WebInterface");
                wi.Save(element);
                _Data.Add(element);
            }
        }

        public void Compile()
        {
            XElement file = new XElement("project");
            file.Add(Sequence.Save());

            foreach (FrontEndMainStation ms in mainstations)
            {
                XElement mainstation = new XElement("MainStation");
                ms.Save(mainstation);
                file.Add(mainstation);
            }

            foreach (WebInterface.WebInterface wi in webinterfaces)
            {
                XElement element = new XElement("WebInterface");
                wi.Save(element);
                file.Add(element);
            }
            /*
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
             * 
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
            file.Add(webinterface);*/
            try
            {
                if (Connection == null) Connection = new LazyNetworking.TCPConnection(new System.Net.Sockets.TcpClient(IPAddress.ToString(), Port));
                Connection.Write("project " + file.ToString());
            }
            catch { MessageBox.Show("Failed to connect to desktopclient"); }
            file.Save("c:/sequence.xml");
        }
    }

    public class FrontEndWebInterface : WebInterface.WebInterface
    {
        public FrontEndWebInterface(XElement _Data) : base(_Data) { }

        public ContextMenu GetContextMenu()
        {
            ContextMenu menu = new ContextMenu();
            menu.MenuItems.Add("Add Scene", new EventHandler(ContextMenuAddScene));
            return menu;
        }

        public void ContextMenuAddScene(object sender, EventArgs e)
        {
            scenes.Add(new Scene("New scene"));
            MainWindow.mainwindow.UpdateTree();
        }
    }
}
