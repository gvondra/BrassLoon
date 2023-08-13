using Autofac;
using BrassLoon.Config.TestClient.Settings;
using Serilog;

namespace BrassLoon.Config.TestClient.DependencyInjection
{
    public static class ContainerFactory
    {
        private static IContainer _container;

        public static IContainer Container => _container;

        public static void Initialize(
            AppSettings appSettings = null,
            ILogger logger = null)
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new TestClientModule());
            if (appSettings != null)
                builder.RegisterInstance(appSettings);
            if (logger != null)
                builder.RegisterInstance(logger);
            _container = builder.Build();
        }

        public static ILifetimeScope BeginLifeTimescope() => _container.BeginLifetimeScope();
    }
}
