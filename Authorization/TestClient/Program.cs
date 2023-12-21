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
                RootCommand rootCommand = new RootCommand("Authorization Test Harness");

                Command command;

                command = new Command("create_token");
                command.SetHandler(
                    () => CreateTokenTest());
                rootCommand.AddCommand(command);

                command = new Command("signing_key");
                command.SetHandler(
                    () => CreateSigningKeyTest());
                rootCommand.AddCommand(command);

                command = new Command("cient");
                command.SetHandler(
                    () => ClientTest());
                rootCommand.AddCommand(command);

                command = new Command("role");
                command.SetHandler(
                    () => RoleTest());
                rootCommand.AddCommand(command);

                command = new Command("user");
                command.SetHandler(
                    () => UserTest());
                rootCommand.AddCommand(command);

                _ = await rootCommand.InvokeAsync(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task UserTest()
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope();
            UserTest test = scope.Resolve<UserTest>();
            await test.Execute();
        }

        private static async Task RoleTest()
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope();
            RoleTest test = scope.Resolve<RoleTest>();
            await test.Execute();
        }

        private static async Task ClientTest()
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope();
            ClientTest test = scope.Resolve<ClientTest>();
            await test.Execute();
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
            _ = builder
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