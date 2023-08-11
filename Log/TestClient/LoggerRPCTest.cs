using BrassLoon.Extensions.Logging;
using BrassLoon.Log.TestClient.Settings;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.TestClient
{
    public class LoggerRPCTest
    {
        private readonly AppSettings _appSettings;

        public LoggerRPCTest(AppSettings settings)
        {
            _appSettings = settings;
        }
        public Task Generate()
        {
            DateTime start = DateTime.Now;
            Console.WriteLine($"start    {start:hh:mm:ss tt}");
            using (ILoggerFactory loggerFactory = LoadLogger(_appSettings))
            {
                ILogger logger = loggerFactory.CreateLogger("LoggingTest");
                logger.Log(LogLevel.Information, new EventId(1, "test client"), "test message");
                logger.LogMetric(
                    new EventId(1, "test client"),
                    new Metric
                    {
                        //CreateTimestamp = new DateTime(2023, 1, 1), is not used
                        EventCode = "test code",
                        Magnitude = 1.23,
                        Requestor = "test requestor",
                        Status = "500"
                    }); 
            }
            DateTime finish = DateTime.Now;
            TimeSpan duration = finish.Subtract(start);
            Console.WriteLine($"finish   {finish:hh:mm:ss tt}");
            Console.WriteLine($"duration {Math.Round(duration.TotalMinutes, 3)} minute");
            return Task.CompletedTask;
        }

        private static ILoggerFactory LoadLogger(AppSettings settings)
        {
            return LoggerFactory.Create(builder =>
            {
                builder.AddBrassLoonLogger((config) =>
                {
                    config.AccountApiBaseAddress = settings.AccountAPIBaseAddress;
                    config.LogApiBaseAddress = settings.LogAPIBaseAddress;
                    config.LogDomainId = settings.DomainId;
                    config.LogClientId = settings.ClientId;
                    config.LogClientSecret = settings.Secret;
                });
            });
        }
    }
}
