using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SynergyNode;

namespace SynergyClient
{
    public partial class f_Main : Form
    {
        bool needsredraw = false;
        Dictionary<string, Scene> scenes;
        
        float SceneSize;
  
        public f_Main()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            worldview.Objects.Add(new NetworkDeviceAnalog(3000));
            worldview.Objects[0].X = 0.25f;
            worldview.Objects.Add(new NetworkDeviceDigital(2005));

            Resources.Load();
            SynergyNode.NetworkNode.OnDeviceFound += DeviceAdded;
            SynergyNode.NetworkNode.OnDeviceMemoryChanged += AnyThingChanged;
        }

        private void f_Main_Load(object sender, EventArgs e)
        {
            //this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            scenes = Scene.LoadScenes("Scenes");
            RebuildSceneList();
            l_Scenes.SelectedItems.Clear();
            l_Scenes.Items[1].Selected = true;
            tabControl1.SelectedIndex = 1;
            Resize();
            ConnectionList.Load("Connections.xml");
            foreach (ConnectionListItem i in ConnectionList.Items)
            {
                d_Connections.Rows.Add(i.IP, i.Port.ToString());
                try
                {
                    SynergyNode.NetworkNode.AddConnection(new SynergyNode.TCPConnection(i.IP, i.Port, true));
                }
                catch { }
                finally
                {
                    SynergyNode.NetworkNode.RequestNetworkMap();
                }
            }
        }

        public void RebuildSceneList()
        {
            l_Scenes.Items.Clear();
            foreach (Scene s in scenes.Values)
            {
                l_Scenes.Items.Add(s.Name);
            }
        }

        public void AnyThingChanged(SynergyNode.Device _Device)
        {
            needsredraw = true;
        }
        public void DeviceAdded(SynergyNode.Device _Device)
        {
            needsredraw = true;
        }

        public void UpdateDeviceList()
        {
            /*
            d_Devices.Rows.Clear();
            foreach (SynergyNode.Device d in SynergyNode.NetworkNode.RemoteDevices.Values)
            {
                d_Devices.Rows.Add(d.ID, d.DeviceType, d.Memory.GetState());
            }
            */
        }

        private void panel2_Resize(object sender, EventArgs e)
        {
            Resize();
        }

        public void Resize()
        {
            p_Graphic.Width = p_Graphic.Height = Math.Min(p_Container.Width, p_Container.Height);
            p_Graphic.Left = (p_Container.Width - p_Graphic.Width) / 2;
            p_Graphic.Top = (p_Container.Height - p_Graphic.Height) / 2;
            SceneSize = p_Graphic.Width;
            if (SceneSize < 1) SceneSize = 1;
        }

        public void ReDraw()
        {
            p_Graphic.Invalidate();
        }

        private void b_update_Click(object sender, EventArgs e)
        {
            UpdateDeviceList();
        }

        private void b_Save_Click(object sender, EventArgs e)
        {
            if (p_Graphic.scene != null)
                p_Graphic.scene.Save();
        }
        
        public void Save(string _Path)
        {
            
        }

        private void f_Main_FormClosed(object sender, FormClosedEventArgs e)
        {


            ConnectionList.Save("Connections.xml");
            
            
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                try
                {
                    string ip = (string)d_Connections.Rows[e.RowIndex].Cells[0].Value;
                    int port = int.Parse((string)d_Connections.Rows[e.RowIndex].Cells[1].Value);
                    try
                    {
                        SynergyNode.NetworkNode.Connections.Add(new SynergyNode.TCPConnection(new System.Net.Sockets.TcpClient(ip, port), false));
                    }
                    catch { MessageBox.Show("Could not connect."); }
                    finally
                    {
                        SynergyNode.NetworkNode.RequestNetworkMap();
                    }
                }
                catch { MessageBox.Show("Error in syntax"); }
            }
        }

        private void t_refresh_Tick(object sender, EventArgs e)
        {
            if (needsredraw) { ReDraw(); needsredraw = false; }
            //worldView1.Update();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void l_Scenes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                p_Graphic.scene = scenes[l_Scenes.SelectedItems[0].Text];
                ReDraw();
            }
            catch { }
        }

        private void b_Whois_Click(object sender, EventArgs e)
        {
            SynergyNode.NetworkNode.RemoteDevices.Clear();
            SynergyNode.NetworkNode.RequestNetworkMap();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ((DigitalMemoryBin)NetworkNode.RemoteDevices[1001].Memory).On = true;
            NetworkNode.RemoteDevices[1001].UpdateRemoteMemory();
        }
    }
}
