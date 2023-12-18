using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace BrassLoon.Extensions.Logging
{
    internal class LoggerProcessor : IDisposable
    {
        private const int MAX_QUEUE_LENGTH = 10240;
        private readonly Thread _outputThread;
        private readonly IOptionsMonitor<LoggerConfiguration> _options;
        private readonly IAccessTokenFactory _accessTokenFactory;
        private bool _disposedValue;
        private readonly Queue<LogMessageEntry> _logEntries;
        private bool _exit;

        public LoggerProcessor(
            IOptionsMonitor<LoggerConfiguration> options,
            IAccessTokenFactory accessTokenFactory)
        {
            _options = options;
            _exit = false;
            _logEntries = new Queue<LogMessageEntry>();
            _outputThread = new Thread(ProcessQueue)
            {
                IsBackground = true,
                Name = "Log queue processor"
            };
            _outputThread.Start();
            _accessTokenFactory = accessTokenFactory;
        }

        public void Enque(LogMessageEntry entry)
        {
            lock (_logEntries)
            {
                while (_logEntries.Count >= MAX_QUEUE_LENGTH && !_exit)
                {
                    _ = Monitor.Wait(_logEntries);
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

        private Task StartWriteMessage(LogMessageEntry[] entries) => Task.Run(() => WriteMessages(in entries));

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
            entries = Array.Empty<LogMessageEntry>();
            lock (_logEntries)
            {
                while (_logEntries.Count == 0 && !_exit)
                {
                    _ = Monitor.Wait(_logEntries);
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
                if (result && _logEntries.Count >= MAX_QUEUE_LENGTH - entries.Length)
                {
                    Monitor.PulseAll(_logEntries);
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
                    List<LogMessageEntry> traces = new List<LogMessageEntry>();
                    List<LogMessageEntry> metrics = new List<LogMessageEntry>();
                    for (int i = 0; i < entries.Length; i += 1)
                    {
                        if (!string.IsNullOrEmpty(entries[i].Message) && entries[i].Metric == null)
                        {
                            traces.Add(entries[i]);
                        }
                        else if (entries[i].Metric != null)
                        {
                            metrics.Add(entries[i]);
                        }
                        if (entries[i].Exception != null)
                        {
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
                    if (metrics.Count > 0)
                        SubmitMetrics(metrics).Wait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task SubmitExcepiton(
            Exception exception,
            DateTime? timestamp,
            string category,
            string level,
            EventId? eventId)
        {
            using (GrpcChannel channel = GrpcChannel.ForAddress(_options.CurrentValue.LogApiBaseAddress))
            {
                Metadata headers = new Metadata()
                    {
                        { "Authorization", string.Format(CultureInfo.InvariantCulture, "Bearer {0}", await _accessTokenFactory.GetAccessToken(_options.CurrentValue, channel)) }
                    };
                LogRPC.Protos.ExceptionService.ExceptionServiceClient client = new LogRPC.Protos.ExceptionService.ExceptionServiceClient(channel);
                _ = await client.CreateAsync(Map(exception, timestamp, category, level, eventId), headers: headers);
            }
        }

        private LogRPC.Protos.LogException Map(
            Exception exception,
            DateTime? timestamp,
            string category,
            string level,
            EventId? eventId)
        {
            Timestamp timestampConverted = timestamp.HasValue ? Timestamp.FromDateTime(timestamp.Value) : null;
            LogRPC.Protos.LogException logException = new LogRPC.Protos.LogException();
            logException.AppDomain = AppDomain.CurrentDomain.FriendlyName;
            logException.Category = category;
            logException.CreateTimestamp = timestampConverted;
            MapExcpetionData(logException.Data, exception.Data);
            logException.DomainId = _options.CurrentValue.LogDomainId.ToString("D");
            logException.EventId = eventId.HasValue && eventId.Value.Id != default(int) && !string.IsNullOrEmpty(eventId.Value.Name) ? new LogRPC.Protos.EventId { Id = eventId.Value.Id, Name = eventId.Value.Name } : null;
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

        private static void MapExcpetionData(MapField<string, string> map, IDictionary exceptionData)
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

        private async Task SubmitTraces(List<LogMessageEntry> entries)
        {
            if (entries.Count > 0)
            {
                using (GrpcChannel channel = GrpcChannel.ForAddress(_options.CurrentValue.LogApiBaseAddress))
                {
                    Metadata headers = new Metadata()
                    {
                        { "Authorization", string.Format(CultureInfo.InvariantCulture, "Bearer {0}", await _accessTokenFactory.GetAccessToken(_options.CurrentValue, channel)) }
                    };
                    LogRPC.Protos.TraceService.TraceServiceClient client = new LogRPC.Protos.TraceService.TraceServiceClient(channel);
                    AsyncClientStreamingCall<LogRPC.Protos.Trace, Empty> call = client.Create(headers);
                    foreach (LogMessageEntry entry in entries)
                    {
                        await call.RequestStream.WriteAsync(
                            new LogRPC.Protos.Trace
                            {
                                Category = entry.Category,
                                DomainId = _options.CurrentValue.LogDomainId.ToString("D"),
                                EventCode = entry.Category,
                                Level = entry.LogLevel.ToString(),
                                EventId = entry.EventId.Id != default(int) && !string.IsNullOrEmpty(entry.EventId.Name) ? new LogRPC.Protos.EventId { Id = entry.EventId.Id, Name = entry.EventId.Name } : null,
                                Message = entry.Message
                            });
                    }
                    await call.RequestStream.CompleteAsync();
                    _ = await call;
                }
            }
        }

        private async Task SubmitMetrics(List<LogMessageEntry> entries)
        {
            if (entries.Count > 0)
            {
                using (GrpcChannel channel = GrpcChannel.ForAddress(_options.CurrentValue.LogApiBaseAddress))
                {
                    Metadata headers = new Metadata()
                    {
                        { "Authorization", string.Format(CultureInfo.InvariantCulture, "Bearer {0}", await _accessTokenFactory.GetAccessToken(_options.CurrentValue, channel)) }
                    };
                    LogRPC.Protos.MetricService.MetricServiceClient client = new LogRPC.Protos.MetricService.MetricServiceClient(channel);
                    AsyncClientStreamingCall<LogRPC.Protos.Metric, Empty> call = client.Create(headers);
                    foreach (LogMessageEntry entry in entries)
                    {
                        Metric metric = entry.Metric;
                        LogRPC.Protos.Metric request = new LogRPC.Protos.Metric
                        {
                            Category = entry.Category,
                            CreateTimestamp = Timestamp.FromDateTime(metric.CreateTimestamp),
                            DomainId = _options.CurrentValue.LogDomainId.ToString("D"),
                            EventCode = metric.EventCode,
                            EventId = entry.EventId.Id != default(int) && !string.IsNullOrEmpty(entry.EventId.Name) ? new LogRPC.Protos.EventId { Id = entry.EventId.Id, Name = entry.EventId.Name } : null,
                            Level = entry.LogLevel.ToString(),
                            Magnitude = metric.Magnitude,
                            Requestor = metric.Requestor ?? string.Empty,
                            Status = metric.Status
                        };
                        MapMetricData(request.Data, metric.Data);
                        await call.RequestStream.WriteAsync(request);
                    }
                    await call.RequestStream.CompleteAsync();
                    _ = await call;
                }
            }
        }

        private static void MapMetricData(MapField<string, string> map, Dictionary<string, string> sourceData)
        {
            if (sourceData != null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in sourceData)
                {
                    map.Add(keyValuePair.Key, keyValuePair.Value);
                }
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
                        _ = _outputThread.Join(60000);
                    }
                    catch (ThreadStateException)
                    {
                        // do nothing
                    }
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
