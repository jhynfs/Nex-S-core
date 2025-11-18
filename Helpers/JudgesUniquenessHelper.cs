using System.Linq;
using System.Windows.Forms;
using NexScore.CreateEventPages.SetupJudgesControl;

namespace NexScore.Helpers
{
    public static class JudgesUniquenessHelper
    {
        private static string Norm(string? s) => (s ?? string.Empty).Trim().ToLowerInvariant();

        public static bool ValidateUniqueJudgeNames(FlowLayoutPanel container, ErrorProvider ep, string placeholder)
        {
            var judges = container.Controls.OfType<AddJudgeControl>().ToList();

            // Clear only duplicate errors
            foreach (var j in judges)
                if (ep.GetError(j.txtJudgeName) == "Duplicate judge name")
                    ep.SetError(j.txtJudgeName, "");

            var duplicates = judges
                .Select(j => j.txtJudgeName)
                .Where(tb => !string.IsNullOrWhiteSpace(tb.Text) && tb.Text != placeholder)
                .GroupBy(tb => Norm(tb.Text))
                .Where(g => g.Count() > 1)
                .SelectMany(g => g);

            bool ok = true;
            foreach (var tb in duplicates)
            {
                ok = false;
                ep.SetError(tb, "Duplicate judge name");
            }
            return ok;
        }
    }
}