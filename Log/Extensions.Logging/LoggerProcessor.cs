using BrassLoon.Interface.Log;
using BrassLoon.Interface.Log.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Account = BrassLoon.Interface.Account;

namespace BrassLoon.Extensions.Logging
{
    internal class LoggerProcessor : IDisposable
    {
        private const int MAX_QUEUE_LENGTH = 10240;
        private readonly Thread _outputThread;
        private readonly ITraceService _traceService;
        private readonly IExceptionService _exceptionService;
        private readonly Account.ITokenService _tokenService;
        private readonly IOptionsMonitor<LoggerConfiguration> _options;
        private bool _disposedValue;
        private Queue<LogMessageEntry> _logEntries;
        private bool _exit;        

        public LoggerProcessor(ITraceService traceService,
            IExceptionService exceptionService,
            Account.ITokenService tokenService,
            IOptionsMonitor<LoggerConfiguration> options)
        {
            _traceService = traceService;
            _exceptionService = exceptionService;
            _tokenService = tokenService;
            _options = options;
            _exit = false;
            _logEntries = new Queue<LogMessageEntry>();
            _outputThread = new Thread(ProcessQueue)
            {
                IsBackground = true,
                Name = "Log queue processor"
            };
            _outputThread.Start();
        }

        public void Enque(LogMessageEntry entry)
        {
            lock (_logEntries)
            {
                while (_logEntries.Count >= MAX_QUEUE_LENGTH && !_exit)
                {
                    Monitor.Wait(_logEntries);
                }
                if (!_exit)
                {
                    bool startedEmpty = _logEntries.Count == 0;
                    _logEntries.Enqueue(entry);
                    // if the queue was empty, the processor thread could be stuck waiting
                    if (startedEmpty)
                    {
                        Monitor.PulseAll(_logEntries);
                    }
                }
            }
        }

        private void ProcessQueue()
        {
            LogMessageEntry[] entries;
            while (TryDeque(out entries))
            {
                WriteMessages(entries);
            }
        }

        private bool TryDeque(out LogMessageEntry[] entries)
        {
            bool result = false;
            entries = new LogMessageEntry[0];
            lock (_logEntries)
            {
                while (_logEntries.Count == 0 && !_exit)
                {
                    Monitor.Wait(_logEntries);
                }
                if (_logEntries.Count > 0)
                {
                    entries = new LogMessageEntry[] { _logEntries.Dequeue() };
                    result = true;  
                    if (_logEntries.Count >= MAX_QUEUE_LENGTH - 1)
                    {
                        Monitor.PulseAll(_logEntries);
                    }
                }
            }            
            return result;
        }

        private void WriteMessages(in LogMessageEntry[] entries)
        {
            try
            {
                if (entries != null && entries.Length > 0)
                {
                    for (int i = 0; i < entries.Length; i += 1)
                    {
                        LoggerConfiguration configuration = _options.CurrentValue;
                        LogSettings settings = new LogSettings(_tokenService, configuration);
                        Trace trace = new Trace
                        {
                            DomainId = configuration.LogDomainId,
                            EventCode = entries[i].Category,
                            Message = entries[i].Message,
                            CreateTimestamp = entries[i].Timestamp
                        };
                        trace = _traceService.Create(settings, trace).Result;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Shutdown();
                    try
                    {
                        _outputThread.Join(5000);
                    }
                    catch (ThreadStateException) { }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Shutdown()
        {
            lock (_logEntries)
            {
                _exit = true;
                Monitor.PulseAll(_logEntries);
            }
            bool done = false;
            while (!done)
            {
                Thread.Sleep(250);
                lock(_logEntries)
                {
                    done = (_logEntries.Count == 0);
                }
            }
        }
    }
}
