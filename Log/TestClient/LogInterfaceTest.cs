using BrassLoon.Interface.Log;
using BrassLoon.Interface.Log.Models;
using BrassLoon.Log.TestClient.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Log.TestClient
{
    public class LogInterfaceTest
    {
        private readonly AppSettings _appSettings;
        private readonly IMetricService _metricService;
        private readonly ITraceService _traceService;
        private readonly IExceptionService _exceptionService;
        private readonly ISettingsFactory _settingsFactory;

        public LogInterfaceTest(
            AppSettings appSettings,
            IMetricService metricService,
            ITraceService traceService,
            IExceptionService exceptionService,
            ISettingsFactory settingsFactory)
        {
            _appSettings = appSettings;
            _metricService = metricService;
            _traceService = traceService;
            _exceptionService = exceptionService;
            _settingsFactory = settingsFactory;
        }

        public async Task GenerateEntries()
        {
            DateTime start = DateTime.UtcNow;            
            LogSettings logSettings = _settingsFactory.CreateLog();
            Queue<Task<Metric>> queue = new Queue<Task<Metric>>();
            Console.WriteLine($"Entry generation started at {start:HH:mm:ss}");
            await _traceService.Create(logSettings, _appSettings.DomainId, "bl-t-client-gen", $"Entry generation started at {start:HH:mm:ss}", new { Date = DateTime.UtcNow });
            for (int i = 0; i < _appSettings.EntryCount; i += 1)
            {
                while (queue.Count >= _appSettings.ConcurentTaskCount)
                {
                    try
                    {
                        await queue.Dequeue();
                    }
                    catch (System.Exception ex)
                    {
                        await _exceptionService.Create(_settingsFactory.CreateLog(), _appSettings.DomainId, ex);
                    }
                }
                queue.Enqueue(_metricService.Create(logSettings, _appSettings.DomainId, DateTime.UtcNow, "bl-t-client-gen", DateTime.UtcNow.Subtract(start).TotalSeconds, data: null));
            }
            await Task.WhenAll(queue);
            DateTime end = DateTime.UtcNow;
            Console.WriteLine($"Entry generation ended at {end:HH:mm:ss} and took {end.Subtract(start).TotalMinutes:0.0##} minutes");
            await _traceService.Create(logSettings, _appSettings.DomainId, "bl-t-client-gen", $"Entry generation ended at {end:HH:mm:ss} and took {end.Subtract(start).TotalMinutes:0.0##} minutes");
            Console.WriteLine($"{_appSettings.EntryCount} records created");
            await _traceService.Create(logSettings, _appSettings.DomainId, "bl-t-client-gen", $"{_appSettings.EntryCount} records created");
            double rate = Math.Round((double)_appSettings.EntryCount / end.Subtract(start).TotalSeconds, 3, MidpointRounding.ToEven);
            Console.WriteLine($"at {rate} records per second");
            await _traceService.Create(logSettings, _appSettings.DomainId, "bl-t-client-gen", $"at {rate} records per second");
        }
    }
}
