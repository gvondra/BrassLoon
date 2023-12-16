using Autofac;
using BrassLoon.Authorization.Data.Framework;
using BrassLoon.DataClient;

namespace BrassLoon.Authorization.Data
{
    public class AuthorizationDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterType<BrassLoon.DataClient.SqlClientProviderFactory>().As<IDbProviderFactory>();
            _ = builder.RegisterType<ClientDataFactory>().As<IClientDataFactory>();
            _ = builder.RegisterType<ClientDataSaver>().As<IClientDataSaver>();
            _ = builder.RegisterType<EmailAddressDataFactory>().As<IEmailAddressDataFactory>();
            _ = builder.RegisterType<EmailAddressDataSaver>().As<IEmailAddressDataSaver>();
            _ = builder.RegisterType<RoleDataFactory>().As<IRoleDataFactory>();
            _ = builder.RegisterType<RoleDataSaver>().As<IRoleDataSaver>();
            _ = builder.RegisterType<SigningKeyDataFactory>().As<ISigningKeyDataFactory>();
            _ = builder.RegisterType<SigningKeyDataSaver>().As<ISigningKeyDataSaver>();
            _ = builder.RegisterType<UserDataFactory>().As<IUserDataFactory>();
            _ = builder.RegisterType<UserDataSaver>().As<IUserDataSaver>();
        }
    }
}
