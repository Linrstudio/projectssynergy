namespace MainStationFrontEnd
{
    partial class SequenceEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (base.IsDisposed) return;
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.p_workspace = new System.Windows.Forms.Panel();
            this.p_Properties = new System.Windows.Forms.PropertyGrid();
            this.l_CodeBlocks = new System.Windows.Forms.ListView();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 355);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(676, 4);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // p_workspace
            // 
            this.p_workspace.AutoScroll = true;
            this.p_workspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.p_workspace.Location = new System.Drawing.Point(0, 0);
            this.p_workspace.Name = "p_workspace";
            this.p_workspace.Size = new System.Drawing.Size(676, 355);
            this.p_workspace.TabIndex = 2;
            // 
            // p_Properties
            // 
            this.p_Properties.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.p_Properties.Location = new System.Drawing.Point(0, 359);
            this.p_Properties.Name = "p_Properties";
            this.p_Properties.Size = new System.Drawing.Size(676, 126);
            this.p_Properties.TabIndex = 0;
            // 
            // l_CodeBlocks
            // 
            this.l_CodeBlocks.Dock = System.Windows.Forms.DockStyle.Right;
            this.l_CodeBlocks.Location = new System.Drawing.Point(679, 0);
            this.l_CodeBlocks.Name = "l_CodeBlocks";
            this.l_CodeBlocks.Size = new System.Drawing.Size(140, 485);
            this.l_CodeBlocks.TabIndex = 0;
            this.l_CodeBlocks.TileSize = new System.Drawing.Size(128, 30);
            this.l_CodeBlocks.UseCompatibleStateImageBehavior = false;
            this.l_CodeBlocks.View = System.Windows.Forms.View.Tile;
            this.l_CodeBlocks.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.l_CodeBlocks_ItemDrag);
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter2.Location = new System.Drawing.Point(676, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 485);
            this.splitter2.TabIndex = 3;
            this.splitter2.TabStop = false;
            // 
            // SequenceEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 485);
            this.Controls.Add(this.p_workspace);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.p_Properties);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.l_CodeBlocks);
            this.Name = "SequenceEditorForm";
            this.Text = "EventEditor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.EventEditor_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel p_workspace;
        private System.Windows.Forms.PropertyGrid p_Properties;
        private System.Windows.Forms.ListView l_CodeBlocks;
        private System.Windows.Forms.Splitter splitter2;
    }
}