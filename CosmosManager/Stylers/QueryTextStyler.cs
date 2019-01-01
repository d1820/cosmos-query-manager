using CosmosManager.Domain;
using CosmosManager.Interfaces;
using ScintillaNET;
using System.Drawing;
using System.Linq;

namespace CosmosManager.Stylers
{
    public class QueryTextStyler : BaseStyler, IQueryStyler
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

            // Configure the SQL lexer styles
            _textbox.Styles[Style.Sql.Identifier].ForeColor = IntToColor(0xD0DAE2);
            _textbox.Styles[Style.Sql.Comment].ForeColor = IntToColor(0x6A993E);
            _textbox.Styles[Style.Sql.CommentLine].ForeColor = IntToColor(0x6A993E);
            _textbox.Styles[Style.Sql.CommentDoc].ForeColor = IntToColor(0x6A993E);
            _textbox.Styles[Style.Sql.Number].ForeColor = IntToColor(0xE685FF);
            _textbox.Styles[Style.Sql.String].ForeColor = IntToColor(0xCE9178);
            _textbox.Styles[Style.Sql.Character].ForeColor = IntToColor(0xE95454);
            _textbox.Styles[Style.Sql.Operator].ForeColor = IntToColor(0xE0E0E0);
            _textbox.Styles[Style.Sql.CommentLineDoc].ForeColor = IntToColor(0x77A7DB);
            _textbox.Styles[Style.Sql.Word].ForeColor = IntToColor(0x48A8EE);
            _textbox.Styles[Style.Sql.Word2].ForeColor = IntToColor(0xF98906);
            _textbox.Styles[Style.Sql.CommentDocKeyword].ForeColor = Color.Gray;
            _textbox.Styles[Style.Sql.CommentDocKeywordError].ForeColor = Color.Gray;

            _textbox.Lexer = Lexer.Sql;
            _textbox.SetKeywords(0, string.Join(" ", Constants.KeyWordList.Select(s => s.ToLowerInvariant()).Concat(Constants.KeyWordList.Select(s => s.ToUpperInvariant()))));
            _textbox.SetKeywords(1, string.Join(" ", Constants.BuiltInKeyWordList.Select(s => s.ToUpperInvariant()).Concat(Constants.BuiltInKeyWordList.Select(s => s.ToLowerInvariant()))));

        }


    }
}