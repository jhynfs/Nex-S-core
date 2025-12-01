namespace NexScore
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            pnlMainContent = new Panel();
            pnlHeading = new Panel();
            logoShort = new PictureBox();
            logoLong = new PictureBox();
            btnMenu = new Button();
            Separator1 = new Panel();
            btnDashboard = new Button();
            Separator2 = new Panel();
            pnlSidebar = new Panel();
            _adminPanel = new Panel();
            _btnSaveAdminBaseUrl = new Button();
            _btnUseMyIp = new Button();
            _txtAdminBaseUrl = new TextBox();
            _lblAdminBaseUrl = new Label();
            btnScorecards = new Button();
            Separator8 = new Panel();
            btnResults = new Button();
            Separator7 = new Panel();
            btnJudges = new Button();
            Separator6 = new Panel();
            btnCriteria = new Button();
            Separator5 = new Panel();
            btnContestants = new Button();
            Separator4 = new Panel();
            btnEvents = new Button();
            pnlHeading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logoShort).BeginInit();
            ((System.ComponentModel.ISupportInitialize)logoLong).BeginInit();
            pnlSidebar.SuspendLayout();
            _adminPanel.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMainContent
            // 
            pnlMainContent.AutoSize = true;
            pnlMainContent.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlMainContent.BackColor = Color.FromArgb(15, 23, 42);
            pnlMainContent.BackgroundImageLayout = ImageLayout.Stretch;
            pnlMainContent.Dock = DockStyle.Fill;
            pnlMainContent.Location = new Point(380, 0);
            pnlMainContent.Name = "pnlMainContent";
            pnlMainContent.Size = new Size(1524, 1041);
            pnlMainContent.TabIndex = 1;
            // 
            // pnlHeading
            // 
            pnlHeading.Controls.Add(logoShort);
            pnlHeading.Controls.Add(logoLong);
            pnlHeading.Dock = DockStyle.Top;
            pnlHeading.Location = new Point(5, 5);
            pnlHeading.Name = "pnlHeading";
            pnlHeading.Size = new Size(370, 47);
            pnlHeading.TabIndex = 0;
            // 
            // logoShort
            // 
            logoShort.Image = (Image)resources.GetObject("logoShort.Image");
            logoShort.InitialImage = (Image)resources.GetObject("logoShort.InitialImage");
            logoShort.Location = new Point(0, 0);
            logoShort.Name = "logoShort";
            logoShort.Size = new Size(47, 47);
            logoShort.SizeMode = PictureBoxSizeMode.Zoom;
            logoShort.TabIndex = 28;
            logoShort.TabStop = false;
            // 
            // logoLong
            // 
            logoLong.BackgroundImage = (Image)resources.GetObject("logoLong.BackgroundImage");
            logoLong.BackgroundImageLayout = ImageLayout.Zoom;
            logoLong.Dock = DockStyle.Fill;
            logoLong.ErrorImage = null;
            logoLong.InitialImage = (Image)resources.GetObject("logoLong.InitialImage");
            logoLong.Location = new Point(0, 0);
            logoLong.Name = "logoLong";
            logoLong.Size = new Size(370, 47);
            logoLong.TabIndex = 0;
            logoLong.TabStop = false;
            // 
            // btnMenu
            // 
            btnMenu.BackColor = Color.FromArgb(23, 23, 23);
            btnMenu.Dock = DockStyle.Top;
            btnMenu.FlatAppearance.BorderSize = 0;
            btnMenu.FlatStyle = FlatStyle.Flat;
            btnMenu.ForeColor = Color.FromArgb(247, 246, 237);
            btnMenu.Location = new Point(5, 52);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(370, 30);
            btnMenu.TabIndex = 1;
            btnMenu.TextAlign = ContentAlignment.MiddleRight;
            btnMenu.UseVisualStyleBackColor = false;
            btnMenu.Click += btnMenu_Click;
            // 
            // Separator1
            // 
            Separator1.BackColor = Color.FromArgb(23, 23, 23);
            Separator1.Dock = DockStyle.Top;
            Separator1.Location = new Point(5, 82);
            Separator1.Name = "Separator1";
            Separator1.Size = new Size(370, 5);
            Separator1.TabIndex = 10;
            // 
            // btnDashboard
            // 
            btnDashboard.AutoSize = true;
            btnDashboard.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnDashboard.BackColor = Color.FromArgb(53, 55, 102);
            btnDashboard.Dock = DockStyle.Top;
            btnDashboard.FlatAppearance.BorderSize = 0;
            btnDashboard.FlatStyle = FlatStyle.Flat;
            btnDashboard.Font = new Font("Lexend Deca SemiBold", 20F, FontStyle.Bold);
            btnDashboard.ForeColor = Color.FromArgb(247, 246, 237);
            btnDashboard.Image = Properties.Resources.material_symbols_dashboard_rounded;
            btnDashboard.ImageAlign = ContentAlignment.MiddleLeft;
            btnDashboard.Location = new Point(5, 87);
            btnDashboard.Name = "btnDashboard";
            btnDashboard.Padding = new Padding(5, 0, 5, 0);
            btnDashboard.Size = new Size(370, 53);
            btnDashboard.TabIndex = 11;
            btnDashboard.Tag = "Welcome";
            btnDashboard.Text = "Welcome";
            btnDashboard.UseVisualStyleBackColor = false;
            btnDashboard.Click += btnDashboard_Click;
            // 
            // Separator2
            // 
            Separator2.BackColor = Color.FromArgb(23, 23, 23);
            Separator2.Dock = DockStyle.Top;
            Separator2.Location = new Point(5, 140);
            Separator2.Name = "Separator2";
            Separator2.Size = new Size(370, 5);
            Separator2.TabIndex = 12;
            // 
            // pnlSidebar
            // 
            pnlSidebar.BackColor = Color.FromArgb(23, 23, 23);
            pnlSidebar.Controls.Add(_adminPanel);
            pnlSidebar.Controls.Add(btnScorecards);
            pnlSidebar.Controls.Add(Separator8);
            pnlSidebar.Controls.Add(btnResults);
            pnlSidebar.Controls.Add(Separator7);
            pnlSidebar.Controls.Add(btnJudges);
            pnlSidebar.Controls.Add(Separator6);
            pnlSidebar.Controls.Add(btnCriteria);
            pnlSidebar.Controls.Add(Separator5);
            pnlSidebar.Controls.Add(btnContestants);
            pnlSidebar.Controls.Add(Separator4);
            pnlSidebar.Controls.Add(btnEvents);
            pnlSidebar.Controls.Add(Separator2);
            pnlSidebar.Controls.Add(btnDashboard);
            pnlSidebar.Controls.Add(Separator1);
            pnlSidebar.Controls.Add(btnMenu);
            pnlSidebar.Controls.Add(pnlHeading);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Location = new Point(0, 0);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Padding = new Padding(5);
            pnlSidebar.Size = new Size(380, 1041);
            pnlSidebar.TabIndex = 0;
            // 
            // _adminPanel
            // 
            _adminPanel.Controls.Add(_btnSaveAdminBaseUrl);
            _adminPanel.Controls.Add(_btnUseMyIp);
            _adminPanel.Controls.Add(_txtAdminBaseUrl);
            _adminPanel.Controls.Add(_lblAdminBaseUrl);
            _adminPanel.Dock = DockStyle.Bottom;
            _adminPanel.Location = new Point(5, 936);
            _adminPanel.Name = "_adminPanel";
            _adminPanel.Padding = new Padding(10);
            _adminPanel.Size = new Size(370, 100);
            _adminPanel.TabIndex = 27;
            // 
            // _btnSaveAdminBaseUrl
            // 
            _btnSaveAdminBaseUrl.BackColor = Color.FromArgb(55, 53, 105);
            _btnSaveAdminBaseUrl.FlatStyle = FlatStyle.Popup;
            _btnSaveAdminBaseUrl.Font = new Font("Lexend Deca Medium", 9.75F, FontStyle.Bold);
            _btnSaveAdminBaseUrl.ForeColor = Color.FromArgb(247, 246, 237);
            _btnSaveAdminBaseUrl.Location = new Point(182, 24);
            _btnSaveAdminBaseUrl.Name = "_btnSaveAdminBaseUrl";
            _btnSaveAdminBaseUrl.Size = new Size(75, 32);
            _btnSaveAdminBaseUrl.TabIndex = 3;
            _btnSaveAdminBaseUrl.Text = "Save";
            _btnSaveAdminBaseUrl.UseVisualStyleBackColor = false;
            // 
            // _btnUseMyIp
            // 
            _btnUseMyIp.BackColor = Color.FromArgb(55, 53, 105);
            _btnUseMyIp.FlatStyle = FlatStyle.Popup;
            _btnUseMyIp.Font = new Font("Lexend Deca Medium", 9.75F, FontStyle.Bold);
            _btnUseMyIp.ForeColor = Color.FromArgb(247, 246, 237);
            _btnUseMyIp.Location = new Point(263, 24);
            _btnUseMyIp.Name = "_btnUseMyIp";
            _btnUseMyIp.Size = new Size(94, 32);
            _btnUseMyIp.TabIndex = 2;
            _btnUseMyIp.Text = "Use My IP";
            _btnUseMyIp.UseVisualStyleBackColor = false;
            // 
            // _txtAdminBaseUrl
            // 
            _txtAdminBaseUrl.Font = new Font("Lexend Deca Medium", 9.75F, FontStyle.Bold);
            _txtAdminBaseUrl.Location = new Point(81, 63);
            _txtAdminBaseUrl.Name = "_txtAdminBaseUrl";
            _txtAdminBaseUrl.Size = new Size(276, 24);
            _txtAdminBaseUrl.TabIndex = 1;
            // 
            // _lblAdminBaseUrl
            // 
            _lblAdminBaseUrl.AutoSize = true;
            _lblAdminBaseUrl.Font = new Font("Lexend Deca Medium", 9.75F, FontStyle.Bold);
            _lblAdminBaseUrl.ForeColor = Color.FromArgb(247, 246, 237);
            _lblAdminBaseUrl.Location = new Point(81, 30);
            _lblAdminBaseUrl.Name = "_lblAdminBaseUrl";
            _lblAdminBaseUrl.Size = new Size(98, 21);
            _lblAdminBaseUrl.TabIndex = 0;
            _lblAdminBaseUrl.Text = "Admin's IPv4:";
            // 
            // btnScorecards
            // 
            btnScorecards.AutoSize = true;
            btnScorecards.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnScorecards.BackColor = Color.FromArgb(55, 53, 105);
            btnScorecards.Dock = DockStyle.Top;
            btnScorecards.FlatAppearance.BorderSize = 0;
            btnScorecards.FlatStyle = FlatStyle.Flat;
            btnScorecards.Font = new Font("Lexend Deca SemiBold", 20F, FontStyle.Bold);
            btnScorecards.ForeColor = Color.FromArgb(247, 246, 237);
            btnScorecards.Image = Properties.Resources.tabler_logs;
            btnScorecards.ImageAlign = ContentAlignment.MiddleLeft;
            btnScorecards.Location = new Point(5, 435);
            btnScorecards.Name = "btnScorecards";
            btnScorecards.Padding = new Padding(5, 0, 5, 0);
            btnScorecards.Size = new Size(370, 53);
            btnScorecards.TabIndex = 26;
            btnScorecards.Tag = "Scorecards";
            btnScorecards.Text = "Scorecards";
            btnScorecards.UseVisualStyleBackColor = false;
            btnScorecards.Click += btnScorecards_Click;
            // 
            // Separator8
            // 
            Separator8.BackColor = Color.FromArgb(23, 23, 23);
            Separator8.Dock = DockStyle.Top;
            Separator8.Location = new Point(5, 430);
            Separator8.Name = "Separator8";
            Separator8.Size = new Size(370, 5);
            Separator8.TabIndex = 24;
            // 
            // btnResults
            // 
            btnResults.AutoSize = true;
            btnResults.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnResults.BackColor = Color.FromArgb(55, 53, 105);
            btnResults.Dock = DockStyle.Top;
            btnResults.FlatAppearance.BorderSize = 0;
            btnResults.FlatStyle = FlatStyle.Flat;
            btnResults.Font = new Font("Lexend Deca SemiBold", 20F, FontStyle.Bold);
            btnResults.ForeColor = Color.FromArgb(247, 246, 237);
            btnResults.Image = Properties.Resources.counter;
            btnResults.ImageAlign = ContentAlignment.MiddleLeft;
            btnResults.Location = new Point(5, 377);
            btnResults.Name = "btnResults";
            btnResults.Padding = new Padding(5, 0, 5, 0);
            btnResults.Size = new Size(370, 53);
            btnResults.TabIndex = 23;
            btnResults.Tag = "Results";
            btnResults.Text = "Results";
            btnResults.UseVisualStyleBackColor = false;
            btnResults.Click += btnResults_Click;
            // 
            // Separator7
            // 
            Separator7.BackColor = Color.FromArgb(23, 23, 23);
            Separator7.Dock = DockStyle.Top;
            Separator7.Location = new Point(5, 372);
            Separator7.Name = "Separator7";
            Separator7.Size = new Size(370, 5);
            Separator7.TabIndex = 22;
            // 
            // btnJudges
            // 
            btnJudges.AutoSize = true;
            btnJudges.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnJudges.BackColor = Color.FromArgb(55, 53, 105);
            btnJudges.Dock = DockStyle.Top;
            btnJudges.FlatAppearance.BorderSize = 0;
            btnJudges.FlatStyle = FlatStyle.Flat;
            btnJudges.Font = new Font("Lexend Deca SemiBold", 20F, FontStyle.Bold);
            btnJudges.ForeColor = Color.FromArgb(247, 246, 237);
            btnJudges.Image = Properties.Resources.people;
            btnJudges.ImageAlign = ContentAlignment.MiddleLeft;
            btnJudges.Location = new Point(5, 319);
            btnJudges.Name = "btnJudges";
            btnJudges.Padding = new Padding(5, 0, 5, 0);
            btnJudges.Size = new Size(370, 53);
            btnJudges.TabIndex = 21;
            btnJudges.Tag = "Judges";
            btnJudges.Text = "Judges";
            btnJudges.UseVisualStyleBackColor = false;
            btnJudges.Click += btnJudges_Click;
            // 
            // Separator6
            // 
            Separator6.BackColor = Color.FromArgb(23, 23, 23);
            Separator6.Dock = DockStyle.Top;
            Separator6.Location = new Point(5, 314);
            Separator6.Name = "Separator6";
            Separator6.Size = new Size(370, 5);
            Separator6.TabIndex = 20;
            // 
            // btnCriteria
            // 
            btnCriteria.AutoSize = true;
            btnCriteria.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnCriteria.BackColor = Color.FromArgb(55, 53, 105);
            btnCriteria.Dock = DockStyle.Top;
            btnCriteria.FlatAppearance.BorderSize = 0;
            btnCriteria.FlatStyle = FlatStyle.Flat;
            btnCriteria.Font = new Font("Lexend Deca SemiBold", 20F, FontStyle.Bold);
            btnCriteria.ForeColor = Color.FromArgb(247, 246, 237);
            btnCriteria.Image = Properties.Resources.checklist;
            btnCriteria.ImageAlign = ContentAlignment.MiddleLeft;
            btnCriteria.Location = new Point(5, 261);
            btnCriteria.Name = "btnCriteria";
            btnCriteria.Padding = new Padding(5, 0, 5, 0);
            btnCriteria.Size = new Size(370, 53);
            btnCriteria.TabIndex = 19;
            btnCriteria.Tag = "Phases && Criteria";
            btnCriteria.Text = "Phases && Criteria";
            btnCriteria.UseVisualStyleBackColor = false;
            btnCriteria.Click += btnCriteria_Click;
            // 
            // Separator5
            // 
            Separator5.BackColor = Color.FromArgb(23, 23, 23);
            Separator5.Dock = DockStyle.Top;
            Separator5.Location = new Point(5, 256);
            Separator5.Name = "Separator5";
            Separator5.Size = new Size(370, 5);
            Separator5.TabIndex = 18;
            // 
            // btnContestants
            // 
            btnContestants.AutoSize = true;
            btnContestants.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnContestants.BackColor = Color.FromArgb(55, 53, 105);
            btnContestants.Dock = DockStyle.Top;
            btnContestants.FlatAppearance.BorderSize = 0;
            btnContestants.FlatStyle = FlatStyle.Flat;
            btnContestants.Font = new Font("Lexend Deca SemiBold", 20F, FontStyle.Bold);
            btnContestants.ForeColor = Color.FromArgb(247, 246, 237);
            btnContestants.Image = Properties.Resources.crown;
            btnContestants.ImageAlign = ContentAlignment.MiddleLeft;
            btnContestants.Location = new Point(5, 203);
            btnContestants.Name = "btnContestants";
            btnContestants.Padding = new Padding(5, 0, 5, 0);
            btnContestants.Size = new Size(370, 53);
            btnContestants.TabIndex = 17;
            btnContestants.Tag = "Contestants";
            btnContestants.Text = "Contestants";
            btnContestants.UseVisualStyleBackColor = false;
            btnContestants.Click += btnContestants_Click;
            // 
            // Separator4
            // 
            Separator4.BackColor = Color.FromArgb(23, 23, 23);
            Separator4.Dock = DockStyle.Top;
            Separator4.Location = new Point(5, 198);
            Separator4.Name = "Separator4";
            Separator4.Size = new Size(370, 5);
            Separator4.TabIndex = 16;
            // 
            // btnEvents
            // 
            btnEvents.AutoSize = true;
            btnEvents.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnEvents.BackColor = Color.FromArgb(55, 53, 105);
            btnEvents.Dock = DockStyle.Top;
            btnEvents.FlatAppearance.BorderSize = 0;
            btnEvents.FlatStyle = FlatStyle.Flat;
            btnEvents.Font = new Font("Lexend Deca SemiBold", 20F, FontStyle.Bold);
            btnEvents.ForeColor = Color.FromArgb(247, 246, 237);
            btnEvents.Image = Properties.Resources._event;
            btnEvents.ImageAlign = ContentAlignment.MiddleLeft;
            btnEvents.Location = new Point(5, 145);
            btnEvents.Name = "btnEvents";
            btnEvents.Padding = new Padding(5, 0, 5, 0);
            btnEvents.Size = new Size(370, 53);
            btnEvents.TabIndex = 15;
            btnEvents.Tag = "Event Details";
            btnEvents.Text = "Event Details";
            btnEvents.UseVisualStyleBackColor = false;
            btnEvents.Click += btnEvents_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            AutoSize = true;
            ClientSize = new Size(1904, 1041);
            Controls.Add(pnlMainContent);
            Controls.Add(pnlSidebar);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1000, 480);
            Name = "MainForm";
            Text = "NexScore";
            WindowState = FormWindowState.Maximized;
            pnlHeading.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)logoShort).EndInit();
            ((System.ComponentModel.ISupportInitialize)logoLong).EndInit();
            pnlSidebar.ResumeLayout(false);
            pnlSidebar.PerformLayout();
            _adminPanel.ResumeLayout(false);
            _adminPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel pnlMainContent;
        private Panel pnlHeading;
        private Button btnMenu;
        private Panel Separator1;
        private Button btnDashboard;
        private Panel Separator2;
        private Panel pnlSidebar;
        private Panel Separator7;
        private Button btnJudges;
        private Panel Separator6;
        private Button btnCriteria;
        private Panel Separator5;
        private Button btnContestants;
        private Panel Separator4;
        private Button btnEvents;
        private PictureBox pictureBox1;
        private Button btnResults;
        private Panel Separator8;
        private Button btnScorecards;
        private Panel _adminPanel;
        private Label _lblAdminBaseUrl;
        private TextBox _txtAdminBaseUrl;
        private Button _btnUseMyIp;
        private Button _btnSaveAdminBaseUrl;
        private PictureBox logoShort;
        private PictureBox logoLong;
    }
}