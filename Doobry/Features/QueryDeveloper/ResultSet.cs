using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Azure.Documents;

namespace Doobry.Features.QueryDeveloper
{
    public class ResultSet
    {
        private readonly ObservableCollection<Result> _results;

        public ResultSet(IEnumerable<Result> results)
        {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            _results = new ObservableCollection<Result>(results);
            Results = new ReadOnlyObservableCollection<Result>(_results);
        }

        public ResultSet(string error)
        {
            Error = error;
        }

        public ResultSet(DocumentClientException documentClientException)
        {
            if (documentClientException == null)
                throw new ArgumentNullException(nameof(documentClientException));

            Error =
                    $"Error: {documentClientException.Error}{Environment.NewLine}Message: {documentClientException.Message}{Environment.NewLine}Status Code: {documentClientException.StatusCode}{Environment.NewLine}";
        }

        public string Error { get; }

        public double Cost { get; set; }

        public ReadOnlyObservableCollection<Result> Results { get; }

        public void Append(dynamic results)
        {
            foreach (var result in results)
                _results.Add(new Result(Results.Count, result));
        }
    }
}