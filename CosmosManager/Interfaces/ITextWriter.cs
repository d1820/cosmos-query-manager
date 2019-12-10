using System;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{
    public interface ITextWriter: IDisposable
    {
        void Close();
        Task WriteAsync(string value);
        void WriteLine(string value);
    }
}