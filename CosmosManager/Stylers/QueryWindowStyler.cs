using CosmosManager.Domain;
using System.Drawing;

namespace CosmosManager.Stylers
{
    public static class QueryWindowStyler
    {
        public static void ApplyTheme(ThemeType themeType, QueryWindowControl form)
        {
            switch (themeType)
            {
                case ThemeType.Dark:
                    form.BackColor = Color.FromArgb(60, 60, 60);
                    form.queryToolStrip.BackColor = Color.FromArgb(60, 60, 60);
                    form.resultListToolStrip.BackColor = Color.FromArgb(60, 60, 60);
                    form.documentToolstrip.BackColor = Color.FromArgb(60, 60, 60);
                    form.splitQueryResult.BackColor = Color.FromArgb(60, 60, 60);
                    form.splitQueryResult.Panel1.BackColor = Color.FromArgb(51, 51, 51);
                    form.splitQueryResult.Panel2.BackColor = Color.FromArgb(51, 51, 51);
                    form.resultCountTextbox.BackColor = Color.FromArgb(60, 60, 60);
                    form.resultCountTextbox.ForeColor = Color.FromArgb(190, 190, 190);
                    form.selectConnections.BackColor = Color.FromArgb(60, 60, 60);
                    form.selectConnections.ForeColor = Color.FromArgb(190, 190, 190);
                    break;

                case ThemeType.Light:
                    form.BackColor = Color.FromArgb(255,255,255);
                    form.queryToolStrip.BackColor = Color.FromArgb(243,243,243);
                    form.resultListToolStrip.BackColor = Color.FromArgb(243, 243, 243);
                    form.documentToolstrip.BackColor = Color.FromArgb(243, 243, 243);
                    form.splitQueryResult.BackColor = Color.FromArgb(243, 243, 243);
                    form.splitQueryResult.Panel1.BackColor = Color.FromArgb(221, 221, 221);
                    form.splitQueryResult.Panel2.BackColor = Color.FromArgb(221, 221, 221);
                    form.resultCountTextbox.BackColor = Color.FromArgb(243, 243, 243);
                    form.resultCountTextbox.ForeColor = Color.FromArgb(122, 122, 122);
                    form.selectConnections.BackColor = Color.FromArgb(221, 221, 221);
                    form.selectConnections.ForeColor = Color.FromArgb(97, 97, 97);
                    break;
            }
        }
    }
}