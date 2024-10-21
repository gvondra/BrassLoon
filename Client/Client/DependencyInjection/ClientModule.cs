using Autofac;
using BrassLoon.Client.Behaviors;
using BrassLoon.Client.Settings;

namespace BrassLoon.Client.DependencyInjection
{
    internal sealed class ClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.Authorization.AuthorizationInterfaceModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.Config.ConfigInterfaceModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.WorkTask.WorkTaskInterfaceModule());
            _ = builder.RegisterType<AccountLoader>();
            _ = builder.RegisterType<AccountLockToggler>();
            _ = builder.RegisterType<AccountSaver>();
            _ = builder.RegisterType<AccountsLoader>();
            _ = builder.RegisterType<AccountUserRemover>();
            _ = builder.RegisterInstance(AppSettingsLoader.Load());
            _ = builder.RegisterType<ClientSaver>();
            _ = builder.RegisterType<ClientSecretGenreator>();
            _ = builder.RegisterType<ClientValidator>();
            _ = builder.RegisterType<CreateInvitationValidator>();
            _ = builder.RegisterType<DomainClientAdd>();
            _ = builder.RegisterType<DomainDeleter>();
            _ = builder.RegisterType<DomainLoader>();
            _ = builder.RegisterType<DomainRoleAdd>();
            _ = builder.RegisterType<DomainRoleSaver>();
            _ = builder.RegisterType<DomainSigningKeyAdd>();
            _ = builder.RegisterType<DomainSigningKeySaver>();
            _ = builder.RegisterType<DomainUpdater>();
            _ = builder.RegisterType<DomainUserSearcher>();
            _ = builder.RegisterType<DomainValidator>();
            _ = builder.RegisterType<DomainTaskTypeAdd>();
            _ = builder.RegisterType<DomainTaskTypeLoader>();
            _ = builder.RegisterType<DomainWorkGroupAdd>();
            _ = builder.RegisterType<DomainWorkGroupLoader>();
            _ = builder.RegisterType<ExceptionsLoader>();
            _ = builder.RegisterType<HomeLoader>();
            _ = builder.RegisterType<InvitationCancel>();
            _ = builder.RegisterType<InvitationCreator>();
            _ = builder.RegisterType<MetricsLoader>();
            _ = builder.RegisterType<MoreExceptionsLoader>();
            _ = builder.RegisterType<MoreMetricsLoader>();
            _ = builder.RegisterType<MoreTracesLoader>();
            _ = builder.RegisterType<RoleValidator>();
            _ = builder.RegisterType<SettingsFactory>()
                .SingleInstance()
                .As<ISettingsFactory>();
            _ = builder.RegisterType<TracesLoader>();
            _ = builder.RegisterType<UserRoleSaver>();
            _ = builder.RegisterType<UsersLoader>();
        }
    }
}
