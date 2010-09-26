namespace MainStationFrontEnd
{
    partial class AddDevice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddDevice));
            this.t_Devices = new System.Windows.Forms.TreeView();
            this.i_devices = new System.Windows.Forms.ImageList(this.components);
            this.l_Description = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.t_Name = new System.Windows.Forms.TextBox();
            this.n_ID = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.b_Ok = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.n_ID)).BeginInit();
            this.SuspendLayout();
            // 
            // t_Devices
            // 
            this.t_Devices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.t_Devices.ImageIndex = 0;
            this.t_Devices.ImageList = this.i_devices;
            this.t_Devices.Location = new System.Drawing.Point(12, 83);
            this.t_Devices.Name = "t_Devices";
            this.t_Devices.SelectedImageIndex = 1;
            this.t_Devices.Size = new System.Drawing.Size(174, 278);
            this.t_Devices.TabIndex = 0;
            this.t_Devices.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.t_Devices_AfterSelect);
            // 
            // i_devices
            // 
            this.i_devices.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("i_devices.ImageStream")));
            this.i_devices.TransparentColor = System.Drawing.Color.Transparent;
            this.i_devices.Images.SetKeyName(0, "Device.png");
            this.i_devices.Images.SetKeyName(1, "DeviceSelected.png");
            // 
            // l_Description
            // 
            this.l_Description.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.l_Description.Location = new System.Drawing.Point(197, 83);
            this.l_Description.Name = "l_Description";
            this.l_Description.Size = new System.Drawing.Size(194, 252);
            this.l_Description.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(192, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Description";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Device Type";
            // 
            // t_Name
            // 
            this.t_Name.Location = new System.Drawing.Point(12, 25);
            this.t_Name.Name = "t_Name";
            this.t_Name.Size = new System.Drawing.Size(174, 20);
            this.t_Name.TabIndex = 5;
            this.t_Name.TextChanged += new System.EventHandler(this.t_Name_TextChanged);
            // 
            // n_ID
            // 
            this.n_ID.BackColor = System.Drawing.SystemColors.Window;
            this.n_ID.Location = new System.Drawing.Point(195, 25);
            this.n_ID.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.n_ID.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.n_ID.Name = "n_ID";
            this.n_ID.Size = new System.Drawing.Size(66, 20);
            this.n_ID.TabIndex = 6;
            this.n_ID.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.n_ID.ValueChanged += new System.EventHandler(this.n_ID_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Device Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(197, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Device ID";
            // 
            // b_Ok
            // 
            this.b_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_Ok.Location = new System.Drawing.Point(311, 338);
            this.b_Ok.Name = "b_Ok";
            this.b_Ok.Size = new System.Drawing.Size(75, 23);
            this.b_Ok.TabIndex = 9;
            this.b_Ok.Text = "Add device";
            this.b_Ok.UseVisualStyleBackColor = true;
            this.b_Ok.Click += new System.EventHandler(this.b_Ok_Click);
            // 
            // AddDevice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 373);
            this.Controls.Add(this.b_Ok);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.n_ID);
            this.Controls.Add(this.t_Name);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.l_Description);
            this.Controls.Add(this.t_Devices);
            this.Name = "AddDevice";
            this.Text = "AddDevice";
            this.Load += new System.EventHandler(this.AddDevice_Load);
            ((System.ComponentModel.ISupportInitialize)(this.n_ID)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView t_Devices;
        private System.Windows.Forms.ImageList i_devices;
        private System.Windows.Forms.Label l_Description;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox t_Name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button b_Ok;
        private System.Windows.Forms.NumericUpDown n_ID;
    }
}