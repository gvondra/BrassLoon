using Autofac;
using BrassLoon.Config.TestClient.Settings;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using System;
using System.CommandLine;
using System.Globalization;
using System.Threading.Tasks;

namespace BrassLoon.Config.TestClient
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            AppSettings appSettings = BindConfiguration(GetConfiguration());
            DependencyInjection.ContainerFactory.Initialize(
                appSettings: appSettings,
                logger: CreateLogger(appSettings.LogFile));
            try
            {
                Option<bool> lookupTest = new Option<bool>(
                   name: "--lookup",
                   getDefaultValue: () => false);
                Option<bool> itemTest = new Option<bool>(
                   name: "--item",
                   getDefaultValue: () => false);
                RootCommand rootCommand = new RootCommand
                {
                    lookupTest,
                    itemTest
                };
                rootCommand.SetHandler(
                    async (runLookup, runItem) =>
                    {
                        if (runLookup)
                            await LookupTest();
                        if (runItem)
                            await ItemTest();
                    },
                    lookupTest,
                    itemTest);
                _ = await rootCommand.InvokeAsync(args);
            }
            catch (Exception ex)
            {
                using (ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope())
                {
                    ILogger logger = scope.Resolve<ILogger>();
                    logger.Error(ex, ex.Message);
                }
            }
        }

        private static async Task ItemTest()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope())
            {
                ItemTest itemTest = scope.Resolve<ItemTest>();
                await itemTest.Execute();
            }
        }

        private static async Task LookupTest()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope())
            {
                LookupTest lookupTest = scope.Resolve<LookupTest>();
                await lookupTest.Execute();
            }
        }

        private static AppSettings BindConfiguration(IConfiguration configuration)
        {
            AppSettings settings = new AppSettings();
            ConfigurationBinder.Bind(configuration, settings);
            return settings;
        }

        private static IConfiguration GetConfiguration()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            _ = builder.AddJsonFile("appSettings.json", false)
                .AddEnvironmentVariables();
            return builder.Build();
        }

        private static Logger CreateLogger(string fileName)
        {
            LoggerConfiguration configuration = new LoggerConfiguration()
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(fileName))
            {
                configuration = configuration.WriteTo.File(
                    fileName,
                    formatProvider: CultureInfo.InvariantCulture);
            }
            return configuration.CreateLogger();
        }
    }
}