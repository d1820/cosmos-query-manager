using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CosmosQueryEditor.Forms.Presenters;

namespace CosmosQueryEditor.Forms
{
    public partial class Main : Form, IFileListView
    {
        public Main()
        {
            InitializeComponent();
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //raise presenter event
            //load treeview with directories and files
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {

            }
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
           //raise presenter event
           //load tree view with fileInfo
        }

        private void fileView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is FileInfo)
            {

            }
        }

        private void fileView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is DirectoryInfo)
            {

            }
        }


        public void LoadTreeView(List<FileInfo> files)
        {
            this.fileView.Nodes.
        }

        public void LoadTreeView(List<DirectoryInfo> directories)
        {
            throw new NotImplementedException();
        }

        public void OpenFile(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
