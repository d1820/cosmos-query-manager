using ScintillaNET;

namespace CosmosManager.Interfaces
{
    public interface ITextStyler
    {
        void SyntaxifyTextBox(Scintilla textQuery);
    }

    public interface IQueryStyler: ITextStyler
    {
    }

    public interface IJsonStyler: ITextStyler
    {
    }
}