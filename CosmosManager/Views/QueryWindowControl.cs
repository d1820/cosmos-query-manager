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



        private void saveResultButton_Click(object sender, EventArgs e)
        {

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

        }

        private void selectedToDeleteButton_Click(object sender, EventArgs e)
        {

        }

        private void saveQueryButton_Click(object sender, EventArgs e)
        {
            Presenter.SaveQuery();
        }

        private void resultListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (resultListView.SelectedItems.Count == 0)
            {
                return;
            }
            var selectedItem = resultListView.SelectedItems[0];
            //await SetSyntaxHighlightAsync((JObject)selectedItem.Tag);
            textDocument.Text = JsonConvert.SerializeObject(selectedItem.Tag, Formatting.Indented);

        }

        //private Task SetSyntaxHighlightAsync(JObject document)
        //{
        //    //https://www.codeproject.com/Articles/10675/Enabling-syntax-highlighting-in-a-RichTextBox

        //    return Task.Run(() =>
        //    {
        //        if (textDocument.InvokeRequired)
        //        {
        //            textDocument.BeginInvoke((Action)(() =>
        //            {
        //                GetAllProperties(document, textDocument.Settings.Keywords);

        //                // Set the colors that will be used.
        //                textDocument.Settings.KeywordColor = Color.SlateBlue;
        //                textDocument.Settings.CommentColor = Color.Green;
        //                textDocument.Settings.StringColor = Color.DarkGray;
        //                textDocument.Settings.IntegerColor = Color.Red;

        //                // Let's not process strings and integers.
        //                textDocument.Settings.EnableStrings = false;
        //                textDocument.Settings.EnableIntegers = false;

        //                // Let's make the settings we just set valid by compiling
        //                // the keywords to a regular expression.
        //                textDocument.CompileKeywords();
        //                textDocument.ProcessAllLines();

        //            }));
        //        }
        //        else
        //        {
        //            GetAllProperties(document, textDocument.Settings.Keywords);

        //            // Set the colors that will be used.
        //            textDocument.Settings.KeywordColor = Color.SlateBlue;
        //            textDocument.Settings.CommentColor = Color.Green;
        //            textDocument.Settings.StringColor = Color.DarkGray;
        //            textDocument.Settings.IntegerColor = Color.Red;

        //            // Let's not process strings and integers.
        //            //textDocument.Settings.EnableStrings = false;
        //            //textDocument.Settings.EnableIntegers = false;

        //            // Let's make the settings we just set valid by compiling
        //            // the keywords to a regular expression.
        //            textDocument.CompileKeywords();
        //            textDocument.ProcessAllLines();
        //        }

        //    });


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
}
