using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Extensions.Logging
{
    internal class MessageFormatter
    {
        public void Write<TState>(in LogEntry<TState> logEntry, TextWriter textWriter)
        {
            string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            textWriter.Write(logEntry.LogLevel.ToString());
            textWriter.Write(" ");
            if (!string.IsNullOrEmpty(message))
            {
                textWriter.Write(message.TrimEnd());
            }
            else if (logEntry.Exception != null)
            {
                textWriter.Write(logEntry.Exception.Message?.TrimEnd());
            }
        }
    }
}
