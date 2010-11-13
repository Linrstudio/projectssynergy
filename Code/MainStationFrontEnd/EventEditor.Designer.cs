namespace MainStationFrontEnd
{
    partial class EventEditor
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
            this.p_Properties = new System.Windows.Forms.PropertyGrid();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.p_workspace = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // p_Properties
            // 
            this.p_Properties.Dock = System.Windows.Forms.DockStyle.Right;
            this.p_Properties.Location = new System.Drawing.Point(449, 0);
            this.p_Properties.Name = "p_Properties";
            this.p_Properties.Size = new System.Drawing.Size(185, 247);
            this.p_Properties.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(445, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(4, 247);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // p_workspace
            // 
            this.p_workspace.AutoScroll = true;
            this.p_workspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.p_workspace.Location = new System.Drawing.Point(0, 0);
            this.p_workspace.Name = "p_workspace";
            this.p_workspace.Size = new System.Drawing.Size(445, 247);
            this.p_workspace.TabIndex = 2;
            // 
            // EventEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 247);
            this.Controls.Add(this.p_workspace);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.p_Properties);
            this.Name = "EventEditor";
            this.Text = "EventEditor";
            this.Load += new System.EventHandler(this.EventEditor_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid p_Properties;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel p_workspace;
    }
}