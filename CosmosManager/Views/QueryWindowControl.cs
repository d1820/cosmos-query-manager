using System;
using System.Windows.Forms;
using CosmosManager.Domain;
using CosmosManager.Interfaces;
using System.Linq;
using CosmosManager.Presenters;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using static System.Windows.Forms.ListViewItem;

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

        }

        public void RenderResults(IReadOnlyCollection<object> results)
        {
            resultListView.Items.Clear();
            foreach (var item in results)
            {
                var fromObject = JObject.FromObject(item);
                var newList = new ListViewItem();
                var subItem = new ListViewSubItem
                {
                    Text = fromObject["id"]?.Value<string>()
                };
                newList.SubItems.Add(subItem);
                subItem = new ListViewSubItem
                {
                    Text = fromObject["PartitionKey"]?.Value<string>()
                };
                newList.SubItems.Add(subItem);
                resultListView.Items.Add(newList);
            }

        }

        private void Checkbox_CheckedChanged(object sender, EventArgs e) => throw new NotImplementedException();
    }
}
