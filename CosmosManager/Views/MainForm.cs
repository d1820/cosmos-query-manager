using CosmosManager.Domain;
using CosmosManager.Interfaces;
using CosmosManager.Presenters;
using CosmosManager.Views;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CosmosManager
{
    public partial class MainForm : Form, IMainForm
    {
        private TreeNode _contextSelectedNode;
        private TabPage contextTabPage;
        private readonly IFormOpener _formManager;

        public IMainFormPresenter Presenter { private get; set; }

        public MainForm(IFormOpener formManager, IMainFormPresenter presenter)
        {
            InitializeComponent();
            _formManager = formManager;
            presenter.InitializePresenter(new
            {
                MainForm = this
            });
        }

        protected override void WndProc(ref Message m)
        {
            // Suppress the WM_UPDATEUISTATE message
            if (m.Msg == 0x128)
                return;
            base.WndProc(ref m);
        }

        public void ClearFileTreeView()
        {
            fileTreeView.Nodes.Clear();
        }

        public void SetFileWatcherPath(string path)
        {
            fileSystemWatcher1.Path = path;
        }

        public void AddFileNode(TreeNode newNode)
        {
            fileTreeView.Nodes.Add(newNode);
        }

        public void SetConnectionsOnExistingTabs()
        {
            foreach (TabPage tab in queryTabControl.TabPages)
            {
                (tab.Tag as QueryWindowPresenter).SetConnections(Presenter.Connections);
            }
        }

        public void ShowMessage(string message, string title = null)
        {
            MessageBox.Show(message, title);
        }

        public void SetStatusBarMessage(string message)
        {
            appStatusLabel.Text = message;
        }

        public void UpdateNewQueryTabName(string newTabName)
        {
            queryTabControl.SelectedTab.Text = newTabName + "    ";
            Presenter.RefreshTreeView();
        }

        public void CreateTempQueryTab(string query)
        {
            var tabName = "New Query *";
            CreateTab(tabName, null, query);
        }

        public void SetTransactionCacheLabel(string text)
        {
            transactionCacheSizeLabel.Text = text;
        }

        public void UpdateTabHeaderColors()
        {
            queryTabControl.Invalidate();
        }

        private void fileTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var newSelected = e.Node;
            if (newSelected.Tag is DirectoryInfo)
            {
                if (newSelected.IsExpanded)
                {
                    var nodeDirInfo = (DirectoryInfo)newSelected.Tag;
                    Presenter.LoadSubDirsAndFiles(nodeDirInfo, newSelected);
                }

            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                queryTabControl.TabPages.Clear();
                Presenter.PopulateTreeView(folderBrowserDialog1.SelectedPath);
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
                CreateTab(fi.Name, fi);
            }
        }

        private void queryTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabRect = queryTabControl.GetTabRect(e.Index);
            var tabPage = queryTabControl.TabPages[e.Index];
            var presenter = tabPage.Tag as QueryWindowPresenter;
            var brushColor = Color.Transparent;
            if (presenter.SelectedConnection != null)
            {
                brushColor = Presenter.GetConnectionColor(presenter.SelectedConnection.Name);
            }

            using (Brush br = new SolidBrush(brushColor))
            {
                e.Graphics.FillRectangle(br, tabRect);
                var rect = tabRect;
                rect.Offset(0, 1);
                rect.Inflate(0, -1);
                e.Graphics.DrawRectangle(Pens.DarkGray, rect);
                e.DrawFocusRectangle();
            }

            tabRect.Inflate(-2, -2);
            var closeImage = Properties.Resources.closeIcon;
            e.Graphics.DrawImage(closeImage, (tabRect.Right - 10), tabRect.Top + (tabRect.Height - closeImage.Height) / 2, 10, 10);

            TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font, tabRect, Color.Black, TextFormatFlags.Left);
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
                Presenter.SetupConnections(openFileDialog1.FileName);
            }
        }

        private void createNewQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var (result, form) = _formManager.ShowModalForm<NewFileForm>();
            if (result == DialogResult.OK)
            {
                var saveFile = $"{ (_contextSelectedNode.Tag as DirectoryInfo).FullName}/{form.FileName}.csql";
                Presenter.SaveNewQuery(saveFile, _contextSelectedNode);
            }
        }

        private void fileTreeView_MouseUp(object sender, MouseEventArgs e)
        {
            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {
                // Point where the mouse is clicked.
                var p = new Point(e.X, e.Y);

                // Get the node that the user has clicked.
                var node = fileTreeView.GetNodeAt(p);
                if (node != null && node.Tag is DirectoryInfo)
                {
                    _contextSelectedNode = fileTreeView.SelectedNode;
                    fileTreeView.SelectedNode = node;
                    contextMenuStrip1.Show(fileTreeView, p);
                    fileTreeView.SelectedNode = _contextSelectedNode;
                }
            }
        }

        private void duplicateTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (contextTabPage != null && contextTabPage.Tag != null)
            {
                var currentTabPresenter = (QueryWindowPresenter)contextTabPage.Tag;
                CreateTempQueryTab(currentTabPresenter.CurrentTabQuery);
                contextTabPage = null;
            }
        }

        private void viewTransactionCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Presenter.OpenTransactionCacheFolder();
        }

        private void queryTabControl_MouseUp(object sender, MouseEventArgs e)
        {
            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {
                for (var i = 0; i < queryTabControl.TabCount; i++)
                {
                    var r = queryTabControl.GetTabRect(i);
                    if (r.Contains(e.Location))
                    {
                        contextTabPage = queryTabControl.TabPages[i];
                        if (contextTabPage.Tag != null)
                        {
                            contextTabs.Show(queryTabControl, e.Location);
                        }
                        return;
                    }
                }
            }
        }

        private void transactionCacheSizeLabel_DoubleClick(object sender, EventArgs e)
        {
            Presenter.OpenTransactionCacheFolder();
        }

        private void CreateTab(string tabName, FileInfo fileInfo, string tempQuery = null)
        {
            var tab = new TabPage(tabName + "   ");
            tab.Name = $"tab{queryTabControl.TabPages.Count + 1}";

            var queryWindow = new QueryWindowControl();
            queryWindow.Dock = DockStyle.Fill;
            queryWindow.MainPresenter = Presenter;

            //var presenter = new QueryWindowPresenter(queryWindow, queryTabControl.TabPages.Count);

            var presenter = AppReferences.Container.GetInstance<IQueryWindowPresenter>();
            presenter.InitializePresenter(new
            {
                TabIndexReference = queryTabControl.TabPages.Count,
                QueryWindowControl = queryWindow
            });

            if (fileInfo != null)
            {
                presenter.SetFile(fileInfo);
            }
            else if (!string.IsNullOrEmpty(tempQuery))
            {
                presenter.SetTempQuery(tempQuery);
            }
            if (Presenter.Connections != null)
            {
                presenter.SetConnections(Presenter.Connections);
            }
            tab.Tag = presenter;
            tab.Controls.Add(queryWindow);
            queryTabControl.TabPages.Add(tab);
            queryTabControl.SelectedTab = tab;
        }

        private void guideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _formManager.ShowModelessForm<HelpForm>();
        }

        private void reportABugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://github.com/d1820/cosmos-query-manager/issues");
        }

        private void fileSystemWatcher1_Created(object sender, FileSystemEventArgs e)
        {
            Presenter.UpdateTransactionFolderSize();
        }

        private void fileSystemWatcher1_Deleted(object sender, FileSystemEventArgs e)
        {
            Presenter.UpdateTransactionFolderSize();
        }

        private void newQueryTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateTempQueryTab("");
        }

        private void newFileQueryTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateTempQueryTab("");
        }


        private void tabBackgroundPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //find the bounding box of the top tab area and if greater then last tab X/Y open new tab
            var lastTab = queryTabControl.GetTabRect(queryTabControl.TabPages.Count - 1);
            if (e.Location.X > lastTab.Right && e.Location.Y < lastTab.Bottom)
            {
                CreateTempQueryTab("");
            }
        }

        private void aboutCosmosManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _formManager.ShowModalForm<AboutCosmosManager>();
        }

        private void openInFileExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Presenter.OpenInFileExporer((_contextSelectedNode.Tag as DirectoryInfo)?.FullName);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}