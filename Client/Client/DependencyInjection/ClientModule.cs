﻿using Autofac;
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
            builder.RegisterModule(new BrassLoon.Interface.Authorization.AuthorizationInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.Config.ConfigInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.WorkTask.WorkTaskInterfaceModule());
            builder.RegisterType<AccountLoader>();
            builder.RegisterType<AccountLockToggler>();
            builder.RegisterType<AccountSaver>();
            builder.RegisterType<AccountsLoader>();
            builder.RegisterType<AccountUserRemover>();
            builder.RegisterInstance(AppSettingsLoader.Load());
            builder.RegisterType<ClientSaver>();
            builder.RegisterType<ClientSecretGenreator>();
            builder.RegisterType<ClientValidator>();
            builder.RegisterType<CreateInvitationValidator>();
            builder.RegisterType<DomainClientAdd>();
            builder.RegisterType<DomainDeleter>();
            builder.RegisterType<DomainLoader>();
            builder.RegisterType<DomainRoleAdd>();
            builder.RegisterType<DomainRoleSaver>();
            builder.RegisterType<DomainSigningKeyAdd>();
            builder.RegisterType<DomainSigningKeySaver>();
            builder.RegisterType<DomainUpdater>();
            builder.RegisterType<DomainUserSearcher>();
            builder.RegisterType<DomainValidator>();
            builder.RegisterType<DomainTaskTypeAdd>();
            builder.RegisterType<DomainTaskTypeLoader>();
            builder.RegisterType<DomainWorkGroupAdd>();
            builder.RegisterType<DomainWorkGroupLoader>();
            builder.RegisterType<ExceptionsLoader>();
            builder.RegisterType<HomeLoader>();
            builder.RegisterType<InvitationCancel>();
            builder.RegisterType<InvitationCreator>();
            builder.RegisterType<MetricsLoader>();
            builder.RegisterType<MoreExceptionsLoader>();
            builder.RegisterType<MoreMetricsLoader>();
            builder.RegisterType<MoreTracesLoader>();
            builder.RegisterType<RoleValidator>();
            builder.RegisterType<SettingsFactory>()
                .SingleInstance()
                .As<ISettingsFactory>();
            builder.RegisterType<TracesLoader>();
            builder.RegisterType<UserRoleSaver>();
            builder.RegisterType<UsersLoader>();
        }
    }
}
