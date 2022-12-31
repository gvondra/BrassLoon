﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Extensions.Logging
{
    internal sealed class Logger : ILogger
    {
        private readonly string _name;
        private readonly LoggerConfiguration _loggerConfiguration;
        private readonly MessageFormatter _messageFormatter;
        private readonly LoggerProcessor _loggerProcessor;

        public Logger(string name, 
            LoggerConfiguration loggerConfiguration,
            MessageFormatter messageFormatter,
            LoggerProcessor loggerProcessor)
        {
            _name = name;
            _loggerConfiguration = loggerConfiguration;
            _messageFormatter = messageFormatter;
            _loggerProcessor = loggerProcessor;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => (logLevel != LogLevel.None);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                StringBuilder stringBuilder;
                using (StringWriter writer = new StringWriter())
                {
                    LogEntry<TState> logEntry = new LogEntry<TState>(logLevel, _name, eventId, state, exception, formatter);
                    _messageFormatter.Write(logEntry, writer);
                    stringBuilder = writer.GetStringBuilder();
                }
                if (stringBuilder.Length > 0)
                {
                    _loggerProcessor.Enque(new LogMessageEntry(_name, stringBuilder.ToString(), DateTime.UtcNow));
                }                
            }
        }
    }
}
