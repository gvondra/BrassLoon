using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Extensions.Logging
{
    internal readonly struct LogMessageEntry
    {
        private readonly string _category;
        private readonly string _message;
        private readonly DateTime _timestamp;
        private readonly EventId _eventId;
        private readonly Exception _exception;
        private readonly LogLevel _logLevel;
        private readonly Metric _metric;

        public LogMessageEntry(
            string category,
            string message,
            DateTime timestamp,
            EventId eventId,
            Exception exception,
            LogLevel logLevel,
            Metric metric)
        {
            _category = category;
            _message = message;
            _timestamp = timestamp;
            _eventId = eventId;
            _exception = exception;
            _logLevel = logLevel;
            _metric = metric;
        }

        public string Category => _category;
        public string Message => _message;
        public DateTime Timestamp => _timestamp;
        public EventId EventId => _eventId;
        public Exception Exception => _exception;
        public LogLevel LogLevel => _logLevel;
        public Metric Metric => _metric;
    }
}
