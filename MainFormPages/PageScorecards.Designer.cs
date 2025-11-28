namespace NexScore.MainFormPages
{
    partial class PageScorecards
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
            pnlMainScorecards = new Panel();
            SuspendLayout();
            // 
            // pnlMainScorecards
            // 
            pnlMainScorecards.BackColor = Color.FromArgb(15, 23, 42);
            pnlMainScorecards.Dock = DockStyle.Fill;
            pnlMainScorecards.Location = new Point(0, 0);
            pnlMainScorecards.Name = "pnlMainScorecards";
            pnlMainScorecards.Padding = new Padding(200, 150, 200, 150);
            pnlMainScorecards.Size = new Size(1524, 1041);
            pnlMainScorecards.TabIndex = 0;
            // 
            // PageScorecards
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = Color.FromArgb(247, 246, 237);
            Controls.Add(pnlMainScorecards);
            Name = "PageScorecards";
            Size = new Size(1524, 1041);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlMainScorecards;
    }
}
