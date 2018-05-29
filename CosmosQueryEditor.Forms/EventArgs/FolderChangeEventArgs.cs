using System.IO;

namespace CosmosQueryEditor.Forms.EventArgs {
    public class FolderChangeEventArgs : System.EventArgs
    {
        public DirectoryInfo DirectoryInfo { get; }

        public FolderChangeEventArgs(DirectoryInfo directoryInfo)
        {
            DirectoryInfo = directoryInfo;
        }
    }
}