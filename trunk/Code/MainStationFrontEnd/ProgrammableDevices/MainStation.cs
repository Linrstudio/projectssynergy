using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using SynergySequence;
using MainStation;
using MainStationCodeBlocks;

namespace MainStationFrontEnd
{
    public class FrontEndMainStation : MainStation.MainStation, ProgrammableDevice
    {
        public bool Found;

        string name;
        public string Name { get { return name; } set { name = value; } }

        public Sequence Sequence { get { return base.Sequence; } set { base.Sequence = value; } }
        public SequenceManager Manager { get { return base.Manager; } set { base.Manager = value; } }

        public FrontEndMainStation()
            : base()
        {
            Manager = new MainStationSequenceManager();
            MainStationCodeBlock.AddAllPrototypes(Manager);

            Manager.AddDataType(new SequenceManager.DataType("int", System.Drawing.Color.Blue));
            Manager.AddDataType(new SequenceManager.DataType("bool", System.Drawing.Color.Green));

            Sequence = new Sequence(Manager);
        }


        public System.Windows.Forms.TreeNode GetTreeNode()
        {
            int icon = (int)(Found ? MainWindow.Icons.MainStationOnline : MainWindow.Icons.MainStationOffline);
            TreeNode node = new TreeNode("mainstation", icon, icon);
            node.Tag = this;
            foreach (MainStationDevice d in Devices)
            {
                node.Nodes.Add(GetTreeNode(d));
                node.Expand();
            }
            return node;
        }

        public System.Windows.Forms.TreeNode GetTreeNode(MainStation.MainStationDevice _Device)
        {
            int icon = (int)(_Device.Found ? MainWindow.Icons.Device : MainWindow.Icons.DeviceError);
            var node = new System.Windows.Forms.TreeNode(_Device.Name + "(" + _Device.ID + ")", icon, icon);
            node.Tag = _Device;

            foreach (ProductDataBase.Device.Event e in _Device.device.events)
            {
                int eicon = (int)MainWindow.Icons.EventLocal;
                TreeNode enode = new TreeNode(e.Name, eicon, eicon);
                enode.Tag = e;
                node.Nodes.Add(enode);
            }

            foreach (ProductDataBase.Device.RemoteEvent e in _Device.device.remoteevents)
            {
                int eicon = (int)MainWindow.Icons.EventRemote;
                TreeNode enode = new TreeNode(e.Name,eicon,eicon);
                enode.Tag = e;
                node.Nodes.Add(enode);
            }

            return node;
        }

        public System.Windows.Forms.ContextMenu GetContextMenu()
        {
            var menu = new System.Windows.Forms.ContextMenu();
            menu.MenuItems.Add("Remove", new EventHandler(ContextMenuRemove));
            menu.MenuItems.Add("Register Device", new EventHandler(ContextMenuAddDevice));
            return menu;
        }

        void ContextMenuAddDevice(object sender, EventArgs e)
        {
            new AddDevice(this).ShowDialog();
            MainWindow.mainwindow.UpdateTree();
        }

        void ContextMenuRemove(object sender, EventArgs e)
        {
            Solution.RemoveSystem(this);
        }
    }
}
