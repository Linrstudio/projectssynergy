namespace SynergyClient
{
    partial class Form1
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
            this.p_WorkingArea = new System.Windows.Forms.Panel();
            this.t_tick = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // p_WorkingArea
            // 
            this.p_WorkingArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.p_WorkingArea.Location = new System.Drawing.Point(0, 0);
            this.p_WorkingArea.Name = "p_WorkingArea";
            this.p_WorkingArea.Size = new System.Drawing.Size(785, 513);
            this.p_WorkingArea.TabIndex = 0;
            // 
            // t_tick
            // 
            this.t_tick.Enabled = true;
            this.t_tick.Interval = 20;
            this.t_tick.Tick += new System.EventHandler(this.t_tick_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 513);
            this.Controls.Add(this.p_WorkingArea);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel p_WorkingArea;
        private System.Windows.Forms.Timer t_tick;
    }
}

