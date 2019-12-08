using ConsoleTables;
using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using CosmosManager.Managers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosManager.Presenters
{
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
            AddToOutputStream(message);
        }

        private void AddToOutputStream(string message)
        {
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

            _options = context.Options;
            if (_options.WriteToOutput)
            {
                _sw = new StreamWriter(_options.OutputPath, true);
            }
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
            _summaryTable.AddRow(queryStatementIndex, results.Count, collectionName, query.ToRawQuery());
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
                _table.AddRow(groupHeader, "");
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

        public async Task<int> RunAsync(string query, CancellationToken cancelToken)
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
                if (hasNonTransactionDelete && !_options.IgnorePrompts)
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
                            if (!_options.ContinueOnError)
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
                            if (!_options.ContinueOnError)
                            {
                                break;
                            }
                        }
                    }
                }
                if (hasError && !_options.ContinueOnError)
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

        public override void SetConnections(List<Connection> connections)
        {
            _currentConnections = connections;
            if (connections != null)
            {
                _clientConnectionManager.Clear();
            }
        }

        public async Task WriteToOutput()
        {
            try
            {
                //output to console and file
                AddToQueryOutput("==========================================================");
                AddToQueryOutput("Execution Summary");
                AddToQueryOutput("==========================================================");
                _summaryTable.Write();
                AddToOutputStream(_summaryTable.ToString());
                AddToQueryOutput("");

                AddToQueryOutput("==========================================================");
                AddToQueryOutput("Results");
                AddToQueryOutput("==========================================================");
                _table.Write();
                AddToOutputStream(_table.ToString());
                AddToQueryOutput("");

                if (_options.IncludeDocumentInOutput)
                {
                    foreach (var item in _documentsByGroup)
                    {
                        AddToQueryOutput("==========================================================");
                        AddToQueryOutput($"Documents for Query {item.Key}");
                        AddToQueryOutput("==========================================================");
                        foreach (var str in item.Value)
                        {
                            AddToQueryOutput(str);
                        }
                        AddToQueryOutput("");
                    }
                }
            }
            finally
            {
                await _sw.FlushAsync();
                _sw.Close();
            }
        }

        public void Dispose()
        {
            if (_sw != null && _sw.BaseStream != null)
            {
                _sw.Flush();
                _sw.Close();
            }
        }
    }
}