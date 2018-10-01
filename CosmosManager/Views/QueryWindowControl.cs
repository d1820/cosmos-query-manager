using System;
using System.Windows.Forms;
using CosmosManager.Domain;
using CosmosManager.Interfaces;
using System.Linq;
using CosmosManager.Presenters;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using static System.Windows.Forms.ListViewItem;
using Newtonsoft.Json;
using System.Drawing;
using System.Threading.Tasks;
using System.IO;
using CosmosManager.Controls;

namespace CosmosManager
{
    public partial class QueryWindowControl : UserControl, IQueryWindowControl
    {

        private StatsLogger _logger;

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

        public string Stats
        {
            get
            {
                return textStats.Text;
            }
            set
            {
                textStats.Text = value;
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
            textStats.Text = "";
        }

        public QueryWindowPresenter Presenter { private get; set; }
        public MainFormPresenter MainPresenter { private get; set; }

        public void ToggleStatsPanel(bool collapse)
        {
            splitQueryAndStats.Panel2Collapsed = collapse;
        }


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

        public void ShowMessage(string message, string title = null)
        {
            MessageBox.Show(message, title);
        }

        public void SetStatusBarMessage(string message)
        {
            MainPresenter.SetStatusBarMessage(message);
        }

        public void RenderResults(IReadOnlyCollection<object> results)
        {
            resultListView.Items.Clear();
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
                    Text = fromObject["PartitionKey"]?.Value<string>()
                };
                listItem.SubItems.Add(subItem);
                resultListView.Items.Add(listItem);
            }

        }

        private void Checkbox_CheckedChanged(object sender, EventArgs e) => throw new NotImplementedException();

        private async void saveRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveJsonDialog.ShowDialog() == DialogResult.OK)
            {
                await Presenter.SaveDocumentAsync(saveJsonDialog.FileName);
            }
        }

        private async void saveAllResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveJsonDialog.ShowDialog() == DialogResult.OK)
            {
                var objects = new List<JObject>();
                foreach (ListViewItem item in resultListView.Items)
                {
                    objects.Add((JObject)item.Tag);
                }
                await Presenter.SaveAllToDocumentAsync(objects, saveJsonDialog.FileName);
            }

        }

        private void selectedToUpdateButton_Click(object sender, EventArgs e)
        {
            var items = GetCheckedListItems();
            var ids = items.Select(s => s["id"]);
            MainPresenter.CreateTempQueryTab($"UPDATE @{{['{string.Join("','", ids)}']}}@{Environment.NewLine}FROM {Presenter.GetCurrentQueryCollectionName()}{Environment.NewLine}SET @SET{{ }}SET@");
        }

        private void selectedToDeleteButton_Click(object sender, EventArgs e)
        {
            var items = GetCheckedListItems();
            var ids = items.Select(s => s["id"]);
            MainPresenter.CreateTempQueryTab($"DELETE @{{['{string.Join("','", ids)}']}}@{Environment.NewLine}FROM {Presenter.GetCurrentQueryCollectionName()}");
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
            //textDocument.Visible = false;

            //textDocument.Text = JsonConvert.SerializeObject(selectedItem.Tag, Formatting.Indented);
            var tempTextbox = new SyntaxRichTextBox();
            textDocument.Text = JsonConvert.SerializeObject(selectedItem.Tag, Formatting.Indented);
            //await SetSyntaxHighlightAsync((JObject)selectedItem.Tag, tempTextbox);

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
    }
}
