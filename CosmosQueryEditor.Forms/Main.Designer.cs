namespace CosmosQueryEditor.Forms
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewCosmosConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelQueryRunning = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBarQueryRunning = new System.Windows.Forms.ToolStripProgressBar();
            this.ParentContainer = new System.Windows.Forms.SplitContainer();
            this.fileView = new System.Windows.Forms.TreeView();
            this.MainQueryContainer = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabQuery1 = new System.Windows.Forms.TabPage();
            this.query1Textbox = new System.Windows.Forms.RichTextBox();
            this.query1ToolStrip = new System.Windows.Forms.ToolStrip();
            this.resultConatiner = new System.Windows.Forms.SplitContainer();
            this.resultListView = new System.Windows.Forms.ListView();
            this.innerRightResultPanel = new System.Windows.Forms.Panel();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPageTextView = new System.Windows.Forms.TabPage();
            this.tabPageJsonView = new System.Windows.Forms.TabPage();
            this.treeViewJSON = new System.Windows.Forms.TreeView();
            this.resultMenuStrip = new System.Windows.Forms.MenuStrip();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ParentContainer)).BeginInit();
            this.ParentContainer.Panel1.SuspendLayout();
            this.ParentContainer.Panel2.SuspendLayout();
            this.ParentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainQueryContainer)).BeginInit();
            this.MainQueryContainer.Panel1.SuspendLayout();
            this.MainQueryContainer.Panel2.SuspendLayout();
            this.MainQueryContainer.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabQuery1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultConatiner)).BeginInit();
            this.resultConatiner.Panel1.SuspendLayout();
            this.resultConatiner.Panel2.SuspendLayout();
            this.resultConatiner.SuspendLayout();
            this.innerRightResultPanel.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPageJsonView.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Gray;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewCosmosConnectionToolStripMenuItem,
            this.openFolderToolStripMenuItem,
            this.openFileToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // addNewCosmosConnectionToolStripMenuItem
            // 
            this.addNewCosmosConnectionToolStripMenuItem.Name = "addNewCosmosConnectionToolStripMenuItem";
            this.addNewCosmosConnectionToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.addNewCosmosConnectionToolStripMenuItem.Text = "Add New Cosmos Connection";
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.openFolderToolStripMenuItem.Text = "Open Folder";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.openFileToolStripMenuItem.Text = "Open File";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelQueryRunning,
            this.toolStripProgressBarQueryRunning});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelQueryRunning
            // 
            this.toolStripStatusLabelQueryRunning.Name = "toolStripStatusLabelQueryRunning";
            this.toolStripStatusLabelQueryRunning.Size = new System.Drawing.Size(96, 17);
            this.toolStripStatusLabelQueryRunning.Text = "Query Running...";
            // 
            // toolStripProgressBarQueryRunning
            // 
            this.toolStripProgressBarQueryRunning.Name = "toolStripProgressBarQueryRunning";
            this.toolStripProgressBarQueryRunning.Size = new System.Drawing.Size(100, 16);
            // 
            // ParentContainer
            // 
            this.ParentContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParentContainer.Location = new System.Drawing.Point(0, 24);
            this.ParentContainer.Name = "ParentContainer";
            // 
            // ParentContainer.Panel1
            // 
            this.ParentContainer.Panel1.Controls.Add(this.fileView);
            // 
            // ParentContainer.Panel2
            // 
            this.ParentContainer.Panel2.Controls.Add(this.MainQueryContainer);
            this.ParentContainer.Size = new System.Drawing.Size(800, 404);
            this.ParentContainer.SplitterDistance = 201;
            this.ParentContainer.TabIndex = 2;
            // 
            // fileView
            // 
            this.fileView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileView.Location = new System.Drawing.Point(0, 0);
            this.fileView.Name = "fileView";
            this.fileView.Size = new System.Drawing.Size(201, 404);
            this.fileView.TabIndex = 0;
            // 
            // MainQueryContainer
            // 
            this.MainQueryContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainQueryContainer.Location = new System.Drawing.Point(0, 0);
            this.MainQueryContainer.Name = "MainQueryContainer";
            this.MainQueryContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // MainQueryContainer.Panel1
            // 
            this.MainQueryContainer.Panel1.Controls.Add(this.tabControl1);
            // 
            // MainQueryContainer.Panel2
            // 
            this.MainQueryContainer.Panel2.Controls.Add(this.resultConatiner);
            this.MainQueryContainer.Size = new System.Drawing.Size(595, 404);
            this.MainQueryContainer.SplitterDistance = 198;
            this.MainQueryContainer.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabQuery1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(595, 198);
            this.tabControl1.TabIndex = 0;
            // 
            // tabQuery1
            // 
            this.tabQuery1.Controls.Add(this.query1Textbox);
            this.tabQuery1.Controls.Add(this.query1ToolStrip);
            this.tabQuery1.Location = new System.Drawing.Point(4, 22);
            this.tabQuery1.Name = "tabQuery1";
            this.tabQuery1.Padding = new System.Windows.Forms.Padding(3);
            this.tabQuery1.Size = new System.Drawing.Size(587, 172);
            this.tabQuery1.TabIndex = 0;
            this.tabQuery1.Text = "Query or FileName";
            this.tabQuery1.UseVisualStyleBackColor = true;
            // 
            // query1Textbox
            // 
            this.query1Textbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.query1Textbox.Location = new System.Drawing.Point(3, 28);
            this.query1Textbox.Name = "query1Textbox";
            this.query1Textbox.Size = new System.Drawing.Size(581, 141);
            this.query1Textbox.TabIndex = 1;
            this.query1Textbox.Text = "";
            // 
            // query1ToolStrip
            // 
            this.query1ToolStrip.Location = new System.Drawing.Point(3, 3);
            this.query1ToolStrip.Name = "query1ToolStrip";
            this.query1ToolStrip.Size = new System.Drawing.Size(581, 25);
            this.query1ToolStrip.TabIndex = 0;
            this.query1ToolStrip.Text = "toolStrip1";
            // 
            // resultConatiner
            // 
            this.resultConatiner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultConatiner.Location = new System.Drawing.Point(0, 0);
            this.resultConatiner.Name = "resultConatiner";
            // 
            // resultConatiner.Panel1
            // 
            this.resultConatiner.Panel1.Controls.Add(this.resultListView);
            // 
            // resultConatiner.Panel2
            // 
            this.resultConatiner.Panel2.Controls.Add(this.innerRightResultPanel);
            this.resultConatiner.Size = new System.Drawing.Size(595, 202);
            this.resultConatiner.SplitterDistance = 157;
            this.resultConatiner.TabIndex = 0;
            // 
            // resultListView
            // 
            this.resultListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultListView.Location = new System.Drawing.Point(0, 0);
            this.resultListView.Name = "resultListView";
            this.resultListView.Size = new System.Drawing.Size(157, 202);
            this.resultListView.TabIndex = 0;
            this.resultListView.UseCompatibleStateImageBehavior = false;
            // 
            // innerRightResultPanel
            // 
            this.innerRightResultPanel.Controls.Add(this.tabControl2);
            this.innerRightResultPanel.Controls.Add(this.resultMenuStrip);
            this.innerRightResultPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.innerRightResultPanel.Location = new System.Drawing.Point(0, 0);
            this.innerRightResultPanel.Name = "innerRightResultPanel";
            this.innerRightResultPanel.Size = new System.Drawing.Size(434, 202);
            this.innerRightResultPanel.TabIndex = 0;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPageTextView);
            this.tabControl2.Controls.Add(this.tabPageJsonView);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 24);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(434, 178);
            this.tabControl2.TabIndex = 1;
            // 
            // tabPageTextView
            // 
            this.tabPageTextView.Location = new System.Drawing.Point(4, 22);
            this.tabPageTextView.Name = "tabPageTextView";
            this.tabPageTextView.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTextView.Size = new System.Drawing.Size(426, 152);
            this.tabPageTextView.TabIndex = 0;
            this.tabPageTextView.Text = "Test View";
            this.tabPageTextView.UseVisualStyleBackColor = true;
            // 
            // tabPageJsonView
            // 
            this.tabPageJsonView.Controls.Add(this.treeViewJSON);
            this.tabPageJsonView.Location = new System.Drawing.Point(4, 22);
            this.tabPageJsonView.Name = "tabPageJsonView";
            this.tabPageJsonView.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageJsonView.Size = new System.Drawing.Size(426, 152);
            this.tabPageJsonView.TabIndex = 1;
            this.tabPageJsonView.Text = "JSON View";
            this.tabPageJsonView.UseVisualStyleBackColor = true;
            // 
            // treeViewJSON
            // 
            this.treeViewJSON.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewJSON.Location = new System.Drawing.Point(3, 3);
            this.treeViewJSON.Name = "treeViewJSON";
            this.treeViewJSON.Size = new System.Drawing.Size(420, 146);
            this.treeViewJSON.TabIndex = 0;
            // 
            // resultMenuStrip
            // 
            this.resultMenuStrip.BackColor = System.Drawing.Color.Gray;
            this.resultMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.resultMenuStrip.Name = "resultMenuStrip";
            this.resultMenuStrip.Size = new System.Drawing.Size(434, 24);
            this.resultMenuStrip.TabIndex = 0;
            this.resultMenuStrip.Text = "menuStrip2";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ParentContainer);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Text = "Cosmos Query Manager";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ParentContainer.Panel1.ResumeLayout(false);
            this.ParentContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ParentContainer)).EndInit();
            this.ParentContainer.ResumeLayout(false);
            this.MainQueryContainer.Panel1.ResumeLayout(false);
            this.MainQueryContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainQueryContainer)).EndInit();
            this.MainQueryContainer.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabQuery1.ResumeLayout(false);
            this.tabQuery1.PerformLayout();
            this.resultConatiner.Panel1.ResumeLayout(false);
            this.resultConatiner.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.resultConatiner)).EndInit();
            this.resultConatiner.ResumeLayout(false);
            this.innerRightResultPanel.ResumeLayout(false);
            this.innerRightResultPanel.PerformLayout();
            this.tabControl2.ResumeLayout(false);
            this.tabPageJsonView.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer ParentContainer;
        private System.Windows.Forms.SplitContainer MainQueryContainer;
        private System.Windows.Forms.TreeView fileView;
        private System.Windows.Forms.SplitContainer resultConatiner;
        private System.Windows.Forms.ListView resultListView;
        private System.Windows.Forms.Panel innerRightResultPanel;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPageTextView;
        private System.Windows.Forms.TabPage tabPageJsonView;
        private System.Windows.Forms.TreeView treeViewJSON;
        private System.Windows.Forms.MenuStrip resultMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewCosmosConnectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelQueryRunning;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBarQueryRunning;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabQuery1;
        private System.Windows.Forms.RichTextBox query1Textbox;
        private System.Windows.Forms.ToolStrip query1ToolStrip;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

