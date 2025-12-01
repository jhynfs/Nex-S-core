using System;
using System.Drawing;
using System.Windows.Forms;

namespace NexScore.Helpers
{
    public static class TextboxHelper
    {
        public static void SetPlaceholder(TextBox tb, string placeholder)
        {
            tb.GotFocus -= RemovePlaceholder;
            tb.LostFocus -= ApplyPlaceholder;

            tb.Tag = placeholder;

            ApplyPlaceholder(tb, EventArgs.Empty);

            tb.GotFocus += RemovePlaceholder;
            tb.LostFocus += ApplyPlaceholder;
        }

        private static void RemovePlaceholder(object sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                var placeholder = tb.Tag as string;

                if (tb.Text == placeholder)
                {
                    tb.Text = "";
                    tb.ForeColor = Color.Black;
                }
            }
        }

        private static void ApplyPlaceholder(object sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                var placeholder = tb.Tag as string;

                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Text = placeholder;
                    tb.ForeColor = Color.Gray;
                }
            }
        }

        public static bool IsBlankOrPlaceholder(TextBox tb)
        {
            string placeholder = tb.Tag as string;
            return string.IsNullOrWhiteSpace(tb.Text) || tb.Text == placeholder;
        }
    }
}
