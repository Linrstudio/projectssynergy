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
    public partial class AddDevice : Form
    {
        public AddDevice()
        {
            InitializeComponent();
        }

        private void AddDevice_Load(object sender, EventArgs e)
        {
            UpdateTree();
            InputChanged();
        }

        public void UpdateTree()
        {
            t_Devices.Nodes.Clear();
            foreach (ProductDataBase.Device d in ProductDataBase.Devices)
            {
                if (d.ID == 0) continue;//main station

                TreeNode node = new TreeNode(d.Name);
                node.Tag = d;
                t_Devices.Nodes.Add(node);
            }
        }

        private void t_Devices_AfterSelect(object sender, TreeViewEventArgs e)
        {
            InputChanged();
            if (e.Node == null) return;
            ProductDataBase.Device device = (ProductDataBase.Device)e.Node.Tag;
            l_Description.Text = device.Description;
        }

        private void n_ID_ValueChanged(object sender, EventArgs e)
        {
            InputChanged();
        }

        public bool CanAdd()
        {
            if (t_Devices.SelectedNode == null) return false;
            foreach (EEPROM.Device d in EEPROM.Devices.Values)
            {
                if (d.ID == n_ID.Value) return false;
                if (d.Name == t_Name.Text) return false;
            }
            if (t_Name.Text != System.Text.RegularExpressions.Regex.Match(t_Name.Text, "[-_a-zA-Z0-9\\s]*").Value) return false;

            return true;
        }

        private void b_Ok_Click(object sender, EventArgs e)
        {
            if (!CanAdd()) return;

            EEPROM.RegisterDevice(t_Name.Text, (ProductDataBase.Device)(t_Devices.SelectedNode.Tag), (ushort)n_ID.Value);
            Close();
        }

        void InputChanged()
        {
            bool found = false;
            foreach (EEPROM.Device d in EEPROM.Devices.Values)
            {
                if (d.ID == n_ID.Value) found = true;
            }
            n_ID.BackColor = found ? Color.Red : SystemColors.Window;

            if (t_Name.Text == System.Text.RegularExpressions.Regex.Match(t_Name.Text, "[-_a-zA-Z0-9\\s]*").Value)
                t_Name.BackColor = SystemColors.Window;
            else t_Name.BackColor = Color.Red;

            b_Ok.Enabled = CanAdd();
        }

        private void t_Name_TextChanged(object sender, EventArgs e)
        {
            InputChanged();
        }
    }
}
