using CosmosManager.Domain;
using CosmosManager.Views;
using System.Drawing;

namespace CosmosManager.Stylers
{
    public class PreferencesFormStyler : BaseStyler
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

        // The preferences popup is themed by Application.SetColorMode(Dark); no manual coloring.
        public void ApplyTheme(ThemeType themeType, PreferencesForm form)
        {
        }
    }
}
