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
    public partial class ScheduleEventEditor : ChildForm
    {
        KismetSequence Sequence = null;
        public ScheduleEventEditor(KismetSequence _Sequence)
        {
            Sequence = _Sequence;
            Content = Sequence;
            InitializeComponent();
        }

        public ScheduleEventEditor()
        {
            InitializeComponent();
        }

        SequenceEditWindow window;
        private void EventEditor_Load(object sender, EventArgs e)
        {
            window = new SequenceEditWindow();
            p_workspace.Controls.Add(window);
            if (Sequence != null) window.Sequence = Sequence;
            window.OnBlockSelect += new SequenceEditWindow.OnBlockSelectHandler(window_OnBlockSelect);
        }

        void window_OnBlockSelect(CodeBlock _SelectedBlock)
        {
            p_Properties.SelectedObject = _SelectedBlock;
        }
    }
}
