﻿namespace CosmosManager
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
            this.splitQueryAndStats = new System.Windows.Forms.SplitContainer();
            this.textQuery = new System.Windows.Forms.RichTextBox();
            this.textStats = new System.Windows.Forms.RichTextBox();
            this.queryStatusBar = new System.Windows.Forms.Panel();
            this.runQueryButton = new System.Windows.Forms.Button();
            this.selectConnections = new System.Windows.Forms.ComboBox();
            this.splitResultVIew = new System.Windows.Forms.SplitContainer();
            this.resultListView = new System.Windows.Forms.ListView();
            this.textDocument = new System.Windows.Forms.RichTextBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.saveResultButton = new System.Windows.Forms.ToolStripButton();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.columnSelect = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPartitionKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.connectionBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitQueryResult)).BeginInit();
            this.splitQueryResult.Panel1.SuspendLayout();
            this.splitQueryResult.Panel2.SuspendLayout();
            this.splitQueryResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitQueryAndStats)).BeginInit();
            this.splitQueryAndStats.Panel1.SuspendLayout();
            this.splitQueryAndStats.Panel2.SuspendLayout();
            this.splitQueryAndStats.SuspendLayout();
            this.queryStatusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitResultVIew)).BeginInit();
            this.splitResultVIew.Panel1.SuspendLayout();
            this.splitResultVIew.Panel2.SuspendLayout();
            this.splitResultVIew.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.connectionBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // splitQueryResult
            // 
            this.splitQueryResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitQueryResult.Location = new System.Drawing.Point(0, 0);
            this.splitQueryResult.Name = "splitQueryResult";
            this.splitQueryResult.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitQueryResult.Panel1
            // 
            this.splitQueryResult.Panel1.Controls.Add(this.splitQueryAndStats);
            this.splitQueryResult.Panel1.Controls.Add(this.queryStatusBar);
            // 
            // splitQueryResult.Panel2
            // 
            this.splitQueryResult.Panel2.Controls.Add(this.splitResultVIew);
            this.splitQueryResult.Size = new System.Drawing.Size(930, 585);
            this.splitQueryResult.SplitterDistance = 221;
            this.splitQueryResult.TabIndex = 1;
            // 
            // splitQueryAndStats
            // 
            this.splitQueryAndStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitQueryAndStats.Location = new System.Drawing.Point(0, 32);
            this.splitQueryAndStats.Name = "splitQueryAndStats";
            // 
            // splitQueryAndStats.Panel1
            // 
            this.splitQueryAndStats.Panel1.Controls.Add(this.textQuery);
            // 
            // splitQueryAndStats.Panel2
            // 
            this.splitQueryAndStats.Panel2.Controls.Add(this.textStats);
            this.splitQueryAndStats.Panel2Collapsed = true;
            this.splitQueryAndStats.Size = new System.Drawing.Size(930, 189);
            this.splitQueryAndStats.SplitterDistance = 609;
            this.splitQueryAndStats.TabIndex = 2;
            // 
            // textQuery
            // 
            this.textQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textQuery.Location = new System.Drawing.Point(0, 0);
            this.textQuery.Name = "textQuery";
            this.textQuery.Size = new System.Drawing.Size(930, 189);
            this.textQuery.TabIndex = 0;
            this.textQuery.Text = "";
            // 
            // textStats
            // 
            this.textStats.BackColor = System.Drawing.SystemColors.ControlLight;
            this.textStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textStats.Location = new System.Drawing.Point(0, 0);
            this.textStats.Name = "textStats";
            this.textStats.ReadOnly = true;
            this.textStats.Size = new System.Drawing.Size(317, 189);
            this.textStats.TabIndex = 0;
            this.textStats.Text = "";
            // 
            // queryStatusBar
            // 
            this.queryStatusBar.Controls.Add(this.runQueryButton);
            this.queryStatusBar.Controls.Add(this.selectConnections);
            this.queryStatusBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.queryStatusBar.Location = new System.Drawing.Point(0, 0);
            this.queryStatusBar.Name = "queryStatusBar";
            this.queryStatusBar.Size = new System.Drawing.Size(930, 32);
            this.queryStatusBar.TabIndex = 1;
            // 
            // runQueryButton
            // 
            this.runQueryButton.Location = new System.Drawing.Point(192, 4);
            this.runQueryButton.Name = "runQueryButton";
            this.runQueryButton.Size = new System.Drawing.Size(53, 20);
            this.runQueryButton.TabIndex = 1;
            this.runQueryButton.Text = "Run";
            this.runQueryButton.UseVisualStyleBackColor = true;
            this.runQueryButton.Click += new System.EventHandler(this.runQueryButton_Click_1);
            // 
            // selectConnections
            // 
            this.selectConnections.DisplayMember = "Name";
            this.selectConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectConnections.FormattingEnabled = true;
            this.selectConnections.Location = new System.Drawing.Point(3, 3);
            this.selectConnections.Name = "selectConnections";
            this.selectConnections.Size = new System.Drawing.Size(182, 21);
            this.selectConnections.TabIndex = 0;
            this.selectConnections.SelectedValueChanged += new System.EventHandler(this.selectConnections_SelectedValueChanged);
            // 
            // splitResultVIew
            // 
            this.splitResultVIew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitResultVIew.Location = new System.Drawing.Point(0, 0);
            this.splitResultVIew.Name = "splitResultVIew";
            // 
            // splitResultVIew.Panel1
            // 
            this.splitResultVIew.Panel1.Controls.Add(this.resultListView);
            // 
            // splitResultVIew.Panel2
            // 
            this.splitResultVIew.Panel2.Controls.Add(this.textDocument);
            this.splitResultVIew.Panel2.Controls.Add(this.toolStrip2);
            this.splitResultVIew.Size = new System.Drawing.Size(930, 360);
            this.splitResultVIew.SplitterDistance = 310;
            this.splitResultVIew.TabIndex = 0;
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
            this.resultListView.Location = new System.Drawing.Point(0, 0);
            this.resultListView.Name = "resultListView";
            this.resultListView.Size = new System.Drawing.Size(310, 360);
            this.resultListView.TabIndex = 0;
            this.resultListView.UseCompatibleStateImageBehavior = false;
            this.resultListView.View = System.Windows.Forms.View.Details;
            // 
            // textDocument
            // 
            this.textDocument.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textDocument.Location = new System.Drawing.Point(0, 25);
            this.textDocument.Name = "textDocument";
            this.textDocument.Size = new System.Drawing.Size(616, 335);
            this.textDocument.TabIndex = 2;
            this.textDocument.Text = "";
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveResultButton});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(616, 25);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // saveResultButton
            // 
            this.saveResultButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.saveResultButton.Image = ((System.Drawing.Image)(resources.GetObject("saveResultButton.Image")));
            this.saveResultButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.saveResultButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveResultButton.Name = "saveResultButton";
            this.saveResultButton.Size = new System.Drawing.Size(35, 22);
            this.saveResultButton.Text = "Save";
            this.saveResultButton.Click += new System.EventHandler(this.saveResultButton_Click);
            // 
            // columnSelect
            // 
            this.columnSelect.Text = "";
            this.columnSelect.Width = 37;
            // 
            // columnId
            // 
            this.columnId.Text = "id";
            this.columnId.Width = 103;
            // 
            // columnPartitionKey
            // 
            this.columnPartitionKey.Text = "PartitionKey";
            this.columnPartitionKey.Width = 150;
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
            this.splitQueryAndStats.Panel1.ResumeLayout(false);
            this.splitQueryAndStats.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitQueryAndStats)).EndInit();
            this.splitQueryAndStats.ResumeLayout(false);
            this.queryStatusBar.ResumeLayout(false);
            this.splitResultVIew.Panel1.ResumeLayout(false);
            this.splitResultVIew.Panel2.ResumeLayout(false);
            this.splitResultVIew.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitResultVIew)).EndInit();
            this.splitResultVIew.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.connectionBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitQueryResult;
        private System.Windows.Forms.RichTextBox textQuery;
        private System.Windows.Forms.SplitContainer splitResultVIew;
        private System.Windows.Forms.ListView resultListView;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton saveResultButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Panel queryStatusBar;
        private System.Windows.Forms.ComboBox selectConnections;
        private System.Windows.Forms.Button runQueryButton;
        private System.Windows.Forms.BindingSource connectionBindingSource;
        private System.Windows.Forms.RichTextBox textStats;
        private System.Windows.Forms.SplitContainer splitQueryAndStats;
        private System.Windows.Forms.RichTextBox textDocument;
        private System.Windows.Forms.ColumnHeader columnSelect;
        private System.Windows.Forms.ColumnHeader columnId;
        private System.Windows.Forms.ColumnHeader columnPartitionKey;
    }
}