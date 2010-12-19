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
    public partial class SequenceEditorForm:Form
    {
        Sequence Sequence = null;
        public SequenceEditorForm(Sequence _Sequence)
        {
            Sequence = _Sequence;
            InitializeComponent();
        }

        public SequenceEditorForm()
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
            
            Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();
            foreach (SynergySequence.Prototype p in SynergySequence.Prototypes)
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

        void window_OnBlockSelect(CodeBlock _SelectedBlock)
        {
            p_Properties.SelectedObject = _SelectedBlock;
        }

        private void l_CodeBlocks_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(new object[]{((ListViewItem)e.Item).Tag}, DragDropEffects.All);
        }
    }
}
