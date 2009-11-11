namespace SynergyClient
{
    partial class f_Main
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
            this.button6 = new System.Windows.Forms.Button();
            this.l_Scenes = new System.Windows.Forms.ListView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.p_Container = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.b_Whois = new System.Windows.Forms.Button();
            this.b_Save = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.t_World = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.d_Connections = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.t_refresh = new System.Windows.Forms.Timer(this.components);
            this.p_Graphic = new SynergyClient.SceneControl();
            this.worldview = new SynergyClient.WorldView();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.p_Container.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.t_World.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.d_Connections)).BeginInit();
            this.SuspendLayout();
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(-231, 18);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(64, 64);
            this.button6.TabIndex = 0;
            this.button6.Text = "button1";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // l_Scenes
            // 
            this.l_Scenes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.l_Scenes.Location = new System.Drawing.Point(0, 13);
            this.l_Scenes.Name = "l_Scenes";
            this.l_Scenes.Size = new System.Drawing.Size(151, 508);
            this.l_Scenes.SmallImageList = this.imageList;
            this.l_Scenes.TabIndex = 1;
            this.l_Scenes.UseCompatibleStateImageBehavior = false;
            this.l_Scenes.View = System.Windows.Forms.View.List;
            this.l_Scenes.SelectedIndexChanged += new System.EventHandler(this.l_Scenes_SelectedIndexChanged);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList.ImageSize = new System.Drawing.Size(1, 24);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.t_World);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(768, 553);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.splitter1);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(760, 527);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Scene";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel2.Controls.Add(this.p_Container);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(157, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(600, 521);
            this.panel2.TabIndex = 3;
            this.panel2.Resize += new System.EventHandler(this.panel2_Resize);
            // 
            // p_Container
            // 
            this.p_Container.BackColor = System.Drawing.SystemColors.Control;
            this.p_Container.Controls.Add(this.p_Graphic);
            this.p_Container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.p_Container.Location = new System.Drawing.Point(0, 26);
            this.p_Container.Name = "p_Container";
            this.p_Container.Size = new System.Drawing.Size(600, 495);
            this.p_Container.TabIndex = 1;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Control;
            this.panel4.Controls.Add(this.button1);
            this.panel4.Controls.Add(this.b_Whois);
            this.panel4.Controls.Add(this.b_Save);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(600, 26);
            this.panel4.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(81, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "F1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // b_Whois
            // 
            this.b_Whois.Dock = System.Windows.Forms.DockStyle.Right;
            this.b_Whois.Location = new System.Drawing.Point(546, 0);
            this.b_Whois.Name = "b_Whois";
            this.b_Whois.Size = new System.Drawing.Size(54, 26);
            this.b_Whois.TabIndex = 1;
            this.b_Whois.Text = "Whois";
            this.b_Whois.UseVisualStyleBackColor = true;
            this.b_Whois.Click += new System.EventHandler(this.b_Whois_Click);
            // 
            // b_Save
            // 
            this.b_Save.Dock = System.Windows.Forms.DockStyle.Left;
            this.b_Save.Location = new System.Drawing.Point(0, 0);
            this.b_Save.Name = "b_Save";
            this.b_Save.Size = new System.Drawing.Size(75, 26);
            this.b_Save.TabIndex = 0;
            this.b_Save.Text = "Save";
            this.b_Save.UseVisualStyleBackColor = true;
            this.b_Save.Click += new System.EventHandler(this.b_Save_Click);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(154, 3);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 521);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.l_Scenes);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(151, 521);
            this.panel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Scenes";
            // 
            // t_World
            // 
            this.t_World.Controls.Add(this.worldview);
            this.t_World.Location = new System.Drawing.Point(4, 22);
            this.t_World.Name = "t_World";
            this.t_World.Padding = new System.Windows.Forms.Padding(3);
            this.t_World.Size = new System.Drawing.Size(773, 568);
            this.t_World.TabIndex = 2;
            this.t_World.Text = "World";
            this.t_World.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(773, 568);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Node";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.d_Connections);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(257, 562);
            this.panel3.TabIndex = 4;
            // 
            // d_Connections
            // 
            this.d_Connections.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.d_Connections.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column4});
            this.d_Connections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.d_Connections.Location = new System.Drawing.Point(0, 13);
            this.d_Connections.Name = "d_Connections";
            this.d_Connections.Size = new System.Drawing.Size(257, 549);
            this.d_Connections.TabIndex = 3;
            this.d_Connections.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "IPAddress";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Port";
            this.Column2.Name = "Column2";
            this.Column2.Width = 50;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Connect";
            this.Column4.Name = "Column4";
            this.Column4.Width = 64;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Connection list";
            // 
            // t_refresh
            // 
            this.t_refresh.Enabled = true;
            this.t_refresh.Interval = 50;
            this.t_refresh.Tick += new System.EventHandler(this.t_refresh_Tick);
            // 
            // p_Graphic
            // 
            this.p_Graphic.Location = new System.Drawing.Point(59, 45);
            this.p_Graphic.Name = "p_Graphic";
            this.p_Graphic.Size = new System.Drawing.Size(265, 158);
            this.p_Graphic.TabIndex = 1;
            this.p_Graphic.Text = "sceneControl1";
            // 
            // worldview
            // 
            this.worldview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.worldview.Location = new System.Drawing.Point(3, 3);
            this.worldview.Name = "worldview";
            this.worldview.Size = new System.Drawing.Size(767, 562);
            this.worldview.TabIndex = 1;
            this.worldview.Text = "worldView1";
            this.worldview.Click += new System.EventHandler(this.worldview_Click);
            // 
            // f_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 553);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button6);
            this.DoubleBuffered = true;
            this.Name = "f_Main";
            this.ShowIcon = false;
            this.Text = "Client";
            this.Load += new System.EventHandler(this.f_Main_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.f_Main_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.p_Container.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.t_World.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.d_Connections)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.ListView l_Scenes;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Panel p_Container;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TabPage t_World;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button b_Save;
        private System.Windows.Forms.DataGridView d_Connections;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewButtonColumn Column4;
        private System.Windows.Forms.Timer t_refresh;
        private System.Windows.Forms.Splitter splitter1;
        private SceneControl p_Graphic;
        private System.Windows.Forms.Button b_Whois;
        private System.Windows.Forms.Button button1;
        private WorldView worldview;
    }
}

