using MongoDB.Driver;
using NexScore.CreateEventPages.SetupCriteriaControls;
using NexScore.Helpers;
using NexScore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using MongoDB.Bson;

namespace NexScore
{
    public partial class SetupCriteria : UserControl
    {
        private readonly ToolTip _toolTipCriteria = new ToolTip();
        public string CurrentEventId { get; set; }
        public event Action<bool>? CriteriaValidityChanged;
        public event Action<bool>? CriteriaSaved; // notify when save succeeds/fails
        private bool _isSavedOnce;
        private readonly ErrorProvider errorProvider = new ErrorProvider();

        // Guard to avoid double-wiring text boxes
        private readonly HashSet<TextBox> _wiredTextBoxes = new HashSet<TextBox>();

        public SetupCriteria()
        {
            InitializeComponent();
            btnAddPhase.Click += (s, e) =>
            {
                var phase = CreatePhaseControl();
                phase.Dock = DockStyle.Top;
                _flowMain.Controls.Add(phase);
                _flowMain.Controls.SetChildIndex(phase, _flowMain.Controls.IndexOf(panelSpaceBelow));
                UpdatePhaseNumbers();
                UpdateEventTotalWeightLabel();
                RevalidateUniquenessForUI();
                ValidateCriteriaPage();
            };

            btnRefresh.Click += (s, e) =>
            {
                RefreshAllUI();
            };

            _flowMain.ControlAdded += (s, e) => WireTextBoxes(e.Control);
            WireTextBoxes(this);
        }

        // Force "Update" mode in UI
        public void SetEditMode(bool isEditMode)
        {
            if (isEditMode)
            {
                _isSavedOnce = true;
                if (btnSaveCriteria != null)
                    btnSaveCriteria.Text = "update";
            }
        }

        // Load the structure from DB and rebuild the UI (now with cleanup of auto-added blank segment/criteria)
        public async Task LoadEventStructureAsync(string eventId)
        {
            CurrentEventId = eventId;

            var col = Database.GetCollection<EventStructureModel>("EventStructures");
            var existing = await col.Find(s => s.EventId == eventId).FirstOrDefaultAsync();

            // Remove all existing PhaseControls first
            foreach (var p in _flowMain.Controls.OfType<PhaseControl>().ToList())
            {
                _flowMain.Controls.Remove(p);
                p.Dispose();
            }

            if (existing?.Phases == null || existing.Phases.Count == 0)
            {
                UpdatePhaseNumbers();
                UpdateEventTotalWeightLabel();
                RevalidateUniquenessForUI();
                ValidateCriteriaPage();
                _isSavedOnce = false;
                if (btnSaveCriteria != null) btnSaveCriteria.Text = "save";
                return;
            }

            foreach (var phaseModel in existing.Phases.OrderBy(p => p.Sequence ?? int.MaxValue))
            {
                var phaseCtrl = CreatePhaseControl();

                // The constructor may have auto-added a blank SegmentControl. Remove all current segment controls before loading real ones.
                foreach (var autoSeg in phaseCtrl.flowSegment.Controls.OfType<SegmentControl>().ToList())
                {
                    phaseCtrl.flowSegment.Controls.Remove(autoSeg);
                    autoSeg.Dispose();
                }

                // Assign phase properties
                if (!string.IsNullOrWhiteSpace(phaseModel.Name))
                    phaseCtrl.txtPhaseName.Text = phaseModel.Name;
                phaseCtrl.chkIndependentPhase.Checked = phaseModel.IsIndependent;
                phaseCtrl.txtPhaseWeight.Text = phaseModel.Weight.ToString("0.##");

                // Load segments
                foreach (var segModel in phaseModel.Segments.OrderBy(s => s.Sequence))
                {
                    var segCtrl = CreateSegmentControl(phaseCtrl);

                    // Remove auto-added blank criteria from this new segment before populating
                    foreach (var autoCrit in segCtrl.flowCriteria.Controls.OfType<CriteriaControl>().ToList())
                    {
                        segCtrl.flowCriteria.Controls.Remove(autoCrit);
                        autoCrit.Dispose();
                    }

                    if (!string.IsNullOrWhiteSpace(segModel.Name))
                        segCtrl.txtSegmentName.Text = segModel.Name;
                    segCtrl.txtSegmentWeight.Text = segModel.Weight.ToString("0.##");

                    // Load criteria
                    foreach (var critModel in segModel.Criteria)
                    {
                        var critCtrl = CreateCriteriaControl();
                        if (!string.IsNullOrWhiteSpace(critModel.Name))
                            critCtrl.txtCriteriaName.Text = critModel.Name;
                        critCtrl.txtCriteriaWeight.Text = critModel.Weight.ToString("0.##");
                        segCtrl.flowCriteria.Controls.Add(critCtrl);
                    }

                    segCtrl.UpdateSegmentTotalWeightLabel();
                    phaseCtrl.flowSegment.Controls.Add(segCtrl);
                }

                phaseCtrl.UpdateSegmentNumbers();
                phaseCtrl.UpdatePhaseTotalWeightLabel();

                _flowMain.Controls.Add(phaseCtrl);
                _flowMain.Controls.SetChildIndex(phaseCtrl, _flowMain.Controls.IndexOf(panelSpaceBelow));
            }

            UpdatePhaseNumbers();
            UpdateEventTotalWeightLabel();
            RevalidateUniquenessForUI();
            ValidateCriteriaPage();

            // Reflect loaded state in UI
            _isSavedOnce = true;
            if (btnSaveCriteria != null)
                btnSaveCriteria.Text = "update";
        }

        public void RefreshAllUI()
        {
            UpdatePhaseNumbers();
            foreach (var phase in _flowMain.Controls.OfType<PhaseControl>())
            {
                foreach (var seg in phase.flowSegment.Controls.OfType<SegmentControl>())
                {
                    seg.UpdateSegmentTotalWeightLabel();
                }
                phase.UpdatePhaseTotalWeightLabel();
            }
            UpdateEventTotalWeightLabel();
            RevalidateUniquenessForUI();
            ValidateCriteriaPage();
        }

        // Uniqueness
        private bool ValidateUniqueNames() => CriteriaUniquenessHelper.ValidateAll(_flowMain, errorProvider);
        public void RevalidateUniquenessForUI() => ValidateUniqueNames();

        internal void ValidateNameDuplicates(TextBox changed)
        {
            if (changed == null) return;
            var phase = FindPhaseParent(changed);
            var segment = FindSegmentParent(changed);

            if (phase != null && changed == phase.txtPhaseName)
                CriteriaUniquenessHelper.PhaseScope(_flowMain, phase, errorProvider);
            else if (segment != null && changed == segment.txtSegmentName && phase != null)
                CriteriaUniquenessHelper.SegmentScope(phase, segment, errorProvider);
            else if (segment != null && changed.Name.Contains("Criteria"))
                CriteriaUniquenessHelper.CriteriaScope(segment, errorProvider);
        }

        #region Create Controls

        private PhaseControl CreatePhaseControl()
        {
            var phase = new PhaseControl();
            phase.txtPhaseWeight.TextChanged += (s, e) =>
            {
                // Update both event total and this phase's total status (now compared to phase weight)
                UpdateEventTotalWeightLabel();
                phase.UpdatePhaseTotalWeightLabel();
            };
            phase.txtPhaseName.TextChanged += (s, e) => ValidateNameDuplicates(phase.txtPhaseName);

            phase.btnAddSegment.Click += (s, e) =>
            {
                var newSegment = CreateSegmentControl(phase);
                phase.flowSegment.Controls.Add(newSegment);
                phase.UpdateSegmentNumbers();
                phase.UpdatePhaseTotalWeightLabel();
                UpdateEventTotalWeightLabel();
                RevalidateUniquenessForUI();
                ValidateCriteriaPage();
            };

            phase.btnRemovePhase.Click += (s, e) =>
            {
                _flowMain.Controls.Remove(phase);
                UpdatePhaseNumbers();
                UpdateEventTotalWeightLabel();
                RevalidateUniquenessForUI();
                ValidateCriteriaPage();
            };

            return phase;
        }

        private SegmentControl CreateSegmentControl(PhaseControl parentPhase)
        {
            var segment = new SegmentControl();

            segment.txtSegmentWeight.TextChanged += (s, e) =>
            {
                // Threshold changes -> update own label and parent totals
                segment.UpdateSegmentTotalWeightLabel();
                parentPhase.UpdatePhaseTotalWeightLabel();
                UpdateEventTotalWeightLabel();
            };
            segment.txtSegmentName.TextChanged += (s, e) => ValidateNameDuplicates(segment.txtSegmentName);

            segment.btnRemoveSegment.Click += (s, e) =>
            {
                parentPhase.flowSegment.Controls.Remove(segment);
                parentPhase.UpdateSegmentNumbers();
                parentPhase.UpdatePhaseTotalWeightLabel();
                UpdateEventTotalWeightLabel();
                RevalidateUniquenessForUI();
                ValidateCriteriaPage();
            };

            segment.UpdateSegmentTotalWeightLabel();
            return segment;
        }

        private CriteriaControl CreateCriteriaControl()
        {
            var crit = new CriteriaControl();

            crit.btnRemoveCriteria.Click += (s, e) =>
            {
                var seg = crit.Parent as SegmentControl;
                seg?.flowCriteria.Controls.Remove(crit);
                if (seg != null && FindParentPhase(seg) is PhaseControl pc)
                {
                    seg.UpdateSegmentTotalWeightLabel();
                    pc.UpdatePhaseTotalWeightLabel();
                    UpdateEventTotalWeightLabel();
                }
                RevalidateUniquenessForUI();
                ValidateCriteriaPage();
            };

            return crit;
        }

        private PhaseControl? FindParentPhase(SegmentControl segment)
            => _flowMain.Controls.OfType<PhaseControl>().FirstOrDefault(p => p.flowSegment.Controls.Contains(segment));

        #endregion

        #region Validation

        private bool IsPlaceholderOrEmpty(TextBox tb)
            => PlaceholderHelper.IsPlaceholder(tb) || string.IsNullOrWhiteSpace(tb.Text);

        public bool ValidateAllInputs()
        {
            var phases = _flowMain.Controls.OfType<PhaseControl>().ToList();
            if (phases.Count == 0) return false;

            foreach (var phase in phases)
            {
                if (IsPlaceholderOrEmpty(phase.txtPhaseName)) return false;

                var segments = phase.flowSegment.Controls.OfType<SegmentControl>().ToList();
                if (segments.Count == 0) return false;

                foreach (var seg in segments)
                {
                    if (IsPlaceholderOrEmpty(seg.txtSegmentName)) return false;

                    var criteria = seg.flowCriteria.Controls.OfType<CriteriaControl>().ToList();
                    foreach (var c in criteria)
                        if (IsPlaceholderOrEmpty(c.txtCriteriaName)) return false;
                }
            }
            return true;
        }

        // Validation rules:
        // - Event (non-independent phases) must total 100
        // - Each phase's segments must total EXACTLY the phase's weight (independent phases have 100)
        // - Each segment's criteria must total EXACTLY the segment's weight
        public bool ValidateTotals()
        {
            // Event total (non-independent phases)
            decimal eventTotal = _flowMain.Controls.OfType<PhaseControl>()
                .Where(p => !p.chkIndependentPhase.Checked)
                .Sum(p => decimal.TryParse(p.txtPhaseWeight.Text, out var w) ? w : 0m);
            if (eventTotal != 100m) return false;

            foreach (var phase in _flowMain.Controls.OfType<PhaseControl>())
            {
                // Phase weight threshold
                decimal phaseWeight = phase.chkIndependentPhase.Checked
                    ? 100m
                    : (decimal.TryParse(phase.txtPhaseWeight.Text, out var pw) ? pw : 0m);

                // Sum of segments under this phase
                decimal sumSegments = 0m;
                foreach (var seg in phase.flowSegment.Controls.OfType<SegmentControl>())
                {
                    sumSegments += decimal.TryParse(seg.txtSegmentWeight.Text, out var sw) ? sw : 0m;

                    // Criteria sum must equal segment weight
                    var criteriaControls = seg.flowCriteria.Controls.OfType<CriteriaControl>().ToList();
                    if (criteriaControls.Count > 0)
                    {
                        decimal sumCriteria = 0m;
                        foreach (var crit in criteriaControls)
                            sumCriteria += decimal.TryParse(crit.txtCriteriaWeight.Text, out var cw) ? cw : 0m;

                        decimal segWeight = decimal.TryParse(seg.txtSegmentWeight.Text, out var sww) ? sww : 0m;
                        if (sumCriteria != segWeight) return false; // must be exact
                    }
                    else
                    {
                        // no criteria controls present means cannot equal intended segment weight (unless 0), let label logic show red; still enforce exact match
                        decimal segWeight = decimal.TryParse(seg.txtSegmentWeight.Text, out var sww2) ? sww2 : 0m;
                        if (segWeight != 0m) return false;
                    }
                }

                // Segments must equal the phase weight exactly
                if (sumSegments != phaseWeight) return false;
            }
            return true;
        }

        public void ValidateCriteriaPage()
        {
            bool ok = ValidateAllInputs() && ValidateTotals() && ValidateUniqueNames();
            CriteriaValidityChanged?.Invoke(ok);
        }

        #endregion

        #region Save

        public async Task<bool> SaveEventStructureAsync(string? eventId)
        {
            if (string.IsNullOrEmpty(eventId)) return false;

            try
            {
                var raw = Database.GetCollection<BsonDocument>("EventStructures");
                var legacyFilter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Type("eventId", BsonType.String),
                    Builders<BsonDocument>.Filter.Eq("eventId", eventId)
                );
                await raw.DeleteManyAsync(legacyFilter);

                var col = Database.GetCollection<EventStructureModel>("EventStructures");
                var existing = await col.Find(x => x.EventId == eventId).FirstOrDefaultAsync();

                if (existing == null)
                {
                    var doc = new EventStructureModel
                    {
                        EventId = eventId,
                        Phases = GetPhasesFromUI()
                    };
                    await col.InsertOneAsync(doc);
                }
                else
                {
                    var update = Builders<EventStructureModel>.Update
                        .Set(x => x.Phases, GetPhasesFromUI());
                    await col.UpdateOneAsync(x => x.Id == existing.Id, update);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving criteria: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async void btnSaveCriteria_Click(object sender, EventArgs e)
        {
            bool inputsOk = ValidateAllInputs();
            bool totalsOk = ValidateTotals();
            bool uniquesOk = ValidateUniqueNames();

            if (!inputsOk || !totalsOk || !uniquesOk)
            {
                _toolTipCriteria.Show(
                    "Cannot save. Ensure unique names and that: (1) non-independent phases total 100, (2) each phase's segments equal the phase weight, (3) each segment's criteria equal the segment weight.",
                    btnSaveCriteria, 10, -30, 3800);
                CriteriaValidityChanged?.Invoke(false);
                CriteriaSaved?.Invoke(false);
                return;
            }

            bool first = !_isSavedOnce;
            bool saved = false;
            if (!string.IsNullOrEmpty(CurrentEventId))
                saved = await SaveEventStructureAsync(CurrentEventId);

            if (!saved)
            {
                _toolTipCriteria.Show("Save failed. Please try again.",
                    btnSaveCriteria, 10, -30, 2800);
                CriteriaValidityChanged?.Invoke(false);
                CriteriaSaved?.Invoke(false);
                return;
            }

            _isSavedOnce = true;
            btnSaveCriteria.Text = "update";
            CriteriaValidityChanged?.Invoke(true);
            CriteriaSaved?.Invoke(true);
            _toolTipCriteria.Show(first ? "Event criteria saved!" : "Event criteria updated!",
                btnSaveCriteria, 10, -30, 2000);
        }

        #endregion

        #region Helpers
        private List<PhaseModel> GetPhasesFromUI()
        {
            var list = new List<PhaseModel>();
            int sequenceCounter = 1;

            foreach (var phaseCtrl in _flowMain.Controls.OfType<PhaseControl>())
            {
                int segmentNo = 1;
                bool isIndependent = phaseCtrl.chkIndependentPhase.Checked;

                var phase = new PhaseModel
                {
                    Sequence = isIndependent ? (int?)null : sequenceCounter++,
                    Name = PlaceholderHelper.IsPlaceholder(phaseCtrl.txtPhaseName)
                        ? ""
                        : phaseCtrl.txtPhaseName.Text.Trim(),
                    Weight = decimal.TryParse(phaseCtrl.txtPhaseWeight.Text, out var w) ? w : 0m,
                    IsIndependent = isIndependent,
                    Segments = phaseCtrl.flowSegment.Controls.OfType<SegmentControl>()
                        .Select(seg => new SegmentModel
                        {
                            Sequence = segmentNo++,
                            Name = PlaceholderHelper.IsPlaceholder(seg.txtSegmentName)
                                ? ""
                                : seg.txtSegmentName.Text.Trim(),
                            Weight = decimal.TryParse(seg.txtSegmentWeight.Text, out var sw) ? sw : 0m,
                            Criteria = seg.flowCriteria.Controls.OfType<CriteriaControl>()
                                .Select(c => new CriteriaModel
                                {
                                    Name = PlaceholderHelper.IsPlaceholder(c.txtCriteriaName)
                                        ? ""
                                        : c.txtCriteriaName.Text.Trim(),
                                    Weight = decimal.TryParse(c.txtCriteriaWeight.Text, out var cw) ? cw : 0m
                                }).ToList()
                        }).ToList()
                };

                list.Add(phase);
            }

            return list;
        }

        public int GetPhaseSequence(PhaseControl target)
        {
            int seq = 1;
            foreach (var p in _flowMain.Controls.OfType<PhaseControl>())
            {
                if (!p.chkIndependentPhase.Checked)
                {
                    if (p == target) return seq;
                    seq++;
                }
            }
            return -1;
        }

        public void UpdatePhaseNumbers()
        {
            int seq = 1;
            foreach (var p in _flowMain.Controls.OfType<PhaseControl>())
            {
                if (p.chkIndependentPhase.Checked)
                {
                    p.txtPhaseNo.Text = "";
                    p.txtPhaseNo.BackColor = SystemColors.Control;
                }
                else
                {
                    p.txtPhaseNo.Text = seq.ToString();
                    p.txtPhaseNo.BackColor = Color.White;
                    seq++;
                }

                p.UpdateSegmentNumbers();
            }
        }

        public void UpdateEventTotalWeightLabel()
        {
            decimal total = _flowMain.Controls.OfType<PhaseControl>()
                .Where(p => !p.chkIndependentPhase.Checked)
                .Sum(p => decimal.TryParse(p.txtPhaseWeight.Text, out var w) ? w : 0m);
            lblEventTotalWeight.Text = total.ToString("0.##");
            lblEventTotalWeight.ForeColor = (total == 100m) ? Color.Green : Color.Red;
        }

        private void WireTextBoxes(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is TextBox tb)
                {
                    if (_wiredTextBoxes.Contains(tb)) { continue; }
                    _wiredTextBoxes.Add(tb);

                    tb.TextChanged += (s, e) =>
                    {
                        bool isName = tb.Name.Contains("Name");
                        bool isWeight = tb.Name.Contains("Weight");

                        if (isWeight)
                        {
                            if (!InputValidator.ValidateDecimal(tb, false))
                                tb.BackColor = Color.MistyRose;
                            else
                                tb.BackColor = Color.White;

                            // Update thresholds when weights change
                            if (tb.Name.Contains("PhaseWeight"))
                            {
                                UpdateEventTotalWeightLabel();
                                var phase = FindPhaseParent(tb);
                                phase?.UpdatePhaseTotalWeightLabel();
                            }
                            else if (tb.Name.Contains("SegmentWeight"))
                            {
                                var segment = FindSegmentParent(tb);
                                segment?.UpdateSegmentTotalWeightLabel();
                                var phase = FindPhaseParent(tb);
                                phase?.UpdatePhaseTotalWeightLabel();
                            }
                        }
                        else if (isName)
                        {
                            if (!InputValidator.ValidateName(tb))
                            {
                                tb.BackColor = Color.MistyRose;
                                var err = errorProvider.GetError(tb);
                                if (string.IsNullOrEmpty(err) ||
                                    (err != "Duplicate phase name" && err != "Duplicate segment name" && err != "Duplicate criteria name"))
                                {
                                    errorProvider.SetError(tb, "Invalid name");
                                }
                            }
                            else
                            {
                                if (errorProvider.GetError(tb) == "Invalid name")
                                    errorProvider.SetError(tb, "");
                                tb.BackColor = Color.White;
                            }

                            ValidateNameDuplicates(tb);
                        }

                        ValidateCriteriaPage();
                    };

                    tb.Leave += (s, e) =>
                    {
                        if (tb.Name.Contains("PhaseWeight"))
                            UpdateEventTotalWeightLabel();
                    };
                }
                WireTextBoxes(c);
            }
        }

        private PhaseControl? FindPhaseParent(Control c)
        {
            while (c != null)
            {
                if (c is PhaseControl pc) return pc;
                c = c.Parent;
            }
            return null;
        }

        private SegmentControl? FindSegmentParent(Control c)
        {
            while (c != null)
            {
                if (c is SegmentControl sc) return sc;
                c = c.Parent;
            }
            return null;
        }

        #endregion
    }
}