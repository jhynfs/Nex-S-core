namespace NexScore.MainFormPages
{
    partial class PageLogs
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
            pnlMainLogs = new Panel();
            SuspendLayout();
            // 
            // pnlMainLogs
            // 
            pnlMainLogs.AutoSize = true;
            pnlMainLogs.BackColor = Color.FromArgb(15, 23, 42);
            pnlMainLogs.Dock = DockStyle.Fill;
            pnlMainLogs.Location = new Point(0, 0);
            pnlMainLogs.Name = "pnlMainLogs";
            pnlMainLogs.Padding = new Padding(200, 150, 200, 150);
            pnlMainLogs.Size = new Size(1524, 1041);
            pnlMainLogs.TabIndex = 0;
            // 
            // PageLogs
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            Controls.Add(pnlMainLogs);
            Name = "PageLogs";
            Size = new Size(1524, 1041);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlMainLogs;
    }
}
