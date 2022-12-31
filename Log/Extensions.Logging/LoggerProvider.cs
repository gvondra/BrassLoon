using BrassLoon.Interface.Log;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Account = BrassLoon.Interface.Account;

namespace BrassLoon.Extensions.Logging
{
    [ProviderAlias("BrassLoonLog")]
    internal sealed class LoggerProvider : ILoggerProvider
    {
        private readonly IOptionsMonitor<LoggerConfiguration> _options;
        private readonly MessageFormatter _messageFormatter;
        private readonly LoggerProcessor _loggerProcessor;
        private readonly ConcurrentDictionary<string, Logger> _loggers;

        public LoggerProvider(ITraceService traceService,
            IExceptionService exceptionService,
            Account.ITokenService tokenService,
            IOptionsMonitor<LoggerConfiguration> options, 
            MessageFormatter messageFormatter)
        {
            _options = options;
            _loggers = new ConcurrentDictionary<string, Logger>();
            _messageFormatter = messageFormatter;
            _loggerProcessor = new LoggerProcessor(traceService, exceptionService, tokenService, options);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.TryGetValue(categoryName, out Logger logger) ? logger :
                _loggers.GetOrAdd(categoryName, new Logger(categoryName, _options.CurrentValue, _messageFormatter, _loggerProcessor));
        }
        public void Dispose() 
        {
            _loggerProcessor.Dispose();
        }
    }
}
