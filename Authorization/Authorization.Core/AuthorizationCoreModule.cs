using Autofac;
using BrassLoon.Authorization.Framework;
using BrassLoon.CommonCore;

namespace BrassLoon.Authorization.Core
{
    public class AuthorizationCoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new Data.AuthorizationDataModule());
            _ = builder.RegisterType<KeyVault>().As<IKeyVault>().SingleInstance();
            _ = builder.RegisterType<ClientFactory>().As<IClientFactory>();
            _ = builder.RegisterType<ClientSaver>().As<IClientSaver>();
            _ = builder.RegisterType<EmailAddressFactory>().As<IEmailAddressFactory>();
            _ = builder.RegisterType<RoleFactory>().As<IRoleFactory>();
            _ = builder.RegisterType<RoleSaver>().As<IRoleSaver>();
            _ = builder.RegisterType<SecretGenerator>().As<ISecretGenerator>();
            _ = builder.RegisterType<SigningKeyFactory>().As<ISigningKeyFactory>();
            _ = builder.RegisterType<SigningKeySaver>().As<ISigningKeySaver>();
            _ = builder.RegisterType<TokenClaimGenerator>().As<ITokenClaimGenerator>();
            _ = builder.RegisterType<UserFactory>().As<IUserFactory>();
            _ = builder.RegisterType<UserSaver>().As<IUserSaver>();
        }
    }
}
