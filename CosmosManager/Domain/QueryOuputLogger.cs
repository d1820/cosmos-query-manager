using CosmosManager.Interfaces;
using CosmosManager.Presenters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CosmosManager.Domain
{
    public class QueryOuputLogger : IQueryPresenterLogger
    {
        private BaseQueryPresenter _presenter;

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (exception != null)
            {
                var logObject = new
                {
                    logLevel,
                    eventId,
                    state,
                    errorMessage = exception.GetBaseException().Message,
                    details = exception.StackTrace
                };

                _presenter?.AddToQueryOutput(JsonConvert.SerializeObject(logObject, Formatting.Indented));
            }
            else
            {
                if (state == null)
                {
                    return;
                }
                if (state is IReadOnlyList<KeyValuePair<string, object>> values)
                {
                    foreach (var stat in values)
                    {
                        var parts = stat.Value?.ToString().Split(new[] { ',' });

                        _presenter?.AddToQueryOutput(JsonConvert.SerializeObject(parts, Formatting.Indented));
                    }
                }
                else
                {
                    var parts = state.ToString().Split(new[] { ',' });

                    _presenter?.AddToQueryOutput(JsonConvert.SerializeObject(parts, Formatting.Indented));
                }
            }
        }

        public void SetPresenter(BaseQueryPresenter presenter)
        {
            _presenter = presenter;
        }
    }
}