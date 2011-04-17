using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SynergySequence
{
    public partial class SequenceEditorForm : Form
    {
        Sequence Sequence = null;
        public SequenceEditorForm(Sequence _Sequence)
        {
            Sequence = _Sequence;
            InitializeComponent();
        }

        private void EventEditor_Load(object sender, EventArgs e)
        {
            if (Sequence != null) window.SetSequence(Sequence);
            window.OnBlockSelect += new SequenceEditWindow.OnBlockSelectHandler(window_OnBlockSelect);

            Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();
            if (Sequence != null && Sequence.Manager != null)
            {
                foreach (SequenceManager.Prototype p in Sequence.Manager.Prototypes)
                {
                    if (p.UserCanAdd)
                    {
                        if (!groups.ContainsKey(p.GroupName))
                        {
                            ListViewGroup group = new ListViewGroup(p.GroupName, HorizontalAlignment.Left);
                            l_CodeBlocks.Groups.Add(group);
                            groups.Add(p.GroupName, group);
                        }
                        ListViewItem item = new ListViewItem(p.Name, groups[p.GroupName]);
                        item.Tag = p;
                        l_CodeBlocks.Items.Add(item);
                    }
                }
            }
        }

        void window_OnBlockSelect(CodeBlock _SelectedBlock)
        {
            p_Properties.SelectedObject = _SelectedBlock;
        }



        private void l_CodeBlocks_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(new object[] { ((ListViewItem)e.Item).Tag }, DragDropEffects.All);
        }

        private void p_Properties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            window.Do();
        }
    }
}
