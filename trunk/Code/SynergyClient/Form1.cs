using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SynergyClient
{
    public partial class f_Main : Form
    {
        bool needsredraw = false;
        Dictionary<string, Scene> scenes;
        
        float SceneSize;
  
        public f_Main()
        {
            InitializeComponent();
            Resources.Load();
            SynergyNode.ConnectionManager.OnDeviceFound += DeviceAdded;
            SynergyNode.ConnectionManager.OnDeviceMemoryChanged += AnyThingChanged;
        }

        private void f_Main_Load(object sender, EventArgs e)
        {
            //this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            scenes = Scene.LoadScenes("Scenes");
            
            RebuildSceneList();
            
            Resize();
            ConnectionList.Load("Connections.xml");
            foreach (ConnectionListItem i in ConnectionList.Items)
            {
                d_Connections.Rows.Add(i.IP, i.Port.ToString());
                try
                {
                    SynergyNode.ConnectionManager.Connections.Add(new SynergyNode.TCPConnection(i.IP, i.Port, true));
                }
                catch { }
                finally
                {
                    SynergyNode.ConnectionManager.RequestDeviceList();
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
            d_Devices.Rows.Clear();
            foreach (SynergyNode.Device d in SynergyNode.ConnectionManager.RemoteDevices.Values)
            {
                d_Devices.Rows.Add(d.ID, d.DeviceType, d.Memory.GetState());
            }
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
                        SynergyNode.ConnectionManager.Connections.Add(new SynergyNode.TCPConnection(new System.Net.Sockets.TcpClient(ip, port), false));
                    }
                    catch { MessageBox.Show("Could not connect."); }
                    finally
                    {
                        SynergyNode.ConnectionManager.RequestDeviceList();
                    }
                }
                catch { MessageBox.Show("Error in syntax"); }
            }
        }

        private void t_refresh_Tick(object sender, EventArgs e)
        {
            if (needsredraw) { ReDraw(); needsredraw = false; }
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
            SynergyNode.ConnectionManager.RemoteDevices.Clear();
            SynergyNode.ConnectionManager.RequestDeviceList();
        }
    }
}
