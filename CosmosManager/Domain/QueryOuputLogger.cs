using CosmosManager.Interfaces;
using CosmosManager.Presenters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
namespace CosmosManager.Domain
{
    public class QueryOuputLogger : IQueryPresenterLogger
    {
        private BaseQueryPresenter _presenter;
        private ILogger<QueryOuputLogger> _logger;

        public QueryOuputLogger(ILogger<QueryOuputLogger> logger)
        {
            _logger = logger;
        }

        public IDisposable BeginScope<TState>(TState state) => _logger.BeginScope<TState>(state);

        public bool IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);

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
                var message = formatter(state, exception);
                var parts = message.Split(new[] { ',' });
                _presenter?.AddToQueryOutput(JsonConvert.SerializeObject(parts, Formatting.Indented));
            }
        }

        public void SetPresenter(BaseQueryPresenter presenter)
        {
            _presenter = presenter;
        }
    }
}