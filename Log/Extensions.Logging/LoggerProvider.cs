using BrassLoon.Interface.Log;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Account = BrassLoon.Interface.Account;

namespace BrassLoon.Extensions.Logging
{
    [ProviderAlias("BrassLoonLog")]
    internal sealed class LoggerProvider : ILoggerProvider
    {
        private readonly IOptionsMonitor<LoggerConfiguration> _options;
        private readonly MessageFormatter _messageFormatter;
        private readonly LoggerProcessor _loggerProcessor;

        public LoggerProvider(ITraceService traceService,
            IExceptionService exceptionService,
            Account.ITokenService tokenService,
            IOptionsMonitor<LoggerConfiguration> options, 
            MessageFormatter messageFormatter)
        {
            _options = options;
            _messageFormatter = messageFormatter;
            _loggerProcessor = new LoggerProcessor(traceService, exceptionService, tokenService, options);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(categoryName, _options.CurrentValue, _messageFormatter, _loggerProcessor);
        }

        public void Dispose() 
        {
            _loggerProcessor.Dispose();
        }
    }
}
