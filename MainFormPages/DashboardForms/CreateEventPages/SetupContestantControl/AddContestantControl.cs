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
        }

        private void ConfigureDefaults()
        {
            _txtConNo.ReadOnly = true;
            _txtAdvocacy.Multiline = true;
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
                if (_cbGender.Enabled && _cbGender.SelectedItem != null)
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
        }

        public void ConfigureGenderMixed()
        {
            _cbGender.Enabled = true;
            _cbGender.Text = "Gender";
            _cbGender.Items.Clear();
            _cbGender.Items.AddRange(new object[] { "Male", "Female" });
            _cbGender.SelectedIndex = 0;
            _cbGender.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public void ConfigureGenderOpen()
        {
            _cbGender.Enabled = true;
            _cbGender.Text = "Gender";
            _cbGender.Items.Clear();
            _cbGender.Items.AddRange(new object[] { "Female", "Male", "Non-binary", "Other:" });
            _cbGender.SelectedIndex = 0;
            _cbGender.DropDownStyle = ComboBoxStyle.DropDown;
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
                using (var temp = Image.FromStream(fs))
                {
                    picPortrait.Image = new Bitmap(temp);
                }
            }
            catch
            {
                // ignore if cannot load
            }
        }

        public TextBox FullNameTextBox => _txtFullName;
        public TextBox RepresentingTextBox => _txtRep;
        public TextBox AgeTextBox => _txtAge;
        public TextBox NumberTextBox => _txtConNo;
        public TextBox AdvocacyTextBox => _txtAdvocacy;
    }
}