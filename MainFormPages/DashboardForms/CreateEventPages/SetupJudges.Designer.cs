namespace NexScore.CreateEventPages
{
    partial class SetupJudges
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
            _flowMainJ = new FlowLayoutPanel();
            pnlSpaceAboveJ = new Panel();
            btnAddJudge = new Button();
            pnlSpaceBelowJ = new Panel();
            btnSaveJudges = new Button();
            tblLayoutMainJ = new TableLayoutPanel();
            _flowMainJ.SuspendLayout();
            tblLayoutMainJ.SuspendLayout();
            SuspendLayout();
            // 
            // _flowMainJ
            // 
            _flowMainJ.AutoScroll = true;
            _flowMainJ.BackColor = Color.Transparent;
            _flowMainJ.Controls.Add(pnlSpaceAboveJ);
            _flowMainJ.Controls.Add(btnAddJudge);
            _flowMainJ.Controls.Add(pnlSpaceBelowJ);
            _flowMainJ.FlowDirection = FlowDirection.TopDown;
            _flowMainJ.Location = new Point(3, 3);
            _flowMainJ.Name = "_flowMainJ";
            _flowMainJ.Padding = new Padding(25, 15, 25, 15);
            _flowMainJ.Size = new Size(800, 767);
            _flowMainJ.TabIndex = 0;
            _flowMainJ.WrapContents = false;
            // 
            // pnlSpaceAboveJ
            // 
            pnlSpaceAboveJ.Dock = DockStyle.Top;
            pnlSpaceAboveJ.Location = new Point(28, 18);
            pnlSpaceAboveJ.Name = "pnlSpaceAboveJ";
            pnlSpaceAboveJ.Size = new Size(749, 41);
            pnlSpaceAboveJ.TabIndex = 0;
            // 
            // btnAddJudge
            // 
            btnAddJudge.BackColor = Color.FromArgb(53, 55, 102);
            btnAddJudge.FlatStyle = FlatStyle.Popup;
            btnAddJudge.Font = new Font("Lexend Deca", 9F);
            btnAddJudge.ForeColor = Color.FromArgb(247, 246, 237);
            btnAddJudge.Location = new Point(28, 65);
            btnAddJudge.Name = "btnAddJudge";
            btnAddJudge.Size = new Size(749, 29);
            btnAddJudge.TabIndex = 1;
            btnAddJudge.Text = "+ Add Judge";
            btnAddJudge.UseVisualStyleBackColor = false;
            // 
            // pnlSpaceBelowJ
            // 
            pnlSpaceBelowJ.Dock = DockStyle.Bottom;
            pnlSpaceBelowJ.Location = new Point(28, 100);
            pnlSpaceBelowJ.Name = "pnlSpaceBelowJ";
            pnlSpaceBelowJ.Size = new Size(749, 41);
            pnlSpaceBelowJ.TabIndex = 2;
            // 
            // btnSaveJudges
            // 
            btnSaveJudges.BackColor = Color.FromArgb(53, 55, 102);
            btnSaveJudges.FlatStyle = FlatStyle.Popup;
            btnSaveJudges.Font = new Font("Lexend Deca", 9F);
            btnSaveJudges.ForeColor = Color.FromArgb(247, 246, 237);
            btnSaveJudges.Location = new Point(25, 788);
            btnSaveJudges.Margin = new Padding(25, 15, 25, 15);
            btnSaveJudges.Name = "btnSaveJudges";
            btnSaveJudges.Size = new Size(124, 29);
            btnSaveJudges.TabIndex = 3;
            btnSaveJudges.Text = "Save";
            btnSaveJudges.UseVisualStyleBackColor = false;
            btnSaveJudges.Click += btnSaveJudges_Click;
            // 
            // tblLayoutMainJ
            // 
            tblLayoutMainJ.BackColor = Color.Transparent;
            tblLayoutMainJ.ColumnCount = 1;
            tblLayoutMainJ.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblLayoutMainJ.Controls.Add(_flowMainJ, 0, 0);
            tblLayoutMainJ.Controls.Add(btnSaveJudges, 0, 1);
            tblLayoutMainJ.Dock = DockStyle.Fill;
            tblLayoutMainJ.Location = new Point(0, 0);
            tblLayoutMainJ.Name = "tblLayoutMainJ";
            tblLayoutMainJ.RowCount = 2;
            tblLayoutMainJ.RowStyles.Add(new RowStyle(SizeType.Percent, 92.3536453F));
            tblLayoutMainJ.RowStyles.Add(new RowStyle(SizeType.Percent, 7.646356F));
            tblLayoutMainJ.Size = new Size(806, 837);
            tblLayoutMainJ.TabIndex = 1;
            // 
            // SetupJudges
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.bg_dark;
            BackgroundImageLayout = ImageLayout.Stretch;
            Controls.Add(tblLayoutMainJ);
            Name = "SetupJudges";
            Size = new Size(806, 837);
            _flowMainJ.ResumeLayout(false);
            tblLayoutMainJ.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel _flowMainJ;
        private Panel pnlSpaceAboveJ;
        private Button btnAddJudge;
        private Panel pnlSpaceBelowJ;
        private Button btnSaveJudges;
        private TableLayoutPanel tblLayoutMainJ;
    }
}
