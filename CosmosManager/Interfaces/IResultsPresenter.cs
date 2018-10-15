using System.Collections.Generic;

namespace CosmosManager.Interfaces
{
    public interface IResultsPresenter
    {
        void RenderResults(IReadOnlyCollection<object> results);

        void ResetQueryOutput();

        void AddToQueryOutput(string message);

        void ShowOutputTab();
    }
}