using System.Windows.Forms;

namespace CosmosManager.Interfaces
{
    public interface IMainForm
    {
        void SetStatusBarMessage(string message);

        IMainFormPresenter Presenter { set; }

        void RenderTheme();

        void ClearFileTreeView();

        void AddFileNode(TreeNode newNode);

        void SetConnectionsOnExistingTabs();

        void ShowMessage(string message, string title = null);

        void UpdateNewQueryTabName(string newTabName);

        void CreateTempQueryTab(string query);

        void SetTransactionCacheLabel(string text);

        void UpdateTabHeaderColors();

        void SetFileWatcherPath(string path);
    }
}