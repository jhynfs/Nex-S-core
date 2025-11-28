namespace NexScore
{
    partial class PhaseControl
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
            pnlPhase = new Panel();
            _lblPhaseTotalWeight = new Label();
            lblforPhaseTotal = new Label();
            Separator3 = new Panel();
            _chkIndependentPhase = new CheckBox();
            _btnRemovePhase = new PictureBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            _flowSegment = new FlowLayoutPanel();
            _btnAddSegment = new Button();
            _txtPhaseNo = new TextBox();
            _txtPhaseName = new TextBox();
            _txtPhaseWeight = new TextBox();
            lblPhaseName = new Label();
            lblPhaseNo = new Label();
            lblPhaseWeight = new Label();
            pnlPhase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_btnRemovePhase).BeginInit();
            SuspendLayout();
            // 
            // pnlPhase
            // 
            pnlPhase.AutoSize = true;
            pnlPhase.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlPhase.Controls.Add(_lblPhaseTotalWeight);
            pnlPhase.Controls.Add(lblforPhaseTotal);
            pnlPhase.Controls.Add(Separator3);
            pnlPhase.Controls.Add(_chkIndependentPhase);
            pnlPhase.Controls.Add(_btnRemovePhase);
            pnlPhase.Controls.Add(flowLayoutPanel1);
            pnlPhase.Controls.Add(_flowSegment);
            pnlPhase.Controls.Add(_btnAddSegment);
            pnlPhase.Controls.Add(_txtPhaseNo);
            pnlPhase.Controls.Add(_txtPhaseName);
            pnlPhase.Controls.Add(_txtPhaseWeight);
            pnlPhase.Controls.Add(lblPhaseName);
            pnlPhase.Controls.Add(lblPhaseNo);
            pnlPhase.Controls.Add(lblPhaseWeight);
            pnlPhase.Dock = DockStyle.Fill;
            pnlPhase.Location = new Point(0, 0);
            pnlPhase.Name = "pnlPhase";
            pnlPhase.Size = new Size(731, 135);
            pnlPhase.TabIndex = 2;
            // 
            // _lblPhaseTotalWeight
            // 
            _lblPhaseTotalWeight.AutoSize = true;
            _lblPhaseTotalWeight.Font = new Font("Lexend Deca Medium", 10F, FontStyle.Bold);
            _lblPhaseTotalWeight.ForeColor = Color.FromArgb(247, 246, 237);
            _lblPhaseTotalWeight.Location = new Point(212, 40);
            _lblPhaseTotalWeight.Name = "_lblPhaseTotalWeight";
            _lblPhaseTotalWeight.Size = new Size(19, 22);
            _lblPhaseTotalWeight.TabIndex = 28;
            _lblPhaseTotalWeight.Text = "0";
            // 
            // lblforPhaseTotal
            // 
            lblforPhaseTotal.AutoSize = true;
            lblforPhaseTotal.Font = new Font("Lexend Deca Medium", 10F, FontStyle.Bold);
            lblforPhaseTotal.ForeColor = Color.FromArgb(247, 246, 237);
            lblforPhaseTotal.Location = new Point(41, 40);
            lblforPhaseTotal.Name = "lblforPhaseTotal";
            lblforPhaseTotal.Size = new Size(165, 22);
            lblforPhaseTotal.TabIndex = 27;
            lblforPhaseTotal.Text = "Total Segment Weight:";
            // 
            // Separator3
            // 
            Separator3.BackColor = Color.FromArgb(171, 168, 166);
            Separator3.Dock = DockStyle.Top;
            Separator3.Location = new Point(0, 37);
            Separator3.Name = "Separator3";
            Separator3.Size = new Size(731, 2);
            Separator3.TabIndex = 19;
            // 
            // _chkIndependentPhase
            // 
            _chkIndependentPhase.Appearance = Appearance.Button;
            _chkIndependentPhase.AutoSize = true;
            _chkIndependentPhase.BackColor = Color.DimGray;
            _chkIndependentPhase.FlatAppearance.BorderSize = 0;
            _chkIndependentPhase.FlatAppearance.CheckedBackColor = Color.FromArgb(53, 55, 102);
            _chkIndependentPhase.FlatStyle = FlatStyle.Flat;
            _chkIndependentPhase.Font = new Font("Lexend Deca", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _chkIndependentPhase.ForeColor = Color.FromArgb(247, 246, 237);
            _chkIndependentPhase.Location = new Point(595, 40);
            _chkIndependentPhase.Name = "_chkIndependentPhase";
            _chkIndependentPhase.Size = new Size(96, 29);
            _chkIndependentPhase.TabIndex = 26;
            _chkIndependentPhase.Text = "Independent";
            _chkIndependentPhase.UseVisualStyleBackColor = false;
            // 
            // _btnRemovePhase
            // 
            _btnRemovePhase.BackgroundImage = Properties.Resources.lets_icons_remove_fill;
            _btnRemovePhase.BackgroundImageLayout = ImageLayout.Zoom;
            _btnRemovePhase.Location = new Point(9, 94);
            _btnRemovePhase.Name = "_btnRemovePhase";
            _btnRemovePhase.Size = new Size(26, 24);
            _btnRemovePhase.TabIndex = 25;
            _btnRemovePhase.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Dock = DockStyle.Top;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(731, 37);
            flowLayoutPanel1.TabIndex = 18;
            // 
            // _flowSegment
            // 
            _flowSegment.AutoSize = true;
            _flowSegment.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _flowSegment.FlowDirection = FlowDirection.TopDown;
            _flowSegment.Location = new Point(9, 130);
            _flowSegment.MinimumSize = new Size(719, 2);
            _flowSegment.Name = "_flowSegment";
            _flowSegment.Padding = new Padding(15, 0, 0, 0);
            _flowSegment.Size = new Size(719, 2);
            _flowSegment.TabIndex = 17;
            _flowSegment.WrapContents = false;
            // 
            // _btnAddSegment
            // 
            _btnAddSegment.BackColor = Color.FromArgb(53, 55, 102);
            _btnAddSegment.FlatStyle = FlatStyle.Popup;
            _btnAddSegment.Font = new Font("Lexend Deca", 10F);
            _btnAddSegment.ForeColor = Color.FromArgb(247, 246, 237);
            _btnAddSegment.Location = new Point(578, 86);
            _btnAddSegment.Margin = new Padding(2);
            _btnAddSegment.Name = "_btnAddSegment";
            _btnAddSegment.Size = new Size(134, 32);
            _btnAddSegment.TabIndex = 16;
            _btnAddSegment.Text = "+ Add Segment";
            _btnAddSegment.UseVisualStyleBackColor = false;
            // 
            // _txtPhaseNo
            // 
            _txtPhaseNo.Font = new Font("Lexend Deca Medium", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txtPhaseNo.Location = new Point(41, 94);
            _txtPhaseNo.Name = "_txtPhaseNo";
            _txtPhaseNo.Size = new Size(78, 24);
            _txtPhaseNo.TabIndex = 15;
            _txtPhaseNo.TextAlign = HorizontalAlignment.Center;
            // 
            // _txtPhaseName
            // 
            _txtPhaseName.Font = new Font("Lexend Deca Medium", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txtPhaseName.Location = new Point(131, 94);
            _txtPhaseName.Margin = new Padding(15, 3, 15, 3);
            _txtPhaseName.Name = "_txtPhaseName";
            _txtPhaseName.Size = new Size(316, 24);
            _txtPhaseName.TabIndex = 13;
            // 
            // _txtPhaseWeight
            // 
            _txtPhaseWeight.Font = new Font("Lexend Deca Medium", 9.75F, FontStyle.Bold);
            _txtPhaseWeight.Location = new Point(463, 94);
            _txtPhaseWeight.Name = "_txtPhaseWeight";
            _txtPhaseWeight.Size = new Size(100, 24);
            _txtPhaseWeight.TabIndex = 14;
            _txtPhaseWeight.TextAlign = HorizontalAlignment.Center;
            // 
            // lblPhaseName
            // 
            lblPhaseName.AutoSize = true;
            lblPhaseName.Font = new Font("Lexend Deca", 12F, FontStyle.Bold);
            lblPhaseName.ForeColor = Color.FromArgb(247, 246, 237);
            lblPhaseName.Location = new Point(131, 66);
            lblPhaseName.Name = "lblPhaseName";
            lblPhaseName.Size = new Size(114, 25);
            lblPhaseName.TabIndex = 10;
            lblPhaseName.Text = "Phase Name";
            // 
            // lblPhaseNo
            // 
            lblPhaseNo.AutoSize = true;
            lblPhaseNo.Font = new Font("Lexend Deca", 12F, FontStyle.Bold);
            lblPhaseNo.ForeColor = Color.FromArgb(247, 246, 237);
            lblPhaseNo.Location = new Point(41, 66);
            lblPhaseNo.Name = "lblPhaseNo";
            lblPhaseNo.Size = new Size(39, 25);
            lblPhaseNo.TabIndex = 11;
            lblPhaseNo.Text = "No.";
            // 
            // lblPhaseWeight
            // 
            lblPhaseWeight.AutoSize = true;
            lblPhaseWeight.Font = new Font("Lexend Deca", 12F, FontStyle.Bold);
            lblPhaseWeight.ForeColor = Color.FromArgb(247, 246, 237);
            lblPhaseWeight.Location = new Point(463, 66);
            lblPhaseWeight.Name = "lblPhaseWeight";
            lblPhaseWeight.Size = new Size(69, 25);
            lblPhaseWeight.TabIndex = 12;
            lblPhaseWeight.Text = "Weight";
            // 
            // PhaseControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.Transparent;
            Controls.Add(pnlPhase);
            Name = "PhaseControl";
            Size = new Size(731, 135);
            pnlPhase.ResumeLayout(false);
            pnlPhase.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_btnRemovePhase).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlPhase;
        private FlowLayoutPanel _flowSegment;
        private Button _btnAddSegment;
        private TextBox _txtPhaseNo;
        private TextBox _txtPhaseName;
        private TextBox _txtPhaseWeight;
        private Label lblPhaseName;
        private Label lblPhaseNo;
        private Label lblPhaseWeight;
        private FlowLayoutPanel flowLayoutPanel1;
        private Panel Separator3;
        private PictureBox _btnRemovePhase;
        private CheckBox _chkIndependentPhase;
        private Label _lblPhaseTotalWeight;
        private Label lblforPhaseTotal;
    }
}
