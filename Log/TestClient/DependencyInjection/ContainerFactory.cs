using Autofac;
using BrassLoon.Log.TestClient.Settings;

namespace BrassLoon.Log.TestClient.DependencyInjection
{
    public static class ContainerFactory
    {
        private static IContainer _container;

        public static IContainer Container => _container;

        public static void Initialize(
            AppSettings appSettings = null)
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new TestClientModule());
            if (appSettings != null)
                builder.RegisterInstance(appSettings);
            _container = builder.Build();
        }

        public static ILifetimeScope BeginLifeTimescope() => _container.BeginLifetimeScope();
    }
}
