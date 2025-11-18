using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Driver;
using NexScore.CreateEventPages.SetupContestantControls;
using NexScore.Models;
using NexScore.Helpers;

namespace NexScore
{
    public partial class SetupContestants : UserControl
    {
        public string CurrentEventId { get; set; }

        private bool _isSavedOnce = false;
        private ToolTip _toolTipContestants = new ToolTip();
        private ErrorProvider _errorProvider = new ErrorProvider();
        private string? _currentEventName;

        // Track which textboxes have actual user input (touched)
        private readonly HashSet<TextBox> _touched = new();
        private System.Windows.Forms.Timer _validateTimer;

        // notify CreateEventForm when contestants are saved (or fail to save)
        public event Action<bool>? ContestantsSaved;

        public SetupContestants()
        {
            InitializeComponent();
            InitializeExtras();
        }

        // Force Update-only mode in UI
        public void SetEditMode(bool isEditMode)
        {
            if (isEditMode)
            {
                _isSavedOnce = true;
                btnSaveContestants.Text = "update";
                _toolTipContestants.SetToolTip(btnSaveContestants, "Update contestants");
            }
        }

        private void InitializeExtras()
        {
            cbContestType.Items.Clear();
            cbContestType.Items.AddRange(new object[] { "Male", "Female", "Mixed", "Open" });
            cbContestType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbContestType.SelectedIndexChanged += (s, e) => ApplyGenderModeToAll();

            btnAddContestant.Click += BtnAddContestant_Click;
            btnSaveContestants.Click += BtnSaveContestants_Click;

            _toolTipContestants.SetToolTip(btnSaveContestants, "Save contestants");

            _validateTimer = new System.Windows.Forms.Timer { Interval = 300 };
            _validateTimer.Tick += (s, e) =>
            {
                _validateTimer.Stop();
                if (!IsDisposed) // guard
                    ValidateAll();
            };

            // Configure ErrorProvider to reduce flicker
            _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        }

        // Cleanup invoked by parent before form closes
        public void PrepareForClose()
        {
            try
            {
                _validateTimer?.Stop();
                if (_toolTipContestants != null && btnSaveContestants != null && btnSaveContestants.IsHandleCreated)
                {
                    // Safely hide tooltip if still visible
                    _toolTipContestants.Hide(btnSaveContestants);
                }
                if (_toolTipContestants != null)
                {
                    _toolTipContestants.Active = false;
                    _toolTipContestants.RemoveAll();
                }
            }
            catch
            {
                // swallow - safe cleanup
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    _validateTimer?.Stop();
                    _validateTimer?.Dispose();
                }
                catch { }
                _validateTimer = null;

                if (_toolTipContestants != null)
                {
                    try
                    {
                        if (btnSaveContestants != null && btnSaveContestants.IsHandleCreated)
                            _toolTipContestants.Hide(btnSaveContestants);
                        _toolTipContestants.RemoveAll();
                        _toolTipContestants.Active = false;
                    }
                    catch { }
                    _toolTipContestants.Dispose();
                    _toolTipContestants = null;
                }

                _errorProvider?.Dispose();
            }
            base.Dispose(disposing);
        }

        // Allow SetupDetails (or parent) to pass event id + name directly
        public void SetEventContext(string eventId, string eventName)
        {
            CurrentEventId = eventId;
            _currentEventName = string.IsNullOrWhiteSpace(eventName) ? null : eventName.Trim();
        }

        // Optional: if you only want to update the name later
        public void SetEventName(string eventName)
        {
            _currentEventName = string.IsNullOrWhiteSpace(eventName) ? null : eventName.Trim();
        }

        // Fallback: ensure we have the EventName; query DB if still missing
        private async Task EnsureEventNameAsync()
        {
            if (!string.IsNullOrWhiteSpace(_currentEventName))
                return;
            if (string.IsNullOrWhiteSpace(CurrentEventId))
                return;

            try
            {
                var eventsCol = Database.GetCollection<EventModel>("Events");
                var evt = await eventsCol.Find(e => e.Id == CurrentEventId).FirstOrDefaultAsync();
                _currentEventName = evt?.EventName?.Trim();
            }
            catch
            {
                // swallow to avoid UI disruption; _currentEventName remains null if it fails
            }
        }

        #region Load / Prepare
        public async Task LoadContestantsForEventAsync(string eventId)
        {
            CurrentEventId = eventId;

            // Prefill name if not provided by SetEventContext
            await EnsureEventNameAsync();

            var existingDynamic = _flowMainC.Controls.OfType<AddContestantControl>().ToList();
            foreach (var c in existingDynamic)
            {
                _flowMainC.Controls.Remove(c);
                c.Dispose();
            }

            var contestantsCol = Database.GetCollection<ContestantModel>("Contestants");
            var existingContestants = await contestantsCol
                .Find(c => c.EventId == eventId)
                .SortBy(c => c.Number)
                .ToListAsync();

            foreach (var c in existingContestants)
            {
                var ctrl = CreateContestantControl();
                ctrl.SetNumber(c.Number);
                ctrl.FullNameTextBox.Text = c.FullName; // not placeholder (color differs)
                ctrl.RepresentingTextBox.Text = c.Representing;
                ctrl.ConfigureGenderOpen();
                if (c.Age.HasValue)
                    ctrl.AgeTextBox.Text = c.Age.Value.ToString();
                ctrl.SetPhoto(c.PhotoPath ?? "");

                // FIX: Load Advocacy so it doesn't remain placeholder in edit mode
                if (!string.IsNullOrWhiteSpace(c.Advocacy))
                {
                    ctrl.AdvocacyTextBox.Text = c.Advocacy;
                    MarkTouched(ctrl.AdvocacyTextBox);
                }

                // Mark loaded fields as touched so validation applies sensibly
                MarkTouched(ctrl.FullNameTextBox);
                MarkTouched(ctrl.RepresentingTextBox);
                if (!string.IsNullOrWhiteSpace(ctrl.AgeTextBox.Text))
                    MarkTouched(ctrl.AgeTextBox);

                InsertContestantControlAtQueueEnd(ctrl);
            }

            RenumberContestants();

            if (existingContestants.Any())
            {
                btnSaveContestants.Text = "update";
                _toolTipContestants.SetToolTip(btnSaveContestants, "Update contestants");
                _isSavedOnce = true;
            }
            else
            {
                btnSaveContestants.Text = "save";
                _toolTipContestants.SetToolTip(btnSaveContestants, "Save contestants");
                _isSavedOnce = false;
            }

            ApplyGenderModeToAll();
            ValidateAll();
        }
        #endregion

        #region Adding / Removing
        private void BtnAddContestant_Click(object sender, EventArgs e)
        {
            if (cbContestType.SelectedItem is null)
            {
                MessageBox.Show("Select a Contest Type first.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var ctrl = CreateContestantControl();
            InsertContestantControlAtQueueEnd(ctrl);
            RenumberContestants();
            ApplyGenderMode(ctrl);
            // Do not validate immediately (avoid flicker); will occur when user types.
        }

        private AddContestantControl CreateContestantControl()
        {
            var ctrl = new AddContestantControl();
            ctrl.RemoveRequested += Ctrl_RemoveRequested;

            // IMPORTANT: async handler so we can EnsureEventNameAsync on demand
            ctrl.AddPhotoRequested += Ctrl_AddPhotoRequestedAsync;

            // Debounced validation only after meaningful typing (ignore placeholder states).
            ctrl.FullNameTextBox.TextChanged += (s, e) =>
            {
                if (!ctrl.IsPlaceholder(ctrl.FullNameTextBox))
                    MarkTouched(ctrl.FullNameTextBox);
                DebouncedValidate();
            };
            ctrl.RepresentingTextBox.TextChanged += (s, e) =>
            {
                if (!ctrl.IsPlaceholder(ctrl.RepresentingTextBox))
                    MarkTouched(ctrl.RepresentingTextBox);
                DebouncedValidate();
            };
            ctrl.AgeTextBox.TextChanged += (s, e) =>
            {
                if (!ctrl.IsPlaceholder(ctrl.AgeTextBox))
                    MarkTouched(ctrl.AgeTextBox);
                DebouncedValidate();
            };
            ctrl.AdvocacyTextBox.TextChanged += (s, e) =>
            {
                if (!ctrl.IsPlaceholder(ctrl.AdvocacyTextBox))
                    MarkTouched(ctrl.AdvocacyTextBox);
                // Advocacy optional; no error needed
            };

            return ctrl;
        }

        private void MarkTouched(TextBox tb)
        {
            if (!_touched.Contains(tb))
                _touched.Add(tb);
        }

        private void DebouncedValidate()
        {
            _validateTimer.Stop();
            _validateTimer.Start();
        }

        private void InsertContestantControlAtQueueEnd(AddContestantControl ctrl)
        {
            int bottomIndex = _flowMainC.Controls.IndexOf(pnlSpaceBelowC);
            if (bottomIndex < 0)
            {
                _flowMainC.Controls.Add(ctrl);
            }
            else
            {
                _flowMainC.Controls.Add(ctrl);
                _flowMainC.Controls.SetChildIndex(ctrl, bottomIndex);
            }
        }

        private void Ctrl_RemoveRequested(object sender, EventArgs e)
        {
            if (sender is AddContestantControl acc)
            {
                _flowMainC.Controls.Remove(acc);
                acc.Dispose();
                RenumberContestants();
                DebouncedValidate();
            }
        }
        #endregion

        #region Photo
        // Async so we can fetch event name if missing at click-time
        private async void Ctrl_AddPhotoRequestedAsync(object sender, EventArgs e)
        {
            if (sender is not AddContestantControl acc) return;

            if (string.IsNullOrWhiteSpace(acc.FullName))
            {
                MessageBox.Show("Enter Full Name before adding a photo.", "Name Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // If name is missing (e.g., navigation timing), fetch it now
            await EnsureEventNameAsync();

            if (string.IsNullOrWhiteSpace(_currentEventName))
            {
                MessageBox.Show("Event name not available yet. Save event details first or ensure SetEventContext was called.",
                    "Event Name Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var ofd = new OpenFileDialog
            {
                Title = "Select portrait photo (4:5 ratio)",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Multiselect = false
            };

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                using var img = Image.FromFile(ofd.FileName);
                double ratio = (double)img.Width / img.Height;
                double target = 4.0 / 5.0;
                if (Math.Abs(ratio - target) > 0.05)
                {
                    MessageBox.Show("Image must have approximately a 4:5 aspect ratio.", "Invalid Aspect Ratio",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string folder = GetPortraitFolder(_currentEventName);
                Directory.CreateDirectory(folder);

                string safeName = string.Join("_",
                    acc.FullName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries))
                    .Replace(' ', '_');

                string filePath = Path.Combine(folder, $"{safeName}.jpg");

                // Save to a temp file first, then atomically replace the target (avoids overwrite issues)
                string tempPath = Path.Combine(folder, $"{safeName}_{Guid.NewGuid():N}.tmp.jpg");
                img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Jpeg);

                if (File.Exists(filePath))
                    File.Delete(filePath);

                File.Move(tempPath, filePath);

                acc.SetPhoto(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to process image: {ex.Message}", "Photo Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetPortraitFolder(string eventName)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NexScore",
                "Portraits",
                eventName);
        }
        #endregion

        #region Validation
        private IEnumerable<AddContestantControl> GetContestantControls() =>
            _flowMainC.Controls.OfType<AddContestantControl>();

        private void RenumberContestants()
        {
            int n = 1;
            foreach (var c in GetContestantControls())
                c.SetNumber(n++);
        }

        private void ApplyGenderModeToAll()
        {
            foreach (var c in GetContestantControls())
                ApplyGenderMode(c);
        }

        private void ApplyGenderMode(AddContestantControl ctrl)
        {
            var mode = cbContestType.SelectedItem?.ToString();
            switch (mode)
            {
                case "Male":
                    ctrl.ConfigureGenderFixed("Male");
                    break;
                case "Female":
                    ctrl.ConfigureGenderFixed("Female");
                    break;
                case "Mixed":
                    ctrl.ConfigureGenderMixed();
                    break;
                case "Open":
                    ctrl.ConfigureGenderOpen();
                    break;
            }
        }

        private bool IsTouched(TextBox tb) => _touched.Contains(tb);

        private void ValidateAll()
        {
            if (IsDisposed) return; // defensive
            // Clear errors first (we reapply selectively)
            foreach (var c in GetContestantControls())
            {
                _errorProvider.SetError(c.FullNameTextBox, "");
                _errorProvider.SetError(c.RepresentingTextBox, "");
                _errorProvider.SetError(c.AgeTextBox, "");
            }

            var list = GetContestantControls().ToList();

            foreach (var c in list)
            {
                // Full Name
                if (IsTouched(c.FullNameTextBox))
                {
                    if (string.IsNullOrWhiteSpace(c.FullName))
                        _errorProvider.SetError(c.FullNameTextBox, "Full Name required");
                }

                // Representing
                if (IsTouched(c.RepresentingTextBox))
                {
                    if (string.IsNullOrWhiteSpace(c.Representing))
                        _errorProvider.SetError(c.RepresentingTextBox, "Representing required");
                }

                // Age (optional)
                if (IsTouched(c.AgeTextBox))
                {
                    if (!string.IsNullOrWhiteSpace(c.AgeRaw) && !int.TryParse(c.AgeRaw, out _))
                        _errorProvider.SetError(c.AgeTextBox, "Whole number only");
                }
            }

            // Duplicate detection: only among entries where both fields are touched & non-empty
            var dupGroups = list
                .Where(c => IsTouched(c.FullNameTextBox) && IsTouched(c.RepresentingTextBox))
                .Where(c => !string.IsNullOrWhiteSpace(c.FullName) && !string.IsNullOrWhiteSpace(c.Representing))
                .Select(c => new
                {
                    Ctrl = c,
                    Key = $"{c.FullName.ToLowerInvariant()}||{c.Representing.ToLowerInvariant()}"
                })
                .GroupBy(x => x.Key)
                .Where(g => g.Count() > 1);

            foreach (var g in dupGroups)
            {
                foreach (var item in g)
                {
                    _errorProvider.SetError(item.Ctrl.FullNameTextBox, "Duplicate name & representing");
                    _errorProvider.SetError(item.Ctrl.RepresentingTextBox, "Duplicate name & representing");
                }
            }
        }

        private bool IsValidForSave(out string message)
        {
            message = "";
            var list = GetContestantControls().ToList();
            if (list.Count == 0)
            {
                message = "No contestants added.";
                return false;
            }

            // Strict required-field checks irrespective of 'touched' state
            if (list.Any(c => string.IsNullOrWhiteSpace(c.FullName)))
            {
                message = "Full Name is required for all contestants.";
                return false;
            }
            if (list.Any(c => string.IsNullOrWhiteSpace(c.Representing)))
            {
                message = "Representing is required for all contestants.";
                return false;
            }

            // Check any field-level errors currently set (e.g., invalid Age, etc.)
            foreach (var c in list)
            {
                var fullErr = _errorProvider.GetError(c.FullNameTextBox);
                var repErr = _errorProvider.GetError(c.RepresentingTextBox);
                var ageErr = _errorProvider.GetError(c.AgeTextBox);

                if (!string.IsNullOrEmpty(fullErr) && fullErr != "Duplicate name & representing")
                    return false;
                if (!string.IsNullOrEmpty(repErr) && repErr != "Duplicate name & representing")
                    return false;
                if (!string.IsNullOrEmpty(ageErr))
                    return false;
            }

            // Duplicate check across all non-empty entries (ignore 'touched')
            bool hasDupes = list
                .Where(c => !string.IsNullOrWhiteSpace(c.FullName) && !string.IsNullOrWhiteSpace(c.Representing))
                .Select(c => $"{c.FullName.ToLowerInvariant()}||{c.Representing.ToLowerInvariant()}")
                .GroupBy(k => k)
                .Any(g => g.Count() > 1);
            if (hasDupes)
            {
                message = "Resolve duplicates (Full Name + Representing).";
                return false;
            }

            if (string.IsNullOrWhiteSpace(CurrentEventId))
            {
                message = "Event ID missing.";
                return false;
            }

            return true;
        }
        #endregion

        #region Save
        private async void BtnSaveContestants_Click(object sender, EventArgs e)
        {
            if (IsDisposed) return;

            ValidateAll();
            if (!IsValidForSave(out var msg))
            {
                if (!IsDisposed && btnSaveContestants.IsHandleCreated)
                {
                    _toolTipContestants.Show(string.IsNullOrEmpty(msg) ? "Fix validation errors." : msg,
                        btnSaveContestants, 10, -30, 2500);
                }
                ContestantsSaved?.Invoke(false);
                return;
            }

            try
            {
                var now = DateTime.Now;
                var col = Database.GetCollection<ContestantModel>("Contestants");

                // Load existing docs to preserve PhotoPath if UI accidentally cleared it
                var existingDocs = await col.Find(c => c.EventId == CurrentEventId).ToListAsync();

                var byNumber = existingDocs
                    .GroupBy(c => c.Number)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.UpdatedAt).First());

                var byKey = existingDocs
                    .Where(c => !string.IsNullOrWhiteSpace(c.FullName) && !string.IsNullOrWhiteSpace(c.Representing))
                    .GroupBy(c => $"{c.FullName.Trim().ToLowerInvariant()}||{c.Representing.Trim().ToLowerInvariant()}")
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.UpdatedAt).First());

                var docs = GetContestantControls().Select(c =>
                {
                    var normKey = $"{(c.FullName ?? "").Trim().ToLowerInvariant()}||{(c.Representing ?? "").Trim().ToLowerInvariant()}";

                    string? photo = !string.IsNullOrWhiteSpace(c.PhotoPath)
                        ? c.PhotoPath
                        : (byNumber.TryGetValue(c.Number, out var prevByNum) && !string.IsNullOrWhiteSpace(prevByNum.PhotoPath)
                            ? prevByNum.PhotoPath
                            : (byKey.TryGetValue(normKey, out var prevByKey) ? prevByKey.PhotoPath : null));

                    return new ContestantModel
                    {
                        EventId = CurrentEventId,
                        Number = c.Number,
                        FullName = c.FullName.Trim(),
                        Representing = c.Representing.Trim(),
                        Gender = c.Gender,
                        Age = int.TryParse(c.AgeRaw, out var a) ? a : (int?)null,
                        Advocacy = string.IsNullOrWhiteSpace(c.Advocacy) ? null : c.Advocacy.Trim(),
                        PhotoPath = photo,
                        CreatedAt = now,
                        UpdatedAt = now
                    };
                }).ToList();

                await col.DeleteManyAsync(c => c.EventId == CurrentEventId);
                if (docs.Any())
                    await col.InsertManyAsync(docs);

                bool first = !_isSavedOnce;
                _isSavedOnce = true;
                btnSaveContestants.Text = "update";
                _toolTipContestants.SetToolTip(btnSaveContestants, "Update contestants");
                if (!IsDisposed && btnSaveContestants.IsHandleCreated)
                {
                    _toolTipContestants.Show(first ? "Contestants saved!" : "Contestants updated!",
                        btnSaveContestants, 10, -30, 2000);
                }

                // Notify success to enable Done (when not edit mode)
                ContestantsSaved?.Invoke(true);
            }
            catch (Exception ex)
            {
                if (!IsDisposed)
                {
                    MessageBox.Show($"Error saving contestants: {ex.Message}",
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                ContestantsSaved?.Invoke(false);
            }
        }
        #endregion
    }
}