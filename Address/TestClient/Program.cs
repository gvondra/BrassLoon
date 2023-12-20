using Autofac;
using System.CommandLine;

namespace BrassLoon.Address.TestClient
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                DependencyInjection.ContainerFactory.Initialize();
                RootCommand rootCommand = new RootCommand("Address Test Harness");

                Command command;

                command = new Command("address");
                command.SetHandler(AddressTest);
                rootCommand.AddCommand(command);

                command = new Command("email-address");
                command.SetHandler(EmailAddressTest);
                rootCommand.AddCommand(command);

                _ = await rootCommand.InvokeAsync(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task AddressTest()
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope();
            AddressTest addressTest = scope.Resolve<AddressTest>();
            await addressTest.Execute();
        }

        private static async Task EmailAddressTest()
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope();
            EmailAddressTest emailAddressTest = scope.Resolve<EmailAddressTest>();
            await emailAddressTest.Execute();
        }
    }
}
