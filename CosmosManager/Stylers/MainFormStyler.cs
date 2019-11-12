using CosmosManager.Domain;
using System.Drawing;

namespace CosmosManager.Stylers
{
    public class MainFormStyler : BaseStyler
    {
        public static Brush GetTabBrush(ThemeType themeType)
        {
            switch (themeType)
            {
                case ThemeType.Dark:
                    return new SolidBrush(Color.FromArgb(51, 51, 51));
                case ThemeType.Light:
                    return new SolidBrush(Color.FromArgb(236, 236, 236));
            }
            return new SolidBrush(Color.Transparent);
        }

        public static Color GetTabBackground(ThemeType themeType)
        {
            switch (themeType)
            {
                case ThemeType.Dark:
                    return Color.FromArgb(240, 240, 240);
                case ThemeType.Light:
                    return Color.FromArgb(236, 236, 236);
            }
            return Color.FromArgb(240, 240, 240);
        }

        public void ApplyTheme(ThemeType themeType, MainForm form)
        {
            switch (themeType)
            {
                case ThemeType.Dark:
                    form.fileTreeView.BackColor = Color.FromArgb(38, 38, 38);
                    form.fileTreeView.ForeColor = Color.FromArgb(190, 190, 190);
                    form.BackColor = Color.FromArgb(60, 60, 60);
                    form.menuStrip1.BackColor = Color.FromArgb(60, 60, 60);
                    ApplyDarkMenuItemTheme(form.menuStrip1.Items);
                    form.statusStrip1.BackColor = Color.FromArgb(0, 122, 204);
                    form.splitContainer1.BackColor = Color.FromArgb(38, 38, 38);
                    form.splitContainer1.Panel1.BackColor = Color.FromArgb(38, 38, 38);
                    form.splitContainer1.Panel2.BackColor = Color.FromArgb(38, 38, 38);
                    form.tabBackgroundPanel.BackColor = Color.FromArgb(30, 30, 30);
                    break;
                case ThemeType.Light:
                    form.fileTreeView.BackColor = Color.FromArgb(243, 243, 243);
                    form.fileTreeView.ForeColor = Color.FromArgb(122, 122, 122);
                    form.BackColor = Color.FromArgb(255, 255, 255);
                    form.menuStrip1.BackColor = Color.FromArgb(221, 221, 221);
                    ApplyLightMenuItemTheme(form.menuStrip1.Items);
                    form.statusStrip1.BackColor = Color.FromArgb(0, 122, 204);
                    form.splitContainer1.BackColor = Color.FromArgb(243, 243, 243);
                    form.splitContainer1.Panel1.BackColor = Color.FromArgb(243, 243, 243);
                    form.splitContainer1.Panel2.BackColor = Color.FromArgb(243, 243, 243);
                    form.tabBackgroundPanel.BackColor = Color.FromArgb(235, 235, 235);
                    break;
            }
        }
    }
}