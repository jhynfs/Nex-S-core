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

        private readonly HashSet<TextBox> _touched = new();
        private System.Windows.Forms.Timer _validateTimer;

        public event Action<bool>? ContestantsSaved;

        public SetupContestants()
        {
            InitializeComponent();
            InitializeExtras();
        }

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
            cbContestType.SelectedIndexChanged += (s, e) =>
            {
                ApplyLayoutForContestType();
                ApplyGenderModeToAll();
                RenumberContestants();
                ResizeAllContestantControlsToFlowWidth();
            };

            btnAddContestant.Click += BtnAddContestant_Click;
            btnSaveContestants.Click += BtnSaveContestants_Click;

            _toolTipContestants.SetToolTip(btnSaveContestants, "Save contestants");

            _validateTimer = new System.Windows.Forms.Timer { Interval = 300 };
            _validateTimer.Tick += (s, e) =>
            {
                _validateTimer.Stop();
                if (!IsDisposed)
                    ValidateAll();
            };

            _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            if (_flowMainC != null)
                _flowMainC.SizeChanged += (s, e) => ResizeAllContestantControlsToFlowWidth();

            ApplyLayoutForContestType();
        }

        public void PrepareForClose()
        {
            try
            {
                _validateTimer?.Stop();
                if (_toolTipContestants != null && btnSaveContestants != null && btnSaveContestants.IsHandleCreated)
                    _toolTipContestants.Hide(btnSaveContestants);
                if (_toolTipContestants != null)
                {
                    _toolTipContestants.Active = false;
                    _toolTipContestants.RemoveAll();
                }
            }
            catch { }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try { _validateTimer?.Stop(); _validateTimer?.Dispose(); } catch { }
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

        public void SetEventContext(string eventId, string eventName)
        {
            CurrentEventId = eventId;
            _currentEventName = string.IsNullOrWhiteSpace(eventName) ? null : eventName.Trim();
        }

        public void SetEventName(string eventName)
        {
            _currentEventName = string.IsNullOrWhiteSpace(eventName) ? null : eventName.Trim();
        }

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
            catch { }
        }

        #region Load / Prepare
        public async Task LoadContestantsForEventAsync(string eventId)
        {
            CurrentEventId = eventId;
            await EnsureEventNameAsync();

            ClearAllContestantControls();

            var contestantsCol = Database.GetCollection<ContestantModel>("Contestants");
            var existingContestants = await contestantsCol
                .Find(c => c.EventId == eventId)
                .SortBy(c => c.Number)
                .ToListAsync();

            var mode = cbContestType.SelectedItem?.ToString();

            foreach (var c in existingContestants)
            {
                var ctrl = CreateContestantControl();
                ctrl.SetNumber(c.Number);
                ctrl.FullNameTextBox.Text = c.FullName;
                ctrl.RepresentingTextBox.Text = c.Representing;

                if (mode == "Mixed") ctrl.ConfigureGenderMixedNoDefault();
                else if (mode == "Male" || mode == "Female") ctrl.ConfigureGenderFixed(mode);
                else ctrl.ConfigureGenderOpen();

                if (c.Age.HasValue)
                    ctrl.AgeTextBox.Text = c.Age.Value.ToString();
                ctrl.SetPhoto(c.PhotoPath ?? "");

                if (!string.IsNullOrWhiteSpace(c.Advocacy))
                {
                    ctrl.AdvocacyTextBox.Text = c.Advocacy;
                    MarkTouched(ctrl.AdvocacyTextBox);
                }

                MarkTouched(ctrl.FullNameTextBox);
                MarkTouched(ctrl.RepresentingTextBox);
                if (!string.IsNullOrWhiteSpace(ctrl.AgeTextBox.Text))
                    MarkTouched(ctrl.AgeTextBox);

                InsertContestantControl(ctrl, mode);
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
            ResizeAllContestantControlsToFlowWidth();
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

            var mode = cbContestType.SelectedItem?.ToString();
            var ctrl = CreateContestantControl();

            InsertContestantControl(ctrl, mode);
            RenumberContestants();
            ApplyGenderMode(ctrl);
            ResizeContestantToFlowWidth(ctrl);
        }

        private AddContestantControl CreateContestantControl()
        {
            var ctrl = new AddContestantControl();
            ctrl.RemoveRequested += Ctrl_RemoveRequested;
            ctrl.AddPhotoRequested += Ctrl_AddPhotoRequestedAsync;
            ctrl.GenderChanged += Ctrl_GenderChanged;

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
            };

            ctrl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            ctrl.AutoSize = false;
            ctrl.Height = 212;
            ctrl.Margin = new Padding(6);

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

        private void InsertContestantControl(AddContestantControl ctrl, string? mode)
        {
            if (_flowMainC == null)
                return;

            if (string.Equals(mode, "Mixed", StringComparison.OrdinalIgnoreCase))
            {
                // No default gender -> place under pnlSpaceAboveC until selected
                var gender = NormalizeGender(ctrl.Gender);
                bool hasGender = gender == "Male" || gender == "Female";

                if (!hasGender)
                {
                    int sepIndexGeneral = GetControlIndex(_flowMainC, pnlSpaceAboveC);
                    if (sepIndexGeneral < 0)
                    {
                        _flowMainC.Controls.Add(ctrl);
                    }
                    else
                    {
                        _flowMainC.Controls.Add(ctrl);
                        _flowMainC.Controls.SetChildIndex(ctrl, sepIndexGeneral + 1);
                    }
                }
                else
                {
                    Control sep = gender == "Female" ? _pnlFemale : _pnlMale;
                    int sepIndex = GetControlIndex(_flowMainC, sep);
                    if (sepIndex < 0)
                    {
                        _flowMainC.Controls.Add(ctrl);
                    }
                    else
                    {
                        _flowMainC.Controls.Add(ctrl);
                        _flowMainC.Controls.SetChildIndex(ctrl, sepIndex + 1);
                    }
                }
            }
            else
            {
                int sepIndex = GetControlIndex(_flowMainC, pnlSpaceAboveC);
                if (sepIndex < 0)
                {
                    _flowMainC.Controls.Add(ctrl);
                }
                else
                {
                    _flowMainC.Controls.Add(ctrl);
                    _flowMainC.Controls.SetChildIndex(ctrl, sepIndex + 1);
                }
            }
        }

        private int GetControlIndex(Control container, Control child)
        {
            if (container == null || child == null) return -1;
            return container.Controls.IndexOf(child);
        }

        private void Ctrl_RemoveRequested(object sender, EventArgs e)
        {
            if (sender is AddContestantControl acc)
            {
                var parent = acc.Parent;
                parent?.Controls.Remove(acc);
                acc.Dispose();
                RenumberContestants();
                DebouncedValidate();
                ResizeAllContestantControlsToFlowWidth();
            }
        }

        private void Ctrl_GenderChanged(object sender, EventArgs e)
        {
            if (_flowMainC == null) return;
            if (sender is not AddContestantControl acc) return;

            var mode = cbContestType.SelectedItem?.ToString();

            // If not Mixed, ignore
            if (!string.Equals(mode, "Mixed", StringComparison.OrdinalIgnoreCase))
                return;

            // Remove from current spot
            var parent = acc.Parent;
            parent?.Controls.Remove(acc);

            var gender = NormalizeGender(acc.Gender);
            bool hasGender = gender == "Male" || gender == "Female";

            if (!hasGender)
            {
                // Put back under pnlSpaceAboveC until gender selected
                int sepIndexGeneral = GetControlIndex(_flowMainC, pnlSpaceAboveC);
                _flowMainC.Controls.Add(acc);
                if (sepIndexGeneral >= 0)
                    _flowMainC.Controls.SetChildIndex(acc, sepIndexGeneral + 1);
            }
            else
            {
                Control sep = gender == "Female" ? _pnlFemale : _pnlMale;
                int sepIndex = GetControlIndex(_flowMainC, sep);
                _flowMainC.Controls.Add(acc);
                if (sepIndex >= 0)
                    _flowMainC.Controls.SetChildIndex(acc, sepIndex + 1);
            }

            RenumberContestants();
            DebouncedValidate();
            ResizeContestantToFlowWidth(acc);
        }
        #endregion

        #region Photo
        private async void Ctrl_AddPhotoRequestedAsync(object sender, EventArgs e)
        {
            if (sender is not AddContestantControl acc) return;

            if (string.IsNullOrWhiteSpace(acc.FullName))
            {
                MessageBox.Show("Enter Full Name before adding a photo.", "Name Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

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
                string folder = PathHelpers.ContestantPhotosFolder;
                Directory.CreateDirectory(folder);

                string safeName = $"{PathHelpers.MakeSafeFilename(acc.FullName)}_{acc.Number}";
                string fileName = $"{safeName}.jpg";

                string absPath = Path.Combine(folder, fileName);
                string tempPath = Path.Combine(folder, $"{safeName}_{Guid.NewGuid():N}.tmp.jpg");

                img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Jpeg);

                if (File.Exists(absPath)) File.Delete(absPath);
                File.Move(tempPath, absPath);

                string relPhotoPath = Path.Combine("Contestants", fileName).Replace('\\', '/');

                acc.SetPhoto(relPhotoPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to process image: {ex.Message}", "Photo Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Validation and Numbering
        private IEnumerable<AddContestantControl> GetContestantControls()
        {
            if (_flowMainC == null) return Enumerable.Empty<AddContestantControl>();
            return _flowMainC.Controls.OfType<AddContestantControl>();
        }

        private void ClearAllContestantControls()
        {
            if (_flowMainC != null)
            {
                var existing = _flowMainC.Controls.OfType<AddContestantControl>().ToList();
                foreach (var x in existing)
                {
                    _flowMainC.Controls.Remove(x);
                    x.Dispose();
                }
            }
        }

        private void RenumberContestants()
        {
            var mode = cbContestType.SelectedItem?.ToString();

            if (!string.Equals(mode, "Mixed", StringComparison.OrdinalIgnoreCase))
            {
                int n = 1;
                foreach (var c in GetContestantControls())
                    c.SetNumber(n++);
                return;
            }

            if (_flowMainC == null) return;

            int male = 1;
            int female = 1;
            bool inMale = false;
            bool inFemale = false;

            foreach (Control ctrl in _flowMainC.Controls)
            {
                if (ctrl == _pnlMale)
                {
                    inMale = true;
                    inFemale = false;
                    continue;
                }
                if (ctrl == _pnlFemale)
                {
                    inFemale = true;
                    inMale = false;
                    continue;
                }
                if (ctrl == pnlSpaceAboveC)
                {
                    // general section before choosing gender
                    inMale = false;
                    inFemale = false;
                    continue;
                }

                if (ctrl is AddContestantControl acc)
                {
                    if (inMale)
                        acc.SetNumber(male++);
                    else if (inFemale)
                        acc.SetNumber(female++);
                    else
                        acc.SetNumber(male++); // general section uses male counter for display continuity
                }
            }
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
                    ctrl.SetReqGenderVisible(false);
                    break;
                case "Female":
                    ctrl.ConfigureGenderFixed("Female");
                    ctrl.SetReqGenderVisible(false);
                    break;
                case "Mixed":
                    ctrl.ConfigureGenderMixedNoDefault();
                    ctrl.SetReqGenderVisible(true);
                    break;
                case "Open":
                    ctrl.ConfigureGenderOpen();
                    ctrl.SetReqGenderVisible(false);
                    break;
            }
        }

        private string NormalizeGender(string? gender)
        {
            if (string.IsNullOrWhiteSpace(gender)) return null;
            var g = gender.Trim();
            if (g.Equals("Male", StringComparison.OrdinalIgnoreCase)) return "Male";
            if (g.Equals("Female", StringComparison.OrdinalIgnoreCase)) return "Female";
            return null;
        }

        private bool IsTouched(TextBox tb) => _touched.Contains(tb);

        private void ValidateAll()
        {
            if (IsDisposed) return;
            foreach (var c in GetContestantControls())
            {
                _errorProvider.SetError(c.FullNameTextBox, "");
                _errorProvider.SetError(c.RepresentingTextBox, "");
                _errorProvider.SetError(c.AgeTextBox, "");
            }

            var list = GetContestantControls().ToList();

            foreach (var c in list)
            {
                if (IsTouched(c.FullNameTextBox))
                {
                    if (string.IsNullOrWhiteSpace(c.FullName))
                        _errorProvider.SetError(c.FullNameTextBox, "Full Name required");
                }

                if (IsTouched(c.RepresentingTextBox))
                {
                    if (string.IsNullOrWhiteSpace(c.Representing))
                        _errorProvider.SetError(c.RepresentingTextBox, "Representing required");
                }

                if (IsTouched(c.AgeTextBox))
                {
                    if (!string.IsNullOrWhiteSpace(c.AgeRaw) && !int.TryParse(c.AgeRaw, out _))
                        _errorProvider.SetError(c.AgeTextBox, "Whole number only");
                }
            }

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
        #endregion

        #region Width helpers
        private void ResizeAllContestantControlsToFlowWidth()
        {
            if (_flowMainC == null) return;
            int usable = Math.Max(100, _flowMainC.ClientSize.Width - 12);
            foreach (var acc in _flowMainC.Controls.OfType<AddContestantControl>())
                ResizeContestantToFlowWidth(acc, usable);
        }

        private void ResizeContestantToFlowWidth(AddContestantControl acc, int? widthOverride = null)
        {
            if (_flowMainC == null || acc == null) return;
            int usable = widthOverride ?? Math.Max(100, _flowMainC.ClientSize.Width - 12);
            acc.Width = usable;
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

        private bool IsValidForSave(out string message)
        {
            message = "";
            var list = GetContestantControls().ToList();
            if (list.Count == 0)
            {
                message = "No contestants added.";
                return false;
            }

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

        #region Layout helpers
        private void ApplyLayoutForContestType()
        {
            var mode = cbContestType.SelectedItem?.ToString();
            bool mixed = string.Equals(mode, "Mixed", StringComparison.OrdinalIgnoreCase);

            if (_pnlMale != null) _pnlMale.Visible = mixed;
            if (_pnlFemale != null) _pnlFemale.Visible = mixed;

            // lblReqGen visibility handled per control in ApplyGenderMode
        }
        #endregion
    }
}