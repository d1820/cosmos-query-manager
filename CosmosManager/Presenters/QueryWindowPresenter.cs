using ConsoleTables;
using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using CosmosManager.Managers;
using CosmosManager.Parsers;
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
    public abstract class BaseQueryPresenter
    {
        protected readonly IQueryStatementParser _queryParser;
        protected IClientConnectionManager _clientConnectionManager;
        protected Dictionary<string, IReadOnlyCollection<object>> _variables = new Dictionary<string, IReadOnlyCollection<object>>();
        public Connection SelectedConnection { get; set; }

        public BaseQueryPresenter(IQueryStatementParser queryStatementParser)
        {
            _queryParser = queryStatementParser;
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

        public (string header1, string header2) LookupResultListViewHeaders(object item, string textPartitionKeyPath)
        {
            if (item == null)
            {
                return (null, null);
            }
            var fromObject = JObject.FromObject(item);

            JProperty col1Prop = null;
            var resultProps = fromObject.Properties();
            col1Prop = resultProps.FirstOrDefault(f => f.Name == Constants.DocumentFields.ID);
            if (col1Prop == null)
            {
                col1Prop = resultProps.FirstOrDefault();
            }

            JProperty col2Prop = null;
            if (resultProps.Count() > 1)
            {

                col2Prop = resultProps.FirstOrDefault(f => f.Name == textPartitionKeyPath);
                if (col2Prop == null)
                {
                    var prop = resultProps.FirstOrDefault(f => f != col1Prop);
                    if (prop != null)
                    {
                        col2Prop = prop;

                    }
                }

            }
            return (col1Prop?.Name, col2Prop?.Name);
        }

        public Task<string> LookupPartitionKeyPath(string collectionName)
        {
            var documentStore = _clientConnectionManager.CreateDocumentClientAndStore(SelectedConnection);
            return documentStore.LookupPartitionKeyPath(SelectedConnection.Database, collectionName);
        }

        public abstract void RenderResults(IReadOnlyCollection<object> results, string collectionName, QueryParts query, bool appendResults, int queryStatementIndex);
        public abstract void SetConnections(List<Connection> connections);
        public abstract void AddToQueryOutput(string message);
        public abstract void InitializePresenter(dynamic context);
    }
    public class CommandlinePresenter : BaseQueryPresenter, ICommandlinePresenter
    {
        private readonly IQueryManager _queryManager;
        private List<Connection> _currentConnections;
        private IQueryPresenterLogger _logger;
        private CommandlineOptions _options;
        private StreamWriter _sw;
        private readonly IEnumerable<IQueryRunner> _queryRunners = new List<IQueryRunner>();

        public CommandlinePresenter(IClientConnectionManager clientConnectionManager,
                                    IQueryStatementParser queryStatementParser,
                                    IQueryPresenterLogger logger,
                                    IEnumerable<IQueryRunner> queryRunners,
                                    IQueryManager queryManager) : base(queryStatementParser)
        {
            _logger = logger;
            _logger.SetPresenter(this);
            _queryRunners = queryRunners;
            _clientConnectionManager = clientConnectionManager;
            _queryManager = queryManager;
        }

        public override void AddToQueryOutput(string message)
        {
            Console.WriteLine(message);
            if (_sw != null)
            {
                _sw.Write(message);
            }
        }

        private int _totalDocumentCount;
        private int _groupCount;
        private ConsoleTable _table;
        private ConsoleTable _summaryTable;
        private Dictionary<string, List<string>> _documentsByGroup;

        public override void InitializePresenter(dynamic context)
        {
            _totalDocumentCount = 0;
            _groupCount = 0;
            _table = new ConsoleTable();
            _summaryTable = new ConsoleTable();
            _summaryTable.AddColumn(new string[] { "Query Id", "Result Count", "Collection", "Query" });
            _documentsByGroup = new Dictionary<string, List<string>>();
        }

        public override async void RenderResults(IReadOnlyCollection<object> results, string collectionName, QueryParts query, bool appendResults, int queryStatementIndex)
        {
            if (!appendResults)
            {
                _totalDocumentCount = 0;
            }
            var textPartitionKeyPath = await LookupPartitionKeyPath(query.CollectionName);
            var column1Header = "";
            var column2Header = "";
            var groupHeader = "";

            //table.AddRow(1, 2, 3)
            //     .AddRow("this line should be longer", "yes it is", "oh");
            _summaryTable.AddRow(queryStatementIndex, results.Count, collectionName, query);
            groupHeader = $"Query {queryStatementIndex} ({results.Count} Documents)";
            var headers = LookupResultListViewHeaders(results.FirstOrDefault(), textPartitionKeyPath);
            if (appendResults)
            {
                if (_groupCount == 1)
                {
                    //first group set headers
                    column1Header = headers.header1;
                    column2Header = headers.header2;
                }
                else
                {
                    //if the next query has a different select, then clear column headers
                    if (column1Header != headers.header1)
                    {
                        column1Header = string.Empty;
                    }
                    if (column2Header != headers.header2)
                    {
                        column2Header = string.Empty;
                    }
                }
            }
            else
            {
                column1Header = headers.header1;
                column2Header = headers.header2;
            }
            if (_table.Columns.Count != 2)
            {
                _table.AddColumn(new List<string> { column1Header, column2Header });
            }
            else
            {
                //reset headers
                _table.Columns[0] = column1Header;
                _table.Columns[1] = column2Header;
            }
            if (!string.IsNullOrEmpty(groupHeader))
            {
                _table.AddRow(groupHeader);
            }
            if (!_documentsByGroup.ContainsKey(groupHeader))
            {
                _documentsByGroup.Add(groupHeader, new List<string>());
            }


            var validResultCount = 0;
            foreach (var item in results)
            {
                var fromObject = JObject.FromObject(item);
                if (!fromObject.HasValues)
                {
                    continue;
                }
                _documentsByGroup[groupHeader].Add(fromObject.ToString());

                validResultCount++;
                JProperty col1Prop = null;
                JToken col1Token = null;
                var resultProps = fromObject.Properties();
                var col1RowText = string.Empty;
                var col2RowText = string.Empty;

                if (resultProps.Count() > 0)
                {
                    col1Prop = resultProps.FirstOrDefault(f => f.Name == Constants.DocumentFields.ID);
                    if (col1Prop == null)
                    {
                        col1Prop = resultProps.FirstOrDefault();
                    }
                    col1Token = col1Prop?.Value;
                    if (col1Token != null)
                    {
                        col1RowText = col1Token.Type.IsPrimitiveType() ? col1Token?.ToStringValue() : col1Token?.GetObjectValue(Constants.DocumentFields.ID);
                    }
                }

                if (resultProps.Count() > 1)
                {
                    JProperty col2Prop = null;
                    JToken col2Token = null;

                    col2Prop = resultProps.FirstOrDefault(f => f.Name == textPartitionKeyPath);
                    if (col2Prop == null)
                    {
                        var prop = resultProps.FirstOrDefault(f => f != col1Prop);
                        if (prop != null)
                        {
                            col2Prop = prop;
                        }
                    }
                    col2Token = col2Prop?.Value;
                    if (col2Token != null)
                    {
                        col2RowText = col2Token.Type.IsPrimitiveType() ? col2Token?.ToStringValue() : col2Token?.GetObjectValue("");
                    }
                }
                _table.AddRow(new string[] { col1RowText, col2RowText });
            }
            _totalDocumentCount += validResultCount;
        }
        public async Task<int> RunAsync(string query, CommandlineOptions options, CancellationToken cancelToken)
        {
            _options = options;
            if (string.IsNullOrEmpty(_options.OutputPath))
            {
                _sw = new StreamWriter(_options.OutputPath, true);
            }

            try
            {
                //execute th interpretor and run against cosmos and connection
                if (SelectedConnection is Connection && SelectedConnection != null)
                {
                    _variables.Clear();

                    var documentStore = _clientConnectionManager.CreateDocumentClientAndStore(SelectedConnection);
                    //get each query and run it aggregating the results
                    var queries = _queryManager.ConveryQueryTextToQueryParts(query);

                    var hasNonTransactionDelete = queries.Any(q => q.CleanQueryType == Constants.QueryParsingKeywords.DELETE && !q.IsTransaction);
                    //check all the queries for deletes without transactions
                    if (hasNonTransactionDelete && !options.IgnorePrompts)
                    {
                        Console.WriteLine("Are you sure you want to delete documents without a transaction. This can not be undone? (Y/N): ");
                        if (Console.ReadLine() == "N")
                        {
                            return -99;
                        }
                    }

                    var hasResults = false;
                    var hasError = false;
                    //_source = new CancellationTokenSource();
                    for (var i = 0; i < queries.Length; i++)
                    {
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

                            var response = await runner.RunAsync(documentStore, SelectedConnection, queryParts, true, _logger, cancelToken, _variables);
                            if (!response.success)
                            {
                                //on error stop loop and return
                                hasError = true;
                                if (!options.ContinueOnError)
                                {
                                    break;
                                }
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
                                if (!options.ContinueOnError)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (hasError && !options.ContinueOnError)
                    {
                        //make sure process exits with non zero
                        return -999;
                    }
                }
                else
                {
                    _logger.LogError("Invalid connection. Please select a valid connection and try again", "Data Connection Error");
                    return -99;
                }
                return 0;//success
            }
            finally
            {
                await _sw.FlushAsync();
                _sw.Close();
            }
        }
        public override void SetConnections(List<Connection> connections)
        {
            _currentConnections = connections;
            if (connections != null)
            {
                _clientConnectionManager.Clear();
            }
        }

        public void RenderTables()
        {
            //output to console and file
        }
    }
    public class QueryWindowPresenter : BaseQueryPresenter, IQueryWindowPresenter
    {
        static ManualResetEventSlim mres = new ManualResetEventSlim(false);
        private readonly IQueryManager _queryManager;

        private List<Connection> _currentConnections;

        public FileInfo CurrentFileInfo { get; private set; }
        public int TabIndexReference { get; private set; }

        private IQueryWindowControl _view;
        private IQueryPresenterLogger _logger;

        private IEnumerable<IQueryRunner> _queryRunners = new List<IQueryRunner>();

        private IPubSub _pubsub;

        private CancellationTokenSource _source;

        public QueryWindowPresenter(IClientConnectionManager clientConnectionManager,
                                    IQueryStatementParser queryStatementParser,
                                    IQueryPresenterLogger logger,
                                    IEnumerable<IQueryRunner> queryRunners,
                                    IQueryManager queryManager) : base(queryStatementParser)
        {
            _logger = logger;
            _logger.SetPresenter(this);
            _queryRunners = queryRunners;
            _clientConnectionManager = clientConnectionManager;
            _queryManager = queryManager;
        }

        public override void InitializePresenter(dynamic context)
        {
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

        public override void SetConnections(List<Connection> connections)
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

        public override void AddToQueryOutput(string message)
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
                var queries = _queryManager.ConveryQueryTextToQueryParts(_view.Query);

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

        public override void RenderResults(IReadOnlyCollection<object> results, string collectionName, QueryParts query, bool appendResults, int queryStatementIndex)
        {
            _view.RenderResults(results, collectionName, query, appendResults, queryStatementIndex);
        }

        public void ShowOutputTab()
        {
            _view.ShowOutputTab();
        }

        public void Dispose()
        {
            _pubsub.Unsubscribe(this, Constants.SubscriptionTypes.THEME_CHANGE);
        }
    }
}