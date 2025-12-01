using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NexScore.CreateEventPages.SetupJudgesControl;
using NexScore.Models;
using MongoDB.Driver;
using NexScore.Helpers;
using NexScore.Utils;

namespace NexScore.CreateEventPages
{
    public partial class SetupJudges : UserControl
    {
        public string CurrentEventId { get; set; }

        private readonly ToolTip _toolTipJudges = new ToolTip();
        private readonly ErrorProvider _errorProvider = new ErrorProvider();
        private bool _isSavedOnce = false;

        public event Action<bool>? JudgesValidityChanged;

        private const string NamePlaceholder = "Judge's Full Name";
        private const string TitlePlaceholder = "Title/Position: e.g., Dr., Ms., Engr. (Optional)";

        public SetupJudges()
        {
            InitializeComponent();
            _toolTipJudges.SetToolTip(btnSaveJudges, "Save judges");

            btnAddJudge.Click += (_, __) =>
            {
                var judge = CreateJudgeControl();
                _flowMainJ.Controls.Add(judge);
                _flowMainJ.Controls.SetChildIndex(judge, _flowMainJ.Controls.IndexOf(pnlSpaceAboveJ) + 1);
                RefreshJudgeNumbers();
                ValidateJudgesPage();
            };

            btnSaveJudges.Click += btnSaveJudges_Click;
        }

        public void SetEditMode(bool isEditMode)
        {
            if (isEditMode)
            {
                _isSavedOnce = true;
                btnSaveJudges.Text = "update";
                _toolTipJudges.SetToolTip(btnSaveJudges, "Update judges");
                
                btnAddJudge.Visible = true;
                btnAddJudge.Enabled = true;
            }
            else
            {
                btnSaveJudges.Text = "save";
                _toolTipJudges.SetToolTip(btnSaveJudges, "Save judges");
            }
        }

        private AddJudgeControl CreateJudgeControl()
        {
            var ctrl = new AddJudgeControl();
            ctrl.DeleteRequested += Judge_DeleteRequested;

            ctrl.txtJudgeName.TextChanged += (s, e) =>
            {
                ValidateSingleJudgeName(ctrl.txtJudgeName);
                ValidateJudgesPage();
            };
            
            return ctrl;
        }

        public bool AreJudgesValid()
        {
            var judges = _flowMainJ.Controls.OfType<AddJudgeControl>().ToList();
            if (judges.Count == 0) return false;

            if (judges.Any(j =>
                string.IsNullOrWhiteSpace(j.txtJudgeName.Text) ||
                j.txtJudgeName.Text == NamePlaceholder))
                return false;

            if (!JudgesUniquenessHelper.ValidateUniqueJudgeNames(_flowMainJ, _errorProvider, NamePlaceholder))
                return false;

            if (judges.Any(j =>
            {
                var err = _errorProvider.GetError(j.txtJudgeName);
                return !string.IsNullOrEmpty(err) && err != "Duplicate judge name";
            }))
                return false;

            return true;
        }

        private void ValidateSingleJudgeName(TextBox tb)
        {
            if (_errorProvider.GetError(tb) != "Duplicate judge name")
                _errorProvider.SetError(tb, "");

            if (tb.Text == NamePlaceholder || string.IsNullOrWhiteSpace(tb.Text))
            {
                _errorProvider.SetError(tb, "Judge name required");
                return;
            }

            if (!InputValidator.ValidateName(tb))
            {
                _errorProvider.SetError(tb, "Invalid characters");
                return;
            }

            _errorProvider.SetError(tb, "");
            JudgesUniquenessHelper.ValidateUniqueJudgeNames(_flowMainJ, _errorProvider, NamePlaceholder);
        }

        private void Judge_DeleteRequested(AddJudgeControl judge)
        {
            _flowMainJ.Controls.Remove(judge);
            judge.Dispose();
            RefreshJudgeNumbers();
            JudgesUniquenessHelper.ValidateUniqueJudgeNames(_flowMainJ, _errorProvider, NamePlaceholder);
            ValidateJudgesPage();
        }

        private void RefreshJudgeNumbers()
        {
            int n = 1;
            foreach (var jc in _flowMainJ.Controls.OfType<AddJudgeControl>())
                jc.txtJudgeNo.Text = n++.ToString();
        }

        public async void LoadJudgesForEvent(string eventId)
        {
            CurrentEventId = eventId;

            _flowMainJ.SuspendLayout();
            _flowMainJ.Controls.Clear();
            _flowMainJ.Controls.Add(pnlSpaceAboveJ);
            _flowMainJ.Controls.Add(btnAddJudge);
            _flowMainJ.Controls.Add(pnlSpaceBelowJ);

            var existingJudges = await Database.Judges
                .Find(j => j.EventId == eventId)
                .SortBy(j => j.Number)
                .ToListAsync();

            foreach (var j in existingJudges)
            {
                var ctrl = CreateJudgeControl();
                ctrl.txtJudgeName.Text = string.IsNullOrWhiteSpace(j.Name) ? NamePlaceholder : j.Name;
                ctrl.txtJudgeTitle.Text = string.IsNullOrWhiteSpace(j.Title) ? TitlePlaceholder : j.Title;
                ctrl.Tag = j;
                _flowMainJ.Controls.Add(ctrl);
                _flowMainJ.Controls.SetChildIndex(ctrl, _flowMainJ.Controls.IndexOf(pnlSpaceAboveJ) + 1);
            }

            _flowMainJ.ResumeLayout();

            RefreshJudgeNumbers();

            if (existingJudges.Any())
            {
                btnSaveJudges.Text = "update";
                _toolTipJudges.SetToolTip(btnSaveJudges, "Update judges");
                _isSavedOnce = true;
            }
            else
            {
                btnSaveJudges.Text = "save";
                _toolTipJudges.SetToolTip(btnSaveJudges, "Save judges");
                _isSavedOnce = false;
            }

            btnAddJudge.Visible = true;
            btnAddJudge.Enabled = true;

            ValidateJudgesPage();
        }

        private void ValidateJudgesPage()
        {
            JudgesValidityChanged?.Invoke(AreJudgesValid());
        }

        private async void btnSaveJudges_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentEventId))
            {
                _toolTipJudges.Show("No event selected (EventId empty).", btnSaveJudges, 10, -30, 2500);
                return;
            }

            if (!AreJudgesValid())
            {
                _toolTipJudges.Show("Fix judge names (required, unique).", btnSaveJudges, 10, -30, 2500);
                return;
            }

            var judgeControls = _flowMainJ.Controls.OfType<AddJudgeControl>().ToList();
            var existingByName = (await Database.Judges
                .Find(j => j.EventId == CurrentEventId)
                .ToListAsync())
                .ToDictionary(j => j.Name?.Trim() ?? "", j => j, StringComparer.OrdinalIgnoreCase);

            var newJudges = new List<JudgeModel>();
            var usedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var jc in judgeControls)
            {
                var name = jc.txtJudgeName.Text?.Trim();
                if (string.IsNullOrEmpty(name) || name == NamePlaceholder) continue;

                string judgeId;
                if (existingByName.TryGetValue(name, out var existing))
                {
                    judgeId = existing.JudgeId;
                }
                else
                {
                    judgeId = SafeId.NewId(10);
                    while (!usedIds.Add(judgeId))
                        judgeId = SafeId.NewId(10);
                }

                var numberText = jc.txtJudgeNo.Text;
                var title = jc.txtJudgeTitle.Text == TitlePlaceholder ? "" : jc.txtJudgeTitle.Text.Trim();

                newJudges.Add(new JudgeModel
                {
                    EventId = CurrentEventId,
                    JudgeId = judgeId,
                    Name = name,
                    Number = numberText,
                    Title = title
                });
            }

            if (newJudges.Count == 0)
            {
                _toolTipJudges.Show("Provide at least one judge.", btnSaveJudges, 10, -30, 2500);
                return;
            }

            await Database.Judges.DeleteManyAsync(j => j.EventId == CurrentEventId);
            await Database.Judges.InsertManyAsync(newJudges);

            bool first = !_isSavedOnce;
            _isSavedOnce = true;
            btnSaveJudges.Text = "update";
            _toolTipJudges.Show(first ? "Judges saved!" : "Judges updated!", btnSaveJudges, 10, -30, 2000);

            ValidateJudgesPage();
        }
    }
}