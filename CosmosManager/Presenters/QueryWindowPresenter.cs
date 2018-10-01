using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using CosmosManager.QueryRunners;
using CosmosManager.Stores;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosManager.Presenters
{

    public class QueryWindowPresenter : IResultsPresenter
    {
        private List<Connection> _currentConnections;
        public Connection SelectedConnection { get; set; }
        public FileInfo CurrentFileInfo { get; private set; }
        public int TabIndexReference { get; }

        private IDocumentClient _client;
        private IDocumentStore _documentStore;
        private IQueryWindowControl _view;
        private StatsLogger _logger;
        private List<IQueryRunner> _queryRunners = new List<IQueryRunner>();

        public QueryWindowPresenter(IQueryWindowControl view, int tabIndexReference)
        {
            _view = view;
            view.Presenter = this;
            _logger = new StatsLogger(view);
            _queryRunners.Add(new SelectQueryRunner(this));
            TabIndexReference = tabIndexReference;
        }

        public string CurrentTabQuery
        {
            get
            {
                return _view.Query;
            }
        }

        public void SetConnections(List<Connection> connections)
        {
            _currentConnections = connections;
            if (connections != null)
            {
                _client = null;
                var c = new List<object>();
                c.Add("Select Connection");
                c.AddRange(connections.ToArray());
                _view.ConnectionsList = c.ToArray();
            }
        }

        public void SetFile(FileInfo fileInfo)
        {
            if (fileInfo == null)
            {
                return;
            }
            CurrentFileInfo = fileInfo;
            _view.Query = File.ReadAllText(fileInfo.FullName);
        }

        public void SetTempQuery(string query)
        {
            _view.Query = query;
        }

        public async void Run()
        {
            _view.ResetResultsView();
            //execute th interpretor and run against cosmos and connection
            if (SelectedConnection is Connection && SelectedConnection != null)
            {
                _view.ToggleStatsPanel(true);
                _view.SetStatusBarMessage("Executing Query...");

                CreateDocumentClientAndStore();
                //TODO  RUN PARSER
                var query = CleanQuery(_view.Query);
                var collectionName = ParseCollectionName(query);
                var queryType = ParseQueryType(query);

                var runner = _queryRunners.FirstOrDefault(f => f.CanRun(queryType));
                if (runner != null)
                {
                    var didRun = await runner.RunAsync(_documentStore, SelectedConnection.Database, collectionName, query, true, _logger);
                    if (!didRun)
                    {
                        _view.ShowMessage("Unable to execute query. Verify query and try again.", "Query Execution Error");
                    }
                }
                _view.SetStatusBarMessage("");
            }
            else
            {
                _view.ShowMessage("Invalid connection. Please select a valid connection and try again", "Data Connection Error");
            }
        }

        public async Task<object> SaveDocumentAsync(object document)
        {
            _view.SetStatusBarMessage("Saving Document...");
            CreateDocumentClientAndStore();
            var query = CleanQuery(_view.Query);
            var collectionName = ParseCollectionName(query);
            var result =  await _documentStore.ExecuteAsync(SelectedConnection.Database, collectionName, context => context.UpdateAsync(document));
            _view.SetStatusBarMessage("Document Saved");
            return result;
        }

        public async Task SaveQueryAsync()
        {
            using (var sw = new StreamWriter(CurrentFileInfo.FullName))
            {
                await sw.WriteAsync(_view.Query);
            }
            _view.SetStatusBarMessage($"{CurrentFileInfo.Name} Saved");
        }

        public async Task SaveTempQueryAsync(string fileName)
        {
            using (var sw = new StreamWriter(fileName))
            {
                await sw.WriteAsync(_view.Query);
            }
            _view.SetStatusBarMessage($"{fileName} Saved");
        }

        public async Task ExportDocumentAsync(string fileName)
        {
            _view.SetStatusBarMessage("Exporting Document...");
            using (var sw = new StreamWriter(fileName))
            {
                await sw.WriteAsync(_view.DocumentText);
            }
            _view.SetStatusBarMessage($"{fileName} Exported");
        }

        public async Task ExportAllToDocumentAsync(List<JObject> documents, string fileName)
        {
            _view.SetStatusBarMessage("Exporting documents...");

            using (var sw = new StreamWriter(fileName))
            {
                await sw.WriteAsync(JsonConvert.SerializeObject(documents, Formatting.Indented));
            }

            _view.SetStatusBarMessage($"{fileName} Exported");
        }

        public string GetCurrentQueryCollectionName()
        {
            var query = CleanQuery(_view.Query);
            return ParseCollectionName(query);
        }

        public void RenderResults(IReadOnlyCollection<object> results)
        {
            _view.RenderResults(results);
        }

        private string CleanQuery(string query)
        {
            return query
                .Replace("from", "FROM")
                .Replace("From", "FROM")
                .Replace("select", "SELECT")
                .Replace("Select", "SELECT")
                .Replace("set", "SET")
                .Replace("Set", "SET")
                .Replace("update", "UPDATE")
                .Replace("Update", "UPDATE");
        }

        private string ParseCollectionName(string query)
        {
            query = query.Replace("from", "FROM").Replace("select", "SELECT").Replace("update", "UPDATE");
            var parts = query.Trim().Split(new string[] { "FROM" }, StringSplitOptions.None);
            var collectionName = parts[1].Trim().Split(new[] { ' ' }).FirstOrDefault();
            return collectionName;
        }

        private string ParseQueryType(string query)
        {
            var parts = query.Trim().Split(new[] { ' ' });
            return parts[0];
        }

        private void CreateDocumentClientAndStore()
        {
            if (_client == null)
            {
                _client = new DocumentClient(new Uri(SelectedConnection.EndPointUrl), SelectedConnection.ConnectionKey);
                _documentStore = new CosmosDocumentStore(_client);
            }
        }
    }
}
