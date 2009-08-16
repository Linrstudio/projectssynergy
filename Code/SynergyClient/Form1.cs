using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SynergyClient
{
    public partial class f_Main : Form
    {
        Scene CurrentScene = null;
        SceneDevice CurrentDevice = null;
        int DeviceOperation = 0;
        float SceneSize;
        public f_Main()
        {
            InitializeComponent();
        }

        private void f_Main_Load(object sender, EventArgs e)
        {
            Resize();
            CurrentScene = new Scene("Scenes/Scene.xml");
            ConnectionList.Load("Connections.xml");
            foreach (ConnectionListItem i in ConnectionList.Items)
            {
                d_Connections.Rows.Add(i.IP, i.Port.ToString());
                try 
                {
                    SynergyNode.ConnectionManager.Connections.Add(new SynergyNode.TCPConnection(new System.Net.Sockets.TcpClient(i.IP, i.Port),true));
                } catch { }
            }
        }

        public void UpdateDeviceList()
        {
            d_Devices.Rows.Clear();
            foreach( SynergyNode.Device d in SynergyNode.ConnectionManager.Devices.Values)
            {
                d_Devices.Rows.Add(d.ID, d.DeviceType, System.Text.Encoding.ASCII.GetString(d.Memory));
            }
        }

        private void panel2_Resize(object sender, EventArgs e)
        {
            Resize();
        }
        public void Resize()
        {
            p_Graphic.Top = 0;
            p_Graphic.Width = p_Graphic.Height = Math.Min(p_Container.Width, p_Container.Height);
            p_Graphic.Left = (p_Container.Width - p_Graphic.Width) / 2;
            p_Graphic.Top = (p_Container.Height - p_Graphic.Height) / 2;
            SceneSize = p_Graphic.Width;
            if (SceneSize < 1) SceneSize = 1;
        }

        private void p_Graphic_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;

                if (CurrentScene != null)
                {
                    g.DrawImage(CurrentScene.BackgroundImage, new RectangleF(0, 0, p_Graphic.Width, p_Graphic.Height));
                    foreach (SceneDevice d in CurrentScene.Devices)
                    {
                       d.OnDraw(g, SceneSize);
                        /*
                        Font font = new Font("Arial", size * 0.2f, FontStyle.Bold);
                        g.FillEllipse(Brushes.Red, new RectangleF((d.X * SceneSize) - size * 0.5f, (float)(d.Y * SceneSize) - size * 0.5f, size, size));
                        g.DrawString(d.Name, font, Brushes.Black, d.X * SceneSize, d.Y * SceneSize, format);
                        */
                    }
                }
            }
            catch { }
        }

        public void ReDraw()
        {
            Refresh();
        }
        private void p_Graphic_MouseClick(object sender, MouseEventArgs e)
        {
            if (DeviceOperation != 0)
                DeviceOperation = 0;
            else
            {
                SceneDevice d = CurrentScene.GetDevice((float)e.X / SceneSize, (float)e.Y / SceneSize);
                if (d != null)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        d.OnClick();
                        ReDraw();
                    }
                    if (e.Button == MouseButtons.Right)
                    {
                        CurrentDevice = d;
                        c_Menu.Show(p_Graphic, e.Location);
                    }
                }
            }
        }

        private void b_update_Click(object sender, EventArgs e)
        {
            UpdateDeviceList();
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeviceOperation = 1;
        }

        private void resizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeviceOperation = 2;
        }

        private void p_Graphic_MouseMove(object sender, MouseEventArgs e)
        {
            float X = (float)e.X / SceneSize;
            float Y = (float)e.Y / SceneSize;
            switch (DeviceOperation)
            {
                case 1:
                    CurrentDevice.X = X;
                    CurrentDevice.Y = Y;
                    ReDraw();
                    break;
                case 2:
                    {
                        float dx = CurrentDevice.X - X;
                        float dy = CurrentDevice.Y - Y;
                        CurrentDevice.Size = (float)Math.Sqrt(dx * dx + dy * dy) * 2;
                        ReDraw();
                    }
                    break;
            }
        }

        private void p_Graphic_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void b_Save_Click(object sender, EventArgs e)
        {
            CurrentScene.Save("Scenes/Scene.xml");
        }
        public void Save(string _Path)
        {
            
        }

        private void f_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
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
                        SynergyNode.ConnectionManager.Connections.Add(new SynergyNode.TCPConnection(new System.Net.Sockets.TcpClient(ip, port), true));
                    }
                    catch { MessageBox.Show("Could not connect."); }
                    finally
                    {
                        SynergyNode.ConnectionManager.Whois();
                    }
                }
                catch { MessageBox.Show("Error in syntax"); }
            }
        }
    }
}
