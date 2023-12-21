using Autofac;

namespace BrassLoon.Authorization.TestClient.DependencyInjection
{
    public static class ContainerFactory
    {
        private static IContainer _container;

        public static IContainer Container => _container;

        public static void Initialize(
            AppSettings appSettings = null)
        {
            ContainerBuilder builder = new ContainerBuilder();
            _ = builder.RegisterModule(new TestClientModule());
            if (appSettings != null)
                _ = builder.RegisterInstance(appSettings);
            _container = builder.Build();
        }

        public static ILifetimeScope BeginLifeTimescope() => _container.BeginLifetimeScope();
    }
}
