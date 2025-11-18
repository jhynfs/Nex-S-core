namespace NexScore.MainFormPages
{
    partial class PageJudges
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
            pnlMainJudges = new Panel();
            SuspendLayout();
            // 
            // pnlMainJudges
            // 
            pnlMainJudges.AutoScroll = true;
            pnlMainJudges.Dock = DockStyle.Fill;
            pnlMainJudges.Location = new Point(0, 0);
            pnlMainJudges.Name = "pnlMainJudges";
            pnlMainJudges.Padding = new Padding(400, 150, 400, 150);
            pnlMainJudges.Size = new Size(1014, 684);
            pnlMainJudges.TabIndex = 0;
            // 
            // PageJudges
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Transparent;
            Controls.Add(pnlMainJudges);
            Name = "PageJudges";
            Size = new Size(1014, 684);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlMainJudges;
    }
}
