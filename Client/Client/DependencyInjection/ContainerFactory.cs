using Autofac;

namespace BrassLoon.Client.DependencyInjection
{
    internal class ContainerFactory
    {
        private static readonly IContainer _container;

        static ContainerFactory()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new ClientModule());
            _container = builder.Build();
        }

        public static IContainer Container => _container;

        public static ILifetimeScope BeginLifetimeScope() => Container.BeginLifetimeScope();
    }
}
