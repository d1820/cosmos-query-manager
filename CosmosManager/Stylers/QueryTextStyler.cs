using CosmosManager.Domain;
using CosmosManager.Interfaces;
using ScintillaNET;
using System.Drawing;
using System.Linq;

namespace CosmosManager.Stylers
{
    public class QueryTextStyler : IQueryStyler
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

            // Configure the SQL lexer styles
            _textbox.Styles[Style.Sql.Identifier].ForeColor = IntToColor(0xD0DAE2);
            _textbox.Styles[Style.Sql.Comment].ForeColor = IntToColor(0x6A993E);
            _textbox.Styles[Style.Sql.CommentLine].ForeColor = IntToColor(0x6A993E);
            _textbox.Styles[Style.Sql.CommentDoc].ForeColor = IntToColor(0x6A993E);
            _textbox.Styles[Style.Sql.Number].ForeColor = IntToColor(0xb453e9);
            _textbox.Styles[Style.Sql.String].ForeColor = IntToColor(0xFFFF00);
            _textbox.Styles[Style.Sql.Character].ForeColor = IntToColor(0xE95454);
            _textbox.Styles[Style.Sql.Operator].ForeColor = IntToColor(0xE0E0E0);
            _textbox.Styles[Style.Sql.CommentLineDoc].ForeColor = IntToColor(0x77A7DB);
            _textbox.Styles[Style.Sql.Word].ForeColor = IntToColor(0x48A8EE);
            _textbox.Styles[Style.Sql.Word2].ForeColor = IntToColor(0xF98906);
            _textbox.Styles[Style.Sql.CommentDocKeyword].ForeColor = Color.Gray;
            _textbox.Styles[Style.Sql.CommentDocKeywordError].ForeColor = Color.Gray;

            _textbox.Lexer = Lexer.Sql;
            _textbox.SetKeywords(0, string.Join(" ", Constants.KeyWordList.Select(s => s.Value).Concat(Constants.KeyWordList.Select(s => s.Key))));
            _textbox.SetKeywords(1, string.Join(" ", Constants.BuiltInKeyWordList.Select(s => s).Concat(Constants.BuiltInKeyWordList.Select(s => s.ToLowerInvariant()))));

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