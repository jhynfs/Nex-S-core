using NexScore.CreateEventPages.SetupCriteriaControls;
using NexScore.Helpers;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NexScore
{
    public partial class PhaseControl : UserControl
    {
        public TextBox txtPhaseNo => _txtPhaseNo;
        public TextBox txtPhaseName => _txtPhaseName;
        public TextBox txtPhaseWeight => _txtPhaseWeight;
        public PictureBox btnRemovePhase => _btnRemovePhase;
        public Button btnAddSegment => _btnAddSegment;
        public FlowLayoutPanel flowSegment => _flowSegment;
        public CheckBox chkIndependentPhase => _chkIndependentPhase;
        public Label lblPhaseTotalWeight => _lblPhaseTotalWeight;

        private readonly ToolTip _hoverCheckboxes = new ToolTip();

        public PhaseControl()
        {
            InitializeComponent();
            _txtPhaseNo.ReadOnly = true;

            InitializePhase();

            _btnRemovePhase.Click += (s, e) =>
            {
                var parentFlow = this.Parent as FlowLayoutPanel;
                parentFlow?.Controls.Remove(this);
                this.Dispose();
                var setup = parentFlow?.Parent as SetupCriteria;
                if (setup != null)
                {
                    setup.UpdatePhaseNumbers();
                    setup.UpdateEventTotalWeightLabel();
                    setup.RevalidateUniquenessForUI();
                    setup.ValidateCriteriaPage();
                }
            };
            PlaceholderHelper.SetPlaceholder(_txtPhaseName, "(e.g. Talent Portion, Coronation)");
            PlaceholderHelper.SetPlaceholder(_txtPhaseWeight, "1-100%");

            _chkIndependentPhase.CheckedChanged += ToggleFlagsChanged;

            txtPhaseName.TextChanged += (s, e) =>
                FindSetupCriteria()?.ValidateNameDuplicates(txtPhaseName);

            txtPhaseWeight.TextChanged += (s, e) =>
            {
                UpdatePhaseTotalWeightLabel();
                FindSetupCriteria()?.UpdateEventTotalWeightLabel();
            };

            _hoverCheckboxes.AutoPopDelay = 8000;
            _hoverCheckboxes.InitialDelay = 300;
            _hoverCheckboxes.ReshowDelay = 100;
            _hoverCheckboxes.ShowAlways = true;
            _hoverCheckboxes.IsBalloon = true;

            _hoverCheckboxes.SetToolTip(chkIndependentPhase, "Independent: Not part of the event sequence but still must total 100 internally.");
        }

        private void ToggleFlagsChanged(object? sender, EventArgs e)
        {
            var setup = FindSetupCriteria();

            if (chkIndependentPhase.Checked)
            {
                _txtPhaseWeight.Text = "100";
                _txtPhaseWeight.Enabled = false;
            }
            else
            {
                _txtPhaseWeight.Enabled = true;
                _txtPhaseWeight.Text = "";
                PlaceholderHelper.RestoreIfEmpty(_txtPhaseWeight);
            }

            bool nowIndependent = chkIndependentPhase.Checked;

            if (nowIndependent)
            {
                _txtPhaseNo.Enabled = false;
                _txtPhaseNo.Text = "";
                _txtPhaseNo.BackColor = SystemColors.Control;
            }
            else
            {
                _txtPhaseNo.Enabled = true;
                _txtPhaseNo.BackColor = Color.White;
            }

            if (setup != null)
            {
                setup.BeginInvoke(new Action(() =>
                {
                    setup.UpdatePhaseNumbers();

                    if (!chkIndependentPhase.Checked)
                    {
                        if (string.IsNullOrEmpty(_txtPhaseNo.Text))
                        {
                            int seq = setup.GetPhaseSequence(this);
                            if (seq > 0)
                                _txtPhaseNo.Text = seq.ToString();
                        }
                    }

                    UpdatePhaseTotalWeightLabel();
                    setup.UpdateEventTotalWeightLabel();
                    setup.ValidateCriteriaPage();
                }));
            }
        }

        private SetupCriteria? FindSetupCriteria() => this.Parent?.Parent as SetupCriteria;

        private void InitializePhase() => AddSegment();

        private void AddSegment()
        {
            var segment = new SegmentControl { Dock = DockStyle.Top };

            segment.txtSegmentWeight.TextChanged += (s, e) =>
            {
                UpdatePhaseTotalWeightLabel();
                FindSetupCriteria()?.UpdateEventTotalWeightLabel();
            };
            segment.txtSegmentName.TextChanged += (s, e) =>
            {
                FindSetupCriteria()?.ValidateNameDuplicates(segment.txtSegmentName);
            };

            foreach (var crit in segment.flowCriteria.Controls.OfType<CriteriaControl>())
            {
                crit.txtCriteriaWeight.TextChanged += (s2, e2) =>
                {
                    segment.UpdateSegmentTotalWeightLabel();
                    UpdatePhaseTotalWeightLabel();
                    FindSetupCriteria()?.UpdateEventTotalWeightLabel();
                };
                crit.txtCriteriaName.TextChanged += (s3, e3) =>
                {
                    FindSetupCriteria()?.ValidateNameDuplicates(crit.txtCriteriaName);
                };
            }

            _flowSegment.Controls.Add(segment);
            UpdateSegmentNumbers();
            UpdatePhaseTotalWeightLabel();
        }

        public void UpdateSegmentNumbers()
        {
            int num = 1;
            foreach (var seg in flowSegment.Controls.OfType<SegmentControl>())
                seg.txtSegmentNo.Text = num++.ToString();
        }

        public void UpdatePhaseTotalWeightLabel()
        {
            decimal sumSegments = flowSegment.Controls.OfType<SegmentControl>()
                .Sum(s => decimal.TryParse(s.txtSegmentWeight.Text, out var w) ? w : 0m);

            decimal phaseWeight;
            bool parsed = true;

            if (chkIndependentPhase.Checked)
            {
                phaseWeight = 100m;
            }
            else
            {
                parsed = decimal.TryParse(txtPhaseWeight.Text, out phaseWeight);
            }

            _lblPhaseTotalWeight.Text = sumSegments.ToString("0.##");

            if (!parsed)
            {
                _lblPhaseTotalWeight.ForeColor = Color.Red;
            }
            else
            {
                _lblPhaseTotalWeight.ForeColor = (sumSegments == phaseWeight) ? Color.Green : Color.Red;
            }
        }
    }
}