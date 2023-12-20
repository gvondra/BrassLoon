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
                command.SetHandler(SaveTest);
                rootCommand.AddCommand(command);

                _ = await rootCommand.InvokeAsync(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task SaveTest()
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifeTimescope();
            AddressTest addressTest = scope.Resolve<AddressTest>();
            await addressTest.Execute();
        }
    }
}
