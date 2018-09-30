using CosmosManager.Presenters;
using System.Collections.Generic;

namespace CosmosManager.Interfaces
{
    public interface IQueryWindowControl
    {
        object[] ConnectionsList { get; set; }
        string Query { get; set; }
        string Stats { get; set; }
        string DocumentText { get; set; }
        QueryWindowPresenter Presenter { set; }
        MainFormPresenter MainPresenter { set; }
        void ToggleStatsPanel(bool collapse);
        void ClearStats();
        void ShowMessage(string message, string title = null);

        void RenderResults(IReadOnlyCollection<object> results);

        void SetStatusBarMessage(string message);
    }
}