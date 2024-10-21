using Autofac;

namespace BrassLoon.Client.DependencyInjection
{
    internal static class ContainerFactory
    {
        private static readonly IContainer _container = InitializeContainer();

        public static IContainer Container => _container;

        public static ILifetimeScope BeginLifetimeScope() => Container.BeginLifetimeScope();

        private static IContainer InitializeContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            _ = builder.RegisterModule(new ClientModule());
            return builder.Build();
        }
    }
}
