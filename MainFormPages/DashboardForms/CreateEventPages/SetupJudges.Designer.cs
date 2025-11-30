namespace NexScore.CreateEventPages
{
    partial class SetupJudges
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.FlowLayoutPanel _flowMainJ;
        private System.Windows.Forms.Panel pnlSpaceAboveJ;
        private System.Windows.Forms.Button btnAddJudge;
        private System.Windows.Forms.Panel pnlSpaceBelowJ;
        private System.Windows.Forms.Button btnSaveJudges;
        private System.Windows.Forms.TableLayoutPanel tblLayoutMainJ;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _flowMainJ = new System.Windows.Forms.FlowLayoutPanel();
            pnlSpaceAboveJ = new System.Windows.Forms.Panel();
            btnAddJudge = new System.Windows.Forms.Button();
            pnlSpaceBelowJ = new System.Windows.Forms.Panel();
            btnSaveJudges = new System.Windows.Forms.Button();
            tblLayoutMainJ = new System.Windows.Forms.TableLayoutPanel();
            _flowMainJ.SuspendLayout();
            tblLayoutMainJ.SuspendLayout();
            SuspendLayout();
            // 
            // _flowMainJ
            // 
            _flowMainJ.AutoScroll = true;
            _flowMainJ.BackColor = System.Drawing.Color.Transparent;
            _flowMainJ.Controls.Add(pnlSpaceAboveJ);
            _flowMainJ.Controls.Add(btnAddJudge);
            _flowMainJ.Controls.Add(pnlSpaceBelowJ);
            _flowMainJ.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            _flowMainJ.Location = new System.Drawing.Point(3, 3);
            _flowMainJ.Name = "_flowMainJ";
            _flowMainJ.Padding = new System.Windows.Forms.Padding(25, 15, 25, 15);
            _flowMainJ.Size = new System.Drawing.Size(800, 767);
            _flowMainJ.TabIndex = 0;
            _flowMainJ.WrapContents = false;
            // 
            // pnlSpaceAboveJ
            // 
            pnlSpaceAboveJ.Location = new System.Drawing.Point(28, 18);
            pnlSpaceAboveJ.Name = "pnlSpaceAboveJ";
            pnlSpaceAboveJ.Size = new System.Drawing.Size(749, 40);
            pnlSpaceAboveJ.TabIndex = 0;
            // 
            // btnAddJudge
            // 
            btnAddJudge.BackColor = System.Drawing.Color.FromArgb(53, 55, 102);
            btnAddJudge.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            btnAddJudge.Font = new System.Drawing.Font("Lexend Deca", 12F);
            btnAddJudge.ForeColor = System.Drawing.Color.FromArgb(247, 246, 237);
            btnAddJudge.Location = new System.Drawing.Point(28, 64);
            btnAddJudge.Name = "btnAddJudge";
            btnAddJudge.Size = new System.Drawing.Size(749, 36);
            btnAddJudge.TabIndex = 1;
            btnAddJudge.Text = "+ Add Judge";
            btnAddJudge.UseVisualStyleBackColor = false;
            // 
            // pnlSpaceBelowJ
            // 
            pnlSpaceBelowJ.Location = new System.Drawing.Point(28, 106);
            pnlSpaceBelowJ.Name = "pnlSpaceBelowJ";
            pnlSpaceBelowJ.Size = new System.Drawing.Size(749, 40);
            pnlSpaceBelowJ.TabIndex = 2;
            // 
            // btnSaveJudges
            // 
            btnSaveJudges.BackColor = System.Drawing.Color.FromArgb(53, 55, 102);
            btnSaveJudges.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            btnSaveJudges.Font = new System.Drawing.Font("Lexend Deca", 9F);
            btnSaveJudges.ForeColor = System.Drawing.Color.FromArgb(247, 246, 237);
            btnSaveJudges.Location = new System.Drawing.Point(3, 776);
            btnSaveJudges.Name = "btnSaveJudges";
            btnSaveJudges.Size = new System.Drawing.Size(800, 38);
            btnSaveJudges.TabIndex = 3;
            btnSaveJudges.Text = "save";
            btnSaveJudges.UseVisualStyleBackColor = false;
            // 
            // tblLayoutMainJ
            // 
            tblLayoutMainJ.ColumnCount = 1;
            tblLayoutMainJ.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblLayoutMainJ.Controls.Add(_flowMainJ, 0, 0);
            tblLayoutMainJ.Controls.Add(btnSaveJudges, 0, 1);
            tblLayoutMainJ.Dock = System.Windows.Forms.DockStyle.Fill;
            tblLayoutMainJ.Location = new System.Drawing.Point(0, 0);
            tblLayoutMainJ.Name = "tblLayoutMainJ";
            tblLayoutMainJ.RowCount = 2;
            tblLayoutMainJ.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblLayoutMainJ.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            tblLayoutMainJ.Size = new System.Drawing.Size(806, 817);
            tblLayoutMainJ.TabIndex = 4;
            // 
            // SetupJudges
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.Transparent;
            Controls.Add(tblLayoutMainJ);
            Name = "SetupJudges";
            Size = new System.Drawing.Size(806, 817);
            _flowMainJ.ResumeLayout(false);
            tblLayoutMainJ.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}