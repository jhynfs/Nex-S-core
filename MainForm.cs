using NexScore.MainFormPages;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace NexScore
{
    public partial class MainForm : Form
    {
        private PageWelcome dashboardPage;
        private PageEvents eventsPage;
        private PageContestants contestantsPage;
        private PageCriteria criteriaPage;
        private PageJudges judgesPage;
        private PageResults resultsPage;
        private PageScorecards logsPage;

        // ---------------- Color palette ----------------
        private Color baseColor = ColorTranslator.FromHtml("#353769");
        private Color hoverColor = ColorTranslator.FromHtml("#4E537A");
        private Color clickColor = ColorTranslator.FromHtml("#454545");
        private Color sidebarBg = ColorTranslator.FromHtml("#171717");

        private bool isCollapsed = false;
        private Button activeButton = null;
        public Button _btnDashboard => btnDashboard;

        public MainForm()
        {
            InitializeComponent();

            pnlMainContent.BackColor = sidebarBg;

            // --- Load dashboard once ---
            dashboardPage = new PageWelcome();
            LoadPage(dashboardPage);
            SetActiveButton(btnDashboard);

            // --- Initialize other pages ---
            eventsPage = new PageEvents();
            contestantsPage = new PageContestants();
            criteriaPage = new PageCriteria();
            judgesPage = new PageJudges();
            resultsPage = new PageResults();
            logsPage = new PageScorecards();

            // --- Sidebar setup ---
            pnlSidebar.BackColor = sidebarBg;
            InitializeSidebar();
            ApplySidebarButtonStyles();

            // Round menu button
            RoundifyButton(btnMenu, 8);
            btnMenu.Resize += (s, e) => RoundifyButton(btnMenu, 8);

            // Make pages resize automatically with pnlMainContent
            pnlMainContent.Resize += (s, e) =>
            {
                foreach (Control page in pnlMainContent.Controls)
                {
                    page.PerformLayout();
                }
            };
        }

        // ---------------- Sidebar Toggle ----------------
        private void btnMenu_Click(object sender, EventArgs e)
        {
            if (isCollapsed)
            {
                pnlSidebar.Width = 380; // expand instantly
                btnMenu.Text = "☰  Menu";
                ShowSidebarButtonText();
                isCollapsed = false;
            }
            else
            {
                pnlSidebar.Width = 60; // collapse instantly
                btnMenu.Text = "›";
                HideSidebarButtonText();
                isCollapsed = true;
            }
        }

        private void ShowSidebarButtonText()
        {
            foreach (Button btn in pnlSidebar.Controls.OfType<Button>())
                if (btn != btnMenu && btn.Tag != null)
                    btn.Text = btn.Tag.ToString();
        }

        private void HideSidebarButtonText()
        {
            foreach (Button btn in pnlSidebar.Controls.OfType<Button>())
                if (btn != btnMenu)
                    btn.Text = "";
        }

        // ---------------- Sidebar Setup ----------------
        private void InitializeSidebar()
        {
            pnlSidebar.Width = isCollapsed ? 60 : 380;
            btnMenu.Text = isCollapsed ? "›" : "☰  Menu";
            ShowSidebarButtonText();
        }

        private void ApplySidebarButtonStyles()
        {
            foreach (Button btn in pnlSidebar.Controls.OfType<Button>())
            {
                if (btn != btnMenu)
                {
                    StyleSidebarButton(btn);
                    RoundifyButton(btn, 8);
                    btn.Resize += (s, e) => RoundifyButton(btn, 8);
                }
            }
        }

        // ---------------- Rounded Buttons ----------------
        private void RoundifyButton(Button btn, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, btn.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            btn.Region = new Region(path);
        }

        // ---------------- Sidebar Button Hover/Click ----------------
        private void StyleSidebarButton(Button btn)
        {
            btn.MouseEnter += (s, e) => { if (btn != activeButton) btn.BackColor = hoverColor; };
            btn.MouseLeave += (s, e) => { if (btn != activeButton) btn.BackColor = baseColor; };
            btn.MouseDown += (s, e) => btn.BackColor = clickColor;
            btn.MouseUp += (s, e) => { if (btn != activeButton) btn.BackColor = hoverColor; };
            btn.Click += SidebarButton_Click;
        }

        private void SidebarButton_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
        }

        private void SetActiveButton(Button btn)
        {
            if (activeButton != null && activeButton != btn)
                activeButton.BackColor = baseColor;

            btn.BackColor = hoverColor;
            activeButton = btn;
        }

        // ---------------- Load Pages ----------------
        public void LoadPage(UserControl page)
        {
            pnlMainContent.Controls.Clear();
            page.Dock = DockStyle.Fill;    // snug fit
            page.Margin = new Padding(0);  // remove gaps
            pnlMainContent.Controls.Add(page);
        }

        // ---------------- Navigation Buttons ----------------
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            LoadPage(dashboardPage);
        }

        private void btnEvents_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            LoadPage(eventsPage);
        }

        private void btnContestants_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            LoadPage(contestantsPage);
        }

        private void btnCriteria_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            LoadPage(criteriaPage);
        }

        private void btnJudges_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            LoadPage(judgesPage);
        }

        private void btnResults_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            LoadPage(resultsPage);
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            LoadPage(logsPage);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }
    }
}
