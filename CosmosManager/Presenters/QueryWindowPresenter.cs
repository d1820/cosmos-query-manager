using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using CosmosManager.Parsers;
using CosmosManager.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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
        private IPubSub _pubsub;

        private Dictionary<string, IReadOnlyCollection<object>> _variables = new Dictionary<string, IReadOnlyCollection<object>>();
        private CancellationTokenSource _source;

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
            _pubsub = context.PubSub;
            _pubsub.Subscribe(this, Constants.SubscriptionTypes.THEME_CHANGE);
            TabIndexReference = (int)context.TabIndexReference;
        }

        public void Receive(object sender, PubSubEventArgs e, int messageId)
        {
            if (messageId == Constants.SubscriptionTypes.THEME_CHANGE)
            {
                _view.RenderTheme();
            }
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

        static ManualResetEventSlim mres = new ManualResetEventSlim(false);

        public void StopQuery()
        {
            if (!_source.IsCancellationRequested)
            {
                _view.SetStatusBarMessage("Stopping Query...", true);
                _source.Cancel();
                try
                {
                    mres.Wait(_source.Token);
                }
                catch (OperationCanceledException)
                {
                    _view.SetStatusBarMessage("Ready");
                }
            }
        }

        public async Task RunAsync()
        {
            _source = new CancellationTokenSource();
            _view.ResetResultsView();
            ResetQueryOutput();

            //execute th interpretor and run against cosmos and connection
            if (SelectedConnection is Connection && SelectedConnection != null)
            {
                _variables.Clear();
                _view.SetStatusBarMessage("Executing Query...", true);

                var documentStore = _clientConnectionManager.CreateDocumentClientAndStore(SelectedConnection);
                //get each query and run it aggregating the results
                var queries = ConveryQueryTextToQueryParts(_view.Query);

                //check all the queries for deletes without transactions
                if (queries.Any(query => query.CleanQueryType == Constants.QueryParsingKeywords.DELETE && !query.IsTransaction) && _view.ShowMessage("Are you sure you want to delete documents without a transaction. This can not be undone?", "Delete Document Confirmation", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                var hasResults = false;
                var hasError = false;
                for (var i = 0; i < queries.Length; i++)
                {
                    if (_source.Token.IsCancellationRequested)
                    {
                        _logger.LogError($"Query has been requested to cancel.");
                        break;
                    }
                    var queryParts = queries[i];
                    var runner = _queryRunners.FirstOrDefault(f => f.CanRun(queryParts));
                    if (runner != null)
                    {
                        if (queries.Length > 1)
                        {
                            AddToQueryOutput(new string('-', 300));
                            AddToQueryOutput($"Query statement {i + 1}");
                            AddToQueryOutput(new string('-', 300));
                        }

                        var response = await runner.RunAsync(documentStore, SelectedConnection, queryParts, true, _logger, _source.Token, _variables);
                        if (!response.success)
                        {
                            _view.ShowMessage($"Unable to execute query: {queryParts.CleanOrginalQuery.TruncateTo(500)}. Verify query and try again.", "Query Execution Error");
                            //on error stop loop and return
                            hasError = true;
                            break;
                        }
                        else if (response.results != null)
                        {
                            //add a header row if more then 1 query needs to be ran
                            RenderResults(response.results, queryParts.CollectionName, queryParts, queries.Length > 1, i + 1);
                            hasResults = true;
                        }
                    }
                    else
                    {
                        //if we have comments then we can assume the whole query is a comment so skip and goto next
                        if (!queryParts.IsCommentOnly)
                        {
                            _logger.LogError($"Unable to find a query processor for query type. query: {queryParts.CleanOrginalQuery}");
                            //on error stop loop and return
                            hasError = true;
                            break;
                        }
                    }

                }

                if (!hasResults || hasError)
                {
                    ShowOutputTab();
                }
                _view.SetStatusBarMessage("Ready");
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
                var source = new CancellationTokenSource();
                var result = await documentStore.ExecuteAsync(SelectedConnection.Database, documentResult.CollectionName, context => context.UpdateAsync(documentResult.Document), source.Token);
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

        public Task<string> LookupPartitionKeyPath(string collectionName)
        {
            var documentStore = _clientConnectionManager.CreateDocumentClientAndStore(SelectedConnection);
            return documentStore.LookupPartitionKeyPath(SelectedConnection.Database, collectionName);
        }

        public async Task<bool> DeleteDocumentAsync(DocumentResult documentResult)
        {
            _view.SetStatusBarMessage("Deleting Document...");
            var documentStore = _clientConnectionManager.CreateDocumentClientAndStore(SelectedConnection);

            try
            {
                var source = new CancellationTokenSource();
                var document = documentResult.Document;
                var partitionKeyPath = await documentStore.LookupPartitionKeyPath(SelectedConnection.Database, documentResult.CollectionName);
                var partionKeyValue = document.SelectToken(partitionKeyPath).ToString();
                var result = await documentStore.ExecuteAsync(SelectedConnection.Database, documentResult.CollectionName,
                       context => context.DeleteAsync(document[Constants.DocumentFields.ID].ToString(), new RequestOptions() { PartitionKey = partionKeyValue }), source.Token);

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

        public void RenderResults(IReadOnlyCollection<object> results, string collectionName, QueryParts query, bool appendResults, int queryStatementIndex)
        {
            _view.RenderResults(results, collectionName, query, appendResults, queryStatementIndex);
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
                var jsonTokenizer = new JsonTokenizer();
                var commentTokenizer = new CommentTokenizer();

                var preCleanString = queryText;
                preCleanString = commentTokenizer.TokenizeComments(preCleanString);

                if (preCleanString.EndsWith(";"))
                {
                    preCleanString = preCleanString.Remove(preCleanString.Length - 1, 1);
                }
                //splits on semi-colon
                var pattern = $@"\s*;\s*[{Constants.NEWLINE}](?!\s*\*\/)";
                var queries = Regex.Split(preCleanString, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

                //this removes empty lines
                var filteredQueries = queries.Where(w => !string.IsNullOrEmpty(w.Trim().Replace(Constants.NEWLINE, "")));


                var startMatchesAt = 0;
                var commentedQueries = new List<string>();
                foreach (var q in filteredQueries)
                {
                    var commentedQuery = q;
                    while (commentedQuery.IndexOf(commentTokenizer.TOKEN) > -1)
                    {
                        commentedQuery = commentTokenizer.DetokenizeCommentsAt(commentedQuery, startMatchesAt);
                        startMatchesAt++;
                    }
                    commentedQueries.Add(commentedQuery);
                }

                foreach (var query in commentedQueries)
                {
                    var trimmedQuery = query;
                    trimmedQuery = _queryParser.CleanAndFormatQueryText(trimmedQuery, true, true, true);
                    var formattedQuery = trimmedQuery.Replace(Constants.NEWLINE, Environment.NewLine);
                    if (queries.Count() > 1)
                    {
                        formattedQuery += ";";
                    }
                    cleanedQueries.Add(formattedQuery);
                }
            }
            catch (Exception)
            {
                return queryText;
            }
            return string.Join($"{Environment.NewLine}{Environment.NewLine}", cleanedQueries);
        }

        private QueryParts[] ConveryQueryTextToQueryParts(string queryToParse)
        {
            if (string.IsNullOrEmpty(queryToParse))
            {
                return new QueryParts[0];
            }
            var queries = SplitQueryText(queryToParse);
            return queries.Select(s => _queryParser.Parse(s)).Where(w => !w.IsCommentOnly).ToArray();
        }

        private IEnumerable<string> SplitQueryText(string queryToParse)
        {
            //remove all the comments then split
            var preCleanString = _queryParser.RemoveComments(queryToParse).Trim();

            if (preCleanString.EndsWith(";"))
            {
                preCleanString = preCleanString.Remove(preCleanString.Length - 1, 1);
            }
            //splits on semi-colon
            var pattern = $@"\s*;\s*[{Constants.NEWLINE}](?!\s*\*\/)";
            var queries = Regex.Split(preCleanString, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            //this removes empty lines
            return queries.Where(w => !string.IsNullOrEmpty(w.Trim().Replace(Constants.NEWLINE, "")));
        }

        public void Dispose()
        {
            _pubsub.Unsubscribe(this, Constants.SubscriptionTypes.THEME_CHANGE);
        }
    }
}