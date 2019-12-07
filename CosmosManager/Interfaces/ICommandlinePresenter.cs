using System.Threading;
using System.Threading.Tasks;
using CosmosManager.Domain;

namespace CosmosManager.Interfaces
{
    public interface ICommandlinePresenter: IPresenter, IDisplayPresenter, IConnectedPresenter
    {
        Task<int> RunAsync(string query, CommandlineOptions options, CancellationToken cancelToken);
        void RenderTables();
    }
}