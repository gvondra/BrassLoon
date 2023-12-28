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
                    PerformanceTestCreateTask);
                rootCommand.AddCommand(command);

                command = new Command("task-type");
                command.SetHandler(
                    WorkTaskTypeTest);
                rootCommand.AddCommand(command);

                command = new Command("work-group");
                command.SetHandler(
                    WorkGroupTest);
                rootCommand.AddCommand(command);

                command = new Command("task");
                command.SetHandler(
                    WorkTaskTest);
                rootCommand.AddCommand(command);

                command = new Command("claim-work-task-debug");
                command.SetHandler(
                    ClaimWorkTaskDebug);
                rootCommand.AddCommand(command);

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

        private static async Task ClaimWorkTaskDebug()
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope();
            ClaimWorkTaskDebug test = scope.Resolve<ClaimWorkTaskDebug>();
            await test.Execute();
        }

        private static async Task WorkTaskTest()
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope();
            WorkTaskTest test = scope.Resolve<WorkTaskTest>();
            await test.Execute();
        }

        private static async Task WorkGroupTest()
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope();
            WorkGroupTest test = scope.Resolve<WorkGroupTest>();
            await test.Execute();
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
            _ = builder.AddJsonFile("appSettings.json", false)
                .AddEnvironmentVariables();
            return builder.Build();
        }

        private static Serilog.Core.Logger CreateLogger(string fileName)
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