using System.IO;
using CosmosQueryEditor.Forms.Presenters;

namespace CosmosQueryEditor.Forms.Interfaces {
    public interface IMenuPresenter
    {
        IMenuView MenuView {get;set;}
        void SetSelectedFolder(DirectoryInfo directoryInfo);
        void SetSelectedFile(FileInfo fileInfo);
        void ShowDirectoryDialog();
        void ShowFileDialog();
    }
}