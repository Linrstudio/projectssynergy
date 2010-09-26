using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MainStationFrontEnd
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            ProductDataBase.Load("products.xml");
            MainStation.Connect();
            EEPROM.FromFile("EEPROM.xml");
            UpdateTree();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            AddDevice win = new AddDevice();
            win.ShowDialog();
            UpdateTree();
        }

        public void UpdateTree()
        {
            t_contents.Nodes.Clear();
            foreach (EEPROM.Device d in EEPROM.Devices.Values)
            {
                TreeNode node = new TreeNode(string.Format("{0}({1})", d.Name, d.device.Name));
                if (d.ID == 0)
                {
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                }
                else
                {
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
                }
                node.ToolTipText = d.device.Description;
                //node.Tag = d;
                foreach (EEPROM.Device.Event e in d.Events.Values)
                {
                    TreeNode enode = new TreeNode(e.eventtype.Name);
                    enode.Tag = e;
                    enode.ImageIndex = 2;
                    enode.SelectedImageIndex = 3;
                    enode.ToolTipText = e.eventtype.Description;
                    node.Nodes.Add(enode);
                }
                foreach (ProductDataBase.Device.RemoteEvent e in d.device.remoteevents)
                {
                    TreeNode enode = new TreeNode(e.Name);
                    enode.Tag = new object[] { d, e };
                    enode.ImageIndex = 4;
                    enode.SelectedImageIndex = 5;
                    enode.ToolTipText = e.Description;
                    node.Nodes.Add(enode);
                }
                t_contents.Nodes.Add(node);
            }
            t_contents.ExpandAll();
        }

        List<ChildForm> OpenedWindows = new List<ChildForm>();
        private void t_contents_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            OpenedWindows.Remove((ChildForm)sender);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            EEPROM.FromFile("EEPROM.xml");
            UpdateTree();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            EEPROM.Save("EEPROM.xml");
            UpdateTree();
        }

        private void t_contents_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (t_contents.SelectedNode.Tag is EEPROM.Device.Event)
            {
                foreach (ChildForm f in OpenedWindows)
                {
                    if (f.Content == t_contents.SelectedNode.Tag)
                    {
                        f.BringToFront();
                        //f.Focus();
                        return;
                    }
                }
                ChildForm form = new EventEditor((EEPROM.Device.Event)t_contents.SelectedNode.Tag);
                form.FormClosed += new FormClosedEventHandler(form_FormClosed);
                form.MdiParent = this;
                form.Show();
                OpenedWindows.Add(form);
            }
        }

        TreeNode t_contentsContextMenuNode = null;
        private void b_Invoke_Click(object sender, EventArgs e)
        {
            //invoke event at remote device
            if (t_contentsContextMenuNode.Tag is object[])
            {
                object[] tag = (object[])t_contentsContextMenuNode.Tag;

                EEPROM.Device device = (EEPROM.Device)tag[0];
                ProductDataBase.Device.RemoteEvent evnt = (ProductDataBase.Device.RemoteEvent)tag[1];
                if (MainStation.Connected())
                    MainStation.InvokeRemoteEvent(device.ID, evnt.ID, 0);
            }
            //invoke event at mainstation
            if (t_contentsContextMenuNode.Tag is EEPROM.Device.Event)
            {
                EEPROM.Device.Event device = (EEPROM.Device.Event)t_contentsContextMenuNode.Tag;
                ushort deviceid = device.device.ID;
                byte eventid = device.eventtype.ID;

                if (MainStation.Connected())
                    MainStation.InvokeLocalEvent(deviceid, eventid, 0);
            }
        }

        /// <summary>
        /// showing a context menu the shitty way
        /// </summary>
        private void t_contents_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = e.Location;
                TreeNode node = t_contents.GetNodeAt(p);
                if (node != null && node.Tag != null)
                {
                    t_contentsContextMenuNode = node;
                    c_TreeEvent.Show(t_contents, e.X, e.Y);
                }
            }
        }
    }
}
