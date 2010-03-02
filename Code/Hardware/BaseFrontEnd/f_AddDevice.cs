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
    public partial class f_AddDevice : Form
    {
        public ProductDataBase.Device SelectedDevice = null;
        public ushort SelectedDeviceID = 0;
        public f_AddDevice()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectedDevice = (ProductDataBase.Device)l_Devices.SelectedItems[0].Tag;
            SelectedDeviceID = (ushort)n_DeviceID.Value;
            Close();
        }

        private void f_AddDevice_Load(object sender, EventArgs e)
        {
            foreach (ProductDataBase.Device d in ProductDataBase.Devices)
            {
                ListViewItem i = new ListViewItem(d.Name);
                i.Tag = d;
                l_Devices.Items.Add(i);
            }
        }

        private void l_Devices_SelectedIndexChanged(object sender, EventArgs e)
        {
            l_Description.Text = ((ProductDataBase.Device)l_Devices.SelectedItems[0].Tag).Description;
        }
    }
}
