using NexScore.Helpers;
using System;
using System.Windows.Forms;

namespace NexScore.CreateEventPages.SetupCriteriaControls
{
    public partial class CriteriaControl : UserControl
    {
        public TextBox txtCriteriaName => _txtCriteriaName;
        public TextBox txtCriteriaWeight => _txtCriteriaWeight;
        public PictureBox btnRemoveCriteria => _btnRemoveCriteria;

        public CriteriaControl()
        {
            InitializeComponent();

            _btnRemoveCriteria.Click += (s, e) =>
            {
                var segment = FindAncestor<SegmentControl>();
                PhaseControl? phase = null;
                if (segment != null)
                    phase = FindAncestorFrom(segment, (c) => c is PhaseControl) as PhaseControl;

                var setup = FindSetupCriteria();

                var parent = this.Parent;
                parent?.Controls.Remove(this);

                segment?.UpdateSegmentTotalWeightLabel();
                phase?.UpdatePhaseTotalWeightLabel();
                setup?.UpdateEventTotalWeightLabel();

                setup?.RevalidateUniquenessForUI();
                setup?.ValidateCriteriaPage();

                this.Dispose();
            };

            PlaceholderHelper.SetPlaceholder(_txtCriteriaName, "(e.g. Execution & Difficulty, Elegance & Poise)");
            PlaceholderHelper.SetPlaceholder(_txtCriteriaWeight, "1-100%");
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

        private T? FindAncestor<T>() where T : Control
        {
            Control cur = this.Parent;
            while (cur != null)
            {
                if (cur is T t) return t;
                cur = cur.Parent;
            }
            return null;
        }

        private Control? FindAncestorFrom(Control start, Func<Control, bool> predicate)
        {
            Control cur = start.Parent;
            while (cur != null)
            {
                if (predicate(cur)) return cur;
                cur = cur.Parent;
            }
            return null;
        }
    }
}