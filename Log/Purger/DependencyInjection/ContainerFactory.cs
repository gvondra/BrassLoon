using Autofac;

namespace BrassLoon.Log.Purger.DependencyInjection
{
    public static class ContainerFactory
    {
        private static IContainer _container;

        public static IContainer Container => _container;

        public static void Initialize(
            AppSettings appSettings = null)
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new PurgerModule());
            if (appSettings != null)
                builder.RegisterInstance(appSettings);
            _container = builder.Build();
        }

        public static ILifetimeScope BeginLifetimeScope() => _container.BeginLifetimeScope();
    }
}
