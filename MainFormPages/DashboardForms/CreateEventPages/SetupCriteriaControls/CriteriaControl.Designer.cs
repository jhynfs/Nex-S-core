namespace NexScore.CreateEventPages.SetupCriteriaControls
{
    partial class CriteriaControl
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
            pnlFiller = new Panel();
            _txtCriteriaName = new TextBox();
            _txtCriteriaWeight = new TextBox();
            lblCriteriaName = new Label();
            lblCriteriaWeight = new Label();
            _btnRemoveCriteria = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)_btnRemoveCriteria).BeginInit();
            SuspendLayout();
            // 
            // pnlFiller
            // 
            pnlFiller.Location = new Point(506, 28);
            pnlFiller.Name = "pnlFiller";
            pnlFiller.Size = new Size(118, 24);
            pnlFiller.TabIndex = 25;
            // 
            // _txtCriteriaName
            // 
            _txtCriteriaName.Font = new Font("Lexend Deca Medium", 11F, FontStyle.Bold);
            _txtCriteriaName.Location = new Point(61, 28);
            _txtCriteriaName.Margin = new Padding(15, 3, 15, 3);
            _txtCriteriaName.Name = "_txtCriteriaName";
            _txtCriteriaName.Size = new Size(316, 26);
            _txtCriteriaName.TabIndex = 21;
            // 
            // _txtCriteriaWeight
            // 
            _txtCriteriaWeight.Font = new Font("Lexend Deca Medium", 11F, FontStyle.Bold);
            _txtCriteriaWeight.Location = new Point(390, 28);
            _txtCriteriaWeight.Name = "_txtCriteriaWeight";
            _txtCriteriaWeight.Size = new Size(100, 26);
            _txtCriteriaWeight.TabIndex = 22;
            _txtCriteriaWeight.TextAlign = HorizontalAlignment.Center;
            // 
            // lblCriteriaName
            // 
            lblCriteriaName.AutoSize = true;
            lblCriteriaName.Font = new Font("Lexend Deca", 11F, FontStyle.Bold);
            lblCriteriaName.ForeColor = Color.FromArgb(247, 246, 237);
            lblCriteriaName.Location = new Point(61, 4);
            lblCriteriaName.Name = "lblCriteriaName";
            lblCriteriaName.Size = new Size(119, 24);
            lblCriteriaName.TabIndex = 19;
            lblCriteriaName.Text = "Criteria Name";
            // 
            // lblCriteriaWeight
            // 
            lblCriteriaWeight.AutoSize = true;
            lblCriteriaWeight.Font = new Font("Lexend Deca", 10F, FontStyle.Bold);
            lblCriteriaWeight.ForeColor = Color.FromArgb(247, 246, 237);
            lblCriteriaWeight.Location = new Point(390, 4);
            lblCriteriaWeight.Name = "lblCriteriaWeight";
            lblCriteriaWeight.Size = new Size(60, 22);
            lblCriteriaWeight.TabIndex = 20;
            lblCriteriaWeight.Text = "Weight";
            // 
            // _btnRemoveCriteria
            // 
            _btnRemoveCriteria.BackgroundImage = Properties.Resources.lets_icons_remove_fill;
            _btnRemoveCriteria.BackgroundImageLayout = ImageLayout.Zoom;
            _btnRemoveCriteria.Location = new Point(29, 28);
            _btnRemoveCriteria.Name = "_btnRemoveCriteria";
            _btnRemoveCriteria.Size = new Size(26, 24);
            _btnRemoveCriteria.TabIndex = 26;
            _btnRemoveCriteria.TabStop = false;
            // 
            // CriteriaControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.Transparent;
            Controls.Add(_btnRemoveCriteria);
            Controls.Add(pnlFiller);
            Controls.Add(_txtCriteriaName);
            Controls.Add(_txtCriteriaWeight);
            Controls.Add(lblCriteriaName);
            Controls.Add(lblCriteriaWeight);
            Name = "CriteriaControl";
            Size = new Size(627, 57);
            ((System.ComponentModel.ISupportInitialize)_btnRemoveCriteria).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlFiller;
        private TextBox _txtCriteriaName;
        private TextBox _txtCriteriaWeight;
        private Label lblCriteriaName;
        private Label lblCriteriaWeight;
        private PictureBox _btnRemoveCriteria;
    }
}
