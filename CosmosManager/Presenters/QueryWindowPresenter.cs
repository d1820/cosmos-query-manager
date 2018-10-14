using CosmosManager.Domain;
using CosmosManager.Interfaces;
using CosmosManager.Parsers;
using CosmosManager.QueryRunners;
using CosmosManager.Stores;
using CosmosManager.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
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

        private Dictionary<string, (IDocumentClient client, IDocumentStore store)> _clients = new Dictionary<string, (IDocumentClient, IDocumentStore)>();
        private IQueryWindowControl _view;
        private QueryOuputLogger _logger;
        private readonly QueryStatementParser _queryParser;
        private List<IQueryRunner> _queryRunners = new List<IQueryRunner>();

        public QueryWindowPresenter(IQueryWindowControl view, int tabIndexReference)
        {
            _view = view;
            view.Presenter = this;
            _logger = new QueryOuputLogger(this);
            var transactionTask = new TransactionTask();
            _queryParser = new QueryStatementParser();
            _queryRunners.Add(new SelectQueryRunner(this));
            _queryRunners.Add(new DeleteByIdQueryRunner(this, transactionTask));
            //_queryRunners.Add(new DeleteByWhereQueryRunner(this));
            _queryRunners.Add(new RollbackQueryRunner(this));

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
                _clients.Clear();
                var c = new List<object>();
                c.Add("Select Connection");
                c.AddRange(connections.ToArray());
                _view.ConnectionsList = c.ToArray();
            }
        }

        public void ResetQueryOutput()
        {
            _view.ResetQueryOutput();
        }

        public void AddToStatsLog(string message)
        {
            _view.AppendToQueryOutput(message + Environment.NewLine);
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
                _view.SetStatusBarMessage("Executing Query...");

                var documentStore = CreateDocumentClientAndStore();

                var runner = _queryRunners.FirstOrDefault(f => f.CanRun(_view.Query));
                if (runner != null)
                {
                    var queryParts = _queryParser.Parse(_view.Query);
                    if (queryParts.QueryType == Constants.QueryKeywords.DELETE &&
                        !queryParts.IsTransaction &&
                        _view.ShowMessage("Are you sure you want to delete these documents. This can not be undone?", "Delete Document Confirmation", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                    var didRun = await runner.RunAsync(documentStore, SelectedConnection, _view.Query, true, _logger);
                    if (!didRun)
                    {
                        _view.ShowMessage("Unable to execute query. Verify query and try again.", "Query Execution Error");
                    }
                }
                else
                {
                    _logger.LogError("Unable to find a query processor for query type");
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
            var documentStore = CreateDocumentClientAndStore();
            var parser = new QueryStatementParser();
            var parts = parser.Parse(_view.Query);

            try
            {
                var result = await documentStore.ExecuteAsync(SelectedConnection.Database, parts.CollectionName, context => context.UpdateAsync(document));
                _view.SetStatusBarMessage("Document Saved");
                return result;
            }
            catch (Exception ex)
            {
                _view.SetStatusBarMessage("Unable to save document");
                _view.ShowMessage(ex.Message, "Document Save Error", icon: System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public Task<string> LookupPartitionKeyPath()
        {
            var documentStore = CreateDocumentClientAndStore();
            var parser = new QueryStatementParser();
            var parts = parser.Parse(_view.Query);
            return documentStore.LookupPartitionKeyPath(SelectedConnection.Database, parts.CollectionName);
        }

        public async Task<bool> DeleteDocumentAsync(JObject document)
        {
            _view.SetStatusBarMessage("Deleting Document...");
            var documentStore = CreateDocumentClientAndStore();
            var parser = new QueryStatementParser();
            var parts = parser.Parse(_view.Query);

            try
            {
                var partitionKeyPath = await documentStore.LookupPartitionKeyPath(SelectedConnection.Database, parts.CollectionName);
                var partionKeyValue = document.SelectToken(partitionKeyPath).ToString();
                var result = await documentStore.ExecuteAsync(SelectedConnection.Database, parts.CollectionName,
                       context => context.DeleteAsync(document["id"].ToString(), new Domain.RequestOptions() { PartitionKey = partionKeyValue }));

                _view.SetStatusBarMessage("Document Deleted");
                _view.DocumentText = string.Empty;
                return result;
            }
            catch (Exception ex)
            {
                _view.SetStatusBarMessage("Unable to delete document");
                _view.ShowMessage(ex.Message, "Document Delete Error", icon: System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

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

        public void RenderResults(IReadOnlyCollection<object> results)
        {
            _view.RenderResults(results);
        }

        private IDocumentStore CreateDocumentClientAndStore()
        {
            if (!_clients.ContainsKey(SelectedConnection.EndPointUrl))
            {
                var client = new DocumentClient(new Uri(SelectedConnection.EndPointUrl), SelectedConnection.ConnectionKey);
                var documentStore = new CosmosDocumentStore(client);
                _clients.Add(SelectedConnection.EndPointUrl, (client, documentStore));
                return documentStore;
            }
            return _clients[SelectedConnection.EndPointUrl].store;
        }

        public void ShowOutputTab()
        {
            _view.ShowOutputTab();
        }
    }
}