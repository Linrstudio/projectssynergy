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
                node.ImageIndex = 0;
                node.SelectedImageIndex = 0;
                node.ToolTipText = d.device.Description;
                node.Tag = d;
                foreach (EEPROM.Device.Event e in d.Events.Values)
                {
                    TreeNode enode = new TreeNode(e.eventtype.Name);
                    enode.ContextMenuStrip = c_TreeEvent;
                    enode.Tag = e;
                    enode.ImageIndex = 1;
                    enode.SelectedImageIndex = 2;
                    enode.ToolTipText = e.eventtype.Description;
                    node.Nodes.Add(enode);
                }
                node.ImageIndex = 0;
                t_contents.Nodes.Add(node);
            }
            t_contents.ExpandAll();
        }

        List<ChildForm> OpenedWindows = new List<ChildForm>();
        private void t_contents_MouseDoubleClick(object sender, MouseEventArgs e)
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

        void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            OpenedWindows.Remove((ChildForm)sender);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            EEPROM.Save("EEPROM.xml");
            UpdateTree();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            EEPROM.FromFile("EEPROM.xml");
            UpdateTree();
        }
    }
}
