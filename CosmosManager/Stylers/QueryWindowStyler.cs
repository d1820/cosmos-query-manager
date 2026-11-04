using CosmosManager.Domain;
using System.Drawing;

namespace CosmosManager.Stylers
{
    public static class QueryWindowStyler
    {
        // Application.SetColorMode(Dark) themes most chrome, but SplitContainers, ToolStrips, the
        // results ListView, the output TextBox and the Scintilla editors need explicit dark colors,
        // and the native scrollbars are switched to the dark Explorer theme.
        public static void ApplyTheme(ThemeType themeType, QueryWindowControl form)
        {
            var dark = Color.FromArgb(30, 30, 30);
            var chrome = Color.FromArgb(37, 37, 38);
            var text = Color.FromArgb(220, 220, 220);

            form.BackColor = chrome;
            form.queryToolStrip.BackColor = chrome;
            form.resultListToolStrip.BackColor = chrome;
            form.documentToolstrip.BackColor = chrome;
            form.splitQueryResult.BackColor = dark;
            form.splitQueryResult.Panel1.BackColor = dark;
            form.splitQueryResult.Panel2.BackColor = dark;
            form.splitResultView.BackColor = dark;
            form.splitResultView.Panel1.BackColor = dark;
            form.splitResultView.Panel2.BackColor = dark;

            // The Results/Output TabPages paint a white visual-styles background that shows through
            // the page padding as a white frame around the whole bottom panel — theme them dark.
            form.tabControlQueryOutput.BackColor = chrome;
            form.tabResultsPage.UseVisualStyleBackColor = false;
            form.tabResultsPage.BackColor = dark;
            form.tabOuputPage.UseVisualStyleBackColor = false;
            form.tabOuputPage.BackColor = dark;
            form.resultCountTextbox.BackColor = chrome;
            form.resultCountTextbox.ForeColor = text;
            form.selectConnections.BackColor = chrome;
            form.selectConnections.ForeColor = text;

            form.resultListView.BackColor = dark;
            form.resultListView.ForeColor = text;
            form.resultListView.GridLines = false;
            form.resultListView.ShowGroups = false;
            form.resultListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            form.textQueryOutput.BackColor = dark;
            form.textQueryOutput.ForeColor = text;
            form.textQueryOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            form.textQuery.BorderStyle = ScintillaNET.BorderStyle.None;
            form.textDocument.BorderStyle = ScintillaNET.BorderStyle.None;

            DarkModeNative.UseDarkScrollbars(form.resultListView);
            DarkModeNative.UseDarkScrollbars(form.textQuery);
            DarkModeNative.UseDarkScrollbars(form.textDocument);
            DarkModeNative.UseDarkScrollbars(form.textQueryOutput);
        }
    }
}
