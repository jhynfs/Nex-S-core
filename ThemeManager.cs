using System.Drawing;
using System.Windows.Forms;

namespace NexScore
{
    public static class ThemeManager
    {
        public static bool IsDarkMode { get; private set; } = false;

        // define colors for both themes
        public static Color LightBackColor = Color.White;
        public static Color DarkBackColor = Color.FromArgb(30, 30, 30);
        public static Color LightTextColor = Color.Black;
        public static Color DarkTextColor = Color.White;

        public static void ApplyTheme(Control parent)
        {
            Color back = IsDarkMode ? DarkBackColor : LightBackColor;
            Color text = IsDarkMode ? DarkTextColor : LightTextColor;

            foreach (Control ctrl in parent.Controls)
            {
                ctrl.BackColor = back;
                ctrl.ForeColor = text;

                // recursively apply to nested controls
                if (ctrl.HasChildren)
                    ApplyTheme(ctrl);
            }
        }

        public static void ToggleTheme(Form form)
        {
            IsDarkMode = !IsDarkMode;
            ApplyTheme(form);
        }
    }
}
