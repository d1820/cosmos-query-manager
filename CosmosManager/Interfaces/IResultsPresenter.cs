using System.Collections.Generic;

namespace CosmosManager.Interfaces
{
    public interface IResultsPresenter
    {
        void RenderResults(IReadOnlyCollection<object> results);

        void ResetQueryOutput();

        void AddToStatsLog(string message);

        void ShowOutputTab();
    }
}