namespace NexScore.MainFormPages.DashboardForms
{
    partial class OpenEventForm
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
            btnSelectEvent = new Button();
            pnlListContainer = new Panel();
            _flowPanelEvents = new FlowLayoutPanel();
            pnlBottom = new Panel();
            pnlListContainer.SuspendLayout();
            pnlBottom.SuspendLayout();
            SuspendLayout();
            // 
            // btnSelectEvent
            // 
            btnSelectEvent.BackColor = Color.FromArgb(53, 55, 102);
            btnSelectEvent.FlatStyle = FlatStyle.Popup;
            btnSelectEvent.Font = new Font("Lexend Deca", 9F);
            btnSelectEvent.ForeColor = Color.FromArgb(247, 246, 237);
            btnSelectEvent.Location = new Point(507, 3);
            btnSelectEvent.Name = "btnSelectEvent";
            btnSelectEvent.Size = new Size(124, 29);
            btnSelectEvent.TabIndex = 1;
            btnSelectEvent.Text = "Select";
            btnSelectEvent.UseVisualStyleBackColor = false;
            // 
            // pnlListContainer
            // 
            pnlListContainer.AutoScroll = true;
            pnlListContainer.BackColor = Color.Transparent;
            pnlListContainer.Controls.Add(_flowPanelEvents);
            pnlListContainer.Dock = DockStyle.Fill;
            pnlListContainer.Location = new Point(25, 15);
            pnlListContainer.Name = "pnlListContainer";
            pnlListContainer.Size = new Size(666, 746);
            pnlListContainer.TabIndex = 1;
            // 
            // _flowPanelEvents
            // 
            _flowPanelEvents.AutoSize = true;
            _flowPanelEvents.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _flowPanelEvents.Dock = DockStyle.Top;
            _flowPanelEvents.FlowDirection = FlowDirection.TopDown;
            _flowPanelEvents.Location = new Point(0, 0);
            _flowPanelEvents.MinimumSize = new Size(634, 45);
            _flowPanelEvents.Name = "_flowPanelEvents";
            _flowPanelEvents.Size = new Size(666, 45);
            _flowPanelEvents.TabIndex = 0;
            _flowPanelEvents.WrapContents = false;
            // 
            // pnlBottom
            // 
            pnlBottom.BackColor = Color.Transparent;
            pnlBottom.Controls.Add(btnSelectEvent);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(25, 761);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(666, 35);
            pnlBottom.TabIndex = 2;
            // 
            // OpenEventForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 23, 42);
            ClientSize = new Size(716, 811);
            Controls.Add(pnlListContainer);
            Controls.Add(pnlBottom);
            Name = "OpenEventForm";
            Padding = new Padding(25, 15, 25, 15);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "NexScore";
            pnlListContainer.ResumeLayout(false);
            pnlListContainer.PerformLayout();
            pnlBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button btnSelectEvent;
        private Panel pnlListContainer;
        private FlowLayoutPanel _flowPanelEvents;
        private Panel pnlBottom;
        private Panel panel1;
    }
}