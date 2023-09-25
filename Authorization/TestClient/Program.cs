using Autofac;
using Microsoft.Extensions.Configuration;
using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.TestClient
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            AppSettings appSettings = LoadSettings();
            DependencyInjection.ContainerFactory.Initialize(appSettings);
            try
            {
                Option<bool> createToken = new Option<bool>(
                    name: "--create-token",
                    getDefaultValue: () => true
                    );
                Option<bool> createSigningKey = new Option<bool>(
                    name: "--create-signing-key",
                    getDefaultValue: () => true
                    );
                RootCommand rootCommand = new RootCommand
                {
                    createSigningKey,
                    createToken
                };
                rootCommand.SetHandler(
                    (ct) => CreateTokenTest(),
                    createToken);
                rootCommand.SetHandler(
                    (csk) => CreateSigningKeyTest(),
                    createSigningKey);
                await rootCommand.InvokeAsync(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task CreateSigningKeyTest()
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope();
            SigningKeyTest test = scope.Resolve<SigningKeyTest>();
            await test.Execute();
        }

        private static async Task CreateTokenTest()
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope();
            CreateTokenTest test = scope.Resolve<CreateTokenTest>();
            await test.Execute();
        }

        private static AppSettings LoadSettings()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder
            .AddJsonFile("appsettings.json", true)
            .AddEnvironmentVariables()
            ;
            IConfiguration configuration = builder.Build();
            AppSettings settings = new AppSettings();
            ConfigurationBinder.Bind(configuration, settings);
            return settings;
        }
    }
}