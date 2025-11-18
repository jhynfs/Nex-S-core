using MongoDB.Driver;
using NexScore.MainFormPages.DashboardForms.OpenEventControl;
using NexScore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
// Added to access AppSession
using NexScore;

namespace NexScore.MainFormPages.DashboardForms
{
    public partial class OpenEventForm : Form
    {
        private EventModel? _selectedEvent;
        private readonly List<EventListItemControl> _itemControls = new();

        public event Action<EventModel>? EventConfirmed;
        // Consumers (e.g., parent form) can subscribe to know which event was opened.

        // Expose the selected event to the caller after OK is pressed
        public EventModel? SelectedEvent => _selectedEvent;

        public OpenEventForm()
        {
            InitializeComponent();
            this.Load += OpenEventForm_Load;
            btnSelectEvent.Click += BtnSelectEvent_Click;

            // Ensure closing with X results in Cancel, does not reopen anything
            this.FormClosing += (s, e) =>
            {
                if (this.DialogResult != DialogResult.OK)
                {
                    this.DialogResult = DialogResult.Cancel;
                }
            };
        }

        private async void OpenEventForm_Load(object? sender, EventArgs e)
        {
            await LoadEventsAsync();
        }

        private async Task LoadEventsAsync()
        {
            try
            {
                UseWaitCursor = true;
                btnSelectEvent.Enabled = false;
                _flowPanelEvents.Controls.Clear();
                _itemControls.Clear();
                _selectedEvent = null;

                // Fetch events from MongoDB
                var events = await Database.Events
                    .Find(FilterDefinition<EventModel>.Empty)
                    .SortByDescending(e => e.CreatedAt)
                    .ToListAsync();

                if (events.Count == 0)
                {
                    var emptyLabel = new Label
                    {
                        Text = "No events found. Create one to get started.",
                        ForeColor = Color.LightGray,
                        AutoSize = true,
                        Font = new Font("Lexend Deca", 10F),
                        Padding = new Padding(10)
                    };
                    _flowPanelEvents.Controls.Add(emptyLabel);
                    return;
                }

                foreach (var evt in events)
                {
                    var ctl = new EventListItemControl(evt);
                    ctl.Selected += HandleItemSelected;
                    _itemControls.Add(ctl);
                    _flowPanelEvents.Controls.Add(ctl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load events:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void HandleItemSelected(EventListItemControl clicked)
        {
            foreach (var ctl in _itemControls)
                ctl.SetSelected(ctl == clicked);

            _selectedEvent = clicked.Event;
            btnSelectEvent.Enabled = true;
        }

        private void BtnSelectEvent_Click(object? sender, EventArgs e)
        {
            if (_selectedEvent == null)
            {
                MessageBox.Show("Please select an event first.", "Select Event",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Publish selected event so subscribers (PageEvents) update UI
            AppSession.SetCurrentEvent(_selectedEvent);

            // Notify listeners (optional) and close dialog with OK
            EventConfirmed?.Invoke(_selectedEvent);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Optional: public method to refresh from outside
        public async Task RefreshEventsAsync() => await LoadEventsAsync();
    }
}