using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing; // Added
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NexScore.MainFormPages
{
    public partial class PageLogs : UserControl
    {
        // --- UI Fields ---
        private Panel _topBar;
        private Label _lblTitle;
        // -----------------

        public PageLogs()
        {
            InitializeComponent();
            InitializeTopBar();
        }

        private void InitializeTopBar()
        {
            _topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(58, 61, 116),
                Padding = new Padding(0)
            };

            _lblTitle = new Label
            {
                Text = "Logs",
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Lexend Deca", 13f, FontStyle.Bold),
                Location = new Point(18, 13)
            };

            _topBar.Controls.Add(_lblTitle);
            this.Controls.Add(_topBar);
            _topBar.BringToFront();
        }
    }
}