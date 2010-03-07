using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BaseFrontEnd
{
    public partial class MainWindow : Form
    {
        List<ChildForm> OpenedWindows = new List<ChildForm>();
        Base Base = null;
        public MainWindow(Base _Base)
        {
            Base = _Base;
            InitializeComponent();
        }

        public void AddChildForm(ChildForm _Form)
        {
            _Form.MdiParent = this;
            _Form.FormClosing += new FormClosingEventHandler(OnChildFormClosed);
            _Form.Show();
            OpenedWindows.Add(_Form);
        }

        void OnChildFormClosed(object sender, FormClosingEventArgs e)
        {
            foreach (ChildForm c in OpenedWindows)
            {
                if (c.Content == ((ChildForm)sender).Content)
                {
                    OpenedWindows.Remove(c);
                    return;
                }
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            
            toolStrip1.Top = 0;
            toolStrip2.Top = 0;
            ProductDataBase.Load(@"products.xml");
            UpdateTree();
            UpdateStatusStrip();
            Base.eeprom.OnAssamble += UpdateStatusStrip;
        }

        public void UpdateStatusStrip()
        {
            toolStripProgressBar1.Maximum = Base.eeprom.Size;
            toolStripProgressBar1.Value = Base.eeprom.BytesUsed;

            toolStripStatusLabel2.Text = (((float)Base.eeprom.BytesUsed / (float)Base.eeprom.Size) * 100.0f).ToString("F") + "%";
        }

        public void UpdateTree()
        {
            t_contents.Nodes.Clear();
            if (Base.eeprom == null) return;
            foreach (EEPROM.Device d in Base.eeprom.Devices.Values)
            {
                TreeNode node = new TreeNode(string.Format("{0}({1})",d.device.Name,d.device.ID));
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

        TreeNode t_contentsContextMenuNode = null;
        private void invokeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (t_contentsContextMenuNode != null)
            {
                if (t_contentsContextMenuNode.Tag is EEPROM.Device.Event)
                {
                    EEPROM.Device.Event evnt = (EEPROM.Device.Event)t_contentsContextMenuNode.Tag;
                    Base.ExecuteRemoteEvent(evnt.device.ID, evnt.eventtype.ID, 1);// the 1 here is hardcoded
                }
            }
        }

        private void t_contents_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = e.Location;
                TreeNode node = t_contents.GetNodeAt(p);
                if (node != null && node.Tag is EEPROM.Device.Event)
                {
                    t_contentsContextMenuNode = node;
                    c_TreeEvent.Show(t_contents, e.X, e.Y);
                }
            }
        }

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
                ChildForm form = new EventEditForm((EEPROM.Device.Event)t_contents.SelectedNode.Tag);

                AddChildForm(form);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Base.UploadEEPROM();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            f_AddDevice d = new f_AddDevice();
            d.ShowDialog();
            Base.eeprom.RegisterDevice(d.SelectedDevice, d.SelectedDeviceID);
            UpdateTree();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Base.eeprom = EEPROM.FromFile("test.eeprom");
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Base.eeprom.Save("test.eeprom");
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Base.SetTime(DateTime.Now);
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            t_Time.Text = Base.ReadTime();
        }
    }
}
