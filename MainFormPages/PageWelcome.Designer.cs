namespace NexScore
{
    partial class PageWelcome
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
            _pnlNoEvent = new Panel();
            btnOpenEvent = new Button();
            btnNewEvent = new Button();
            _pnlEvent = new Panel();
            _pnlNoEvent.SuspendLayout();
            SuspendLayout();
            // 
            // _pnlNoEvent
            // 
            _pnlNoEvent.AutoSize = true;
            _pnlNoEvent.BackColor = Color.FromArgb(15, 23, 42);
            _pnlNoEvent.BackgroundImageLayout = ImageLayout.Stretch;
            _pnlNoEvent.Controls.Add(btnOpenEvent);
            _pnlNoEvent.Controls.Add(btnNewEvent);
            _pnlNoEvent.Dock = DockStyle.Fill;
            _pnlNoEvent.Location = new Point(0, 0);
            _pnlNoEvent.Name = "_pnlNoEvent";
            _pnlNoEvent.Size = new Size(1524, 1041);
            _pnlNoEvent.TabIndex = 0;
            // 
            // btnOpenEvent
            // 
            btnOpenEvent.Anchor = AnchorStyles.None;
            btnOpenEvent.BackColor = Color.FromArgb(53, 55, 102);
            btnOpenEvent.FlatStyle = FlatStyle.Popup;
            btnOpenEvent.Font = new Font("Lexend Deca Medium", 12F, FontStyle.Bold);
            btnOpenEvent.ForeColor = Color.FromArgb(247, 246, 237);
            btnOpenEvent.Image = Properties.Resources.file;
            btnOpenEvent.ImageAlign = ContentAlignment.MiddleLeft;
            btnOpenEvent.Location = new Point(622, 521);
            btnOpenEvent.Name = "btnOpenEvent";
            btnOpenEvent.Size = new Size(237, 45);
            btnOpenEvent.TabIndex = 1;
            btnOpenEvent.Text = "Open Event";
            btnOpenEvent.UseVisualStyleBackColor = false;
            btnOpenEvent.Click += btnOpenEvent_Click;
            // 
            // btnNewEvent
            // 
            btnNewEvent.Anchor = AnchorStyles.None;
            btnNewEvent.BackColor = Color.FromArgb(53, 55, 102);
            btnNewEvent.FlatStyle = FlatStyle.Popup;
            btnNewEvent.Font = new Font("Lexend Deca Medium", 12F, FontStyle.Bold);
            btnNewEvent.ForeColor = Color.FromArgb(247, 246, 237);
            btnNewEvent.Image = Properties.Resources.plus;
            btnNewEvent.ImageAlign = ContentAlignment.MiddleLeft;
            btnNewEvent.Location = new Point(622, 455);
            btnNewEvent.Name = "btnNewEvent";
            btnNewEvent.Size = new Size(237, 45);
            btnNewEvent.TabIndex = 0;
            btnNewEvent.Text = "New Event";
            btnNewEvent.UseVisualStyleBackColor = false;
            btnNewEvent.Click += btnNewEvent_Click;
            // 
            // _pnlEvent
            // 
            _pnlEvent.BackColor = Color.FromArgb(15, 23, 42);
            _pnlEvent.BackgroundImageLayout = ImageLayout.Stretch;
            _pnlEvent.Dock = DockStyle.Fill;
            _pnlEvent.Location = new Point(0, 0);
            _pnlEvent.Name = "_pnlEvent";
            _pnlEvent.Size = new Size(1524, 1041);
            _pnlEvent.TabIndex = 2;
            // 
            // PageDashboard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            Controls.Add(_pnlNoEvent);
            Controls.Add(_pnlEvent);
            Name = "PageDashboard";
            Size = new Size(1524, 1041);
            _pnlNoEvent.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel _pnlNoEvent;
        private Button btnNewEvent;
        private Button btnOpenEvent;
        private Panel _pnlEvent;
    }
}
