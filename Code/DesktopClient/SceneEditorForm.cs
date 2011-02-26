using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WebInterface
{
    public partial class SceneEditorForm : Form
    {
        Scene scene;
        public SceneEditorForm(Scene _Scene)
        {
            scene = _Scene;
            InitializeComponent();
            sceneEditor1.SetScene(scene);
        }

        private void SceneEditorForm_Load(object sender, EventArgs e)
        {
            sceneEditor1.OnSelectedItemChanged += new SceneEditor.SelectedItemChanged(sceneEditor1_OnSelectedItemChanged);
        }

        void sceneEditor1_OnSelectedItemChanged(Control _NewItem)
        {
            propertyGrid1.SelectedObject = _NewItem;
        }
    }
}
