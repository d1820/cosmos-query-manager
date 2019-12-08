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
            InitSyntaxColoring(AppReferences.CurrentTheme);
            InitNumberMargin(_textbox);
            InitColors(_textbox, AppReferences.CurrentTheme);
        }

        private void InitSyntaxColoring(ThemeType themeType)
        {
            // Configure the default style
            SetDefaultStyles(_textbox, AppReferences.CurrentTheme);

            // Configure the SQL lexer styles

            _textbox.Styles[Style.Sql.Comment].ForeColor = Color.FromArgb(106, 153, 62);
            _textbox.Styles[Style.Sql.CommentLine].ForeColor = Color.FromArgb(106, 153, 62);
            _textbox.Styles[Style.Sql.CommentDoc].ForeColor = Color.FromArgb(106, 153, 62);
            _textbox.Styles[Style.Sql.Number].ForeColor = Color.FromArgb(230, 133, 255);
            _textbox.Styles[Style.Sql.String].ForeColor = Color.FromArgb(206, 145, 120);
            _textbox.Styles[Style.Sql.Character].ForeColor = Color.FromArgb(233, 84, 84);

            _textbox.Styles[Style.Sql.CommentLineDoc].ForeColor = Color.FromArgb(119, 167, 219);
            _textbox.Styles[Style.Sql.Word].ForeColor = Color.FromArgb(72, 168, 238);
            _textbox.Styles[Style.Sql.Word2].ForeColor = Color.FromArgb(249, 137, 6);
            _textbox.Styles[Style.Sql.CommentDocKeyword].ForeColor = Color.Gray;
            _textbox.Styles[Style.Sql.CommentDocKeywordError].ForeColor = Color.Gray;

            switch (themeType)
            {
                case ThemeType.Dark:
                    _textbox.Styles[Style.Sql.Operator].ForeColor = Color.FromArgb(224, 224, 224);
                    _textbox.Styles[Style.Sql.Identifier].ForeColor = Color.FromArgb(208, 218, 226);
                    break;

                case ThemeType.Light:
                    _textbox.Styles[Style.Sql.Operator].ForeColor = Color.FromArgb(51, 51, 51);
                    _textbox.Styles[Style.Sql.Identifier].ForeColor = Color.FromArgb(51, 51, 51);
                    break;
            }

            _textbox.Lexer = Lexer.Sql;
            _textbox.SetKeywords(0, string.Join(" ", Constants.KeyWordList.Select(s => s.ToLowerInvariant()).Concat(Constants.KeyWordList.Select(s => s.ToUpperInvariant()))));
            _textbox.SetKeywords(1, string.Join(" ", Constants.BuiltInKeyWordList.Select(s => s.ToUpperInvariant()).Concat(Constants.BuiltInKeyWordList.Select(s => s.ToLowerInvariant()))));
        }
    }
}