namespace SynergySequence
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
            this.p_Properties = new System.Windows.Forms.PropertyGrid();
            this.l_CodeBlocks = new System.Windows.Forms.ListView();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.window = new SynergySequence.SequenceEditWindow();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 272);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(166, 4);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // p_Properties
            // 
            this.p_Properties.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.p_Properties.Location = new System.Drawing.Point(0, 276);
            this.p_Properties.Name = "p_Properties";
            this.p_Properties.Size = new System.Drawing.Size(166, 209);
            this.p_Properties.TabIndex = 0;
            this.p_Properties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.p_Properties_PropertyValueChanged);
            // 
            // l_CodeBlocks
            // 
            this.l_CodeBlocks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.l_CodeBlocks.Location = new System.Drawing.Point(0, 0);
            this.l_CodeBlocks.Name = "l_CodeBlocks";
            this.l_CodeBlocks.Size = new System.Drawing.Size(166, 272);
            this.l_CodeBlocks.TabIndex = 0;
            this.l_CodeBlocks.TileSize = new System.Drawing.Size(128, 30);
            this.l_CodeBlocks.UseCompatibleStateImageBehavior = false;
            this.l_CodeBlocks.View = System.Windows.Forms.View.Tile;
            this.l_CodeBlocks.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.l_CodeBlocks_ItemDrag);
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter2.Location = new System.Drawing.Point(650, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 485);
            this.splitter2.TabIndex = 3;
            this.splitter2.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.l_CodeBlocks);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.p_Properties);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(653, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(166, 485);
            this.panel1.TabIndex = 5;
            // 
            // window
            // 
            this.window.AllowDrop = true;
            this.window.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(233)))), ((int)(((byte)(245)))));
            this.window.Dock = System.Windows.Forms.DockStyle.Fill;
            this.window.Location = new System.Drawing.Point(0, 0);
            this.window.Name = "window";
            this.window.Size = new System.Drawing.Size(650, 485);
            this.window.TabIndex = 4;
            // 
            // SequenceEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 485);
            this.Controls.Add(this.window);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.panel1);
            this.Name = "SequenceEditorForm";
            this.Text = "EventEditor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.EventEditor_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.PropertyGrid p_Properties;
        private System.Windows.Forms.ListView l_CodeBlocks;
        private System.Windows.Forms.Splitter splitter2;
        private SequenceEditWindow window;
        private System.Windows.Forms.Panel panel1;
    }
}