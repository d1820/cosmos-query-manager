using CosmosManager.Domain;
using CosmosManager.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Timers;
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
        private System.Timers.Timer statusTimer;
        private Dictionary<string, Color> _tabColors = new Dictionary<string, Color>();

        public List<Connection> Connections { get; private set; }



        public MainFormPresenter(IMainForm view)
        {
            _view = view;
            _view.Presenter = this;
            InitializeUserAppData();

            statusTimer = new System.Timers.Timer();
            statusTimer.Elapsed += StatusTimer_Elapsed;
            statusTimer.Interval = 5000;
        }

        private void StatusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _view.SetStatusBarMessage("Ready");
            statusTimer.Stop();
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
                _tabColors.Clear();
                _view.SetConnectionsOnExistingTabs();
                for (var i = 0; i < Connections.Count; i++)
                {
                    if (_tabColors.ContainsKey(Connections[i].Name))
                    {
                        continue;
                    }
                    if (i <= _colors.Count - 1)
                    {
                        _tabColors.Add(Connections[i].Name, _colors[i]);
                    }
                    else
                    {
                        _tabColors.Add(Connections[i].Name, Color.Transparent);
                    }
                }
                SetStatusBarMessage("Connections Loaded");
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Unable to open connections file. Please verify format, and try again. Error: {ex.Message}. Base: {ex.GetBaseException().Message}", "Error Parsing Connections File");
            }
        }

        public void UpdateNewQueryTabName(string newTabName)
        {
            _view.UpdateNewQueryTabName(newTabName);
        }


        public void SetStatusBarMessage(string message, bool ignoreClearTimer = false)
        {
            _view.SetStatusBarMessage(message);
            if (!ignoreClearTimer)
            {
                statusTimer.Start();
            }
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
            Process.Start(AppReferences.TransactionCacheDataFolder);
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

        public void UpdateTransactionFolderSize()
        {
            var size = CalculateFolderSize(AppReferences.TransactionCacheDataFolder);
            _view.SetTransactionCacheLabel($"Transaction Cache: {BytesToSting(size)}");
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
            AppReferences.AppDataFolder = Path.Combine(folder, "CosmosManager");
            Directory.CreateDirectory(AppReferences.AppDataFolder);

            AppReferences.TransactionCacheDataFolder = Path.Combine(folder, "CosmosManager/TransactionCache");
            Directory.CreateDirectory(AppReferences.TransactionCacheDataFolder);

            var size = CalculateFolderSize(AppReferences.TransactionCacheDataFolder);
            _view.SetTransactionCacheLabel($"Transaction Cache: {BytesToSting(size)}");
            _view.SetFileWatcherPath(AppReferences.TransactionCacheDataFolder);

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
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (bytes == 0)
            {
                return "0" + suf[0];
            }

            var newbytes = Math.Abs(bytes);
            var place = Convert.ToInt32(Math.Floor(Math.Log(newbytes, 1024)));
            var num = Math.Round(newbytes / Math.Pow(1024, place), 1);
            return (Math.Sign(bytes) * num).ToString() + suf[place];
        }
    }
}