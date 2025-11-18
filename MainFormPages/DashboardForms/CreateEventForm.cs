using Microsoft.Extensions.Logging;
using NexScore.CreateEventPages;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Driver;
using NexScore.Models;

namespace NexScore
{
    public partial class CreateEventForm : Form
    {
        public string? CurrentEventId { get; set; }

        public SetupDetails setupDetailsPage;
        public SetupCriteria setupCriteriaPage;
        public SetupJudges setupJudgesPage;
        public SetupContestants setupContestantsPage;

        private Button activeButton = null;

        private Color baseColor = ColorTranslator.FromHtml("#353769");
        private Color hoverColor = ColorTranslator.FromHtml("#4E537A");
        private Color clickColor = ColorTranslator.FromHtml("#454545");

        private readonly string eventsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NexScore",
            "Events"
        );

        private readonly bool _isEditMode;

        public CreateEventForm(string? eventId = null, bool isEditMode = false)
        {
            InitializeComponent();

            _isEditMode = isEditMode;
            CurrentEventId = eventId;

            // Default creation state: disabled until saved progressively
            btnAddCriteria.Enabled = false;
            btnAddJudges.Enabled = false;
            btnAddContestants.Enabled = false;
            btnDone.Enabled = false; // enabled after contestants save, or immediately in edit mode

            pnlEventSetup.Dock = DockStyle.Fill;

            setupDetailsPage = new SetupDetails();
            setupCriteriaPage = new SetupCriteria();
            setupJudgesPage = new SetupJudges();
            setupContestantsPage = new SetupContestants();

            setupDetailsPage.CurrentEventId = eventId;
            setupCriteriaPage.CurrentEventId = eventId;
            setupJudgesPage.CurrentEventId = eventId;
            setupContestantsPage.CurrentEventId = eventId;

            // Force "Update" UI on all pages in edit mode
            if (_isEditMode)
            {
                setupDetailsPage.SetEditMode(true);
                setupCriteriaPage.SetEditMode(true);
                setupJudgesPage.SetEditMode(true);
                setupContestantsPage.SetEditMode(true);
            }
            else
            {
                setupDetailsPage.SetEditMode(false);
            }

            LoadPage(setupDetailsPage);
            SetActiveButton(btnDetails);

            AttachNavigationHandler(btnDetails);
            AttachNavigationHandler(btnAddCriteria);
            AttachNavigationHandler(btnAddJudges);
            AttachNavigationHandler(btnAddContestants);

            RoundifyButton(btnDetails, 8);
            RoundifyButton(btnAddCriteria, 8);
            RoundifyButton(btnAddJudges, 8);
            RoundifyButton(btnAddContestants, 8);

            setupDetailsPage.txtEventName.TextChanged += (s, e) => ValidateEventDetails();
            setupDetailsPage.datePicker.ValueChanged += (s, e) => ValidateEventDetails();
            setupDetailsPage.SavedStatusChanged += (saved) =>
            {
                if (!_isEditMode)
                    btnAddCriteria.Enabled = saved;
            };

            setupCriteriaPage.CriteriaValidityChanged += SetupCriteriaValidityChanged;

            setupCriteriaPage.CriteriaSaved += saved =>
            {
                if (_isEditMode) return;
                if (saved) btnAddJudges.Enabled = true;
            };

            setupJudgesPage.JudgesValidityChanged += SetupJudgesValidityChanged;

            setupContestantsPage.ContestantsSaved += (saved) =>
            {
                if (_isEditMode) return;
                btnDone.Enabled = saved;
            };

            setupDetailsPage.EventIdGenerated += id =>
            {
                CurrentEventId = id;
                setupCriteriaPage.CurrentEventId = id;
                setupJudgesPage.CurrentEventId = id;
                setupContestantsPage.CurrentEventId = id;
            };

            // Edit mode: unlock all sections and preload data
            if (_isEditMode)
            {
                btnAddCriteria.Enabled = true;
                btnAddJudges.Enabled = true;
                btnAddContestants.Enabled = true;
                btnDone.Enabled = true;

                if (!string.IsNullOrWhiteSpace(CurrentEventId))
                {
                    this.Shown += async (_, __) =>
                    {
                        await LoadExistingEventDataAsync(CurrentEventId);
                    };
                }
            }

            // IMPORTANT: ensure resources are cleaned before disposal to avoid ObjectDisposedException
            this.FormClosing += CreateEventForm_FormClosing;

            ValidateEventDetails();
        }

        private void CreateEventForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Gracefully shut down timers/tooltips in child pages
            setupContestantsPage?.PrepareForClose();
            // If other pages later have timers/tooltips, add similar calls here.
        }

        #region UI Styling

        private void AttachNavigationHandler(Button btn)
        {
            btn.MouseEnter += (s, e) => { if (btn != activeButton) btn.BackColor = hoverColor; };
            btn.MouseLeave += (s, e) => { if (btn != activeButton) btn.BackColor = baseColor; };
            btn.MouseDown += (s, e) => btn.BackColor = clickColor;
            btn.MouseUp += (s, e) => { if (btn != activeButton) btn.BackColor = hoverColor; };
            btn.Click += NavigationButton_Click;
        }

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

        #endregion

        #region Navigation

        private async void NavigationButton_Click(object sender, EventArgs e)
        {
            if (sender is not Button clickedButton) return;

            // In edit mode, allow free navigation without gating checks
            if (!_isEditMode)
            {
                // Details -> Criteria
                if (activeButton == btnDetails && clickedButton == btnAddCriteria)
                {
                    if (!setupDetailsPage.IsSaved)
                    {
                        MessageBox.Show("Please save your event details first.", "Unsaved event",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    setupCriteriaPage.CurrentEventId = setupDetailsPage.CurrentEventId;
                    btnAddCriteria.Enabled = true;
                    LoadPage(setupCriteriaPage);
                    SetActiveButton(btnAddCriteria);
                    return;
                }

                // Criteria -> Judges or Contestants
                if (activeButton == btnAddCriteria &&
                    (clickedButton == btnAddJudges || clickedButton == btnAddContestants))
                {
                    if (!setupCriteriaPage.ValidateAllInputs() || !setupCriteriaPage.ValidateTotals())
                    {
                        MessageBox.Show("Please complete all phases, segments, and criteria correctly.",
                            "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var saved = await setupCriteriaPage.SaveEventStructureAsync(setupCriteriaPage.CurrentEventId);
                    if (!saved)
                    {
                        btnAddJudges.Enabled = false;
                        return;
                    }

                    btnAddJudges.Enabled = true;
                }

                // Judges -> Contestants
                if (activeButton == btnAddJudges && clickedButton == btnAddContestants)
                {
                    if (!setupJudgesPage.AreJudgesValid())
                    {
                        MessageBox.Show("Please ensure judge names are filled and unique.",
                            "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    btnAddContestants.Enabled = true;
                }
            }

            // Load pages (shared)
            if (clickedButton == btnDetails) LoadPage(setupDetailsPage);
            else if (clickedButton == btnAddCriteria) LoadPage(setupCriteriaPage);
            else if (clickedButton == btnAddJudges) LoadPage(setupJudgesPage);
            else if (clickedButton == btnAddContestants) LoadPage(setupContestantsPage);

            SetActiveButton(clickedButton);
        }

        public void SetActiveButton(Button btn)
        {
            if (activeButton != null && activeButton != btn)
                activeButton.BackColor = baseColor;

            btn.BackColor = hoverColor;
            activeButton = btn;
        }

        public void LoadPage(UserControl page)
        {
            pnlEventSetup.Controls.Clear();
            page.Dock = DockStyle.Fill;
            pnlEventSetup.Controls.Add(page);
        }

        #endregion

        #region Validation

        private bool AreEventDetailsValid()
        {
            bool isNameFilled = !string.IsNullOrWhiteSpace(setupDetailsPage.txtEventName.Text);
            bool isDateFilled = setupDetailsPage.datePicker.CustomFormat != " ";
            if (!_isEditMode)
            {
                btnAddCriteria.Enabled = setupDetailsPage.IsSaved;
            }
            return isNameFilled && isDateFilled;
        }

        public void ValidateEventDetails()
        {
            AreEventDetailsValid();
        }

        private void SetupCriteriaValidityChanged(bool isValid)
        {
            if (_isEditMode) return;

            if (!isValid)
                btnAddJudges.Enabled = false;

            if (!setupJudgesPage.AreJudgesValid())
                btnAddContestants.Enabled = false;
        }

        private void SetupJudgesValidityChanged(bool isValid)
        {
            if (_isEditMode) return;

            btnAddContestants.Enabled = isValid;
        }

        #endregion

        private void btnDone_Click(object sender, EventArgs e)
        {
            // Basic guard: ensure we have an event ID
            if (string.IsNullOrWhiteSpace(CurrentEventId))
            {
                MessageBox.Show("Event ID is missing. Please ensure event details are saved.",
                    "Finalize Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Directory.CreateDirectory(eventsFolder);
                string currentEventFile = Path.Combine(eventsFolder, "current_event.json");

                // Minimal JSON payload; expand if you need more metadata.
                string json = $"{{\"eventId\":\"{CurrentEventId}\"}}";
                File.WriteAllText(currentEventFile, json);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to finalize event: {ex.Message}",
                    "Finalize Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadExistingEventDataAsync(string documentId)
        {
            try
            {
                // Fetch the EventModel first (basic details)
                var evt = await Database.Events
                    .Find(e => e.Id == documentId)
                    .FirstOrDefaultAsync();

                if (evt == null)
                {
                    MessageBox.Show("Event not found for editing.", "Load Event",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Apply IDs to all pages
                setupDetailsPage.CurrentEventId = documentId;
                setupCriteriaPage.CurrentEventId = documentId;
                setupJudgesPage.CurrentEventId = documentId;
                setupContestantsPage.CurrentEventId = documentId;

                // 1) Details
                await setupDetailsPage.LoadExistingEventAsync(documentId);
                setupDetailsPage.IsSaved = true;
                setupDetailsPage.SetEditMode(true);

                // 2) Criteria/Structure
                await setupCriteriaPage.LoadEventStructureAsync(documentId);
                setupCriteriaPage.SetEditMode(true);

                // 3) Judges
                setupJudgesPage.LoadJudgesForEvent(documentId);
                setupJudgesPage.SetEditMode(true);

                // 4) Contestants
                await setupContestantsPage.LoadContestantsForEventAsync(documentId);
                setupContestantsPage.SetEditMode(true);

                // Enable all buttons in edit mode
                btnAddCriteria.Enabled = true;
                btnAddJudges.Enabled = true;
                btnAddContestants.Enabled = true;
                btnDone.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load event data:\n{ex.Message}", "Load Event",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // Optional shared interface to standardize loading across pages
    public interface IEventDataLoader
    {
        Task LoadEventAsync(string eventId);
    }
}