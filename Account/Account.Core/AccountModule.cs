using Autofac;
using BrassLoon.Account.Data;
using BrassLoon.Account.Framework;

namespace BrassLoon.Account.Core
{
    public class AccountModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new AccountDataModule());
            _ = builder.RegisterInstance(new SettingsFactory());
            _ = builder.RegisterType<AccountFactory>().As<IAccountFactory>();
            _ = builder.RegisterType<AccountSaver>().As<IAccountSaver>();
            _ = builder.RegisterType<ClientFactory>().As<IClientFactory>();
            _ = builder.RegisterType<ClientSaver>().As<IClientSaver>();
            _ = builder.RegisterType<DomainFactory>().As<IDomainFactory>();
            _ = builder.RegisterType<DomainSaver>().As<IDomainSaver>();
            _ = builder.RegisterType<EmailAddressFactory>().As<IEmailAddressFactory>();
            _ = builder.RegisterType<EmailAddressSaver>().As<IEmailAddressSaver>();
            _ = builder.RegisterType<KeyVault>().As<IKeyVault>();
            _ = builder.RegisterType<SecretProcessor>()
                .SingleInstance()
                .As<ISecretProcessor>();
            _ = builder.RegisterType<UserFactory>().As<IUserFactory>();
            _ = builder.RegisterType<UserSaver>().As<IUserSaver>();
            _ = builder.RegisterType<UserInvitationFactory>().As<IUserInvitationFactory>();
            _ = builder.RegisterType<UserInvitationSaver>().As<IUserInvitationSaver>();
        }
    }
}
