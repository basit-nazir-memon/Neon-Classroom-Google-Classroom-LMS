namespace Neon_Classroom
{
    partial class Grade
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Grade));
            this.classWorkPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // classWorkPanel
            // 
            this.classWorkPanel.Location = new System.Drawing.Point(5, 10);
            this.classWorkPanel.Name = "classWorkPanel";
            this.classWorkPanel.Size = new System.Drawing.Size(717, 428);
            this.classWorkPanel.TabIndex = 7;
            // 
            // Grade
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(734, 447);
            this.Controls.Add(this.classWorkPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Grade";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Neon Classroom";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel classWorkPanel;
    }
}

