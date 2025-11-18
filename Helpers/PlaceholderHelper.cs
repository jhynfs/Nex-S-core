using System;
using System.Drawing;
using System.Windows.Forms;

namespace NexScore.Helpers
{
    public static class PlaceholderHelper
    {
        private class PlaceholderState
        {
            public string Placeholder { get; set; } = "";
        }

        public static void SetPlaceholder(TextBox tb, string placeholder)
        {
            if (tb == null) return;

            // If not tagged yet, set up events
            if (tb.Tag is PlaceholderState psExisting)
            {
                // Update placeholder text (in case you want to change dynamically)
                psExisting.Placeholder = placeholder;
                if (string.IsNullOrWhiteSpace(tb.Text) || tb.Text == psExisting.Placeholder)
                    ApplyPlaceholder(tb, psExisting.Placeholder);
                return;
            }

            var state = new PlaceholderState { Placeholder = placeholder };
            tb.Tag = state;

            ApplyPlaceholder(tb, placeholder);

            tb.Enter += (s, e) =>
            {
                if (tb.Tag is PlaceholderState ps)
                {
                    if (tb.Text == ps.Placeholder)
                    {
                        tb.Text = "";
                        tb.ForeColor = Color.Black;
                    }
                }
            };

            tb.Leave += (s, e) =>
            {
                RestoreIfEmpty(tb);
            };
        }

        public static void RestoreIfEmpty(TextBox tb)
        {
            if (tb?.Tag is not PlaceholderState ps) return;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                ApplyPlaceholder(tb, ps.Placeholder);
            }
        }

        public static bool IsPlaceholder(TextBox tb)
        {
            return tb?.Tag is PlaceholderState ps && tb.Text == ps.Placeholder;
        }

        private static void ApplyPlaceholder(TextBox tb, string placeholder)
        {
            tb.Text = placeholder;
            tb.ForeColor = Color.DarkGray;
        }
    }
}