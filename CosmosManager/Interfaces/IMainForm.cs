using CosmosManager.Presenters;
using System.Windows.Forms;

namespace CosmosManager.Interfaces
{
    public interface IMainForm
    {
        void SetStatusBarMessage(string message);
         MainFormPresenter Presenter { set; }
        void ClearFileTreeView();
        void AddFileNode(TreeNode newNode);
        void SetConnectionsOnExistingTabs();
        void ShowMessage(string message, string title = null);
    }
}