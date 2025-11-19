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
            pictureBox1 = new PictureBox();
            btnMenu = new Button();
            Separator1 = new Panel();
            btnDashboard = new Button();
            Separator2 = new Panel();
            pnlSidebar = new Panel();
            btnLogs = new Button();
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
            Separator3 = new Panel();
            btnLiveControl = new Button();
            pnlHeading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            pnlSidebar.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMainContent
            // 
            pnlMainContent.AutoSize = true;
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
            pnlHeading.Controls.Add(pictureBox1);
            pnlHeading.Dock = DockStyle.Top;
            pnlHeading.Location = new Point(5, 5);
            pnlHeading.Name = "pnlHeading";
            pnlHeading.Size = new Size(370, 47);
            pnlHeading.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(370, 47);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
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
            btnDashboard.BackColor = Color.FromArgb(53, 55, 102);
            btnDashboard.Dock = DockStyle.Top;
            btnDashboard.FlatAppearance.BorderSize = 0;
            btnDashboard.FlatStyle = FlatStyle.Flat;
            btnDashboard.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            btnDashboard.ForeColor = Color.FromArgb(247, 246, 237);
            btnDashboard.Image = Properties.Resources.material_symbols_dashboard_rounded;
            btnDashboard.ImageAlign = ContentAlignment.MiddleLeft;
            btnDashboard.Location = new Point(5, 87);
            btnDashboard.Name = "btnDashboard";
            btnDashboard.Size = new Size(370, 45);
            btnDashboard.TabIndex = 11;
            btnDashboard.Tag = "Dashboard";
            btnDashboard.Text = "Dashboard";
            btnDashboard.UseVisualStyleBackColor = false;
            btnDashboard.Click += btnDashboard_Click;
            // 
            // Separator2
            // 
            Separator2.BackColor = Color.FromArgb(23, 23, 23);
            Separator2.Dock = DockStyle.Top;
            Separator2.Location = new Point(5, 132);
            Separator2.Name = "Separator2";
            Separator2.Size = new Size(370, 5);
            Separator2.TabIndex = 12;
            // 
            // pnlSidebar
            // 
            pnlSidebar.BackColor = Color.FromArgb(23, 23, 23);
            pnlSidebar.Controls.Add(btnLogs);
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
            pnlSidebar.Controls.Add(Separator3);
            pnlSidebar.Controls.Add(btnLiveControl);
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
            // btnLogs
            // 
            btnLogs.BackColor = Color.FromArgb(55, 53, 105);
            btnLogs.Dock = DockStyle.Top;
            btnLogs.FlatAppearance.BorderSize = 0;
            btnLogs.FlatStyle = FlatStyle.Flat;
            btnLogs.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            btnLogs.ForeColor = Color.FromArgb(247, 246, 237);
            btnLogs.Image = Properties.Resources.tabler_logs;
            btnLogs.ImageAlign = ContentAlignment.MiddleLeft;
            btnLogs.Location = new Point(5, 437);
            btnLogs.Name = "btnLogs";
            btnLogs.Size = new Size(370, 45);
            btnLogs.TabIndex = 26;
            btnLogs.Tag = "Logs";
            btnLogs.Text = "Logs";
            btnLogs.UseVisualStyleBackColor = false;
            btnLogs.Click += btnLogs_Click;
            // 
            // Separator8
            // 
            Separator8.BackColor = Color.FromArgb(23, 23, 23);
            Separator8.Dock = DockStyle.Top;
            Separator8.Location = new Point(5, 432);
            Separator8.Name = "Separator8";
            Separator8.Size = new Size(370, 5);
            Separator8.TabIndex = 24;
            // 
            // btnResults
            // 
            btnResults.BackColor = Color.FromArgb(55, 53, 105);
            btnResults.Dock = DockStyle.Top;
            btnResults.FlatAppearance.BorderSize = 0;
            btnResults.FlatStyle = FlatStyle.Flat;
            btnResults.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            btnResults.ForeColor = Color.FromArgb(247, 246, 237);
            btnResults.Image = Properties.Resources.counter;
            btnResults.ImageAlign = ContentAlignment.MiddleLeft;
            btnResults.Location = new Point(5, 387);
            btnResults.Name = "btnResults";
            btnResults.Size = new Size(370, 45);
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
            Separator7.Location = new Point(5, 382);
            Separator7.Name = "Separator7";
            Separator7.Size = new Size(370, 5);
            Separator7.TabIndex = 22;
            // 
            // btnJudges
            // 
            btnJudges.BackColor = Color.FromArgb(55, 53, 105);
            btnJudges.Dock = DockStyle.Top;
            btnJudges.FlatAppearance.BorderSize = 0;
            btnJudges.FlatStyle = FlatStyle.Flat;
            btnJudges.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            btnJudges.ForeColor = Color.FromArgb(247, 246, 237);
            btnJudges.Image = Properties.Resources.people;
            btnJudges.ImageAlign = ContentAlignment.MiddleLeft;
            btnJudges.Location = new Point(5, 337);
            btnJudges.Name = "btnJudges";
            btnJudges.Size = new Size(370, 45);
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
            Separator6.Location = new Point(5, 332);
            Separator6.Name = "Separator6";
            Separator6.Size = new Size(370, 5);
            Separator6.TabIndex = 20;
            // 
            // btnCriteria
            // 
            btnCriteria.BackColor = Color.FromArgb(55, 53, 105);
            btnCriteria.Dock = DockStyle.Top;
            btnCriteria.FlatAppearance.BorderSize = 0;
            btnCriteria.FlatStyle = FlatStyle.Flat;
            btnCriteria.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            btnCriteria.ForeColor = Color.FromArgb(247, 246, 237);
            btnCriteria.Image = Properties.Resources.checklist;
            btnCriteria.ImageAlign = ContentAlignment.MiddleLeft;
            btnCriteria.Location = new Point(5, 287);
            btnCriteria.Name = "btnCriteria";
            btnCriteria.Size = new Size(370, 45);
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
            Separator5.Location = new Point(5, 282);
            Separator5.Name = "Separator5";
            Separator5.Size = new Size(370, 5);
            Separator5.TabIndex = 18;
            // 
            // btnContestants
            // 
            btnContestants.BackColor = Color.FromArgb(55, 53, 105);
            btnContestants.Dock = DockStyle.Top;
            btnContestants.FlatAppearance.BorderSize = 0;
            btnContestants.FlatStyle = FlatStyle.Flat;
            btnContestants.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            btnContestants.ForeColor = Color.FromArgb(247, 246, 237);
            btnContestants.Image = Properties.Resources.crown;
            btnContestants.ImageAlign = ContentAlignment.MiddleLeft;
            btnContestants.Location = new Point(5, 237);
            btnContestants.Name = "btnContestants";
            btnContestants.Size = new Size(370, 45);
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
            Separator4.Location = new Point(5, 232);
            Separator4.Name = "Separator4";
            Separator4.Size = new Size(370, 5);
            Separator4.TabIndex = 16;
            // 
            // btnEvents
            // 
            btnEvents.BackColor = Color.FromArgb(55, 53, 105);
            btnEvents.Dock = DockStyle.Top;
            btnEvents.FlatAppearance.BorderSize = 0;
            btnEvents.FlatStyle = FlatStyle.Flat;
            btnEvents.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            btnEvents.ForeColor = Color.FromArgb(247, 246, 237);
            btnEvents.Image = Properties.Resources._event;
            btnEvents.ImageAlign = ContentAlignment.MiddleLeft;
            btnEvents.Location = new Point(5, 187);
            btnEvents.Name = "btnEvents";
            btnEvents.Size = new Size(370, 45);
            btnEvents.TabIndex = 15;
            btnEvents.Tag = "Event Details";
            btnEvents.Text = "Event Details";
            btnEvents.UseVisualStyleBackColor = false;
            btnEvents.Click += btnEvents_Click;
            // 
            // Separator3
            // 
            Separator3.BackColor = Color.FromArgb(23, 23, 23);
            Separator3.Dock = DockStyle.Top;
            Separator3.Location = new Point(5, 182);
            Separator3.Name = "Separator3";
            Separator3.Size = new Size(370, 5);
            Separator3.TabIndex = 14;
            // 
            // btnLiveControl
            // 
            btnLiveControl.BackColor = Color.FromArgb(55, 53, 105);
            btnLiveControl.Dock = DockStyle.Top;
            btnLiveControl.FlatAppearance.BorderSize = 0;
            btnLiveControl.FlatStyle = FlatStyle.Flat;
            btnLiveControl.Font = new Font("Lexend Deca SemiBold", 12F, FontStyle.Bold);
            btnLiveControl.ForeColor = Color.FromArgb(247, 246, 237);
            btnLiveControl.Image = Properties.Resources.live;
            btnLiveControl.ImageAlign = ContentAlignment.MiddleLeft;
            btnLiveControl.Location = new Point(5, 137);
            btnLiveControl.Name = "btnLiveControl";
            btnLiveControl.Size = new Size(370, 45);
            btnLiveControl.TabIndex = 13;
            btnLiveControl.Tag = "Live Control";
            btnLiveControl.Text = "Live Control";
            btnLiveControl.UseVisualStyleBackColor = false;
            btnLiveControl.Click += btnLiveControl_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            ClientSize = new Size(1904, 1041);
            Controls.Add(pnlMainContent);
            Controls.Add(pnlSidebar);
            MinimumSize = new Size(854, 480);
            Name = "MainForm";
            Text = "NexScore";
            WindowState = FormWindowState.Maximized;
            pnlHeading.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            pnlSidebar.ResumeLayout(false);
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
        private Panel Separator3;
        private Button btnLiveControl;
        private PictureBox pictureBox1;
        private Button btnResults;
        private Panel Separator8;
        private Button btnLogs;
    }
}