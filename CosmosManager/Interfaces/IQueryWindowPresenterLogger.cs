using Microsoft.Extensions.Logging;

namespace CosmosManager.Interfaces
{
    public interface IQueryWindowPresenterLogger : ILogger
    {
        void SetPresenter(IQueryWindowPresenter presenter);
    }
}