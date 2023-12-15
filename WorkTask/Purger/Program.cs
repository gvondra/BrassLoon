using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Purger
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                AppSettings appSettings = BindConfiguration(GetConfiguration(args));
                DependencyInjection.ContainerFactory.Initialize(appSettings);
                await StartPurge();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        private static async Task StartPurge()
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            ILogger logger = scope.Resolve<Func<string, ILogger>>()("Program");
            try
            {
                logger.LogInformation("Started Work Task Purge");
                PurgeProcessor purgeProcessor = scope.Resolve<PurgeProcessor>();
                await purgeProcessor.InitializeWorkers();
                await purgeProcessor.StartPurge();
                await purgeProcessor.PugeMetaData();
                logger.LogInformation("Completed Work Task Purge");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        private static AppSettings BindConfiguration(IConfiguration configuration)
        {
            AppSettings settings = new AppSettings();
            ConfigurationBinder.Bind(configuration, settings);
            return settings;
        }

        private static IConfiguration GetConfiguration(string[] args)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            _ = builder.AddJsonFile("appSettings.json", false)
                .AddEnvironmentVariables()
                .AddCommandLine(args);
            return builder.Build();
        }
    }
}