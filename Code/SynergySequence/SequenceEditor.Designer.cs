namespace SynergySequence
{
    partial class SequenceEditWindow
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.t_Update = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // t_Update
            // 
            this.t_Update.Enabled = true;
            this.t_Update.Interval = 50;
            this.t_Update.Tick += new System.EventHandler(this.t_Update_Tick);
            // 
            // SequenceEditWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(233)))), ((int)(((byte)(245)))));
            this.Name = "SequenceEditWindow";
            this.Size = new System.Drawing.Size(455, 150);
            this.Load += new System.EventHandler(this.SequenceEditWindow_Load);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.SequenceEditWindow_DragOver);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SequenceEditWindow_MouseMove);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.SequenceEditWindow_DragDrop);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SequenceEditWindow_MouseDown);
            this.DragLeave += new System.EventHandler(this.SequenceEditWindow_DragLeave);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.SequenceEditWindow_DragEnter);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer t_Update;
    }
}
