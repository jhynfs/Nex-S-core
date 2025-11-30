namespace NexScore.MainFormPages
{
    partial class PageResults
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
            pnlMainResults = new Panel();
            SuspendLayout();
            // 
            // pnlMainResults
            // 
            pnlMainResults.BackColor = Color.FromArgb(15, 23, 42);
            pnlMainResults.Dock = DockStyle.Fill;
            pnlMainResults.Location = new Point(0, 0);
            pnlMainResults.Name = "pnlMainResults";
            pnlMainResults.Padding = new Padding(150);
            pnlMainResults.Size = new Size(1524, 1041);
            pnlMainResults.TabIndex = 0;
            // 
            // PageResults
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = Color.FromArgb(15, 23, 42);
            Controls.Add(pnlMainResults);
            Name = "PageResults";
            Size = new Size(1524, 1041);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlMainResults;
    }
}
