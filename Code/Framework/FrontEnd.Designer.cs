namespace Framework
{
    partial class FrontEnd
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
            this.l_TextLog = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.t_Tick = new System.Windows.Forms.Timer(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.l_VariableLog = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.panel1 = new System.Windows.Forms.Panel();
            this.c_Input = new System.Windows.Forms.ComboBox();
            this.p_EnabledLogs = new System.Windows.Forms.Panel();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // l_TextLog
            // 
            this.l_TextLog.BackColor = System.Drawing.Color.Black;
            this.l_TextLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.l_TextLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.l_TextLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.l_TextLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l_TextLog.ForeColor = System.Drawing.Color.White;
            this.l_TextLog.FullRowSelect = true;
            this.l_TextLog.GridLines = true;
            this.l_TextLog.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.l_TextLog.Location = new System.Drawing.Point(566, 24);
            this.l_TextLog.Name = "l_TextLog";
            this.l_TextLog.Size = new System.Drawing.Size(591, 455);
            this.l_TextLog.TabIndex = 9;
            this.l_TextLog.UseCompatibleStateImageBehavior = false;
            this.l_TextLog.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Width = 300;
            // 
            // t_Tick
            // 
            this.t_Tick.Enabled = true;
            this.t_Tick.Interval = 10;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.l_TextLog);
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Controls.Add(this.l_VariableLog);
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Controls.Add(this.p_EnabledLogs);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1157, 507);
            this.panel3.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(562, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(4, 455);
            this.panel2.TabIndex = 11;
            // 
            // l_VariableLog
            // 
            this.l_VariableLog.BackColor = System.Drawing.Color.Black;
            this.l_VariableLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.l_VariableLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader6,
            this.columnHeader7});
            this.l_VariableLog.Dock = System.Windows.Forms.DockStyle.Left;
            this.l_VariableLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l_VariableLog.ForeColor = System.Drawing.Color.White;
            this.l_VariableLog.FullRowSelect = true;
            this.l_VariableLog.GridLines = true;
            this.l_VariableLog.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.l_VariableLog.Location = new System.Drawing.Point(0, 24);
            this.l_VariableLog.Name = "l_VariableLog";
            this.l_VariableLog.Size = new System.Drawing.Size(562, 455);
            this.l_VariableLog.TabIndex = 10;
            this.l_VariableLog.UseCompatibleStateImageBehavior = false;
            this.l_VariableLog.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 100;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Width = 300;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel1.Controls.Add(this.c_Input);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 479);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1157, 28);
            this.panel1.TabIndex = 5;
            // 
            // c_Input
            // 
            this.c_Input.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.c_Input.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.c_Input.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.c_Input.ForeColor = System.Drawing.Color.White;
            this.c_Input.FormattingEnabled = true;
            this.c_Input.Location = new System.Drawing.Point(4, 3);
            this.c_Input.Name = "c_Input";
            this.c_Input.Size = new System.Drawing.Size(1149, 21);
            this.c_Input.TabIndex = 0;
            this.c_Input.SelectedIndexChanged += new System.EventHandler(this.c_Input_SelectedIndexChanged);
            this.c_Input.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.c_Input_KeyPress);
            // 
            // p_EnabledLogs
            // 
            this.p_EnabledLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.p_EnabledLogs.Dock = System.Windows.Forms.DockStyle.Top;
            this.p_EnabledLogs.ForeColor = System.Drawing.Color.White;
            this.p_EnabledLogs.Location = new System.Drawing.Point(0, 0);
            this.p_EnabledLogs.Name = "p_EnabledLogs";
            this.p_EnabledLogs.Size = new System.Drawing.Size(1157, 24);
            this.p_EnabledLogs.TabIndex = 4;
            // 
            // FrontEnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1157, 507);
            this.Controls.Add(this.panel3);
            this.Name = "FrontEnd";
            this.Text = "FrontEnd";
            this.Load += new System.EventHandler(this.FrontEnd_Load);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView l_TextLog;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Timer t_Tick;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox c_Input;
        private System.Windows.Forms.Panel p_EnabledLogs;
        private System.Windows.Forms.ListView l_VariableLog;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Panel panel2;

    }
}