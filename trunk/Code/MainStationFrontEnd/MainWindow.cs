using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utilities;

namespace MainStationFrontEnd
{
    public partial class MainWindow : Form
    {
        public static MainWindow mainwindow;

        public MainWindow()
        {
            mainwindow = this;
            InitializeComponent();
            MainStation.ProductDataBase.Load("products.xml");
            MainStation.MainStation.Connect();
            Solution.Load("Solution.xml");

            UpdateTree();
        }

        public enum Icons
        {
            Device,
            MainStationOnline,
            MainStationOffline,
            EventLocal,
            EventSelected,
            EventRemote,
            EventRemoteSelected,
            DeviceError,
            Computer,
            ComputerOnline,
            ComputerOffline,
            WebInterface
        };

        private void Form1_Load(object sender, EventArgs e)
        {
            //EEPROM.OnAssamble += new EEPROM.OnAssambleHandler(EEPROM_OnAssamble);
            //new SheduleWindow().ShowDialog();
        }

        bool TreeDirty = true;
        //if one wants to update the tree from a different thread
        public void ScheduleUpdateTree()
        {
            TreeDirty = true;
        }

        public void UpdateTree()
        {
            t_contents.Nodes.Clear();
            TreeNode Root = new TreeNode("Root");
            t_contents.Nodes.Add(Root);
            foreach (ProgrammableDevice pd in Solution.ProgrammableDevices)
            {
                Root.Nodes.Add(pd.GetTreeNode());
            }
            Root.Expand();
        }

        List<Form> OpenedWindows = new List<Form>();
        private void t_contents_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            OpenedWindows.Remove((Form)sender);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Solution.Load("Solution.xml");
            UpdateTree();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Solution.Save("Solution.xml");
        }

        public void ShowDialog(Form _Form)
        {
            foreach (Form f in OpenedWindows)
            {
                if (f.Tag == _Form.Tag)
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

            _Form.Show();
            if (!OpenedWindows.Contains(_Form))
                OpenedWindows.Add(_Form);

        }

        private void t_contents_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (t_contents.SelectedNode == null) return;
            if (t_contents.SelectedNode.Tag is Computer)
            {
                Computer p = (Computer)t_contents.SelectedNode.Tag;
                Form form = new EditComputerDialog(p);
                form.Tag = p;
                ShowDialog(form);
            }
            if (t_contents.SelectedNode.Tag is FrontEndMainStation)
            {
                FrontEndMainStation p = (FrontEndMainStation)t_contents.SelectedNode.Tag;
                Form form = new EditMainStationDialog(p);
                form.Tag = p;
                ShowDialog(form);
            }
            if (t_contents.SelectedNode.Tag is WebInterface.Scene)
            {
                WebInterface.Scene p = (WebInterface.Scene)t_contents.SelectedNode.Tag;
                Form form = new WebInterface.SceneEditorForm(p);
                form.Tag = p;
                ShowDialog(form);
            }
        }

        /// <summary>
        /// shows a context menu the shitty way
        /// </summary>
        private void t_contents_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = e.Location;
                TreeNode node = t_contents.GetNodeAt(p);
                if (node == null) return;
                if (node.Tag is ProgrammableDevice)
                {
                    ((ProgrammableDevice)node.Tag).GetContextMenu().Show(t_contents, new Point(e.X, e.Y));
                }
                if (node.Tag is FrontEndWebInterface)
                {
                    ((FrontEndWebInterface)node.Tag).GetContextMenu().Show(t_contents, new Point(e.X, e.Y));
                }
                if (node.Tag is MainStation.MainStationDevice)
                {
                    c_Device.Tag = node.Tag;
                    c_Device.Show(t_contents, new Point(e.X, e.Y));
                }
                if (node.Parent == null)//if root
                {
                    c_Root.Show(t_contents, e.X, e.Y);
                }
                //add more menu's
            }
        }

        void PollDevice(MainStation.MainStationDevice _Device)
        {
            bool found = !MainStation.MainStation.Poll(_Device.ID);
            if (_Device.Found != found)
            {
                _Device.Found = found;
                UpdateTree();
                //Utilities.Utilities.Log("Device {0} {1}", _Device.ID, found ? "located" : "is missing");
            }
        }
        int lastpolled = 0;
        bool connected = false;
        bool lastconnected = false;
        private void t_ConnectionCheck_Tick(object sender, EventArgs e)
        {
            foreach (ProgrammableDevice pd in Solution.ProgrammableDevices)
            {
                if (pd is Computer)
                {
                    ((Computer)pd).Update();
                }
            }

            if (MainStation.MainStation.Connected())
            {
                bool anymainstationfound = false;
                foreach (ProgrammableDevice pd in Solution.ProgrammableDevices)
                {
                    if (pd is MainStation.MainStation)
                    {
                        MainStation.MainStation ms = (MainStation.MainStation)pd;
                        if (ms.Devices.Length > 0)
                        {
                            MainStation.MainStationDevice d = ms.Devices[lastpolled % ms.Devices.Length];
                            PollDevice(d);
                            lastpolled++;
                        }
                        anymainstationfound = true;
                    }
                }
                if (!anymainstationfound)
                {
                    Solution.AddMainStation(new FrontEndMainStation());
                    MainWindow.mainwindow.UpdateTree();
                }
            }
            /*
            MainStation.MainStation.InvokeLocalEvent(0, 128, 0);
            System.Threading.Thread.Sleep(1000);
            MainStation.MainStation.InvokeLocalEvent(0, 129, 0);
            System.Threading.Thread.Sleep(1000);
            */
            if (!connected)
            {
                MainStation.MainStation.Connect();
            }
            connected = MainStation.MainStation.Connected();
            t_Connected.Text = connected ? "Connected" : "Disconnected";
            if (connected && !lastconnected)
            {
                MainStation.MainStation.TimeWrite();
                readtime();
                foreach (ProgrammableDevice pd in Solution.ProgrammableDevices)
                {
                    if (pd is MainStation.MainStation)
                    {
                        ((MainStation.MainStation)pd).Found = true;
                        foreach (MainStation.MainStationDevice d in ((MainStation.MainStation)pd).Devices)
                        {
                            PollDevice(d);
                        }
                    }
                }
                UpdateTree();
            }
            if (!connected && lastconnected)
            {
                foreach (ProgrammableDevice pd in Solution.ProgrammableDevices)
                {
                    if (pd is FrontEndMainStation)
                    {
                        ((FrontEndMainStation)pd).Found = false;
                        foreach (MainStation.MainStationDevice d in ((FrontEndMainStation)pd).Devices)
                        {
                            d.Found = false;
                        }
                    }
                }
                UpdateTree();
            }
            lastconnected = connected;
        }

        void readtime()
        {
            MainStation.MainStation.Time bla = MainStation.MainStation.TimeRead();
            t_time.Text = bla.DayTime.ToString() + "-" + bla.Day.ToString();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //if (!MainStation.EEPROMWriteVerify(EEPROM.Assamble())) MessageBox.Show("Verify incorrect");
            foreach (ProgrammableDevice pd in Solution.ProgrammableDevices)
            {
                if (pd is FrontEndMainStation)
                {
                    FrontEndMainStation ms = (FrontEndMainStation)pd;
                    byte[] EEPROM = MainStation.MainStationCompiler.Compile(ms);
                    MainStation.MainStation.EEPROMWriteVerify(EEPROM);
                    System.IO.File.WriteAllBytes("c:/newcompiler.bin", EEPROM);
                }
                if (pd is Computer)
                {
                    ((Computer)pd).Compile();
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            MainStation.MainStation.TimeWrite();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            readtime();
        }

        private void t_contents_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode node = (TreeNode)e.Item;
            if (node.Tag == null) return;
            /*
            if (node.Tag is MainStation.MainStationDevice)
            {
                MainStation.MainStationDevice device = (MainStation.MainStationDevice)node.Tag;
                //construct apropriate codeblock
                var block = new MainStationCodeBlocks.MainStationCodeBlockDevice();
                block.type = device.device.ID;
                block.DeviceID = device.ID;
                block.Create();
                t_contents.DoDragDrop(new object[] { block }, DragDropEffects.Copy);
            }
            else
            */
            if (node.Tag is MainStation.ProductDataBase.Device.Event)
            {
                if (node.Parent.Tag is MainStation.MainStationDevice)
                {
                    MainStation.MainStationDevice device = (MainStation.MainStationDevice)node.Parent.Tag;
                    MainStation.ProductDataBase.Device.Event evnt = (MainStation.ProductDataBase.Device.Event)node.Tag;
                    var block = new MainStationCodeBlocks.CodeBlockEvent();
                    block.DeviceType = device.device.ID;
                    block.EventID = evnt.ID;
                    block.DeviceID = device.ID;
                    block.Create();
                    t_contents.DoDragDrop(new object[] { block }, DragDropEffects.Copy);
                }
            }
            else if (node.Tag is MainStation.ProductDataBase.Device.RemoteEvent)
            {
                if (node.Parent.Tag is MainStation.MainStationDevice)
                {
                    MainStation.MainStationDevice device = (MainStation.MainStationDevice)node.Parent.Tag;
                    MainStation.ProductDataBase.Device.RemoteEvent evnt = (MainStation.ProductDataBase.Device.RemoteEvent)node.Tag;
                    var block = new MainStationCodeBlocks.CodeBlockInvokeRemoteEvent();
                    block.DeviceType = device.device.ID;
                    block.EventID = evnt.ID;
                    block.DeviceID = device.ID;
                    block.Create();
                    t_contents.DoDragDrop(new object[] { block }, DragDropEffects.Copy);
                }
            }
            else t_contents.DoDragDrop(node.Tag, DragDropEffects.Copy);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //invoke event at remote device
            /*
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
            }*/
            UpdateTree();
        }

        public void Log(string _Message)
        {
            t_Log.Text += "\r\n> " + _Message;
            t_Log.Select(t_Log.Text.Length - 1, 0);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

            UpdateTree();
        }

        private void addComputerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Computer newcomputer = new Computer();
            new EditComputerDialog(newcomputer).ShowDialog();
            Solution.AddComputer(newcomputer);
            UpdateTree();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (TreeDirty)
            {
                UpdateTree();
                TreeDirty = false;
            }
        }

        private void addMainStationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Solution.AddMainStation(new FrontEndMainStation());
            UpdateTree();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (c_Device.Tag is MainStation.MainStationDevice)
            {
                var v = (MainStation.MainStationDevice)c_Device.Tag;
                v.mainstation.RemoveDevice(v);
            }
            UpdateTree();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            MainStation.MainStation.SetTimer(0, 128, 10);
        }
    }
}
