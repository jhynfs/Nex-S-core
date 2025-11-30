using MongoDB.Driver;
using NexScore.Models;
using NexScore.Helpers;

using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NexScore
{
    public partial class SetupDetails : UserControl
    {
        public string? CurrentEventId { get; set; }
        public bool IsSaved { get; set; } = false;
        private readonly ToolTip _toolTipDetails = new ToolTip();
        public event Action<bool>? SavedStatusChanged;
        public event Action<string>? EventIdGenerated;
        public event Action<string, string>? EventContextReady;

        private string? _bannerPath;
        private bool _bannerRemoved = false;

        private const string EventNamePlaceholder = "Your Event's Name";
        private const string DescriptionPlaceholder = "Your Event Description Here (Optional)";
        private const string VenuePlaceholder = "Your Event's Venue (Optional)";
        private const string OrganizersPlaceholder = "Your Event's Organizers/Sponsors (Optional)";

        public TextBox txtEventName => _txtEventName;
        public DateTimePicker datePicker => _datePicker;
        public TextBox txtDescription => _txtDescription;
        public TextBox txtVenue => _txtVenue;
        public TextBox txtOrganizers => _txtOrganizers;

        public SetupDetails()
        {
            InitializeComponent();

            TextboxHelper.SetPlaceholder(_txtEventName, EventNamePlaceholder);
            TextboxHelper.SetPlaceholder(_txtDescription, DescriptionPlaceholder);
            TextboxHelper.SetPlaceholder(_txtVenue, VenuePlaceholder);
            TextboxHelper.SetPlaceholder(_txtOrganizers, OrganizersPlaceholder);
            btnUpdateDetails.Visible = false;
        }

        public void SetEditMode(bool isEditMode)
        {
            if (isEditMode)
            {
                IsSaved = true;
                btnSaveDetails.Visible = false;
                btnUpdateDetails.Visible = true;
            }
            else
            {
                btnSaveDetails.Visible = !IsSaved;
                btnUpdateDetails.Visible = IsSaved;
            }
        }

        private bool IsEventNameValid()
        {
            var text = txtEventName.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text)) return false;
            if (string.Equals(text, EventNamePlaceholder, StringComparison.Ordinal)) return false;
            return true;
        }

        private static string NormalizeOptional(string? text, string placeholder)
        {
            var t = text?.Trim();
            if (string.IsNullOrWhiteSpace(t)) return string.Empty;
            if (string.Equals(t, placeholder, StringComparison.Ordinal)) return string.Empty;
            return t;
        }

        public async Task<bool> SaveEventDetailsAsync()
        {
            if (!IsEventNameValid())
                return false;

            var collection = Database.GetCollection<EventModel>("Events");
            if (string.IsNullOrWhiteSpace(CurrentEventId))
                CurrentEventId = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

            var existing = await collection.Find(e => e.Id == CurrentEventId).FirstOrDefaultAsync();
            string? previousBannerPath = existing?.BannerPath;

            var newEvent = new EventModel
            {
                Id = CurrentEventId!,
                EventName = txtEventName.Text.Trim(),
                EventDate = datePicker.Value,
                CreatedAt = existing?.CreatedAt ?? DateTime.Now,
                Description = NormalizeOptional(_txtDescription.Text, DescriptionPlaceholder),
                Venue = NormalizeOptional(_txtVenue.Text, VenuePlaceholder),
                Organizers = NormalizeOptional(_txtOrganizers.Text, OrganizersPlaceholder),
                BannerPath = _bannerRemoved
                    ? null
                    : (_bannerPath ?? existing?.BannerPath)
            };

            await collection.ReplaceOneAsync(
                e => e.Id == CurrentEventId,
                newEvent,
                new ReplaceOptions { IsUpsert = true });

            if (_bannerRemoved && !string.IsNullOrWhiteSpace(previousBannerPath))
            {
                try
                {
                    var absOld = PathHelpers.AbsoluteFromRelative(previousBannerPath);
                    if (File.Exists(absOld))
                        File.Delete(absOld);
                }
                catch { }
                _bannerRemoved = false;
            }

            return true;
        }

        public async Task LoadExistingEventAsync(string eventId)
        {
            CurrentEventId = eventId;
            var collection = Database.GetCollection<EventModel>("Events");
            var existing = await collection.Find(e => e.Id == eventId).FirstOrDefaultAsync();
            if (existing != null)
            {
                _txtEventName.Text = existing.EventName;
                datePicker.Value = existing.EventDate;
                _txtDescription.Text = existing.Description ?? string.Empty;
                _txtVenue.Text = existing.Venue ?? string.Empty;
                _txtOrganizers.Text = existing.Organizers ?? string.Empty;

                _bannerPath = existing.BannerPath;
                _bannerRemoved = false;

                if (lblOpt != null)
                    lblOpt.Text = string.IsNullOrWhiteSpace(_bannerPath) ? "(no banner)" : Path.GetFileName(_bannerPath);

                IsSaved = true;
                btnSaveDetails.Visible = false;
                btnUpdateDetails.Visible = true;
            }
        }

        private string GetManagedBannerFolder()
        {
            return PathHelpers.EventsFolder;
        }

        #region Buttons
        private async void btnSaveDetails_Click(object sender, EventArgs e)
        {
            if (!IsEventNameValid())
            {
                MessageBox.Show("Please enter an event name.",
                                "missing event name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var saved = await SaveEventDetailsAsync();
            if (!saved)
            {
                MessageBox.Show("Unable to save event. Please check your inputs.",
                                "save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IsSaved = true;
            SavedStatusChanged?.Invoke(true);
            _toolTipDetails.Show("Event details saved!", btnSaveDetails, 10, -30, 2000);

            btnSaveDetails.Visible = false;
            btnUpdateDetails.Visible = true;
            EventIdGenerated?.Invoke(CurrentEventId!);
            EventContextReady?.Invoke(CurrentEventId!, _txtEventName.Text.Trim());
        }

        private async void btnUpdateDetails_Click(object sender, EventArgs e)
        {
            if (!IsEventNameValid())
            {
                MessageBox.Show("Please enter an event name.",
                                "missing event name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var saved = await SaveEventDetailsAsync();
            if (!saved)
            {
                MessageBox.Show("Unable to update event. Please check your inputs.",
                                "update error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _toolTipDetails.Show("Event details updated!", btnUpdateDetails, 10, -30, 2000);

            EventContextReady?.Invoke(CurrentEventId!, _txtEventName.Text.Trim());
        }

        private async void btnAddBanner_Click(object sender, EventArgs e)
        {
            if (!IsEventNameValid())
            {
                MessageBox.Show("Please enter an event name before adding a banner.",
                                "missing event name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var ofd = new OpenFileDialog
            {
                Title = "Select 4:1 Event Banner (will be resized to 1600x400 and saved as JPEG)",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            int width, height;
            try
            {
                using var probe = ImageBannerHelper.LoadClone(ofd.FileName);
                width = probe.Width;
                height = probe.Height;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not read image: {ex.Message}",
                                "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!ImageBannerHelper.IsFourToOne(width, height, ImageBannerHelper.AspectTolerance))
            {
                double ratio = (double)width / height;
                MessageBox.Show(
                    $"Selected image is {width}x{height} (ratio {ratio:0.###}). It must be 4:1. Please choose another image.",
                    "Invalid Banner", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var managedFolder = GetManagedBannerFolder();
                Directory.CreateDirectory(managedFolder);
                string eventName = _txtEventName.Text.Trim();
                string safeEvent = PathHelpers.MakeSafeFilename(eventName);
                string fileName = $"{safeEvent}_banner.jpg";
                string absPath = Path.Combine(managedFolder, fileName);

                await Task.Run(() =>
                {
                    using (var img = ImageBannerHelper.LoadClone(ofd.FileName))
                    {
                        ImageBannerHelper.SaveResizedBanner(img, absPath, 1600, 400, 85L);
                    }
                });

                _bannerPath = Path.Combine("Events", fileName).Replace('\\', '/');
                _bannerRemoved = false;

                if (lblOpt != null)
                    lblOpt.Text = Path.GetFileName(_bannerPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to finalize banner: {ex.Message}",
                                "Banner Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemoveBanner_Click(object sender, EventArgs e)
        {
            _bannerRemoved = true;
            _bannerPath = null;

            if (lblOpt != null)
                lblOpt.Text = "(Optional)";
        }
        #endregion
    }
}