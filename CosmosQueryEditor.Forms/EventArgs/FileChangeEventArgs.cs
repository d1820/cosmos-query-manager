using System.IO;

namespace CosmosQueryEditor.Forms.EventArgs {
    public class FileChangeEventArgs : System.EventArgs
    {
        public FileInfo FileInfo { get; }

        public FileChangeEventArgs(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }
    }
}