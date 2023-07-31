using Autofac;
using BrassLoon.Account.TestClient.Settings;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.CommandLine;
using System.Globalization;
using System.Threading.Tasks;

namespace BrassLoon.Account.TestClient
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
                Option<bool> performanceTestCreateTask = new Option<bool>(
                   name: "--create-client-token-test",
                   getDefaultValue: () => true);
                RootCommand rootCommand = new RootCommand
                {
                    performanceTestCreateTask
                };
                rootCommand.SetHandler(
                    createTasks => CreateClientTokenTest(),
                    performanceTestCreateTask);
                await rootCommand.InvokeAsync(args);
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

        private static async Task CreateClientTokenTest()
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope())
            {
                CreateClientTokenTest test = scope.Resolve<CreateClientTokenTest>();
                await test.Execute();
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
            builder.AddJsonFile("appSettings.json", false)
                .AddEnvironmentVariables();
            return builder.Build();
        }

        private static ILogger CreateLogger(string fileName)
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