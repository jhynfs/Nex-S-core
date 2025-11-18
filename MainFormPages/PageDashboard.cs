using MongoDB.Bson;
using System;
using System.Windows.Forms;
using NexScore.Models;

namespace NexScore
{
    public partial class PageDashboard : UserControl
    {
        private EventModel? _currentEvent;
        public Panel pnlNoEvent => _pnlNoEvent;
        public Panel pnlEvent => _pnlEvent;
        public PageDashboard()
        {
            InitializeComponent();

            // Make the whole page fill its parent
            this.Dock = DockStyle.Fill;
            this.Margin = new Padding(0);

            // Make inner panels fill the page
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

            // After creating an event, still show "No Event" until user explicitly opens one.
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

                    // Publish to the whole app so other pages can update themselves
                    AppSession.SetCurrentEvent(_currentEvent);

                    // Show the event panel now that an event has been chosen
                    _pnlNoEvent.Visible = false;
                    _pnlEvent.Visible = true;

                    // TODO: Update any event-level labels here, e.g.:
                    // lblEventTitle.Text = _currentEvent.EventName;
                    // lblEventDate.Text = _currentEvent.EventDate.ToString("MMM dd, yyyy");
                }
                else
                {
                    // Keep "No Event" if user cancels or closes
                    _pnlNoEvent.Visible = true;
                    _pnlEvent.Visible = false;
                }
            }
        }

        public void RefreshDashboard()
        {
            // Always start with "No Event" visible until user opens one via btnOpenEvent
            _pnlNoEvent.Visible = true;
            _pnlEvent.Visible = false;
        }
    }
}