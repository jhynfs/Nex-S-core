namespace NexScore.MainFormPages
{
    partial class PageContestants
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
            pnlMainContestants = new Panel();
            SuspendLayout();
            // 
            // pnlMainContestants
            // 
            pnlMainContestants.AutoScroll = true;
            pnlMainContestants.Dock = DockStyle.Fill;
            pnlMainContestants.Location = new Point(0, 0);
            pnlMainContestants.Name = "pnlMainContestants";
            pnlMainContestants.Padding = new Padding(350, 100, 350, 100);
            pnlMainContestants.Size = new Size(1014, 684);
            pnlMainContestants.TabIndex = 0;
            // 
            // PageContestants
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Transparent;
            Controls.Add(pnlMainContestants);
            Name = "PageContestants";
            Size = new Size(1014, 684);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlMainContestants;
    }
}
