using CosmosManager.Domain;
using CosmosManager.Interfaces;
using ScintillaNET;
using System.Drawing;

namespace CosmosManager.Stylers
{

    public class JsonDocumentStyler : BaseStyler, IJsonStyler
    {
        private Scintilla _textbox;

        public void SyntaxifyTextBox(Scintilla textbox)
        {
            _textbox = textbox;
            InitSyntaxColoring();
            InitNumberMargin(_textbox);
            InitColors(_textbox, AppReferences.CurrentTheme);

        }

        private void InitSyntaxColoring()
        {

            // Configure the default style
            SetDefaultStyles(_textbox, AppReferences.CurrentTheme);
            // Configure the JSON lexer styles
            switch (AppReferences.CurrentTheme)
            {
                case ThemeType.Dark:
                    _textbox.Styles[Style.Json.PropertyName].ForeColor = Color.FromArgb(156, 220, 254);
                    _textbox.Styles[Style.Json.Number].ForeColor = Color.FromArgb(230, 133, 255);
                    _textbox.Styles[Style.Json.String].ForeColor = Color.FromArgb(206, 145, 120);
                    break;
                case ThemeType.Light:
                    _textbox.Styles[Style.Json.PropertyName].ForeColor = Color.FromArgb(8, 82, 137);
                    _textbox.Styles[Style.Json.Number].ForeColor = Color.FromArgb(0, 125, 0);
                    _textbox.Styles[Style.Json.String].ForeColor = Color.FromArgb(51, 51, 51);
                    break;
            }
            _textbox.Styles[Style.Json.Operator].ForeColor = Color.FromArgb(224, 224, 224);

            _textbox.Lexer = Lexer.Json;
        }
    }
}