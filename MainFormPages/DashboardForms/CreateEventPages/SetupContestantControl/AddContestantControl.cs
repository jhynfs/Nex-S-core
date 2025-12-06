using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using NexScore.Helpers;

namespace NexScore.CreateEventPages.SetupContestantControls
{
    public partial class AddContestantControl : UserControl
    {
        public event EventHandler RemoveRequested;
        public event EventHandler AddPhotoRequested;
        public event EventHandler GenderChanged;

        private readonly Dictionary<TextBox, string> _placeholders = new();

        public AddContestantControl()
        {
            InitializeComponent();
            HookEvents();
            ConfigureDefaults();
            InitializePlaceholders();
        }

        private void InitializePlaceholders()
        {
            SetupPlaceholder(_txtFullName, "Contestant's Full Name");
            SetupPlaceholder(_txtRep, "e.g., LGU, School, Organization");
            SetupPlaceholder(_txtAge, "Age");
            SetupPlaceholder(_txtAdvocacy, "Contestant's Advocacy (Optional)");
        }

        private void SetupPlaceholder(TextBox tb, string placeholder)
        {
            _placeholders[tb] = placeholder;
            tb.ForeColor = Color.Gray;
            tb.Text = placeholder;

            tb.GotFocus += (s, e) =>
            {
                if (IsPlaceholder(tb))
                {
                    tb.Text = "";
                    tb.ForeColor = SystemColors.WindowText;
                }
            };

            tb.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Text = placeholder;
                    tb.ForeColor = Color.Gray;
                }
            };
        }

        public bool IsPlaceholder(TextBox tb) =>
            _placeholders.TryGetValue(tb, out var ph) &&
            tb.Text == ph &&
            tb.ForeColor == Color.Gray;

        private void HookEvents()
        {
            btnRemoveCon.Click += (s, e) => RemoveRequested?.Invoke(this, EventArgs.Empty);
            btnAddPhoto.Click += (s, e) => AddPhotoRequested?.Invoke(this, EventArgs.Empty);

            _txtAge.KeyPress += (s, e) =>
            {
                if (char.IsControl(e.KeyChar)) return;
                if (!char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };

            _cbGender.SelectedIndexChanged += (s, e) => GenderChanged?.Invoke(this, EventArgs.Empty);
            _cbGender.TextChanged += (s, e) =>
            {
                if (_cbGender.DropDownStyle == ComboBoxStyle.DropDown)
                    GenderChanged?.Invoke(this, EventArgs.Empty);
            };
        }

        private void ConfigureDefaults()
        {
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            this.AutoSize = false;
            this.AutoSizeMode = AutoSizeMode.GrowOnly;
            this.Margin = new Padding(6);
            this.Padding = new Padding(6);

            this.MinimumSize = new Size(320, 212);
            this.Size = new Size(749, 212);
            this.Height = 212;

            _txtConNo.ReadOnly = true;
            _txtAdvocacy.Multiline = true;
            _txtAdvocacy.ScrollBars = ScrollBars.Vertical;

            _txtFullName.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            _txtRep.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            _txtAge.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            _txtAdvocacy.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            _cbGender.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            picPortrait.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            // lblReqGen is hidden by default; shown only when contest type is Mixed
            if (lblReqGen != null)
                lblReqGen.Visible = false;
        }

        public int Number => int.TryParse(_txtConNo.Text, out var n) ? n : 0;
        public string FullName => IsPlaceholder(_txtFullName) ? "" : _txtFullName.Text.Trim();
        public string Representing => IsPlaceholder(_txtRep) ? "" : _txtRep.Text.Trim();
        public string AgeRaw => IsPlaceholder(_txtAge) ? "" : _txtAge.Text.Trim();
        public string Advocacy => IsPlaceholder(_txtAdvocacy) ? "" : _txtAdvocacy.Text.Trim();
        public string PhotoPath { get; private set; }

        public string Gender
        {
            get
            {
                if (_cbGender.Enabled && _cbGender.DropDownStyle == ComboBoxStyle.DropDown)
                {
                    var t = _cbGender.Text?.Trim();
                    return string.IsNullOrWhiteSpace(t) ? null : t;
                }
                if (_cbGender.Enabled && _cbGender.SelectedIndex >= 0 && _cbGender.SelectedItem != null)
                    return _cbGender.SelectedItem.ToString();
                if (!_cbGender.Enabled && _cbGender.Items.Count == 1)
                    return _cbGender.Items[0].ToString();
                return null;
            }
        }

        public void SetNumber(int number) => _txtConNo.Text = number.ToString();

        public void ConfigureGenderFixed(string gender)
        {
            _cbGender.Items.Clear();
            _cbGender.Items.Add(gender);
            _cbGender.SelectedIndex = 0;
            _cbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            _cbGender.Enabled = false;

            SetReqGenderVisible(false);
        }

        public void ConfigureGenderMixedNoDefault()
        {
            _cbGender.Enabled = true;
            _cbGender.Items.Clear();
            _cbGender.Items.AddRange(new object[] { "Male", "Female" });
            _cbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            _cbGender.SelectedIndex = -1; // no default selection
            _cbGender.Text = "Select gender"; // hint text

            SetReqGenderVisible(true);
        }

        public void ConfigureGenderOpen()
        {
            _cbGender.Enabled = true;
            _cbGender.Items.Clear();
            _cbGender.Items.AddRange(new object[] { "Female", "Male", "Non-binary", "Other:" });
            _cbGender.DropDownStyle = ComboBoxStyle.DropDown;
            _cbGender.SelectedIndex = -1;
            _cbGender.Text = "Gender";

            SetReqGenderVisible(false);
        }

        public void SetReqGenderVisible(bool visible)
        {
            try { if (lblReqGen != null) lblReqGen.Visible = visible; } catch { }
        }

        public void SetPhoto(string relativePath)
        {
            PhotoPath = relativePath;
            try
            {
                string absolutePath = PathHelpers.AbsoluteFromRelative(relativePath);
                if (string.IsNullOrWhiteSpace(absolutePath) || !File.Exists(absolutePath))
                    return;
                if (picPortrait.Image != null)
                {
                    var old = picPortrait.Image;
                    picPortrait.Image = null;
                    old.Dispose();
                }
                using (var fs = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var temp = System.Drawing.Image.FromStream(fs))
                {
                    picPortrait.Image = new Bitmap(temp);
                }
            }
            catch
            {
                // ignore image load failures
            }
        }

        public TextBox FullNameTextBox => _txtFullName;
        public TextBox RepresentingTextBox => _txtRep;
        public TextBox AgeTextBox => _txtAge;
        public TextBox NumberTextBox => _txtConNo;
        public TextBox AdvocacyTextBox => _txtAdvocacy;
    }
}