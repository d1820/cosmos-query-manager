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
using CosmosQueryEditor.Forms.EventArgs;
using CosmosQueryEditor.Forms.Interfaces;
using CosmosQueryEditor.Forms.Presenters;

namespace CosmosQueryEditor.Forms
{
    public partial class Main : Form, IFileListView, IMenuView
    {
        private readonly IMenuPresenter _menuPresenter;

        public Main(IMenuPresenter menuPresenter)
        {
            InitializeComponent();

            _menuPresenter = menuPresenter;
            _menuPresenter.MenuView = this;


            OnFolderChange = (sender, args) =>
                             {
                                 fileView.Nodes.Clear();
                                 //load the treeview
                                 var root = new TreeNode
                                            {
                                                    Tag = args.DirectoryInfo,
                                                    Text = args.DirectoryInfo.Name
                                            };

                                 fileView.Nodes.Add(root);

                                 //loop and add
                             };
            OnFileChange = (sender, args) =>
                           {
                               fileView.Nodes.Clear();
                               var root = new TreeNode
                                          {
                                                  Tag = args.FileInfo,
                                                  Text = args.FileInfo.Name
                                          };

                               fileView.Nodes.Add(root);
                           };
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //raise presenter event
            //load treeview with directories and files
            _menuPresenter.ShowDirectoryDialog();
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
           //raise presenter event
           //load tree view with fileInfo
            _menuPresenter.ShowFileDialog();
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

        }

        public void LoadTreeView(List<DirectoryInfo> directories)
        {
            throw new NotImplementedException();
        }

        public void OpenFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public void ShowFolderDialog()
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                _menuPresenter.SetSelectedFolder(new DirectoryInfo(folderBrowserDialog1.SelectedPath));
            }
        }

        public void ShowFileDialog()
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                _menuPresenter.SetSelectedFile(new FileInfo(openFileDialog1.FileName));
            }
        }

        public EventHandler<FolderChangeEventArgs> OnFolderChange { get; set; }
        public EventHandler<FileChangeEventArgs> OnFileChange { get; set; }
    }
}
