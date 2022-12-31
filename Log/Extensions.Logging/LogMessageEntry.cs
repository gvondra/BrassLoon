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
        public readonly DateTime _timestamp;

        public LogMessageEntry(
            string category,
            string message,
            DateTime timestamp)
        {
            _category = category;
            _message = message;
            _timestamp = timestamp;
        }

        public string Category => _category;
        public string Message => _message;
        public DateTime Timestamp => _timestamp;
    }
}
