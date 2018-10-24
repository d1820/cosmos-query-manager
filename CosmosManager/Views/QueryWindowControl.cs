using CosmosManager.Controls;
using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace CosmosManager
{
    public partial class QueryWindowControl : UserControl, IQueryWindowControl
    {
        public QueryWindowControl()
        {
            InitializeComponent();

            resultListView.DoubleBuffered(true);

            //look for a connections string file
            selectConnections.Items.Add("Load Connection File");
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
        }

        public IQueryWindowPresenter Presenter { private get; set; }
        public IMainFormPresenter MainPresenter { private get; set; }

        private async void runQueryButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                runQueryButton.Enabled = false;
                await Presenter.RunAsync();
            }
            finally
            {
                runQueryButton.Enabled = true;
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
            MainPresenter.SetStatusBarMessage(message);
        }

        public void SetUpdatedResultDocument(object document)
        {
            var selectedItem = resultListView.SelectedItems[0];
            ((DocumentResult)selectedItem.Tag).Document = JObject.FromObject(document);
        }

        public async void RenderResults(IReadOnlyCollection<object> results, string collectionName, bool appendResults, int queryStatementIndex)
        {
            if (!appendResults)
            {
                resultListView.Groups.Clear();
                resultListView.Items.Clear();
            }
            var textPartitionKeyPath = await Presenter.LookupPartitionKeyPath();
            if (appendResults)
            {
                resultListView.Groups.Add(new ListViewGroup
                {
                    Header = $"Query {queryStatementIndex}",
                    Name = $"Query{resultListView.Groups.Count}",
                    HeaderAlignment = HorizontalAlignment.Center
                });
            }
            foreach (var item in results)
            {
                var fromObject = JObject.FromObject(item);
                var listItem = new ListViewItem();
                if (appendResults && resultListView.Groups.Count > 0)
                {
                    listItem.Group = resultListView.Groups[resultListView.Groups.Count - 1];
                }
                listItem.Tag = new DocumentResult { Document = fromObject, CollectionName = collectionName };
                var subItem = new ListViewSubItem
                {
                    Text = fromObject[Constants.DocumentFields.ID]?.Value<string>()
                };
                listItem.SubItems.Add(subItem);

                subItem = new ListViewSubItem
                {
                    Text = fromObject[textPartitionKeyPath]?.Value<string>()
                };
                listItem.SubItems.Add(subItem);
                resultListView.Items.Add(listItem);
            }
            resultCountTextbox.Text = $"{results.Count} Documents";
            resultListToolStrip.Refresh();
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
            var items = GetCheckedListItems();
            if (!items.Any())
            {
                return;
            }
            var ids = items.Select(s => s[Constants.DocumentFields.ID]);
            var parser = AppReferences.Container.GetInstance<IQueryStatementParser>();
            var parts = parser.Parse(Query);
            MainPresenter.CreateTempQueryTab($"{Constants.QueryKeywords.TRANSACTION}{Environment.NewLine}{Constants.QueryKeywords.UPDATE} '{string.Join("','", ids)}' {Environment.NewLine}{Constants.QueryKeywords.FROM} {parts.CollectionName} {Environment.NewLine}{Constants.QueryKeywords.SET} {{{Environment.NewLine}{Environment.NewLine}}}");
        }

        private void selectedToDeleteButton_Click(object sender, EventArgs e)
        {
            var items = GetCheckedListItems();
            if (!items.Any())
            {
                return;
            }
            var ids = items.Select(s => s[Constants.DocumentFields.ID]);
            var parser = AppReferences.Container.GetInstance<IQueryStatementParser>();
            var parts = parser.Parse(Query);
            MainPresenter.CreateTempQueryTab($"{Constants.QueryKeywords.TRANSACTION}{Environment.NewLine} {Constants.QueryKeywords.DELETE} '{string.Join("','", ids)}' {Environment.NewLine} {Constants.QueryKeywords.FROM} {parts.CollectionName}");
        }

        private async void saveQueryButton_Click(object sender, EventArgs e)
        {
            if (Presenter.CurrentFileInfo == null)
            {
                if (saveTempQueryDialog.ShowDialog() == DialogResult.OK)
                {
                    await Presenter.SaveTempQueryAsync(saveTempQueryDialog.FileName);
                    var fileName = new FileInfo(saveTempQueryDialog.FileName);
                    MainPresenter.UpdateNewQueryTabName(fileName.Name);
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
            textQuery.ZoomFactor *= 1.25F;
        }

        private void decreaseFontButton_Click(object sender, EventArgs e)
        {
            if (textQuery.ZoomFactor > 0)
            {
                textQuery.ZoomFactor /= 1.25F;
            }
        }

        private void wordWrapToggleButton_Click(object sender, EventArgs e)
        {
            textQuery.WordWrap = !textQuery.WordWrap;
        }

        private void resultWordWrapButton_Click(object sender, EventArgs e)
        {
            textDocument.WordWrap = !textDocument.WordWrap;
        }

        private void resultFontSizeDecreaseButton_Click(object sender, EventArgs e)
        {
            if (textDocument.ZoomFactor > 0)
            {
                textDocument.ZoomFactor /= 1.25F;
            }
        }

        private void resuleFontSizeIncreaseButton_Click(object sender, EventArgs e)
        {
            textDocument.ZoomFactor *= 1.25F;
        }

        private List<JObject> GetCheckedListItems()
        {
            var objects = new List<JObject>();
            foreach (ListViewItem item in resultListView.Items)
            {
                if (item.Tag is DocumentResult && item.Checked)
                {
                    var result = (DocumentResult)item.Tag;
                    objects.Add(result.Document);
                }
            }
            return objects;
        }

        private Task SetSyntaxHighlightAsync(JObject document, SyntaxRichTextBox textbox)
        {
            //https://www.codeproject.com/Articles/10675/Enabling-syntax-highlighting-in-a-RichTextBox

            return Task.Run(() =>
            {
                if (textbox.InvokeRequired)
                {
                    textbox.BeginInvoke((Action)(() =>
                    {
                        // Set the colors that will be used.
                        textbox.Settings.KeywordColor = Color.SlateBlue;
                        textbox.Settings.CommentColor = Color.Green;
                        textbox.Settings.StringColor = Color.DarkGray;
                        textbox.Settings.IntegerColor = Color.Red;

                        // Let's not process strings and integers.
                        textbox.Settings.EnableComments = false;
                        textbox.ProcessAllLines();
                    }));
                }
                else
                {
                    // Set the colors that will be used.
                    textbox.Settings.KeywordColor = Color.SlateBlue;
                    textbox.Settings.CommentColor = Color.Green;
                    textbox.Settings.StringColor = Color.DarkGray;
                    textbox.Settings.IntegerColor = Color.Red;

                    textbox.Settings.EnableComments = false;

                    textbox.ProcessAllLines();
                }
            });

            //}

            //private void GetAllProperties(JObject parent, List<string> propList)
            //{
            //    var props = parent.Properties();
            //    foreach (var property in props)
            //    {
            //        if (!propList.Contains(property.Name))
            //        {
            //            propList.Add($"{property.Name}");
            //            ParseJArray(property.Value.Children<JArray>(), propList);

            //        }
            //    }
            //    foreach (var child in parent.Children<JObject>())
            //    {
            //        GetAllProperties(child, propList);
            //    }

            //    ParseJArray(parent.Children<JArray>(), propList);
            //}

            //private void ParseJArray(IEnumerable<JArray> arrays, List<string> propList)
            //{
            //    foreach (var childArray in arrays)
            //    {
            //        foreach (var child in childArray)
            //        {
            //            var objects = child.Children<JObject>();
            //            foreach (var obj in objects)
            //            {
            //                GetAllProperties(obj, propList);
            //            }
            //        }
            //    }
            //}
        }

        private CheckState headerCheckState = CheckState.Unchecked;

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
                cck.CheckState = headerCheckState;
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
            headerCheckState = listboxCheckHeader.CheckState;
            for (var i = 0; i < resultListView.Items.Count; i++)
            {
                resultListView.Items[i].Checked = listboxCheckHeader.Checked;
            }
        }

        private void resultListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        //        private void HideCheckbox(ListView lvw, ListViewItem item)
        //        {
        //            var lviItem = new LVITEM();
        //            lviItem.iItem = item.Index;
        //            lviItem.mask = LVIF_STATE;
        //            lviItem.stateMask = LVIS_STATEIMAGEMASK;
        //            lviItem.state = 0;
        //            SendMessage(lvw.Handle, LVM_SETITEM, IntPtr.Zero, ref lviItem);
        //        }

        //        private const int LVIF_STATE = 0x8;
        //        private const int LVIS_STATEIMAGEMASK = 0xF000;
        //        private const int LVM_FIRST = 0x1000;
        //        private const int LVM_SETITEM = LVM_FIRST + 76;

        //        // suppress warnings for interop
        //#pragma warning disable 0649
        //        private struct LVITEM
        //        {
        //            public int mask;
        //            public int iItem;
        //            public int iSubItem;
        //            public int state;
        //            public int stateMask;
        //            [MarshalAs(UnmanagedType.LPTStr)]
        //            public String lpszText;
        //            public int cchTextMax;
        //            public int iImage;
        //            public IntPtr iParam;
        //        }
        //#pragma warning restore 0649

        //        [DllImport("user32.dll")]
        //        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref LVITEM lParam);


        private void resultListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private async void saveExistingDocument_Click(object sender, EventArgs e)
        {
            var documentResult = (DocumentResult)resultListView.SelectedItems[0].Tag;
            var doc = JsonConvert.DeserializeObject<object>(textDocument.Text);
            documentResult.Document = JObject.FromObject(doc);
            await Presenter.SaveDocumentAsync(documentResult);
        }

        private async void deleteDocumentButton_Click(object sender, EventArgs e)
        {
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
    }
}