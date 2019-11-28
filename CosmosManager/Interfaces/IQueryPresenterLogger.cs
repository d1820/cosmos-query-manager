using CosmosManager.Presenters;
using Microsoft.Extensions.Logging;

namespace CosmosManager.Interfaces
{
    public interface IQueryPresenterLogger : ILogger
    {
        void SetPresenter(BaseQueryPresenter presenter);
    }
}