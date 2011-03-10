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
    public partial class EditMainStationDialog : Form
    {
        FrontEndMainStation mainstation;
        public EditMainStationDialog(FrontEndMainStation _MainStation)
        {
            mainstation = _MainStation;
            InitializeComponent();
        }

        private void EditMainStation_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form form = new SynergySequence.SequenceEditorForm(mainstation.Sequence);
            form.Tag = mainstation.Sequence;
            MainWindow.mainwindow.ShowDialog(form);
        }
    }
}
