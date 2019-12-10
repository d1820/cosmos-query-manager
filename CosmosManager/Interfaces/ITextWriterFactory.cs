using CosmosManager.Interfaces;

namespace CosmosManager.Interfaces
{
    public interface ITextWriterFactory
    {
        ITextWriter Create(string path, bool append = false);
    }
}