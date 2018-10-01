using CosmosManager.Domain;
using CosmosManager.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CosmosManager.Presenters
{
    public class MainFormPresenter
    {
        private readonly IMainForm _view;
        private string _rootDir;

        private List<Color> _colors = new List<Color>{
            Color.PaleGreen,
            Color.PaleTurquoise,
            Color.Plum,
            Color.WhiteSmoke,
            Color.Moccasin,
            Color.PapayaWhip,
            Color.LightSalmon,
            Color.MediumAquamarine,
            Color.PaleGoldenrod,
        };

        private Dictionary<string, Color> _tabColors = new Dictionary<string, Color>();

        public string AppDataFolder { get; private set; }
        public string TransactionCacheDataFolder { get; private set; }

        public List<Connection> Connections { get; private set; }

        public MainFormPresenter(IMainForm view)
        {
            _view = view;
            _view.Presenter = this;
            InitializeUserAppData();
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
                for (var i = 0; i < Connections.Count; i++)
                {
                    if (i <= _colors.Count - 1)
                    {
                        _tabColors.Add(Connections[i].Name, _colors[i]);
                    }
                    else
                    {
                        _tabColors.Add(Connections[i].Name, Color.Transparent);
                    }

                }
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

        public void OpenTransactionCacheFolder()
        {
            Process.Start(TransactionCacheDataFolder);
        }

        public void UpdateTabHeaderColor()
        {
            _view.UpdateTabHeaderColors();
        }

        public Color GetConnectionColor(string connectionName)
        {
            if (_tabColors.ContainsKey(connectionName))
            {
                return _tabColors[connectionName];
            }
            return Color.Transparent;
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

        private void InitializeUserAppData()
        {
            // The folder for the roaming current user
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            AppDataFolder = Path.Combine(folder, "CosmosManager");
            Directory.CreateDirectory(AppDataFolder);

            TransactionCacheDataFolder = Path.Combine(folder, "CosmosManager/TransactionCache");
            Directory.CreateDirectory(TransactionCacheDataFolder);

            var size = CalculateFolderSize(TransactionCacheDataFolder);
            _view.SetTransactionCacheLabel($"Transaction Cache: {BytesToSting(size)}");

        }

        private long CalculateFolderSize(string folderPath)
        {
            //get the size of the cache folder
            var a = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            long b = 0;
            foreach (var name in a)
            {
                var info = new FileInfo(name);
                b += info.Length;
            }
            return b;
        }

        private string BytesToSting(long bytes)
        {
            var suffix = new List<string> { "b", "KB", "MB", "GB", "TB", "PB", "EB" };
            var index = 0;
            do
            {
                bytes /= 1024;
                index++;
            }
            while (bytes >= 1024);
            return string.Format("{0:0.00} {1}", bytes, suffix[index]);
        }

    }
}
