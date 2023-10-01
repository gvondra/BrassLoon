using Autofac;
using BrassLoon.WorkTask.TestClient.Settings;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.CommandLine;
using System.Globalization;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.TestClient
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
                RootCommand rootCommand = new RootCommand();

                Command command;

                command = new Command("perf-create-tasks");
                command.SetHandler(
                    () => PerformanceTestCreateTask());
                rootCommand.AddCommand(command);

                command = new Command("task-type");
                command.SetHandler(
                    () => WorkTaskTypeTest());
                rootCommand.AddCommand(command);

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

        private static async Task WorkTaskTypeTest()
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope();
            WorkTaskTypeTest test = scope.Resolve<WorkTaskTypeTest>();
            await test.Execute();
        }

        private static async Task PerformanceTestCreateTask()
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope();
            WorkTaskPerformanceTest test = scope.Resolve<WorkTaskPerformanceTest>();
            await test.Execute();
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