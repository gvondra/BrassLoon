using Autofac;
using BrassLoon.Log.TestClient.Settings;
using Microsoft.Extensions.Configuration;
using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace BrassLoon.Log.TestClient
{
    public sealed class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                AppSettings appSettings = LoadSettings();
                if (ValidateArgs(appSettings))
                {
                    DependencyInjection.ContainerFactory.Initialize(appSettings);
                    Option<bool> logInterfaceTest = new Option<bool>(
                       name: "--log-interface-test",
                       getDefaultValue: () => true);
                    Option<bool> loggerExtensionTest = new Option<bool>(
                       name: "--log-extension-test",
                       getDefaultValue: () => true);
                    Option<bool> loggerRpcTest = new Option<bool>(
                       name: "--log-rpc-test",
                       getDefaultValue: () => true);
                    RootCommand rootCommand = new RootCommand
                    {
                        logInterfaceTest,
                        loggerExtensionTest,
                        loggerRpcTest
                    };
                    rootCommand.SetHandler(
                        logInterface => GenerateInterfaceEntries(),
                        logInterfaceTest);
                    rootCommand.SetHandler(
                        logExtension => GenerateExtensionEntries(),
                        loggerExtensionTest);
                    rootCommand.SetHandler(
                        logExtension => GenerateRPCEntries(),
                        loggerRpcTest);
                    await rootCommand.InvokeAsync(args);
                }
            }
            catch (System.Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        private static async Task GenerateExtensionEntries()
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope())
            {
                LoggerExtensionTest test = scope.Resolve<LoggerExtensionTest>();
                await test.Generate();
            }
        }

        private static async Task GenerateRPCEntries()
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope())
            {
                LoggerRPCTest test = scope.Resolve<LoggerRPCTest>();
                await test.Generate();
            }
        }

        private static Task GenerateInterfaceEntries()
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope())
            {
                LogInterfaceTest test = scope.Resolve<LogInterfaceTest>();
                return test.GenerateEntries();
            }
        }

        private static AppSettings LoadSettings()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder
            .AddJsonFile("appSettings.json", false)
            .AddEnvironmentVariables()
            ;
            IConfiguration configuration = builder.Build();
            AppSettings settings = new AppSettings();
            ConfigurationBinder.Bind(configuration, settings);
            return settings;
        }

        private static bool ValidateArgs(AppSettings appSettings)
        {
            bool result = true;
            if (appSettings.DomainId.Equals(Guid.Empty))
            {
                result = false;
                Console.Error.WriteLine("Missing or invalid domain id");
            }
            if (appSettings.ClientId.Equals(Guid.Empty))
            {
                result = false;
                Console.Error.WriteLine("Missing or invalid client id");
            }
            if (string.IsNullOrEmpty(appSettings.Secret))
            {
                result = false;
                Console.Error.WriteLine("Mssing client secret");
            }
            return result;
        }

    }
}
