using CosmosManager.Domain;
using CosmosManager.Interfaces;
using CosmosManager.Presenters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CosmosManager
{

    public partial class MainForm : Form, IMainForm
    {


        public static List<Connection> Connections { get; private set; }
        public MainFormPresenter Presenter { private get; set; }

        public MainForm()
        {
            InitializeComponent();
            PopulateTreeView(@"C:\Users\Administrator.WIN-JLVDOKCVKPQ\Desktop\TestScripts");

        }


        private void PopulateTreeView(string rootDir)
        {
            TreeNode rootNode;
            fileTreeView.Nodes.Clear();
            var info = new DirectoryInfo(rootDir);
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                GetFiles(rootNode);
                fileTreeView.Nodes.Add(rootNode);
            }
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

        private void fileTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var newSelected = e.Node;
            if (newSelected.Tag is DirectoryInfo)
            {
                var nodeDirInfo = (DirectoryInfo)newSelected.Tag;

                GetDirectories(nodeDirInfo.GetDirectories(), newSelected);
                GetFiles(newSelected);
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                PopulateTreeView(folderBrowserDialog1.SelectedPath);
            }
        }

        private void fileTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is FileInfo)
            {
                var fi = (FileInfo)e.Node.Tag;

                //check if tab already open
                foreach (TabPage tabpage in queryTabControl.TabPages)
                {
                    if (tabpage.Text.Trim() == fi.Name)
                    {
                        queryTabControl.SelectedTab = tabpage;
                        return;
                    }
                }

                //create the tab
                var tab = new TabPage(fi.Name + "   ");
                tab.Name = $"tab{queryTabControl.TabPages.Count + 1}";


                var queryWindow = new QueryWindowControl();
                queryWindow.Dock = DockStyle.Fill;
                queryWindow.MainPresenter = Presenter;

                var presenter = new QueryWindowPresenter(queryWindow);
                presenter.SetFile(fi);
                if(Connections != null)
                {
                    presenter.SetConnections(Connections);
                }
                tab.Tag = presenter;
                tab.Controls.Add(queryWindow);
                queryTabControl.TabPages.Add(tab);

            }
        }

        private void queryTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabPage = this.queryTabControl.TabPages[e.Index];

            var tabRect = this.queryTabControl.GetTabRect(e.Index);
            tabRect.Inflate(-2, -2);
            var closeImage = Properties.Resources.closeIcon;
            e.Graphics.DrawImage(closeImage,
                (tabRect.Right - 10),
                tabRect.Top + (tabRect.Height - closeImage.Height) / 2,
                10, 10);



            TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font,
                tabRect, tabPage.ForeColor, TextFormatFlags.Left);

        }

        private void queryTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            var tabRect = queryTabControl.GetTabRect(queryTabControl.SelectedIndex);
            var closeImage = Properties.Resources.closeIcon;
            var closeButton = new Rectangle((tabRect.Right - 10), tabRect.Top + (tabRect.Height - closeImage.Height) / 2, 10, 10);
            if (closeButton.Contains(e.Location))
            {
                queryTabControl.TabPages.Remove(queryTabControl.SelectedTab);
            }
        }

        private void loadConnectionFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select Connection Information File";
            openFileDialog1.Filter = "Connection File|*.json";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var jsonString = File.ReadAllText(openFileDialog1.FileName);
                try
                {
                    Connections = JsonConvert.DeserializeObject<List<Connection>>(jsonString);
                    foreach (TabPage tab in queryTabControl.TabPages)
                    {

                        (tab.Tag as QueryWindowPresenter).SetConnections(Connections);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to open connections file. Please verify format, and try again", "Error Parsing Connections File");
                }
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var helpForm = new HelpForm();
            helpForm.Show();
        }

        public void SetStatusBarMessage(string message)
        {
            appStatusLabel.Text = message;
        }
    }
}
