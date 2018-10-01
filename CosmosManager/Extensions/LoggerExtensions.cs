using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using System;

namespace CosmosManager.Extensions
{
    public static class LoggerExtensions
    {
        private static readonly Func<object, Exception, string> _messageFormatter = (o, exception) => o.ToString();



        public static void Log(this ILogger logger, LogLevel logLevel, EventId eventId, string message, Exception exception = null)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            logger.Log(logLevel, eventId, (object) new FormattedLogValues(message), exception, _messageFormatter);
        }

        public static void Log(this ILogger logger, LogLevel logLevel, string message, Exception exception = null)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            logger.Log(logLevel, new EventId(), (object) new FormattedLogValues(message), exception, _messageFormatter);
        }

        //NOTE [dturco 7.18.2018] these were created because of a bug in App Insights and ILogger integration where exceptions and messages are not being logged correctly
        //https://github.com/Azure/azure-functions-host/issues/2556
        public static void LogException(this ILogger logger, EventId eventId, Exception exception, string message)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            logger.Log(LogLevel.Error, eventId, (object) new FormattedLogValues(exception.Message), exception, _messageFormatter);
            logger.Log(LogLevel.Error, eventId, (object) new FormattedLogValues(message), (Exception)null, _messageFormatter);
        }

        public static void LogCriticalException(this ILogger logger, EventId eventId, Exception exception, string message)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            logger.Log(LogLevel.Critical, eventId, (object) new FormattedLogValues(exception.Message), exception, _messageFormatter);
            logger.Log(LogLevel.Critical, eventId, (object) new FormattedLogValues(message), (Exception)null, _messageFormatter);
        }

        public static void LogWarningException(this ILogger logger, EventId eventId, Exception exception, string message)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            logger.Log(LogLevel.Warning, eventId, (object) new FormattedLogValues(exception.Message), exception, _messageFormatter);
            logger.Log(LogLevel.Warning, eventId, (object) new FormattedLogValues(message), (Exception)null, _messageFormatter);
        }
    }
}
