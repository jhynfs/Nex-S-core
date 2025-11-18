namespace NexScore.MainFormPages.DashboardForms.OpenEventControl
{
    partial class EventListItemControl
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
            _icon = new PictureBox();
            _lblName = new Label();
            _lblDate = new Label();
            _rightContainer = new Panel();
            ((System.ComponentModel.ISupportInitialize)_icon).BeginInit();
            _rightContainer.SuspendLayout();
            SuspendLayout();
            // 
            // _icon
            // 
            _icon.Dock = DockStyle.Left;
            _icon.Image = Properties.Resources.bxs_file;
            _icon.Location = new Point(10, 10);
            _icon.Name = "_icon";
            _icon.Size = new Size(40, 50);
            _icon.SizeMode = PictureBoxSizeMode.CenterImage;
            _icon.TabIndex = 0;
            _icon.TabStop = false;
            // 
            // _lblName
            // 
            _lblName.AutoSize = true;
            _lblName.Font = new Font("Lexend Deca", 9.75F, FontStyle.Bold);
            _lblName.ForeColor = Color.FromArgb(247, 246, 237);
            _lblName.Location = new Point(46, 4);
            _lblName.MinimumSize = new Size(550, 24);
            _lblName.Name = "_lblName";
            _lblName.Size = new Size(550, 24);
            _lblName.TabIndex = 1;
            _lblName.Text = "Event Name";
            // 
            // _lblDate
            // 
            _lblDate.AutoSize = true;
            _lblDate.Font = new Font("Lexend", 8.25F, FontStyle.Italic, GraphicsUnit.Point, 0);
            _lblDate.ForeColor = Color.FromArgb(247, 246, 237);
            _lblDate.Location = new Point(46, 28);
            _lblDate.MinimumSize = new Size(550, 17);
            _lblDate.Name = "_lblDate";
            _lblDate.Size = new Size(550, 17);
            _lblDate.TabIndex = 2;
            _lblDate.Text = "Date";
            // 
            // _rightContainer
            // 
            _rightContainer.Controls.Add(_lblDate);
            _rightContainer.Controls.Add(_lblName);
            _rightContainer.Dock = DockStyle.Fill;
            _rightContainer.Location = new Point(10, 10);
            _rightContainer.Name = "_rightContainer";
            _rightContainer.Size = new Size(607, 50);
            _rightContainer.TabIndex = 3;
            // 
            // EventListItemControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(35, 37, 70);
            Controls.Add(_icon);
            Controls.Add(_rightContainer);
            Name = "EventListItemControl";
            Padding = new Padding(10);
            Size = new Size(627, 70);
            ((System.ComponentModel.ISupportInitialize)_icon).EndInit();
            _rightContainer.ResumeLayout(false);
            _rightContainer.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox _icon;
        private Label _lblName;
        private Label _lblDate;
        private Panel _rightContainer;
    }
}
