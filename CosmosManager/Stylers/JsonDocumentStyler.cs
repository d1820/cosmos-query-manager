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
            InitColors(_textbox);

        }

        private void InitSyntaxColoring()
        {

            // Configure the default style
            SetDefaultStyles(_textbox);

            // Configure the JSON lexer styles
            _textbox.Styles[Style.Json.PropertyName].ForeColor = IntToColor(0x9CDCFE);
            _textbox.Styles[Style.Json.Number].ForeColor = IntToColor(0xE685FF);
            _textbox.Styles[Style.Json.String].ForeColor = IntToColor(0xCE9178);
            _textbox.Styles[Style.Json.Operator].ForeColor = IntToColor(0xE0E0E0);

            _textbox.Lexer = Lexer.Json;
        }
    }
}