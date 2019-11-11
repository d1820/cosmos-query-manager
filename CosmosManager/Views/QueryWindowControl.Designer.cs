using System.Windows.Forms;

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
            this.textQuery = new ScintillaNET.Scintilla();
            this.queryStatusBar = new System.Windows.Forms.Panel();
            this.queryToolStrip = new System.Windows.Forms.ToolStrip();
            this.beautifyQueryButton = new System.Windows.Forms.ToolStripButton();
            this.lowercaseTextButton = new System.Windows.Forms.ToolStripButton();
            this.textUppercaseButon = new System.Windows.Forms.ToolStripButton();
            this.textIndentButton = new System.Windows.Forms.ToolStripButton();
            this.textOutdentButton = new System.Windows.Forms.ToolStripButton();
            this.wordWrapToggleButton = new System.Windows.Forms.ToolStripButton();
            this.decreaseFontButton = new System.Windows.Forms.ToolStripButton();
            this.increaseFontButton = new System.Windows.Forms.ToolStripButton();
            this.saveQueryButton = new System.Windows.Forms.ToolStripButton();
            this.runQueryButton = new System.Windows.Forms.ToolStripButton();
            this.selectConnections = new System.Windows.Forms.ToolStripComboBox();
            this.stopQueryButton = new System.Windows.Forms.ToolStripButton();
            this.tabControlQueryOutput = new System.Windows.Forms.TabControl();
            this.tabResultsPage = new System.Windows.Forms.TabPage();
            this.splitResultView = new System.Windows.Forms.SplitContainer();
            this.resultListView = new System.Windows.Forms.ListView();
            this.columnSelect = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPartitionKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.resultListToolStrip = new System.Windows.Forms.ToolStrip();
            this.selectedToUpdateButton = new System.Windows.Forms.ToolStripButton();
            this.selectedToDeleteButton = new System.Windows.Forms.ToolStripButton();
            this.resultCountTextbox = new System.Windows.Forms.ToolStripTextBox();
            this.textDocument = new ScintillaNET.Scintilla();
            this.documentToolstrip = new System.Windows.Forms.ToolStrip();
            this.beautifyResultDocumentButton = new System.Windows.Forms.ToolStripButton();
            this.resultLowercaseButton = new System.Windows.Forms.ToolStripButton();
            this.resultUppercaseButton = new System.Windows.Forms.ToolStripButton();
            this.resultIndentButton = new System.Windows.Forms.ToolStripButton();
            this.resultOutdentButton = new System.Windows.Forms.ToolStripButton();
            this.resultWordWrapButton = new System.Windows.Forms.ToolStripButton();
            this.resultFontSizeDecreaseButton = new System.Windows.Forms.ToolStripButton();
            this.resuleFontSizeIncreaseButton = new System.Windows.Forms.ToolStripButton();
            this.deleteDocumentButton = new System.Windows.Forms.ToolStripButton();
            this.saveExistingDocument = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.saveRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabOuputPage = new System.Windows.Forms.TabPage();
            this.textQueryOutput = new System.Windows.Forms.RichTextBox();
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
            this.queryToolStrip.SuspendLayout();
            this.tabControlQueryOutput.SuspendLayout();
            this.tabResultsPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitResultView)).BeginInit();
            this.splitResultView.Panel1.SuspendLayout();
            this.splitResultView.Panel2.SuspendLayout();
            this.splitResultView.SuspendLayout();
            this.resultListToolStrip.SuspendLayout();
            this.documentToolstrip.SuspendLayout();
            this.tabOuputPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.connectionBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // splitQueryResult
            // 
            this.splitQueryResult.BackColor = System.Drawing.SystemColors.Control;
            this.splitQueryResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitQueryResult.Location = new System.Drawing.Point(0, 0);
            this.splitQueryResult.Name = "splitQueryResult";
            this.splitQueryResult.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitQueryResult.Panel1
            // 
            this.splitQueryResult.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitQueryResult.Panel1.Controls.Add(this.textQuery);
            this.splitQueryResult.Panel1.Controls.Add(this.queryStatusBar);
            this.splitQueryResult.Panel1MinSize = 200;
            // 
            // splitQueryResult.Panel2
            // 
            this.splitQueryResult.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitQueryResult.Panel2.Controls.Add(this.tabControlQueryOutput);
            this.splitQueryResult.Size = new System.Drawing.Size(930, 585);
            this.splitQueryResult.SplitterDistance = 221;
            this.splitQueryResult.TabIndex = 1;
            // 
            // textQuery
            // 
            this.textQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textQuery.Location = new System.Drawing.Point(0, 28);
            this.textQuery.Name = "textQuery";
            this.textQuery.Size = new System.Drawing.Size(930, 193);
            this.textQuery.TabIndex = 3;
            // 
            // queryStatusBar
            // 
            this.queryStatusBar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.queryStatusBar.Controls.Add(this.queryToolStrip);
            this.queryStatusBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.queryStatusBar.Location = new System.Drawing.Point(0, 0);
            this.queryStatusBar.Name = "queryStatusBar";
            this.queryStatusBar.Size = new System.Drawing.Size(930, 28);
            this.queryStatusBar.TabIndex = 1;
            // 
            // queryToolStrip
            // 
            this.queryToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.queryToolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.queryToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.beautifyQueryButton,
            this.lowercaseTextButton,
            this.textUppercaseButon,
            this.textIndentButton,
            this.textOutdentButton,
            this.wordWrapToggleButton,
            this.decreaseFontButton,
            this.increaseFontButton,
            this.saveQueryButton,
            this.runQueryButton,
            this.selectConnections,
            this.stopQueryButton});
            this.queryToolStrip.Location = new System.Drawing.Point(0, 0);
            this.queryToolStrip.Name = "queryToolStrip";
            this.queryToolStrip.Size = new System.Drawing.Size(930, 27);
            this.queryToolStrip.TabIndex = 3;
            this.queryToolStrip.Text = "queryToolstrip";
            // 
            // beautifyQueryButton
            // 
            this.beautifyQueryButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.beautifyQueryButton.Image = global::CosmosManager.Properties.Resources.json;
            this.beautifyQueryButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.beautifyQueryButton.Name = "beautifyQueryButton";
            this.beautifyQueryButton.Size = new System.Drawing.Size(24, 24);
            this.beautifyQueryButton.ToolTipText = "Beautify Query";
            this.beautifyQueryButton.Click += new System.EventHandler(this.beautifyQueryButton_Click);
            // 
            // lowercaseTextButton
            // 
            this.lowercaseTextButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.lowercaseTextButton.Image = global::CosmosManager.Properties.Resources.text_lowercase;
            this.lowercaseTextButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.lowercaseTextButton.Name = "lowercaseTextButton";
            this.lowercaseTextButton.Size = new System.Drawing.Size(24, 24);
            this.lowercaseTextButton.Text = "To Lowercase";
            this.lowercaseTextButton.Click += new System.EventHandler(this.lowercaseTextButton_Click);
            // 
            // textUppercaseButon
            // 
            this.textUppercaseButon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.textUppercaseButon.Image = global::CosmosManager.Properties.Resources.text_uppercase;
            this.textUppercaseButon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.textUppercaseButon.Name = "textUppercaseButon";
            this.textUppercaseButon.Size = new System.Drawing.Size(24, 24);
            this.textUppercaseButon.Text = "To Uppercase";
            this.textUppercaseButon.Click += new System.EventHandler(this.textUppercaseButon_Click);
            // 
            // textIndentButton
            // 
            this.textIndentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.textIndentButton.Image = global::CosmosManager.Properties.Resources.text_indent;
            this.textIndentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.textIndentButton.Name = "textIndentButton";
            this.textIndentButton.Size = new System.Drawing.Size(24, 24);
            this.textIndentButton.Text = "Indent";
            this.textIndentButton.Click += new System.EventHandler(this.textIndentButton_Click);
            // 
            // textOutdentButton
            // 
            this.textOutdentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.textOutdentButton.Image = global::CosmosManager.Properties.Resources.text_indent_remove;
            this.textOutdentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.textOutdentButton.Name = "textOutdentButton";
            this.textOutdentButton.Size = new System.Drawing.Size(24, 24);
            this.textOutdentButton.Text = "Outdent";
            this.textOutdentButton.Click += new System.EventHandler(this.textOutdentButton_Click);
            // 
            // wordWrapToggleButton
            // 
            this.wordWrapToggleButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.wordWrapToggleButton.Image = global::CosmosManager.Properties.Resources.arrow_undo;
            this.wordWrapToggleButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.wordWrapToggleButton.Name = "wordWrapToggleButton";
            this.wordWrapToggleButton.Size = new System.Drawing.Size(24, 24);
            this.wordWrapToggleButton.ToolTipText = "Toggle Word Wrap";
            this.wordWrapToggleButton.Click += new System.EventHandler(this.wordWrapToggleButton_Click);
            // 
            // decreaseFontButton
            // 
            this.decreaseFontButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.decreaseFontButton.Image = global::CosmosManager.Properties.Resources.format_font_size_less;
            this.decreaseFontButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.decreaseFontButton.Name = "decreaseFontButton";
            this.decreaseFontButton.Size = new System.Drawing.Size(24, 24);
            this.decreaseFontButton.ToolTipText = "Decrease Font Size";
            this.decreaseFontButton.Click += new System.EventHandler(this.decreaseFontButton_Click);
            // 
            // increaseFontButton
            // 
            this.increaseFontButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.increaseFontButton.Image = global::CosmosManager.Properties.Resources.format_font_size_more;
            this.increaseFontButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.increaseFontButton.Name = "increaseFontButton";
            this.increaseFontButton.Size = new System.Drawing.Size(24, 24);
            this.increaseFontButton.ToolTipText = "Increase Font Size";
            this.increaseFontButton.Click += new System.EventHandler(this.increaseFontButton_Click);
            // 
            // saveQueryButton
            // 
            this.saveQueryButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveQueryButton.Image = global::CosmosManager.Properties.Resources._3floppy_unmount_icon;
            this.saveQueryButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveQueryButton.Name = "saveQueryButton";
            this.saveQueryButton.Size = new System.Drawing.Size(24, 24);
            this.saveQueryButton.ToolTipText = "Save Query";
            this.saveQueryButton.Click += new System.EventHandler(this.saveQueryButton_Click);
            // 
            // runQueryButton
            // 
            this.runQueryButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.runQueryButton.Image = global::CosmosManager.Properties.Resources.Start_icon;
            this.runQueryButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.runQueryButton.Name = "runQueryButton";
            this.runQueryButton.Size = new System.Drawing.Size(24, 24);
            this.runQueryButton.ToolTipText = "Run Query";
            this.runQueryButton.Click += new System.EventHandler(this.runQueryButton_Click_1);
            // 
            // selectConnections
            // 
            this.selectConnections.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.selectConnections.AutoSize = false;
            this.selectConnections.BackColor = System.Drawing.SystemColors.Control;
            this.selectConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectConnections.MaxDropDownItems = 10;
            this.selectConnections.Name = "selectConnections";
            this.selectConnections.Size = new System.Drawing.Size(250, 23);
            this.selectConnections.SelectedIndexChanged += new System.EventHandler(this.selectConnections_SelectedValueChanged);
            // 
            // stopQueryButton
            // 
            this.stopQueryButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopQueryButton.Image = global::CosmosManager.Properties.Resources.Stop_red_icon;
            this.stopQueryButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopQueryButton.Name = "stopQueryButton";
            this.stopQueryButton.Size = new System.Drawing.Size(24, 24);
            this.stopQueryButton.Text = "toolStripButton1";
            this.stopQueryButton.Visible = false;
            this.stopQueryButton.Click += new System.EventHandler(this.stopQueryButton_Click);
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
            this.splitResultView.Panel1.Controls.Add(this.resultListToolStrip);
            // 
            // splitResultView.Panel2
            // 
            this.splitResultView.Panel2.Controls.Add(this.textDocument);
            this.splitResultView.Panel2.Controls.Add(this.documentToolstrip);
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
            this.resultListView.Location = new System.Drawing.Point(0, 27);
            this.resultListView.MultiSelect = false;
            this.resultListView.Name = "resultListView";
            this.resultListView.OwnerDraw = true;
            this.resultListView.Size = new System.Drawing.Size(350, 301);
            this.resultListView.TabIndex = 0;
            this.resultListView.TabStop = false;
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
            this.columnId.Text = "";
            this.columnId.Width = 136;
            // 
            // columnPartitionKey
            // 
            this.columnPartitionKey.Text = "";
            this.columnPartitionKey.Width = 166;
            // 
            // resultListToolStrip
            // 
            this.resultListToolStrip.BackColor = System.Drawing.SystemColors.Control;
            this.resultListToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.resultListToolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.resultListToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectedToUpdateButton,
            this.selectedToDeleteButton,
            this.resultCountTextbox});
            this.resultListToolStrip.Location = new System.Drawing.Point(0, 0);
            this.resultListToolStrip.Name = "resultListToolStrip";
            this.resultListToolStrip.Size = new System.Drawing.Size(350, 27);
            this.resultListToolStrip.TabIndex = 1;
            this.resultListToolStrip.Text = "resultListToolstrip";
            // 
            // selectedToUpdateButton
            // 
            this.selectedToUpdateButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectedToUpdateButton.Image = ((System.Drawing.Image)(resources.GetObject("selectedToUpdateButton.Image")));
            this.selectedToUpdateButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectedToUpdateButton.Name = "selectedToUpdateButton";
            this.selectedToUpdateButton.Size = new System.Drawing.Size(24, 24);
            this.selectedToUpdateButton.Text = "Selected to Update Query";
            this.selectedToUpdateButton.ToolTipText = "Selected to Update Query";
            this.selectedToUpdateButton.Click += new System.EventHandler(this.selectedToUpdateButton_Click);
            // 
            // selectedToDeleteButton
            // 
            this.selectedToDeleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectedToDeleteButton.Image = global::CosmosManager.Properties.Resources.documents_delete;
            this.selectedToDeleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectedToDeleteButton.Name = "selectedToDeleteButton";
            this.selectedToDeleteButton.Size = new System.Drawing.Size(24, 24);
            this.selectedToDeleteButton.Text = "Selected to Delete Query";
            this.selectedToDeleteButton.Click += new System.EventHandler(this.selectedToDeleteButton_Click);
            // 
            // resultCountTextbox
            // 
            this.resultCountTextbox.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.resultCountTextbox.BackColor = System.Drawing.SystemColors.Control;
            this.resultCountTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.resultCountTextbox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.resultCountTextbox.Name = "resultCountTextbox";
            this.resultCountTextbox.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.resultCountTextbox.ReadOnly = true;
            this.resultCountTextbox.Size = new System.Drawing.Size(85, 27);
            this.resultCountTextbox.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textDocument
            // 
            this.textDocument.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textDocument.Location = new System.Drawing.Point(0, 27);
            this.textDocument.Name = "textDocument";
            this.textDocument.Size = new System.Drawing.Size(562, 301);
            this.textDocument.TabIndex = 4;
            // 
            // documentToolstrip
            // 
            this.documentToolstrip.BackColor = System.Drawing.SystemColors.Control;
            this.documentToolstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.documentToolstrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.documentToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.beautifyResultDocumentButton,
            this.resultLowercaseButton,
            this.resultUppercaseButton,
            this.resultIndentButton,
            this.resultOutdentButton,
            this.resultWordWrapButton,
            this.resultFontSizeDecreaseButton,
            this.resuleFontSizeIncreaseButton,
            this.deleteDocumentButton,
            this.saveExistingDocument,
            this.toolStripDropDownButton1});
            this.documentToolstrip.Location = new System.Drawing.Point(0, 0);
            this.documentToolstrip.Name = "documentToolstrip";
            this.documentToolstrip.Size = new System.Drawing.Size(562, 27);
            this.documentToolstrip.TabIndex = 0;
            this.documentToolstrip.Text = "documentToolstrip";
            // 
            // beautifyResultDocumentButton
            // 
            this.beautifyResultDocumentButton.BackColor = System.Drawing.Color.Transparent;
            this.beautifyResultDocumentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.beautifyResultDocumentButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.beautifyResultDocumentButton.Image = global::CosmosManager.Properties.Resources.json;
            this.beautifyResultDocumentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.beautifyResultDocumentButton.Name = "beautifyResultDocumentButton";
            this.beautifyResultDocumentButton.Size = new System.Drawing.Size(24, 24);
            this.beautifyResultDocumentButton.ToolTipText = "Beautify Document";
            this.beautifyResultDocumentButton.Click += new System.EventHandler(this.beautifyResultDocumentButton_Click);
            // 
            // resultLowercaseButton
            // 
            this.resultLowercaseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resultLowercaseButton.Image = global::CosmosManager.Properties.Resources.text_lowercase;
            this.resultLowercaseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resultLowercaseButton.Name = "resultLowercaseButton";
            this.resultLowercaseButton.Size = new System.Drawing.Size(24, 24);
            this.resultLowercaseButton.Text = "To Lowercase";
            this.resultLowercaseButton.Click += new System.EventHandler(this.resultLowercaseButton_Click);
            // 
            // resultUppercaseButton
            // 
            this.resultUppercaseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resultUppercaseButton.Image = global::CosmosManager.Properties.Resources.text_uppercase;
            this.resultUppercaseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resultUppercaseButton.Name = "resultUppercaseButton";
            this.resultUppercaseButton.Size = new System.Drawing.Size(24, 24);
            this.resultUppercaseButton.Text = "To Uppercase";
            this.resultUppercaseButton.Click += new System.EventHandler(this.resultUppercaseButton_Click);
            // 
            // resultIndentButton
            // 
            this.resultIndentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resultIndentButton.Image = global::CosmosManager.Properties.Resources.text_indent;
            this.resultIndentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resultIndentButton.Name = "resultIndentButton";
            this.resultIndentButton.Size = new System.Drawing.Size(24, 24);
            this.resultIndentButton.Text = "Indent";
            this.resultIndentButton.Click += new System.EventHandler(this.resultIndentButton_Click);
            // 
            // resultOutdentButton
            // 
            this.resultOutdentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resultOutdentButton.Image = global::CosmosManager.Properties.Resources.text_indent_remove;
            this.resultOutdentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resultOutdentButton.Name = "resultOutdentButton";
            this.resultOutdentButton.Size = new System.Drawing.Size(24, 24);
            this.resultOutdentButton.Text = "Outdent";
            this.resultOutdentButton.Click += new System.EventHandler(this.resultOutdentButton_Click);
            // 
            // resultWordWrapButton
            // 
            this.resultWordWrapButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resultWordWrapButton.Image = global::CosmosManager.Properties.Resources.arrow_undo;
            this.resultWordWrapButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resultWordWrapButton.Name = "resultWordWrapButton";
            this.resultWordWrapButton.Size = new System.Drawing.Size(24, 24);
            this.resultWordWrapButton.Text = "toolStripButton1";
            this.resultWordWrapButton.ToolTipText = "Toggle Word Wrap";
            this.resultWordWrapButton.Click += new System.EventHandler(this.resultWordWrapButton_Click);
            // 
            // resultFontSizeDecreaseButton
            // 
            this.resultFontSizeDecreaseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resultFontSizeDecreaseButton.Image = global::CosmosManager.Properties.Resources.format_font_size_less;
            this.resultFontSizeDecreaseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resultFontSizeDecreaseButton.Name = "resultFontSizeDecreaseButton";
            this.resultFontSizeDecreaseButton.Size = new System.Drawing.Size(24, 24);
            this.resultFontSizeDecreaseButton.Text = "Decrease Font Size";
            this.resultFontSizeDecreaseButton.Click += new System.EventHandler(this.resultFontSizeDecreaseButton_Click);
            // 
            // resuleFontSizeIncreaseButton
            // 
            this.resuleFontSizeIncreaseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resuleFontSizeIncreaseButton.Image = global::CosmosManager.Properties.Resources.format_font_size_more;
            this.resuleFontSizeIncreaseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resuleFontSizeIncreaseButton.Name = "resuleFontSizeIncreaseButton";
            this.resuleFontSizeIncreaseButton.Size = new System.Drawing.Size(24, 24);
            this.resuleFontSizeIncreaseButton.Text = "Increase Font Size";
            this.resuleFontSizeIncreaseButton.Click += new System.EventHandler(this.resuleFontSizeIncreaseButton_Click);
            // 
            // deleteDocumentButton
            // 
            this.deleteDocumentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteDocumentButton.Image = global::CosmosManager.Properties.Resources.Delete_file_icon__1_;
            this.deleteDocumentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteDocumentButton.Name = "deleteDocumentButton";
            this.deleteDocumentButton.Size = new System.Drawing.Size(24, 24);
            this.deleteDocumentButton.ToolTipText = "Delete Document";
            this.deleteDocumentButton.Click += new System.EventHandler(this.deleteDocumentButton_Click);
            // 
            // saveExistingDocument
            // 
            this.saveExistingDocument.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveExistingDocument.Image = global::CosmosManager.Properties.Resources._3floppy_unmount_icon;
            this.saveExistingDocument.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveExistingDocument.Name = "saveExistingDocument";
            this.saveExistingDocument.Size = new System.Drawing.Size(24, 24);
            this.saveExistingDocument.ToolTipText = "Save Document";
            this.saveExistingDocument.Click += new System.EventHandler(this.saveExistingDocument_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveRecordToolStripMenuItem,
            this.saveAllResultsToolStripMenuItem});
            this.toolStripDropDownButton1.Image = global::CosmosManager.Properties.Resources.Downloads_icon;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(33, 24);
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
            // tabOuputPage
            // 
            this.tabOuputPage.Controls.Add(this.textQueryOutput);
            this.tabOuputPage.Location = new System.Drawing.Point(4, 4);
            this.tabOuputPage.Name = "tabOuputPage";
            this.tabOuputPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabOuputPage.Size = new System.Drawing.Size(922, 334);
            this.tabOuputPage.TabIndex = 1;
            this.tabOuputPage.Text = "Output";
            this.tabOuputPage.UseVisualStyleBackColor = true;
            // 
            // textQueryOutput
            // 
            this.textQueryOutput.BackColor = System.Drawing.Color.WhiteSmoke;
            this.textQueryOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textQueryOutput.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textQueryOutput.Location = new System.Drawing.Point(3, 3);
            this.textQueryOutput.Name = "textQueryOutput";
            this.textQueryOutput.ReadOnly = true;
            this.textQueryOutput.Size = new System.Drawing.Size(916, 328);
            this.textQueryOutput.TabIndex = 1;
            this.textQueryOutput.Text = "";
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
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.Controls.Add(this.splitQueryResult);
            this.Name = "QueryWindowControl";
            this.Size = new System.Drawing.Size(930, 585);
            this.splitQueryResult.Panel1.ResumeLayout(false);
            this.splitQueryResult.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitQueryResult)).EndInit();
            this.splitQueryResult.ResumeLayout(false);
            this.queryStatusBar.ResumeLayout(false);
            this.queryStatusBar.PerformLayout();
            this.queryToolStrip.ResumeLayout(false);
            this.queryToolStrip.PerformLayout();
            this.tabControlQueryOutput.ResumeLayout(false);
            this.tabResultsPage.ResumeLayout(false);
            this.splitResultView.Panel1.ResumeLayout(false);
            this.splitResultView.Panel1.PerformLayout();
            this.splitResultView.Panel2.ResumeLayout(false);
            this.splitResultView.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitResultView)).EndInit();
            this.splitResultView.ResumeLayout(false);
            this.resultListToolStrip.ResumeLayout(false);
            this.resultListToolStrip.PerformLayout();
            this.documentToolstrip.ResumeLayout(false);
            this.documentToolstrip.PerformLayout();
            this.tabOuputPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.connectionBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.SplitContainer splitQueryResult;
        public System.Windows.Forms.SplitContainer splitResultView;
        public System.Windows.Forms.ListView resultListView;
        public System.Windows.Forms.ToolStrip documentToolstrip;
        public System.Windows.Forms.SaveFileDialog saveQueryDialog;
        public System.Windows.Forms.Panel queryStatusBar;
        public System.Windows.Forms.BindingSource connectionBindingSource;
        public System.Windows.Forms.ColumnHeader columnSelect;
        public System.Windows.Forms.ColumnHeader columnId;
        public System.Windows.Forms.ColumnHeader columnPartitionKey;
        public System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        public System.Windows.Forms.ToolStripMenuItem saveRecordToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem saveAllResultsToolStripMenuItem;
        public System.Windows.Forms.ToolStrip resultListToolStrip;
        public System.Windows.Forms.ToolStripButton selectedToUpdateButton;
        public System.Windows.Forms.ToolStripButton selectedToDeleteButton;
        public System.Windows.Forms.ToolTip toolTip1;
        public System.Windows.Forms.SaveFileDialog saveJsonDialog;
        public System.Windows.Forms.SaveFileDialog saveTempQueryDialog;
        public System.Windows.Forms.ToolStripButton resultWordWrapButton;
        public System.Windows.Forms.ToolStripButton resultFontSizeDecreaseButton;
        public System.Windows.Forms.ToolStripButton resuleFontSizeIncreaseButton;
        public System.Windows.Forms.ToolStripButton saveExistingDocument;
        public System.Windows.Forms.ToolStripButton deleteDocumentButton;
        public System.Windows.Forms.TabControl tabControlQueryOutput;
        public System.Windows.Forms.TabPage tabResultsPage;
        public System.Windows.Forms.TabPage tabOuputPage;
        public System.Windows.Forms.RichTextBox textQueryOutput;
        public System.Windows.Forms.ToolStripTextBox resultCountTextbox;
        public System.Windows.Forms.ToolStripButton beautifyResultDocumentButton;
        public System.Windows.Forms.ToolStrip queryToolStrip;
        public System.Windows.Forms.ToolStripButton beautifyQueryButton;
        public System.Windows.Forms.ToolStripButton wordWrapToggleButton;
        public System.Windows.Forms.ToolStripButton decreaseFontButton;
        public System.Windows.Forms.ToolStripButton increaseFontButton;
        public System.Windows.Forms.ToolStripButton saveQueryButton;
        public System.Windows.Forms.ToolStripButton runQueryButton;
        public System.Windows.Forms.ToolStripComboBox selectConnections;
        public ScintillaNET.Scintilla textQuery;
        public ScintillaNET.Scintilla textDocument;
        private ToolStripButton lowercaseTextButton;
        private ToolStripButton textUppercaseButon;
        private ToolStripButton textIndentButton;
        private ToolStripButton textOutdentButton;
        private ToolStripButton resultLowercaseButton;
        private ToolStripButton resultUppercaseButton;
        private ToolStripButton resultIndentButton;
        private ToolStripButton resultOutdentButton;
        private ToolStripButton stopQueryButton;
    }
}
