using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace MainStationFrontEnd
{
    public partial class InvokeRemoteEventWindow : Form
    {
        ProductDataBase.Device.RemoteEvent remoteevent = null;
        ushort deviceid;
        [Browsable(true), CategoryAttribute("Constant")]
        List<object> Parameters = new List<object>();

        public InvokeRemoteEventWindow(ProductDataBase.Device.RemoteEvent _Event, ushort _DeviceID)
        {
            deviceid = _DeviceID;
            remoteevent = _Event;
            InitializeComponent();
            foreach (ProductDataBase.Device.RemoteEvent.Input input in _Event.Inputs)
            {
                d_Parameters.Rows.Add(input.Name, CodeBlock.GetDataType(input.Type).GetDefaultValue());
            }
        }

        private void InvokeEventWindow_Load(object sender, EventArgs e)
        {

        }

        private void b_Invoke_Click(object sender, EventArgs e)
        {
            MainStation.InvokeRemoteEvent(deviceid, remoteevent.ID, 2);
        }
    }
}
