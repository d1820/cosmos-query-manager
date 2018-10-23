using System.Collections.Generic;
using System.Windows.Forms;

namespace CosmosManager.Interfaces
{
    public interface IQueryWindowControl
    {
        object[] ConnectionsList { get; set; }
        string Query { get; set; }
        string QueryOutput { get; }
        string DocumentText { get; set; }
        IQueryWindowPresenter Presenter { set; }
        IMainFormPresenter MainPresenter { set; }

        void ClearStats();

        DialogResult ShowMessage(string message, string title = null, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None);

        void RenderResults(IReadOnlyCollection<object> results);

        void SetStatusBarMessage(string message, bool ignoreClearTimer = false);

        void ResetResultsView();

        void ShowOutputTab();

        void AppendToQueryOutput(string message);

        void ResetQueryOutput();

        void SetUpdatedResultDocument(object document);
    }
}