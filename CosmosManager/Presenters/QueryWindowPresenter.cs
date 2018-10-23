using CosmosManager.Domain;
using CosmosManager.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosManager.Presenters
{
    public class QueryWindowPresenter : IQueryWindowPresenter
    {
        private List<Connection> _currentConnections;
        public Connection SelectedConnection { get; set; }
        public FileInfo CurrentFileInfo { get; private set; }
        public int TabIndexReference { get; private set; }

        private IQueryWindowControl _view;
        private IQueryWindowPresenterLogger _logger;
        private readonly IQueryStatementParser _queryParser;
        private IEnumerable<IQueryRunner> _queryRunners = new List<IQueryRunner>();
        private dynamic _context;
        private readonly IClientConnectionManager _clientConnectionManager;

        public QueryWindowPresenter(IClientConnectionManager clientConnectionManager,
                                    IQueryStatementParser queryStatementParser,
                                    IQueryWindowPresenterLogger logger,
                                    IEnumerable<IQueryRunner> queryRunners)
        {
            _logger = logger;
            _logger.SetPresenter(this);
            _queryParser = queryStatementParser;
            _queryRunners = queryRunners;
            _clientConnectionManager = clientConnectionManager;
        }

        public void InitializePresenter(dynamic context)
        {
            _context = context;
            _view = (IQueryWindowControl)context.QueryWindowControl;
            _view.Presenter = this;
            TabIndexReference = (int)context.TabIndexReference;
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
                _clientConnectionManager.Clear();
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

        public void AddToQueryOutput(string message)
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

        public async Task RunAsync()
        {
            _view.ResetResultsView();
            //execute th interpretor and run against cosmos and connection
            if (SelectedConnection is Connection && SelectedConnection != null)
            {
                _view.SetStatusBarMessage("Executing Query...", true);

                var documentStore = _clientConnectionManager.CreateDocumentClientAndStore(SelectedConnection);

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
                    ResetQueryOutput();
                    var response = await runner.RunAsync(documentStore, SelectedConnection, _view.Query, true, _logger);
                    if (!response.success)
                    {
                        _view.ShowMessage("Unable to execute query. Verify query and try again.", "Query Execution Error");
                        ShowOutputTab();
                    }
                    else if (response.results != null)
                    {
                        RenderResults(response.results);
                    }
                    else
                    {
                        ShowOutputTab();
                    }
                }
                else
                {
                    _logger.LogError("Unable to find a query processor for query type");
                    ShowOutputTab();
                }
                _view.SetStatusBarMessage("", false);
            }
            else
            {
                _view.ShowMessage("Invalid connection. Please select a valid connection and try again", "Data Connection Error");
            }
        }

        public async Task<object> SaveDocumentAsync(object document)
        {
            _view.SetStatusBarMessage("Saving Document...");
            var documentStore = _clientConnectionManager.CreateDocumentClientAndStore(SelectedConnection);

            var parts = _queryParser.Parse(_view.Query);

            try
            {
                var result = await documentStore.ExecuteAsync(SelectedConnection.Database, parts.CollectionName, context => context.UpdateAsync(document));
                _view.SetStatusBarMessage("Document Saved");
                _view.SetUpdatedResultDocument(result);
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
            var documentStore = _clientConnectionManager.CreateDocumentClientAndStore(SelectedConnection);
            var parts = _queryParser.Parse(_view.Query);
            return documentStore.LookupPartitionKeyPath(SelectedConnection.Database, parts.CollectionName);
        }

        public async Task<bool> DeleteDocumentAsync(JObject document)
        {
            _view.SetStatusBarMessage("Deleting Document...");
            var documentStore = _clientConnectionManager.CreateDocumentClientAndStore(SelectedConnection);
            var parts = _queryParser.Parse(_view.Query);

            try
            {
                var partitionKeyPath = await documentStore.LookupPartitionKeyPath(SelectedConnection.Database, parts.CollectionName);
                var partionKeyValue = document.SelectToken(partitionKeyPath).ToString();
                var result = await documentStore.ExecuteAsync(SelectedConnection.Database, parts.CollectionName,
                       context => context.DeleteAsync(document[Constants.DocumentFields.ID].ToString(), new Domain.RequestOptions() { PartitionKey = partionKeyValue }));

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

        public void ShowOutputTab()
        {
            _view.ShowOutputTab();
        }

        public string Beautify(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return data;
            }
            var obj = JObject.Parse(data);
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public string BeautifyQuery(string query)
        {
            try
            {
                var queryParts = _queryParser.Parse(query);
                var sql = new StringBuilder();
                if (queryParts.IsTransaction)
                {
                    sql.AppendLine(Constants.QueryKeywords.TRANSACTION);
                }
                if (queryParts.IsValidInsertQuery())
                {
                    sql.AppendLine(queryParts.QueryType);
                    sql.AppendLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(queryParts.QueryBody), Formatting.Indented));
                    sql.AppendLine(queryParts.QueryInto);
                    return sql.ToString();
                }

                if (queryParts.IsRollback)
                {
                    sql.AppendLine($"ROLLBACK {queryParts.RollbackName}");
                    return sql.ToString();
                }

                if (queryParts.IsValidQuery())
                {
                    if (queryParts.IsUpdateQuery())
                    {
                        sql.AppendLine($"{queryParts.QueryType} {queryParts.QueryBody}");
                        sql.AppendLine($"{queryParts.QueryFrom}");
                        if (queryParts.HasWhereClause())
                        {
                            sql.AppendLine(queryParts.QueryWhere);
                        }
                        sql.AppendLine(queryParts.QueryUpdateType);
                        sql.AppendLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(queryParts.QueryUpdateBody), Formatting.Indented));
                        return sql.ToString();
                    }
                    sql.AppendLine($"{queryParts.QueryType} {queryParts.QueryBody}");
                    sql.AppendLine($"{queryParts.QueryFrom}");
                    if (queryParts.HasWhereClause())
                    {
                        sql.AppendLine(queryParts.QueryWhere);
                    }
                    return sql.ToString();
                }
            }
            catch (Exception)
            {
            }
            return query;
        }
    }
}