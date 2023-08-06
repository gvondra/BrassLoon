﻿using BrassLoon.Interface.Log;
using BrassLoon.Interface.Log.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Account = BrassLoon.Interface.Account;
using LogModels = BrassLoon.Interface.Log.Models;

namespace BrassLoon.Extensions.Logging
{
    internal class LoggerProcessor : IDisposable
    {
        private const int MAX_QUEUE_LENGTH = 10240;
        private readonly Thread _outputThread;
        private readonly ITraceService _traceService;
        private readonly IExceptionService _exceptionService;
        private readonly IMetricService _metricService;
        private readonly Account.ITokenService _tokenService;
        private readonly IOptionsMonitor<LoggerConfiguration> _options;
        private bool _disposedValue;
        private Queue<LogMessageEntry> _logEntries;
        private bool _exit; 

        public LoggerProcessor(ITraceService traceService,
            IExceptionService exceptionService,
            IMetricService metricService,
            Account.ITokenService tokenService,
            IOptionsMonitor<LoggerConfiguration> options)
        {
            _traceService = traceService;
            _exceptionService = exceptionService;
            _metricService = metricService;
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

        private Task StartWriteMessage(LogMessageEntry[] entries)
        {
            return Task.Run(() => WriteMessages(in entries));
        }

        private void ProcessQueue()
        {
            LogMessageEntry[] entries;
            Queue<Task> tasks = new Queue<Task>();
            while (TryDeque(out entries))
            {
                tasks.Enqueue(StartWriteMessage(entries));
                while (tasks.Count >= 4)
                {
                    tasks.Dequeue().Wait();
                }
            }
            Task.WaitAll(tasks.ToArray());
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
                if (_logEntries.Count > 16)
                {
                    entries = new LogMessageEntry[Math.Min(_logEntries.Count, 256)];
                    for (int i = 0; i < entries.Length; i += 1)
                    {
                        entries[i] = _logEntries.Dequeue(); 
                    }
                    result = true;
                }
                else if (_logEntries.Count > 0)
                {
                    entries = new LogMessageEntry[] { _logEntries.Dequeue() };
                    result = true;                      
                }
                if (result)
                {
                    if (_logEntries.Count >= MAX_QUEUE_LENGTH - entries.Length)
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
                    LoggerConfiguration configuration = _options.CurrentValue;
                    LogSettings settings = new LogSettings(_tokenService, configuration);
                    List<Trace> traces = new List<Trace>();
                    List<LogModels.Metric> metrics = new List<LogModels.Metric>();
                    for (int i = 0; i < entries.Length; i += 1)
                    {
                        if (!string.IsNullOrEmpty(entries[i].Message) && entries[i].Metric == null)
                        {
                            traces.Add(CreateLogTrace(configuration.LogDomainId, entries[i]));
                        }
                        else if (entries[i].Metric != null)
                        {
                            metrics.Add(CreateLogMetric(configuration.LogDomainId, entries[i]));
                        }
                        else if (entries[i].Exception != null)
                        {
                            _exceptionService.Create(
                                settings,
                                configuration.LogDomainId,
                                entries[i].Exception,
                                createTimestamp: entries[i].Timestamp,
                                category: entries[i].Category,
                                level: entries[i].LogLevel.ToString(),
                                eventId: (entries[i].EventId.Id != default(int) || !string.IsNullOrEmpty(entries[i].EventId.Name)) ? new LogModels.EventId { Id = entries[i].EventId.Id, Name = entries[i].EventId.Name } : default(LogModels.EventId)
                                ).Wait();
                        }
                    }
                    //if (traces.Count > 0) 
                    //    SubmitTraces(traces).Wait();  
                    if (traces.Count > 0)
                        _traceService.Create(settings, configuration.LogDomainId, traces).Wait();
                    if (metrics.Count > 0)
                        _metricService.Create(settings, configuration.LogDomainId, metrics).Wait();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private LogModels.Metric CreateLogMetric(Guid domainId, LogMessageEntry entry)
        {
            return new LogModels.Metric
            {
                DomainId = domainId,
                EventCode = entry.Metric.EventCode,
                Magnitude = entry.Metric.Magnitude,
                Status = entry.Metric.Status,
                Category = entry.Category,
                Level = entry.LogLevel.ToString(),
                Requestor = entry.Metric.Requestor,
                EventId = (entry.EventId.Id != default(int) || !string.IsNullOrEmpty(entry.EventId.Name)) ? new LogModels.EventId { Id = entry.EventId.Id, Name = entry.EventId.Name } : default(LogModels.EventId?)
            };
        }

        private async Task SubmitTraces(List<Trace> traces)
        {   
            if (traces.Count > 0)
            {
                LoggerConfiguration configuration = _options.CurrentValue;
                using (GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:5001"))
                {
                    LogRPC.Protos.TraceService.TraceServiceClient client = new LogRPC.Protos.TraceService.TraceServiceClient(channel);
                    Grpc.Core.AsyncClientStreamingCall<LogRPC.Protos.Trace, Google.Protobuf.WellKnownTypes.Empty> call = client.Create();
                    foreach (Trace trace in traces)
                    {
                        await call.RequestStream.WriteAsync(
                            new LogRPC.Protos.Trace
                            {
                                Category = trace.Category,
                                //Data = trace.Data,
                                DomainId = configuration.LogDomainId.ToString("D"),
                                EventCode = trace.EventCode,
                                Level = trace.Level,
                                //EventId = 
                                Message = trace.Message
                            });
                    }
                    await call.RequestStream.CompleteAsync();
                    await call;
                }
            }
        }

        private LogModels.Trace CreateLogTrace(Guid domainId, LogMessageEntry entry)
        {
            return new LogModels.Trace
            {
                DomainId = domainId,
                EventCode = entry.Category,
                Message = entry.Message,
                Category = entry.Category,
                Level = entry.LogLevel.ToString(),
                EventId = (entry.EventId.Id != default(int) || !string.IsNullOrEmpty(entry.EventId.Name)) ? new LogModels.EventId { Id = entry.EventId.Id, Name = entry.EventId.Name } : default(LogModels.EventId?)
            };
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
                        _outputThread.Join(60000);
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
        }
    }
}
