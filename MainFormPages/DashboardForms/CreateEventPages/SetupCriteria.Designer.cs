namespace NexScore
{
    partial class SetupCriteria
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
            _flowMain = new FlowLayoutPanel();
            panelSpaceAbove = new Panel();
            btnRefresh = new PictureBox();
            lblforEventWeight = new Label();
            lblEventTotalWeight = new Label();
            panelSpaceBelow = new Panel();
            btnAddPhase = new Button();
            panelSpaceNext = new Panel();
            btnSaveCriteria = new Button();
            tblLayoutMain = new TableLayoutPanel();
            _flowMain.SuspendLayout();
            panelSpaceAbove.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)btnRefresh).BeginInit();
            tblLayoutMain.SuspendLayout();
            SuspendLayout();
            // 
            // _flowMain
            // 
            _flowMain.AutoScroll = true;
            _flowMain.BackColor = Color.Transparent;
            _flowMain.Controls.Add(panelSpaceAbove);
            _flowMain.Controls.Add(panelSpaceBelow);
            _flowMain.Controls.Add(btnAddPhase);
            _flowMain.Controls.Add(panelSpaceNext);
            _flowMain.FlowDirection = FlowDirection.TopDown;
            _flowMain.Location = new Point(3, 3);
            _flowMain.Name = "_flowMain";
            _flowMain.Padding = new Padding(25, 15, 25, 15);
            _flowMain.Size = new Size(800, 767);
            _flowMain.TabIndex = 2;
            _flowMain.WrapContents = false;
            // 
            // panelSpaceAbove
            // 
            panelSpaceAbove.Controls.Add(btnRefresh);
            panelSpaceAbove.Controls.Add(lblforEventWeight);
            panelSpaceAbove.Controls.Add(lblEventTotalWeight);
            panelSpaceAbove.Dock = DockStyle.Top;
            panelSpaceAbove.Location = new Point(28, 18);
            panelSpaceAbove.Name = "panelSpaceAbove";
            panelSpaceAbove.Size = new Size(747, 25);
            panelSpaceAbove.TabIndex = 14;
            // 
            // btnRefresh
            // 
            btnRefresh.Image = Properties.Resources.material_symbols_refresh;
            btnRefresh.Location = new Point(724, 5);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(20, 20);
            btnRefresh.SizeMode = PictureBoxSizeMode.StretchImage;
            btnRefresh.TabIndex = 18;
            btnRefresh.TabStop = false;
            // 
            // lblforEventWeight
            // 
            lblforEventWeight.AutoSize = true;
            lblforEventWeight.Font = new Font("Lexend Deca Medium", 9F, FontStyle.Bold);
            lblforEventWeight.ForeColor = Color.FromArgb(247, 246, 237);
            lblforEventWeight.Location = new Point(3, 2);
            lblforEventWeight.Name = "lblforEventWeight";
            lblforEventWeight.Size = new Size(128, 19);
            lblforEventWeight.TabIndex = 0;
            lblforEventWeight.Text = "Total Phase Weight:";
            // 
            // lblEventTotalWeight
            // 
            lblEventTotalWeight.AutoSize = true;
            lblEventTotalWeight.Font = new Font("Lexend Deca Medium", 9F, FontStyle.Bold);
            lblEventTotalWeight.ForeColor = Color.FromArgb(247, 246, 237);
            lblEventTotalWeight.Location = new Point(137, 2);
            lblEventTotalWeight.Name = "lblEventTotalWeight";
            lblEventTotalWeight.Size = new Size(17, 19);
            lblEventTotalWeight.TabIndex = 17;
            lblEventTotalWeight.Text = "0";
            // 
            // panelSpaceBelow
            // 
            panelSpaceBelow.Dock = DockStyle.Bottom;
            panelSpaceBelow.Location = new Point(28, 49);
            panelSpaceBelow.Name = "panelSpaceBelow";
            panelSpaceBelow.Size = new Size(747, 41);
            panelSpaceBelow.TabIndex = 15;
            // 
            // btnAddPhase
            // 
            btnAddPhase.Anchor = AnchorStyles.None;
            btnAddPhase.BackColor = Color.FromArgb(53, 55, 102);
            btnAddPhase.FlatStyle = FlatStyle.Popup;
            btnAddPhase.Font = new Font("Lexend Deca", 9F);
            btnAddPhase.ForeColor = Color.FromArgb(247, 246, 237);
            btnAddPhase.Location = new Point(27, 95);
            btnAddPhase.Margin = new Padding(2);
            btnAddPhase.Name = "btnAddPhase";
            btnAddPhase.Size = new Size(749, 29);
            btnAddPhase.TabIndex = 13;
            btnAddPhase.Text = "+ Add Phase";
            btnAddPhase.UseVisualStyleBackColor = false;
            // 
            // panelSpaceNext
            // 
            panelSpaceNext.Location = new Point(28, 129);
            panelSpaceNext.Name = "panelSpaceNext";
            panelSpaceNext.Size = new Size(747, 41);
            panelSpaceNext.TabIndex = 16;
            // 
            // btnSaveCriteria
            // 
            btnSaveCriteria.BackColor = Color.FromArgb(53, 55, 102);
            btnSaveCriteria.FlatStyle = FlatStyle.Popup;
            btnSaveCriteria.Font = new Font("Lexend Deca", 9F);
            btnSaveCriteria.ForeColor = Color.FromArgb(247, 246, 237);
            btnSaveCriteria.Location = new Point(25, 788);
            btnSaveCriteria.Margin = new Padding(25, 15, 25, 15);
            btnSaveCriteria.Name = "btnSaveCriteria";
            btnSaveCriteria.Size = new Size(124, 29);
            btnSaveCriteria.TabIndex = 16;
            btnSaveCriteria.Text = "Save";
            btnSaveCriteria.UseVisualStyleBackColor = false;
            btnSaveCriteria.Click += btnSaveCriteria_Click;
            // 
            // tblLayoutMain
            // 
            tblLayoutMain.BackColor = Color.Transparent;
            tblLayoutMain.ColumnCount = 1;
            tblLayoutMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblLayoutMain.Controls.Add(_flowMain, 0, 0);
            tblLayoutMain.Controls.Add(btnSaveCriteria, 0, 1);
            tblLayoutMain.Dock = DockStyle.Fill;
            tblLayoutMain.Location = new Point(0, 0);
            tblLayoutMain.Name = "tblLayoutMain";
            tblLayoutMain.RowCount = 2;
            tblLayoutMain.RowStyles.Add(new RowStyle(SizeType.Percent, 92.3536453F));
            tblLayoutMain.RowStyles.Add(new RowStyle(SizeType.Percent, 7.646356F));
            tblLayoutMain.Size = new Size(806, 837);
            tblLayoutMain.TabIndex = 3;
            // 
            // SetupCriteria
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.bg_dark;
            Controls.Add(tblLayoutMain);
            Name = "SetupCriteria";
            Size = new Size(806, 837);
            _flowMain.ResumeLayout(false);
            panelSpaceAbove.ResumeLayout(false);
            panelSpaceAbove.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)btnRefresh).EndInit();
            tblLayoutMain.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private FlowLayoutPanel _flowMain;
        private Button btnAddPhase;
        private Panel panelSpaceAbove;
        private Panel panelSpaceBelow;
        private Button btnSaveCriteria;
        private TableLayoutPanel tblLayoutMain;
        private Label lblforEventWeight;
        private Panel panelSpaceNext;
        private Label lblEventTotalWeight;
        private PictureBox btnRefresh;
    }
}
