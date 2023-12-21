using Microsoft.Extensions.Logging.Abstractions;
using System.IO;

namespace BrassLoon.Extensions.Logging
{
    internal sealed class MessageFormatter
    {
#pragma warning disable CA1822 // Mark members as static
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
#pragma warning restore CA1822 // Mark members as static
    }
}
