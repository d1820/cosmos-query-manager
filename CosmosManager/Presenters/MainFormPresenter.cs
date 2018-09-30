using CosmosManager.Domain;
using CosmosManager.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CosmosManager.Presenters
{
    public class MainFormPresenter
    {
        private readonly IMainForm _view;
        private string _rootDir;

        public List<Connection> Connections { get; private set; }

        public MainFormPresenter(IMainForm view)
        {
            _view = view;
            _view.Presenter = this;
        }

        public void PopulateTreeView(string rootDir)
        {
            TreeNode rootNode;
            _view.ClearFileTreeView();
            var info = new DirectoryInfo(rootDir);
            if (info.Exists)
            {
                _rootDir = rootDir;
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                GetFiles(rootNode);
                _view.AddFileNode(rootNode);
            }
        }

         public void RefreshTreeView()
        {
            PopulateTreeView(_rootDir);
        }

        public void LoadSubDirsAndFiles(DirectoryInfo folder, TreeNode currentNode)
        {
            GetDirectories(folder.GetDirectories(), currentNode);
            GetFiles(currentNode);
        }

        public void SetupConnections(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            try
            {
                Connections = JsonConvert.DeserializeObject<List<Connection>>(jsonString);
                _view.SetConnectionsOnExistingTabs();
            }
            catch (Exception)
            {
                _view.ShowMessage("Unable to open connections file. Please verify format, and try again", "Error Parsing Connections File");
            }
        }

        public void UpdateNewQueryTabName(string newTabName)
        {
            _view.UpdateNewQueryTabName(newTabName);
        }

        public void SetStatusBarMessage(string message)
        {
            _view.SetStatusBarMessage(message);
        }

        public void SaveNewQuery(string fileLocation, TreeNode currentNode)
        {
            File.WriteAllText(fileLocation, "");
            var fileInfo = new FileInfo(fileLocation);
            LoadSubDirsAndFiles(fileInfo.Directory, currentNode);
        }

        public void CreateTempQueryTab(string query)
        {
            _view.CreateTempQueryTab(query);
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (var subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "Folder";
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode);
                }
                nodeToAddTo.Nodes.Add(aNode);
            }
        }

        private void GetFiles(TreeNode nodeToAddTo)
        {
            var nodeDirInfo = (DirectoryInfo)nodeToAddTo.Tag;

            TreeNode aNode;
            nodeToAddTo.Nodes.Clear();
            var files = nodeDirInfo.GetFiles("*.csql", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                nodeToAddTo.Expand();
            }
            foreach (var file in files)
            {
                aNode = new TreeNode(file.Name, 1, 1);
                aNode.Tag = file;
                aNode.ImageKey = "File";
                nodeToAddTo.Nodes.Add(aNode);
            }
        }




    }
}
