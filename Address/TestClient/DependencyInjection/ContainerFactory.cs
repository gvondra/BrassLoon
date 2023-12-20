using Autofac;
using Microsoft.Extensions.Configuration;

namespace BrassLoon.Address.TestClient.DependencyInjection
{
    public static class ContainerFactory
    {
        private static IContainer _container;

        public static IContainer Container => _container;

        public static void Initialize()
        {
            ContainerBuilder builder = new ContainerBuilder();
            _ = builder.RegisterModule(new TestClientModule());
            _ = builder.RegisterInstance(
                BindAppSettings(LoadConfiguration()));
            _container = builder.Build();
        }

        public static ILifetimeScope BeginLifeTimescope() => _container.BeginLifetimeScope();

        private static IConfiguration LoadConfiguration()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            _ = builder
            .AddJsonFile("appsettings.json", true)
            .AddEnvironmentVariables()
            ;
            return builder.Build();
        }

        private static AppSettings BindAppSettings(IConfiguration configuration)
        {
            AppSettings settings = new AppSettings();
            ConfigurationBinder.Bind(configuration, settings);
            return settings;
        }
    }
}
