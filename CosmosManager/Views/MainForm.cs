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
        public MainFormPresenter Presenter { private get; set; }

        public MainForm()
        {
            InitializeComponent();
            //Presenter.PopulateTreeView(@"C:\Users\Administrator.WIN-JLVDOKCVKPQ\Desktop\TestScripts");

        }

        public void ClearFileTreeView()
        {
            fileTreeView.Nodes.Clear();
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

        private void fileTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var newSelected = e.Node;
            if (newSelected.Tag is DirectoryInfo)
            {
                var nodeDirInfo = (DirectoryInfo)newSelected.Tag;
                Presenter.LoadSubDirsAndFiles(nodeDirInfo, newSelected);
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
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

                //create the tab
                var tab = new TabPage(fi.Name + "   ");
                tab.Name = $"tab{queryTabControl.TabPages.Count + 1}";


                var queryWindow = new QueryWindowControl();
                queryWindow.Dock = DockStyle.Fill;
                queryWindow.MainPresenter = Presenter;

                var presenter = new QueryWindowPresenter(queryWindow);
                presenter.SetFile(fi);
                if (Presenter.Connections != null)
                {
                    presenter.SetConnections(Presenter.Connections);
                }
                tab.Tag = presenter;
                tab.Controls.Add(queryWindow);
                queryTabControl.TabPages.Add(tab);

            }
        }

        private void queryTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabPage = queryTabControl.TabPages[e.Index];
            var tabRect = queryTabControl.GetTabRect(e.Index);
            tabRect.Inflate(-2, -2);
            var closeImage = Properties.Resources.closeIcon;
            e.Graphics.DrawImage(closeImage, (tabRect.Right - 10), tabRect.Top + (tabRect.Height - closeImage.Height) / 2, 10, 10);

            TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font, tabRect, tabPage.ForeColor, TextFormatFlags.Left);
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

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var helpForm = new HelpForm();
            helpForm.Show();
        }

        public void SetStatusBarMessage(string message)
        {
            appStatusLabel.Text = message;
        }

        private void createNewQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newFileForm = new NewFileForm();
            if (newFileForm.ShowDialog() == DialogResult.OK)
            {
                var saveFile = $"{ (_contextSelectedNode.Tag as DirectoryInfo).FullName}/{newFileForm.FileName}.csql";
                Presenter.SaveNewQuery(saveFile, _contextSelectedNode);
            }
        }

        private TreeNode _contextSelectedNode;
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
    }
}
