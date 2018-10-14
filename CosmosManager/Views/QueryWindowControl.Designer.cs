namespace CosmosManager
{
    partial class QueryWindowControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryWindowControl));
            this.splitQueryResult = new System.Windows.Forms.SplitContainer();
            this.textQuery = new System.Windows.Forms.RichTextBox();
            this.queryStatusBar = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.decreaseFontButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.selectConnections = new System.Windows.Forms.ComboBox();
            this.wordWrapToggleButton = new System.Windows.Forms.Button();
            this.runQueryButton = new System.Windows.Forms.Button();
            this.increaseFontButton = new System.Windows.Forms.Button();
            this.saveQueryButton = new System.Windows.Forms.Button();
            this.tabControlQueryOutput = new System.Windows.Forms.TabControl();
            this.tabResultsPage = new System.Windows.Forms.TabPage();
            this.splitResultView = new System.Windows.Forms.SplitContainer();
            this.resultListView = new System.Windows.Forms.ListView();
            this.columnSelect = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPartitionKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.selectedToUpdateButton = new System.Windows.Forms.ToolStripButton();
            this.selectedToDeleteButton = new System.Windows.Forms.ToolStripButton();
            this.textDocument = new System.Windows.Forms.RichTextBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.resultWordWrapButton = new System.Windows.Forms.ToolStripButton();
            this.resultFontSizeDecreaseButton = new System.Windows.Forms.ToolStripButton();
            this.resuleFontSizeIncreaseButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.saveRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteDocumentButton = new System.Windows.Forms.ToolStripButton();
            this.saveExistingDocument = new System.Windows.Forms.ToolStripButton();
            this.tabOuputPage = new System.Windows.Forms.TabPage();
            this.textStats = new System.Windows.Forms.RichTextBox();
            this.saveQueryDialog = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.saveJsonDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveTempQueryDialog = new System.Windows.Forms.SaveFileDialog();
            this.connectionBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitQueryResult)).BeginInit();
            this.splitQueryResult.Panel1.SuspendLayout();
            this.splitQueryResult.Panel2.SuspendLayout();
            this.splitQueryResult.SuspendLayout();
            this.queryStatusBar.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControlQueryOutput.SuspendLayout();
            this.tabResultsPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitResultView)).BeginInit();
            this.splitResultView.Panel1.SuspendLayout();
            this.splitResultView.Panel2.SuspendLayout();
            this.splitResultView.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.tabOuputPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.connectionBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // splitQueryResult
            // 
            this.splitQueryResult.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitQueryResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitQueryResult.Location = new System.Drawing.Point(0, 0);
            this.splitQueryResult.Name = "splitQueryResult";
            this.splitQueryResult.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitQueryResult.Panel1
            // 
            this.splitQueryResult.Panel1.Controls.Add(this.textQuery);
            this.splitQueryResult.Panel1.Controls.Add(this.queryStatusBar);
            // 
            // splitQueryResult.Panel2
            // 
            this.splitQueryResult.Panel2.Controls.Add(this.tabControlQueryOutput);
            this.splitQueryResult.Size = new System.Drawing.Size(930, 585);
            this.splitQueryResult.SplitterDistance = 221;
            this.splitQueryResult.TabIndex = 1;
            // 
            // textQuery
            // 
            this.textQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textQuery.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textQuery.Location = new System.Drawing.Point(0, 32);
            this.textQuery.Name = "textQuery";
            this.textQuery.Size = new System.Drawing.Size(930, 189);
            this.textQuery.TabIndex = 2;
            this.textQuery.Text = "";
            this.textQuery.WordWrap = false;
            // 
            // queryStatusBar
            // 
            this.queryStatusBar.Controls.Add(this.panel1);
            this.queryStatusBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.queryStatusBar.Location = new System.Drawing.Point(0, 0);
            this.queryStatusBar.Name = "queryStatusBar";
            this.queryStatusBar.Size = new System.Drawing.Size(930, 32);
            this.queryStatusBar.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.decreaseFontButton);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.wordWrapToggleButton);
            this.panel1.Controls.Add(this.runQueryButton);
            this.panel1.Controls.Add(this.increaseFontButton);
            this.panel1.Controls.Add(this.saveQueryButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(930, 32);
            this.panel1.TabIndex = 2;
            // 
            // decreaseFontButton
            // 
            this.decreaseFontButton.AutoSize = true;
            this.decreaseFontButton.BackColor = System.Drawing.SystemColors.Control;
            this.decreaseFontButton.FlatAppearance.BorderSize = 0;
            this.decreaseFontButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.decreaseFontButton.Image = global::CosmosManager.Properties.Resources.gb_font_smaller_d;
            this.decreaseFontButton.Location = new System.Drawing.Point(30, 4);
            this.decreaseFontButton.Name = "decreaseFontButton";
            this.decreaseFontButton.Size = new System.Drawing.Size(45, 26);
            this.decreaseFontButton.TabIndex = 3;
            this.decreaseFontButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.decreaseFontButton, "Decrease Font Size");
            this.decreaseFontButton.UseVisualStyleBackColor = false;
            this.decreaseFontButton.Click += new System.EventHandler(this.decreaseFontButton_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.selectConnections);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(653, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(277, 32);
            this.panel2.TabIndex = 2;
            // 
            // selectConnections
            // 
            this.selectConnections.DisplayMember = "Name";
            this.selectConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectConnections.FormattingEnabled = true;
            this.selectConnections.Location = new System.Drawing.Point(6, 6);
            this.selectConnections.Name = "selectConnections";
            this.selectConnections.Size = new System.Drawing.Size(262, 21);
            this.selectConnections.TabIndex = 0;
            this.selectConnections.SelectedValueChanged += new System.EventHandler(this.selectConnections_SelectedValueChanged);
            // 
            // wordWrapToggleButton
            // 
            this.wordWrapToggleButton.BackColor = System.Drawing.SystemColors.Control;
            this.wordWrapToggleButton.FlatAppearance.BorderSize = 0;
            this.wordWrapToggleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.wordWrapToggleButton.Image = global::CosmosManager.Properties.Resources.arrow_undo;
            this.wordWrapToggleButton.Location = new System.Drawing.Point(3, 4);
            this.wordWrapToggleButton.Name = "wordWrapToggleButton";
            this.wordWrapToggleButton.Size = new System.Drawing.Size(30, 25);
            this.wordWrapToggleButton.TabIndex = 5;
            this.wordWrapToggleButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.wordWrapToggleButton, "Toggle Word Wrap");
            this.wordWrapToggleButton.UseVisualStyleBackColor = false;
            this.wordWrapToggleButton.Click += new System.EventHandler(this.wordWrapToggleButton_Click);
            // 
            // runQueryButton
            // 
            this.runQueryButton.BackColor = System.Drawing.SystemColors.Control;
            this.runQueryButton.FlatAppearance.BorderSize = 0;
            this.runQueryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.runQueryButton.Image = ((System.Drawing.Image)(resources.GetObject("runQueryButton.Image")));
            this.runQueryButton.Location = new System.Drawing.Point(138, 4);
            this.runQueryButton.Name = "runQueryButton";
            this.runQueryButton.Size = new System.Drawing.Size(27, 25);
            this.runQueryButton.TabIndex = 1;
            this.runQueryButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.runQueryButton, "Run Query");
            this.runQueryButton.UseVisualStyleBackColor = false;
            this.runQueryButton.Click += new System.EventHandler(this.runQueryButton_Click_1);
            // 
            // increaseFontButton
            // 
            this.increaseFontButton.AutoSize = true;
            this.increaseFontButton.BackColor = System.Drawing.SystemColors.Control;
            this.increaseFontButton.FlatAppearance.BorderSize = 0;
            this.increaseFontButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.increaseFontButton.Image = global::CosmosManager.Properties.Resources.gb_font_larger_d;
            this.increaseFontButton.Location = new System.Drawing.Point(72, 4);
            this.increaseFontButton.Name = "increaseFontButton";
            this.increaseFontButton.Size = new System.Drawing.Size(47, 26);
            this.increaseFontButton.TabIndex = 4;
            this.increaseFontButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.increaseFontButton, "Increase Font Size");
            this.increaseFontButton.UseVisualStyleBackColor = false;
            this.increaseFontButton.Click += new System.EventHandler(this.increaseFontButton_Click);
            // 
            // saveQueryButton
            // 
            this.saveQueryButton.BackColor = System.Drawing.SystemColors.Control;
            this.saveQueryButton.FlatAppearance.BorderSize = 0;
            this.saveQueryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveQueryButton.Image = ((System.Drawing.Image)(resources.GetObject("saveQueryButton.Image")));
            this.saveQueryButton.Location = new System.Drawing.Point(114, 4);
            this.saveQueryButton.Name = "saveQueryButton";
            this.saveQueryButton.Size = new System.Drawing.Size(30, 25);
            this.saveQueryButton.TabIndex = 2;
            this.saveQueryButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.saveQueryButton, "Save Query");
            this.saveQueryButton.UseVisualStyleBackColor = false;
            this.saveQueryButton.Click += new System.EventHandler(this.saveQueryButton_Click);
            // 
            // tabControlQueryOutput
            // 
            this.tabControlQueryOutput.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControlQueryOutput.Controls.Add(this.tabResultsPage);
            this.tabControlQueryOutput.Controls.Add(this.tabOuputPage);
            this.tabControlQueryOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlQueryOutput.Location = new System.Drawing.Point(0, 0);
            this.tabControlQueryOutput.Name = "tabControlQueryOutput";
            this.tabControlQueryOutput.SelectedIndex = 0;
            this.tabControlQueryOutput.Size = new System.Drawing.Size(930, 360);
            this.tabControlQueryOutput.TabIndex = 1;
            // 
            // tabResultsPage
            // 
            this.tabResultsPage.Controls.Add(this.splitResultView);
            this.tabResultsPage.Location = new System.Drawing.Point(4, 4);
            this.tabResultsPage.Name = "tabResultsPage";
            this.tabResultsPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabResultsPage.Size = new System.Drawing.Size(922, 334);
            this.tabResultsPage.TabIndex = 0;
            this.tabResultsPage.Text = "Results";
            this.tabResultsPage.UseVisualStyleBackColor = true;
            // 
            // splitResultView
            // 
            this.splitResultView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitResultView.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitResultView.Location = new System.Drawing.Point(3, 3);
            this.splitResultView.Name = "splitResultView";
            // 
            // splitResultView.Panel1
            // 
            this.splitResultView.Panel1.Controls.Add(this.resultListView);
            this.splitResultView.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitResultView.Panel2
            // 
            this.splitResultView.Panel2.Controls.Add(this.textDocument);
            this.splitResultView.Panel2.Controls.Add(this.toolStrip2);
            this.splitResultView.Size = new System.Drawing.Size(916, 328);
            this.splitResultView.SplitterDistance = 350;
            this.splitResultView.TabIndex = 0;
            // 
            // resultListView
            // 
            this.resultListView.CheckBoxes = true;
            this.resultListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnSelect,
            this.columnId,
            this.columnPartitionKey});
            this.resultListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultListView.FullRowSelect = true;
            this.resultListView.GridLines = true;
            this.resultListView.HideSelection = false;
            this.resultListView.Location = new System.Drawing.Point(0, 25);
            this.resultListView.Name = "resultListView";
            this.resultListView.OwnerDraw = true;
            this.resultListView.Size = new System.Drawing.Size(350, 303);
            this.resultListView.TabIndex = 0;
            this.resultListView.UseCompatibleStateImageBehavior = false;
            this.resultListView.View = System.Windows.Forms.View.Details;
            this.resultListView.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.resultListView_DrawColumnHeader);
            this.resultListView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.resultListView_DrawItem);
            this.resultListView.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.resultListView_DrawSubItem);
            this.resultListView.SelectedIndexChanged += new System.EventHandler(this.resultListView_SelectedIndexChanged);
            // 
            // columnSelect
            // 
            this.columnSelect.Text = "";
            this.columnSelect.Width = 20;
            // 
            // columnId
            // 
            this.columnId.Text = "id";
            this.columnId.Width = 136;
            // 
            // columnPartitionKey
            // 
            this.columnPartitionKey.Text = "PartitionKey";
            this.columnPartitionKey.Width = 166;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectedToUpdateButton,
            this.selectedToDeleteButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(350, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // selectedToUpdateButton
            // 
            this.selectedToUpdateButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectedToUpdateButton.Image = global::CosmosManager.Properties.Resources.file_document_sync_synchronization_reload_refresh_update_128;
            this.selectedToUpdateButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectedToUpdateButton.Name = "selectedToUpdateButton";
            this.selectedToUpdateButton.Size = new System.Drawing.Size(23, 22);
            this.selectedToUpdateButton.Text = "Selected to Update Query";
            this.selectedToUpdateButton.ToolTipText = "Selected to Update Query";
            this.selectedToUpdateButton.Visible = false;
            this.selectedToUpdateButton.Click += new System.EventHandler(this.selectedToUpdateButton_Click);
            // 
            // selectedToDeleteButton
            // 
            this.selectedToDeleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectedToDeleteButton.Image = global::CosmosManager.Properties.Resources._776907_document_512x512;
            this.selectedToDeleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectedToDeleteButton.Name = "selectedToDeleteButton";
            this.selectedToDeleteButton.Size = new System.Drawing.Size(23, 22);
            this.selectedToDeleteButton.Text = "Selected to Delete Query";
            this.selectedToDeleteButton.Click += new System.EventHandler(this.selectedToDeleteButton_Click);
            // 
            // textDocument
            // 
            this.textDocument.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textDocument.Location = new System.Drawing.Point(0, 25);
            this.textDocument.Name = "textDocument";
            this.textDocument.Size = new System.Drawing.Size(562, 303);
            this.textDocument.TabIndex = 1;
            this.textDocument.Text = "";
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resultWordWrapButton,
            this.resultFontSizeDecreaseButton,
            this.resuleFontSizeIncreaseButton,
            this.toolStripDropDownButton1,
            this.deleteDocumentButton,
            this.saveExistingDocument});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(562, 25);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // resultWordWrapButton
            // 
            this.resultWordWrapButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resultWordWrapButton.Image = global::CosmosManager.Properties.Resources.arrow_undo;
            this.resultWordWrapButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resultWordWrapButton.Name = "resultWordWrapButton";
            this.resultWordWrapButton.Size = new System.Drawing.Size(23, 22);
            this.resultWordWrapButton.Text = "toolStripButton1";
            this.resultWordWrapButton.ToolTipText = "Toggle Word Wrap";
            this.resultWordWrapButton.Click += new System.EventHandler(this.resultWordWrapButton_Click);
            // 
            // resultFontSizeDecreaseButton
            // 
            this.resultFontSizeDecreaseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resultFontSizeDecreaseButton.Image = global::CosmosManager.Properties.Resources.gb_font_smaller_d;
            this.resultFontSizeDecreaseButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.resultFontSizeDecreaseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resultFontSizeDecreaseButton.Name = "resultFontSizeDecreaseButton";
            this.resultFontSizeDecreaseButton.Size = new System.Drawing.Size(43, 22);
            this.resultFontSizeDecreaseButton.Text = "Decrease Font Size";
            this.resultFontSizeDecreaseButton.Click += new System.EventHandler(this.resultFontSizeDecreaseButton_Click);
            // 
            // resuleFontSizeIncreaseButton
            // 
            this.resuleFontSizeIncreaseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resuleFontSizeIncreaseButton.Image = global::CosmosManager.Properties.Resources.gb_font_larger_d;
            this.resuleFontSizeIncreaseButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.resuleFontSizeIncreaseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resuleFontSizeIncreaseButton.Name = "resuleFontSizeIncreaseButton";
            this.resuleFontSizeIncreaseButton.Size = new System.Drawing.Size(43, 22);
            this.resuleFontSizeIncreaseButton.Text = "Increase Font Size";
            this.resuleFontSizeIncreaseButton.Click += new System.EventHandler(this.resuleFontSizeIncreaseButton_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveRecordToolStripMenuItem,
            this.saveAllResultsToolStripMenuItem});
            this.toolStripDropDownButton1.Image = global::CosmosManager.Properties.Resources.move_waiting_down_alternative;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(29, 22);
            this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.ToolTipText = "Save Options";
            // 
            // saveRecordToolStripMenuItem
            // 
            this.saveRecordToolStripMenuItem.Name = "saveRecordToolStripMenuItem";
            this.saveRecordToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.saveRecordToolStripMenuItem.Text = "Export Document";
            this.saveRecordToolStripMenuItem.Click += new System.EventHandler(this.exportRecordToolStripMenuItem_Click);
            // 
            // saveAllResultsToolStripMenuItem
            // 
            this.saveAllResultsToolStripMenuItem.Name = "saveAllResultsToolStripMenuItem";
            this.saveAllResultsToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.saveAllResultsToolStripMenuItem.Text = "Export All Documents";
            this.saveAllResultsToolStripMenuItem.Click += new System.EventHandler(this.exportAllResultsToolStripMenuItem_Click);
            // 
            // deleteDocumentButton
            // 
            this.deleteDocumentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteDocumentButton.Image = global::CosmosManager.Properties.Resources.page_delete;
            this.deleteDocumentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteDocumentButton.Name = "deleteDocumentButton";
            this.deleteDocumentButton.Size = new System.Drawing.Size(23, 22);
            this.deleteDocumentButton.Text = "toolStripButton1";
            this.deleteDocumentButton.Click += new System.EventHandler(this.deleteDocumentButton_Click);
            // 
            // saveExistingDocument
            // 
            this.saveExistingDocument.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveExistingDocument.Image = global::CosmosManager.Properties.Resources.gbl_Save;
            this.saveExistingDocument.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveExistingDocument.Name = "saveExistingDocument";
            this.saveExistingDocument.Size = new System.Drawing.Size(23, 22);
            this.saveExistingDocument.Text = "toolStripButton1";
            this.saveExistingDocument.Click += new System.EventHandler(this.saveExistingDocument_Click);
            // 
            // tabOuputPage
            // 
            this.tabOuputPage.Controls.Add(this.textStats);
            this.tabOuputPage.Location = new System.Drawing.Point(4, 4);
            this.tabOuputPage.Name = "tabOuputPage";
            this.tabOuputPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabOuputPage.Size = new System.Drawing.Size(922, 334);
            this.tabOuputPage.TabIndex = 1;
            this.tabOuputPage.Text = "Output";
            this.tabOuputPage.UseVisualStyleBackColor = true;
            // 
            // textStats
            // 
            this.textStats.BackColor = System.Drawing.SystemColors.ControlLight;
            this.textStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textStats.Location = new System.Drawing.Point(3, 3);
            this.textStats.Name = "textStats";
            this.textStats.ReadOnly = true;
            this.textStats.Size = new System.Drawing.Size(916, 328);
            this.textStats.TabIndex = 1;
            this.textStats.Text = "";
            this.textStats.WordWrap = false;
            // 
            // saveJsonDialog
            // 
            this.saveJsonDialog.DefaultExt = "json";
            this.saveJsonDialog.Filter = "Json|*.json";
            this.saveJsonDialog.SupportMultiDottedExtensions = true;
            // 
            // saveTempQueryDialog
            // 
            this.saveTempQueryDialog.DefaultExt = "csql";
            this.saveTempQueryDialog.Filter = "Cosmos Script|*.csql";
            this.saveTempQueryDialog.SupportMultiDottedExtensions = true;
            this.saveTempQueryDialog.Title = "Save New Query";
            // 
            // connectionBindingSource
            // 
            this.connectionBindingSource.DataSource = typeof(CosmosManager.Domain.Connection);
            // 
            // QueryWindowControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitQueryResult);
            this.Name = "QueryWindowControl";
            this.Size = new System.Drawing.Size(930, 585);
            this.splitQueryResult.Panel1.ResumeLayout(false);
            this.splitQueryResult.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitQueryResult)).EndInit();
            this.splitQueryResult.ResumeLayout(false);
            this.queryStatusBar.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tabControlQueryOutput.ResumeLayout(false);
            this.tabResultsPage.ResumeLayout(false);
            this.splitResultView.Panel1.ResumeLayout(false);
            this.splitResultView.Panel1.PerformLayout();
            this.splitResultView.Panel2.ResumeLayout(false);
            this.splitResultView.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitResultView)).EndInit();
            this.splitResultView.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tabOuputPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.connectionBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitQueryResult;
        private System.Windows.Forms.SplitContainer splitResultView;
        private System.Windows.Forms.ListView resultListView;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.SaveFileDialog saveQueryDialog;
        private System.Windows.Forms.Panel queryStatusBar;
        private System.Windows.Forms.ComboBox selectConnections;
        private System.Windows.Forms.Button runQueryButton;
        private System.Windows.Forms.BindingSource connectionBindingSource;
        private System.Windows.Forms.ColumnHeader columnSelect;
        private System.Windows.Forms.ColumnHeader columnId;
        private System.Windows.Forms.ColumnHeader columnPartitionKey;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem saveRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAllResultsToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton selectedToUpdateButton;
        private System.Windows.Forms.ToolStripButton selectedToDeleteButton;
        private System.Windows.Forms.Button saveQueryButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.SaveFileDialog saveJsonDialog;
        private System.Windows.Forms.SaveFileDialog saveTempQueryDialog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button wordWrapToggleButton;
        private System.Windows.Forms.Button increaseFontButton;
        private System.Windows.Forms.Button decreaseFontButton;
        private System.Windows.Forms.ToolStripButton resultWordWrapButton;
        private System.Windows.Forms.ToolStripButton resultFontSizeDecreaseButton;
        private System.Windows.Forms.ToolStripButton resuleFontSizeIncreaseButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripButton saveExistingDocument;
        private System.Windows.Forms.RichTextBox textDocument;
        private System.Windows.Forms.ToolStripButton deleteDocumentButton;
        private System.Windows.Forms.TabControl tabControlQueryOutput;
        private System.Windows.Forms.TabPage tabResultsPage;
        private System.Windows.Forms.TabPage tabOuputPage;
        private System.Windows.Forms.RichTextBox textQuery;
        private System.Windows.Forms.RichTextBox textStats;
    }
}
