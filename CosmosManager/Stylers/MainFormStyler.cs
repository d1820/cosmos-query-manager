using CosmosManager.Domain;
using System.Drawing;
using System.Windows.Forms;

namespace CosmosManager.Stylers
{
    public static class MainFormStyler
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

        public static void ApplyTheme(ThemeType themeType, MainForm form)
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
                    form.ForeColor = Color.FromArgb(60, 60, 60);
                    form.tabBackgroundPanel.BackColor = Color.FromArgb(30, 30, 30);
                    break;
            }
        }

        private static void ApplyDarkMenuItemTheme(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                item.ForeColor = Color.FromArgb(190, 190, 190);
                item.BackColor = Color.FromArgb(60, 60, 60);
            }
        }
    }
}