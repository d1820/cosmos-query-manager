using CosmosManager.Domain;
using CosmosManager.Views;
using System.Drawing;

namespace CosmosManager.Stylers
{
    public class PreferencesFormStyler: BaseStyler
    {
        public static Brush GetTabBrush(ThemeType themeType)
        {
            switch (themeType)
            {
                case ThemeType.Dark:
                    return new SolidBrush(Color.FromArgb(51, 51, 51));
            }
            return new SolidBrush(Color.Transparent);
        }

        public void ApplyTheme(ThemeType themeType, PreferencesForm form)
        {
            switch (themeType)
            {
                case ThemeType.Dark:

                    form.BackColor = Color.FromArgb(60, 60, 60);
                    form.menuStrip1.BackColor = Color.FromArgb(60, 60, 60);
                    ApplyDarkMenuItemTheme(form.menuStrip1.Items);
                    form.statusStrip1.BackColor = Color.FromArgb(0, 122, 204);
                    form.label1.ForeColor = Color.FromArgb(190, 190, 190);
                    form.toolStripStatusLabel1.ForeColor = Color.FromArgb(255, 255, 255);
                    break;
            }
        }
    }
}