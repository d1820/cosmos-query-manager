using CosmosManager.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{
    public interface ICommandlinePresenter: IPresenter, IDisplayPresenter, IConnectedPresenter, IDisposable
    {
        Task<int> RunAsync(string queryGroupName, string query, CancellationToken cancelToken);

        //Task WriteResults();

        void WriteHeader(char outlineChar, int width, string message);
    }
}