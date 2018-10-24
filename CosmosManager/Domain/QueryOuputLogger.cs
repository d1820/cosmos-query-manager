using CosmosManager.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Newtonsoft.Json;
using System;

namespace CosmosManager.Domain
{
    public class QueryOuputLogger : IQueryWindowPresenterLogger
    {
        private IQueryWindowPresenter _presenter;

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

                _presenter.AddToQueryOutput(JsonConvert.SerializeObject(logObject, Formatting.Indented));
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

                    _presenter.AddToQueryOutput(JsonConvert.SerializeObject(parts, Formatting.Indented));
                }
            }
        }

        public void SetPresenter(IQueryWindowPresenter presenter)
        {
            _presenter = presenter;
        }
    }
}