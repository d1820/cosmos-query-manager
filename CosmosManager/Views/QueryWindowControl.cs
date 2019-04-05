using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using CosmosManager.Stylers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace CosmosManager
{
    public partial class QueryWindowControl : UserControl, IQueryWindowControl
    {
        private int _totalDocumentCount;

        private CheckState _headerCheckState;

        public QueryWindowControl(IQueryStyler queryTextStyler, IJsonStyler jsonStyler)
        {
            InitializeComponent();

            QueryWindowStyler.ApplyTheme(ThemeType.Dark, this);

            resultListView.DoubleBuffered(true);

            //look for a connections string file
            selectConnections.Items.Add("Load Connection File");

            queryTextStyler.SyntaxifyTextBox(textQuery);
            jsonStyler.SyntaxifyTextBox(textDocument);
        }

        public object[] ConnectionsList
        {
            get
            {
                return selectConnections.ComboBox.Items.Cast<object>().ToArray();
            }
            set
            {
                selectConnections.ComboBox.Items.Clear();
                selectConnections.ComboBox.DisplayMember = "Name";
                selectConnections.ComboBox.Items.AddRange(value);
                selectConnections.ComboBox.SelectedIndex = 0;
            }
        }

        public string Query
        {
            get
            {
                return textQuery.Text;
            }
            set
            {
                textQuery.Text = value;
            }
        }

        public string QueryOutput
        {
            get
            {
                return textQueryOutput.Text;
            }
        }

        public string DocumentText
        {
            get
            {
                return textDocument.Text;
            }
            set
            {
                textDocument.Text = value;
            }
        }

        public void ClearStats()
        {
            textQueryOutput.Text = "";
        }

        public void ResetResultsView()
        {
            resultListView.Items.Clear();
            textDocument.Clear();
            tabControlQueryOutput.SelectedIndex = 0;
        }

        public IQueryWindowPresenter Presenter { private get; set; }

        public IMainFormPresenter MainPresenter { private get; set; }

        private async void runQueryButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                runQueryButton.Visible = false;
                stopQueryButton.Visible = true;
                _totalDocumentCount = 0;
                textDocument.Text = string.Empty;
                SetResultCountLabel();
                resultListView.Groups.Clear();
                resultListView.Items.Clear();
                Application.DoEvents();
                await Presenter.RunAsync();
            }
            finally
            {
                runQueryButton.Visible = true;
                stopQueryButton.Visible = false;
            }
        }

        private void selectConnections_SelectedValueChanged(object sender, EventArgs e)
        {
            if (selectConnections.SelectedItem is Connection)
            {
                Presenter.SelectedConnection = (Connection)selectConnections.SelectedItem;
                MainPresenter.UpdateTabHeaderColor();
                return;
            }
            Presenter.SelectedConnection = null;
        }

        public DialogResult ShowMessage(string message, string title = null, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            return MessageBox.Show(message, title, buttons, icon);
        }

        public void SetStatusBarMessage(string message, bool ignoreClearTimer = false)
        {
            MainPresenter.SetStatusBarMessage(message, ignoreClearTimer);
        }

        public void SetUpdatedResultDocument(object document)
        {
            var selectedItem = resultListView.SelectedItems[0];
            ((DocumentResult)selectedItem.Tag).Document = JObject.FromObject(document);
        }

        public async void RenderResults(IReadOnlyCollection<object> results, string collectionName, QueryParts query, bool appendResults, int queryStatementIndex)
        {
            if (!appendResults)
            {
                resultListView.Groups.Clear();
                resultListView.Items.Clear();
                _totalDocumentCount = 0;
            }
            var textPartitionKeyPath = await Presenter.LookupPartitionKeyPath();
            var groupName = "Query 1";
            if (appendResults)
            {
                resultListView.Groups.Add(new ListViewGroup
                {
                    Header = $"Query {queryStatementIndex} ({results.Count} Documents)",
                    Name = $"Query{resultListView.Groups.Count}",
                    HeaderAlignment = HorizontalAlignment.Center
                });
                groupName = $"Query {queryStatementIndex}";
                if (resultListView.Groups.Count == 1)
                {
                    //first group set headers
                    var headers = SetResultListViewHeaders(results.FirstOrDefault(), textPartitionKeyPath);
                    resultListView.Columns[1].Text = headers.header1;
                    resultListView.Columns[2].Text = headers.header2;
                }
                else
                {
                    var headers = SetResultListViewHeaders(results.FirstOrDefault(), textPartitionKeyPath);
                    //if the next query has a different select, then clear column headers
                    if (resultListView.Columns[1].Text != headers.header1)
                    {
                        resultListView.Columns[1].Text = string.Empty;
                    }
                    if (resultListView.Columns[2].Text != headers.header2)
                    {
                        resultListView.Columns[2].Text = string.Empty;
                    }
                }
            }
            else
            {
                var headers = SetResultListViewHeaders(results.FirstOrDefault(), textPartitionKeyPath);
                resultListView.Columns[1].Text = headers.header1;
                resultListView.Columns[2].Text = headers.header2;
            }
            var validResultCount = 0;
            foreach (var item in results)
            {
                var fromObject = JObject.FromObject(item);
                if (!fromObject.HasValues)
                {
                    continue;
                }
                validResultCount++;
                var listItem = new ListViewItem();
                if (appendResults && resultListView.Groups.Count > 0)
                {
                    listItem.Group = resultListView.Groups[resultListView.Groups.Count - 1];
                }
                listItem.Tag = new DocumentResult { Document = fromObject, CollectionName = collectionName, Query = query, GroupName = groupName };
                JProperty col1Prop = null;
                JToken col1Token = null;
                var resultProps = fromObject.Properties();
                var subItem = new ListViewSubItem
                {
                    Text = String.Empty
                };

                if (resultProps.Count() > 0)
                {
                    col1Prop = resultProps.FirstOrDefault(f => f.Name == Constants.DocumentFields.ID);
                    if (col1Prop == null)
                    {
                        col1Prop = resultProps.FirstOrDefault();
                    }
                    col1Token = col1Prop?.Value;
                    subItem.Text = col1Token?.Type != JTokenType.Object ? col1Token?.Value<string>() : "";
                    if (col1Prop?.Type == JTokenType.Property)
                    {
                        var resultPropJToken = col1Prop?.Value;
                        if(resultPropJToken?.Type == JTokenType.Object)
                        {
                            col1Token = resultPropJToken.SelectToken(Constants.DocumentFields.ID);
                            if(col1Token?.Value<object>() == null)
                            {
                                subItem.Text = resultPropJToken.FirstOrDefault()?.FirstOrDefault()?.Value<dynamic>();
                            }
                            else
                            {
                                subItem.Text = col1Token?.Value<string>();
                            }
                        }
                    }
                }
                listItem.SubItems.Add(subItem);

                if (resultProps.Count() > 1)
                {
                    subItem = new ListViewSubItem
                    {
                        Text = String.Empty
                    };
                    JProperty col2Prop = null;
                    JToken col2Token = null;

                    col2Prop = resultProps.FirstOrDefault(f => f.Name == textPartitionKeyPath);
                    if (col2Prop == null)
                    {
                        var prop = resultProps.FirstOrDefault(f => f != col1Prop);
                        if (prop != null)
                        {
                            col2Prop = prop;
                            col2Token = prop.Value;

                        }
                    }
                    col2Token = col2Prop?.Value;
                    subItem.Text = col2Token?.Value<string>();

                    if (!String.IsNullOrEmpty(subItem.Text))
                    {
                        listItem.SubItems.Add(subItem);
                    }
                }
                resultListView.Items.Add(listItem);
            }
            _totalDocumentCount += validResultCount;
            SetResultCountLabel();
            resultListToolStrip.Refresh();
        }

        private void SetResultCountLabel()
        {
            resultCountTextbox.Text = $"{_totalDocumentCount} Documents";
        }

        private (string header1, string header2) SetResultListViewHeaders(object item, string textPartitionKeyPath)
        {
            if (item == null)
            {
                return (null, null);
            }
            var fromObject = JObject.FromObject(item);

            JProperty col1Prop = null;
            var resultProps = fromObject.Properties();
            col1Prop = resultProps.FirstOrDefault(f => f.Name == Constants.DocumentFields.ID);
            if (col1Prop == null)
            {
                col1Prop = resultProps.First();
            }

            JProperty col2Prop = null;
            if (resultProps.Count() > 1)
            {

                col2Prop = resultProps.FirstOrDefault(f => f.Name == textPartitionKeyPath);
                if (col2Prop == null)
                {
                    var prop = resultProps.FirstOrDefault(f => f != col1Prop);
                    if (prop != null)
                    {
                        col2Prop = prop;

                    }
                }

            }
            return (col1Prop.Name, col2Prop?.Name);
        }

        private async void exportRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveJsonDialog.ShowDialog() == DialogResult.OK)
            {
                await Presenter.ExportDocumentAsync(saveJsonDialog.FileName);
            }
        }

        private async void exportAllResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveJsonDialog.ShowDialog() == DialogResult.OK)
            {
                var objects = new List<JObject>();
                foreach (ListViewItem item in resultListView.Items)
                {
                    var result = (DocumentResult)item.Tag;
                    objects.Add(result.Document);
                }
                await Presenter.ExportAllToDocumentAsync(objects, saveJsonDialog.FileName);
            }
        }

        private void selectedToUpdateButton_Click(object sender, EventArgs e)
        {
            var checklistItems = GetCheckedListItems();
            if (!checklistItems.Any())
            {
                return;
            }
            var groupNamesNotSupported = new List<string>();
            foreach (var group in checklistItems.GroupBy(g => g.Query))
            {
                var items = group.Select(s => s.Document);
                var ids = items.Where(w => w[Constants.DocumentFields.ID] != null).Select(s => s[Constants.DocumentFields.ID]);
                if (ids.Count() == 0)
                {
                    var documentResult = group.First();
                    groupNamesNotSupported.Add(documentResult.GroupName);
                    continue;
                }
                var parts = group.Key;
                MainPresenter.CreateTempQueryTab($"{Constants.QueryParsingKeywords.TRANSACTION}{Environment.NewLine}{Constants.QueryParsingKeywords.UPDATE} '{string.Join("','", ids)}'{Environment.NewLine}{Constants.QueryParsingKeywords.FROM} {parts.CollectionName}{Environment.NewLine}{Constants.QueryParsingKeywords.SET} {{{Environment.NewLine}{Environment.NewLine}}}");
            }
            if (groupNamesNotSupported.Count > 0)
            {
                ShowMessage($"The id column must be part of the select output in {string.Join(", ", groupNamesNotSupported)} to use this feature.", "Action Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void selectedToDeleteButton_Click(object sender, EventArgs e)
        {
            var checklistItems = GetCheckedListItems();
            if (!checklistItems.Any())
            {
                return;
            }
            var groupNamesNotSupported = new List<string>();
            foreach (var group in checklistItems.GroupBy(g => g.Query))
            {
                var items = group.Select(s => s.Document);

                var ids = items.Where(w => w[Constants.DocumentFields.ID] != null).Select(s => s[Constants.DocumentFields.ID]);
                if (ids.Count() == 0)
                {
                    var documentResult = group.First();
                    groupNamesNotSupported.Add(documentResult.GroupName);
                    continue;
                }
                var parts = group.Key;
                MainPresenter.CreateTempQueryTab($"{Constants.QueryParsingKeywords.TRANSACTION}{Environment.NewLine} {Constants.QueryParsingKeywords.DELETE} '{string.Join("','", ids)}' {Environment.NewLine} {Constants.QueryParsingKeywords.FROM} {parts.CollectionName}");
            }
            if (groupNamesNotSupported.Count > 0)
            {
                ShowMessage($"The id column must be part of the select output in {string.Join(", ", groupNamesNotSupported)} to use this feature.", "Action Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private async void saveQueryButton_Click(object sender, EventArgs e)
        {
            if (Presenter.CurrentFileInfo == null)
            {
                if (saveTempQueryDialog.ShowDialog() == DialogResult.OK)
                {
                    await Presenter.SaveTempQueryAsync(saveTempQueryDialog.FileName);
                    var fileInfo = new FileInfo(saveTempQueryDialog.FileName);
                    Presenter.SetFile(fileInfo);
                    MainPresenter.UpdateNewQueryTabName(fileInfo.Name);
                }
                return;
            }
            await Presenter.SaveQueryAsync();
        }

        private void resultListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (resultListView.SelectedItems.Count == 0)
            {
                return;
            }
            var selectedItem = resultListView.SelectedItems[0];
            if (selectedItem.Tag == null)
            {
                return;
            }
            textDocument.Text = JsonConvert.SerializeObject(((DocumentResult)selectedItem.Tag).Document, Formatting.Indented);
        }

        private void increaseFontButton_Click(object sender, EventArgs e)
        {
            textQuery.ZoomIn();
        }

        private void decreaseFontButton_Click(object sender, EventArgs e)
        {
            if (textQuery.Zoom > 0)
            {
                textQuery.ZoomOut();
            }
        }

        private void wordWrapToggleButton_Click(object sender, EventArgs e)
        {
            if (textQuery.WrapMode == WrapMode.None)
            {
                textQuery.WrapMode = WrapMode.Word;
            }
            else
            {
                textQuery.WrapMode = WrapMode.None;
            }
        }

        private void resultWordWrapButton_Click(object sender, EventArgs e)
        {
            if (textDocument.WrapMode == WrapMode.None)
            {
                textDocument.WrapMode = WrapMode.Word;
            }
            else
            {
                textDocument.WrapMode = WrapMode.None;
            }
        }

        private void resultFontSizeDecreaseButton_Click(object sender, EventArgs e)
        {
            if (textDocument.Zoom > 0)
            {
                textDocument.ZoomOut();
            }
        }

        private void resuleFontSizeIncreaseButton_Click(object sender, EventArgs e)
        {
            textDocument.ZoomIn();
        }

        private List<DocumentResult> GetCheckedListItems()
        {
            var objects = new List<DocumentResult>();
            foreach (ListViewItem item in resultListView.Items)
            {
                if (item.Tag is DocumentResult && item.Checked)
                {
                    var result = (DocumentResult)item.Tag;
                    objects.Add(result);
                }
            }
            return objects;
        }

        private void resultListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {

                var cck = new CheckBox();
                // With...
                Text = "";
                Visible = true;
                resultListView.SuspendLayout();
                e.DrawBackground();
                cck.BackColor = e.BackColor;
                cck.UseVisualStyleBackColor = true;

                cck.SetBounds(e.Bounds.X, e.Bounds.Y, cck.GetPreferredSize(new Size(e.Bounds.Width, e.Bounds.Height)).Width, cck.GetPreferredSize(new Size(e.Bounds.Width, e.Bounds.Height)).Width);
                cck.Size = new Size((cck.GetPreferredSize(new Size((e.Bounds.Width - 1), e.Bounds.Height)).Width + 1), e.Bounds.Height);
                cck.Location = new Point(4, 0);
                cck.CheckState = _headerCheckState;
                resultListView.Controls.Add(cck);
                cck.Show();
                cck.BringToFront();
                e.DrawText((TextFormatFlags.VerticalCenter | TextFormatFlags.Left));
                cck.Click += resultListViewheaderCheckAll;
                resultListView.ResumeLayout(true);
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void resultListViewheaderCheckAll(object sender, EventArgs e)
        {
            var listboxCheckHeader = sender as CheckBox;
            _headerCheckState = listboxCheckHeader.CheckState;
            for (var i = 0; i < resultListView.Items.Count; i++)
            {
                resultListView.Items[i].Checked = listboxCheckHeader.Checked;
            }
        }

        private void resultListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void resultListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private async void saveExistingDocument_Click(object sender, EventArgs e)
        {
            if (resultListView.SelectedItems.Count == 0)
            {
                return;
            }
            var documentResult = (DocumentResult)resultListView.SelectedItems[0].Tag;
            var doc = JsonConvert.DeserializeObject<object>(textDocument.Text);
            documentResult.Document = JObject.FromObject(doc);
            await Presenter.SaveDocumentAsync(documentResult);
        }

        private async void deleteDocumentButton_Click(object sender, EventArgs e)
        {
            if (resultListView.SelectedItems.Count == 0)
            {
                return;
            }
            var documentResult = (DocumentResult)resultListView.SelectedItems[0].Tag;
            var selectedDocument = documentResult.Document;
            if (MessageBox.Show(this, $"Are you sure you want to delete document {selectedDocument[Constants.DocumentFields.ID]}", "Delete Document", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                var wasDeleted = await Presenter.DeleteDocumentAsync(documentResult);
                if (wasDeleted)
                {
                    //remove item from list
                    resultListView.Items.Remove(resultListView.SelectedItems[0]);
                }
            }
        }

        public void ShowOutputTab()
        {
            tabControlQueryOutput.SelectedIndex = 1;
        }

        public void AppendToQueryOutput(string message)
        {
            if (textQueryOutput.InvokeRequired)
            {
                textQueryOutput.BeginInvoke((Action)(() =>
                        {
                            // Set the colors that will be used.
                            textQueryOutput.Text += message;
                        }));
                return;
            }
            textQueryOutput.Text += message;
        }

        public void ResetQueryOutput()
        {
            if (textQueryOutput.InvokeRequired)
            {
                textQueryOutput.BeginInvoke((Action)(() =>
                        {
                            // Set the colors that will be used.
                            textQueryOutput.Text = string.Empty;
                        }));
                return;
            }
            textQueryOutput.Text = string.Empty;
        }

        private void beautifyResultDocumentButton_Click(object sender, EventArgs e)
        {
            textDocument.Text = Presenter.Beautify(textDocument.Text);
        }

        private void beautifyQueryButton_Click(object sender, EventArgs e)
        {
            textQuery.Text = Presenter.BeautifyQuery(textQuery.Text);
        }

        private void GenerateKeystrokes(string keys, Scintilla textbox)
        {
            //HotKeyManager.Enable = false;
            textbox.Focus();
            SendKeys.Send(keys);
            //HotKeyManager.Enable = true;
        }


        private void lowercaseTextButton_Click(object sender, EventArgs e)
        {
            // save the selection
            var start = textQuery.SelectionStart;
            var end = textQuery.SelectionEnd;

            // modify the selected text
            textQuery.ReplaceSelection(textQuery.GetTextRange(start, end - start).ToLower());

            // preserve the original selection
            textQuery.SetSelection(start, end);
        }

        private void textUppercaseButon_Click(object sender, EventArgs e)
        {
            // save the selection
            var start = textQuery.SelectionStart;
            var end = textQuery.SelectionEnd;

            // modify the selected text
            textQuery.ReplaceSelection(textQuery.GetTextRange(start, end - start).ToUpper());

            // preserve the original selection
            textQuery.SetSelection(start, end);
        }

        private void textIndentButton_Click(object sender, EventArgs e)
        {
            GenerateKeystrokes("{TAB}", textQuery);
        }

        private void textOutdentButton_Click(object sender, EventArgs e)
        {
            GenerateKeystrokes("+{TAB}", textQuery);
        }

        private void resultLowercaseButton_Click(object sender, EventArgs e)
        {
            // save the selection
            var start = textDocument.SelectionStart;
            var end = textDocument.SelectionEnd;

            // modify the selected text
            textDocument.ReplaceSelection(textDocument.GetTextRange(start, end - start).ToLower());

            // preserve the original selection
            textDocument.SetSelection(start, end);
        }

        private void resultUppercaseButton_Click(object sender, EventArgs e)
        {
            // save the selection
            var start = textDocument.SelectionStart;
            var end = textDocument.SelectionEnd;

            // modify the selected text
            textDocument.ReplaceSelection(textDocument.GetTextRange(start, end - start).ToUpper());

            // preserve the original selection
            textDocument.SetSelection(start, end);
        }

        private void resultIndentButton_Click(object sender, EventArgs e)
        {
            GenerateKeystrokes("{TAB}", textDocument);
        }

        private void resultOutdentButton_Click(object sender, EventArgs e)
        {
            GenerateKeystrokes("+{TAB}", textDocument);
        }

        private void stopQueryButton_Click(object sender, EventArgs e)
        {
            runQueryButton.Visible = true;
            stopQueryButton.Visible = false;
            Application.DoEvents();
            Presenter.StopQuery();
        }
    }
}