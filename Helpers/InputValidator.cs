using NexScore.CreateEventPages.SetupCriteriaControls;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NexScore.Helpers
{
    public static class InputValidator
    {
        // Updated regex: allow / ( ) & along with existing characters. Hyphen placed first to avoid range ambiguity.
        private static readonly Regex NamePattern = new Regex(@"^(?=.*[A-Za-z])[-A-Za-z0-9\s,'()/&]*$");
        private static readonly Regex WholeNumberPattern = new Regex(@"^\d*$");
        private static readonly Regex DecimalPattern = new Regex(@"^\d*\.?\d*$");

        public static bool ValidateName(TextBox textBox)
        {
            if (!NamePattern.IsMatch(textBox.Text))
            {
                return false;
            }
            return true;
        }

        public static bool ValidateWholeNumber(TextBox textBox)
        {
            if (!WholeNumberPattern.IsMatch(textBox.Text))
            {
                MessageBox.Show("Please enter a valid whole number.", "Invalid Input");
                return false;
            }
            return true;
        }

        public static bool ValidateDecimal(TextBox textBox, bool allowZero = false)
        {
            if (!DecimalPattern.IsMatch(textBox.Text))
                return false;

            if (decimal.TryParse(textBox.Text, out decimal val))
            {
                if (val < 0 || (!allowZero && val == 0) || val > 100)
                    return false;
            }
            else
                return false;

            return true;
        }

    }
}