using NexScore.Helpers;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NexScore.CreateEventPages.SetupCriteriaControls
{
    public partial class SegmentControl : UserControl
    {
        public TextBox txtSegmentNo => _txtSegmentNo;
        public TextBox txtSegmentName => _txtSegmentName;
        public TextBox txtSegmentWeight => _txtSegmentWeight;
        public PictureBox btnRemoveSegment => _btnRemoveSegment;
        public Button btnAddCriteria => _btnAddCriteria;
        public FlowLayoutPanel flowCriteria => _flowCriteria;
        public Label lblSegmentTotalWeight => _lblSegmentTotalWeight;

        public SegmentControl()
        {
            InitializeComponent();
            _txtSegmentNo.ReadOnly = true;

            InitializeSegment();
            // Automatically seed with one criteria
            AddCriteria();

            _btnRemoveSegment.Click += (s, e) => OnRemoveSegmentClicked();

            // PLACEHOLDER
            PlaceholderHelper.SetPlaceholder(_txtSegmentName, "(e.g. Technical Skill, Evening Gown)");
            PlaceholderHelper.SetPlaceholder(_txtSegmentWeight, "1-100%");
        }

        private void OnRemoveSegmentClicked()
        {
            var parentPhase = FindParentPhase(this);
            if (parentPhase == null)
            {
                (this.Parent as FlowLayoutPanel)?.Controls.Remove(this);
                this.Dispose();
                return;
            }

            parentPhase.flowSegment.Controls.Remove(this);
            this.Dispose();
            parentPhase.UpdateSegmentNumbers();
            parentPhase.UpdatePhaseTotalWeightLabel();
            var setup = parentPhase.Parent?.Parent as SetupCriteria;
            setup?.UpdateEventTotalWeightLabel();
            setup?.RevalidateUniquenessForUI();
            setup?.ValidateCriteriaPage();
        }

        private void InitializeSegment()
        {
            _btnAddCriteria.Click += (s, e) => AddCriteria();
        }

        private void AddCriteria()
        {
            var criteria = new CriteriaControl { Dock = DockStyle.Top };
            criteria.txtCriteriaWeight.TextChanged += (s, e) =>
            {
                UpdateSegmentTotalWeightLabel();
                var phase = FindParentPhase(this);
                phase?.UpdatePhaseTotalWeightLabel();
                (phase?.Parent?.Parent as SetupCriteria)?.UpdateEventTotalWeightLabel();
            };
            criteria.txtCriteriaName.TextChanged += (s, e) =>
            {
                var setup = FindSetupCriteria();
                setup?.ValidateNameDuplicates(criteria.txtCriteriaName);
            };
            _flowCriteria.Controls.Add(criteria);
            UpdateSegmentTotalWeightLabel();
        }

        public void UpdateSegmentTotalWeightLabel()
        {
            var crits = flowCriteria.Controls.OfType<CriteriaControl>().ToList();
            decimal total = crits.Sum(c => decimal.TryParse(c.txtCriteriaWeight.Text, out var w) ? w : 0m);

            _lblSegmentTotalWeight.Text = total.ToString("0.##");

            if (crits.Count == 0)
            {
                _lblSegmentTotalWeight.ForeColor = SystemColors.ControlText;
            }
            else
            {
                _lblSegmentTotalWeight.ForeColor = (total == 100m ? Color.Green : Color.Red);
            }
        }

        private PhaseControl? FindParentPhase(Control control)
        {
            while (control != null)
            {
                if (control is PhaseControl p) return p;
                control = control.Parent;
            }
            return null;
        }

        private SetupCriteria? FindSetupCriteria()
        {
            Control cur = this;
            while (cur != null)
            {
                if (cur is SetupCriteria sc) return sc;
                cur = cur.Parent;
            }
            return null;
        }
    }
}