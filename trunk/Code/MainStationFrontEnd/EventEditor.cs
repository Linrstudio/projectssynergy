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
    public partial class EventEditor : ChildForm
    {
        EEPROM.Device.Event Event = null;
        public EventEditor(EEPROM.Device.Event _Event)
        {
            Event = _Event;
            Content = _Event;
            InitializeComponent();
        }

        public EventEditor()
        {
            InitializeComponent();
        }

        SequenceEditWindow window;
        private void EventEditor_Load(object sender, EventArgs e)
        {
            window = new SequenceEditWindow();
            Controls.Add(window);
            if (Event != null && Event.sequence != null)
            window.Sequence = Event.sequence;
        }
    }
}
