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
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            pnlMainEvents.SuspendLayout();
            pnlCurrentEvtDetails.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMainEvents
            // 
            pnlMainEvents.BackColor = Color.Transparent;
            pnlMainEvents.Controls.Add(pnlCurrentEvtDetails);
            pnlMainEvents.Dock = DockStyle.Fill;
            pnlMainEvents.Location = new Point(0, 0);
            pnlMainEvents.Name = "pnlMainEvents";
            pnlMainEvents.Padding = new Padding(25);
            pnlMainEvents.Size = new Size(1524, 1041);
            pnlMainEvents.TabIndex = 0;
            // 
            // pnlCurrentEvtDetails
            // 
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
            pnlCurrentEvtDetails.Controls.Add(textBox1);
            pnlCurrentEvtDetails.Controls.Add(textBox2);
            pnlCurrentEvtDetails.Controls.Add(textBox3);
            pnlCurrentEvtDetails.Controls.Add(textBox4);
            pnlCurrentEvtDetails.Location = new Point(280, 172);
            pnlCurrentEvtDetails.Name = "pnlCurrentEvtDetails";
            pnlCurrentEvtDetails.Padding = new Padding(10);
            pnlCurrentEvtDetails.Size = new Size(964, 665);
            pnlCurrentEvtDetails.TabIndex = 0;
            // 
            // btnEvtSelect
            // 
            btnEvtSelect.BackColor = Color.FromArgb(53, 55, 102);
            btnEvtSelect.FlatStyle = FlatStyle.Popup;
            btnEvtSelect.Font = new Font("Lexend Deca", 9F);
            btnEvtSelect.ForeColor = Color.FromArgb(247, 246, 237);
            btnEvtSelect.Location = new Point(577, 550);
            btnEvtSelect.Name = "btnEvtSelect";
            btnEvtSelect.Size = new Size(124, 35);
            btnEvtSelect.TabIndex = 13;
            btnEvtSelect.Text = "Select Event";
            btnEvtSelect.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            button1.Location = new Point(626, 549);
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
            btnDetailsMod.Location = new Point(264, 550);
            btnDetailsMod.Name = "btnDetailsMod";
            btnDetailsMod.Size = new Size(124, 35);
            btnDetailsMod.TabIndex = 11;
            btnDetailsMod.Text = "Modify";
            btnDetailsMod.UseVisualStyleBackColor = false;
            btnDetailsMod.Click += btnDetailsMod_Click;
            // 
            // _txtVenueHere
            // 
            _txtVenueHere.BackColor = Color.FromArgb(58, 61, 116);
            _txtVenueHere.BorderStyle = BorderStyle.None;
            _txtVenueHere.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            _txtVenueHere.ForeColor = Color.FromArgb(247, 246, 237);
            _txtVenueHere.Location = new Point(107, 329);
            _txtVenueHere.Margin = new Padding(3, 0, 3, 0);
            _txtVenueHere.Name = "_txtVenueHere";
            _txtVenueHere.ReadOnly = true;
            _txtVenueHere.Size = new Size(356, 20);
            _txtVenueHere.TabIndex = 10;
            // 
            // _txtEvtOrg
            // 
            _txtEvtOrg.BackColor = Color.FromArgb(58, 61, 116);
            _txtEvtOrg.BorderStyle = BorderStyle.None;
            _txtEvtOrg.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            _txtEvtOrg.ForeColor = Color.FromArgb(247, 246, 237);
            _txtEvtOrg.Location = new Point(499, 329);
            _txtEvtOrg.Multiline = true;
            _txtEvtOrg.Name = "_txtEvtOrg";
            _txtEvtOrg.Size = new Size(346, 108);
            _txtEvtOrg.TabIndex = 9;
            // 
            // _lblOrg
            // 
            _lblOrg.AutoSize = true;
            _lblOrg.Font = new Font("Lexend Deca", 12F);
            _lblOrg.ForeColor = Color.FromArgb(247, 246, 237);
            _lblOrg.Location = new Point(499, 299);
            _lblOrg.Name = "_lblOrg";
            _lblOrg.Size = new Size(144, 25);
            _lblOrg.TabIndex = 8;
            _lblOrg.Text = "Event Organizers";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Lexend Deca", 12F);
            label3.ForeColor = Color.FromArgb(247, 246, 237);
            label3.Location = new Point(107, 384);
            label3.Name = "label3";
            label3.Size = new Size(97, 25);
            label3.TabIndex = 7;
            label3.Text = "Event Date";
            // 
            // _txtDateHere
            // 
            _txtDateHere.BackColor = Color.FromArgb(58, 61, 116);
            _txtDateHere.BorderStyle = BorderStyle.None;
            _txtDateHere.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            _txtDateHere.ForeColor = Color.FromArgb(247, 246, 237);
            _txtDateHere.Location = new Point(107, 414);
            _txtDateHere.Name = "_txtDateHere";
            _txtDateHere.ReadOnly = true;
            _txtDateHere.Size = new Size(356, 20);
            _txtDateHere.TabIndex = 6;
            // 
            // _lblVen
            // 
            _lblVen.AutoSize = true;
            _lblVen.Font = new Font("Lexend Deca", 12F);
            _lblVen.ForeColor = Color.FromArgb(247, 246, 237);
            _lblVen.Location = new Point(107, 299);
            _lblVen.Name = "_lblVen";
            _lblVen.Size = new Size(108, 25);
            _lblVen.TabIndex = 4;
            _lblVen.Text = "Event Venue";
            // 
            // _txtDescriptionHere
            // 
            _txtDescriptionHere.BackColor = Color.FromArgb(58, 61, 116);
            _txtDescriptionHere.BorderStyle = BorderStyle.None;
            _txtDescriptionHere.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            _txtDescriptionHere.ForeColor = Color.FromArgb(247, 246, 237);
            _txtDescriptionHere.Location = new Point(107, 140);
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
            btnEvtNew.Location = new Point(420, 550);
            btnEvtNew.Name = "btnEvtNew";
            btnEvtNew.Size = new Size(124, 35);
            btnEvtNew.TabIndex = 1;
            btnEvtNew.Text = "Add New Event";
            btnEvtNew.UseVisualStyleBackColor = false;
            btnEvtNew.Click += btnEvtNew_Click;
            // 
            // _lblDesc
            // 
            _lblDesc.AutoSize = true;
            _lblDesc.Font = new Font("Lexend Deca", 12F);
            _lblDesc.ForeColor = Color.FromArgb(247, 246, 237);
            _lblDesc.Location = new Point(107, 110);
            _lblDesc.Name = "_lblDesc";
            _lblDesc.Size = new Size(100, 25);
            _lblDesc.TabIndex = 2;
            _lblDesc.Text = "Description";
            // 
            // _lblCurrentEvtName
            // 
            _lblCurrentEvtName.AutoSize = true;
            _lblCurrentEvtName.Font = new Font("Lexend Deca", 20F, FontStyle.Bold);
            _lblCurrentEvtName.ForeColor = Color.FromArgb(247, 246, 237);
            _lblCurrentEvtName.Location = new Point(25, 33);
            _lblCurrentEvtName.MinimumSize = new Size(920, 25);
            _lblCurrentEvtName.Name = "_lblCurrentEvtName";
            _lblCurrentEvtName.Size = new Size(920, 43);
            _lblCurrentEvtName.TabIndex = 0;
            _lblCurrentEvtName.Text = "Current Event Name";
            _lblCurrentEvtName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(58, 61, 116);
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            textBox1.ForeColor = Color.FromArgb(247, 246, 237);
            textBox1.Location = new Point(104, 138);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(745, 134);
            textBox1.TabIndex = 14;
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.FromArgb(58, 61, 116);
            textBox2.BorderStyle = BorderStyle.None;
            textBox2.Font = new Font("Lexend Deca SemiBold", 16F, FontStyle.Bold);
            textBox2.ForeColor = Color.FromArgb(247, 246, 237);
            textBox2.Location = new Point(104, 327);
            textBox2.Margin = new Padding(3, 0, 3, 0);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(361, 27);
            textBox2.TabIndex = 15;
            // 
            // textBox3
            // 
            textBox3.BackColor = Color.FromArgb(58, 61, 116);
            textBox3.BorderStyle = BorderStyle.None;
            textBox3.Font = new Font("Lexend Deca SemiBold", 16F, FontStyle.Bold);
            textBox3.ForeColor = Color.FromArgb(247, 246, 237);
            textBox3.Location = new Point(104, 412);
            textBox3.Margin = new Padding(3, 0, 3, 0);
            textBox3.Name = "textBox3";
            textBox3.ReadOnly = true;
            textBox3.Size = new Size(361, 27);
            textBox3.TabIndex = 16;
            // 
            // textBox4
            // 
            textBox4.BackColor = Color.FromArgb(58, 61, 116);
            textBox4.BorderStyle = BorderStyle.None;
            textBox4.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            textBox4.ForeColor = Color.FromArgb(247, 246, 237);
            textBox4.Location = new Point(496, 327);
            textBox4.Multiline = true;
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(353, 112);
            textBox4.TabIndex = 17;
            // 
            // PageEvents
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 23, 42);
            BackgroundImageLayout = ImageLayout.Stretch;
            Controls.Add(pnlMainEvents);
            Name = "PageEvents";
            Size = new Size(1524, 1041);
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
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
    }
}
