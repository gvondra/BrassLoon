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
            Console.WriteLine($"start {DateTime.Now.ToString("hh:mm:ss tt")}");
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
                List<Task> tasks = new List<Task>();
                foreach (int i in Enumerable.Range(0, 1000))
                {
                    tasks.Add(Task.Run(() => logger.LogInformation(new EventId(1, "test info log"), $"The current time is {DateTime.Now:hh:mm:ss}")));
                }
                await Task.WhenAll(tasks);
                return 0;
            }
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