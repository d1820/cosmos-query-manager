using ConsoleTables;
using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using CosmosManager.Managers;
using McMaster.Extensions.CommandLineUtils;
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
        private ITextWriter _sw;
        private readonly IEnumerable<IQueryRunner> _queryRunners = new List<IQueryRunner>();
        private int _groupCount;
        private OutputTraceInformation _outputTraceInformation;
        private readonly IConsoleLogger _console;
        private readonly ITextWriter _textWriter;

        public CommandlinePresenter(IClientConnectionManager clientConnectionManager,
                                    IQueryStatementParser queryStatementParser,
                                    IQueryPresenterLogger logger,
                                    IEnumerable<IQueryRunner> queryRunners,
                                    IQueryManager queryManager,
                                    IConsoleLogger console,
                                    ITextWriter textWriter) : base(queryStatementParser)
        {
            _logger = logger;
            _logger.SetPresenter(this);
            _queryRunners = queryRunners;
            _clientConnectionManager = clientConnectionManager;
            _queryManager = queryManager;
            _console = console;
            _textWriter = textWriter;
        }

        public override void AddToQueryOutput(string message, bool includeTrailingLine = true)
        {
            _console.WriteLine(message);
            if (includeTrailingLine)
            {
                _console.WriteLine("");
            }
            AddToOutputStream(message, includeTrailingLine);
        }

        public override void InitializePresenter(dynamic context)
        {
            _groupCount = 0;
            _options = context.Options;
            if (_options.WriteToOutput)
            {
                _textWriter.Open(_options.OutputPath, true);
            }
        }

        public override async Task RenderResults(IReadOnlyCollection<object> results, string collectionName, QueryParts query, bool appendResults, int queryStatementIndex)
        {
            var textPartitionKeyPath = await LookupPartitionKeyPath(query.CollectionName);
            var column1Header = "";
            var column2Header = "";
            var queryHeader = "";


            queryHeader = $"Query {queryStatementIndex} ({results.Count} Documents)";
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

            _outputTraceInformation.OutputColumn1 = column1Header;
            _outputTraceInformation.OutputColumn2 = column2Header;
            var outputDetailRecord = new OutputDetailRecord { QueryHeader = queryHeader };
            _outputTraceInformation.OutputDetailRecords.Add(outputDetailRecord);

            foreach (var item in results)
            {
                var fromObject = JObject.FromObject(item);
                if (!fromObject.HasValues)
                {
                    continue;
                }
                var documentDetail = new DocumentDetail { Document = fromObject.ToString() };

                outputDetailRecord.Records.Add(documentDetail);

                var columnText = fromObject.Properties().ParseColumnText(textPartitionKeyPath);
                documentDetail.DisplayField1 = columnText.col1RowText;
                documentDetail.DisplayField2 = columnText.col2RowText;
            }
        }

        public void WriteHeader(char outlineChar, int width, string message)
        {
            AddToQueryOutput(new string(outlineChar, width), false);
            var cap = new string(outlineChar, 2);
            AddToQueryOutput($"{cap} {message.PadRight(width - 5, ' ')}{cap}", false);
            AddToQueryOutput(new string(outlineChar, width));
        }

        public async Task<int> RunAsync(string queryGroupName, string query, CancellationToken cancelToken)
        {
            _outputTraceInformation = new OutputTraceInformation();

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
                    _console.WriteLine("Are you sure you want to delete documents without a transaction. This can not be undone? (Y/N): ");
                    if (_console.ReadLine() == "N")
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
                        var queryIndex = i + 1;
                        if (queries.Length > 1)
                        {
                            WriteHeader('/', 250, $"{queryGroupName} - Query statement {queryIndex}");
                        }

                        var response = await runner.RunAsync(documentStore, SelectedConnection, queryParts, true, _logger, cancelToken, _variables);

                        var querySummaryRecord = new OutputSummaryRecord
                        {
                            CollectionName = queryParts.CollectionName,
                            Query = queryParts.ToRawQuery(),
                            QueryStatementIndex = queryIndex
                        };
                        _outputTraceInformation.OutputSummaryRecords.Add(querySummaryRecord);
                        if (!response.success)
                        {
                            //on error stop loop and return
                            querySummaryRecord.HasError = hasError = true;
                            if (!_options.ContinueOnError)
                            {
                                break;
                            }
                        }
                        else if (response.results != null)
                        {
                            querySummaryRecord.ResultCount = response.results.Count;
                            //add a header row if more then 1 query needs to be ran
                            await RenderResults(response.results, queryParts.CollectionName, queryParts, queries.Length > 1, queryIndex);
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

                await WriteResults();

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

        public void Dispose()
        {
            _textWriter.Close();
        }

        private Task WriteResults()
        {
            //if we get an error from a script and have no results then dont render all the headers and sections, cause an error will be displayed
            if (_outputTraceInformation.OutputSummaryRecords.Any())
            {
                var summaryTable = new ConsoleTable(new ConsoleTableOptions { EnableCount = false });
                summaryTable.AddColumn(new string[] { "Query Id", "Result Count", "Collection", "Query", "HasError" });
                _outputTraceInformation.OutputSummaryRecords.ForEach(f => summaryTable.AddRow(f.QueryStatementIndex, f.ResultCount, f.CollectionName, f.Query, f.HasError ? "Y" : ""));
                //output to console and file
                WriteHeader('=', 200, "Execution Summary");
                summaryTable.Write();
                AddToOutputStream(summaryTable.ToString());
                AddToQueryOutput($"Total Records Returned: {_outputTraceInformation.OutputSummaryRecords.Sum(s => s.ResultCount)}");
            }

            if (_outputTraceInformation.OutputDetailRecords.Any())
            {
                var table = new ConsoleTable(new ConsoleTableOptions { EnableCount = false });
                table.AddColumn(new List<string> { _outputTraceInformation.OutputColumn1, _outputTraceInformation.OutputColumn2 });
                WriteHeader('=', 200, "Execution Results");
                foreach (var detailRecord in _outputTraceInformation.OutputDetailRecords)
                {
                    table.AddRow(detailRecord.QueryHeader, "");
                    detailRecord.Records.ForEach(rec => table.AddRow(rec.DisplayField1, rec.DisplayField2));
                }
                table.Write();
                AddToOutputStream(table.ToString());
                if (_options.IncludeDocumentInOutput)
                {
                    foreach (var detailRecord in _outputTraceInformation.OutputDetailRecords)
                    {
                        WriteHeader('=', 200, $"Documents for Query {detailRecord.QueryHeader}");
                        detailRecord.Records.ForEach(rec => AddToQueryOutput(rec.Document));
                        AddToQueryOutput("");
                    }
                }
            }
            return Task.CompletedTask;
        }

        private void AddToOutputStream(string message, bool includeTrailingLine = true)
        {
            if (_textWriter != null)
            {
                _textWriter.WriteLine(message + (includeTrailingLine ? Environment.NewLine : string.Empty));
            }
        }

    }
}