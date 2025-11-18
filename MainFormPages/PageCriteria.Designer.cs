using System.Drawing;
using System.Windows.Forms;

namespace NexScore.MainFormPages
{
    partial class PageCriteria
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlMainPhasesCri;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlMainPhasesCri = new Panel();
            SuspendLayout();
            // 
            // pnlMainPhasesCri
            // 
            pnlMainPhasesCri.AutoScroll = true;
            pnlMainPhasesCri.BackColor = Color.Transparent;
            pnlMainPhasesCri.Dock = DockStyle.Fill;
            pnlMainPhasesCri.Location = new Point(0, 0);
            pnlMainPhasesCri.Name = "pnlMainPhasesCri";
            pnlMainPhasesCri.Padding = new Padding(400, 150, 400, 150);
            pnlMainPhasesCri.Size = new Size(1014, 681);
            pnlMainPhasesCri.TabIndex = 0;
            // 
            // PageCriteria
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Transparent;
            BackgroundImageLayout = ImageLayout.Stretch;
            Controls.Add(pnlMainPhasesCri);
            Name = "PageCriteria";
            Size = new Size(1014, 681);
            ResumeLayout(false);
        }
    }
}