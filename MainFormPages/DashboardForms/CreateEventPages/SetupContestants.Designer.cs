namespace NexScore
{
    partial class SetupContestants
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tblLayoutMainC = new TableLayoutPanel();
            _flowMainC = new FlowLayoutPanel();
            pnlSpaceAboveC = new Panel();
            cbContestType = new ComboBox();
            lblContestType = new Label();
            btnAddContestant = new Button();
            _pnlFemale = new Panel();
            _lblFemale = new Label();
            _pnlMale = new Panel();
            _lblMale = new Label();
            pnlSpaceBelowC = new Panel();
            btnSaveContestants = new Button();
            tblLayoutMainC.SuspendLayout();
            _flowMainC.SuspendLayout();
            pnlSpaceAboveC.SuspendLayout();
            _pnlFemale.SuspendLayout();
            _pnlMale.SuspendLayout();
            SuspendLayout();
            // 
            // tblLayoutMainC
            // 
            tblLayoutMainC.BackColor = Color.Transparent;
            tblLayoutMainC.ColumnCount = 1;
            tblLayoutMainC.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblLayoutMainC.Controls.Add(_flowMainC, 0, 0);
            tblLayoutMainC.Controls.Add(btnSaveContestants, 0, 1);
            tblLayoutMainC.Dock = DockStyle.Fill;
            tblLayoutMainC.Location = new Point(0, 0);
            tblLayoutMainC.Name = "tblLayoutMainC";
            tblLayoutMainC.RowCount = 2;
            tblLayoutMainC.RowStyles.Add(new RowStyle(SizeType.Percent, 92.3536453F));
            tblLayoutMainC.RowStyles.Add(new RowStyle(SizeType.Percent, 7.646356F));
            tblLayoutMainC.Size = new Size(806, 837);
            tblLayoutMainC.TabIndex = 0;
            // 
            // _flowMainC
            // 
            _flowMainC.AutoScroll = true;
            _flowMainC.Controls.Add(pnlSpaceAboveC);
            _flowMainC.Controls.Add(btnAddContestant);
            _flowMainC.Controls.Add(_pnlFemale);
            _flowMainC.Controls.Add(_pnlMale);
            _flowMainC.Controls.Add(pnlSpaceBelowC);
            _flowMainC.FlowDirection = FlowDirection.TopDown;
            _flowMainC.Location = new Point(3, 3);
            _flowMainC.Name = "_flowMainC";
            _flowMainC.Padding = new Padding(25, 15, 25, 15);
            _flowMainC.Size = new Size(800, 767);
            _flowMainC.TabIndex = 0;
            _flowMainC.WrapContents = false;
            // 
            // pnlSpaceAboveC
            // 
            pnlSpaceAboveC.Controls.Add(cbContestType);
            pnlSpaceAboveC.Controls.Add(lblContestType);
            pnlSpaceAboveC.Dock = DockStyle.Top;
            pnlSpaceAboveC.Location = new Point(28, 18);
            pnlSpaceAboveC.Name = "pnlSpaceAboveC";
            pnlSpaceAboveC.Size = new Size(749, 58);
            pnlSpaceAboveC.TabIndex = 0;
            // 
            // cbContestType
            // 
            cbContestType.Font = new Font("Lexend Deca", 9F);
            cbContestType.FormattingEnabled = true;
            cbContestType.Location = new Point(0, 29);
            cbContestType.Name = "cbContestType";
            cbContestType.Size = new Size(121, 27);
            cbContestType.TabIndex = 0;
            // 
            // lblContestType
            // 
            lblContestType.AutoSize = true;
            lblContestType.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            lblContestType.ForeColor = Color.FromArgb(247, 246, 237);
            lblContestType.Location = new Point(0, 0);
            lblContestType.Name = "lblContestType";
            lblContestType.Size = new Size(119, 25);
            lblContestType.TabIndex = 1;
            lblContestType.Text = "Contest Type:";
            // 
            // btnAddContestant
            // 
            btnAddContestant.BackColor = Color.FromArgb(53, 55, 102);
            btnAddContestant.FlatStyle = FlatStyle.Popup;
            btnAddContestant.Font = new Font("Lexend Deca", 12F);
            btnAddContestant.ForeColor = Color.FromArgb(247, 246, 237);
            btnAddContestant.Location = new Point(28, 82);
            btnAddContestant.Name = "btnAddContestant";
            btnAddContestant.Size = new Size(749, 35);
            btnAddContestant.TabIndex = 1;
            btnAddContestant.Text = "+ Add Contestant";
            btnAddContestant.UseVisualStyleBackColor = false;
            // 
            // _pnlFemale
            // 
            _pnlFemale.BackColor = Color.DimGray;
            _pnlFemale.Controls.Add(_lblFemale);
            _pnlFemale.Dock = DockStyle.Top;
            _pnlFemale.Location = new Point(28, 123);
            _pnlFemale.Name = "_pnlFemale";
            _pnlFemale.Size = new Size(749, 32);
            _pnlFemale.TabIndex = 3;
            // 
            // _lblFemale
            // 
            _lblFemale.Anchor = AnchorStyles.None;
            _lblFemale.AutoSize = true;
            _lblFemale.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            _lblFemale.ForeColor = Color.FromArgb(247, 246, 237);
            _lblFemale.Location = new Point(343, 4);
            _lblFemale.Name = "_lblFemale";
            _lblFemale.Size = new Size(70, 25);
            _lblFemale.TabIndex = 3;
            _lblFemale.Text = "Female";
            _lblFemale.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _pnlMale
            // 
            _pnlMale.BackColor = Color.DimGray;
            _pnlMale.Controls.Add(_lblMale);
            _pnlMale.Dock = DockStyle.Top;
            _pnlMale.Location = new Point(28, 161);
            _pnlMale.Name = "_pnlMale";
            _pnlMale.Size = new Size(749, 32);
            _pnlMale.TabIndex = 4;
            // 
            // _lblMale
            // 
            _lblMale.Anchor = AnchorStyles.None;
            _lblMale.AutoSize = true;
            _lblMale.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            _lblMale.ForeColor = Color.FromArgb(247, 246, 237);
            _lblMale.Location = new Point(353, 4);
            _lblMale.Name = "_lblMale";
            _lblMale.Size = new Size(50, 25);
            _lblMale.TabIndex = 2;
            _lblMale.Text = "Male";
            _lblMale.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlSpaceBelowC
            // 
            pnlSpaceBelowC.Dock = DockStyle.Bottom;
            pnlSpaceBelowC.Location = new Point(28, 199);
            pnlSpaceBelowC.Name = "pnlSpaceBelowC";
            pnlSpaceBelowC.Size = new Size(749, 41);
            pnlSpaceBelowC.TabIndex = 2;
            // 
            // btnSaveContestants
            // 
            btnSaveContestants.BackColor = Color.FromArgb(53, 55, 102);
            btnSaveContestants.FlatStyle = FlatStyle.Popup;
            btnSaveContestants.Font = new Font("Lexend Deca", 9F);
            btnSaveContestants.ForeColor = Color.FromArgb(247, 246, 237);
            btnSaveContestants.Location = new Point(25, 788);
            btnSaveContestants.Margin = new Padding(25, 15, 25, 15);
            btnSaveContestants.Name = "btnSaveContestants";
            btnSaveContestants.Size = new Size(124, 29);
            btnSaveContestants.TabIndex = 1;
            btnSaveContestants.Text = "Save";
            btnSaveContestants.UseVisualStyleBackColor = false;
            // 
            // SetupContestants
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 23, 42);
            BackgroundImageLayout = ImageLayout.Stretch;
            Controls.Add(tblLayoutMainC);
            Name = "SetupContestants";
            Size = new Size(806, 837);
            tblLayoutMainC.ResumeLayout(false);
            _flowMainC.ResumeLayout(false);
            pnlSpaceAboveC.ResumeLayout(false);
            pnlSpaceAboveC.PerformLayout();
            _pnlFemale.ResumeLayout(false);
            _pnlFemale.PerformLayout();
            _pnlMale.ResumeLayout(false);
            _pnlMale.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tblLayoutMainC;
        private FlowLayoutPanel _flowMainC;
        private Button btnSaveContestants;
        private Panel pnlSpaceAboveC;
        private Button btnAddContestant;
        private Panel pnlSpaceBelowC;
        private ComboBox cbContestType;
        private Label lblContestType;
        private CheckBox _chkSeparate;
        private Panel _pnlFemale;
        private Panel _pnlMale;
        private Label label1;
        private Label _lblMale;
        private Label _lblFemale;
    }
}
