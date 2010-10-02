namespace MainStationFrontEnd
{
    partial class InvokeRemoteEventWindow
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.b_Invoke = new System.Windows.Forms.Button();
            this.d_Parameters = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.d_Parameters)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.b_Invoke);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 276);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(203, 25);
            this.panel1.TabIndex = 1;
            // 
            // b_Invoke
            // 
            this.b_Invoke.Dock = System.Windows.Forms.DockStyle.Right;
            this.b_Invoke.Location = new System.Drawing.Point(125, 0);
            this.b_Invoke.Name = "b_Invoke";
            this.b_Invoke.Size = new System.Drawing.Size(78, 25);
            this.b_Invoke.TabIndex = 0;
            this.b_Invoke.Text = "Invoke";
            this.b_Invoke.UseVisualStyleBackColor = true;
            this.b_Invoke.Click += new System.EventHandler(this.b_Invoke_Click);
            // 
            // d_Parameters
            // 
            this.d_Parameters.AllowUserToAddRows = false;
            this.d_Parameters.AllowUserToDeleteRows = false;
            this.d_Parameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.d_Parameters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.d_Parameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.d_Parameters.Location = new System.Drawing.Point(0, 0);
            this.d_Parameters.Name = "d_Parameters";
            this.d_Parameters.RowHeadersVisible = false;
            this.d_Parameters.Size = new System.Drawing.Size(203, 276);
            this.d_Parameters.TabIndex = 2;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Name";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Value";
            this.Column2.Name = "Column2";
            // 
            // InvokeRemoteEventWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(203, 301);
            this.Controls.Add(this.d_Parameters);
            this.Controls.Add(this.panel1);
            this.Name = "InvokeRemoteEventWindow";
            this.Text = "InvokeEventWindow";
            this.Load += new System.EventHandler(this.InvokeEventWindow_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.d_Parameters)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button b_Invoke;
        private System.Windows.Forms.DataGridView d_Parameters;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
    }
}