using BrassLoon.Interface.Log;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
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
                    List<LogModels.Trace> traces = new List<LogModels.Trace>();
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
                        if (entries[i].Exception != null)
                        {
                            //_exceptionService.Create(
                            //    settings,
                            //    configuration.LogDomainId,
                            //    entries[i].Exception,
                            //    createTimestamp: entries[i].Timestamp,
                            //    category: entries[i].Category,
                            //    level: entries[i].LogLevel.ToString(),
                            //    eventId: (entries[i].EventId.Id != default(int) || !string.IsNullOrEmpty(entries[i].EventId.Name)) ? new LogModels.EventId { Id = entries[i].EventId.Id, Name = entries[i].EventId.Name } : default(LogModels.EventId)
                            //    ).Wait();
                            SubmitExcepiton(
                                entries[i].Exception,
                                entries[i].Timestamp,
                                entries[i].Category,
                                entries[i].LogLevel.ToString(),
                                entries[i].EventId).Wait();
                        }
                    }
                    if (traces.Count > 0) 
                        SubmitTraces(traces).Wait();
                    //if (traces.Count > 0)
                    //    _traceService.Create(settings, configuration.LogDomainId, traces).Wait();
                    //if (metrics.Count > 0)
                    //    _metricService.Create(settings, configuration.LogDomainId, metrics).Wait();
                    if (metrics.Count > 0)
                        SubmitMetrics(metrics).Wait();
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

        private async Task SubmitExcepiton(
            System.Exception exception,
            DateTime? timestamp,
            string category,
            string level,
            EventId? eventId)
        {
            using (GrpcChannel channel = GrpcChannel.ForAddress(_options.CurrentValue.LogApiBaseAddress))
            {
                Metadata headers = new Metadata()
                    {
                        { "Authorization", string.Format("Bearer {0}", await GetAccessToken(channel)) }
                    };
                LogRPC.Protos.ExceptionService.ExceptionServiceClient client = new LogRPC.Protos.ExceptionService.ExceptionServiceClient(channel);
                await client.CreateAsync(Map(exception, timestamp, category, level, eventId), headers: headers);
            }
        }

        private LogRPC.Protos.LogException Map(
            System.Exception exception,
            DateTime? timestamp,
            string category,
            string level,
            EventId? eventId)
        {
            Google.Protobuf.WellKnownTypes.Timestamp timestampConverted = timestamp.HasValue ? Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(timestamp.Value) : null;
            LogRPC.Protos.LogException logException = new LogRPC.Protos.LogException();
            logException.AppDomain = AppDomain.CurrentDomain.FriendlyName;
            logException.Category = category;
            logException.CreateTimestamp = timestampConverted;
            MapExcpetionData(logException.Data, exception.Data);
            logException.DomainId = _options.CurrentValue.LogDomainId.ToString("D");
            logException.EventId = eventId.HasValue && eventId.Value.Id != default(int) && !string.IsNullOrEmpty(eventId.Value.Name) ? new LogRPC.Protos.EventId { Id = eventId.Value.Id, Name = eventId.Value.Name} : null;
            logException.Level = level;
            logException.Message = exception.Message;
            logException.Source = exception.Source;
            logException.StackTrace = exception.StackTrace;
            logException.TargetSite = exception.TargetSite?.ToString() ?? string.Empty;
            logException.TypeName = exception.GetType().FullName;
            if (exception.InnerException != null)
                logException.InnerException = Map(exception.InnerException, timestamp, category, level, eventId);
            return logException;
        }

        private void MapExcpetionData(Google.Protobuf.Collections.MapField<string, string> map, IDictionary exceptionData)
        {
            if (exceptionData != null)
            {
                IDictionaryEnumerator enumerator = exceptionData.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    map.Add((enumerator.Key ?? string.Empty).ToString(), enumerator.Value?.ToString() ?? string.Empty);
                }
            }
        }

        private async Task SubmitTraces(List<LogModels.Trace> traces)
        {
            if (traces.Count > 0)
            {
                using (GrpcChannel channel = GrpcChannel.ForAddress(_options.CurrentValue.LogApiBaseAddress))
                {
                    Metadata headers = new Metadata()
                    {
                        { "Authorization", string.Format("Bearer {0}", await GetAccessToken(channel)) }
                    };
                    LogRPC.Protos.TraceService.TraceServiceClient client = new LogRPC.Protos.TraceService.TraceServiceClient(channel);
                    Grpc.Core.AsyncClientStreamingCall<LogRPC.Protos.Trace, Google.Protobuf.WellKnownTypes.Empty> call = client.Create(headers);
                    foreach (LogModels.Trace trace in traces)
                    {
                        await call.RequestStream.WriteAsync(
                            new LogRPC.Protos.Trace
                            {
                                Category = trace.Category,
                                //Data = trace.Data,
                                DomainId = _options.CurrentValue.LogDomainId.ToString("D"),
                                EventCode = trace.EventCode,
                                Level = trace.Level,
                                EventId = trace.EventId.HasValue ? new LogRPC.Protos.EventId { Id = trace.EventId.Value.Id, Name = trace.EventId.Value.Name } : null,
                                Message = trace.Message
                            });
                    }
                    await call.RequestStream.CompleteAsync();
                    await call;
                }
            }
        }

        private async Task SubmitMetrics(List<LogModels.Metric> metrics)
        {
            if (metrics.Count > 0)
            {
                using (GrpcChannel channel = GrpcChannel.ForAddress(_options.CurrentValue.LogApiBaseAddress))
                {
                    Metadata headers = new Metadata()
                    {
                        { "Authorization", string.Format("Bearer {0}", await GetAccessToken(channel)) }
                    };
                    LogRPC.Protos.MetricService.MetricServiceClient client = new LogRPC.Protos.MetricService.MetricServiceClient(channel);
                    Grpc.Core.AsyncClientStreamingCall<LogRPC.Protos.Metric, Google.Protobuf.WellKnownTypes.Empty> call = client.Create(headers);
                    foreach (LogModels.Metric metric in metrics)
                    {
                        Google.Protobuf.WellKnownTypes.Timestamp createTimestamp = metric.CreateTimestamp.HasValue ? Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(metric.CreateTimestamp.Value) : null;
                        await call.RequestStream.WriteAsync(
                            new LogRPC.Protos.Metric
                            {
                                Category = metric.Category,
                                CreateTimestamp = createTimestamp,
                                //Data = 
                                DomainId = _options.CurrentValue.LogDomainId.ToString("D"),
                                EventCode = metric.EventCode,
                                EventId = metric.EventId.HasValue ? new LogRPC.Protos.EventId { Id = metric.EventId.Value.Id, Name = metric.EventId.Value.Name } : null,
                                Level = metric.Level,
                                Magnitude = metric.Magnitude,
                                Requestor = metric.Requestor,
                                Status = metric.Status
                            });
                    }
                    await call.RequestStream.CompleteAsync();
                    await call;
                }
            }
        }

        private async Task<string> GetAccessToken(GrpcChannel channel)
        {
            LogRPC.Protos.TokenService.TokenServiceClient client = new LogRPC.Protos.TokenService.TokenServiceClient(channel);
            LogRPC.Protos.TokenRequest request = new LogRPC.Protos.TokenRequest
            {
                ClientId = _options.CurrentValue.LogClientId.ToString("D"),
                Secret = _options.CurrentValue.LogClientSecret
            };
            LogRPC.Protos.Token token = await client.CreateAsync(request, new CallOptions());
            return token.Value;
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
