using BrassLoon.Extensions.Logging;
using BrassLoon.Log.TestClient.Settings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Log.TestClient
{
    public class LoggerExtensionTest
    {
        private readonly AppSettings _appSettings;

        public LoggerExtensionTest(AppSettings settings)
        {
            _appSettings = settings;
        }

        public async Task Generate()
        {
            DateTime start = DateTime.Now;
            Console.WriteLine($"start    {start:hh:mm:ss tt}");
            using (ILoggerFactory loggerFactory = LoadLogger(_appSettings))
            {
                ILogger logger = loggerFactory.CreateLogger("LoggingTest");
                try
                {
                    RaiseException();
                }
                catch (Exception ex)
                {
                    logger.LogError(new EventId(2, "test error event"), ex, "alt error message");
                }
                Queue<Task> tasks = new Queue<Task>();
                foreach (int i in Enumerable.Range(0, 25))
                {
                    tasks.Enqueue(Task.Run(() => logger.LogMetric(new EventId(3, "test metric"), new Metric() { EventCode = "LoggingTest", Magnitude = 4.3, Status = "107" })));
                    while (tasks.Count > 250)
                    {
                        await tasks.Dequeue();
                    }
                }
                await Task.WhenAll(tasks);
            }
            DateTime finish = DateTime.Now;
            TimeSpan duration = finish.Subtract(start);
            Console.WriteLine($"finish   {finish:hh:mm:ss tt}");
            Console.WriteLine($"duration {Math.Round(duration.TotalMinutes, 3)} minute");
        }

        private static ILoggerFactory LoadLogger(AppSettings settings)
        {
            return LoggerFactory.Create(builder =>
            {
                _ = builder.AddBrassLoonLogger((config) =>
                {
                    config.LogApiBaseAddress = settings.LogAPIBaseAddress;
                    config.LogDomainId = settings.DomainId;
                    config.LogClientId = settings.ClientId;
                    config.LogClientSecret = settings.Secret;
                });
            });
        }

        private static void RaiseException() => throw new ApplicationException("test exception");
    }
}
