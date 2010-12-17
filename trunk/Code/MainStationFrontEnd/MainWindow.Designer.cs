namespace MainStationFrontEnd
{
    partial class MainWindow
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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.p_progress = new System.Windows.Forms.ToolStripProgressBar();
            this.t_progress = new System.Windows.Forms.ToolStripStatusLabel();
            this.t_Connected = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.t_time = new System.Windows.Forms.ToolStripLabel();
            this.i_contents = new System.Windows.Forms.ImageList(this.components);
            this.c_TreeEvent = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.b_Invoke = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.t_ConnectionCheck = new System.Windows.Forms.Timer(this.components);
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.t_Log = new System.Windows.Forms.TextBox();
            this.c_treedevice = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.t_contents = new System.Windows.Forms.TreeView();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.c_TreeEvent.SuspendLayout();
            this.c_treedevice.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.p_progress,
            this.t_progress,
            this.t_Connected});
            this.statusStrip1.Location = new System.Drawing.Point(0, 494);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(850, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(87, 17);
            this.toolStripStatusLabel1.Text = "Memory Usage";
            // 
            // p_progress
            // 
            this.p_progress.Name = "p_progress";
            this.p_progress.Size = new System.Drawing.Size(300, 16);
            // 
            // t_progress
            // 
            this.t_progress.Name = "t_progress";
            this.t_progress.Size = new System.Drawing.Size(23, 17);
            this.t_progress.Text = "0%";
            // 
            // t_Connected
            // 
            this.t_Connected.Name = "t_Connected";
            this.t_Connected.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton4,
            this.toolStripButton5,
            this.toolStripSeparator2,
            this.toolStripButton6,
            this.toolStripSeparator1,
            this.toolStripButton1,
            this.toolStripSeparator3,
            this.toolStripButton2,
            this.toolStripButton3,
            this.t_time});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(850, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton4.Text = "toolStripButton2";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton5.Text = "toolStripButton4";
            this.toolStripButton5.Click += new System.EventHandler(this.toolStripButton5_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
            this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton6.Text = "Register Device";
            this.toolStripButton6.Click += new System.EventHandler(this.toolStripButton6_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Upload to Mainstation";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "Sync Time";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "Get Time";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // t_time
            // 
            this.t_time.Name = "t_time";
            this.t_time.Size = new System.Drawing.Size(12, 22);
            this.t_time.Text = "-";
            // 
            // i_contents
            // 
            this.i_contents.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("i_contents.ImageStream")));
            this.i_contents.TransparentColor = System.Drawing.Color.Transparent;
            this.i_contents.Images.SetKeyName(0, "Device.png");
            this.i_contents.Images.SetKeyName(1, "mainstation.png");
            this.i_contents.Images.SetKeyName(2, "EventLocal.png");
            this.i_contents.Images.SetKeyName(3, "EventLocalSelected.png");
            this.i_contents.Images.SetKeyName(4, "EventRemote.png");
            this.i_contents.Images.SetKeyName(5, "EventRemoteSelected.png");
            this.i_contents.Images.SetKeyName(6, "DeviceError.png");
            // 
            // c_TreeEvent
            // 
            this.c_TreeEvent.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.b_Invoke,
            this.renameToolStripMenuItem});
            this.c_TreeEvent.Name = "c_TreeEvent";
            this.c_TreeEvent.Size = new System.Drawing.Size(118, 48);
            // 
            // b_Invoke
            // 
            this.b_Invoke.Name = "b_Invoke";
            this.b_Invoke.Size = new System.Drawing.Size(117, 22);
            this.b_Invoke.Text = "Invoke";
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(226, 25);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 469);
            this.splitter1.TabIndex = 6;
            this.splitter1.TabStop = false;
            // 
            // t_ConnectionCheck
            // 
            this.t_ConnectionCheck.Enabled = true;
            this.t_ConnectionCheck.Interval = 500;
            this.t_ConnectionCheck.Tick += new System.EventHandler(this.t_ConnectionCheck_Tick);
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter2.Location = new System.Drawing.Point(848, 25);
            this.splitter2.MinExtra = 0;
            this.splitter2.MinSize = 0;
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(2, 469);
            this.splitter2.TabIndex = 12;
            this.splitter2.TabStop = false;
            // 
            // t_Log
            // 
            this.t_Log.Dock = System.Windows.Forms.DockStyle.Right;
            this.t_Log.Location = new System.Drawing.Point(850, 25);
            this.t_Log.Multiline = true;
            this.t_Log.Name = "t_Log";
            this.t_Log.Size = new System.Drawing.Size(0, 469);
            this.t_Log.TabIndex = 13;
            this.t_Log.Text = "> Hi there, Im surprised you found this";
            // 
            // c_treedevice
            // 
            this.c_treedevice.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.c_treedevice.Name = "c_TreeEvent";
            this.c_treedevice.Size = new System.Drawing.Size(118, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            this.toolStripMenuItem1.Text = "Remove";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // t_contents
            // 
            this.t_contents.Dock = System.Windows.Forms.DockStyle.Left;
            this.t_contents.ImageIndex = 0;
            this.t_contents.ImageList = this.i_contents;
            this.t_contents.Location = new System.Drawing.Point(0, 25);
            this.t_contents.Name = "t_contents";
            this.t_contents.SelectedImageIndex = 0;
            this.t_contents.ShowNodeToolTips = true;
            this.t_contents.Size = new System.Drawing.Size(226, 469);
            this.t_contents.TabIndex = 5;
            this.t_contents.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.t_contents_NodeMouseDoubleClick);
            this.t_contents.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.t_contents_MouseDoubleClick);
            this.t_contents.MouseUp += new System.Windows.Forms.MouseEventHandler(this.t_contents_MouseUp);
            this.t_contents.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.t_contents_ItemDrag);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 516);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.t_Log);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.t_contents);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.IsMdiContainer = true;
            this.Name = "MainWindow";
            this.Text = "FrontEnd";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.c_TreeEvent.ResumeLayout(false);
            this.c_treedevice.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ImageList i_contents;
        private System.Windows.Forms.ContextMenuStrip c_TreeEvent;
        private System.Windows.Forms.ToolStripMenuItem b_Invoke;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar p_progress;
        private System.Windows.Forms.ToolStripStatusLabel t_progress;
        private System.Windows.Forms.Timer t_ConnectionCheck;
        private System.Windows.Forms.ToolStripStatusLabel t_Connected;
        private System.Windows.Forms.ToolStripLabel t_time;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.TextBox t_Log;
        private System.Windows.Forms.ContextMenuStrip c_treedevice;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.TreeView t_contents;
      

    }
}

