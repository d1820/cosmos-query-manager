using System.Collections.Generic;

namespace CosmosManager.Interfaces
{
    public interface IResultsPresenter
    {
        void RenderResults(IReadOnlyCollection<object> results);
        void ResetStatsLog();
        void AddToStatsLog(string message);
        void ToggleStatsPanel(bool collapse);
    }
}