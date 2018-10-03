using CosmosManager.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Newtonsoft.Json;
using System;

namespace CosmosManager.Domain
{
    public class StatsLogger : ILogger
    {
        private readonly IResultsPresenter _presenter;

        public StatsLogger(IResultsPresenter presenter)
        {
            _presenter = presenter;
        }

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
                    errorMessage = exception != null ? exception.GetBaseException().Message : ""
                };

                _presenter.AddToStatsLog(JsonConvert.SerializeObject(logObject, Formatting.Indented));
            }
            else
            {
                if (state == null)
                {
                    return;
                }
                foreach (var stat in state as FormattedLogValues)
                {
                    var parts = stat.Value?.ToString().Split(new[] { ',' });

                    _presenter.AddToStatsLog(JsonConvert.SerializeObject(parts, Formatting.Indented) + Environment.NewLine);
                }
            }

            _presenter.ToggleStatsPanel(false);
        }
    }
}