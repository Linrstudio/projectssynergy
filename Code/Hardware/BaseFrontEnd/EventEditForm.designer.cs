namespace BaseFrontEnd
{
    partial class EventEditForm
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
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.PropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.s_WorkingArea = new BaseFrontEnd.SequenceEditWindow();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter2.Location = new System.Drawing.Point(636, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 334);
            this.splitter2.TabIndex = 4;
            this.splitter2.TabStop = false;
            // 
            // PropertyGrid
            // 
            this.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Right;
            this.PropertyGrid.Location = new System.Drawing.Point(639, 0);
            this.PropertyGrid.Name = "PropertyGrid";
            this.PropertyGrid.Size = new System.Drawing.Size(232, 334);
            this.PropertyGrid.TabIndex = 1;
            this.PropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PropertyGrid_PropertyValueChanged);
            // 
            // s_WorkingArea
            // 
            this.s_WorkingArea.BackColor = System.Drawing.Color.Transparent;
            this.s_WorkingArea.Location = new System.Drawing.Point(56, 46);
            this.s_WorkingArea.Name = "s_WorkingArea";
            this.s_WorkingArea.Size = new System.Drawing.Size(531, 238);
            this.s_WorkingArea.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.s_WorkingArea);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(636, 334);
            this.panel1.TabIndex = 6;
            // 
            // EventEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(871, 334);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.PropertyGrid);
            this.DoubleBuffered = true;
            this.Name = "EventEditForm";
            this.Text = "KismetEditor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.KismetEditor_Load);
            this.ResizeEnd += new System.EventHandler(this.KismetEditor_ResizeEnd);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.PropertyGrid PropertyGrid;
        private SequenceEditWindow s_WorkingArea;
        private System.Windows.Forms.Panel panel1;
    }
}