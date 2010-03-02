namespace BaseFrontEnd
{
    partial class f_AddDevice
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
            this.n_DeviceID = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.l_Description = new System.Windows.Forms.Label();
            this.l_Devices = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.n_DeviceID)).BeginInit();
            this.SuspendLayout();
            // 
            // n_DeviceID
            // 
            this.n_DeviceID.Location = new System.Drawing.Point(12, 25);
            this.n_DeviceID.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.n_DeviceID.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.n_DeviceID.Name = "n_DeviceID";
            this.n_DeviceID.Size = new System.Drawing.Size(132, 20);
            this.n_DeviceID.TabIndex = 0;
            this.n_DeviceID.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Insert your device ID here:";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(277, 295);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 20);
            this.button1.TabIndex = 2;
            this.button1.Text = "Go for it";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // l_Description
            // 
            this.l_Description.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.l_Description.Location = new System.Drawing.Point(150, 64);
            this.l_Description.Name = "l_Description";
            this.l_Description.Size = new System.Drawing.Size(206, 228);
            this.l_Description.TabIndex = 5;
            this.l_Description.Text = "-";
            // 
            // l_Devices
            // 
            this.l_Devices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.l_Devices.Location = new System.Drawing.Point(12, 64);
            this.l_Devices.Name = "l_Devices";
            this.l_Devices.Size = new System.Drawing.Size(132, 251);
            this.l_Devices.TabIndex = 6;
            this.l_Devices.UseCompatibleStateImageBehavior = false;
            this.l_Devices.View = System.Windows.Forms.View.List;
            this.l_Devices.SelectedIndexChanged += new System.EventHandler(this.l_Devices_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Select the type of device";
            // 
            // f_AddDevice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 327);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.l_Devices);
            this.Controls.Add(this.l_Description);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.n_DeviceID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "f_AddDevice";
            this.Text = "Register new device";
            this.Load += new System.EventHandler(this.f_AddDevice_Load);
            ((System.ComponentModel.ISupportInitialize)(this.n_DeviceID)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown n_DeviceID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label l_Description;
        private System.Windows.Forms.ListView l_Devices;
        private System.Windows.Forms.Label label2;
    }
}