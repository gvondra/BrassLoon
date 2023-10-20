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
            builder.RegisterType<AccountLoader>();
            builder.RegisterType<AccountLockToggler>();
            builder.RegisterType<AccountSaver>();
            builder.RegisterType<AccountsLoader>();
            builder.RegisterType<AccountUserRemover>();
            builder.RegisterInstance(AppSettingsLoader.Load());
            builder.RegisterType<ClientSaver>();
            builder.RegisterType<ClientSecretGenreator>();
            builder.RegisterType<ClientValidator>();
            builder.RegisterType<HomeLoader>();
            builder.RegisterType<SettingsFactory>()
                .SingleInstance()
                .As<ISettingsFactory>();
            builder.RegisterType<CreateInvitationValidator>();
            builder.RegisterType<InvitationCancel>();
            builder.RegisterType<InvitationCreator>();
            builder.RegisterType<UserRoleSaver>();
            builder.RegisterType<UsersLoader>();
        }
    }
}
