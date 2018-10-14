using CosmosManager.Controls;
using CosmosManager.Domain;
using CosmosManager.Interfaces;
using CosmosManager.Parsers;
using CosmosManager.Presenters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace CosmosManager
{
    public partial class QueryWindowControl : UserControl, IQueryWindowControl
    {
        private QueryOuputLogger _logger;

        public QueryWindowControl()
        {
            InitializeComponent();

            //look for a connections string file
            selectConnections.Items.Add("Load Connection File");
        }

        public object[] ConnectionsList
        {
            get
            {
                return selectConnections.Items.Cast<object>().ToArray();
            }
            set
            {
                selectConnections.Items.Clear();
                selectConnections.Items.AddRange(value);
                selectConnections.SelectedIndex = 0;
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

        public QueryWindowPresenter Presenter { private get; set; }
        public MainFormPresenter MainPresenter { private get; set; }

        private async void runQueryButton_Click_1(object sender, EventArgs e)
        {
            Presenter.Run();
        }

        private void selectConnections_SelectedValueChanged(object sender, EventArgs e)
        {
            if (selectConnections.SelectedItem is Connection)
            {
                Presenter.SelectedConnection = (Connection)selectConnections.SelectedItem;
                MainPresenter.UpdateTabHeaderColor();
            }
        }

        public void ShowMessage(string message, string title = null, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            MessageBox.Show(message, title, buttons, icon);
        }

        public void SetStatusBarMessage(string message)
        {
            MainPresenter.SetStatusBarMessage(message);
        }

        public async void RenderResults(IReadOnlyCollection<object> results)
        {
            resultListView.Items.Clear();
            var textPartitionKeyPath = await Presenter.LookupPartitionKeyPath();
            foreach (var item in results)
            {
                var fromObject = JObject.FromObject(item);
                var listItem = new ListViewItem();
                listItem.Tag = fromObject;
                var subItem = new ListViewSubItem
                {
                    Text = fromObject["id"]?.Value<string>()
                };
                listItem.SubItems.Add(subItem);

                subItem = new ListViewSubItem
                {
                    Text = fromObject[textPartitionKeyPath]?.Value<string>()
                };
                listItem.SubItems.Add(subItem);
                resultListView.Items.Add(listItem);
            }
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
                    objects.Add((JObject)item.Tag);
                }
                await Presenter.ExportAllToDocumentAsync(objects, saveJsonDialog.FileName);
            }
        }

        private void selectedToUpdateButton_Click(object sender, EventArgs e)
        {
            var items = GetCheckedListItems();
            var ids = items.Select(s => s["id"]);
            var parser = new QueryStatementParser();
            var parts = parser.Parse(Query);
            MainPresenter.CreateTempQueryTab($"TRANSACTION{Environment.NewLine}@UPDATE{{'{string.Join("','", ids)}'}}@{Environment.NewLine}@FROM{{ {parts.CollectionName} }}@{Environment.NewLine}SET @SET{{ }}@");
        }

        private void selectedToDeleteButton_Click(object sender, EventArgs e)
        {
            var items = GetCheckedListItems();
            var ids = items.Select(s => s["id"]);
            var parser = new QueryStatementParser();
            var parts = parser.Parse(Query);
            MainPresenter.CreateTempQueryTab($"TRANSACTION{Environment.NewLine} DELETE '{string.Join("','", ids)}' {Environment.NewLine} FROM {parts.CollectionName}");
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

        private async void resultListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (resultListView.SelectedItems.Count == 0)
            {
                return;
            }
            var selectedItem = resultListView.SelectedItems[0];

            textDocument.Text = JsonConvert.SerializeObject(selectedItem.Tag, Formatting.Indented);
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
                if (item.Tag is JObject && item.Checked)
                {
                    objects.Add(item.Tag as JObject);
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
            if ((e.ColumnIndex == 0))
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

        private void resultListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private async void saveExistingDocument_Click(object sender, EventArgs e)
        {
            var doc = JsonConvert.DeserializeObject<object>(textDocument.Text);
            await Presenter.SaveDocumentAsync(doc);
        }

        private async void deleteDocumentButton_Click(object sender, EventArgs e)
        {
            var selectedItem = (JObject)resultListView.SelectedItems[0].Tag;
            if (MessageBox.Show(this, $"Are you sure you want to delete document {selectedItem["id"]}", "Delete Document", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                var wasDeleted = await Presenter.DeleteDocumentAsync(selectedItem);
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
    }
}