using Autofac;
using BrassLoon.Client.Behaviors;
using BrassLoon.Client.Settings;

namespace BrassLoon.Client.DependencyInjection
{
    internal class ClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterType<AccountSaver>();
            builder.RegisterType<AccountsLoader>();
            builder.RegisterInstance(AppSettingsLoader.Load());
            builder.RegisterType<HomeLoader>();
            builder.RegisterType<SettingsFactory>()
                .SingleInstance()
                .As<ISettingsFactory>();
            builder.RegisterType<UserRoleSaver>();
            builder.RegisterType<UsersLoader>();
        }
    }
}
