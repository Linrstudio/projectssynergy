namespace WebInterface
{
    partial class SceneEditor
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
            this.SuspendLayout();
            // 
            // SceneEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "SceneEditor";
            this.Size = new System.Drawing.Size(559, 418);
            this.Load += new System.EventHandler(this.SceneEditor_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SceneEditor_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SceneEditor_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SceneEditor_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SceneEditor_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
