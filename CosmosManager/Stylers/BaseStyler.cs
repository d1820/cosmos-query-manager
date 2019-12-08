using CosmosManager.Domain;
using ScintillaNET;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CosmosManager.Stylers
{
    public class BaseStyler
    {
        public void SetDefaultStyles(Scintilla textbox, ThemeType themeType)
        {
            textbox.StyleResetDefault();
            textbox.Styles[Style.Default].Font = "Consolas";
            textbox.Styles[Style.Default].Size = 11;

            switch (themeType)
            {
                case ThemeType.Dark:
                    textbox.Styles[Style.Default].BackColor = Color.FromArgb(30, 30, 30);
                    textbox.Styles[Style.Default].ForeColor = Color.FromArgb(255, 255, 255);
                    break;

                case ThemeType.Light:
                    textbox.Styles[Style.Default].BackColor = Color.FromArgb(255, 255, 255);
                    textbox.Styles[Style.Default].ForeColor = Color.FromArgb(0, 0, 0);
                    break;
            }

            textbox.StyleClearAll();
        }

        protected static Color IntToColor(int rgb)
        {
            var c = Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
            Console.WriteLine($"Color {rgb}  {c.R} {c.G} {c.B}");
            return c;
        }

        protected void InitColors(Scintilla textbox, ThemeType themeType)
        {
            switch (themeType)
            {
                case ThemeType.Dark:
                    textbox.CaretForeColor = Color.White;
                    break;

                case ThemeType.Light:
                    textbox.CaretForeColor = Color.FromArgb(51, 51, 51);
                    break;
            }

            textbox.CaretLineVisible = false;
            textbox.SetSelectionBackColor(true, Color.DarkGray);
        }

        protected void InitNumberMargin(Scintilla textbox)
        {
            textbox.Styles[Style.LineNumber].BackColor = Color.FromArgb(51, 51, 51);
            textbox.Styles[Style.LineNumber].ForeColor = Color.FromArgb(241, 220, 1);
            textbox.Styles[Style.IndentGuide].ForeColor = Color.FromArgb(241, 220, 1);
            textbox.Styles[Style.IndentGuide].BackColor = Color.FromArgb(51, 51, 51);

            var nums = textbox.Margins[1];
            nums.Width = 30;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;
        }

        protected static void ApplyDarkMenuItemTheme(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                item.ForeColor = Color.FromArgb(190, 190, 190);
                item.BackColor = Color.FromArgb(60, 60, 60);
            }
        }

        protected static void ApplyLightMenuItemTheme(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                item.ForeColor = Color.FromArgb(97, 97, 97);
                item.BackColor = Color.FromArgb(221, 221, 221);
            }
        }
    }
}