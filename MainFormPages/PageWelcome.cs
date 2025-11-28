using MongoDB.Bson;
using NexScore.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace NexScore
{
    public partial class PageWelcome : UserControl
    {
        private EventModel? _currentEvent;
        public Panel pnlNoEvent => _pnlNoEvent;
        public Panel pnlEvent => _pnlEvent;
        public PageWelcome()
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;
            this.Margin = new Padding(0);

            _pnlNoEvent.Dock = DockStyle.Fill;
            _pnlEvent.Dock = DockStyle.Fill;

            _pnlNoEvent.Margin = new Padding(0);
            _pnlEvent.Margin = new Padding(0);

            RefreshDashboard();
        }

        private void btnNewEvent_Click(object sender, EventArgs e)
        {
            string newEventId = ObjectId.GenerateNewId().ToString();
            CreateEventForm form = new CreateEventForm(newEventId);
            form.ShowDialog();

            RefreshDashboard();
        }

        private void btnOpenEvent_Click(object sender, EventArgs e)
        {
            using (var openEventForm = new MainFormPages.DashboardForms.OpenEventForm())
            {
                var result = openEventForm.ShowDialog();

                if (result == DialogResult.OK && openEventForm.SelectedEvent != null)
                {
                    _currentEvent = openEventForm.SelectedEvent;

                    AppSession.SetCurrentEvent(_currentEvent);

                    var host = this.FindForm() as MainForm;
                    if (host != null)
                    {
                        HideButtonByName(host, "btnDashboard");
                        HideButtonByName(host, "btnDahsboard");

                        var btnEvents = host.Controls.Find("btnEvents", true).FirstOrDefault() as Button;
                        if (btnEvents != null)
                        {
                            btnEvents.PerformClick();
                        }
                        else
                        {
                            host.LoadPage(new PageEvents());
                        }
                    }

                    _pnlNoEvent.Visible = false;
                    _pnlEvent.Visible = false;
                }
                else
                {
                    _pnlNoEvent.Visible = true;
                    _pnlEvent.Visible = false;
                }
            }
        }

        public void RefreshDashboard()
        {
            _pnlNoEvent.Visible = true;
            _pnlEvent.Visible = false;
        }

        private static void HideButtonByName(Form host, string name)
        {
            try
            {
                var ctrl = host.Controls.Find(name, true).FirstOrDefault();
                if (ctrl != null) ctrl.Visible = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Dashboard] Failed to hide '{name}': {ex.Message}");
            }
        }
    }
}