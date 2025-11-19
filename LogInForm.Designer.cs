namespace NexScore
{
    partial class LogInForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogInForm));
            pnlSetup = new Panel();
            btn_NewHide2 = new Button();
            btn_NewShow2 = new Button();
            btn_NewHide1 = new Button();
            btn_NewShow1 = new Button();
            btnSave = new Button();
            txtRePin = new TextBox();
            txtPin = new TextBox();
            lblSubtitle = new Label();
            lblTitle = new Label();
            pnlLogIn = new Panel();
            btn_EnterHide = new Button();
            btn_EnterShow = new Button();
            btnForgotPin = new Label();
            btnLogin = new Button();
            txtLoginPin = new TextBox();
            lblSubtitle1 = new Label();
            lblTitle1 = new Label();
            pnlSetup.SuspendLayout();
            pnlLogIn.SuspendLayout();
            SuspendLayout();
            // 
            // pnlSetup
            // 
            pnlSetup.BackColor = Color.FromArgb(15, 23, 42);
            pnlSetup.BackgroundImageLayout = ImageLayout.Stretch;
            pnlSetup.Controls.Add(btn_NewHide2);
            pnlSetup.Controls.Add(btn_NewShow2);
            pnlSetup.Controls.Add(btn_NewHide1);
            pnlSetup.Controls.Add(btn_NewShow1);
            pnlSetup.Controls.Add(btnSave);
            pnlSetup.Controls.Add(txtRePin);
            pnlSetup.Controls.Add(txtPin);
            pnlSetup.Controls.Add(lblSubtitle);
            pnlSetup.Controls.Add(lblTitle);
            pnlSetup.Dock = DockStyle.Fill;
            pnlSetup.Location = new Point(0, 0);
            pnlSetup.Name = "pnlSetup";
            pnlSetup.Size = new Size(1904, 1041);
            pnlSetup.TabIndex = 0;
            // 
            // btn_NewHide2
            // 
            btn_NewHide2.Anchor = AnchorStyles.None;
            btn_NewHide2.BackColor = SystemColors.ButtonHighlight;
            btn_NewHide2.BackgroundImage = (Image)resources.GetObject("btn_NewHide2.BackgroundImage");
            btn_NewHide2.BackgroundImageLayout = ImageLayout.Zoom;
            btn_NewHide2.FlatAppearance.BorderSize = 0;
            btn_NewHide2.FlatStyle = FlatStyle.Flat;
            btn_NewHide2.Location = new Point(984, 526);
            btn_NewHide2.Name = "btn_NewHide2";
            btn_NewHide2.Size = new Size(26, 17);
            btn_NewHide2.TabIndex = 11;
            btn_NewHide2.UseVisualStyleBackColor = false;
            btn_NewHide2.Click += btn_NewHide2_Click;
            // 
            // btn_NewShow2
            // 
            btn_NewShow2.Anchor = AnchorStyles.None;
            btn_NewShow2.BackColor = SystemColors.ButtonHighlight;
            btn_NewShow2.BackgroundImage = (Image)resources.GetObject("btn_NewShow2.BackgroundImage");
            btn_NewShow2.BackgroundImageLayout = ImageLayout.Zoom;
            btn_NewShow2.FlatAppearance.BorderSize = 0;
            btn_NewShow2.FlatStyle = FlatStyle.Flat;
            btn_NewShow2.Location = new Point(984, 526);
            btn_NewShow2.Name = "btn_NewShow2";
            btn_NewShow2.Size = new Size(26, 17);
            btn_NewShow2.TabIndex = 10;
            btn_NewShow2.UseVisualStyleBackColor = false;
            btn_NewShow2.Click += btn_NewShow2_Click;
            // 
            // btn_NewHide1
            // 
            btn_NewHide1.Anchor = AnchorStyles.None;
            btn_NewHide1.BackColor = SystemColors.ButtonHighlight;
            btn_NewHide1.BackgroundImage = (Image)resources.GetObject("btn_NewHide1.BackgroundImage");
            btn_NewHide1.BackgroundImageLayout = ImageLayout.Zoom;
            btn_NewHide1.FlatAppearance.BorderSize = 0;
            btn_NewHide1.FlatStyle = FlatStyle.Flat;
            btn_NewHide1.Location = new Point(984, 490);
            btn_NewHide1.Name = "btn_NewHide1";
            btn_NewHide1.Size = new Size(26, 17);
            btn_NewHide1.TabIndex = 9;
            btn_NewHide1.UseVisualStyleBackColor = false;
            btn_NewHide1.Click += btn_NewHide1_Click;
            // 
            // btn_NewShow1
            // 
            btn_NewShow1.Anchor = AnchorStyles.None;
            btn_NewShow1.BackColor = SystemColors.ButtonHighlight;
            btn_NewShow1.BackgroundImage = (Image)resources.GetObject("btn_NewShow1.BackgroundImage");
            btn_NewShow1.BackgroundImageLayout = ImageLayout.Zoom;
            btn_NewShow1.FlatAppearance.BorderSize = 0;
            btn_NewShow1.FlatStyle = FlatStyle.Flat;
            btn_NewShow1.Location = new Point(984, 490);
            btn_NewShow1.Name = "btn_NewShow1";
            btn_NewShow1.Size = new Size(26, 17);
            btn_NewShow1.TabIndex = 8;
            btn_NewShow1.UseVisualStyleBackColor = false;
            btn_NewShow1.Click += btn_NewShow1_Click;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.None;
            btnSave.BackColor = Color.FromArgb(53, 55, 102);
            btnSave.FlatStyle = FlatStyle.Popup;
            btnSave.Font = new Font("Lexend Deca Medium", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSave.ForeColor = Color.FromArgb(247, 246, 237);
            btnSave.Location = new Point(901, 559);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(86, 35);
            btnSave.TabIndex = 4;
            btnSave.Text = "Save PIN";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // txtRePin
            // 
            txtRePin.Anchor = AnchorStyles.None;
            txtRePin.Font = new Font("Lexend Deca", 12F);
            txtRePin.ForeColor = SystemColors.InactiveCaption;
            txtRePin.Location = new Point(874, 521);
            txtRePin.Margin = new Padding(1, 1, 1, 2);
            txtRePin.Name = "txtRePin";
            txtRePin.Size = new Size(136, 27);
            txtRePin.TabIndex = 3;
            txtRePin.TextAlign = HorizontalAlignment.Center;
            // 
            // txtPin
            // 
            txtPin.Anchor = AnchorStyles.None;
            txtPin.Font = new Font("Lexend Deca", 12F);
            txtPin.ForeColor = SystemColors.InactiveCaption;
            txtPin.Location = new Point(874, 485);
            txtPin.Margin = new Padding(1, 1, 1, 2);
            txtPin.Name = "txtPin";
            txtPin.Size = new Size(136, 27);
            txtPin.TabIndex = 2;
            txtPin.TextAlign = HorizontalAlignment.Center;
            // 
            // lblSubtitle
            // 
            lblSubtitle.Anchor = AnchorStyles.None;
            lblSubtitle.AutoSize = true;
            lblSubtitle.BackColor = Color.Transparent;
            lblSubtitle.Font = new Font("Lexend Deca Medium", 12F, FontStyle.Bold);
            lblSubtitle.ForeColor = Color.Snow;
            lblSubtitle.Location = new Point(827, 451);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(242, 25);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Let's set up your 6-digit PIN.";
            // 
            // lblTitle
            // 
            lblTitle.Anchor = AnchorStyles.None;
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Lexend Deca", 20F, FontStyle.Bold);
            lblTitle.ForeColor = SystemColors.ControlLightLight;
            lblTitle.Location = new Point(752, 396);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(395, 43);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Welcome, NexScore Admin!";
            // 
            // pnlLogIn
            // 
            pnlLogIn.BackgroundImage = Properties.Resources.bg_dark;
            pnlLogIn.BackgroundImageLayout = ImageLayout.Stretch;
            pnlLogIn.Controls.Add(btn_EnterHide);
            pnlLogIn.Controls.Add(btn_EnterShow);
            pnlLogIn.Controls.Add(btnForgotPin);
            pnlLogIn.Controls.Add(btnLogin);
            pnlLogIn.Controls.Add(txtLoginPin);
            pnlLogIn.Controls.Add(lblSubtitle1);
            pnlLogIn.Controls.Add(lblTitle1);
            pnlLogIn.Dock = DockStyle.Fill;
            pnlLogIn.Location = new Point(0, 0);
            pnlLogIn.Name = "pnlLogIn";
            pnlLogIn.Size = new Size(1904, 1041);
            pnlLogIn.TabIndex = 5;
            // 
            // btn_EnterHide
            // 
            btn_EnterHide.Anchor = AnchorStyles.None;
            btn_EnterHide.BackColor = SystemColors.ButtonHighlight;
            btn_EnterHide.BackgroundImage = (Image)resources.GetObject("btn_EnterHide.BackgroundImage");
            btn_EnterHide.BackgroundImageLayout = ImageLayout.Zoom;
            btn_EnterHide.FlatAppearance.BorderSize = 0;
            btn_EnterHide.FlatStyle = FlatStyle.Flat;
            btn_EnterHide.Location = new Point(996, 485);
            btn_EnterHide.Name = "btn_EnterHide";
            btn_EnterHide.Size = new Size(20, 17);
            btn_EnterHide.TabIndex = 7;
            btn_EnterHide.UseVisualStyleBackColor = false;
            btn_EnterHide.Click += btn_EnterHide_Click;
            // 
            // btn_EnterShow
            // 
            btn_EnterShow.Anchor = AnchorStyles.None;
            btn_EnterShow.BackColor = SystemColors.ButtonHighlight;
            btn_EnterShow.BackgroundImage = (Image)resources.GetObject("btn_EnterShow.BackgroundImage");
            btn_EnterShow.BackgroundImageLayout = ImageLayout.Zoom;
            btn_EnterShow.FlatAppearance.BorderSize = 0;
            btn_EnterShow.FlatStyle = FlatStyle.Flat;
            btn_EnterShow.Location = new Point(996, 485);
            btn_EnterShow.Name = "btn_EnterShow";
            btn_EnterShow.Size = new Size(20, 17);
            btn_EnterShow.TabIndex = 6;
            btn_EnterShow.UseVisualStyleBackColor = false;
            btn_EnterShow.Click += btn_EnterShow_Click;
            // 
            // btnForgotPin
            // 
            btnForgotPin.Anchor = AnchorStyles.None;
            btnForgotPin.AutoSize = true;
            btnForgotPin.BackColor = Color.Transparent;
            btnForgotPin.Font = new Font("Lexend Deca Light", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnForgotPin.ForeColor = SystemColors.Highlight;
            btnForgotPin.Location = new Point(916, 553);
            btnForgotPin.Name = "btnForgotPin";
            btnForgotPin.Size = new Size(76, 19);
            btnForgotPin.TabIndex = 5;
            btnForgotPin.Text = "Forgot PIN?";
            btnForgotPin.Click += btnForgotPin_Click;
            // 
            // btnLogin
            // 
            btnLogin.Anchor = AnchorStyles.None;
            btnLogin.BackColor = Color.FromArgb(53, 55, 102);
            btnLogin.FlatStyle = FlatStyle.Popup;
            btnLogin.Font = new Font("Lexend Deca Medium", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLogin.ForeColor = Color.FromArgb(247, 246, 237);
            btnLogin.Location = new Point(916, 523);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(75, 27);
            btnLogin.TabIndex = 4;
            btnLogin.Text = "Log In";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // txtLoginPin
            // 
            txtLoginPin.Anchor = AnchorStyles.None;
            txtLoginPin.Font = new Font("Lexend Deca", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtLoginPin.ForeColor = SystemColors.InactiveCaption;
            txtLoginPin.Location = new Point(883, 483);
            txtLoginPin.Margin = new Padding(1, 1, 1, 2);
            txtLoginPin.Name = "txtLoginPin";
            txtLoginPin.Size = new Size(135, 22);
            txtLoginPin.TabIndex = 2;
            txtLoginPin.TextAlign = HorizontalAlignment.Center;
            // 
            // lblSubtitle1
            // 
            lblSubtitle1.Anchor = AnchorStyles.None;
            lblSubtitle1.AutoSize = true;
            lblSubtitle1.BackColor = Color.Transparent;
            lblSubtitle1.Font = new Font("Lexend Deca Medium", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSubtitle1.ForeColor = SystemColors.ControlLightLight;
            lblSubtitle1.Location = new Point(874, 455);
            lblSubtitle1.Name = "lblSubtitle1";
            lblSubtitle1.Size = new Size(152, 21);
            lblSubtitle1.TabIndex = 1;
            lblSubtitle1.Text = "Enter your 6-digit PIN";
            // 
            // lblTitle1
            // 
            lblTitle1.Anchor = AnchorStyles.None;
            lblTitle1.AutoSize = true;
            lblTitle1.BackColor = Color.Transparent;
            lblTitle1.Font = new Font("Lexend Deca", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle1.ForeColor = SystemColors.ControlLightLight;
            lblTitle1.Location = new Point(783, 396);
            lblTitle1.Name = "lblTitle1";
            lblTitle1.Size = new Size(364, 33);
            lblTitle1.TabIndex = 0;
            lblTitle1.Text = "Welcome back, NexScore Admin!";
            // 
            // LogInForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1904, 1041);
            Controls.Add(pnlSetup);
            Controls.Add(pnlLogIn);
            MinimumSize = new Size(820, 480);
            Name = "LogInForm";
            Text = "NexScore";
            WindowState = FormWindowState.Maximized;
            pnlSetup.ResumeLayout(false);
            pnlSetup.PerformLayout();
            pnlLogIn.ResumeLayout(false);
            pnlLogIn.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private Panel pnlSetup;
        private Label lblTitle;
        private Label lblSubtitle;
        private TextBox txtPin;
        private Button btnSave;
        private TextBox txtRePin;
        private Panel pnlLogIn;
        private Button btnLogin;
        private TextBox txtLoginPin;
        private Label lblSubtitle1;
        private Label lblTitle1;
        private Label btnForgotPin;
        private Button btn_EnterShow;
        private Button btn_EnterHide;
        private Button btn_NewHide2;
        private Button btn_NewShow2;
        private Button btn_NewHide1;
        private Button btn_NewShow1;
    }
}