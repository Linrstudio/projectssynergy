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
        public static MainWindow mainwindow;
        public MainWindow()
        {
            mainwindow = this;
            InitializeComponent();
            ProductDataBase.Load("products.xml");
            MainStation.Connect();
            EEPROM.FromFile("EEPROM.xml");
            foreach (EEPROM.Device d in EEPROM.Devices.Values)
            {
                PollDevice(d);
            }
            UpdateTree();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EEPROM.OnAssamble += new EEPROM.OnAssambleHandler(EEPROM_OnAssamble);
            //new SheduleWindow().ShowDialog();
            UpdateSchedule();
            c_day.SelectionStart = DateTime.Now;
        }

        public void UpdateSchedule()
        {
            List<DateTime> dates = new List<DateTime>();
            foreach (EEPROM.ScheduleEntry entry in EEPROM.ScheduleEntries)
            {
                dates.Add(entry.Moment);
            }
            c_day.BoldedDates = dates.ToArray();
        }

        void EEPROM_OnAssamble()
        {
            p_progress.Maximum = EEPROM.Size;
            p_progress.Value = EEPROM.BytesUsed;
            t_progress.Text = string.Format("{0:0.000}%", ((float)EEPROM.BytesUsed / EEPROM.Size) * 100.0f);
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
                node.Tag = d;
                if (d.ID == 0)
                {
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                }
                else
                {
                    if (d.Found)
                    {
                        node.ImageIndex = 0;
                        node.SelectedImageIndex = 0;
                    }
                    else
                    {
                        node.ImageIndex = 6;
                        node.SelectedImageIndex = 6;
                    }
                }
                node.ToolTipText = d.device.Description;
                //node.Tag = d;
                foreach (EEPROM.Device.Event e in d.Events.Values)
                {
                    if (e.eventtype == null) continue;
                    TreeNode enode = new TreeNode(e.Name);
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

        public void ShowDialog(ChildForm _Form)
        {
            foreach (ChildForm f in OpenedWindows)
            {
                if (f.Content == _Form.Content)
                {
                    try
                    {
                        f.BringToFront();// sometimes the window randomly gets disposed
                        return;
                    }
                    catch { OpenedWindows.Remove(f); }
                    break;
                }
            }

            _Form.FormClosed += new FormClosedEventHandler(form_FormClosed);
            _Form.MdiParent = this;
            try
            {
                _Form.Show();
                if (!OpenedWindows.Contains(_Form))
                    OpenedWindows.Add(_Form);
            }
            catch { }
        }

        private void t_contents_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (t_contents.SelectedNode == null) return;
            if (t_contents.SelectedNode.Tag is EEPROM.Device.Event)
            {
                ChildForm form = new EventEditor(((EEPROM.Device.Event)t_contents.SelectedNode.Tag).sequence);
                ShowDialog(form);
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
                {
                    new InvokeRemoteEventWindow(evnt, device.ID).Show();
                    //MainStation.InvokeRemoteEvent(device.ID, evnt.ID, 2);
                }
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
                if (node != null && node.Tag is EEPROM.Device)
                {
                    t_contentsContextMenuNode = node;
                    c_treedevice.Show(t_contents, e.X, e.Y);
                }
                else if (node != null && node.Tag != null)
                {
                    t_contentsContextMenuNode = node;
                    c_TreeEvent.Show(t_contents, e.X, e.Y);
                }
            }
        }

        bool lastconnected = false;
        int lastpolled = 0;

        void PollDevice(EEPROM.Device _Device)
        {
            bool found = !MainStation.Poll(_Device.ID);
            if (_Device.Found != found)
            {
                _Device.Found = found;
                UpdateTree();
                Utilities.Log("Device {0} {1}", _Device.ID, found ? "located" : "is missing");
            }

        }

        private void t_ConnectionCheck_Tick(object sender, EventArgs e)
        {
            bool connected = MainStation.Connected();
            if (connected)
            {
                lastpolled = (lastpolled + 1) % EEPROM.Devices.Count;
                int i = 0;
                foreach (EEPROM.Device d in EEPROM.Devices.Values)
                {
                    if (i == lastpolled && d.ID != 0)
                    {
                        PollDevice(d);
                        break;
                    }
                    i++;
                }
                lastpolled++;
            }

            t_Connected.Text = connected ? "Connected" : "Disconnected";
            if (connected && !lastconnected)
            {
                MainStation.TimeWrite();
                readtime();
            }
            lastconnected = connected;
        }

        void readtime()
        {
            MainStation.Time bla = MainStation.TimeRead();
            t_time.Text = bla.DayTime.ToString() + "-" + bla.Day.ToString();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (!MainStation.EEPROMWriteVerify(EEPROM.Assamble())) MessageBox.Show("Verify incorrect");
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            MainStation.TimeWrite();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            readtime();
        }

        private void t_contents_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode node = (TreeNode)e.Item;
            if (node.Tag == null) return;
            t_contents.DoDragDrop(node.Tag, DragDropEffects.Link);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //invoke event at remote device
            if (t_contentsContextMenuNode.Tag is object[])
            {
                object[] tag = (object[])t_contentsContextMenuNode.Tag;

                EEPROM.Device device = (EEPROM.Device)tag[0];
                ProductDataBase.Device.RemoteEvent evnt = (ProductDataBase.Device.RemoteEvent)tag[1];
                if (MainStation.Connected())
                {
                    new InvokeRemoteEventWindow(evnt, device.ID).Show();
                    //MainStation.InvokeRemoteEvent(device.ID, evnt.ID, 2);
                }
            }
            //invoke event at mainstation
            if (t_contentsContextMenuNode.Tag is EEPROM.Device.Event)
            {
                EEPROM.Device.Event evnt = (EEPROM.Device.Event)t_contentsContextMenuNode.Tag;

                RenameDialog dialog = new RenameDialog(evnt.Name);
                switch (dialog.ShowDialog())
                {
                    case DialogResult.Yes:
                        evnt.Name = dialog.NewName;
                        break;
                    case DialogResult.No:
                        evnt.Name = evnt.DefaultName;
                        break;
                }
            }
            UpdateTree();
        }

        private void c_day_DateChanged(object sender, DateRangeEventArgs e)
        {
            s_hours.SelectedDay = Utilities.GetDay(c_day.SelectionStart);
        }

        public void Log(string _Message)
        {
            t_Log.Text += "\r\n> " + _Message;
            t_Log.Select(t_Log.Text.Length - 1, 0);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EEPROM.Devices.Remove(
            ((EEPROM.Device)t_contentsContextMenuNode.Tag).ID);
            UpdateTree();
        }
    }
}
