namespace NexScore.CreateEventPages.SetupJudgesControl
{
    partial class AddJudgeControl
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
            btnRemoveJudge = new PictureBox();
            lblJudgeName = new Label();
            _txtJudgeNo = new TextBox();
            _txtJudgeTitle = new TextBox();
            _txtJudgeName = new TextBox();
            lblJudgeTitle = new Label();
            lblJudgeNo = new Label();
            _pnlAddJudgeControl = new Panel();
            ((System.ComponentModel.ISupportInitialize)btnRemoveJudge).BeginInit();
            _pnlAddJudgeControl.SuspendLayout();
            SuspendLayout();
            // 
            // btnRemoveJudge
            // 
            btnRemoveJudge.BackgroundImage = Properties.Resources.lets_icons_remove_fill;
            btnRemoveJudge.BackgroundImageLayout = ImageLayout.Stretch;
            btnRemoveJudge.Location = new Point(91, 25);
            btnRemoveJudge.Name = "btnRemoveJudge";
            btnRemoveJudge.Size = new Size(27, 27);
            btnRemoveJudge.SizeMode = PictureBoxSizeMode.StretchImage;
            btnRemoveJudge.TabIndex = 0;
            btnRemoveJudge.TabStop = false;
            // 
            // lblJudgeName
            // 
            lblJudgeName.AutoSize = true;
            lblJudgeName.BackColor = Color.Transparent;
            lblJudgeName.Font = new Font("Lexend Deca", 11F, FontStyle.Bold);
            lblJudgeName.ForeColor = Color.FromArgb(247, 246, 237);
            lblJudgeName.Location = new Point(344, 4);
            lblJudgeName.Name = "lblJudgeName";
            lblJudgeName.Size = new Size(119, 24);
            lblJudgeName.TabIndex = 3;
            lblJudgeName.Text = "Judge's Name";
            // 
            // _txtJudgeNo
            // 
            _txtJudgeNo.Font = new Font("Lexend Deca Medium", 12F, FontStyle.Bold);
            _txtJudgeNo.Location = new Point(136, 28);
            _txtJudgeNo.Name = "_txtJudgeNo";
            _txtJudgeNo.Size = new Size(63, 27);
            _txtJudgeNo.TabIndex = 4;
            _txtJudgeNo.TextAlign = HorizontalAlignment.Center;
            // 
            // _txtJudgeTitle
            // 
            _txtJudgeTitle.Font = new Font("Lexend Deca Medium", 12F, FontStyle.Bold);
            _txtJudgeTitle.Location = new Point(221, 28);
            _txtJudgeTitle.Name = "_txtJudgeTitle";
            _txtJudgeTitle.Size = new Size(100, 27);
            _txtJudgeTitle.TabIndex = 5;
            _txtJudgeTitle.TextAlign = HorizontalAlignment.Center;
            // 
            // _txtJudgeName
            // 
            _txtJudgeName.Font = new Font("Lexend Deca Medium", 12F, FontStyle.Bold);
            _txtJudgeName.Location = new Point(344, 28);
            _txtJudgeName.Name = "_txtJudgeName";
            _txtJudgeName.Size = new Size(316, 27);
            _txtJudgeName.TabIndex = 6;
            // 
            // lblJudgeTitle
            // 
            lblJudgeTitle.AutoSize = true;
            lblJudgeTitle.BackColor = Color.Transparent;
            lblJudgeTitle.Font = new Font("Lexend Deca", 11F, FontStyle.Bold);
            lblJudgeTitle.ForeColor = Color.FromArgb(247, 246, 237);
            lblJudgeTitle.Location = new Point(221, 4);
            lblJudgeTitle.Name = "lblJudgeTitle";
            lblJudgeTitle.Size = new Size(45, 24);
            lblJudgeTitle.TabIndex = 7;
            lblJudgeTitle.Text = "Title";
            // 
            // lblJudgeNo
            // 
            lblJudgeNo.AutoSize = true;
            lblJudgeNo.BackColor = Color.Transparent;
            lblJudgeNo.Font = new Font("Lexend Deca", 11F, FontStyle.Bold);
            lblJudgeNo.ForeColor = Color.FromArgb(247, 246, 237);
            lblJudgeNo.Location = new Point(136, 4);
            lblJudgeNo.Name = "lblJudgeNo";
            lblJudgeNo.Size = new Size(36, 24);
            lblJudgeNo.TabIndex = 8;
            lblJudgeNo.Text = "No.";
            // 
            // _pnlAddJudgeControl
            // 
            _pnlAddJudgeControl.Controls.Add(lblJudgeNo);
            _pnlAddJudgeControl.Controls.Add(_txtJudgeName);
            _pnlAddJudgeControl.Controls.Add(lblJudgeTitle);
            _pnlAddJudgeControl.Controls.Add(btnRemoveJudge);
            _pnlAddJudgeControl.Controls.Add(lblJudgeName);
            _pnlAddJudgeControl.Controls.Add(_txtJudgeTitle);
            _pnlAddJudgeControl.Controls.Add(_txtJudgeNo);
            _pnlAddJudgeControl.Dock = DockStyle.Fill;
            _pnlAddJudgeControl.Location = new Point(0, 0);
            _pnlAddJudgeControl.Name = "_pnlAddJudgeControl";
            _pnlAddJudgeControl.Size = new Size(747, 63);
            _pnlAddJudgeControl.TabIndex = 9;
            // 
            // AddJudgeControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Transparent;
            Controls.Add(_pnlAddJudgeControl);
            Name = "AddJudgeControl";
            Size = new Size(747, 63);
            ((System.ComponentModel.ISupportInitialize)btnRemoveJudge).EndInit();
            _pnlAddJudgeControl.ResumeLayout(false);
            _pnlAddJudgeControl.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox btnRemoveJudge;
        private Label lblJudgeName;
        private TextBox _txtJudgeNo;
        private TextBox _txtJudgeTitle;
        private TextBox _txtJudgeName;
        private Label lblJudgeTitle;
        private Label lblJudgeNo;
        private Panel _pnlAddJudgeControl;
    }
}
