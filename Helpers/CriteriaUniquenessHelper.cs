using System.Linq;
using System.Windows.Forms;
using NexScore.CreateEventPages.SetupCriteriaControls;

namespace NexScore.Helpers
{
    public static class CriteriaUniquenessHelper
    {
        private static string Norm(string? s) => (s ?? string.Empty).Trim().ToLowerInvariant();

        private static bool IsCandidate(TextBox tb)
            => !string.IsNullOrWhiteSpace(tb.Text) && InputValidator.ValidateName(tb);

        public static bool ValidateUniquePhaseNames(FlowLayoutPanel flowMain, ErrorProvider ep)
        {
            var phases = flowMain.Controls.OfType<PhaseControl>().ToList();
            foreach (var p in phases) ep.SetError(p.txtPhaseName, "");

            var dups = phases.Select(p => p.txtPhaseName)
                .Where(IsCandidate)
                .GroupBy(x => Norm(x.Text))
                .Where(g => g.Count() > 1)
                .SelectMany(g => g);

            bool ok = true;
            foreach (var tb in dups)
            {
                ok = false;
                ep.SetError(tb, "Duplicate phase name");
            }
            return ok;
        }

        public static bool ValidateUniqueSegmentNames(PhaseControl phase, ErrorProvider ep)
        {
            var segs = phase.flowSegment.Controls.OfType<SegmentControl>().ToList();
            foreach (var s in segs) ep.SetError(s.txtSegmentName, "");

            var dups = segs.Select(s => s.txtSegmentName)
                .Where(IsCandidate)
                .GroupBy(x => Norm(x.Text))
                .Where(g => g.Count() > 1)
                .SelectMany(g => g);

            bool ok = true;
            foreach (var tb in dups)
            {
                ok = false;
                ep.SetError(tb, "Duplicate segment name");
            }
            return ok;
        }

        public static bool ValidateUniqueCriteriaNames(SegmentControl seg, ErrorProvider ep)
        {
            var crits = seg.flowCriteria.Controls.OfType<CriteriaControl>().ToList();
            foreach (var c in crits) ep.SetError(c.txtCriteriaName, "");

            var dups = crits.Select(c => c.txtCriteriaName)
                .Where(IsCandidate)
                .GroupBy(x => Norm(x.Text))
                .Where(g => g.Count() > 1)
                .SelectMany(g => g);

            bool ok = true;
            foreach (var tb in dups)
            {
                ok = false;
                ep.SetError(tb, "Duplicate criteria name");
            }
            return ok;
        }

        public static bool ValidateAll(FlowLayoutPanel flowMain, ErrorProvider ep)
        {
            bool ok = ValidateUniquePhaseNames(flowMain, ep);
            foreach (var phase in flowMain.Controls.OfType<PhaseControl>())
            {
                ok &= ValidateUniqueSegmentNames(phase, ep);
                foreach (var seg in phase.flowSegment.Controls.OfType<SegmentControl>())
                    ok &= ValidateUniqueCriteriaNames(seg, ep);
            }
            return ok;
        }

        public static void PhaseScope(FlowLayoutPanel flowMain, PhaseControl changed, ErrorProvider ep)
            => ValidateUniquePhaseNames(flowMain, ep);

        public static void SegmentScope(PhaseControl phase, SegmentControl changed, ErrorProvider ep)
            => ValidateUniqueSegmentNames(phase, ep);

        public static void CriteriaScope(SegmentControl seg, ErrorProvider ep)
            => ValidateUniqueCriteriaNames(seg, ep);
    }
}