using BrassLoon.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.LoggingTest
{
    public static class Program
    {
        private static Settings _settings;

        public static async Task<int> Main(string[] args)
        {
            DateTime start = DateTime.Now;
            Console.WriteLine($"start    {start:hh:mm:ss tt}");
            _settings = LoadSettings(args);
            using (ILoggerFactory loggerFactory = LoadLogger(_settings))
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
            return 0;
        }

        private static void RaiseException()
        {
            throw new ApplicationException("test exception");
        }

        private static ILoggerFactory LoadLogger(Settings settings)
        {
            return LoggerFactory.Create(builder => 
            {
                builder.AddBrassLoonLogger((config) =>
                {
                    config.AccountApiBaseAddress = settings.AccountApiBaseAddress;
                    config.LogApiBaseAddress = settings.LogApiBaseAddress;
                    config.LogDomainId = settings.LogDomainId;
                    config.LogClientId = settings.LogClientId;
                    config.LogClientSecret = settings.LogClientSecret;
                });
            });
        }

        private static Settings LoadSettings(string[] args)
        {
            
            Settings settings = new Settings();
            ConfigurationBinder.Bind(GetConfiguration(args), settings);
            return settings;
        }

        private static IConfiguration GetConfiguration(string[] args)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder
            .AddJsonFile("appSettings.json", false)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            ;
            return builder.Build();
        }

    }
}