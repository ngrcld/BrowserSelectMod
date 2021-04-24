namespace BrowserSelectMod {
    partial class BrowserButtonControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.BrowserIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.BrowserIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // BrowserIcon
            // 
            this.BrowserIcon.Location = new System.Drawing.Point(0, 0);
            this.BrowserIcon.Name = "BrowserIcon";
            this.BrowserIcon.Size = new System.Drawing.Size(64, 64);
            this.BrowserIcon.TabIndex = 0;
            this.BrowserIcon.TabStop = false;
            // 
            // BrowserButtonControl
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.AutoSize = true;
            this.Controls.Add(this.BrowserIcon);
            this.Name = "BrowserButtonControl";
            this.Size = new System.Drawing.Size(64, 64);
            ((System.ComponentModel.ISupportInitialize)(this.BrowserIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox BrowserIcon;
    }
}
