using System;
using CosmosManager.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Newtonsoft.Json;

namespace CosmosManager.Domain
{

    public class StatsLogger : ILogger
    {
        private readonly IQueryWindowControl _view;

        public StatsLogger(IQueryWindowControl view)
        {
            _view = view;
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

                _view.Stats =  JsonConvert.SerializeObject(logObject, Formatting.Indented);
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

                    _view.Stats += JsonConvert.SerializeObject(parts, Formatting.Indented) + Environment.NewLine;
                }
            }

            _view.ToggleStatsPanel(false);
        }
    }
}
