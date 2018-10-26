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
using System.Text.RegularExpressions;
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
            ResetQueryOutput();
            //execute th interpretor and run against cosmos and connection
            if (SelectedConnection is Connection && SelectedConnection != null)
            {
                _view.SetStatusBarMessage("Executing Query...", true);

                var documentStore = _clientConnectionManager.CreateDocumentClientAndStore(SelectedConnection);

                //get each query and run it aggregating the results
                var queries = SplitQueries();

                //check all the queries for deletes without transactions
                if (queries.Any(a =>
                {
                    var query = _queryParser.Parse(a);
                    return query.QueryType == Constants.QueryKeywords.DELETE && !query.IsTransaction;
                }) && _view.ShowMessage("Are you sure you want to delete documents without a transaction. This can not be undone?", "Delete Document Confirmation", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                var hasResults = false;
                var hasError = false;
                for (var i = 0; i < queries.Length; i++)
                {
                    var query = queries[i];
                    var runner = _queryRunners.FirstOrDefault(f => f.CanRun(query));
                    if (runner != null)
                    {
                        var queryParts = _queryParser.Parse(query);
                        if (queries.Length > 1)
                        {
                            AddToQueryOutput(new string('-', 300));
                            AddToQueryOutput($"Query statement {i + 1}");
                            AddToQueryOutput(new string('-', 300));
                        }

                        var response = await runner.RunAsync(documentStore, SelectedConnection, query, true, _logger);
                        if (!response.success)
                        {
                            _view.ShowMessage($"Unable to execute query: {query}. Verify query and try again.", "Query Execution Error");
                            //on error stop loop and return
                            hasError = true;
                            break;
                        }
                        else if (response.results != null)
                        {
                            //add a header row if more then 1 query needs to be ran
                            RenderResults(response.results, queryParts.CollectionName, queries.Length > 1, i + 1);
                            hasResults = true;
                        }
                    }
                    else
                    {
                        _logger.LogError($"Unable to find a query processor for query type. query: {query}");
                        //on error stop loop and return
                        hasError = true;
                        break;
                    }

                }

                if (!hasResults || hasError)
                {
                    ShowOutputTab();
                }
                _view.SetStatusBarMessage("", false);
            }
            else
            {
                _view.ShowMessage("Invalid connection. Please select a valid connection and try again", "Data Connection Error");
            }
        }

        public async Task<object> SaveDocumentAsync(DocumentResult documentResult)
        {
            _view.SetStatusBarMessage("Saving Document...");
            var documentStore = _clientConnectionManager.CreateDocumentClientAndStore(SelectedConnection);

            try
            {
                var result = await documentStore.ExecuteAsync(SelectedConnection.Database, documentResult.CollectionName, context => context.UpdateAsync(documentResult.Document));
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

        public async Task<bool> DeleteDocumentAsync(DocumentResult documentResult)
        {
            _view.SetStatusBarMessage("Deleting Document...");
            var documentStore = _clientConnectionManager.CreateDocumentClientAndStore(SelectedConnection);

            try
            {
                var document = documentResult.Document;
                var partitionKeyPath = await documentStore.LookupPartitionKeyPath(SelectedConnection.Database, documentResult.CollectionName);
                var partionKeyValue = document.SelectToken(partitionKeyPath).ToString();
                var result = await documentStore.ExecuteAsync(SelectedConnection.Database, documentResult.CollectionName,
                       context => context.DeleteAsync(document[Constants.DocumentFields.ID].ToString(), new RequestOptions() { PartitionKey = partionKeyValue }));

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

        private string[] SplitQueries(string queryText = null)
        {
            var queryToParse = queryText ?? _view.Query;
            var pattern = @"(?!\B[""\'][^""\']*);(?![^""\']*[""\']\B)";
            var preCleanString = queryToParse.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ');
            var queries = Regex.Split(preCleanString, pattern, RegexOptions.IgnoreCase);
            return queries.Where(w => !string.IsNullOrEmpty(w.Trim())).ToArray();
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

        public void RenderResults(IReadOnlyCollection<object> results, string collectionName, bool appendResults, int queryStatementIndex)
        {
            _view.RenderResults(results, collectionName, appendResults, queryStatementIndex);
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

        public string BeautifyQuery(string queryText)
        {
            var cleanedQueries = new List<string>();
            try
            {
                var queries = SplitQueries(queryText);

                foreach (var query in queries)
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
                        cleanedQueries.Add(sql.ToString());
                        continue;
                    }

                    if (queryParts.IsRollback)
                    {
                        sql.AppendLine($"ROLLBACK {queryParts.RollbackName}");
                        cleanedQueries.Add(sql.ToString());
                        continue;
                    }

                    if (queryParts.IsValidQuery())
                    {
                        sql.AppendLine($"{queryParts.QueryType} {queryParts.QueryBody}");
                        sql.AppendLine($"{queryParts.QueryFrom}");
                        if (queryParts.HasWhereClause())
                        {
                            sql.AppendLine(queryParts.QueryWhere);
                        }

                        if (queryParts.IsUpdateQuery())
                        {
                            sql.AppendLine(queryParts.QueryUpdateType);
                            sql.AppendLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(queryParts.QueryUpdateBody), Formatting.Indented));
                            cleanedQueries.Add(sql.ToString());
                            continue;
                        }

                        if (queryParts.HasOrderByClause())
                        {
                            sql.AppendLine(queryParts.QueryOrderBy);
                        }
                        cleanedQueries.Add(sql.ToString());
                        continue;
                    }
                }

            }
            catch (Exception)
            {
                return queryText;
            }
            return string.Join($";{Environment.NewLine}{Environment.NewLine}", cleanedQueries);
            ;
        }
    }
}