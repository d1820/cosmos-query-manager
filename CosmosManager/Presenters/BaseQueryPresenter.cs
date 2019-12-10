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

        public abstract Task RenderResults(IReadOnlyCollection<object> results, string collectionName, QueryParts query, bool appendResults, int queryStatementIndex);

        public abstract void SetConnections(List<Connection> connections);

        public abstract void AddToQueryOutput(string message, bool includeTrailingLine = true);

        public abstract void InitializePresenter(dynamic context);
    }
}