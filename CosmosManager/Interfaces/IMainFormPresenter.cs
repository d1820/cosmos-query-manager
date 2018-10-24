using CosmosManager.Domain;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CosmosManager.Interfaces
{
    public interface IMainFormPresenter : IPresenter
    {
        List<Connection> Connections { get; }

        void CreateTempQueryTab(string query);

        Color GetConnectionColor(string connectionName);

        void LoadSubDirsAndFiles(DirectoryInfo folder, TreeNode currentNode);

        void OpenTransactionCacheFolder();

        void OpenInFileExporer(string path);

        void PopulateTreeView(string rootDir);

        void RefreshTreeView();

        void SaveNewQuery(string fileLocation, TreeNode currentNode);

        void SetStatusBarMessage(string message, bool ignoreClearTimer = false);

        void SetupConnections(string filePath);

        void UpdateNewQueryTabName(string newTabName);

        void UpdateTabHeaderColor();

        void UpdateTransactionFolderSize();
    }
}