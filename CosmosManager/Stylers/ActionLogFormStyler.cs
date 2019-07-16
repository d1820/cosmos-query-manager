using CosmosManager.Domain;
using CosmosManager.Views;
using System.Drawing;
using System.Windows.Forms;

namespace CosmosManager.Stylers
{
    public class ActionLogFormStyler: BaseStyler
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

        public void ApplyTheme(ThemeType themeType, ActionLogForm form)
        {
            switch (themeType)
            {
                case ThemeType.Dark:

                    form.BackColor = Color.FromArgb(60, 60, 60);
                    form.menuStrip1.BackColor = Color.FromArgb(60, 60, 60);
                    ApplyDarkMenuItemTheme(form.menuStrip1.Items);
                    form.logText.BackColor = Color.FromArgb(60, 60, 60);
                    form.logText.ForeColor = Color.FromArgb(190, 190, 190);
                    break;
            }
        }


    }
}