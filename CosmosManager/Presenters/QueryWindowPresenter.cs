using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using CosmosManager.QueryRunners;
using CosmosManager.Stores;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CosmosManager.Presenters
{
    public class QueryWindowPresenter
    {
        private List<Connection> _currentConnections;
        public Connection SelectedConnection { get; set; }
        private IDocumentClient _client;
        private IDocumentStore _documentStore;
        private IQueryWindowControl _view;
        private StatsLogger _logger;
        private List<IQueryRunner> _queryRunners = new List<IQueryRunner>();

        public QueryWindowPresenter(IQueryWindowControl view)
        {
            _view = view;
            view.Presenter = this;
            _logger = new StatsLogger(view);
            _queryRunners.Add(new SelectQueryRunner(view));

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
            _view.Query = File.ReadAllText(fileInfo.FullName);
        }

        public async void Run()
        {
            //execute th interpretor and run against cosmos and connection
            if (SelectedConnection is Connection && SelectedConnection != null)
            {
                _view.ToggleStatsPanel(true);


                if (_client == null)
                {
                    _client = new DocumentClient(new Uri(SelectedConnection.EndPointUrl), SelectedConnection.ConnectionKey);
                    _documentStore = new CosmosDocumentStore(_client);
                }
                //TODO  RUN PARSER

                var collectionName = "Marketplace";
                var queryType = ParseQueryType(_view.Query);

                var runner = _queryRunners.FirstOrDefault(f=>f.CanRun(queryType));
                if(runner != null)
                {
                    var didRun  = await runner.RunAsync(_documentStore, SelectedConnection.Database, collectionName, _view.Query, _logger);
                    if (!didRun)
                    {
                        _view.ShowMessage("Unable to execute query. Verify query and try again.", "Query Execution Error");
                    }
                }
            }
            else
            {
                _view.ShowMessage("Invalid connection. Please select a valid connection and try again", "Data Connection Error");
            }
        }

        public void SaveSelectedRecord(string fileName)
        {

        }

        public void SetCurrentRecordInView(object record)
        {

        }

        private string ParseQueryType(string query)
        {
            var parts = query.Trim().Split(new [] { ' '});
            return parts[0];
        }
    }
}
