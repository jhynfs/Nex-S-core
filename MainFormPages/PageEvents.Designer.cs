namespace NexScore
{
    partial class PageEvents
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
            pnlMainEvents = new Panel();
            pnlCurrentEvtDetails = new Panel();
            btnEvtSelect = new Button();
            button1 = new Button();
            btnDetailsMod = new Button();
            _txtVenueHere = new TextBox();
            _txtEvtOrg = new TextBox();
            _lblOrg = new Label();
            label3 = new Label();
            _txtDateHere = new TextBox();
            _lblVen = new Label();
            _txtDescriptionHere = new TextBox();
            btnEvtNew = new Button();
            _lblDesc = new Label();
            _lblCurrentEvtName = new Label();
            pnlMainEvents.SuspendLayout();
            pnlCurrentEvtDetails.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMainEvents
            // 
            pnlMainEvents.Anchor = AnchorStyles.None;
            pnlMainEvents.BackColor = Color.Transparent;
            pnlMainEvents.Controls.Add(pnlCurrentEvtDetails);
            pnlMainEvents.Location = new Point(0, 0);
            pnlMainEvents.Name = "pnlMainEvents";
            pnlMainEvents.Padding = new Padding(25);
            pnlMainEvents.Size = new Size(1014, 684);
            pnlMainEvents.TabIndex = 0;
            // 
            // pnlCurrentEvtDetails
            // 
            pnlCurrentEvtDetails.Anchor = AnchorStyles.None;
            pnlCurrentEvtDetails.Controls.Add(btnEvtSelect);
            pnlCurrentEvtDetails.Controls.Add(button1);
            pnlCurrentEvtDetails.Controls.Add(btnDetailsMod);
            pnlCurrentEvtDetails.Controls.Add(_txtVenueHere);
            pnlCurrentEvtDetails.Controls.Add(_txtEvtOrg);
            pnlCurrentEvtDetails.Controls.Add(_lblOrg);
            pnlCurrentEvtDetails.Controls.Add(label3);
            pnlCurrentEvtDetails.Controls.Add(_txtDateHere);
            pnlCurrentEvtDetails.Controls.Add(_lblVen);
            pnlCurrentEvtDetails.Controls.Add(_txtDescriptionHere);
            pnlCurrentEvtDetails.Controls.Add(btnEvtNew);
            pnlCurrentEvtDetails.Controls.Add(_lblDesc);
            pnlCurrentEvtDetails.Controls.Add(_lblCurrentEvtName);
            pnlCurrentEvtDetails.Location = new Point(25, 25);
            pnlCurrentEvtDetails.Name = "pnlCurrentEvtDetails";
            pnlCurrentEvtDetails.Padding = new Padding(10);
            pnlCurrentEvtDetails.Size = new Size(964, 634);
            pnlCurrentEvtDetails.TabIndex = 0;
            // 
            // btnEvtSelect
            // 
            btnEvtSelect.BackColor = Color.FromArgb(53, 55, 102);
            btnEvtSelect.FlatStyle = FlatStyle.Popup;
            btnEvtSelect.Font = new Font("Lexend Deca", 9F);
            btnEvtSelect.ForeColor = Color.FromArgb(247, 246, 237);
            btnEvtSelect.Location = new Point(573, 589);
            btnEvtSelect.Name = "btnEvtSelect";
            btnEvtSelect.Size = new Size(124, 29);
            btnEvtSelect.TabIndex = 13;
            btnEvtSelect.Text = "Select Event";
            btnEvtSelect.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            button1.Location = new Point(622, 588);
            button1.Name = "button1";
            button1.Size = new Size(8, 8);
            button1.TabIndex = 12;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // btnDetailsMod
            // 
            btnDetailsMod.BackColor = Color.FromArgb(53, 55, 102);
            btnDetailsMod.FlatStyle = FlatStyle.Popup;
            btnDetailsMod.Font = new Font("Lexend Deca", 9F);
            btnDetailsMod.ForeColor = Color.FromArgb(247, 246, 237);
            btnDetailsMod.Location = new Point(260, 589);
            btnDetailsMod.Name = "btnDetailsMod";
            btnDetailsMod.Size = new Size(124, 29);
            btnDetailsMod.TabIndex = 11;
            btnDetailsMod.Text = "Modify";
            btnDetailsMod.UseVisualStyleBackColor = false;
            btnDetailsMod.Click += btnDetailsMod_Click;
            // 
            // _txtVenueHere
            // 
            _txtVenueHere.BackColor = Color.FromArgb(58, 61, 116);
            _txtVenueHere.BorderStyle = BorderStyle.None;
            _txtVenueHere.Font = new Font("Lexend Deca SemiBold", 9.75F, FontStyle.Bold);
            _txtVenueHere.ForeColor = Color.FromArgb(247, 246, 237);
            _txtVenueHere.Location = new Point(103, 362);
            _txtVenueHere.Name = "_txtVenueHere";
            _txtVenueHere.ReadOnly = true;
            _txtVenueHere.Size = new Size(356, 17);
            _txtVenueHere.TabIndex = 10;
            // 
            // _txtEvtOrg
            // 
            _txtEvtOrg.BackColor = Color.FromArgb(58, 61, 116);
            _txtEvtOrg.BorderStyle = BorderStyle.None;
            _txtEvtOrg.Font = new Font("Lexend Deca SemiBold", 9.75F, FontStyle.Bold);
            _txtEvtOrg.ForeColor = Color.FromArgb(247, 246, 237);
            _txtEvtOrg.Location = new Point(495, 362);
            _txtEvtOrg.Multiline = true;
            _txtEvtOrg.Name = "_txtEvtOrg";
            _txtEvtOrg.Size = new Size(346, 108);
            _txtEvtOrg.TabIndex = 9;
            // 
            // _lblOrg
            // 
            _lblOrg.AutoSize = true;
            _lblOrg.Font = new Font("Lexend Deca", 9.75F);
            _lblOrg.ForeColor = Color.FromArgb(247, 246, 237);
            _lblOrg.Location = new Point(495, 338);
            _lblOrg.Name = "_lblOrg";
            _lblOrg.Size = new Size(118, 21);
            _lblOrg.TabIndex = 8;
            _lblOrg.Text = "Event Organizers";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Lexend Deca", 9.75F);
            label3.ForeColor = Color.FromArgb(247, 246, 237);
            label3.Location = new Point(103, 423);
            label3.Name = "label3";
            label3.Size = new Size(81, 21);
            label3.TabIndex = 7;
            label3.Text = "Event Date";
            // 
            // _txtDateHere
            // 
            _txtDateHere.BackColor = Color.FromArgb(58, 61, 116);
            _txtDateHere.BorderStyle = BorderStyle.None;
            _txtDateHere.Font = new Font("Lexend Deca SemiBold", 9.75F, FontStyle.Bold);
            _txtDateHere.ForeColor = Color.FromArgb(247, 246, 237);
            _txtDateHere.Location = new Point(103, 447);
            _txtDateHere.Name = "_txtDateHere";
            _txtDateHere.ReadOnly = true;
            _txtDateHere.Size = new Size(356, 17);
            _txtDateHere.TabIndex = 6;
            // 
            // _lblVen
            // 
            _lblVen.AutoSize = true;
            _lblVen.Font = new Font("Lexend Deca", 9.75F);
            _lblVen.ForeColor = Color.FromArgb(247, 246, 237);
            _lblVen.Location = new Point(103, 338);
            _lblVen.Name = "_lblVen";
            _lblVen.Size = new Size(90, 21);
            _lblVen.TabIndex = 4;
            _lblVen.Text = "Event Venue";
            // 
            // _txtDescriptionHere
            // 
            _txtDescriptionHere.BackColor = Color.FromArgb(58, 61, 116);
            _txtDescriptionHere.BorderStyle = BorderStyle.None;
            _txtDescriptionHere.Font = new Font("Lexend Deca SemiBold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txtDescriptionHere.ForeColor = Color.FromArgb(247, 246, 237);
            _txtDescriptionHere.Location = new Point(103, 173);
            _txtDescriptionHere.Multiline = true;
            _txtDescriptionHere.Name = "_txtDescriptionHere";
            _txtDescriptionHere.ReadOnly = true;
            _txtDescriptionHere.Size = new Size(738, 129);
            _txtDescriptionHere.TabIndex = 3;
            // 
            // btnEvtNew
            // 
            btnEvtNew.BackColor = Color.FromArgb(53, 55, 102);
            btnEvtNew.FlatStyle = FlatStyle.Popup;
            btnEvtNew.Font = new Font("Lexend Deca", 9F);
            btnEvtNew.ForeColor = Color.FromArgb(247, 246, 237);
            btnEvtNew.ImageAlign = ContentAlignment.MiddleLeft;
            btnEvtNew.Location = new Point(416, 589);
            btnEvtNew.Name = "btnEvtNew";
            btnEvtNew.Size = new Size(124, 29);
            btnEvtNew.TabIndex = 1;
            btnEvtNew.Text = "Add New Event";
            btnEvtNew.UseVisualStyleBackColor = false;
            btnEvtNew.Click += btnEvtNew_Click;
            // 
            // _lblDesc
            // 
            _lblDesc.AutoSize = true;
            _lblDesc.Font = new Font("Lexend Deca", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _lblDesc.ForeColor = Color.FromArgb(247, 246, 237);
            _lblDesc.Location = new Point(103, 149);
            _lblDesc.Name = "_lblDesc";
            _lblDesc.Size = new Size(83, 21);
            _lblDesc.TabIndex = 2;
            _lblDesc.Text = "Description";
            // 
            // _lblCurrentEvtName
            // 
            _lblCurrentEvtName.AutoSize = true;
            _lblCurrentEvtName.Font = new Font("Lexend Deca", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _lblCurrentEvtName.ForeColor = Color.FromArgb(247, 246, 237);
            _lblCurrentEvtName.Location = new Point(21, 102);
            _lblCurrentEvtName.MinimumSize = new Size(920, 25);
            _lblCurrentEvtName.Name = "_lblCurrentEvtName";
            _lblCurrentEvtName.Size = new Size(920, 33);
            _lblCurrentEvtName.TabIndex = 0;
            _lblCurrentEvtName.Text = "Current Event Name";
            _lblCurrentEvtName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PageEvents
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.bg_dark;
            BackgroundImageLayout = ImageLayout.Stretch;
            Controls.Add(pnlMainEvents);
            Name = "PageEvents";
            Size = new Size(1014, 684);
            pnlMainEvents.ResumeLayout(false);
            pnlCurrentEvtDetails.ResumeLayout(false);
            pnlCurrentEvtDetails.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlMainEvents;
        private Panel pnlCurrentEvtDetails;
        private Label _lblCurrentEvtName;
        private Button btnEvtNew;
        private Label label2;
        private Button btnDetailsMod;
        private Button btnEvtSelect;
        private Button button1;
        private TextBox _txtVenueHere;
        private TextBox _txtEvtOrg;
        private Label _lblOrg;
        private Label label3;
        private TextBox _txtDateHere;
        private Label _lblVen;
        private TextBox _txtDescriptionHere;
        private Label _lblDesc;
    }
}
