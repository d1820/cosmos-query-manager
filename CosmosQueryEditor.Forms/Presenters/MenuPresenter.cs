using System.IO;
using CosmosQueryEditor.Forms.EventArgs;
using CosmosQueryEditor.Forms.Interfaces;

namespace CosmosQueryEditor.Forms.Presenters {
    public class MenuPresenter : IMenuPresenter
    {
        public IMenuView MenuView {get;set;}


        private DirectoryInfo SelectedDirectory { get; set; }
        private FileInfo SelectedFile { get; set; }

        public void SetSelectedFolder(DirectoryInfo directoryInfo)
        {
            SelectedFile = null;
            SelectedDirectory = directoryInfo;
            MenuView.OnFolderChange?.Invoke(this, new FolderChangeEventArgs(directoryInfo));
        }

        public void SetSelectedFile(FileInfo fileInfo)
        {
            SelectedFile = fileInfo;
            SelectedDirectory = null;
            MenuView.OnFileChange?.Invoke(this, new FileChangeEventArgs(fileInfo));
        }

        public void ShowDirectoryDialog()
        {
            MenuView.ShowFolderDialog();
        }

        public void ShowFileDialog()
        {
            MenuView.ShowFileDialog();
        }
    }
}