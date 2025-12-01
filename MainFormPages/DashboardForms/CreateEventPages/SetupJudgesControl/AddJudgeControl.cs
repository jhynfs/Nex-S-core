using NexScore.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NexScore.CreateEventPages.SetupJudgesControl
{
    public partial class AddJudgeControl : UserControl
    {
        public TextBox txtJudgeName => _txtJudgeName;
        public TextBox txtJudgeNo => _txtJudgeNo;
        public TextBox txtJudgeTitle => _txtJudgeTitle;

        public event Action<AddJudgeControl>? DeleteRequested;

        private const string NamePlaceholder = "Judge's Full Name";
        private const string TitlePlaceholder = "e.g., Dr., Ms., Engr. (Optional)";

        public AddJudgeControl()
        {
            InitializeComponent();
            _txtJudgeNo.ReadOnly = true;

            TextboxHelper.SetPlaceholder(_txtJudgeName, NamePlaceholder);
            TextboxHelper.SetPlaceholder(_txtJudgeTitle, TitlePlaceholder);

            btnRemoveJudge.Click += (s, e) => DeleteRequested?.Invoke(this);
        }

        public bool IsNamePlaceholder => _txtJudgeName.Text == NamePlaceholder;
        public bool IsTitlePlaceholder => _txtJudgeTitle.Text == TitlePlaceholder;
    }
}