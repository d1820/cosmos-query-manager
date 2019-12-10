using System;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{
    public interface ITextWriter: IDisposable
    {
        void Close();
        ITextWriter Open(string path, bool append = false);
        Task WriteAsync(string value);
        void WriteLine(string value);
    }
}