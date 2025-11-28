using MongoDB.Driver;
using NexScore.Models;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NexScore
{
    public partial class PageEvents : UserControl
    {
        private EventModel? _currentEvent;

        private bool _isOpenEventDialogOpen = false;
        private bool _isEditEventDialogOpen = false;

        public PageEvents()
        {
            InitializeComponent();

            ConfigureNonSelectableTextBox(_txtDescriptionHere);
            ConfigureNonSelectableTextBox(_txtVenueHere);
            ConfigureNonSelectableTextBox(_txtDateHere);
            ConfigureNonSelectableTextBox(_txtEvtOrg);

            _lblCurrentEvtName.Visible = false;
            _txtEvtOrg.ReadOnly = true;

            AppSession.CurrentEventChanged += OnCurrentEventChanged;

            if (AppSession.CurrentEvent != null)
            {
                OnCurrentEventChanged(AppSession.CurrentEvent);
            }
            else
            {
                ClearEventUi();
            }
            btnEvtNew.Click -= btnEvtNew_Click;
            btnEvtNew.Click += btnEvtNew_Click;

            btnDetailsMod.Click -= btnDetailsMod_Click;
            btnDetailsMod.Click += btnDetailsMod_Click;

            btnEvtSelect.Click -= btnEvtSelect_Click;
            btnEvtSelect.Click += btnEvtSelect_Click;
        }

        #region Non-selectable TextBox Helper
        [DllImport("user32.dll")]
        private static extern bool HideCaret(IntPtr hWnd);

        private void ConfigureNonSelectableTextBox(TextBox tb)
        {
            tb.ReadOnly = true;
            tb.TabStop = false;
            tb.ShortcutsEnabled = false;
            tb.Cursor = Cursors.Arrow;
            tb.GotFocus += (s, e) => RemoveFocusAndHideCaret(tb);
            tb.Enter += (s, e) => RemoveFocusAndHideCaret(tb);
            tb.MouseDown += (s, e) => RemoveFocusAndHideCaret(tb);
            tb.MouseUp += (s, e) => RemoveFocusAndHideCaret(tb);
            tb.MouseWheel += (s, e) => RemoveFocusAndHideCaret(tb);
        }

        private void RemoveFocusAndHideCaret(Control c)
        {
            try
            {
                HideCaret(c.Handle);

                if (pnlMainEvents != null && pnlMainEvents.CanFocus)
                    pnlMainEvents.Focus();
                else
                    this.Focus();
            }
            catch
            {

            }
        }
        #endregion

        private void OnCurrentEventChanged(EventModel evt)
        {
            _currentEvent = evt;

            _lblCurrentEvtName.Text = evt.EventName ?? string.Empty;
            _lblCurrentEvtName.Visible = !string.IsNullOrWhiteSpace(_lblCurrentEvtName.Text);

            _txtDescriptionHere.Text = evt.Description ?? string.Empty;
            _txtVenueHere.Text = evt.Venue ?? string.Empty;
            _txtDateHere.Text = evt.EventDate.ToString("MMM dd, yyyy");

            try
            {
                _txtEvtOrg.Text = (evt.Organizers != null)
                    ? string.Join(Environment.NewLine, evt.Organizers)
                    : string.Empty;
            }
            catch
            {
                _txtEvtOrg.Text = string.Empty;
            }
        }

        private void ClearEventUi()
        {
            _currentEvent = null;

            _lblCurrentEvtName.Visible = false;
            _lblCurrentEvtName.Text = string.Empty;

            _txtDescriptionHere.Text = string.Empty;
            _txtVenueHere.Text = string.Empty;
            _txtDateHere.Text = string.Empty;
            _txtEvtOrg.Text = string.Empty;
        }

        private void btnEvtNew_Click(object sender, EventArgs e)
        {
            using (var form = new CreateEventForm(null))
            {
                form.ShowDialog();
            }
        }

        private async void btnDetailsMod_Click(object sender, EventArgs e)
        {
            if (_isEditEventDialogOpen)
                return;

            if (AppSession.CurrentEvent == null)
            {
                MessageBox.Show("No event selected. Please open an event first.", "Edit Event",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _isEditEventDialogOpen = true;
            try
            {
                var eventDocumentId = AppSession.CurrentEvent.Id;
                using (var form = new CreateEventForm(eventDocumentId, isEditMode: true))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        await ReloadAndPublishEventAsync(eventDocumentId);
                    }
                }
            }
            finally
            {
                _isEditEventDialogOpen = false;
            }
        }

        private void btnEvtSelect_Click(object sender, EventArgs e)
        {
            if (_isOpenEventDialogOpen)
                return;

            _isOpenEventDialogOpen = true;
            try
            {
                using (var form = new MainFormPages.DashboardForms.OpenEventForm())
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK && form.SelectedEvent != null)
                    {
                        AppSession.SetCurrentEvent(form.SelectedEvent);
                    }
                }
            }
            finally
            {
                _isOpenEventDialogOpen = false;
            }
        }

        private async Task ReloadAndPublishEventAsync(string eventDocumentId)
        {
            try
            {
                var evt = await Database.Events
                    .Find(e => e.Id == eventDocumentId)
                    .FirstOrDefaultAsync();

                if (evt != null)
                {
                    AppSession.SetCurrentEvent(evt);
                }
                else
                {
                    MessageBox.Show("The event could not be found after editing.", "Reload Event",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to reload event:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}