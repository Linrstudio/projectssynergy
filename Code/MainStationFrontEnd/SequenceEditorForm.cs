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
    public partial class SequenceEditorForm : ChildForm
    {
        KismetSequence Sequence = null;
        public SequenceEditorForm(KismetSequence _Sequence)
        {
            Sequence = _Sequence;
            Content = Sequence;
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
            CodeBlock.Initialize();
            Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();
            foreach (CodeBlock.Prototype p in CodeBlock.CodeBlocks)
            {
                if (p.UserCanAdd)
                {
                    if (!groups.ContainsKey(p.GroupName))
                    {
                        ListViewGroup group = new ListViewGroup(p.GroupName, HorizontalAlignment.Left);
                        l_CodeBlocks.Groups.Add(group);
                        groups.Add(p.GroupName, group);
                    }
                    ListViewItem item = new ListViewItem(p.BlockName, groups[p.GroupName]);
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
