using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BaseFrontEnd
{
    public partial class EventEditForm : ChildForm
    {
        public EEPROM.Device.Event Event;

        public EventEditForm(EEPROM.Device.Event _Event)
        {
            Content = _Event;
            InitializeComponent();
            Event = _Event;
            DoubleBuffered = true;

            s_WorkingArea.Sequence = Event.sequence;
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
    }
}
