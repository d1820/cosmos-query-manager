using CosmosManager.Domain;
using CosmosManager.Interfaces;
using ScintillaNET;
using System.Drawing;
using System.Linq;

namespace CosmosManager.Stylers
{
    public class JsonDocumentStyler : IJsonStyler
    {
        private Scintilla _textbox;

        public void SyntaxifyTextBox(Scintilla textbox)
        {
            _textbox = textbox;
            InitSyntaxColoring();
            InitNumberMargin();
            InitColors();

        }

        private void InitSyntaxColoring()
        {

            // Configure the default style
            _textbox.StyleResetDefault();
            _textbox.Styles[Style.Default].Font = "Consolas";
            _textbox.Styles[Style.Default].Size = 10;
            _textbox.Styles[Style.Default].BackColor = IntToColor(0x212121);
            _textbox.Styles[Style.Default].ForeColor = IntToColor(0xFFFFFF);

            _textbox.StyleClearAll();

            // Configure the JSON lexer styles
            _textbox.Styles[Style.Json.PropertyName].ForeColor = IntToColor(0x9CDCFE);
            _textbox.Styles[Style.Json.Number].ForeColor = IntToColor(0xb453e9);
            _textbox.Styles[Style.Json.String].ForeColor = IntToColor(0xCE9178);

            _textbox.Styles[Style.Json.Operator].ForeColor = IntToColor(0xffffff);

            _textbox.Lexer = Lexer.Json;
        }

        private void InitNumberMargin()
        {

            _textbox.Styles[Style.LineNumber].BackColor = Color.Black;
            _textbox.Styles[Style.LineNumber].ForeColor = Color.White;
            _textbox.Styles[Style.IndentGuide].ForeColor = Color.White;
            _textbox.Styles[Style.IndentGuide].BackColor = Color.Black;

            var nums = _textbox.Margins[1];
            nums.Width = 30;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

        }

        private static Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        private void InitColors()
        {
            _textbox.CaretForeColor = Color.White;
            _textbox.CaretLineVisible = false;
            _textbox.SetSelectionBackColor(true, Color.DarkGray);

        }
    }
}