using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BaseFrontEnd
{
    public partial class KismetEditor : Form
    {
        public EEPROM eeprom;

        public KismetEditor(EEPROM _EEPROM)
        {
            InitializeComponent();
            eeprom = _EEPROM;
            UpdateTree();
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.EnableNotifyMessage, true);
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            s_WorkingArea.OnBlockSelect += new SequenceEditWindow.OnBlockSelectHandler(s_WorkingArea_OnBlockSelect);
        }

        void s_WorkingArea_OnBlockSelect(CodeBlock _SelectedBlock)
        {
            PropertyGrid.SelectedObject = _SelectedBlock;
        }

        protected override void OnNotifyMessage(Message m)
        {
            if (m.Msg != 0x14)
                base.OnNotifyMessage(m);
        }

        public void UpdateTree()
        {
            t_Contents.Nodes.Clear();
            foreach (EEPROM.Device d in eeprom.Devices.Values)
            {
                TreeNode node = new TreeNode(d.ID.ToString());
                node.Tag = d;
                foreach (EEPROM.Device.Event e in d.Events.Values)
                {
                    TreeNode enode = new TreeNode(e.ID.ToString());
                    enode.Tag = e;
                    node.Nodes.Add(enode);
                }
                t_Contents.Nodes.Add(node);
            }
            t_Contents.ExpandAll();
        }

        private void KismetEditor_Load(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.EnableNotifyMessage, true);
            s_WorkingArea.Format();
        }

        private void KismetEditor_ResizeEnd(object sender, EventArgs e)
        {
            s_WorkingArea.Format();
        }

        private void t_Contents_AfterSelect(object sender, TreeViewEventArgs e)
        {
            s_WorkingArea.Sequence = null;
            if (e.Node.Tag.GetType() == typeof(EEPROM.Device.Event))
            {
                s_WorkingArea.Sequence = ((EEPROM.Device.Event)e.Node.Tag).sequence;
            }
            s_WorkingArea.Format();
        }

        private void PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            s_WorkingArea.Format();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            f_AddEvent addevent = new f_AddEvent();
            addevent.ShowDialog();

            if (!eeprom.Devices.ContainsKey(addevent.DeviceID))
            {
                eeprom.Devices.Add(addevent.DeviceID, new EEPROM.Device(eeprom, addevent.DeviceID));
            }
            if (!eeprom.Devices[addevent.DeviceID].Events.ContainsKey(addevent.EventID))
            {
                eeprom.Devices[addevent.DeviceID].Events.Add(addevent.EventID, new EEPROM.Device.Event(eeprom.Devices[addevent.DeviceID], addevent.EventID));
            }
            s_WorkingArea.Sequence = eeprom.Devices[addevent.DeviceID].Events[addevent.EventID].sequence;
            UpdateTree();
        }
    }
}
