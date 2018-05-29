using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CosmosQueryEditor.Forms.Presenters
{
    public class FileListPresenter
    {
        private readonly IFileListView _fileListView;

        public FileListPresenter(IFileListView fileListView)
        {
            _fileListView = fileListView;
        }

        public void OpenFile()
        {

        }

        public void GetFilesFromPath(string directoryPath)
        {

        }
    }

    public interface IFileListView {
        void LoadTreeView( List<FileInfo> files);
        void LoadTreeView( List<DirectoryInfo> directories);
        void OpenFile(string filePath);

    }

    public class FileListModel
    {

    }

    public class FileListView : IFileListView
    {

        public void LoadTreeView( List<FileInfo> files)
        {

        }

        public void LoadTreeView( List<DirectoryInfo> directories)
        {

        }

        public void OpenFile(string filePath)
        {
            //raises an event??
        }

    }
}
